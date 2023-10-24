using Bloop.Editor.Window;
using System.Collections.Immutable;

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

            var foregroundColor = _frame.InFocus ? ConsoleColor.White : ConsoleColor.DarkGray;
            var backgroundColor = ConsoleColor.Black;
            var barStart = _offset * Height;
            var barEnd = (_offset + _length) * Height;
            for (var i = 0; i < Height; i++)
            {
                if (i < barStart && _length < 1.0f)
                {
                    var value = 1.0f - Math.Min(barStart - i, 1.0f);
                    builder.Add(GetGraphics(value, foregroundColor, backgroundColor));
                    continue;
                }

                if (i < barEnd && _length < 1.0f)
                {
                    var value = 1.0f - Math.Min(barEnd - i, 1.0f);
                    builder.Add(GetGraphics(value, backgroundColor, foregroundColor));
                    continue;
                }

                builder.AddRange(CharInfo.FromText(" ", foregroundColor, backgroundColor));
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
