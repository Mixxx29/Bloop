using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Popup
{
    internal abstract class PopupWindow
    {
        private ImmutableArray<CharInfo> _originalContent;

        public PopupWindow(int x, int y, int width, int height)
        {
            Bounds = new Rect()
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };

            _originalContent = ConsoleManager.Read(Bounds);
        }

        protected Rect Bounds { get; set; }

        public abstract void Render();

        public virtual void Remove()
        {
            ConsoleManager.Write(_originalContent, Bounds);
        }
    }
}
