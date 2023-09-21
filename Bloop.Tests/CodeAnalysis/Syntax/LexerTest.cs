using Bloop.CodeAnalysis.Syntax;

namespace Bloop.Tests.CodeAnalysis.Syntax
{

    public class LexerTest
    {
        [Fact]
        public void Lexer_Test_TokenTypes()
        {
            var tokenType = Enum.GetValues(typeof(SyntaxType))
                                .Cast<SyntaxType>()
                                .Where(type => type.ToString().EndsWith("_TOKEN") ||
                                               type.ToString().EndsWith("_KEYWORD"));

            var testedTokenType = GetTokens().Concat(GetSeparators()).Select(token => token.type);

            var untestedTokenTypes = new SortedSet<SyntaxType>(tokenType);
            untestedTokenTypes.Remove(SyntaxType.INVALID_TOKEN);
            untestedTokenTypes.Remove(SyntaxType.END_OF_FILE_TOKEN);
            untestedTokenTypes.ExceptWith(testedTokenType);

            Assert.Empty(untestedTokenTypes);
        }

        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Test_SingleToken(SyntaxType syntaxType, string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);

            var token = Assert.Single(tokens);
            Assert.Equal(syntaxType, token.Type);
            Assert.Equal(text, token.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokensCombinationData))]
        public void Test_TokenCombination(SyntaxType type1, string text1, SyntaxType type2, string text2)
        {
            var text = text1 + text2;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(2, tokens.Length);
            Assert.Equal(type1, tokens[0].Type);
            Assert.Equal(text1, tokens[0].Text);
            Assert.Equal(type2, tokens[1].Type);
            Assert.Equal(text2, tokens[1].Text);
        }

        [Theory]
        [MemberData(nameof(GetTokensCombinationWithSeparatorsData))]
        public void Test_TokenCombinationWithSeparators(SyntaxType type1, string text1,
                                          SyntaxType separatorType, string separatorText,
                                          SyntaxType type2, string text2)
        {
            var text = text1 + separatorText + text2;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(3, tokens.Length);
            Assert.Equal(type1, tokens[0].Type);
            Assert.Equal(text1, tokens[0].Text);
            Assert.Equal(separatorType, tokens[1].Type);
            Assert.Equal(separatorText, tokens[1].Text);
            Assert.Equal(type2, tokens[2].Type);
            Assert.Equal(text2, tokens[2].Text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var token in GetTokens().Concat(GetSeparators()))
                yield return new object[] { token.type, token.text };
        }

        public static IEnumerable<object[]> GetTokensCombinationData()
        {
            foreach (var combination in GetTokenCombination())
                yield return new object[] { combination.type1, combination.text1, combination.type2, combination.text2 };
        }

        public static IEnumerable<object[]> GetTokensCombinationWithSeparatorsData()
        {
            foreach (var combination in GetTokenCombinationWithSeparators())
                yield return new object[] 
                { 
                    combination.type1, 
                    combination.text1, 
                    combination.separatorType, 
                    combination.separatorText, 
                    combination.type2, 
                    combination.text2 
                };
        }

        private static IEnumerable<(SyntaxType type1, string text1, SyntaxType type2, string text2)> GetTokenCombination()
        {
            foreach (var token1 in GetTokens())
            {
                foreach (var token2 in GetTokens())
                {
                    if (!RequiresSeparator(token1.type, token2.type))
                        yield return (token1.type, token1.text, token2.type, token2.text);
                }
            }
        }

        private static IEnumerable<(SyntaxType type1, string text1,
                                    SyntaxType separatorType, string separatorText,
                                    SyntaxType type2, string text2)> GetTokenCombinationWithSeparators()
        {
            foreach (var token1 in GetTokens())
            {
                foreach (var token2 in GetTokens())
                {
                    if (RequiresSeparator(token1.type, token2.type))
                    {
                        foreach (var separator in GetSeparators())
                            yield return (token1.type, token1.text, separator.type, separator.text, token2.type, token2.text);
                    }
                }
            }
        }

        private static bool RequiresSeparator(SyntaxType type1, SyntaxType type2)
        {
            var isType1Keyword = type1.ToString().EndsWith("_KEYWORD");
            var isType2Keyword = type2.ToString().EndsWith("_KEYWORD");

            if (type1 == SyntaxType.IDENTIFIER_TOKEN && type2 == SyntaxType.IDENTIFIER_TOKEN)
                return true;

            if (isType1Keyword && isType2Keyword)
                return true;

            if (isType1Keyword && type2 == SyntaxType.IDENTIFIER_TOKEN)
                return true;

            if (type1 == SyntaxType.IDENTIFIER_TOKEN && isType2Keyword)
                return true;

            if (type1 == SyntaxType.NUMBER_TOKEN && type2 == SyntaxType.NUMBER_TOKEN)
                return true;

            if (type1 == SyntaxType.EXCLAMATION_MARK_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.EQUALS_TOKEN && type2 == SyntaxType.EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.EXCLAMATION_MARK_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            if (type1 == SyntaxType.EQUALS_TOKEN && type2 == SyntaxType.DOUBLE_EQUALS_TOKEN)
                return true;

            return false;
        }

        private static IEnumerable<(SyntaxType type, string text)> GetSeparators()
        {
            return new[]
            {
                (SyntaxType.WHITE_SPACE_TOKEN, " "),
                (SyntaxType.WHITE_SPACE_TOKEN, "  "),
                (SyntaxType.WHITE_SPACE_TOKEN, "\r"),
                (SyntaxType.WHITE_SPACE_TOKEN, "\n"),
                (SyntaxType.WHITE_SPACE_TOKEN, "\r\n"),
            };
        }

        private static IEnumerable<(SyntaxType type, string text)> GetTokens()
        {
            var fixedTokens = Enum.GetValues(typeof(SyntaxType))
                                  .Cast<SyntaxType>()
                                  .Select(type => (type: type, text: type.GetText()))
                                  .Where(token => token.text != null);

            var dynamicTokens = new[]
            {
                (SyntaxType.IDENTIFIER_TOKEN, "a"),
                (SyntaxType.IDENTIFIER_TOKEN, "abc"),

                (SyntaxType.NUMBER_TOKEN, "0"),
                (SyntaxType.NUMBER_TOKEN, "1"),
                (SyntaxType.NUMBER_TOKEN, "2147483647")
            };

            return dynamicTokens.Concat(fixedTokens);
        }
    }
}