using Microsoft.VisualStudio.TestTools.UnitTesting;
using YAMEP;

namespace YAMEPTests {
    [TestClass]
    public class Factorial_Tests {
        [TestMethod]
        public void Test_001( ) {
            var expression = "5!";

            var evalEngine = new ExpressionEngine();

            Assert.AreEqual( 120, evalEngine.Evaluate( expression ) );
        }


        [TestMethod]
        public void Test_002( ) {
            var expression = "-5!";

            var evalEngine = new ExpressionEngine();

            Assert.AreEqual( -120, evalEngine.Evaluate( expression ) );
        }

        [TestMethod]
        public void Test_003( ) {
            var expression = "(2 + 3)!";

            var evalEngine = new ExpressionEngine();

            Assert.AreEqual( 120, evalEngine.Evaluate( expression ) );
        }

        [TestMethod]
        public void Test_004( ) {
            var expression = "-(2 + 3)!";

            var evalEngine = new ExpressionEngine();

            Assert.AreEqual( -120, evalEngine.Evaluate( expression ) );
        }
    }
}
