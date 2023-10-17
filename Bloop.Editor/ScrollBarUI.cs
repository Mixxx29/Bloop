using Bloop.Editor.Window;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor
{
    internal class ScrollBarUI
    {
        private readonly WindowFrame _frame;

        private float _offset;
        private float _length;

        public ScrollBarUI(WindowFrame frame)
        {
            _frame = frame;
        }

        private int Height => _frame.Height - 2;

        public void Update(float offset, float length)
        {
            _offset = offset;
            _length = length;
            Render();
        }

        private void Render()
        {
            var rect = new Rect()
            {
                X = _frame.Left + _frame.Width - 2,
                Y = _frame.Top + 1,
                Width = 1,
                Height = Height
            };

            var builder = ImmutableArray.CreateBuilder<CharInfo>();

            var barStart = _offset * Height;
            var barEnd = (_offset + _length) * Height;
            for (var i = 0; i < Height; i++)
            {
                if (i < barStart)
                {
                    var value = 1.0f - Math.Min(barStart - i, 1.0f);
                    builder.Add(GetGraphics(value, ConsoleColor.White, ConsoleColor.Black));
                    continue;
                }

                if (i < barEnd)
                {
                    var value = 1.0f - Math.Min(barEnd - i, 1.0f);
                    builder.Add(GetGraphics(value, ConsoleColor.Black, ConsoleColor.White));
                    continue;
                }

                builder.AddRange(CharInfo.FromText(" "));
            }

            ConsoleManager.Write(builder.ToImmutable(), rect);
        }

        private CharInfo GetGraphics(float value, ConsoleColor foreground, ConsoleColor background)
        {
            if (value == 0.0f)
                return CharInfo.FromText(" ", foreground, background)[0];

            if (value <= 0.11f)
                return CharInfo.FromText("▁", foreground, background)[0];

            if (value <= 0.22f)
                return CharInfo.FromText("▂", foreground, background)[0];

            if (value <= 0.33f)
                return CharInfo.FromText("▃", foreground, background)[0];

            if (value <= 0.44f)
                return CharInfo.FromText("▄", foreground, background)[0];

            if (value <= 0.55f)
                return CharInfo.FromText("▅", foreground, background)[0];

            if (value <= 0.66f)
                return CharInfo.FromText("▆", foreground, background)[0];

            if (value <= 0.77f)
                return CharInfo.FromText("▇", foreground, background)[0];

            if (value <= 0.88f)
                return CharInfo.FromText("█", foreground, background)[0];

            return CharInfo.FromText(" ", background, foreground)[0];
        }
    }
}
