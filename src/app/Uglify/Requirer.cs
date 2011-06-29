using System;

using IronJS;
using IronJS.Hosting;

namespace Uglify
{
   internal class Requirer
   {
      private readonly CSharp.Context context;
      private readonly ResourceHelper resourceHelper;


      public Requirer(CSharp.Context context, ResourceHelper resourceHelper)
      {
         if (context == null)
            throw new ArgumentNullException("context");

         if (resourceHelper == null)
            throw new ArgumentNullException("resourceHelper");

         this.context = context;
         this.resourceHelper = resourceHelper;
      }


      public CommonObject Require(string file)
      {
         if (String.IsNullOrEmpty(file))
            throw new ArgumentNullException("file");

         file = String.Concat(file, ".js");

         string code = this.resourceHelper.Get(file);

         // Extra semicolon provided after file contents.. just in case
         code = String.Concat("var exports = {}; ", code, "; exports;");

         return this.context.Execute<CommonObject>(code);
      }
   }
}