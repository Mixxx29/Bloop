using System.Collections.ObjectModel;

namespace Bloop.Editor
{
    public class DocumentLine
    {
        private readonly ObservableCollection<char> _characters;
        private readonly BloopDocument _document;

        private int _currentCharacterIndex;

        public DocumentLine(BloopDocument document, string text) 
            : this(document)
        {
            AddText(text);
        }

        public DocumentLine(BloopDocument document)
        {
            _document = document;

            _characters = new ObservableCollection<char>();
            _characters.CollectionChanged += _document.OnLineChanged;
        }

        public int CurrentCharacterIndex
        {
            get => _currentCharacterIndex;
            set
            {
                if (_currentCharacterIndex != value)
                {
                    _currentCharacterIndex = value;
                    _document.UpdateCursor();
                }
            }
        }

        public int Length => _characters.Count;

        internal void AddText(string text)
        {
            foreach (char character in text)
            {
                _characters.Insert(_currentCharacterIndex++, character);
            }
        }

        internal bool DeleteCharacter()
        {
            if (_currentCharacterIndex == 0)
                return false;

            _characters.RemoveAt(--CurrentCharacterIndex);
            return true;
        }

        public override string ToString()
        {
            return new string(_characters.ToArray());
        }

        internal void SetCharacterIndex(int index)
        {
            if (index < 0)
            {
                index = 0;
            }
            else if (index > _characters.Count)
            {
                index = _characters.Count;
            }

            CurrentCharacterIndex = index;
        }

        internal string Slice()
        {
            var slicedText = ToString().Substring(CurrentCharacterIndex);

            // Remove sliced text
            while (_characters.Count > CurrentCharacterIndex)
                _characters.RemoveAt(_characters.Count - 1);

            return slicedText;
        }
    }
}