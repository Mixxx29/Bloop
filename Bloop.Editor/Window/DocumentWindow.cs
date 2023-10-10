using Bloop.Editor;
using System.Collections.Specialized;
using System.Reflection.Metadata;

namespace Bloop.Editor.Window
{
    internal class DocumentWindow : BloopWindow, DocumentSubscriber
    {
        private readonly BloopDocument _document;
        private readonly WindowFrame _frame;
        private readonly WindowCursor _cursor;

        private readonly int _leftOffset = 9;
        private readonly int _topOffset = 2;

        private List<int> _lengths;

        public DocumentWindow(BloopDocument document, WindowFrame frame)
        {
            _document = document;
            _document.Subscribe(this);

            _frame = frame;

            _cursor = new WindowCursor(
                _frame.Left + _leftOffset, 
                _frame.Top + _topOffset, 
                _frame.Width - _leftOffset - 2, 
                _frame.Height - _topOffset - 3
            );

            _lengths = new List<int>();
            foreach (var line in _document.Lines)
                _lengths.Add(line.Length);
        }

        public void OnDocumentChanged(int index)
        {
            RenderDocument(index);
        }

        public void OnLineChanged(int index)
        {

        }

        public void Render()
        {
            _frame.Render();
            RenderDocument(0);
        }

        public void HandleKey(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Escape:
                    HandleEscape();
                    break;

                case ConsoleKey.Enter:
                    HandleEnter();
                    break;

                case ConsoleKey.Backspace:
                    HandleBackspace();
                    break;

                case ConsoleKey.UpArrow:
                    HandleUpArrow();
                    break;

                case ConsoleKey.DownArrow:
                    HandleDownArrow();
                    break;
            }
        }

        private void HandleEscape()
        {
            
        }

        private void HandleEnter()
        {
            _document.NewLine(_cursor);
        }

        private void HandleBackspace()
        {
            _document.DeleteCharacter(_cursor);
        }

        private void HandleUpArrow()
        {
            _cursor.MoveUp();
        }

        private void HandleDownArrow()
        {
            _cursor.MoveDown();
        }

        private void RenderDocument(int startIndex)
        {
            Console.CursorVisible = false;
            
            ResetCursorLeft();
            ResetCursorTop();

            Console.CursorTop += startIndex;
            DrawLines(startIndex);

            _cursor.Reset();

            Console.CursorVisible = true;
        }

        private void DrawLines(int startIndex)
        {
            for (var i = startIndex; i < _document.Lines.Count; i++)
                DrawLine(i);
        }

        private void DrawLine(int lineIndex)
        {
            ResetCursorLeft();
            DrawLineNumber(lineIndex + 1);
            DrawLineContent(lineIndex);
        }

        private void DrawLineNumber(int lineNumber)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" " + lineNumber);
            Console.ResetColor();
            Console.Write(new string(' ', _leftOffset - lineNumber.ToString().Length - 1));
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

        private void ResetCursorLeft()
        {
            Console.CursorLeft = _frame.Left + 1;
        }

        private void ResetCursorTop()
        {
            Console.CursorTop = _frame.Top + 2;
        }
    }
}