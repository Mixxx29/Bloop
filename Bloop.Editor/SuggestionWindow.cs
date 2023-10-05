using System.Collections.Immutable;

namespace Bloop.Editor
{
    public sealed class SuggestionWindow
    {
        private static SuggestionWindow? _instance;

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

        public void Create(BloopDocument document, int left, int top, ImmutableArray<string> suggestions)
        {
            CloseWindow(document);

            _left = left;
            _top = top;

            _suggestions = suggestions;
            _selectedIndex = 0;

            Visible = true;
            Render();
        }

        public void CloseWindow(BloopDocument document)
        {
            Visible = false;
            Render();
            _suggestions = ImmutableArray<string>.Empty;
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

            return _suggestions[_selectedIndex];
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
                Console.ForegroundColor = ConsoleColor.Black;

                for (var i = 0; i < _suggestions.Length; i++)
                {
                    var suggestion = _suggestions[i];

                    if (i == _selectedIndex)
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                    else
                        Console.BackgroundColor = ConsoleColor.Gray;

                    Console.CursorLeft = _left;
                    Console.Write($" {suggestion}");
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