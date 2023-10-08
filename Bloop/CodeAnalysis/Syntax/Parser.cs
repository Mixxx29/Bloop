using Bloop.CodeAnalysis.Symbol;
using Bloop.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq.Expressions;
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
            var members = ParseMembers();
            var endOfFileToken = MatchToken(SyntaxType.END_OF_FILE_TOKEN);
            return new CompilationUnitSyntax(members, endOfFileToken);
        }

        private ImmutableArray<MemberSyntax> ParseMembers()
        {
            var members = ImmutableArray.CreateBuilder<MemberSyntax>();


            while (Current.Type != SyntaxType.END_OF_FILE_TOKEN)
            {
                var startToken = Current;

                var member = ParseMember();
                members.Add(member);

                if (Current == startToken)
                    NextToken();
            }

            return members.ToImmutable();
        }

        private MemberSyntax ParseMember()
        {
            if (Current.Type == SyntaxType.FUNCTION_KEYWORD)
                return ParseFunctionDeclaration();

            return ParseGlobalStatement();
        }

        private MemberSyntax ParseFunctionDeclaration()
        {
            var functionKeyword = MatchToken(SyntaxType.FUNCTION_KEYWORD);
            var identifier = MatchToken(SyntaxType.FUNCTION_IDENTIFIER_TOKEN);
            var openParenthesis = MatchToken(SyntaxType.OPEN_PARENTHESIS_TOKEN);
            var parameters = ParseParameters();
            var closeParenthesis = MatchToken(SyntaxType.CLOSE_PARENTHESIS_TOKEN);
            var typeClause = ParseOptionalTypeClause();
            var body = ParseBlockStatement();
            return new FunctionDeclarationSyntax(
                functionKeyword,
                identifier,
                openParenthesis,
                parameters,
                closeParenthesis,
                typeClause,
                body
            );
        }

        private SeparatedSyntaxList<ParameterSyntax> ParseParameters()
        {
            var nodes = ImmutableArray.CreateBuilder<SyntaxNode>();

            while (Current.Type != SyntaxType.CLOSE_PARENTHESIS_TOKEN &&
                   Current.Type != SyntaxType.END_OF_FILE_TOKEN)
            {
                var parameter = ParseParameter();
                nodes.Add(parameter);

                if (Current.Type == SyntaxType.COMMA_TOKEN)
                {
                    var comma = MatchToken(SyntaxType.COMMA_TOKEN);
                    nodes.Add(comma);
                }
            }

            return new SeparatedSyntaxList<ParameterSyntax>(nodes.ToImmutable());
        }

        private ParameterSyntax ParseParameter()
        {
            var identifier = MatchToken(SyntaxType.IDENTIFIER_TOKEN);
            var typeClause = ParseTypeClause();
            return new ParameterSyntax(identifier, typeClause);
        }

        private MemberSyntax ParseGlobalStatement()
        {
            var statement = ParseStatement();
            return new GlobalStatementSyntax(statement);
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

                case SyntaxType.IF_KEYWORD:
                    return ParseIfStatement();

                case SyntaxType.WHILE_KEYWORD:
                    return ParseWhileStatement();

                case SyntaxType.FOR_KEYWORD:
                    return ParseForStatement();

                default:
                    return ParseExpressionStatement();
            }
        }

        private BlockStatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

            var openBraceToken = MatchToken(SyntaxType.OPEN_BRACE_TOKEN);

            while (Current.Type != SyntaxType.END_OF_FILE_TOKEN &&
                   Current.Type != SyntaxType.CLOSE_BRACE_TOKEN)
            {
                var startToken = Current;

                var statement = ParseStatement();
                statements.Add(statement);

                if (Current == startToken)
                    NextToken();
            }

            var closeBraceToken = MatchToken(SyntaxType.CLOSE_BRACE_TOKEN);

            return new BlockStatementSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);
        }

        private VariableDeclarationStatement ParseVariableDeclarationStatement()
        {
            var expected = Current.Type == SyntaxType.VAR_KEYWORD ? SyntaxType.VAR_KEYWORD : SyntaxType.CONST_KEYWORD;
            var keyword = MatchToken(expected);
            var identifier = MatchToken(SyntaxType.IDENTIFIER_TOKEN);
            var typeClause = ParseOptionalTypeClause();
            var equals = MatchToken(SyntaxType.EQUALS_TOKEN);
            var expression = ParseExpression();
            return new VariableDeclarationStatement(keyword, identifier, typeClause, equals, expression);
        }

        private TypeClauseSyntax? ParseOptionalTypeClause()
        {
            if (Current.Type != SyntaxType.COLON_TOKEN)
                return null;

            return ParseTypeClause();
        }

        private TypeClauseSyntax ParseTypeClause()
        {
            var colonToken = MatchToken(SyntaxType.COLON_TOKEN);
            var identifier = NextToken();
            return new TypeClauseSyntax(colonToken, identifier);
        }

        private IfStatementSyntax ParseIfStatement()
        {
            var keyword = MatchToken(SyntaxType.IF_KEYWORD);
            var condition = ParseExpression();
            var statement = ParseStatement();
            var elseStatement = ParseElseStatement();
            return new IfStatementSyntax(keyword, condition, statement, elseStatement);
        }

        private ElseStatementSyntax? ParseElseStatement()
        {
            if (Current.Type != SyntaxType.ELSE_KEYWORD)
                return null;

            var keyword = MatchToken(SyntaxType.ELSE_KEYWORD);;
            var statement = ParseStatement();
            return new ElseStatementSyntax(keyword, statement);
        }

        private WhileStatementSyntax ParseWhileStatement()
        {
            var keyword = MatchToken(SyntaxType.WHILE_KEYWORD);
            var condition = ParseExpression();
            var statement = ParseStatement();
            return new WhileStatementSyntax(keyword, condition, statement);
        }

        private ForStatementSyntax ParseForStatement()
        {
            var forKeyword = MatchToken(SyntaxType.FOR_KEYWORD);
            var identifier = MatchToken(SyntaxType.IDENTIFIER_TOKEN);
            var equals = MatchToken(SyntaxType.EQUALS_TOKEN);
            var firstBound = ParseExpression();
            var toKeyword = MatchToken(SyntaxType.TO_KEYWORD);
            var secondBound = ParseExpression();
            var statement = ParseStatement();
            return new ForStatementSyntax(forKeyword, identifier, equals, firstBound, toKeyword, secondBound, statement);
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

                if (Current.Type == SyntaxType.AS_KEYWORD)
                {
                    var asKeyword = MatchToken(SyntaxType.AS_KEYWORD);

                    var targetTypeToken = NextToken();
                    var targetType = targetTypeToken.Type.GetTypeSymbol();
                    if (targetType != null)
                    {
                        first = new ConversionExpression(first, asKeyword, targetType);
                    }
                    else
                    {
                        _diagnostics.ReportUndefinedType(targetTypeToken.Span, targetTypeToken.Text);
                    }
                }
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

                case SyntaxType.STRING_TOKEN:
                    return ParseStringLiteral();

                case SyntaxType.FUNCTION_IDENTIFIER_TOKEN:
                    return ParseFunctionCallExpression();

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

        private ExpressionSyntax ParseStringLiteral()
        {
            var stringToken = MatchToken(SyntaxType.STRING_TOKEN);
            return new LiteralExpressionNode(stringToken);
        }

        private ExpressionSyntax ParseFunctionCallExpression()
        {
            var identifier = MatchToken(SyntaxType.FUNCTION_IDENTIFIER_TOKEN);
            var openParethesis = MatchToken(SyntaxType.OPEN_PARENTHESIS_TOKEN);
            var arguments = ParseArguments();
            var closeParenthesis = MatchToken(SyntaxType.CLOSE_PARENTHESIS_TOKEN);
            return new FunctionCallExpression(identifier, openParethesis, arguments, closeParenthesis);
        }

        private SeparatedSyntaxList<ExpressionSyntax> ParseArguments()
        {
            var nodes = ImmutableArray.CreateBuilder<SyntaxNode>();
            
            while (Current.Type != SyntaxType.CLOSE_PARENTHESIS_TOKEN &&
                   Current.Type != SyntaxType.END_OF_FILE_TOKEN)
            {
                var expression = ParseExpression();
                nodes.Add(expression);

                if (Current.Type == SyntaxType.COMMA_TOKEN)
                {
                    var comma = MatchToken(SyntaxType.COMMA_TOKEN);
                    nodes.Add(comma);
                }
            }

            return new SeparatedSyntaxList<ExpressionSyntax>(nodes.ToImmutable());
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var identifierToken = NextToken();
            return new NameExpressionSyntax(identifierToken);
        }
    }
}
