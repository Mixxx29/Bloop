namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    { 
        public CompilationUnitSyntax(ExpressionSyntax expression, SyntaxToken endOfFileToken)
        {
            Expression = expression;
            EndOfFileToken = endOfFileToken;
        }
        public override SyntaxType Type => SyntaxType.COMPILATION_UNIT;

        public ExpressionSyntax Expression { get; }
        public SyntaxToken EndOfFileToken { get; }

    }
}
