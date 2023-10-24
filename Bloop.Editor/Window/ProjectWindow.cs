using Bloop.Editor.Model;
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
        private readonly ProjectRenderer _renderer;

        public ProjectWindow(BloopProject project, WindowFrame frame)
        {
            _project = project;
            _frame = frame;

            _renderer = new ProjectRenderer(_project, _frame);
        }

        public void HandleKey(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    HandleEnter();
                    break;

                case ConsoleKey.UpArrow:
                    HandleUpArrow();
                    break;

                case ConsoleKey.DownArrow:
                    HandleDownArrow();
                    break;
            }
        }

        private void HandleEnter()
        {
            if (_renderer.Selected is BloopFolder folder)
            {
                folder.Toggle();
                _renderer.Render();
            }
        }

        private void HandleUpArrow()
        {
            _renderer.MoveUp();
        }

        private void HandleDownArrow()
        {
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
        }
    }
}
