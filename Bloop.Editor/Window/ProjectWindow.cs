using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Window
{
    internal class ProjectWindow : BloopWindow
    {
        private readonly WindowFrame _frame;

        public ProjectWindow(WindowFrame frame)
        {
            _frame = frame;
        }

        public void HandleKey(ConsoleKeyInfo keyInfo)
        {
            
        }

        public void Render()
        {
            _frame.Render();
        }

        public void SetFocus(bool focus)
        {
            _frame.SetFocus(focus);
        }
    }
}
