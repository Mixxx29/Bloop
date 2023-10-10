using System.Collections.ObjectModel;

namespace Bloop.Editor
{
    internal class DocumentLine
    {
        private readonly BloopDocument _document;
        private readonly ObservableCollection<char> _characters;

        public DocumentLine(BloopDocument document, string text) 
            : this(document)
        {
            AddText(0, text);
        }

        public DocumentLine(BloopDocument document)
        {
            _document = document;

            _characters = new ObservableCollection<char>();
            _characters.CollectionChanged += _document.OnLineChanged;
        }

        public int Length => _characters.Count;

        public char GetChar(int index)
        {
            if (index < 0 || index >= _characters.Count)
                return '\0';
                
            return _characters[index];
        }

        internal void AddText(string text)
        {
            AddText(_characters.Count, text);
        }

        internal void AddText(int index, string text)
        {
            foreach (char character in text)
            {
                _characters.Insert(index, character);
            }
        }

        internal string Slice(WindowCursor cursor)
        {
            var slicedText = ToString().Substring(cursor.Left);
            DeleteText(cursor, slicedText.Length);
            return slicedText;
        }

        internal bool DeleteText(WindowCursor cursor, int length = 1)
        {
            if (cursor.Left <= 0 || cursor.Left > _characters.Count - length)
                return false;

            while (length-- > 0)
                _characters.RemoveAt(cursor.Left);

            return true;
        }

        public override string ToString()
        {
            return new string(_characters.ToArray());
        }
    }
}