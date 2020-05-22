using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YAMEP {
    public class Lexer {

        const char PLUS                 = '+';
        const char MINUS                = '-';
        const char MULTIPLICATION       = '*';
        const char DIVISION             = '/';
        const char DECIMAL_SEPERATOR    = '.';
        const char OPEN_PAREN           = '(';
        const char CLOSE_PAREN          = ')';
        const char FACTORIAL            = '!';
        const char EXPONENT             = '^';

        static readonly char[] E_NOTATION           = new char[] { 'e', 'E' };
        static readonly char[] SIGN_OPERATORS       = new char[] { PLUS, MINUS };

        static readonly Dictionary<char,Func<int,char,Token>> SimpleTokenMap = new Dictionary<char, Func<int, char, Token>> {
            { PLUS, (p,v) => new Token(Token.TokenType.Addition, p,v.ToString()) },
            { MINUS, (p,v) => new Token(Token.TokenType.Minus, p,v.ToString()) },
            { MULTIPLICATION, (p,v) => new Token(Token.TokenType.Multiplication, p,v.ToString()) },
            { DIVISION, (p,v) => new Token(Token.TokenType.Division, p,v.ToString()) },
            { OPEN_PAREN, (p,v) => new Token(Token.TokenType.OpenParen, p,v.ToString()) },
            { CLOSE_PAREN, (p,v) => new Token(Token.TokenType.CloseParen, p,v.ToString()) },
            { FACTORIAL, (p,v) => new Token(Token.TokenType.Factorial, p,v.ToString()) },
            { EXPONENT, (p,v) => new Token(Token.TokenType.Exponent, p,v.ToString()) },
        };

        readonly SourceScanner _scanner;

        /// <summary>
        /// Current Position within the Source Scanner
        /// </summary>
        public int Position => _scanner.Position;

        /// <summary>
        /// Creates an instance of the lexer with the specified <see cref="SourceScanner"/>
        /// </summary>
        /// <param name="scanner">The <see cref="SourceScanner"/> to read from</param>
        public Lexer( SourceScanner scanner ) => _scanner = scanner;

        /// <summary>
        /// Read the next token
        /// </summary>
        /// <returns></returns>
        public Token ReadNext( ) {

            if( _scanner.EndOfSource )
                return new Token( Token.TokenType.EOE, _scanner.Position, null );

            ConsumeWhiteSpace( );

            //Attempt to parse out the supported tokens

            if( TryTokenizeSimpleToken( out Token token ) )
                return token;

            if( TryTokenizeNumber( out token ) )
                return token;

            //not good to be here
            throw new Exception( $"Error parsing expression at {_scanner.Position} value of {_scanner.Peek( )}" );
        }


        /// <summary>
        /// Peek at the next token
        /// </summary>
        /// <returns></returns>
        public Token Peek( ) {
            _scanner.Push( );
            var token = ReadNext();
            _scanner.Pop( );
            return token;
        }

        /// <summary>
        /// Try to lex an operator
        /// </summary>
        /// <param name="token">Token representing the Operator if found</param>
        /// <returns></returns>
        private bool TryTokenizeSimpleToken( out Token token ) {
            token = null;

            if(IsNext(SimpleTokenMap.ContainsKey)) {
                var position = Position;
                var op = Accept();
                token = SimpleTokenMap[ op ]( position, op );
            }

            return token != null;
        }

        /// <summary>
        /// examples 1 100 1.5 .5 1e5 .1e5 1e-5
        ///   \d* .? \d+ ( [eE] [+-]? \d+ )?
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool TryTokenizeNumber( out Token token ) {
            token = null;
            var sb = new StringBuilder();
            var position = Position;    //where did we start finding the digits


            // \d*
            sb.Append( ReadDigits( ) ); ;

            //.?
            if( IsNext( DECIMAL_SEPERATOR ) ) {
                sb.Append( Accept( ) ); //accept the decimal
            }

            //\d +
            sb.Append( ReadDigits( ) );

            //( [eE] [+-]? \d+ )?
            if( sb.Length > 0 && char.IsDigit( sb[ sb.Length - 1 ] ) && IsNext( E_NOTATION ) ) {
                sb.Append( Accept( ) );         //accept the e

                if( IsNext( SIGN_OPERATORS ) ) {    // accept + | -
                    sb.Append( Accept( ) );
                }
                Expect( char.IsDigit );
                sb.Append( ReadDigits( ) );
            }


            if( sb.Length > 0 ) //found something
                token = new Token( Token.TokenType.Number, position, sb.ToString( ) );

            if( token != null && !double.TryParse( token.Value, out _ ) )
                throw new Exception( $"Invalid numeric value {token.Value} found at position {token.Position}" );

            return token != null;
        }

        private string ReadDigits( ) {
            var sb = new StringBuilder();
            while( IsNext( char.IsDigit ) )
                sb.Append( Accept( ) );
            return sb.ToString( );
        }

        private char Accept( ) => _scanner.Read( ).Value;

        private void Expect( Predicate<char> match ) {
            if( !IsNext( match ) )
                throw new Exception( $"Unexpected value at position {Position}" );
        }

        private bool IsNext( params char[ ] possibleValues )
            => IsNext( x => possibleValues.Any( k => k == x ) );

        private bool IsNext( Predicate<char> match ) {
            var lookahead = _scanner.Peek();
            return lookahead.HasValue && match( lookahead.Value );
        }

        /// <summary>
        /// gobble, gobble, gobble
        /// </summary>
        private void ConsumeWhiteSpace( ) {
            while( IsNext( char.IsWhiteSpace ) )
                Accept( );
        }
    }
}
