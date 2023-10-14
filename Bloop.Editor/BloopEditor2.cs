

using Bloop.Editor.Document;
using Bloop.Editor.Configuration;
using Bloop.Editor.Window;

namespace Bloop
{
    public class BloopEditor2
    {
        private bool _processing = true;

        private BloopWindow _focusedWindow;

        private DocumentWindow _documentWindow;
        private ProjectWindow _projectWindow;

        public BloopEditor2()
        {
            var projectWindowFrame = new WindowFrame(
                "DemoProject",
                "Ctrl+P",
                0,
                0,
                0.3f,
                1.0f
            );
            _projectWindow = new ProjectWindow(projectWindowFrame);

            var document = new BloopDocument();

            var documentWindowFrame = new WindowFrame(
                document.Name,
                "Ctrl+E",
                0.3f,
                0,
                0.7f,
                1.0f
            );
            _documentWindow = new DocumentWindow(document, documentWindowFrame);

            _focusedWindow = _documentWindow;
            _focusedWindow.SetFocus(true);
        }

        public void Run()
        {
            Render();

            ProccessInput();
        }

        private void Render()
        {
            RenderToolBar();
            RenderStatusBar();
            _projectWindow.Render();
            _documentWindow.Render();

        }

        private void Update()
        {
            Console.Clear();
            Render();
        }

        private void ProccessInput()
        {
            while (_processing)
            {
                var key = Console.ReadKey(true);
                HandleKey(key);
            }
        }

        private void HandleKey(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    HandleEscapeKey();
                    break;

                case ConsoleKey.Add:
                    HandlePlusKey(key);
                    break;

                case ConsoleKey.Subtract:
                    HandleMinusKey(key);
                    break;

                case ConsoleKey.E:
                    HandleEKey(key);
                    break;

                case ConsoleKey.P:
                    HandlePKey(key);
                    break;
            }

            _focusedWindow?.HandleKey(key);
        }

        private void HandleEscapeKey()
        {
            _processing = false;
        }

        private void HandlePlusKey(ConsoleKeyInfo key)
        {
            if (key.Modifiers == ConsoleModifiers.Control)
            {
                Configure.IncrementFontSize();
                Update();
            }
        }

        private void HandleMinusKey(ConsoleKeyInfo key)
        {
            if (key.Modifiers == ConsoleModifiers.Control)
            {
                Configure.DecrementFontSize();
                Update();
            }
        }

        private void HandleEKey(ConsoleKeyInfo key)
        {
            if (key.Modifiers == ConsoleModifiers.Control)
            {
                SetFocusedWindow(_documentWindow);
                Console.CursorVisible = true;
            }
        }

        private void HandlePKey(ConsoleKeyInfo key)
        {
            if (key.Modifiers == ConsoleModifiers.Control)
            {
                SetFocusedWindow(_projectWindow);
                Console.CursorVisible = false;
            }
        }

        private void SetFocusedWindow(BloopWindow window)
        {
            if (_focusedWindow != null)
            {
                if (_focusedWindow == window)
                    return;

                _focusedWindow.SetFocus(false);
            }
            
            _focusedWindow = window;
            _focusedWindow.SetFocus(true);
        }

        private void RenderToolBar()
        {
            Console.CursorTop = 0;
            Console.CursorLeft = 0;

            Console.ResetColor();
            Console.Write(" File  Edit  Settings");

            Console.CursorLeft = (Console.BufferWidth - 10) / 2;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Ensure UTF-8 encoding for Unicode characters
            Console.Write("▶");

            Console.ResetColor();
            Console.Write(" Run (");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("F5");
            Console.ResetColor();
            Console.Write(")");
        }

        private void RenderStatusBar()
        {
            Console.CursorLeft = 0;
            Console.CursorTop = Console.BufferHeight - 1;

            Console.ResetColor();
            Console.Write(" Help (");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Ctrl");
            Console.ResetColor();
            Console.Write("+");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("H");
            Console.ResetColor();
            Console.Write(")");
        }
    }
}