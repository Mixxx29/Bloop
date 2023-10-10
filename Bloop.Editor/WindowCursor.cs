using System;

namespace Bloop.Editor
{
    internal class WindowCursor
    {
        private readonly int _leftBound;
        private readonly int _topBound;
        private readonly int _width;
        private readonly int _height;

        private int _left;
        private int _top;

        public WindowCursor(int leftBound, int topBound, int width, int height)
        {
            _leftBound = leftBound;
            _topBound = topBound;
            _width = width;
            _height = height;
        }

        public int Left => _left;
        public int Top => _top;

        public void Reset()
        {
            Console.SetCursorPosition(_leftBound + _left, _topBound + _top);
        }

        public void MoveUp()
        {
            if (_top == 0)
                return;

            --_top;
            Reset();
        }

        public void MoveDown()
        {
            if (_top >= _height)
                return;

            ++_top;
            Reset();
        }

        public void MoveLeft()
        {
            if (_left <= 0)
                return;

            --_left;
            Reset();
        }

        public void MoveRight()
        {
            if (_left >= _width)
                return;

            ++_left;
            Reset();
        }
    }
}
