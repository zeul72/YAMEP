namespace YAMEP {

    public class Token {

        public enum TokenType {
            EOE,                    // =>  End of Expression Sentinel 
            Number,                 // =>  [0-9]+
            Addition,               // =>  +
            Minus,                  // =>  -
            Multiplication,         // =>  *
            Division,               // =>  /
            OpenParen,              // =>  (
            CloseParen,             // =>  )
            Factorial,              // => !
            Exponent,               // => ^
            Identifier,             // => _?[a-zA-Z]+[a-zA-Z0-9_]+
        }

        /// <summary>
        /// The type of token <see cref="TokenType"/>
        /// </summary>
        public TokenType Type { get; private set; }

        /// <summary>
        /// Position of where the token is found in the source expression
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Textual value of the token
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// ctor for a Token with the specified position and value
        /// </summary>
        /// <param name="type"><see cref="Type"/></param>
        /// <param name="position"><see cref="Position"/></param>
        /// <param name="value"><see cref="Value"/></param>
        public Token( TokenType type, int position, string value ) {
            Type = type;
            Position = position;
            Value = value;
        }
    }
}
