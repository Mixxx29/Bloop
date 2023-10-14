using Bloop.CodeAnalysis.Syntax;
using Bloop.Editor.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Document
{
    internal class DocumentRenderer : DocumentSubscriber
    {
        private readonly BloopDocument _document;
        private readonly WindowFrame _frame;
        private readonly int _offset = 6;

        private List<int> _lengths;

        public DocumentRenderer(BloopDocument document, WindowFrame frame)
        {
            _document = document;
            _document.Subscribe(this);

            _frame = frame;

            _lengths = new List<int>();
            foreach (var line in _document.Lines)
                _lengths.Add(line.Length);
        }

        public int Offset => _offset;

        public void OnDocumentChanged(int lineIndex)
        {
            RenderDocument(lineIndex);
        }

        public void OnLineChanged(int lineIndex, int charIndex)
        {
            RenderLine(lineIndex, charIndex);
        }

        public void Render()
        {
            RenderDocument(0);
        }

        private void RenderDocument(int lineIndex)
        {
            Console.CursorVisible = false;
            var originalLeft = Console.CursorLeft;
            var originalTop = Console.CursorTop;

            DrawLines(lineIndex);
            ClearLine();

            Console.CursorLeft = originalLeft;
            Console.CursorTop = originalTop;
            Console.CursorVisible = true;
        }

        private void RenderLine(int lineIndex, int charIndex)
        {
            Console.CursorVisible = false;
            var originalLeft = Console.CursorLeft;

            DrawLine(lineIndex);

            Console.CursorLeft = originalLeft;
            Console.CursorVisible = true;
        }

        private void ClearLine()
        {
            Console.CursorLeft = _frame.Left + 1;
            Fill(_frame.Width - 2);
        }

        private void DrawLines(int startIndex)
        {
            Console.CursorTop = _frame.Top + 2 + startIndex;
            for (var i = startIndex; i < _document.Lines.Count; i++)
                DrawLine(i);
        }

        private void DrawLine(int lineIndex)
        {
            Console.CursorLeft = _frame.Left + 1;
            DrawLineNumber(lineIndex + 1);
            DrawLineContent(lineIndex);
            Console.WriteLine();
        }

        private void DrawLineNumber(int lineNumber)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" " + lineNumber);
            Console.ResetColor();
            Console.Write(new string(' ', (_offset - 1) - lineNumber.ToString().Length));
        }

        private void DrawLineContent(int lineIndex)
        {
            var content = _document.Lines[lineIndex].ToString();
            var tokens = SyntaxTree.ParseTokens(content);

            foreach (var token in tokens)
                DrawToken(token);

            Console.ResetColor();

            if (lineIndex >= _lengths.Count)
            {
                _lengths.Add(content.Length);
                return;
            }

            if (content.Length < _lengths[lineIndex])
                Fill(_lengths[lineIndex] - content.Length);

            _lengths[lineIndex] = content.Length;
        }

        private void DrawToken(SyntaxToken token)
        {
            Console.ForegroundColor = SyntaxFacts.GetColor(token.Type);
            Console.Write(token.Text);
        }

        private void Fill(int length)
        {
            Fill(' ', length);
        }

        private void Fill(char c, int length)
        {
            var fill = new string(c, length);
            Console.Write(fill);
        }
    }
}
