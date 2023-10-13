using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Window
{
    internal interface BloopWindow
    {
        void Render();
        void HandleKey(ConsoleKeyInfo keyInfo);
        void SetFocus(bool focus);
    }
}
