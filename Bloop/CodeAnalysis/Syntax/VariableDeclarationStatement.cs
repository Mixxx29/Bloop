namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class VariableDeclarationStatement : StatementSyntax
    {
        public VariableDeclarationStatement(SyntaxToken keyword, SyntaxToken identifier, TypeClauseSyntax? typeClause, SyntaxToken equal, ExpressionSyntax expression)
        {
            Keyword = keyword;
            Identifier = identifier;
            TypeClause = typeClause;
            Equal = equal;
            Expression = expression;
        }

        public override SyntaxType Type => SyntaxType.VARIABLE_DECLARATION_STATEMENT;

        public SyntaxToken Keyword { get; }
        public SyntaxToken Identifier { get; }
        public TypeClauseSyntax? TypeClause { get; }
        public SyntaxToken Equal { get; }
        public ExpressionSyntax Expression { get; }
    }
}
