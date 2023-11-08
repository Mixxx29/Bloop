using Bloop.CodeAnalysis.Binding;
using Bloop.Editor.Model;
using Bloop.Editor.Window;
using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;

namespace Bloop.Editor.View
{
    internal class ProjectRenderer
    {
        private readonly BloopProject _project;
        private readonly WindowFrame _frame;

        private List<BloopModel> _visibleModels;

        private int _selectedIndex;
        private int _contextMenuLeft;

        public ProjectRenderer(BloopProject project, WindowFrame frame)
        {
            _project = project;
            _frame = frame;

            _visibleModels = new List<BloopModel>();
        }

        public int ViewportWidth => _frame.Width - 4;
        public int ViewportHeight => _frame.Height - 4;
        public BloopModel Selected => _visibleModels[_selectedIndex];
        public int ContextMenuLeft => _contextMenuLeft;
        public int ContextMenuTop => _frame.Top + 2 + _selectedIndex;

        public void MoveUp()
        {
            if (_selectedIndex == 0)
                return;

            --_selectedIndex;
            Render();
        }

        public void MoveDown()
        {
            if (_selectedIndex == _visibleModels.Count - 1)
                return;

            ++_selectedIndex;
            Render();
        }

        public void Render()
        {
            var rect = new Rect()
            {
                X = _frame.Left + 2,
                Y = _frame.Top + 2,
                Width = ViewportWidth,
                Height = ViewportHeight,
            };

            _visibleModels.Clear();

            var builder = ImmutableArray.CreateBuilder<CharInfo>();
            Draw(_project, builder);
            Fill(builder);
            ConsoleManager.Write(builder.ToImmutable(), rect);
        }

        private void Draw(
            BloopModel model, 
            ImmutableArray<CharInfo>.Builder builder, 
            string indent = "",
            bool isLast = true)
        {
            var isSelected = _visibleModels.Count == _selectedIndex;

            if (model != _project)
                _visibleModels.Add(model);

            var isFolder = model is BloopFolder;

            var marker = isLast ? "└" : "├";
            if (isFolder)
            {
                marker += ((BloopFolder)model).Collapsed ? "▶─" : "▼─";
            }
            else
            {
                marker += "──";
            }

            var regularColor = _frame.InFocus ? ConsoleColor.White : ConsoleColor.DarkGray;

            var backgroundColor = ConsoleColor.Black;

            var specialColor = isSelected ? ConsoleColor.Black : ConsoleColor.DarkGray;
            if (_frame.InFocus)
            {
                if (isFolder)
                {
                    backgroundColor = isSelected ? ConsoleColor.DarkYellow : ConsoleColor.Black;
                    specialColor = isSelected ? ConsoleColor.White : ConsoleColor.Yellow;
                }
                else
                {
                    backgroundColor = isSelected ? ConsoleColor.Blue : ConsoleColor.Black;
                    specialColor = isSelected ? ConsoleColor.White : ConsoleColor.Cyan;
                }
            }
            else if (isSelected)
            {
                backgroundColor = ConsoleColor.DarkGray;
            }

            if (model != _project)
            {
                builder.AddRange(CharInfo.FromText(indent + marker, regularColor));
                builder.AddRange(CharInfo.FromText(" " + model.Name + " ", specialColor, backgroundColor));

                int count = marker.Length + indent.Length + model.Name.Length + 2;
                builder.AddRange(CharInfo.FromText(new string(' ', ViewportWidth - count)));

                if (isSelected)
                    _contextMenuLeft = _frame.Left + count + 2;
            }

            if (model != _project)
                indent += isLast ? " " : "│";

            if (model is BloopFolder folder)
            {
                if (folder.Collapsed)
                    return;

                var lastChild = folder.GetChildren().LastOrDefault();
                foreach (var child in folder.GetChildren())
                    Draw(child, builder, indent, child == lastChild);
            }
        }

        private void Fill(ImmutableArray<CharInfo>.Builder builder)
        {
            int spaceLeft = ViewportWidth * ViewportHeight - builder.Count;
            builder.AddRange(CharInfo.FromText(new string(' ', spaceLeft)));
        }
    }
}
