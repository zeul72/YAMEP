using System;

namespace YAMEP {

    public class ExpressionEngine {

        /// <summary>
        /// Evaluates an expression and returns the final result
        /// </summary>
        /// <param name="expression">the expression to evalute</param>
        /// <returns>the result</returns>
        public int Evaluate( string expression ) {
            var astRoot = Parser.Parse(expression);
            return Evaluate( astRoot as dynamic );
        }


        protected int Evaluate( AdditionBinaryOperatorASTNode node ) => Evaluate( node.Left as dynamic ) + Evaluate( node.Right as dynamic );
        protected int Evaluate( SubtractionBinaryOperatorASTNode node ) => Evaluate( node.Left as dynamic ) - Evaluate( node.Right as dynamic );
        protected int Evaluate( MultiplicationBinaryOperatorASTNode node ) => Evaluate( node.Left as dynamic ) * Evaluate( node.Right as dynamic );
        protected int Evaluate( DivisionBinaryOperatorASTNode node ) => Evaluate( node.Left as dynamic ) / Evaluate( node.Right as dynamic );
        protected int Evaluate( NumberASTNode node ) => node.Value;
    }
}
