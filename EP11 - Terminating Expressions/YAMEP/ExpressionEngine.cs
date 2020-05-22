using System;

namespace YAMEP {

    public class ExpressionEngine {

        readonly SymbolTable _symbolTable = new SymbolTable();

        /// <summary>
        /// Evaluates an expression and returns the final result
        /// </summary>
        /// <param name="expression">the expression to evalute</param>
        /// <returns>the result</returns>
        /// 

        public double Evaluate( string expression, object variables ) {
            _symbolTable.AddOrUpdate( variables );
            return Evaluate( expression );

        }
        
        public double Evaluate( string expression ) { 
            var astRoot = new Parser(new Lexer( new SourceScanner(expression)) , _symbolTable).Parse();
            return Evaluate( astRoot as dynamic );
        }


        protected double Evaluate( ExponentBinaryOperatorASTNode node )
            => Math.Pow( Evaluate( node.Left as dynamic ), Evaluate( node.Right as dynamic ) );

        protected double Evaluate( AdditionBinaryOperatorASTNode node ) => Evaluate( node.Left as dynamic ) + Evaluate( node.Right as dynamic );
        protected double Evaluate( SubtractionBinaryOperatorASTNode node ) => Evaluate( node.Left as dynamic ) - Evaluate( node.Right as dynamic );
        protected double Evaluate( MultiplicationBinaryOperatorASTNode node ) => Evaluate( node.Left as dynamic ) * Evaluate( node.Right as dynamic );
        protected double Evaluate( DivisionBinaryOperatorASTNode node ) => Evaluate( node.Left as dynamic ) / Evaluate( node.Right as dynamic );
        protected double Evaluate( NumberASTNode node ) => node.Value;
        protected double Evaluate( NegationUnaryOperatorASTNode node ) => -1 * Evaluate( node.Target as dynamic );
        protected double Evaluate( FactorialUnaryOperatorASTNode node ) {
            int fact( int x ) => x == 1 ? 1 : x * fact( x - 1 );
            var value = ( int )Evaluate( node.Target as dynamic );
            if( value < 0 )
                throw new Exception( "Factorial only supports Positive Integers" );
            return fact( value );
        }

        protected double Evaluate( VariableIdentifierASTNode node ) {
            var entry = _symbolTable.Get(node.Name);
            if( entry == null || entry.Type != SymbolTableEntry.EntryType.Variable ) {
                throw new Exception( $"Error Evaluating Expression.  Variable {node.Name}" );
            }
            return ( entry as VariableSymbolTableEntry ).Value;
        }
    }
}
