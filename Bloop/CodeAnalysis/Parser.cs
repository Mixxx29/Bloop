namespace Bloop.CodeAnalysis
{
    class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;
        private List<string> _diagnostics = new List<string>();

        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();

            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.NextToken();

                if (token.Type != TokenType.WHITE_SPACE &&
                    token.Type != TokenType.INVALID)
                {
                    tokens.Add(token);
                }
            } while (token.Type != TokenType.END_OF_FILE);

            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        public SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];

            return _tokens[index];
        }

        public SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken Match(TokenType type)
        {
            if (Current.Type == type)
                return NextToken();

            _diagnostics.Add($"ERROR: Unexpected token <{Current.Type}>, expected <{type}>");
            return new SyntaxToken(type, _position, "");
        }

        public SyntaxTree Parse()
        {
            var expression = ParseTerm();
            var endOfFileToken = Match(TokenType.END_OF_FILE);
            return new SyntaxTree(_diagnostics, expression, endOfFileToken);
        }

        public ExpressionNode ParseTerm()
        {
            var first = ParseFactor();

            while (Current.Type == TokenType.PLUS ||
                   Current.Type == TokenType.MINUS)
            {
                var operatorToken = NextToken();
                var second = ParseFactor();
                first = new BinaryExpressionNode(first, operatorToken, second);
            }

            return first;
        }

        public ExpressionNode ParseFactor()
        {
            var first = ParsePrimaryExpression();

            while (Current.Type == TokenType.ASTERIX ||
                   Current.Type == TokenType.SLASH)
            {
                var operatorToken = NextToken();
                var second = ParsePrimaryExpression();
                first = new BinaryExpressionNode(first, operatorToken, second);
            }

            return first;
        }

        private ExpressionNode ParsePrimaryExpression()
        {
            if (Current.Type == TokenType.OPEN_PARENTHESIS)
            {
                var openParenthesis = NextToken();
                var expression = ParseTerm();
                var closeParenthesis = Match(TokenType.CLOSE_PARENTHESIS);
                return new ParenthesizedExpressionNode(openParenthesis, expression, closeParenthesis);
            }

            var numberToken = Match(TokenType.NUMBER);
            return new NumberExpressionNode(numberToken);
        }
    }
}
