using Bloop.CodeAnalysis;
using Bloop.CodeAnalysis.Syntax;

namespace Bloop.Tests.CodeAnalysis
{
    public class EvaluatorTest
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("-0", 0)]
        [InlineData("-1", -1)]
        [InlineData("-1 + 5", 4)]
        [InlineData("-(18 - 51)", 33)]
        [InlineData("-(10)", -10)]
        [InlineData("5 == 5", true)]
        [InlineData("678 == 21", false)]
        [InlineData("52 != 215", true)]
        [InlineData("52 != 52", false)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!true", false)]
        [InlineData("!false", true)]
        [InlineData("false && true", true)]
        [InlineData("false || false", false)]
        [InlineData("!(false || !false)", false)]
        [InlineData("(a = 10) * a", 100)]
        public void Evaluator_Evaluate(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }
    }
}