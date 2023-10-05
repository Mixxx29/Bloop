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

        public void OnDocumentChanged()
        {
            RenderDocument();
        }

        public void OnLineChanged()
        {
            RenderLine();
        }

        private void RenderDocument()
        {
            Console.CursorVisible = false;
            ClearLastLine();

            Console.SetCursorPosition(0, 0);

            DrawSeparator();

            for (var i = 0; i < _document.Lines.Count; i++)
                DrawLine(i);

            DrawSeparator();

            _document.UpdateCursor();
            Console.CursorVisible = true;
        }

        private void ClearLastLine()
        {
            Console.SetCursorPosition(0, _document.Lines.Count + 2);
            FillUnusedSpace();
            Console.WriteLine();
        }

        public void UpdateLine()
        {
            RenderLine();
        }

        private void RenderLine()
        {
            Console.CursorVisible = false;
            DrawLine();
            _document.UpdateCursor();
            Console.CursorVisible = true;
        }

        private void DrawSeparator()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(">>");
            Console.ResetColor();
            FillUnusedSpace();
            Console.WriteLine();
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

            FillUnusedSpace();

            if (_document.CurrentLineIndex < _document.Lines.Count)
                Console.WriteLine();
        }

        private void DrawPrefix(int lineNumber)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" {lineNumber}");

            int unusedSpace = 8 - Console.CursorLeft;
            Console.Write(new string(' ', unusedSpace));
            Console.ResetColor();
        }

        private static void FillUnusedSpace()
        {
            var unusedSpaceLength = Console.WindowWidth - Console.CursorLeft - 1;
            var unusedSpace = new string(' ', unusedSpaceLength);
            Console.Write(unusedSpace);
        }
    }
}