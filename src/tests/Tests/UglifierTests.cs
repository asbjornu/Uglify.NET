using NUnit.Framework;

namespace Uglify.Tests
{
   [TestFixture]
   public class UglifierTests
   {
      [Test]
      public void Uglify_Loads()
      {
         new Uglifier();
      }


      [Test]
      public void Uglify_ReturnsUglifiedCode()
      {
         var uglifier = new Uglifier();

         // TODO: Find verbose and complicated code to uglify.
         uglifier.Uglify("(function(){ var x = 'hello world'; }();");
      }
   }
}