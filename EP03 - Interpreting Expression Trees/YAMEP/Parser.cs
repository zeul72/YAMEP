using System;

namespace YAMEP {


    /// <summary>
    /// Implements the following Production Rules
    /// EXPRESSION: TERM [('+'|-') TERM]*
    ///       TERM: FACTOR [('*'|'/') FACTOR]*
    ///     FACTOR: NUMBER
    ///     NUMBER: [0-9]+
    /// </summary>
    public static class Parser {

        /// <summary>
        /// Parses the supplied expression and returns the root node of the AST 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static ASTNode Parse( string expression ) => ParseExpression( new Lexer( new SourceScanner( expression ) ) );

        /// <summary>
        /// Parses the EXPRESSION Production Rule
        /// EXPRESSION: TERM [('+'|-') TERM]*
        /// </summary>
        private static ASTNode ParseExpression( Lexer lexer ) {

            var left = ParseTerm( lexer );

            var peekToken = lexer.Peek();   //look ahead 1 token

            while( peekToken.Type == Token.TokenType.Addition || peekToken.Type == Token.TokenType.Subtraction ) {

                var op = lexer.ReadNext( );   //read the operator

                var right = ParseTerm(lexer );

                if( right == null )
                    throw new Exception( $"Invalid Expression. TERM Expected at position {lexer.Position}" );

                left = CreateBinaryOperator( op, left, right );

                peekToken = lexer.Peek( );  //lookahead of 1 token LL(k) where K = 1
            }

            return left;
        }

        /// <summary>
        /// Parses the TERM Production Rule
        /// TERM: FACTOR [('*'|'/') FACTOR]*
        /// </summary>
        private static ASTNode ParseTerm( Lexer lexer ) {

            var left = ParseFactor( lexer );

            var peekToken = lexer.Peek();   //look ahead 1 token

            while( peekToken.Type == Token.TokenType.Multiplication || peekToken.Type == Token.TokenType.Division ) {

                var op = lexer.ReadNext( );   //read the operator

                var right = ParseFactor(lexer );

                if( right == null )
                    throw new Exception( $"Invalid Expression. FACTOR Expected at position {lexer.Position}" );

                left = CreateBinaryOperator( op, left, right );

                peekToken = lexer.Peek( );  //lookahead of 1 token LL(k) where K = 1
            }

            return left;
        }

        /// <summary>
        /// Parses the FACTOR Production Rule
        /// FACTOR: NUMBER
        /// </summary>
        private static ASTNode ParseFactor( Lexer lexer ) => ParseNumber( lexer );

        /// <summary>
        /// Parses the NUMBER Production Rule
        ///   NUMBER: [0-9]+
        /// </summary>
        /// <returns></returns>
        private static ASTNode ParseNumber( Lexer lexer ) {

            var token = lexer.Peek();
            if( token.Type != Token.TokenType.Number )
                throw new Exception( $"Invalid Expression.  NUMBER Expected at position {lexer.Position}" );

            lexer.Accept( );  //consume the token

            return new NumberASTNode( token );
        }

        private static BinaryOperatorASTNode CreateBinaryOperator( Token token, ASTNode left, ASTNode right ) {
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
