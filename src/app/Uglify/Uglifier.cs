using System;

using IronJS.Hosting;

namespace Uglify
{
   /// <summary>
   /// The main Uglify object.
   /// </summary>
   public class Uglifier
   {
      private readonly CSharp.Context context;
      private readonly ResourceHelper resourceHelper;


      /// <summary>
      /// Initializes a new instance of the <see cref="Uglifier"/> class.
      /// </summary>
      public Uglifier()
      {
         this.resourceHelper = new ResourceHelper();
         this.context = SetupContext(this.resourceHelper);
      }


      /// <summary>
      /// Uglifies the specified code.
      /// </summary>
      /// <param name="code">The code.</param>
      /// <returns>
      /// The uglified code.
      /// </returns>
      public string Uglify(string code)
      {
         if (code == null)
            throw new ArgumentNullException("code");

         string uglifyCode = this.resourceHelper.Get("uglify-js.js");
         var x = this.context.Execute(uglifyCode);

         return code;
      }


      /// <summary>
      /// Sets up the context.
      /// </summary>
      /// <param name="resourceHelper">The resource helper.</param>
      /// <returns>
      /// The context.
      /// </returns>
      private static CSharp.Context SetupContext(ResourceHelper resourceHelper)
      {
         var context = new CSharp.Context();
         var requirer = new Requirer(context, resourceHelper);

         context.SetGlobal("require", requirer.Require);

         return context;
      }
   }
}