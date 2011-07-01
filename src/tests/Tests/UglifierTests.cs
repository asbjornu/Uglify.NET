using NUnit.Framework;

namespace Uglify.Tests
{
   [TestFixture]
   public class UglifierTests
   {
      #region Setup/Teardown

      [SetUp]
      public void SetUp()
      {
         this.uglifier = new Uglifier();
      }

      #endregion

      private Uglifier uglifier;


      [Test]
      public void Uglify_ReturnsUglifiedCode()
      {
         // TODO: Find verbose and complicated code to uglify.
         this.uglifier.Uglify("(function(){ var x = 'hello world'; }();");
      }
   }
}