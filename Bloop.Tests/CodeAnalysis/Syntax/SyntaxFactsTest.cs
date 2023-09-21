﻿using Bloop.CodeAnalysis.Syntax;

namespace Bloop.Tests.CodeAnalysis.Syntax
{
    public class SyntaxFactsTest
    {
        [Theory]
        [MemberData(nameof(GetSyntaxTypeData))]
        public void SyntaxFacts_GetText(SyntaxType type)
        {
            var text = SyntaxFacts.GetText(type);
            if (text == null)
                return;

            var tokens = SyntaxTree.ParseTokens(text);
            var token = Assert.Single(tokens);
            Assert.Equal(type, token.Type);
            Assert.Equal(text, token.Text);
        }

        public static IEnumerable<object[]> GetSyntaxTypeData()
        {
            var types = (SyntaxType[]) Enum.GetValues(typeof(SyntaxType));
            foreach (var type in types) 
                yield return new object[] { type };
        }
    }
}