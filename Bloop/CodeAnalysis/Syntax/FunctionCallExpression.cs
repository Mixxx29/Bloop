namespace Bloop.CodeAnalysis.Syntax
{

    public sealed class FunctionCallExpression : ExpressionSyntax
    {
        public FunctionCallExpression(
            SyntaxToken identifier, 
            SyntaxToken openOpenParenthesis, 
            SeparatedSyntaxList<ExpressionSyntax> arguments,
            SyntaxToken openCloseParenthesis)
        {
            Identifier = identifier;
            OpenOpenParenthesis = openOpenParenthesis;
            Arguments = arguments;
            OpenCloseParenthesis = openCloseParenthesis;
        }

        public override SyntaxType Type => SyntaxType.FUNCTION_CALL_EXPRESSION;

        public SyntaxToken Identifier { get; }
        public SyntaxToken OpenOpenParenthesis { get; }
        public SeparatedSyntaxList<ExpressionSyntax> Arguments { get; }
        public SyntaxToken OpenCloseParenthesis { get; }
    }
}
