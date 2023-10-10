using Bloop.Editor.Document;
using System.Collections.Specialized;
using System.Reflection.Metadata;

namespace Bloop.Editor.Window
{
    internal class DocumentWindow : BloopWindow
    {
        private readonly WindowFrame _frame;
        private readonly WindowCursor _cursor;

        private readonly BloopDocument _document;
        private readonly DocumentRenderer _documentRenderer;

        private readonly string _allowedChars = " (){}+-*/";
        
        private List<int> _lengths;

        public DocumentWindow(BloopDocument document, WindowFrame frame)
        {
            _frame = frame;
            _cursor = new WindowCursor(_frame.Left + 9, _frame.Top + 2);

            _document = document;
            _documentRenderer = new DocumentRenderer(_document, _frame.Left + 1, _frame.Top + 2);

            _lengths = new List<int>();
            foreach (var line in _document.Lines)
                _lengths.Add(line.Length);
        }

        public void Render()
        {
            _frame.Render();
            _documentRenderer.Render();
            RenderStatusBar();
            _cursor.Reset();
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

                case ConsoleKey.LeftArrow:
                    HandleLeftArrow();
                    break;

                case ConsoleKey.RightArrow:
                    HandleRightArrow();
                    break;

                default:
                    HandleTyping(keyInfo);
                    break;
            }

            RenderStatusBar();
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
            if (_cursor.Top == 0)
            {
                ScrollUp();
                return;
            }

            _cursor.MoveUp();
        }

        private void HandleDownArrow()
        {
            if (_cursor.Top == _document.Lines.Count - 1)
            {
                ScrollUp();
                return;
            }

            _cursor.MoveDown();
        }

        private void HandleLeftArrow()
        {
            if (_cursor.Left == 0)
                return;

            _cursor.MoveLeft();
        }

        private void HandleRightArrow()
        {
            if (_cursor.Left == _document.Lines[_cursor.Top].Length)
                return;

            _cursor.MoveRight();
        }

        private void HandleTyping(ConsoleKeyInfo keyInfo)
        {
            if (char.IsLetterOrDigit(keyInfo.KeyChar) || _allowedChars.Contains(keyInfo.KeyChar))
            {
                _document.AddText(_cursor.Top, keyInfo.KeyChar.ToString());
                _cursor.MoveRight();
                return;
            }
        }

        private void ScrollUp()
        {
            
        }

        private void ScrollDown()
        {
            
        }

        private void RenderStatusBar()
        {
            Console.CursorLeft = _frame.Left + 1;
            Console.CursorTop = _frame.Top + _frame.Height;
            Console.Write($"Line: {_cursor.Top + 1} Char: {_cursor.Left + 1}  ");
            _cursor.Reset();
        }
    }
}