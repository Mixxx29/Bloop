

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
            var leftOffset = 0;
            var topOffset = 1;
            var projectWindowFrame = new WindowFrame(
                "Demo Project",
                "Ctrl+P",
                leftOffset,
                topOffset,
                30,
                Console.BufferHeight - 2
            );
            _projectWindow = new ProjectWindow(projectWindowFrame);

            var document = new BloopDocument();

            leftOffset = 30;
            topOffset = 1;
            var documentWindowFrame = new WindowFrame(
                document.Name,
                "Ctrl+E",
                leftOffset,
                topOffset,
                Console.BufferWidth - leftOffset,
                Console.BufferHeight - 2
            );
            _documentWindow = new DocumentWindow(document, documentWindowFrame);

            _focusedWindow = _documentWindow;
            _focusedWindow.SetFocus(true);
        }

        public void Run()
        {
            RenderToolBar();
            _projectWindow.Render();
            _documentWindow.Render();

            ProccessInput();
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

        private void HandleEKey(ConsoleKeyInfo key)
        {
            if (key.Modifiers == ConsoleModifiers.Control)
            {
                SetFocusedWindow(_documentWindow);
            }
        }

        private void HandlePKey(ConsoleKeyInfo key)
        {
            if (key.Modifiers == ConsoleModifiers.Control)
            {
                SetFocusedWindow(_projectWindow);
            }
        }

        private void SetFocusedWindow(BloopWindow window)
        {
            if (_focusedWindow != null)
                _focusedWindow.SetFocus(false);
            
            _focusedWindow = window;
            _focusedWindow.SetFocus(true);
        }

        private void RenderToolBar()
        {
            Console.CursorLeft = (Console.BufferWidth - 10) / 2;
            Console.CursorTop = 0;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Ensure UTF-8 encoding for Unicode characters
            Console.Write("▶");

            Console.ResetColor();
            Console.Write(" Run (F5)");
        }
    }
}