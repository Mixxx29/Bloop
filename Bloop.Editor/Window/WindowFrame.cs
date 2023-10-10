using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Window
{
    internal class WindowFrame
    {
        public WindowFrame(string title, int left, int top, int width, int height)
        {
            Title = title;
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public string Title { get; }
        public int Left { get; }
        public int Top { get; }
        public int Width { get; }
        public int Height { get; }

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
            Console.CursorLeft = Left;
            Console.CursorTop = Top;

            Console.Write("┌── " + Title + " ");
            var line = new string('─', Width - (Console.CursorLeft - Left) - 1);
            Console.Write(line);

            if (Console.CursorLeft < Console.BufferWidth - 1)
                Console.WriteLine("┐");
            else
                Console.Write("┐");
        }

        private void DrawBottomEdge()
        {
            Console.CursorLeft = Left;
            Console.CursorTop = Top + Height - 1;
            Console.Write("└");
            var line = new string('─', Width - 2);
            Console.Write(line);
            //Console.Write("┘");
        }

        private void DrawLeftEdge()
        {
            Console.CursorLeft = Left;
            Console.Write("│");
        }

        private void DrawRightEdge()
        {
            Console.CursorLeft = Left + Width - 1;
            if (Console.CursorLeft < Console.BufferWidth - 1)
                Console.WriteLine("│");
            else
                Console.Write("│");
        }
    }
}
