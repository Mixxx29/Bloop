using Bloop.CodeAnalysis.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Bloop.CodeAnalysis.Syntax
{
    public class Lexer
    {
        private readonly SourceText _sourceText;
        private readonly DiagnosticsPool _diagnostics = new DiagnosticsPool();

        private int _position;

        private int _start;
        private SyntaxType _type;
        private object? _value;

        public Lexer(SourceText sourceText)
        {
            _sourceText = sourceText;
        }

        public DiagnosticsPool Diagnostics => _diagnostics;

        public char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _sourceText.ToString().Length)
                return '\0';

            return _sourceText.ToString()[index];
        }

        private char Current => Peek(0);
        private char Lookahead => Peek(1);

        public SyntaxToken NextToken()
        {
            _start = _position;
            _type = SyntaxType.INVALID_TOKEN;
            _value = null;

            switch (Current)
            {
                case '\0':
                    _type = SyntaxType.END_OF_FILE_TOKEN;
                    break;

                case '+':
                    _type = SyntaxType.PLUS_TOKEN;
                    _position++;
                    break;

                case '-':
                    _type = SyntaxType.MINUS_TOKEN;
                    _position++;
                    break;

                case '*':
                    _type = SyntaxType.ASTERIX_TOKEN;
                    _position++;
                    break;

                case '/':
                    _type = SyntaxType.SLASH_TOKEN;
                    _position++;
                    break;

                case '(':
                    _type = SyntaxType.OPEN_PARENTHESIS_TOKEN;
                    _position++;
                    break;

                case ')':
                    _type = SyntaxType.CLOSE_PARENTHESIS_TOKEN;
                    _position++;
                    break;

                case '{':
                    _type = SyntaxType.OPEN_BRACE_TOKEN;
                    _position++;
                    break;

                case '}':
                    _type = SyntaxType.CLOSE_BRACE_TOKEN;
                    _position++;
                    break;

                case '!' when Lookahead == '=':
                    _type = SyntaxType.EXCLAMATION_MARK_EQUALS_TOKEN;
                    _position += 2;
                    break;

                case '!':
                    _type = SyntaxType.EXCLAMATION_MARK_TOKEN;
                    _position++;
                    break;

                case '&' when Lookahead == '&':
                    _type = SyntaxType.DOUBLE_AMPERSAND_TOKEN;
                    _position += 2;
                    break;

                case '|' when Lookahead == '|':
                    _type = SyntaxType.DOUBLE_PIPE_TOKEN;
                    _position += 2;
                    break;

                case '=' when Lookahead == '=':
                    _type = SyntaxType.DOUBLE_EQUALS_TOKEN;
                    _position += 2;
                    break;

                case '=':
                    _type = SyntaxType.EQUALS_TOKEN;
                    _position++;
                    break;

                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                    ReadNumberToken();
                    break;

                case ' ': case '\n': case '\t': case '\r':
                    ReadWhiteSpace();
                    break;

                case '<' when Lookahead == '=':
                    _type = SyntaxType.LESS_THAN_OR_EQUALS_TOKEN;
                    _position += 2;
                    break;

                case '<':
                    _type = SyntaxType.LESS_THAN_TOKEN;
                    _position++;
                    break;

                case '>' when Lookahead == '=':
                    _type = SyntaxType.GREATER_THAN_OR_EQUALS_TOKEN;
                    _position += 2;
                    break;

                case '>':
                    _type = SyntaxType.GREATER_THAN_TOKEN;
                    _position++;
                    break;

                default:
                {
                    if (char.IsLetter(Current))
                    {
                        ReadIdentifierOrKeyword();
                    }
                    else if (char.IsWhiteSpace(Current))
                    {
                        ReadWhiteSpace();
                    }
                    else
                    {
                        _diagnostics.ReportInvalidCharacter(_position++, Current);
                    }
                    break;
                }
            }

            var length = _position - _start;
            var text = _type.GetText();
            if (text == null)
                text = _sourceText.ToString(_start, length);

            return new SyntaxToken(_type, _start, text, _value);
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
                _position++;

            var length = _position - _start;
            var text = _sourceText.ToString(_start, length);
            if (!int.TryParse(text, out var value))
                _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, typeof(int));

            _type = SyntaxType.NUMBER_TOKEN;
            _value = value;
        }

        private void ReadWhiteSpace()
        {
            while (char.IsWhiteSpace(Current))
                _position++;

            _type = SyntaxType.WHITE_SPACE_TOKEN;
        }

        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetter(Current))
                _position++;

            var length = _position - _start;
            var text = _sourceText.ToString(_start, length);
            _type = text.GetKeywordType();
        }
    }
}
