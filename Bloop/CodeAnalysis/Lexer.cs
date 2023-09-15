namespace Bloop.CodeAnalysis
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

        private char Current
        {
            get
            {
                if (_position >= _text.Length)
                    return '\0';
                return _text[_position];
            }
        }


        public SyntaxToken NextToken()
        {
            if (_position >= _text.Length)
                return new SyntaxToken(TokenType.END_OF_FILE, _position, "\0");

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
                return new SyntaxToken(TokenType.NUMBER, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                var start = _position;
                while (char.IsWhiteSpace(Current))
                    _position++;

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(TokenType.WHITE_SPACE, start, text);
            }

            if (Current == '+')
                return new SyntaxToken(TokenType.PLUS, _position++, "+");
            if (Current == '-')
                return new SyntaxToken(TokenType.MINUS, _position++, "-");
            if (Current == '*')
                return new SyntaxToken(TokenType.ASTERIX, _position++, "*");
            if (Current == '/')
                return new SyntaxToken(TokenType.SLASH, _position++, "/");
            if (Current == '(')
                return new SyntaxToken(TokenType.OPEN_PARENTHESIS, _position++, "(");
            if (Current == ')')
                return new SyntaxToken(TokenType.CLOSE_PARENTHESIS, _position++, ")");

            _diagnostics.Add($"ERROR: Unexpected character: '{Current}'");
            return new SyntaxToken(TokenType.INVALID, _position++, _text.Substring(_position - 1, 1));
        }
    }
}
