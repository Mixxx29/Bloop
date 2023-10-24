using System.Collections.Immutable;

namespace Bloop.Editor.Popup
{
    internal class HelpPopupWindow : PopupWindow
    {
        private readonly Rect _rect;
        private readonly ImmutableArray<CharInfo> _originalData;

        private readonly ImmutableArray<string> _commands;

        public HelpPopupWindow(int x, int y, int width, int height)
        {
            _rect = new Rect()
            {
                X = x, 
                Y = y,
                Width = width, 
                Height = height
            };

            _commands = ImmutableArray.Create(
                "Run Project - (F5)",
                "Zoom In - (Ctrl+Plus)",
                "Zoom Out - (Ctrl+Minus)",
                "Editor Window - (Ctrl+E)",
                "Project Window - (Ctrl+P)"
            );

            _originalData = ConsoleManager.Read(_rect);
        }

        public void Render()
        {
            var width = 27;
            var builder = ImmutableArray.CreateBuilder<CharInfo>();
            foreach (var command in _commands)
            {
                builder.AddRange(CharInfo.FromText(" ", background: ConsoleColor.White));
                builder.AddRange(CharInfo.FromText(command, ConsoleColor.Black, ConsoleColor.White));
                builder.AddRange(
                    CharInfo.FromText(
                        new string(' ', width - command.Length - 1),
                        background: ConsoleColor.White
                    )
                );
            }

            for (var i = (_commands.Length + 1) * width; i < _rect.Height * _rect.Width; i++)
            {
                builder.AddRange(CharInfo.FromText(" ", background: ConsoleColor.White));
            }

            builder.AddRange(CharInfo.FromText(" ", background: ConsoleColor.White));
            builder.AddRange(CharInfo.FromText("Close - (Esc)", ConsoleColor.Black, ConsoleColor.White));
            builder.AddRange(
                CharInfo.FromText(
                    new string(' ', width - "Close - (Esc)".Length - 1),
                    background: ConsoleColor.White
                )
            );

            ConsoleManager.Write(builder.ToImmutable(), _rect);
        }

        public void Remove()
        {
            ConsoleManager.Write(_originalData, _rect);
        }
    }
}
