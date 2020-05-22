﻿using System;

namespace YAMEP {
    class Program {
        static void Main( string[ ] args ) {

            var expression = "1 + 2 - 3 * 4 / 5";
            

            var lexer = new Lexer( new SourceScanner( expression ) );

            Console.WriteLine( $"Lexing expression: {expression}" );

            Token token  = null;
            do {
                token = lexer.ReadNext( );
                Console.WriteLine( $"Found Token Type {token.Type} at Position {token.Position} with a value of {token.Value}" );
            } while( token.Type != Token.TokenType.EOE );


            var expressions = new string[] {
                "1 + 2",        //3
                "2 * 3",        //5
                "1 + 2 * 3",    //7
                "1",
                "1 / 2"
            };

            var expressionEngine = new ExpressionEngine();

            foreach( var e in expressions )
                Console.WriteLine( $"{e} = {expressionEngine.Evaluate( e )}" );
        }
    }
}
