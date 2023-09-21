using Bloop.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Bloop.CodeAnalysis.Syntax
{
    public class SyntaxTree
    {
        public SyntaxTree(SourceText sourceText, ImmutableArray<Diagnostic> diagnostics, ExpressionSyntax node, SyntaxToken endOfFileToken)
        {
            SourceText = sourceText;
            Diagnostics = diagnostics;
            Node = node;
            EndOfFileToken = endOfFileToken;
        }

        public SourceText SourceText { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Node { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.FromText(text);
            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText sourceText)
        {
            var parser = new Parser(sourceText);
            return parser.Parse();
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var sourceText = SourceText.FromText(text);
            var lexer = new Lexer(sourceText);
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
