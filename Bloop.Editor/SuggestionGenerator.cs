using Bloop.CodeAnalysis.Symbol;
using Bloop.CodeAnalysis.Syntax;
using Bloop.CodeAnalysis.Binding;
using System.Collections.Immutable;
using System.Text;

namespace Bloop.Editor
{
    public sealed class SuggestionGenerator
    {
        private readonly BloopDocument _document;

        public SuggestionGenerator(BloopDocument document)
        {
            _document = document;
        }

        public void Suggest()
        {
            var position = _document.CurrentLine.CurrentCharacterIndex;
            var text = _document.CurrentLine.ToString().Substring(0, position);
            var token = SyntaxTree.ParseTokens(text).LastOrDefault();

            if (token == null || token.Text == "")
                return;

            var suggestions = Suggest(token);
            PrintSuggestions(token, suggestions);
        }

        private ImmutableArray<string> Suggest(SyntaxToken token)
        {
            var builder = ImmutableArray.CreateBuilder<string>();

            SuggestVariable(token, builder);
            SuggestKeyword(token, builder);
            SuggestBuiltinFunction(token, builder);

            return builder.ToImmutable();
        }

        private void SuggestVariable(SyntaxToken token, ImmutableArray<string>.Builder builder)
        {
            var textBuilder = new StringBuilder();
            for (var i = 0; i < _document.CurrentLineIndex; i++)
            {
                textBuilder.AppendLine(_document.Lines[i].ToString());
            }

            var syntaxTree = SyntaxTree.Parse(textBuilder.ToString());

            var globalScope = Binder.BindGlobalScope(null, syntaxTree.Root);

            foreach (var variable in globalScope.Variables)
            {
                if (variable.Name.StartsWith(token.Text) && variable.Name != token.Text)
                    builder.Add(variable.Name);
            }
        }

        private void SuggestBuiltinFunction(SyntaxToken token, ImmutableArray<string>.Builder builder)
        {
            foreach (var function in BuiltinFunctions.GetAll())
            {
                if (function.Name.StartsWith(token.Text) && function.Name != token.Text)
                {
                    builder.Add(function.Name + "()");
                }
            }

            return;
        }

        private void SuggestKeyword(SyntaxToken token, ImmutableArray<string>.Builder builder)
        {
            foreach (var enumValue in Enum.GetValues(typeof(SyntaxType)).Cast<SyntaxType>())
            {
                var enumText = enumValue.ToString();
                if (!enumText.EndsWith("_KEYWORD"))
                    continue;

                var keywordText = SyntaxFacts.GetText(enumValue);
                if (keywordText.StartsWith(token.Text) && keywordText != token.Text)
                {
                    builder.Add(keywordText);
                }
            }

            return;
        }

        private void PrintSuggestions(SyntaxToken token, ImmutableArray<string> suggestions)
        {
            if (!suggestions.Any())
                return;

            SuggestionWindow.Instance.Create(_document, token, suggestions);
        }
    }
}