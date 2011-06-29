using System;

namespace Uglify.Terminal
{
   internal static class Terminal
   {
      private static void Main()
      {
         try
         {
            Uglifier uglifier = new Uglifier();
            uglifier.Uglify(String.Empty);
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