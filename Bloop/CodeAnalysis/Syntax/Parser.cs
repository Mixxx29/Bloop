using Bloop.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Bloop.CodeAnalysis.Syntax
{
    class Parser
    {
        private readonly DiagnosticsPool _diagnostics = new DiagnosticsPool();
        private readonly ImmutableArray<SyntaxToken> _tokens;
        private readonly SourceText _sourceText;
        private int _position;

        public Parser(SourceText sourceText)
        {
            _sourceText = sourceText;

            var tokens = new List<SyntaxToken>();

            var lexer = new Lexer(sourceText);
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

            _tokens = tokens.ToImmutableArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public DiagnosticsPool Diagnostics => _diagnostics;

        public SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];

            return _tokens[index];
        }

        public SyntaxToken Current => Peek(0);
        public SyntaxToken Lookahead => Peek(1);

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

            _diagnostics.ReportUnexpectedToken(Current.Span, Current.Type, type);
            return new SyntaxToken(type, _position, "");
        }

        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var statement = ParseStatement();
            var endOfFileToken = MatchToken(SyntaxType.END_OF_FILE_TOKEN);
            return new CompilationUnitSyntax(statement, endOfFileToken);
        }

        private StatementSyntax ParseStatement()
        {
            switch (Current.Type)
            {
                case SyntaxType.OPEN_BRACE_TOKEN:
                    return ParseBlockStatement();
                case SyntaxType.VAR_KEYWORD:
                case SyntaxType.CONST_KEYWORD:
                    return ParseVariableDeclarationStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private BLockStatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

            var openBraceToken = MatchToken(SyntaxType.OPEN_BRACE_TOKEN);

            while (Current.Type != SyntaxType.END_OF_FILE_TOKEN &&
                   Current.Type != SyntaxType.CLOSE_BRACE_TOKEN)
            {
                var statement = ParseStatement();
                statements.Add(statement);
            }

            var closeBraceToken = MatchToken(SyntaxType.CLOSE_BRACE_TOKEN);

            return new BLockStatementSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);
        }

        private VariableDeclarationStatement ParseVariableDeclarationStatement()
        {
            var expected = Current.Type == SyntaxType.VAR_KEYWORD ? SyntaxType.VAR_KEYWORD : SyntaxType.CONST_KEYWORD;
            var keyword = MatchToken(expected);
            var identifier = MatchToken(SyntaxType.IDENTIFIER_TOKEN);
            var equals = MatchToken(SyntaxType.EQUALS_TOKEN);
            var expression = ParseExpression();
            return new VariableDeclarationStatement(keyword, identifier, equals, expression);
        }

        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();
            return new ExpressionStatementSyntax(expression);
        }

        private ExpressionSyntax ParseExpression()
        {
            if (Current.Type == SyntaxType.IDENTIFIER_TOKEN &&
                Lookahead.Type == SyntaxType.EQUALS_TOKEN)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var expression = ParseExpression();
                return new AssignmentExpressionSyntax(identifierToken, operatorToken, expression);
            }

            return ParseBinaryExpression();
        }

        private ExpressionSyntax ParseBinaryExpression(int parentPresedence = 0)
        {
            ExpressionSyntax first;
            var unaryOperatorPresedence = Current.Type.GetUnaryOperatorPresedence();
            if (unaryOperatorPresedence != 0 && unaryOperatorPresedence >= parentPresedence)
            {
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression(unaryOperatorPresedence);
                first = new UnaryExpressionSyntax(operatorToken, operand);
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
                var second = ParseBinaryExpression(presedence);
                first = new BinaryExpressionNode(first, operatorToken, second);
            }

            return first;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Type)
            {
                case SyntaxType.OPEN_PARENTHESIS_TOKEN:
                        return ParseParenthesisExpression();

                case SyntaxType.TRUE_KEYWORD:
                case SyntaxType.FALSE_KEYWORD:
                        return ParseBooleanExpression();

                case SyntaxType.NUMBER_TOKEN:
                    return ParseNumberLiteral();

                case SyntaxType.IDENTIFIER_TOKEN:
                default:
                    return ParseNameExpression();
            }
        }

        private ExpressionSyntax ParseParenthesisExpression()
        {
            var openParenthesis = MatchToken(SyntaxType.OPEN_PARENTHESIS_TOKEN);
            var expression = ParseExpression();
            var closeParenthesis = MatchToken(SyntaxType.CLOSE_PARENTHESIS_TOKEN);
            return new ParenthesizedExpressionNode(openParenthesis, expression, closeParenthesis);
        }

        private ExpressionSyntax ParseBooleanExpression()
        {
            var isTrue = Current.Type == SyntaxType.TRUE_KEYWORD;
            var keywordToken = isTrue ? MatchToken(SyntaxType.TRUE_KEYWORD) : MatchToken(SyntaxType.FALSE_KEYWORD);
            return new LiteralExpressionNode(keywordToken, isTrue);
        }

        private ExpressionSyntax ParseNumberLiteral()
        {
            var numberToken = MatchToken(SyntaxType.NUMBER_TOKEN);
            return new LiteralExpressionNode(numberToken);
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var identifierToken = NextToken();
            return new NameExpressionSyntax(identifierToken);
        }
    }
}
