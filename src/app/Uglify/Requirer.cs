using System;

using IronJS;
using IronJS.Hosting;
using IronJS.Native;

namespace Uglify
{
   internal class Requirer
   {
      private readonly CSharp.Context context;
      private readonly FunctionObject require;
      private readonly ResourceHelper resourceHelper;


      public Requirer(CSharp.Context context, ResourceHelper resourceHelper)
      {
         if (context == null)
            throw new ArgumentNullException("context");

         if (resourceHelper == null)
            throw new ArgumentNullException("resourceHelper");

         this.context = context;
         this.resourceHelper = resourceHelper;
         this.require = Utils.createHostFunction<Func<string, CommonObject>>(
            this.context.Environment, RequireInternal);
      }


      public FunctionObject Require
      {
         get { return this.require; }
      }


      private CommonObject RequireInternal(string file)
      {
         if (String.IsNullOrEmpty(file))
            throw new ArgumentNullException("file");

         file = String.Concat(file, ".js");

         string code = this.resourceHelper.Get(file);

         // Extra semicolon provided after file contents.. just in case
         code = String.Concat("var module = { 'exports' : {} }; ", code, ";");

         return this.context.Execute<CommonObject>(code);
      }
   }
}