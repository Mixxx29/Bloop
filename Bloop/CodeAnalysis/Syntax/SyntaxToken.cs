using Bloop.CodeAnalysis.Text;

namespace Bloop.CodeAnalysis.Syntax
{
    public class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxType type, int position, string text, object? value = null)
        {
            Type = type;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxType Type { get; }

        public int Position { get; }
        public string Text { get; }
        public object? Value { get; }
        public TextSpan Span => new TextSpan(Position, Text.Length);
    }
}
