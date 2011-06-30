using System;

using IronJS;
using IronJS.Hosting;
using IronJS.Support;

namespace Uglify
{
   /// <summary>
   /// The main Uglify object.
   /// </summary>
   public class Uglifier
   {
      private readonly CSharp.Context context;
      private readonly ResourceHelper resourceHelper;
      private FunctionObject uglify;


      /// <summary>
      /// Initializes a new instance of the <see cref="Uglifier"/> class.
      /// </summary>
      public Uglifier()
      {
         this.resourceHelper = new ResourceHelper();
         this.context = SetupContext(this.resourceHelper);
         LoadUglify();
      }


      /// <summary>
      /// Uglifies the specified code.
      /// </summary>
      /// <param name="code">The JavaScript code that is to be uglified.</param>
      /// <returns>
      /// The uglified code.
      /// </returns>
      public string Uglify(string code)
      {
         return Uglify(code, "");
      }


      /// <summary>
      /// Uglifies the specified code.
      /// </summary>
      /// <param name="code">The JavaScript code that is to be uglified.</param>
      /// <returns>
      /// The uglified code.
      /// </returns>
      public string Uglify(string code, string options)
      {
         if (code == null)
            throw new ArgumentNullException("code");

         try
         {
            BoxedValue boxedResult = this.uglify.Call(this.context.Globals, code, options);
            return TypeConverter.ToString(boxedResult);
         }
         catch (Error.Error error)
         {
            Console.WriteLine(error.Message);
         }
         return null;
      }


      private void LoadUglify()
      {
         string uglifyCode = this.resourceHelper.Get("uglify-js.js");

         const string defineModule = "var module = {};";
         this.context.Execute(defineModule + uglifyCode);
         this.uglify = this.context.GetGlobalAs<FunctionObject>("uglify");
      }


      private void ExprPrinter(string value)
      {
         Console.Write(value);
      }


      private void AstPrinter(string value)
      {
         Console.Write(value);
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
         context.CreatePrintFunction();
         var requirer = new Requirer(context, resourceHelper);

         // Debug.registerConsolePrinter();
         // IronJS.Support.Debug.registerAstPrinter(AstPrinter);
         // IronJS.Support.Debug.registerExprPrinter(ExprPrinter);

         context.SetGlobal("require", requirer.Require);
         context.SetGlobal("require", requirer.Require);

         return context;
      }
   }
}