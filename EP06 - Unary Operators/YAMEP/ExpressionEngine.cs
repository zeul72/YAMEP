namespace YAMEP {

    public class ExpressionEngine {

        /// <summary>
        /// Evaluates an expression and returns the final result
        /// </summary>
        /// <param name="expression">the expression to evalute</param>
        /// <returns>the result</returns>
        public double Evaluate( string expression ) {

            var astRoot = new Parser(new Lexer( new SourceScanner(expression))).Parse();
            return Evaluate( astRoot as dynamic );
        }


        protected double Evaluate( AdditionBinaryOperatorASTNode node ) => Evaluate( node.Left as dynamic ) + Evaluate( node.Right as dynamic );
        protected double Evaluate( SubtractionBinaryOperatorASTNode node ) => Evaluate( node.Left as dynamic ) - Evaluate( node.Right as dynamic );
        protected double Evaluate( MultiplicationBinaryOperatorASTNode node ) => Evaluate( node.Left as dynamic ) * Evaluate( node.Right as dynamic );
        protected double Evaluate( DivisionBinaryOperatorASTNode node ) => Evaluate( node.Left as dynamic ) / Evaluate( node.Right as dynamic );
        protected double Evaluate( NumberASTNode node ) => node.Value;
        protected double Evaluate( NegationUnaryOperatorASTNode node ) => -1 * Evaluate( node.Target as dynamic );
        protected double Evaluate( FactorialUnaryOperatorASTNode node ) {
            int fact( int x ) => x == 1 ? 1 : x * fact( x - 1 );
            return fact( ( int )Evaluate( node.Target as dynamic ) );
        }
    }
}
