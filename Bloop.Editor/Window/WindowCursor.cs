using System;

namespace Bloop.Editor.Window
{
    internal class WindowCursor
    {
        private int _left;
        private int _top;

        public int Left => _left;
        public int Top => _top;

        public WindowCursor(int left = 0, int top = 0)
        {
            SetPosition(left, top);
        }

        public void SetPosition(int left, int top)
        {
            _left = left;
            _top = top;
            Reset();
        }

        public void Reset()
        {
            Console.SetCursorPosition(_left, _top);
        }

        public void MoveUp()
        {
            --_top;
            Reset();
        }

        public void MoveDown()
        {
            ++_top;
            Reset();
        }

        public void MoveLeft(int amount = 1)
        {
            _left -= amount;
            Reset();
        }

        public void MoveRight(int amount = 1)
        {
            _left += amount;
            Reset();
        }
    }
}
