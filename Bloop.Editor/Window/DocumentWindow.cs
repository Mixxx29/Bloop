using Bloop.Editor.Document;

namespace Bloop.Editor.Window
{
    internal class DocumentWindow : BloopWindow
    {
        private readonly WindowFrame _frame;
        private readonly BloopDocument _document;
        private readonly DocumentRenderer _documentRenderer;
        private readonly WindowCursor _cursor;

        private readonly int _paddingLeft = 1;
        private readonly int _paddingTop = 2;

        private readonly string _allowedChars = " (){}+-*/=,:<>!|&\"";
        
        private List<int> _lengths;

        private int _lastCharIndex;

        public DocumentWindow(BloopDocument document, WindowFrame frame)
        {
            _frame = frame;

            _document = document;
            _documentRenderer = new DocumentRenderer(_document, _frame);

            _cursor = new WindowCursor(
                _frame.Left + _paddingLeft + _documentRenderer.Offset, 
                _frame.Top + _paddingTop
            );

            _lengths = new List<int>();
            foreach (var line in _document.Lines)
                _lengths.Add(line.Length);
        }

        private int CurrentLineIndex => _cursor.Top - _frame.Top - _paddingTop;
        private int CurrentCharIndex => _cursor.Left - _frame.Left - _paddingLeft - _documentRenderer.Offset;

        public void Render()
        {
            _frame.Render();
            _documentRenderer.Render();
            RenderStatusBar();
            _cursor.Reset();

            var targetCursorLeft = _lastCharIndex + _frame.Left + _paddingLeft + _documentRenderer.Offset;
            _cursor.MoveRight(targetCursorLeft - _cursor.Left);
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
            _document.NewLine(CurrentLineIndex, CurrentCharIndex);
            _cursor.MoveDown();
            _cursor.MoveLeft(CurrentCharIndex);
            _lastCharIndex = CurrentCharIndex;
        }

        private void HandleBackspace()
        {
            if (CurrentCharIndex == 0)
            {
                if (CurrentLineIndex == 0)
                    return;

                var lineLength = _document.Lines[CurrentLineIndex - 1].Length;
                var amount = lineLength - CurrentCharIndex;

                _document.DeleteCharacter(CurrentLineIndex, CurrentCharIndex - 1);

                _cursor.MoveUp();
                _cursor.MoveRight(amount);
                return;
            }

            _document.DeleteCharacter(CurrentLineIndex, CurrentCharIndex - 1);
            _cursor.MoveLeft();
            _lastCharIndex = CurrentCharIndex;
            return;
        }

        private void HandleUpArrow()
        {
            if (CurrentLineIndex == 0)
            {
                ScrollUp();
                return;
            }

            _cursor.MoveUp();
            var currentLineLength = _document.Lines[CurrentLineIndex].Length;

            if (currentLineLength < _lastCharIndex)
            {
                _cursor.MoveLeft(CurrentCharIndex - currentLineLength);
            }
            else
            {
                _cursor.MoveLeft(CurrentCharIndex - _lastCharIndex);
            }
        }

        private void HandleDownArrow()
        {
            if (CurrentLineIndex == _document.Lines.Count - 1)
            {
                ScrollDown();
                return;
            }

            _cursor.MoveDown();
            var currentLineLength = _document.Lines[CurrentLineIndex].Length;

            if (currentLineLength <= _lastCharIndex)
            {
                _cursor.MoveLeft(CurrentCharIndex - currentLineLength);
            }
            else
            {
                _cursor.MoveLeft(CurrentCharIndex - _lastCharIndex);
            }
        }

        private void HandleLeftArrow()
        {
            if (CurrentCharIndex == 0)
                return;

            _cursor.MoveLeft();
            _lastCharIndex = CurrentCharIndex;
        }

        private void HandleRightArrow()
        {
            if (CurrentCharIndex == _document.Lines[CurrentLineIndex].Length)
                return;

            _cursor.MoveRight();
            _lastCharIndex = CurrentCharIndex;
        }

        private void HandleTyping(ConsoleKeyInfo keyInfo)
        {
            if (char.IsLetterOrDigit(keyInfo.KeyChar) || _allowedChars.Contains(keyInfo.KeyChar))
            {
                _document.AddText(CurrentLineIndex, CurrentCharIndex, keyInfo.KeyChar.ToString());
                _cursor.MoveRight();
                _lastCharIndex = CurrentCharIndex;
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
            Console.CursorTop = _frame.Top + _frame.Height;
            Console.CursorLeft = _frame.Left + _frame.Width - 20;
            Console.ResetColor();
            Console.Write("Line: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(CurrentLineIndex + 1);
            Console.Write(new string(' ', 3 - (CurrentLineIndex + 1).ToString().Length));
            Console.ResetColor();
            Console.Write(" Char: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(CurrentCharIndex + 1);
            Console.ResetColor();
            _cursor.Reset();
        }

        public void SetFocus(bool focus)
        {
            _frame.SetFocus(focus);
        }
    }
}