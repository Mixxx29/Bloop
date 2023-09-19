namespace Bloop.CodeAnalysis.Syntax
{
    sealed class BinaryExpressionNode : ExpressionSyntax
    {
        public BinaryExpressionNode(ExpressionSyntax firstNode, SyntaxToken operatorToken, ExpressionSyntax secondNode)
        {
            FirstNode = firstNode;
            OperatorToken = operatorToken;
            SecondNode = secondNode;
        }

        public override SyntaxType Type => SyntaxType.BINARY_EXPRESSION;

        public ExpressionSyntax FirstNode { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax SecondNode { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return FirstNode;
            yield return OperatorToken;
            yield return SecondNode;
        }
    }
}
