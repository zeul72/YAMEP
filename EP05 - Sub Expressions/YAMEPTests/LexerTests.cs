using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YAMEP.Tests {

    [TestClass( )]
    public class LexerTests : UnitTestBase {

        [TestMethod]
        public void Test_FP_001() {
            var expected = "100";
            var lexer = new Lexer( new SourceScanner(expected));
            Assert.AreEqual( expected, lexer.ReadNext( ).Value );
        }
        [TestMethod]
        public void Test_FP_002( ) {
            var expected = "1.5";
            var lexer = new Lexer( new SourceScanner(expected));
            Assert.AreEqual( expected, lexer.ReadNext( ).Value );
        }

        [TestMethod]
        public void Test_FP_003( ) {
            var expected = ".5";
            var lexer = new Lexer( new SourceScanner(expected));
            Assert.AreEqual( expected, lexer.ReadNext( ).Value );
        }

        [TestMethod]
        public void Test_FP_004( ) {
            var expected = "1e5";
            var lexer = new Lexer( new SourceScanner(expected));
            Assert.AreEqual( expected, lexer.ReadNext( ).Value );
        }



        [TestMethod( )]
        public void ReadNextTest_SimpleExpresion( ) {
            var lexer = new Lexer( GetSourceScanner( SimpleExpression ) );

            // simple expression contains the following
            // 1 -> number
            // ' ' space
            // + -> addition
            // ' ' space
            // 2 -> number
            // ' ' space
            // - -> subtraction
            // ' ' space
            // 3 -> number
            // ' ' space
            // * -> multiplication
            // ' ' space
            // 4 -> number
            // ' ' space
            // / -> division
            // ' ' space
            // 5 -> number

            (Token.TokenType, int, string)[] expectedValues = new (Token.TokenType, int, string)[] {
               ( Token.TokenType.Number,0,"1"),
               ( Token.TokenType.Addition,2,"+"),
               ( Token.TokenType.Number,4,"2"),
               ( Token.TokenType.Subtraction,6,"-"),
               ( Token.TokenType.Number,8,"3"),
               ( Token.TokenType.Multiplication,10,"*"),
               ( Token.TokenType.Number,12,"4"),
               ( Token.TokenType.Division,14,"/"),
               ( Token.TokenType.Number,16,"5"),
               ( Token.TokenType.EOE,17,null),
           };

            foreach( var (t,p,v) in expectedValues) {
                var token = lexer.ReadNext();
                Assert.AreEqual( token.Type, t );
                Assert.AreEqual( token.Position, p );
                Assert.AreEqual( token.Value, v );
            }
        }

        [TestMethod( )]
        public void ReadNextTest_SimpleExpresion_NoSpaces( ) {
            var lexer = new Lexer( GetSourceScanner( SimpleExpression_NoSpaces ) );

            // simple expression contains the following
            // 1 -> number
            // + -> addition
            // 2 -> number
            // - -> subtraction
            // 3 -> number
            // * -> multiplication
            // 4 -> number
            // / -> division
            // 5 -> number

            (Token.TokenType, int, string)[] expectedValues = new (Token.TokenType, int, string)[] {
               ( Token.TokenType.Number,0,"1"),
               ( Token.TokenType.Addition,1,"+"),
               ( Token.TokenType.Number,2,"2"),
               ( Token.TokenType.Subtraction,3,"-"),
               ( Token.TokenType.Number,4,"3"),
               ( Token.TokenType.Multiplication,5,"*"),
               ( Token.TokenType.Number,6,"4"),
               ( Token.TokenType.Division,7,"/"),
               ( Token.TokenType.Number,8,"5"),
               ( Token.TokenType.EOE,9,null),
           };

            foreach( var (t, p, v) in expectedValues ) {
                var token = lexer.ReadNext();
                Assert.AreEqual( token.Type, t );
                Assert.AreEqual( token.Position, p );
                Assert.AreEqual( token.Value, v );
            }
        }
    }
}