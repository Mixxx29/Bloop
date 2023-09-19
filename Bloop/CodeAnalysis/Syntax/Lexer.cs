namespace Bloop.CodeAnalysis.Syntax
{
    public class Lexer
    {
        private readonly string _text;
        private int _position;
        private List<string> _diagnostics = new List<string>();

        public Lexer(string text)
        {
            _text = text;
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        public char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _text.Length)
                return '\0';

            return _text[index];
        }

        private char Current => Peek(0);
        private char Lookahead => Peek(1);

        public SyntaxToken NextToken()
        {
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxType.END_OF_FILE_TOKEN, _position, "\0");

            if (char.IsDigit(Current))
            {
                var start = _position;
                while (char.IsDigit(Current))
                    _position++;

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.Add($"ERROR: Invalid number value");
                }
                return new SyntaxToken(SyntaxType.NUMBER_TOKEN, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                var start = _position;
                while (char.IsWhiteSpace(Current))
                    _position++;

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxType.WHITE_SPACE_TOKEN, start, text);
            }

            if (char.IsLetter(Current))
            {
                var start = _position;
                while (char.IsLetter(Current))
                    _position++;

                var length = _position - start;
                var text = _text.Substring(start, length);
                var type = text.GetKeywordType();
                return new SyntaxToken(type, start, text);
            }

            if (Current == '+')
                return new SyntaxToken(SyntaxType.PLUS_TOKEN, _position++, "+");
            if (Current == '-')
                return new SyntaxToken(SyntaxType.MINUS_TOKEN, _position++, "-");
            if (Current == '*')
                return new SyntaxToken(SyntaxType.ASTERIX_TOKEN, _position++, "*");
            if (Current == '/')
                return new SyntaxToken(SyntaxType.SLASH_TOKEN, _position++, "/");
            if (Current == '(')
                return new SyntaxToken(SyntaxType.OPEN_PARENTHESIS_TOKEN, _position++, "(");
            if (Current == ')')
                return new SyntaxToken(SyntaxType.CLOSE_PARENTHESIS_TOKEN, _position++, ")");
            if (Current == '!')
                return new SyntaxToken(SyntaxType.EXCLAMATION_MARK_TOKEN, _position++, "!");
            if (Current == '&' && Lookahead == '&')
                return new SyntaxToken(SyntaxType.DOUBLE_AMPERSAND_TOKEN, _position +=2 , "&&");
            if (Current == '|' && Lookahead == '|')
                return new SyntaxToken(SyntaxType.DOUBLE_PIPE_TOKEN, _position += 2, "||");

            _diagnostics.Add($"ERROR: Unexpected character: '{Current}'");
            return new SyntaxToken(SyntaxType.INVALID_TOKEN, _position++, _text.Substring(_position - 1, 1));
        }
    }
}
