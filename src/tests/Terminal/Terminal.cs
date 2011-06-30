using System;
using System.Reflection;

namespace Uglify.Terminal
{
   internal static class Terminal
   {
      private static void Main()
      {
         try
         {
            Console.WriteLine("Loading..");
            Uglifier uglifier = new Uglifier();
            Console.WriteLine("Uglify executed!");

            uglifier.Uglify("var asdfg = 1234;");
         }
         catch (TargetInvocationException exception)
         {
            Console.WriteLine(exception.InnerException);
         }
         catch (Exception exception)
         {
            Console.WriteLine(exception);
         }

         Console.WriteLine();
         Console.WriteLine();
         Console.WriteLine("Press any key to exit.");
         Console.ReadKey();
      }
   }
}