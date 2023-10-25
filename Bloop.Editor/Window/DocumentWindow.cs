using Bloop.Editor.Model;
using Bloop.Editor.View;
using System.Collections.Immutable;
using System.Text;
using System.Windows.Forms;

namespace Bloop.Editor.Window
{
    internal class DocumentWindow : BloopWindow
    {
        private readonly WindowFrame _frame;

        private List<DocumentRenderer> _renderers;
        private DocumentRenderer _currentRenderer;

        private readonly int _paddingLeft = 2;
        private readonly int _paddingTop = 2;

        private readonly string _allowedChars = " (){}+-*/=,:<>!|&\"";

        private int _lastCharIndex;

        public DocumentWindow(BloopDocument document, WindowFrame frame)
        {
            _frame = frame;

            _renderers = new List<DocumentRenderer>();
            DisplayDocument(document);
        }

        private int CusrsorStartLeft => _frame.Left + _paddingLeft + DocumentRenderer.Offset;
        private int CusrsorStartTop => _frame.Top + _paddingTop;

        private BloopDocument Document => _currentRenderer.Document;
        private WindowCursor Cursor => _currentRenderer.Cursor;

        private int CurrentLineIndex => Cursor.Top - _frame.Top - _paddingTop + _currentRenderer.LineOffset;
        private int CurrentCharIndex => Cursor.Left - _frame.Left - _paddingLeft - DocumentRenderer.Offset;

        private bool IsAtBottom => Cursor.Top == _frame.Top + _frame.Height - 3;
        private bool IsAtTop => Cursor.Top == _frame.Top + 2;

        public void DisplayDocument(BloopDocument document)
        {
            _frame.SetTitle(document.Name);
            _currentRenderer = GetRenderer(document);
            _currentRenderer.Render();
        }

        private DocumentRenderer GetRenderer(BloopDocument document)
        {
            foreach (var renderer in _renderers)
            {
                if (renderer.Document == document)
                    return renderer;
            }

            return CreateRenderer(document);
        }

        private DocumentRenderer CreateRenderer(BloopDocument document)
        {
            var newRenderer = new DocumentRenderer(document, _frame, CusrsorStartLeft, CusrsorStartTop);
            _renderers.Add(newRenderer);
            return newRenderer;
        }

        public void Render()
        {
            _frame.Render();
            _currentRenderer.Render();
            RenderStatusBar();

            var targetCursorLeft = _lastCharIndex + _frame.Left + _paddingLeft + DocumentRenderer.Offset;
            Cursor.MoveRight(targetCursorLeft - Cursor.Left);
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
            Document.NewLine(CurrentLineIndex, CurrentCharIndex);
            Cursor.MoveLeft(CurrentCharIndex);
            _lastCharIndex = CurrentCharIndex;
            
            if (IsAtBottom)
            {
                _currentRenderer.ScrollUp();
            }
            else
            {
                Cursor.MoveDown();
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

                var lineLength = Document.Lines[CurrentLineIndex - 1].Length;
                var amount = lineLength - CurrentCharIndex;

                Document.DeleteCharacter(CurrentLineIndex, CurrentCharIndex - 1);

                if (IsAtTop)
                {
                    _currentRenderer.ScrollDown();
                }
                else if (!(IsAtBottom && _currentRenderer.IsAtBottom))
                {
                    Cursor.MoveUp();
                }

                Cursor.MoveRight(amount);
                return;
            }

            Document.DeleteCharacter(CurrentLineIndex, CurrentCharIndex - 1);
            Cursor.MoveLeft();
            _lastCharIndex = CurrentCharIndex;
            return;
        }

        private void HandleUpArrow()
        {
            if (IsAtTop)
            {
                _currentRenderer.ScrollDown();
                return;
            }

            Cursor.MoveUp();
            AlignCursor();
        }

        private void HandleDownArrow()
        {
            if (CurrentLineIndex == Document.Lines.Count - 1 || IsAtBottom)
            {
                _currentRenderer.ScrollUp();
                return;
            }

            Cursor.MoveDown();
            AlignCursor();
        }

        private void AlignCursor()
        {
            var currentLineLength = Document.Lines[CurrentLineIndex].Length;

            if (currentLineLength < _lastCharIndex)
            {
                Cursor.MoveLeft(CurrentCharIndex - currentLineLength);
            }
            else
            {
                Cursor.MoveLeft(CurrentCharIndex - _lastCharIndex);
            }
        }

        private void HandleLeftArrow()
        {
            if (CurrentCharIndex == 0)
                return;

            Cursor.MoveLeft();
            _lastCharIndex = CurrentCharIndex;
        }

        private void HandleRightArrow()
        {
            if (CurrentCharIndex == Document.Lines[CurrentLineIndex].Length)
                return;

            Cursor.MoveRight();
            _lastCharIndex = CurrentCharIndex;
        }

        private void HandlePageUp()
        {
            if (!_currentRenderer.ScrollDown())
                return;

            if (!IsAtBottom) Cursor.MoveDown();
            AlignCursor();
        }

        private void HandlePageDown()
        {
            if (!_currentRenderer.ScrollUp())
                return;

            if (!IsAtTop) Cursor.MoveUp();
            AlignCursor();
        }

        private void HandleTyping(ConsoleKeyInfo keyInfo)
        {
            if (char.IsLetterOrDigit(keyInfo.KeyChar) || _allowedChars.Contains(keyInfo.KeyChar))
            {
                Document.AddText(CurrentLineIndex, CurrentCharIndex, keyInfo.KeyChar.ToString());
                Cursor.MoveRight();
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
            _currentRenderer.Render();

            if (focus)
                Console.CursorVisible = true;
        }

        internal void SaveDocument()
        {
            Document.Save();
        }
    }
}