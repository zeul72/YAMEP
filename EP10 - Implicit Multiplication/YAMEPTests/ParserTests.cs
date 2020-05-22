using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YAMEP.Tests {


    [TestClass( )]
    public class ParserTests {


        [TestMethod( )]
        public void ParseTest_001( ) {

            var expression = "1 + 2";

            //Expected Tree
            //            BinaryOperator(+)
            //            /               \
            //          Left              Right
            //          /                   \
            //       Number(1)            Number(2)

            var ast = new Parser( new Lexer( new SourceScanner(expression))).Parse() as BinaryOperatorASTNode;

            Assert.IsNotNull( ast );

            Assert.AreEqual( Token.TokenType.Addition, ast.Token.Type );
            Assert.AreEqual( Token.TokenType.Number, ast.Left.Token.Type );
            Assert.AreEqual( Token.TokenType.Number, ast.Right.Token.Type );

            Assert.AreEqual( 1, ( ast.Left as NumberASTNode ).Value );
            Assert.AreEqual( 2, ( ast.Right as NumberASTNode ).Value );

        }

        [TestMethod( )]
        public void ParseTest_002( ) {

            var expression = "1 + 2 * 3";

            //Expected Tree
            //            BinaryOperator(+)
            //            /               \
            //          Left          BinaryOpertor(*)
            //          /               /         \
            //       Number(1)      Number(2)    Number(3)

            var ast = new Parser( new Lexer( new SourceScanner(expression))).Parse() as BinaryOperatorASTNode;

            Assert.IsNotNull( ast );

            Assert.AreEqual( Token.TokenType.Addition, ast.Token.Type );
            Assert.AreEqual( Token.TokenType.Number, ast.Left.Token.Type );

            Assert.AreEqual( Token.TokenType.Multiplication, ast.Right.Token.Type );
            Assert.AreEqual( Token.TokenType.Number, ( ast.Right as BinaryOperatorASTNode ).Left.Token.Type );
            Assert.AreEqual( Token.TokenType.Number, ( ast.Right as BinaryOperatorASTNode ).Left.Token.Type );

            Assert.AreEqual( 1, ( ast.Left as NumberASTNode ).Value );
            Assert.AreEqual( 2, ( ( ast.Right as BinaryOperatorASTNode ).Left as NumberASTNode ).Value );
            Assert.AreEqual( 3, ( ( ast.Right as BinaryOperatorASTNode ).Right as NumberASTNode ).Value );
        }

        [TestMethod( )]
        [ExpectedException( typeof( Exception ) )]
        public void ParseTest_003( ) {

            var expression = "1 +";
            _ = new Parser( new Lexer( new SourceScanner( expression ) ) ).Parse( );

        }


        [TestMethod]
        public void ParseTest_004() {

            var expression = "2 * x * y";
            var variables  = new { x = 5, y = 10 };

            var ee = new ExpressionEngine( );

            var result = ee.Evaluate( expression, variables );

            Assert.AreEqual( 100, result );

        }

        [TestMethod]
        public void ParseTest_005( ) {

            var expression = "x + y";
            var variables  = new { x = 5, y = 10 };

            var ee = new ExpressionEngine( );

            var result = ee.Evaluate( expression, variables );

            Assert.AreEqual( 15, result );

        }
    }
}