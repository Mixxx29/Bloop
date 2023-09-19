﻿namespace Bloop.CodeAnalysis.Syntax
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

                if (token.Type != SyntaxType.WHITE_SPACE_TOKEN &&
                    token.Type != SyntaxType.INVALID_TOKEN)
                {
                    tokens.Add(token);
                }
            } while (token.Type != SyntaxType.END_OF_FILE_TOKEN);

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

        private SyntaxToken MatchToken(SyntaxType type)
        {
            if (Current.Type == type)
                return NextToken();

            _diagnostics.Add($"ERROR: Unexpected token <{Current.Type}>, expected <{type}>");
            return new SyntaxToken(type, _position, "");
        }

        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var endOfFileToken = MatchToken(SyntaxType.END_OF_FILE_TOKEN);
            return new SyntaxTree(_diagnostics, expression, endOfFileToken);
        }

        public ExpressionNode ParseExpression(int parentPresedence = 0)
        {
            ExpressionNode first;
            var unaryOperatorPresedence = Current.Type.GetUnaryOperatorPresedence();
            if (unaryOperatorPresedence != 0 && unaryOperatorPresedence >= parentPresedence)
            {
                var operatorToken = NextToken();
                var operand = ParseExpression(unaryOperatorPresedence);
                return new UnaryExpressionNode(operatorToken, operand);
            }
            else
            {
                first = ParsePrimaryExpression();
            }

            while (true)
            {
                var presedence = Current.Type.GetBinaryOperatorPresedence();
                if (presedence == 0 || presedence <= parentPresedence)
                    break;

                var operatorToken = NextToken();
                var second = ParseExpression(presedence);
                first = new BinaryExpressionNode(first, operatorToken, second);
            }

            return first;
        }

        private ExpressionNode ParsePrimaryExpression()
        {
            switch (Current.Type)
            {
                case SyntaxType.OPEN_PARENTHESIS_TOKEN:
                {
                    var openParenthesis = NextToken();
                    var expression = ParseExpression();
                    var closeParenthesis = MatchToken(SyntaxType.CLOSE_PARENTHESIS_TOKEN);
                    return new ParenthesizedExpressionNode(openParenthesis, expression, closeParenthesis);
                }

                case SyntaxType.TRUE_KEYWORD:
                case SyntaxType.FALSE_KEYWORD:
                {
                    var keywordToken = NextToken();
                    var value = keywordToken.Type == SyntaxType.TRUE_KEYWORD;
                    return new LiteralExpressionNode(keywordToken, value);
                }

                default:
                {
                    var numberToken = MatchToken(SyntaxType.NUMBER_TOKEN);
                    return new LiteralExpressionNode(numberToken); 
                }
            }
        }
    }
}