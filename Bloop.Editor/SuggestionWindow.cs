using Bloop.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace Bloop.Editor
{
    public sealed class SuggestionWindow
    {
        private static SuggestionWindow? _instance;

        private SyntaxToken? _token;

        private int _left;
        private int _top;

        private ImmutableArray<string> _suggestions;
        private int _selectedIndex;

        private SuggestionWindow()
        {
            Visible = false;
        }

        public static SuggestionWindow Instance
        {
            get
            {
                _instance ??= new SuggestionWindow();
                return _instance;
            }
        }

        public bool Visible { get; private set; }

        public void Create(BloopDocument document, SyntaxToken token, ImmutableArray<string> suggestions)
        {
            CloseWindow(document);

            _token = token;

            _left = 8 + document.CurrentLine.CurrentCharacterIndex - token.Text.Length;
            _top = Console.CursorTop + 1;

            _selectedIndex = 0;
            _suggestions = suggestions;

            Visible = true;
            Render();
        }

        public void CloseWindow(BloopDocument document)
        {
            Visible = false;
            Render();
            _suggestions = ImmutableArray<string>.Empty;
            _token = null;
            document.Update();
        }

        public void Up()
        {
            --_selectedIndex;
            if (_selectedIndex < 0)
                _selectedIndex = _suggestions.Length - 1;

            Render();
        }

        public void Down()
        {
            ++_selectedIndex;
            if (_selectedIndex >= _suggestions.Length)
                _selectedIndex = 0;

            Render();
        }

        public string? Enter()
        {
            if (_suggestions == null || !_suggestions.Any())
                return null;

            return _suggestions[_selectedIndex].Substring(_token.Text.Length);
        }

        private void Render()
        {
            if (_suggestions == null) 
                return;

            var oldCursorLeft = Console.CursorLeft;
            var oldCursorTop = Console.CursorTop;

            Console.CursorTop = _top;

            int width = GetLongestSuggestion() + 2;

            if (Visible)
            {
                for (var i = 0; i < _suggestions.Length; i++)
                {
                    var suggestion = _suggestions[i];

                    if (i == _selectedIndex)
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                    else
                        Console.BackgroundColor = ConsoleColor.Gray;


                    Console.CursorLeft = _left;
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.Write(" " + _token.Text);
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(suggestion.Substring(_token.Text.Length));
                    Console.Write(new string(' ', width - suggestion.Length - 1));
                    Console.CursorTop++;
                }

                Console.ResetColor();
            }
            else
            {
                foreach (var suggestion in _suggestions)
                {
                    Console.CursorLeft = _left;
                    Console.Write(new string(' ', width));
                    Console.CursorTop++;
                }
            }


            Console.CursorLeft = oldCursorLeft;
            Console.CursorTop = oldCursorTop;
        }

        private int GetLongestSuggestion()
        {
            var longestLength = 0;
            foreach (var suggestion in _suggestions)
            {
                if (suggestion.Length > longestLength)
                    longestLength = suggestion.Length;
            }

            return longestLength;
        }
    }
}