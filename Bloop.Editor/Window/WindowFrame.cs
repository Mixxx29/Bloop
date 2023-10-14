using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Window
{
    internal class WindowFrame
    {
        private bool _inFocus;

        private float _left;
        private float _top;
        private float _width;
        private float _height;

        public WindowFrame(string title, string command, float left, float top, float width, float height)
        {
            Title = title;
            Command = command;

            _left = left;
            _top = top;
            _width = width;
            _height = height;
        }

        public string Title { get; }
        public string Command { get; }
        public int Left => (int)(_left * Console.BufferWidth);
        public int Top => (int)(_top * Console.BufferHeight + 1);
        public int Width => (int)(_width * Console.BufferWidth);
        public int Height => (int)(_height * Console.BufferHeight - 2);

        internal void SetFocus(bool focus)
        {
            _inFocus = focus;
            Render();
        }

        internal void Render()
        {
            Console.CursorVisible = false;
            DrawTopEdge();

            for (int i = 1; i < Height - 1; i++)
            {
                DrawLeftEdge();
                DrawRightEdge();
            }

            DrawBottomEdge();
            Console.CursorVisible = true;
        }

        private void DrawTopEdge()
        {
            Console.ResetColor();
            Console.CursorLeft = Left;
            Console.CursorTop = Top;

            if (!_inFocus)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.Write("┌──");

            if (_inFocus)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.Write($" {Title} ");

            Console.ResetColor();
            if (!_inFocus)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            var line = new string('─', Width - (Console.CursorLeft - Left) - 1);
            Console.Write(line);

            if (Console.CursorLeft < Console.BufferWidth - 1)
                Console.WriteLine("┐");
            else
                Console.Write("┐");

            Console.ResetColor();
        }

        private void DrawBottomEdge()
        {
            if (!_inFocus)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.CursorLeft = Left;
            Console.CursorTop = Top + Height - 1;
            Console.Write($"└─ {Command} ");
            var line = new string('─', Width - 5 - Command.Length);
            Console.Write(line);
            Console.Write("┘");
        }

        private void DrawLeftEdge()
        {
            if (!_inFocus)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.CursorLeft = Left;
            Console.Write("│");
        }

        private void DrawRightEdge()
        {
            if (!_inFocus)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.CursorLeft = Left + Width - 1;
            if (Console.CursorLeft < Console.BufferWidth - 1)
                Console.WriteLine("│");
            else
                Console.Write("│");
        }
    }
}
