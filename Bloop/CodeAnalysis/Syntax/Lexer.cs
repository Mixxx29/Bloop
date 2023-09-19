namespace Bloop.CodeAnalysis.Syntax
{
    internal class Lexer
    {
        private readonly string _text;
        private int _position;
        private DiagnosticsPool _diagnostics = new DiagnosticsPool();

        public Lexer(string text)
        {
            _text = text;
        }

        public DiagnosticsPool Diagnostics => _diagnostics;

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

            var start = _position;

            if (char.IsDigit(Current))
            {
                while (char.IsDigit(Current))
                    _position++;

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.ReportInvalidNumber(new TextSpan(start, length), _text, typeof(int));
                }
                return new SyntaxToken(SyntaxType.NUMBER_TOKEN, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                while (char.IsWhiteSpace(Current))
                    _position++;

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxType.WHITE_SPACE_TOKEN, start, text);
            }

            if (char.IsLetter(Current))
            {
                while (char.IsLetter(Current))
                    _position++;

                var length = _position - start;
                var text = _text.Substring(start, length);
                var type = text.GetKeywordType();
                return new SyntaxToken(type, start, text);
            }

            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxType.PLUS_TOKEN, _position++, "+");

                case '-':
                    return new SyntaxToken(SyntaxType.MINUS_TOKEN, _position++, "-");

                case '*':
                    return new SyntaxToken(SyntaxType.ASTERIX_TOKEN, _position++, "*");

                case '/':
                    return new SyntaxToken(SyntaxType.SLASH_TOKEN, _position++, "/");

                case '(':
                    return new SyntaxToken(SyntaxType.OPEN_PARENTHESIS_TOKEN, _position++, "(");

                case ')':
                    return new SyntaxToken(SyntaxType.CLOSE_PARENTHESIS_TOKEN, _position++, ")");

                case '!' when Lookahead == '=':
                    _position += 2;
                    return new SyntaxToken(SyntaxType.EXCLAMATION_MARK_EQUALS_TOKEN, start, "!=");

                case '!' when Lookahead != '=':
                    return new SyntaxToken(SyntaxType.EXCLAMATION_MARK_TOKEN, _position++, "!");

                case '&' when Lookahead == '&':
                    _position += 2;
                    return new SyntaxToken(SyntaxType.DOUBLE_AMPERSAND_TOKEN, start, "&&");

                case '|' when Lookahead == '|':
                    _position += 2;
                    return new SyntaxToken(SyntaxType.DOUBLE_PIPE_TOKEN, start, "||");

                case '=' when Lookahead == '=':
                    _position += 2;
                    return new SyntaxToken(SyntaxType.DOUBLE_EQUALS_TOKEN, start, "==");

                case '=' when Lookahead != '=':
                    return new SyntaxToken(SyntaxType.EQUALS_TOKEN, _position++, "=");
            }

            _diagnostics.ReportInvalidCharacter(_position, Current);
            return new SyntaxToken(SyntaxType.INVALID_TOKEN, _position++, _text.Substring(_position - 1, 1));
        }
    }
}
