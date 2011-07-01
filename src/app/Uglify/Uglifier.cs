using System;

using IronJS;
using IronJS.Hosting;
using IronJS.Native;

namespace Uglify
{
   /// <summary>
   /// The main Uglify object.
   /// </summary>
   public class Uglifier
   {
      private readonly CSharp.Context context;
      private readonly ResourceHelper resourceHelper;
      private readonly FunctionObject uglify;


      /// <summary>
      /// Initializes a new instance of the <see cref="Uglifier"/> class.
      /// </summary>
      public Uglifier()
      {
         this.resourceHelper = new ResourceHelper();
         this.context = SetupContext(this.resourceHelper);
         this.uglify = LoadUglify(this.context, this.resourceHelper);
      }


      /// <summary>
      /// Uglifies the specified code.
      /// </summary>
      /// <param name="code">The JavaScript code that is to be uglified.</param>
      /// <param name="options">The options.</param>
      /// <returns>
      /// The uglified code.
      /// </returns>
      public string Uglify(
         string code,
         // TODO: The options can probably be made into a neat little object, figure out how. [asbjornu]
         string options = "")
      {
         if (code == null)
            throw new ArgumentNullException("code");

         try
         {
            BoxedValue boxedResult = this.uglify.Call(this.context.Globals, code, options);
            return TypeConverter.ToString(boxedResult);
         }
         catch (UserError error)
         {
            throw new UglifyException(code, error);
         }
      }


      private static FunctionObject LoadUglify(CSharp.Context context, ResourceHelper resourceHelper)
      {
         const string defineModule = "var module = {};";

         string uglifyCode = resourceHelper.Get("uglify-js.js");
         context.Execute(String.Concat(defineModule, uglifyCode));

         return context.GetGlobalAs<FunctionObject>("uglify");
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

         var substr = Utils.CreateFunction<Func<FunctionObject, CommonObject, double, double, CommonObject>>(
            context.Environment, 1, Substr);

         context.Environment.Prototypes.String.Put("substr", substr);

         var requirer = new Requirer(context, resourceHelper);

         requirer.Define();

         return context;
      }


      private static CommonObject Substr(
         FunctionObject function, CommonObject stringObject, double startIndex, double length)
      {
         string stringValue = TypeConverter.ToString(stringObject);

         if (!String.IsNullOrEmpty(stringValue))
         {
            // CLI's System.String.Substring should be equivalent to JavaScript's String.prototype.substr
            stringValue = stringValue.Substring(
               // For some reason, IronJS passes integers as doubles, so we need to cast them here
               (int)startIndex, (int)length);
         }

         return TypeConverter.ToObject(function.Env, stringValue);
      }


      private void AstPrinter(string value)
      {
         Console.Write(value);
      }


      private void ExprPrinter(string value)
      {
         Console.Write(value);
      }
   }
}