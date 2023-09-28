using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Bloop.CodeAnalysis.Syntax;
using Bloop.CodeAnalysis.Text;

namespace Bloop.Editor
{
    public sealed class DocumentView : DocumentSubscriber
    {
        private readonly BloopDocument _document;

        private int _lastLineDrawn;

        public DocumentView(BloopDocument document)
        {
            _document = document;
            _document.Subscribe(this);
            RenderDocument();
        }

        public void OnDocumentChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RenderDocument();
        }

        public void OnLineChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RenderLine();
        }

        private void RenderDocument()
        {
            ClearLine();

            Console.SetCursorPosition(0, 0);

            DrawSeparator();

            for (var i = 0; i < _document.Lines.Count; i++)
                DrawLine(i);

            DrawSeparator();

            _lastLineDrawn = Console.CursorTop;
            _document.UpdateCursor();
        }

        private void ClearLine()
        {
            var width = Console.BufferWidth;
            var blankLine = new StringBuilder();
            while (width-- > 0)
            {
                blankLine.Append(" ");
            }

            Console.SetCursorPosition(0, _lastLineDrawn);
            Console.WriteLine(blankLine.ToString());
        }

        private void RenderLine()
        {
            DrawLine();
            _document.UpdateCursor();
        }

        private void DrawSeparator()
        {
            var width = Console.BufferWidth - ">>".Length;
            var blankLine = new StringBuilder();
            while (width-- > 0)
            {
                blankLine.Append(" ");
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(">>");
            Console.ResetColor();

            Console.WriteLine(blankLine.ToString());
        }

        private void DrawLine()
        {
            DrawLine(_document.CurrentLineIndex);
        }

        private void DrawLine(int lineIndex)
        {
            Console.CursorLeft = 0;
            DrawPrefix(lineIndex + 1);

            var line = _document.Lines[lineIndex].ToString();
            var lexer = new Lexer(SourceText.FromText(line));

            var token = lexer.NextToken();
            while (token.Type != SyntaxType.END_OF_FILE_TOKEN)
            {
                Console.ForegroundColor = SyntaxFacts.GetColor(token.Type);
                Console.Write(token.Text);
                Console.ResetColor();
                token = lexer.NextToken();
            }


            var unusedSpaceLength = Console.BufferWidth - Console.CursorLeft;
            var unusedSpace = new string(' ', unusedSpaceLength);
            Console.Write(unusedSpace);

            if (lineIndex < _document.Lines.Count - 1)
                ++Console.CursorTop;
        }

        private void DrawPrefix(int lineNumber)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" {lineNumber}      ");
            Console.ResetColor();
        }
    }
}