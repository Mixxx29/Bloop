

using Bloop.Editor.Document;
using Bloop.Editor.Configuration;
using Bloop.Editor.Window;
using Bloop.Editor;
using System.Collections.Immutable;

namespace Bloop
{
    public class BloopEditor
    {
        private bool _processing = true;

        private BloopWindow _focusedWindow;

        private DocumentWindow _documentWindow;
        private ProjectWindow _projectWindow;

        public BloopEditor()
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
                Settings.IncrementFontSize();
                Update();
            }
        }

        private void HandleMinusKey(ConsoleKeyInfo key)
        {
            if (key.Modifiers == ConsoleModifiers.Control)
            {
                Settings.DecrementFontSize();
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
            var rect = new Rect()
            {
                X = (Console.BufferWidth - 10) / 2,
                Y = 0,
                Width = 10,
                Height = 1
            };

            var builder = ImmutableArray.CreateBuilder<CharInfo>();

            builder.AddRange(CharInfo.FromText("▶", ConsoleColor.Green));
            builder.AddRange(CharInfo.FromText(" Run (", ConsoleColor.White));
            builder.AddRange(CharInfo.FromText("F5", ConsoleColor.Yellow));
            builder.AddRange(CharInfo.FromText(")", ConsoleColor.White));

            ConsoleManager.Write(builder.ToImmutable(), rect);
        }

        private void RenderStatusBar()
        {
            var rect = new Rect()
            {
                X = 1,
                Y = Console.BufferHeight - 1,
                Width = 13,
                Height = 1
            };

            var builder = ImmutableArray.CreateBuilder<CharInfo>();

            builder.AddRange(CharInfo.FromText("Help (", ConsoleColor.White));
            builder.AddRange(CharInfo.FromText("Ctrl", ConsoleColor.Yellow));
            builder.AddRange(CharInfo.FromText("+", ConsoleColor.White));
            builder.AddRange(CharInfo.FromText("H", ConsoleColor.Yellow));
            builder.AddRange(CharInfo.FromText(")", ConsoleColor.White));

            ConsoleManager.Write(builder.ToImmutable(), rect);
        }
    }
}