using System;
using System.Collections.Generic;
using System.Text;

namespace YAMEP {
    public class Lexer {

        static readonly Dictionary<char,Func<int,char,Token>> OperatorMap = new Dictionary<char, Func<int, char, Token>> {
            { '+', (p,v) => new Token(Token.TokenType.Addition, p,v.ToString()) },
            { '-', (p,v) => new Token(Token.TokenType.Subtraction, p,v.ToString()) },
            { '*', (p,v) => new Token(Token.TokenType.Multiplication, p,v.ToString()) },
            { '/', (p,v) => new Token(Token.TokenType.Division, p,v.ToString()) }
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

            if( TryTokenizeOperator( out Token token ) )
                return token;

            if( TryTokenizeNumber( out token ) )
                return token;

            //not good to be here
            throw new Exception( $"Error parsing expression at {_scanner.Position} value of {_scanner.Peek( )}" );
        }

        /// <summary>
        /// Accept the Current Token (consume the token and advance the lexer)
        /// </summary>
        public Token Accept( ) => ReadNext( );

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
        private bool TryTokenizeOperator( out Token token ) {
            token = null;

            var lookahead = _scanner.Peek();

            if( lookahead.HasValue && OperatorMap.ContainsKey( lookahead.Value ) )
                token = OperatorMap[ lookahead.Value ]( _scanner.Position, _scanner.Read( ).Value );

            return token != null;
        }

        /// <summary>
        /// Try to lex a valid digit [0-9]+
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool TryTokenizeNumber( out Token token ) {

            token = null;

            bool isDigit( char? v ) => v.HasValue && char.IsDigit( v.Value );

            if( isDigit( _scanner.Peek( ) ) ) {
                var position = _scanner.Position;
                var sb = new StringBuilder();

                while( isDigit( _scanner.Peek( ) ) ) {
                    sb.Append( _scanner.Read( ).Value );
                }

                token = new Token( Token.TokenType.Number, position, sb.ToString( ) );
            }

            return token != null;
        }

        /// <summary>
        /// gobble, gobble, gobble
        /// </summary>
        private void ConsumeWhiteSpace( ) {

            bool isWhiteSpace( char? v ) => v.HasValue && char.IsWhiteSpace( v.Value );

            while( isWhiteSpace( _scanner.Peek( ) ) )
                _scanner.Read( );   //eat the whitespace
        }
    }
}
