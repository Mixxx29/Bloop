using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        public bool InFocus => _inFocus;
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
            DrawTopEdge();
            DrawBottomEdge();
            DrawLeftEdge();
            DrawRightEdge();
        }

        private void DrawTopEdge()
        {
            var color = _inFocus ? ConsoleColor.White : ConsoleColor.DarkGray;

            var rect = new Rect()
            {
                X = Left,
                Y = Top,
                Width = Width,
                Height = 1
            };

            var text = new StringBuilder();

            if (_inFocus)
            {
                text.Append($"┏━━ {Title} ");
                text.Append(new string('━', Width - text.Length - 1));
                text.Append('┓');
            }
            else
            {
                text.Append($"┌── {Title} ");
                text.Append(new string('─', Width - text.Length - 1));
                text.Append('┐');
            }

            var data = ImmutableArray.Create(CharInfo.FromText(text.ToString(), color));
            ConsoleManager.Write(data, rect);
        }

        private void DrawBottomEdge()
        {
            var color = _inFocus ? ConsoleColor.White : ConsoleColor.DarkGray;

            var rect = new Rect()
            {
                X = Left,
                Y = Top + Height - 1,
                Width = Width,
                Height = 1
            };

            var text = new StringBuilder();

            if (_inFocus )
            {
                text.Append($"┗━ {Command} ");
                text.Append(new string('━', Width - text.Length - 1));
                text.Append('┛');
            }
            else
            { 
                text.Append($"└─ {Command} ");
                text.Append(new string('─', Width - text.Length - 1));
                text.Append('┘');
            }

            var data = ImmutableArray.Create(CharInfo.FromText(text.ToString(), color));
            ConsoleManager.Write(data, rect);
        }

        private void DrawLeftEdge()
        {
            var character = _inFocus ? '┃' : '│';
            var color = _inFocus ? ConsoleColor.White : ConsoleColor.DarkGray;

            var rect = new Rect()
            {
                X = Left,
                Y = Top + 1,
                Width = 1,
                Height = Height - 2
            };

            var text = new string(character, rect.Height);

            var data = ImmutableArray.Create(CharInfo.FromText(text, color));
            ConsoleManager.Write(data, rect);
        }

        private void DrawRightEdge()
        {
            var character = _inFocus ? '┃' : '│';
            var color = _inFocus ? ConsoleColor.White : ConsoleColor.DarkGray;

            var rect = new Rect()
            {
                X = Left + Width - 1,
                Y = Top + 1,
                Width = 1,
                Height = Height - 2
            };

            var text = new string(character, rect.Height);

            var data = ImmutableArray.Create(CharInfo.FromText(text, color));
            ConsoleManager.Write(data, rect);
        }
    }
}
