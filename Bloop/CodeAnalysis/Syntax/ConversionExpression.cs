using Bloop.CodeAnalysis.Symbol;

namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class ConversionExpression : ExpressionSyntax
    {
        public ConversionExpression(ExpressionSyntax expression, SyntaxToken asKeyword, TypeSymbol targetType)
        {
            Expression = expression;
            AsKeyword = asKeyword;
            TargetType = targetType;
        }

        public override SyntaxType Type => SyntaxType.CONVERSION_EXPRESSION;

        public ExpressionSyntax Expression { get; }
        public SyntaxToken AsKeyword { get; }
        public TypeSymbol TargetType { get; }
    }
}
