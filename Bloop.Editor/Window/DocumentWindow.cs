using Bloop.Editor.Document;
using System.Collections.Immutable;
using System.Text;

namespace Bloop.Editor.Window
{
    internal class DocumentWindow : BloopWindow
    {
        private readonly WindowFrame _frame;
        private readonly BloopDocument _document;
        private readonly DocumentRenderer _documentRenderer;
        private readonly WindowCursor _cursor;

        private readonly int _paddingLeft = 2;
        private readonly int _paddingTop = 2;

        private readonly string _allowedChars = " (){}+-*/=,:<>!|&\"";

        private int _lastCharIndex;

        private bool _ctrlDown;

        public DocumentWindow(BloopDocument document, WindowFrame frame)
        {
            _frame = frame;

            _document = document;
            _documentRenderer = new DocumentRenderer(_document, _frame);

            _cursor = new WindowCursor(
                _frame.Left + _paddingLeft + _documentRenderer.Offset, 
                _frame.Top + _paddingTop
            );
        }

        private int CurrentLineIndex => _cursor.Top - _frame.Top - _paddingTop + _documentRenderer.LineOffset;
        private int CurrentCharIndex => _cursor.Left - _frame.Left - _paddingLeft - _documentRenderer.Offset;

        private bool IsAtBottom => _cursor.Top == _frame.Top + _frame.Height - 3;
        private bool IsAtTop => _cursor.Top == _frame.Top + 2;

        public void Render()
        {
            _frame.Render();
            _documentRenderer.Render();
            RenderStatusBar();

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

                case ConsoleKey.PageUp:
                    HandlePageUp();
                    break;

                case ConsoleKey.PageDown:
                    HandlePageDown();
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
            _cursor.MoveLeft(CurrentCharIndex);
            _lastCharIndex = CurrentCharIndex;
            
            if (IsAtBottom)
            {
                _documentRenderer.ScrollUp();
            }
            else
            {
                _cursor.MoveDown();
            }
        }

        private void HandleBackspace()
        {
            if (CurrentCharIndex == 0)
            {
                if (CurrentLineIndex == 0)
                {
                    return;
                }

                var lineLength = _document.Lines[CurrentLineIndex - 1].Length;
                var amount = lineLength - CurrentCharIndex;

                _document.DeleteCharacter(CurrentLineIndex, CurrentCharIndex - 1);

                if (IsAtTop)
                {
                    _documentRenderer.ScrollDown();
                }
                else if (!(IsAtBottom && _documentRenderer.IsAtBottom))
                {
                    _cursor.MoveUp();
                }

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
            if (IsAtTop)
            {
                _documentRenderer.ScrollDown();
                return;
            }

            _cursor.MoveUp();
            AlignCursor();
        }

        private void HandleDownArrow()
        {
            if (CurrentLineIndex == _document.Lines.Count - 1 || IsAtBottom)
            {
                _documentRenderer.ScrollUp();
                return;
            }

            _cursor.MoveDown();
            AlignCursor();
        }

        private void AlignCursor()
        {
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

        private void HandlePageUp()
        {
            if (!_documentRenderer.ScrollDown())
                return;

            if (!IsAtBottom) _cursor.MoveDown();
            AlignCursor();
        }

        private void HandlePageDown()
        {
            if (!_documentRenderer.ScrollUp())
                return;

            if (!IsAtTop) _cursor.MoveUp();
            AlignCursor();
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

        private void RenderStatusBar()
        {
            var rect = new Rect()
            {
                X = _frame.Left + _frame.Width - 20,
                Y = _frame.Top + _frame.Height,
                Width = 19,
                Height = 1
            };

            var builder = ImmutableArray.CreateBuilder<CharInfo>();

            builder.AddRange(CharInfo.FromText("Line: ", ConsoleColor.White));
            builder.AddRange(CharInfo.FromText((CurrentLineIndex + 1).ToString().PadRight(4), ConsoleColor.Yellow));

            builder.AddRange(CharInfo.FromText("Char: ", ConsoleColor.White));
            builder.AddRange(CharInfo.FromText((CurrentCharIndex + 1).ToString().PadRight(3), ConsoleColor.Yellow));

            ConsoleManager.Write(builder.ToImmutable(), rect);
        }

        public void SetFocus(bool focus)
        {
            _frame.SetFocus(focus);
            _documentRenderer.Render();
        }
    }
}