namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class MissingExpressionSyntax : ExpressionSyntax
    {
        public MissingExpressionSyntax(SyntaxToken endOfFileToken)
        {
            EndOfFileToken = endOfFileToken;
        }

        public override SyntaxType Type => SyntaxType.MISSING_EXPRESSION;

        public SyntaxToken EndOfFileToken { get; }
    }
}
