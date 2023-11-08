using Bloop.Editor.Model;
using Bloop.Editor.Popup;
using Bloop.Editor.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Window
{
    internal class ProjectWindow : BloopWindow
    {
        private readonly BloopProject _project;
        private readonly WindowFrame _frame;
        private readonly BloopEditor _editor;
        private readonly ProjectRenderer _renderer;

        private TreeContextMenu? _contextMenu;

        public ProjectWindow(BloopProject project, WindowFrame frame, BloopEditor bloopEditor)
        {
            _project = project;
            _frame = frame;
            _editor = bloopEditor;
            _renderer = new ProjectRenderer(_project, _frame);
        }

        public void HandleKey(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Escape:
                    HandleEscape();
                    break;

                case ConsoleKey.Enter:
                    HandleEnter(keyInfo.Modifiers);
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
            if (_contextMenu != null)
            {
                _contextMenu.Remove();
                _contextMenu = null;
                return;
            }

            _editor.Quit();
        }

        private void HandleEnter(ConsoleModifiers modifiers)
        {
            if (_contextMenu != null)
            {
                _contextMenu.Enter(_renderer.Selected);
                _contextMenu.Remove();
                _contextMenu = null;
                _renderer.Render();
                return;
            }

            if (modifiers == ConsoleModifiers.Control)
            {
                _contextMenu = new TreeContextMenu(
                    _renderer.Selected, 
                    _renderer.ContextMenuLeft, 
                    _renderer.ContextMenuTop, 
                    20, 
                    5
                );
                _contextMenu.Render();
                return;
            }

            if (_renderer.Selected is BloopFolder folder)
            {
                folder.Toggle();
                _renderer.Render();
            }
            else if (_renderer.Selected is BloopDocument document)
            {
                _editor.DisplayDocument(document);
            }
        }

        private void HandleUpArrow()
        {
            if (_contextMenu != null)
            {
                _contextMenu.MoveUp();
                return;
            }

            _renderer.MoveUp();
        }

        private void HandleDownArrow()
        {
            if (_contextMenu != null)
            {
                _contextMenu.MoveDown();
                return;
            }

            _renderer.MoveDown();
        }

        public void Render()
        {
            _frame.Render();
            _renderer.Render();
        }

        public void SetFocus(bool focus)
        {
            _frame.SetFocus(focus);
            _renderer.Render();

            if (focus)
                Console.CursorVisible = false;
        }
    }
}
