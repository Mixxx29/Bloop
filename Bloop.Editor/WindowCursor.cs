using System;

namespace Bloop.Editor
{
    internal class WindowCursor
    {
        private readonly int _leftOffset;
        private readonly int _topOffset;

        private int _left;
        private int _top;

        public WindowCursor(int leftBound, int topBound)
        {
            _leftOffset = leftBound;
            _topOffset = topBound;
        }

        public int Left => _left;
        public int Top => _top;

        public void Reset()
        {
            Console.SetCursorPosition(_leftOffset + _left, _topOffset + _top);
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

        public void MoveLeft()
        {
            --_left;
            Reset();
        }

        public void MoveRight()
        {
            ++_left;
            Reset();
        }

        public void ResetLeft()
        {
            _left = 0;
            Reset();
        }

        public void ResetTop()
        {
            _top = 0;
            Reset();
        }
    }
}
