using Bloop.Editor.Configuration;
using Bloop.Editor.Window;
using System.Collections.Immutable;
using Bloop.Editor.Popup;
using Bloop.Editor.Model;

namespace Bloop.Editor
{
    public class BloopEditor
    {
        private bool _processing = true;

        private BloopWindow _focusedWindow;

        private DocumentWindow _documentWindow;
        private ProjectWindow _projectWindow;

        private HelpPopupWindow? _popoup;

        public BloopEditor(string projectPath)
        {
            LoadProject(projectPath);
        }

        private void LoadProject(string projectPath)
        {
            // Load project
            var projectInfo = new FileInfo(projectPath);
            var project = new BloopProject(projectInfo.Name.Split(".")[0], projectInfo.Directory.FullName);
            var projectWindowFrame = new WindowFrame(
                project.Name,
                "Ctrl+P",
                0,
                0,
                0.3f,
                1.0f
            );
            _projectWindow = new ProjectWindow(project, projectWindowFrame, this);

            var documentWindowFrame = new WindowFrame(
                project.FirstDocument().Name,
                "Ctrl+E",
                0.3f,
                0,
                0.7f,
                1.0f
            );
            _documentWindow = new DocumentWindow(project.FirstDocument(), documentWindowFrame);

            _focusedWindow = _documentWindow;
            _focusedWindow.SetFocus(true);
        }

        internal void DisplayDocument(BloopDocument document)
        {
            _documentWindow.DisplayDocument(document);
            SetFocusedWindow(_documentWindow);
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

                case ConsoleKey.H:
                    HandleHKey(key);
                    break;

                case ConsoleKey.S:
                    HandleSKey(key);
                    break;
            }

            _focusedWindow?.HandleKey(key);
        }

        private void HandleEscapeKey()
        {
            if (_popoup != null)
            {
                _popoup.Remove();
                _popoup = null;
                return;
            }

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

        private void HandleHKey(ConsoleKeyInfo key)
        {
            _popoup?.Remove();
            _popoup = new HelpPopupWindow(0, 5, 27, Console.BufferHeight - 5);
            _popoup.Render();
        }

        private void HandleSKey(ConsoleKeyInfo key)
        {
            if (key.Modifiers == ConsoleModifiers.Control)
            {
                _documentWindow.SaveDocument();
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