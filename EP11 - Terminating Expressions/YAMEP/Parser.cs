using System;
using System.Linq;

namespace YAMEP {

    //
    // Implicit Multiplication: 2x x(1+2) 
    /// => 2x^2 => 2(x^2)
    /// 2 * -x => 2-x XXX FAIL XXX
    /// -2x! <= (-1)(2*(x!))
    /// 2x(1+2)^2
    /// <summary>
    ///     Implements the following Production Rules
    ///       EXPRESSION: TERM [('+'|-') TERM]*                    AS
    ///             TERM: FACTOR [('*'|'/') FACTOR]*               MD
    ///             ??
    ///           FACTOR: '-'? IMPLICT_MUL
    ///     IMPLICIT_MUL: EXPONENT IMPLICT_MUL
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
        public ASTNode Parse( ) {
            if( TryParseExpression( out var node ) ) {
                Expect( Token.TokenType.EOE );
                return node;
            } else
                throw new Exception( "Failed to Parse Expression Tree" );
        }


        /// <summary>
        /// Parses the EXPRESSION Production Rule
        /// EXPRESSION: TERM [('+'|-') TERM]*
        /// </summary>
        private bool TryParseExpression( out ASTNode node ) {
            if( TryParseTerm( out node ) ) {
                while( IsNext( TERM_OPERATORS ) ) {
                    var op = Accept();  //accept the operator
                    if( TryParseTerm( out var rhs ) )
                        node = CreateBinaryOperator( op, node, rhs );
                    else
                        throw new Exception( $"Failed to Parse Expression Rule at Position {_lexer.Position}" );
                }
            }
            return node != null;
        }

        /// <summary>
        /// Parses the TERM Production Rule
        /// TERM: FACTOR [('*'|'/') FACTOR]*
        /// </summary>
        private bool TryParseTerm( out ASTNode node ) {
            if( TryParseFactor( out node ) ) {
                while( IsNext( FACTOR_OPERATORS ) ) {
                    var op = Accept();  //accept the operator
                    if( TryParseFactor( out var rhs ) )
                        node = CreateBinaryOperator( op, node, rhs );
                    else
                        throw new Exception( $"Failed to Parse Term Rule at Position {_lexer.Position}" );
                }
            }
            return node != null;
        }

        /// FACTOR: '-'? IMPLICT_MUL
        private bool TryParseFactor( out ASTNode node ) {
            if( IsNext( Token.TokenType.Minus ) ) {
                var op = Accept( );
                if( TryParseImplictMultiplication( out node ) ) {
                    node = new NegationUnaryOperatorASTNode( op, node );
                } else {
                    throw new Exception( $"Exception Parsing the Factor Rule at Position {_lexer.Position}" );
                }
            } else {
                TryParseImplictMultiplication( out node );
            }
            return node != null;
        }

        //IMPLICIT_MUL: EXPONENT IMPLICT_MUL*
        private bool TryParseImplictMultiplication( out ASTNode node ) {
            if( TryParseExponent( out node ) ) {
                if( TryParseImplictMultiplication( out var rhs ) ) {
                    node = CreateBinaryOperator( new Token( Token.TokenType.Multiplication, -1, null ), node, rhs );
                }
            }
            return node != null;
        }

        /// EXPONENT: FACTORIAL_FACTOR [ '^' EXPONENT ]*      Exponents
        private bool TryParseExponent( out ASTNode node ) {
            if( TryParseFactorialFactor( out node ) ) {
                if( IsNext( Token.TokenType.Exponent ) ) {
                    var op = Accept();
                    if( TryParseExponent( out var rhs ) )
                        node = new ExponentBinaryOperatorASTNode( op, node, rhs );
                }
            }

            return node != null;
        }


        /// FACTORIAL_FACTOR: PRIMARY '!'?  
        private bool TryParseFactorialFactor( out ASTNode node ) {
            if( TryParsePrimary( out node ) )
                if( IsNext( Token.TokenType.Factorial ) )
                    node = new FactorialUnaryOperatorASTNode( Accept( ), node );

            return node != null;
        }

        /// PRIMARY: IDENTIFIER | NUMER | SUB_EXPRESSION
        private bool TryParsePrimary( out ASTNode node ) {
            if( !TryParseIdentifier( out node ) )
                if( !TryParseNumber( out node ) )
                    if( !TryParseSubExpression( out node ) )
                        return false;

            return true;
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

        /// SUB_EXPRESSION: '(' EXPRESSION ')'
        private bool TryParseSubExpression( out ASTNode node ) {
            node = null;
            if( IsNext( Token.TokenType.OpenParen ) ) {
                Accept( ); //consume the open paren
                if( TryParseExpression( out node ) ) {
                    Expect( Token.TokenType.CloseParen );
                    Accept( );  //consumes the close paren
                }
            }
            return node != null;
        }


        private Token Accept( ) => _lexer.ReadNext( );

        private void Expect( Token.TokenType tokenType ) {
            if( !IsNext( tokenType ) )
                throw new Exception( $"Unexpected token {_lexer.Peek( )?.Value} at position {_lexer.Position}" );
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
