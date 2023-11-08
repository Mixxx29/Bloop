using Bloop.Editor.Actions;
using Bloop.Editor.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Popup
{
    internal class TreeContextMenu : PopupWindow
    {
        private readonly BloopModel _model;

        private readonly List<BloopAction> _actions;

        private int _selectedIndex;

        public TreeContextMenu(BloopModel model, int x, int y, int width, int height) 
            : base(x, y, width, height)
        {
            _model = model;

            _actions = new List<BloopAction>();

            if (model is BloopFolder folder)
            {
                _actions.Add(new AddFolderAction());
                _actions.Add(new AddFileAction());
            }
            
            _actions.Add(new RenameAction());
            _actions.Add(new DeleteAction());

            Bounds = new Rect()
            {
                X = x,
                Y = y,
                Width = width,
                Height = _actions.Count
            };

            Render();
        }

        public override void Render()
        {
            var builder = ImmutableArray.CreateBuilder<CharInfo>();

            foreach (var action in _actions)
            {
                var isSelected = _actions.IndexOf(action) == _selectedIndex;

                var background = isSelected ? ConsoleColor.DarkGray : ConsoleColor.White;

                builder.AddRange(
                    CharInfo.FromText(" " + action.Name, ConsoleColor.Black, background));

                var innerFill = new string(' ', Bounds.Width - (action.Name.Length + action.Shortcut.Length + 2));
                builder.AddRange(
                    CharInfo.FromText(innerFill, ConsoleColor.Black, background));
                
                builder.AddRange(
                    CharInfo.FromText(action.Shortcut + " ", ConsoleColor.DarkBlue, background));

            }

            var fill = new string(' ', (Bounds.Height - _actions.Count) * Bounds.Width);
            builder.AddRange(
                CharInfo.FromText(fill, ConsoleColor.Black, ConsoleColor.White));

            ConsoleManager.Write(builder.ToImmutable(), Bounds);
        }

        public void MoveUp()
        {
            if (_selectedIndex == 0)
                return;

            --_selectedIndex;
            Render();
        }

        public void MoveDown()
        {
            if (_selectedIndex == _actions.Count - 1)
                return;

            ++_selectedIndex;
            Render();
        }

        internal void Enter(BloopModel selected)
        {
            _actions[_selectedIndex].Action(selected);
        }
    }
}
