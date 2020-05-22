using System;
using System.Linq;

namespace YAMEP {


    /// <summary>
    ///     Implements the following Production Rules
    ///       EXPRESSION: TERM [('+'|-') TERM]*                    AS
    ///             TERM: FACTOR [('*'|'/') FACTOR]*               MD
    ///           FACTOR: '-'? EXPONENT
    ///         EXPONENT: FACTORIAL_FACTOR [ '^' EXPONENT ]*      Exponents
    /// FACTORIAL_FACTOR: PRIMARY '!'?                             Factor
    ///          PRIMARY 
    ///                 : IDENTIFIER 
    ///                 | NUMBER 
    ///                 | SUB_EXPRESSION
    ///   SUB_EXPRESSION: '(' EXPRESSION ')'                       Parens
    /// </summary>
    public class Parser {

        static readonly Token.TokenType[] TERM_OPERATORS    = new Token.TokenType[] { Token.TokenType.Addition, Token.TokenType.Minus };
        static readonly Token.TokenType[] FACTOR_OPERATORS  = new Token.TokenType[] { Token.TokenType.Multiplication, Token.TokenType.Division };


        readonly Lexer _lexer;
        readonly SymbolTable _symbolTable;


        public Parser( Lexer lexer ) : this( lexer, new SymbolTable( ) ) { }
        public Parser( Lexer lexer, SymbolTable symbolTable ) {
            _lexer = lexer;
            _symbolTable = symbolTable;
        }

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

        ///   FACTOR: '-'? EXPONENT
        private ASTNode ParseFactor( ) {
            ASTNode node = default;

            if( IsNext( Token.TokenType.Minus ) ) {
                node = new NegationUnaryOperatorASTNode( Accept( ), ParseExponent( ) );
            } else {
                node = ParseExponent( );
            }

            return node;
        }
        /// EXPONENT: FACTORIAL_FACTOR [ '^' EXPONENT ]*      Exponents
        private ASTNode ParseExponent( ) {
            var node = ParseFactorialFactor();
            if( IsNext( Token.TokenType.Exponent ) ) {
                //magic goes here
                var op = Accept();  //accept the exponent operator
                //example 2^3^2
                // left_node(2)
                // right_node( exponent_node( left_node(3), right_node(2) ) );
                //    ^
                //  2   ^
                //    3   2
                node = new ExponentBinaryOperatorASTNode( op, node, ParseExponent( ) );//recursive call for Exponent
            }
            return node;
        }

        /// FACTORIAL_FACTOR: PRIMARY '!'?                             Factor
        /// 5!
        private ASTNode ParseFactorialFactor( ) {
            var node = ParsePrimary();
            if( IsNext( Token.TokenType.Factorial ) ) {
                node = new FactorialUnaryOperatorASTNode( Accept( ), node );
            }
            return node;
        }


        /// PRIMARY: IDENTIFIER | NUMER | SUB_EXPRESSION
        private ASTNode ParsePrimary( ) {
            ASTNode node;

            if( TryParseIdentifier( out node ) )
                return node;

            if( TryParseNumber( out node ) )
                return node;

            if( TryParseSubExpression( out node ) )
                return node;

            //we go boom!
            throw new Exception( $"Invalid Expression exected either Number or ( at {_lexer.Position}" );
        }


        private bool TryParseIdentifier( out ASTNode node ) {
            node = null;

            if( IsNext( Token.TokenType.Identifier ) ) {
                var token = Accept();
                var stEntry = _symbolTable.Get(token.Value);
                if( stEntry == null )
                    throw new Exception( $"Undefined Identifier {token.Value} at position {token.Position}" );

                if( stEntry.Type == SymbolTableEntry.EntryType.Variable )
                    node = new VariableIdentifierASTNode( token, token.Value );
                else {
                    //TODO: Handle Functions next
                }
            }

            return node != null;
        }

        /// <summary>
        /// Parses the NUMBER Production Rule
        ///   NUMBER: [0-9]+
        /// </summary>
        /// <returns></returns>
        private bool TryParseNumber( out ASTNode node ) {
            node = null;
            if( IsNext( Token.TokenType.Number ) ) {
                node = new NumberASTNode( Accept( ) );
            }
            return node != null;
        }

        /// SUB_EXPRESSION: '(' EXPRESSIOn ')'
        private bool TryParseSubExpression( out ASTNode node ) {
            node = null;
            if( IsNext( Token.TokenType.OpenParen ) ) {
                Accept( ); //consume the open paren
                node = ParseExpression( );
                Expect( Token.TokenType.CloseParen );
                Accept( );  //consumes the close paren
            }
            return node != null;
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
                case Token.TokenType.Minus: return new SubtractionBinaryOperatorASTNode( token, left, right );
                case Token.TokenType.Multiplication: return new MultiplicationBinaryOperatorASTNode( token, left, right );
                case Token.TokenType.Division: return new DivisionBinaryOperatorASTNode( token, left, right );
                default:
                    throw new ArgumentOutOfRangeException( nameof( token ) );
            }
        }
    }
}
