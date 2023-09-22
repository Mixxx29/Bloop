using Bloop.CodeAnalysis.Text;

namespace Bloop.Tests.CodeAnalysis.Syntax
{
    public class SourceTextTest
    {
        [Theory]
        [InlineData(".", 1)]
        [InlineData(".\r\n", 2)]
        [InlineData(".\r\n\r\n", 3)]
        public void SourceText_NumberOfLines(string text, int expectedLineCount)
        {
            var sourceText = SourceText.FromText(text);
            Assert.Equal(expectedLineCount, sourceText.Lines.Length);
        }
    }
}