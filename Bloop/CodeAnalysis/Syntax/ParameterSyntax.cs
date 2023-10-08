namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class ParameterSyntax : SyntaxNode
    {
        public ParameterSyntax(SyntaxToken identifier, TypeClauseSyntax typeClause)
        {
            Identifier = identifier;
            TypeClause = typeClause;
        }

        public override SyntaxType Type => SyntaxType.PARAMETER;

        public SyntaxToken Identifier { get; }
        public TypeClauseSyntax TypeClause { get; }
    }
}
