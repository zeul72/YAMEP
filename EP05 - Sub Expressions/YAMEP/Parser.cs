using System;
using System.Linq;

namespace YAMEP {


    /// <summary>
    /// Implements the following Production Rules
    /// EXPRESSION: TERM [('+'|-') TERM]*
    ///       TERM: FACTOR [('*'|'/') FACTOR]*
    ///     FACTOR: '(' EXPRESSION ')' |  NUMBER
    /// </summary>
    public class Parser {

        static readonly Token.TokenType[] TERM_OPERATORS    = new Token.TokenType[] { Token.TokenType.Addition, Token.TokenType.Subtraction };
        static readonly Token.TokenType[] FACTOR_OPERATORS  = new Token.TokenType[] { Token.TokenType.Multiplication, Token.TokenType.Division };


        readonly Lexer _lexer;

        public Parser( Lexer lexer ) => _lexer = lexer;

        /// <summary>
        /// Parses the supplied expression and returns the root node of the AST 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ASTNode Parse( ) => ParseExpression( );


        /// <summary>
        /// Parses the EXPRESSION Production Rule
        /// EXPRESSION: TERM [('+'|-') TERM]*
        /// </summary>
        private ASTNode ParseExpression( ) {

            var left = ParseTerm();

            while( IsNext( TERM_OPERATORS ) ) {
                var op = Accept();  //accept the operator
                var right = ParseTerm();
                left = CreateBinaryOperator( op, left, right );
            }

            return left;
        }

        /// <summary>
        /// Parses the TERM Production Rule
        /// TERM: FACTOR [('*'|'/') FACTOR]*
        /// </summary>
        private ASTNode ParseTerm( ) {

            var left = ParseFactor();

            while( IsNext( FACTOR_OPERATORS ) ) {
                var op = Accept();  //accept the operator
                var right = ParseFactor();
                left = CreateBinaryOperator( op, left, right );
            }

            return left;
        }

        /// <summary>
        /// Parses the FACTOR Production Rule
        ///   FACTOR: '(' EXPRESSION ')' |  NUMBER
        /// </summary>
        private ASTNode ParseFactor( ) {
            ASTNode node;

            if( IsNext( Token.TokenType.OpenParen ) ) {
                Accept( ); //consume the open paren
                node = ParseExpression( );
                Expect( Token.TokenType.CloseParen );
                Accept( );  //consumes the close paren
            } else {
                node = ParseNumber( );
            }

            return node;
        }

        /// <summary>
        /// Parses the NUMBER Production Rule
        ///   NUMBER: [0-9]+
        /// </summary>
        /// <returns></returns>
        private ASTNode ParseNumber( ) {
            Expect( Token.TokenType.Number );
            return new NumberASTNode( Accept( ) );
        }

        private Token Accept( ) => _lexer.ReadNext( );

        private void Expect( Token.TokenType tokenType ) {
            if( !IsNext( tokenType ) )
                throw new Exception( $"Unexpected token {_lexer.Peek( )} at position {_lexer.Position}" );
        }

        private bool IsNext( params Token.TokenType[ ] possibleTokens )
            => IsNext( x => possibleTokens.Any( k => k == x ) );

        private bool IsNext( Predicate<Token.TokenType> match )
            => match( _lexer.Peek( ).Type );

        private BinaryOperatorASTNode CreateBinaryOperator( Token token, ASTNode left, ASTNode right ) {
            switch( token.Type ) {
                case Token.TokenType.Addition: return new AdditionBinaryOperatorASTNode( token, left, right );
                case Token.TokenType.Subtraction: return new SubtractionBinaryOperatorASTNode( token, left, right );
                case Token.TokenType.Multiplication: return new MultiplicationBinaryOperatorASTNode( token, left, right );
                case Token.TokenType.Division: return new DivisionBinaryOperatorASTNode( token, left, right );
                default:
                    throw new ArgumentOutOfRangeException( nameof( token ) );
            }
        }
    }
}
