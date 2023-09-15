namespace Bloop.CodeAnalysis
{
    class Evaluator
    {
        private readonly ExpressionNode _node;

        public Evaluator(ExpressionNode node)
        {
            _node = node;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_node);
        }

        private int EvaluateExpression(ExpressionNode node)
        {
            if (node is NumberExpressionNode numberExpressionNode)
                return (int)numberExpressionNode.NumberToken.Value;

            if (node is BinaryExpressionNode binaryExpressionNode)
            {
                var first = EvaluateExpression(binaryExpressionNode.FirstNode);
                var second = EvaluateExpression(binaryExpressionNode.SecondNode);

                if (binaryExpressionNode.OperatorToken.Type == TokenType.PLUS)
                    return first + second;
                else if (binaryExpressionNode.OperatorToken.Type == TokenType.MINUS)
                    return first - second;
                else if (binaryExpressionNode.OperatorToken.Type == TokenType.ASTERIX)
                    return first * second;
                else if (binaryExpressionNode.OperatorToken.Type == TokenType.SLASH)
                    return first / second;

                throw new Exception($"Unexpected bynary operator {binaryExpressionNode.OperatorToken.Type}");
            }

            if (node is ParenthesizedExpressionNode parenthesizedExpressionNode)
                return EvaluateExpression(parenthesizedExpressionNode.ExpressionNode);

            throw new Exception($"Unexpected node {node.Type}");
        }
    }
}
