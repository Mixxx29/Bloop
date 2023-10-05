using Bloop.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Bloop.CodeAnalysis.Syntax
{
    public class SyntaxTree
    {
        private SyntaxTree(SourceText sourceText)
        {
            var parser = new Parser(sourceText);
            var root = parser.ParseCompilationUnit();
            var diagnostics = parser.Diagnostics.ToImmutableArray();

            SourceText = sourceText;
            Diagnostics = diagnostics;
            Root = root;
        }

        public SourceText SourceText { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.FromText(text);
            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText sourceText)
        {
            return new SyntaxTree(sourceText);
        }

        public static SyntaxToken? GetTokenAtPosition(string text, int position)
        {
            foreach (var token in ParseTokens(text))
            {
                if (token.Span.Start <= position && token.Span.End >= position)
                    return token;
            }

            return null;
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
