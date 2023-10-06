namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class TypeClauseSyntax : SyntaxNode
    {
        public TypeClauseSyntax(SyntaxToken colonToken, SyntaxToken identifier)
        {
            ColonToken = colonToken;
            Identifier = identifier;
        }

        public override SyntaxType Type => SyntaxType.TYPE_CLAUSE;

        public SyntaxToken ColonToken { get; }
        public SyntaxToken Identifier { get; }
    }
}
