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
        private readonly int _startLeft;
        private readonly int _startTop;

        private readonly int _offset = 8;

        private List<int> _lengths;

        public DocumentRenderer(BloopDocument document, int startLeft, int startTop)
        {
            _document = document;
            _document.Subscribe(this);

            _startLeft = startLeft;
            _startTop = startTop;

            _lengths = new List<int>();
            foreach (var line in _document.Lines)
                _lengths.Add(line.Length);
        }

        public void OnDocumentChanged(int lineIndex)
        {
            RenderDocument(lineIndex);
        }

        public void OnLineChanged(int charIndex)
        {
            RenderDocument(0);
        }

        public void Render()
        {
            RenderDocument(0);
        }

        private void RenderDocument(int startIndex)
        {
            Console.CursorVisible = false;
            var originalLeft = Console.CursorLeft;
            var originalTop = Console.CursorTop;

            DrawLines(startIndex);

            Console.CursorLeft = originalLeft;
            Console.CursorTop = originalTop;
            Console.CursorVisible = true;
        }

        private void DrawLines(int startIndex)
        {
            Console.CursorTop = _startTop + startIndex;

            for (var i = startIndex; i < _document.Lines.Count; i++)
                DrawLine(i);
        }

        private void DrawLine(int lineIndex)
        {
            Console.CursorLeft = _startLeft;
            DrawLineNumber(lineIndex + 1);
            DrawLineContent(lineIndex);
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
            Console.Write(content);

            if (lineIndex >= _lengths.Count)
            {
                _lengths.Add(content.Length);
                return;
            }

            if (content.Length < _lengths[lineIndex])
                Fill(_lengths[lineIndex] - content.Length);

            _lengths[lineIndex] = content.Length;
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
