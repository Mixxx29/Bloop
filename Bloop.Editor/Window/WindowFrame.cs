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

        public WindowFrame(string title, string command, int left, int top, int width, int height)
        {
            Title = title;
            Command = command;
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public string Title { get; }
        public string Command { get; }
        public int Left { get; }
        public int Top { get; }
        public int Width { get; }
        public int Height { get; }

        internal void SetFocus(bool focus)
        {
            _inFocus = focus;
            Render();
        }

        internal void Render()
        {
            DrawTopEdge();

            for (int i = 1; i < Height - 1; i++)
            {
                DrawLeftEdge();
                DrawRightEdge();
            }

            DrawBottomEdge();
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
