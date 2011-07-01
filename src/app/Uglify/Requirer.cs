using System;
using System.Collections.Generic;
using System.Threading;

using IronJS;
using IronJS.Hosting;
using IronJS.Native;

namespace Uglify
{
   /// <summary>
   /// The class that implements node's require() function.
   /// </summary>
   internal class Requirer
   {
      private readonly CSharp.Context context;
      private readonly IDictionary<string, CommonObject> objectCache;
      private readonly ReaderWriterLockSlim cacheLock;
      private readonly FunctionObject require;
      private readonly ResourceHelper resourceHelper;


      /// <summary>
      /// Initializes a new instance of the <see cref="Requirer"/> class.
      /// </summary>
      /// <param name="context">The context.</param>
      /// <param name="resourceHelper">The resource helper.</param>
      public Requirer(CSharp.Context context, ResourceHelper resourceHelper)
      {
         if (context == null)
            throw new ArgumentNullException("context");

         if (resourceHelper == null)
            throw new ArgumentNullException("resourceHelper");

         this.context = context;
         this.resourceHelper = resourceHelper;
         this.objectCache = new Dictionary<string, CommonObject>();
         this.cacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
         this.require = Utils.CreateFunction<Func<string, CommonObject>>(this.context.Environment, 1, RequireInternal);
         this.context.SetGlobal("require", this.require);
      }


      /// <summary>
      /// Gets the require() function.
      /// </summary>
      public FunctionObject Require
      {
         get { return this.require; }
      }


      private static string Normalize(string file)
      {
         // We want the file name from the last slash
         int lastSlashIndex = file.LastIndexOf('/');

         if (lastSlashIndex == -1)
            return file;

         // We don't want the slash itself, so increase index with 1.
         lastSlashIndex++;

         return file.Substring(lastSlashIndex, file.Length - lastSlashIndex);
      }


      private void Execute(string file, string code, CommonObject exports)
      {
         try
         {
            // Wrap the required code in its own function.
            var require = this.context.Execute<FunctionObject>("function (exports) {\n" + code + "\n}");

            // Call the required code, passing in the new exports function to be populated.
            require.Call(this.context.Globals, exports);
         }
         catch (Exception exception)
         {
            throw new RequireException(file, exception);
         }
      }


      private CommonObject RequireInternal(string file)
      {
         if (String.IsNullOrEmpty(file))
            throw new ArgumentNullException("file");

         file = Normalize(file);

         CommonObject exports;

         this.cacheLock.EnterReadLock();
         try
         {
            // Check for existence so we can return fast without write-locking.
            if (this.objectCache.TryGetValue(file, out exports))
               return exports;
         }
         finally
         {
            this.cacheLock.ExitReadLock();
         }

         this.cacheLock.EnterWriteLock();
         try
         {
            // Check for existence again after locking.
            if (this.objectCache.TryGetValue(file, out exports))
               return exports;

            string fileName = String.Concat(file, ".js");
            string code = this.resourceHelper.Get(fileName);

            // Allocate a new object for the exports of the require, and add it preemptively, to allow for reentrancy.
            exports = new CommonObject(this.context.Environment, this.context.Environment.Prototypes.Object);
            this.objectCache.Add(file, exports);

            // Populate the exports object.
            Execute(file, code, exports);

            return exports;
         }
         finally
         {
            this.cacheLock.ExitWriteLock();
         }
      }
   }
}