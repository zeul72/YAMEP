namespace YAMEP {

    /// <summary>
    /// base class for all AST Nodes in the tree
    /// </summary>
    public abstract class ASTNode {
        public Token Token { get; protected set; }
        public ASTNode( Token token ) => Token = token;
    }

    /// <summary>
    /// Represents a basic number [0-9]+
    /// </summary>
    public class NumberASTNode : ASTNode {

        /// <summary>
        /// Get the Number value of the Node
        /// </summary>
        public int Value => int.Parse( Token.Value );

        public NumberASTNode( Token token ) : base( token ) { }
    }

    /// <summary>
    /// base class for all operators
    /// </summary>
    public abstract class OperatorASTNode : ASTNode {
        public OperatorASTNode( Token token ) : base( token ) { }
    }

    /// <summary>
    /// for all binary operators=> LEFT OP RIGHT
    /// </summary>
    public abstract class BinaryOperatorASTNode : OperatorASTNode {

        public ASTNode Left { get; protected set; }
        public ASTNode Right { get; protected set; }

        public BinaryOperatorASTNode( Token token , ASTNode left, ASTNode right ) : base( token ) {
            Left = left;
            Right = right;
        }
    }

    public class AdditionBinaryOperatorASTNode : BinaryOperatorASTNode {
        public AdditionBinaryOperatorASTNode( Token token, ASTNode left, ASTNode right) : base( token, left, right ) { }
    }

    public class SubtractionBinaryOperatorASTNode : BinaryOperatorASTNode {
        public SubtractionBinaryOperatorASTNode( Token token, ASTNode left, ASTNode right ) : base( token, left, right ) { }
    }

    public class MultiplicationBinaryOperatorASTNode : BinaryOperatorASTNode {
        public MultiplicationBinaryOperatorASTNode( Token token, ASTNode left, ASTNode right ) : base( token, left, right ) { }
    }

    public class DivisionBinaryOperatorASTNode : BinaryOperatorASTNode {
        public DivisionBinaryOperatorASTNode( Token token, ASTNode left, ASTNode right ) : base( token, left, right ) { }
    }
}
