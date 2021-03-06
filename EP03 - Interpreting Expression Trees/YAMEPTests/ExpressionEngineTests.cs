﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YAMEP.Tests {


    [TestClass( )]
    public class ExpressionEngineTests : UnitTestBase {


        [TestMethod( )]
        public void EvaluateTest_001( ) {
            var evalEngine = new ExpressionEngine();

            (string,int)[] tests = new (string, int)[] {
                 ("1 + 2", 3),
                 ("2 * 2", 4),
                 ("1 - 2", -1),
                 ("2 / 2", 1),
                 ("1 + 2 + 3", 6),
                 ("10 - 9 + 9", 10),
                 ("3 * 4 - 10", 2)
             };

            foreach( var (e, r) in tests )
                Assert.AreEqual( r, evalEngine.Evaluate( e ) );
        }
    }
}