using System;

using IronJS.Hosting;

namespace Uglify
{
   public class Uglifier
   {
      private readonly CSharp.Context context;
      private readonly ResourceHelper resourceHelper;


      public Uglifier()
      {
         this.resourceHelper = new ResourceHelper();
         this.context = SetupContext(this.resourceHelper);
      }


      public string Uglify(string code)
      {
         if (code == null)
            throw new ArgumentNullException("code");

         string uglifyCode = this.resourceHelper.Get("uglify-js.js");
         var x = this.context.Execute(uglifyCode);

         return code;
      }


      private static CSharp.Context SetupContext(ResourceHelper resourceHelper)
      {
         var context = new CSharp.Context();
         var requirer = new Requirer(context, resourceHelper);

         context.SetGlobal("require", requirer.Require);

         return context;
      }
   }
}