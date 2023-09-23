namespace Bloop.CodeAnalysis.Syntax
{
    public sealed class ForStatementSyntax : StatementSyntax
    {
        public ForStatementSyntax(SyntaxToken forKeyword, SyntaxToken identifier, SyntaxToken equals, ExpressionSyntax firstBound, SyntaxToken toKeyword, ExpressionSyntax secondBound, StatementSyntax statement)
        {
            ForKeyword = forKeyword;
            Identifier = identifier;
            Equals = equals;
            FirstBound = firstBound;
            ToKeyword = toKeyword;
            SecondBound = secondBound;
            Statement = statement;
        }

        public override SyntaxType Type => SyntaxType.FOR_STATEMENT;

        public SyntaxToken ForKeyword { get; }
        public SyntaxToken Identifier { get; }
        public new SyntaxToken Equals { get; }
        public ExpressionSyntax FirstBound { get; }
        public SyntaxToken ToKeyword { get; }
        public ExpressionSyntax SecondBound { get; }
        public StatementSyntax Statement { get; }
    }
}
