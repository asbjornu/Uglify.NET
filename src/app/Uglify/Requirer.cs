using System;
using System.Collections.Generic;

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
         this.require = Utils.createHostFunction<Func<string, CommonObject>>(
            this.context.Environment, RequireInternal);
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


      private CommonObject RequireAndAddToCache(string file)
      {
         // Check for existence so we can return fast without locking.
         if (this.objectCache.ContainsKey(file))
            return this.objectCache[file];

         // Lock to make thread-safe.
         lock (this.objectCache)
         {
            // Check for existence again after locking.
            if (this.objectCache.ContainsKey(file))
               return this.objectCache[file];

            string fileName = String.Concat(file, ".js");
            string code = this.resourceHelper.Get(fileName);

            code = String.Concat(
               @"// Define a 'substr' alias for 'substring' that parse-js is dependent on.
               String.prototype.substr = String.prototype.substring;

               // Define the exports variable.
               var exports = {};",
               code,
               // End the whole thing with a semicolon, just to be safe.
               ";");

            CommonObject result;

            try
            {
               result = this.context.Execute<CommonObject>(code);
            }
            catch (Exception exception)
            {
               throw new RequireException(file, exception);
            }

            this.objectCache.Add(file, result);
            return result;
         }
      }


      private CommonObject RequireInternal(string file)
      {
         if (String.IsNullOrEmpty(file))
            throw new ArgumentNullException("file");

         file = Normalize(file);

         return RequireAndAddToCache(file);
      }
   }
}