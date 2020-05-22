using System;

namespace YAMEP {

    public class MyFunctions {
        public static double Good_Func_1( double a ) => 5 * a;
        public static double Good_Func_2( double a, double b ) => a + b;
        public static string Not_Me_1( double a ) => $"{a}";
        public static double Not_Me_2( string a ) => a.Length;
        public static double Not_Me_3( string a, double b ) => a.Length + b;
    }

    class Program {
        static void Main( string[ ] args ) {

            //var funcAST = new FunctionASTNode( new Token( Token.TokenType.Identifier, 0, "Good_Func_1"));
            //var arg = new NumberASTNode( new Token(Token.TokenType.Number,0, "5"));
            //funcAST.ArgumentNodes.Add( arg );

            var expEngine = new ExpressionEngine();
            expEngine.AddFunctions<MyFunctions>( );

            var result1 = expEngine.Evaluate("Good_Func_1( 5 )");


            var result2 = expEngine.Evaluate("Good_Func_2( 5, 5 )");

        }
    }
}
