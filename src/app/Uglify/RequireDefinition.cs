using System;
using System.Collections.Generic;
using System.Threading;

using IronJS;
using IronJS.Hosting;
using IronJS.Native;

namespace Uglify
{
   /// <summary>
   /// The class that implements node's <c>require()</c> function.
   /// </summary>
   internal class RequireDefinition
   {
      private readonly ReaderWriterLockSlim cacheLock;
      private readonly CSharp.Context context;
      private readonly FunctionObject require;
      private readonly IDictionary<string, CommonObject> requireCache;
      private readonly ResourceHelper resourceHelper;


      /// <summary>
      /// Initializes a new instance of the <see cref="RequireDefinition"/> class.
      /// </summary>
      /// <param name="context">The context.</param>
      /// <param name="resourceHelper">The resource helper.</param>
      public RequireDefinition(CSharp.Context context, ResourceHelper resourceHelper)
      {
         if (context == null)
            throw new ArgumentNullException("context");

         if (resourceHelper == null)
            throw new ArgumentNullException("resourceHelper");

         this.context = context;
         this.resourceHelper = resourceHelper;
         this.requireCache = new Dictionary<string, CommonObject>();
         this.cacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
         this.require = Utils.CreateFunction<Func<string, CommonObject>>(this.context.Environment, 1, Require);
      }


      /// <summary>
      /// Defines the require() function as a global in the current context.
      /// </summary>
      public void Define()
      {
         this.context.SetGlobal("require", this.require);
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
            var requiredResult = this.context.Execute<FunctionObject>("function (exports) {\n" + code + "\n}");

            // Call the required code, passing in the new exports function to be populated.
            requiredResult.Call(this.context.Globals, exports);
         }
         catch (Exception exception)
         {
            throw new RequireException(file, exception);
         }
      }


      private CommonObject Require(string file)
      {
         if (String.IsNullOrEmpty(file))
            throw new ArgumentNullException("file");

         file = Normalize(file);

         CommonObject exports;

         this.cacheLock.EnterReadLock();

         try
         {
            // Check for existence so we can return fast without write-locking.
            if (this.requireCache.TryGetValue(file, out exports))
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
            if (this.requireCache.TryGetValue(file, out exports))
               return exports;

            string fileName = String.Concat(file, ".js");
            string code = this.resourceHelper.Get(fileName);

            // Allocate a new object for the exports of the require, and add it preemptively, to allow for reentrancy.
            exports = new CommonObject(this.context.Environment, this.context.Environment.Prototypes.Object);
            this.requireCache.Add(file, exports);

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