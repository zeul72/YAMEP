using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YAMEP.Tests {


    [TestClass]
    public class Exponent_Tests {

        [TestMethod]
        public void TEST_001( ) {
            var expression = "2^3";
            var expressionEngine = new ExpressionEngine();

            Assert.AreEqual( 8, expressionEngine.Evaluate( expression ) );

        }
        [TestMethod]
        public void TEST_002( ) {
            var expression = "2^(3^2)"; // => 2^9 => 512
            var expressionEngine = new ExpressionEngine();

            Assert.AreEqual( 512, expressionEngine.Evaluate( expression ) );

        }
        [TestMethod]
        public void TEST_003( ) {
            var expression = "(2^3)^2"; // => 8 ^ 2 => 64
            var expressionEngine = new ExpressionEngine();

            Assert.AreEqual( 64, expressionEngine.Evaluate( expression ) );

        }
        [TestMethod]
        public void TEST_004( ) {
            var expression = "2^3^2"; // => 2^(3^2) => 2^9 => 512 should be same value as test #2
            var expressionEngine = new ExpressionEngine();

            Assert.AreEqual( 512, expressionEngine.Evaluate( expression ) );

        }
        [TestMethod]
        public void TEST_005( ) {
            var expression = "2^3!^2"; //=> 2^( (3*2) ^ 2 ) => 2^(6^2) => 2^36
            var expressionEngine = new ExpressionEngine();

            Assert.AreEqual( 68719476736, expressionEngine.Evaluate( expression ) );

        }
        [TestMethod]
        public void TEST_006( ) {
            var expression = "(2^3!)^2";
            var expressionEngine = new ExpressionEngine();

            Assert.AreEqual( 4096, expressionEngine.Evaluate( expression ) );
        }

    }
}
