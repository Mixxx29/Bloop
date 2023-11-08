using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Actions
{
    internal abstract class BloopAction
    {
        internal void SetName(string name)
        {
            Name = name;
        }

        internal void SetShortcut(string shortcut)
        {
            Shortcut = shortcut;
        }

        public string Name { get; private set; }
        public string Shortcut { get; private set; }

        public abstract void Action(object obj);
    }
}
