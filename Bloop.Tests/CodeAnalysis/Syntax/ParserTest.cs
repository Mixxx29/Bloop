using Bloop.CodeAnalysis.Syntax;
using System.Collections;

namespace Bloop.Tests.CodeAnalysis.Syntax
{
    public class ParserTest
    {
        [Theory]
        [MemberData(nameof(GetBinaryOperatorsCombinationData))]
        public void Parser_BinaryExpression_HonorPresedences(SyntaxType operator1, SyntaxType operator2)
        {
            var operator1Presedence = operator1.GetBinaryOperatorPresedence();
            var operator2Presedence = operator2.GetBinaryOperatorPresedence();

            var operator1Text = operator1.GetText();
            var operator2Text = operator2.GetText();

            var text = $"a {operator1Text} b {operator2Text} c";
            var expression = SyntaxTree.Parse(text).Node;

            if (operator1Presedence >= operator2Presedence)
            {
                //      op2
                //     /   \
                //   op1    c
                //  /   \
                // a     b

                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxType.BINARY_EXPRESSION);
                    e.AssertNode(SyntaxType.BINARY_EXPRESSION);
                    e.AssertNode(SyntaxType.NAME_EXPRESSION);
                    e.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "a");
                    e.AssertToken(operator1, operator1Text);
                    e.AssertNode(SyntaxType.NAME_EXPRESSION);
                    e.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "b");
                    e.AssertToken(operator2, operator2Text);
                    e.AssertNode(SyntaxType.NAME_EXPRESSION);
                    e.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "c");
                }
            }
            else
            {
                //   op1   
                //  /   \
                // a    op2
                //     /   \
                //    b     c

                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxType.BINARY_EXPRESSION);
                    e.AssertNode(SyntaxType.NAME_EXPRESSION);
                    e.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "a");
                    e.AssertToken(operator1, operator1Text);
                    e.AssertNode(SyntaxType.BINARY_EXPRESSION);
                    e.AssertNode(SyntaxType.NAME_EXPRESSION);
                    e.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "b");
                    e.AssertToken(operator2, operator2Text);
                    e.AssertNode(SyntaxType.NAME_EXPRESSION);
                    e.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "c");
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetUnaryOperatorsCombinationData))]
        public void Parser_UnaryExpression_HonorPresedences(SyntaxType unaryOperator, SyntaxType binaryOperator)
        {
            var unaryOperatorPresedence = unaryOperator.GetUnaryOperatorPresedence();
            var binaryOperatorPresedence = binaryOperator.GetBinaryOperatorPresedence();

            var unaryOperatorText = unaryOperator.GetText();
            var binaryOperatorText = binaryOperator.GetText();

            var text = $"{unaryOperatorText} a {binaryOperatorText} b";
            var expression = SyntaxTree.Parse(text).Node;

            if (unaryOperatorPresedence >= binaryOperatorPresedence)
            {
                //     binary
                //     /    \
                //  unary    b
                //    |
                //    a

                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxType.BINARY_EXPRESSION);
                    e.AssertNode(SyntaxType.UNARY_EXPRESSION);
                    e.AssertToken(unaryOperator, unaryOperatorText);
                    e.AssertNode(SyntaxType.NAME_EXPRESSION);
                    e.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "a");
                    e.AssertToken(binaryOperator, binaryOperatorText);
                    e.AssertNode(SyntaxType.NAME_EXPRESSION);
                    e.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "b");
                }
            }
            else
            {
                //   unary   
                //     |
                //   binary
                //   /    \
                //  a      b

                using (var e = new AssertingEnumerator(expression))
                {
                    e.AssertNode(SyntaxType.UNARY_EXPRESSION);
                    e.AssertToken(unaryOperator, unaryOperatorText);
                    e.AssertNode(SyntaxType.BINARY_EXPRESSION);
                    e.AssertNode(SyntaxType.NAME_EXPRESSION);
                    e.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "a");
                    e.AssertToken(binaryOperator, binaryOperatorText);
                    e.AssertNode(SyntaxType.NAME_EXPRESSION);
                    e.AssertToken(SyntaxType.IDENTIFIER_TOKEN, "b");
                }
            }
        }

        public static IEnumerable<object[]> GetBinaryOperatorsCombinationData()
        {
            foreach (var operator1 in SyntaxFacts.GetBinaryOperatorTypes())
            {
                foreach (var operator2 in SyntaxFacts.GetBinaryOperatorTypes())
                {
                    yield return new object[] { operator1, operator2 };
                }
            }
        }

        public static IEnumerable<object[]> GetUnaryOperatorsCombinationData()
        {
            foreach (var unaryOperator in SyntaxFacts.GetUnaryOperatorTypes())
            {
                foreach (var binaryOperator in SyntaxFacts.GetBinaryOperatorTypes())
                {
                    yield return new object[] { unaryOperator, binaryOperator };
                }
            }
        }
    }
}