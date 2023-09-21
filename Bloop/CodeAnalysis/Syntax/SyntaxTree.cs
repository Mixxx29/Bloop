namespace Bloop.CodeAnalysis.Syntax
{
    public class SyntaxTree
    {
        public SyntaxTree(IEnumerable<Diagnostic> diagnostics, ExpressionSyntax node, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Node = node;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Node { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var lexer = new Lexer(text);
            while (true)
            {
                var token = lexer.NextToken();
                if (token.Type == SyntaxType.END_OF_FILE_TOKEN)
                    break;

                yield return token;
            }
        }
    }
}
