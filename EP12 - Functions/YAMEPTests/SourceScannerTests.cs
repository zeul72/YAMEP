using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YAMEP.Tests {

    [TestClass( )]
    public class SourceScannerTests : UnitTestBase {


        [TestMethod( )]
        public void ReadTest( ) {

            var sourceScanner = GetSourceScanner(SimpleExpression);

            Assert.IsFalse( sourceScanner.EndOfSource );
            Assert.AreEqual( 0, sourceScanner.Position );

            foreach( var c in SimpleExpression )
                Assert.AreEqual( c, sourceScanner.Read( ).Value );

            Assert.IsTrue( sourceScanner.EndOfSource );
            Assert.AreEqual( SimpleExpression.Length, sourceScanner.Position );

        }

        [TestMethod( )]
        public void ReadPastTest( ) {

            var sourceScanner = GetSourceScanner(SimpleExpression);

            Assert.IsFalse( sourceScanner.EndOfSource );
            Assert.AreEqual( 0, sourceScanner.Position );

            while( !sourceScanner.EndOfSource )
                sourceScanner.Read( );

            Assert.IsTrue( sourceScanner.EndOfSource );
            Assert.AreEqual( SimpleExpression.Length, sourceScanner.Position );

            Assert.AreEqual( null, sourceScanner.Read( ) );

        }

        [TestMethod( )]
        public void PeekTest( ) {
            var sourceScanner = GetSourceScanner(SimpleExpression);

            Assert.IsFalse( sourceScanner.EndOfSource );
            Assert.AreEqual( 0, sourceScanner.Position );

            Assert.AreEqual( SimpleExpression[ 0 ], sourceScanner.Peek( ).Value );
            Assert.IsFalse( sourceScanner.EndOfSource );
            Assert.AreEqual( 0, sourceScanner.Position );

            Assert.AreEqual( SimpleExpression[ 0 ], sourceScanner.Read( ).Value );

            Assert.IsFalse( sourceScanner.EndOfSource );
            Assert.AreEqual( 1, sourceScanner.Position );
        }


        [TestMethod( )]
        public void PeekPastTest( ) {
            var sourceScanner = GetSourceScanner(SimpleExpression);

            Assert.IsFalse( sourceScanner.EndOfSource );
            Assert.AreEqual( 0, sourceScanner.Position );

            while( !sourceScanner.EndOfSource )
                sourceScanner.Read( );

            Assert.AreEqual( null, sourceScanner.Peek( ) );
        }

        [TestMethod( )]
        public void PushTest( ) {
            var sourceScanner = GetSourceScanner(SimpleExpression);

            Assert.IsFalse( sourceScanner.EndOfSource );
            Assert.AreEqual( 0, sourceScanner.Position );

            sourceScanner.Push( );

            foreach( var c in SimpleExpression )
                Assert.AreEqual( c, sourceScanner.Read( ).Value );


            Assert.IsTrue( sourceScanner.EndOfSource );
            Assert.AreEqual( SimpleExpression.Length, sourceScanner.Position );

            sourceScanner.Pop( );
            Assert.IsFalse( sourceScanner.EndOfSource );
            Assert.AreEqual( 0, sourceScanner.Position );
        }
    }
}