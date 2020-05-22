using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YAMEP.Tests {

    [TestClass]
    public class ImplicitMultiplication {

        [TestMethod]
        public void Test_001( ) {

            var expression = "2x";
            var variables  = new { x = 5 };

            var ee = new ExpressionEngine( );

            var result = ee.Evaluate( expression, variables );

            Assert.AreEqual( 10, result );

        }

        [TestMethod]
        public void Test_002( ) {

            var expression = "2x + y";
            var variables  = new { x = 5 , y = 10};

            var ee = new ExpressionEngine( );

            var result = ee.Evaluate( expression, variables );

            Assert.AreEqual( 20, result );

        }
    }
}
