namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class FunctionDeclarationSyntax : MemberSyntax
    {
        public FunctionDeclarationSyntax(
            SyntaxToken functionKeyword,
            SyntaxToken identifier,
            SyntaxToken openParehtesis,
            SeparatedSyntaxList<ParameterSyntax> parameters,
            SyntaxToken closeParehtesis,
            TypeClauseSyntax? typeClause,
            BlockStatementSyntax body)
        {
            FunctionKeyword = functionKeyword;
            Identifier = identifier;
            OpenParehtesis = openParehtesis;
            Parameters = parameters;
            CloseParehtesis = closeParehtesis;
            TypeClause = typeClause;
            Body = body;
        }

        public override SyntaxType Type => SyntaxType.FUNCTION_DECLARATION;

        public SyntaxToken FunctionKeyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken OpenParehtesis { get; }
        public SeparatedSyntaxList<ParameterSyntax> Parameters { get; }
        public SyntaxToken CloseParehtesis { get; }
        public TypeClauseSyntax? TypeClause { get; }
        public BlockStatementSyntax Body { get; }
    }
}
