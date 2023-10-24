using System.Collections.ObjectModel;

namespace Bloop.Editor.Model
{
    internal class DocumentLine
    {
        private readonly BloopDocument _document;
        private readonly List<char> _characters;

        public DocumentLine(BloopDocument document, string text)
            : this(document)
        {
            AddText(0, text);
        }

        public DocumentLine(BloopDocument document)
        {
            _document = document;
            _characters = new List<char>();
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
                _characters.Insert(index++, character);
            }
        }

        internal string Slice(int index)
        {
            var slicedText = ToString().Substring(index);
            RemoveText(index, slicedText.Length);
            return slicedText;
        }

        private void RemoveText(int index, int length = 1)
        {
            while (length-- > 0 && _characters.Count - index >= length)
                _characters.RemoveAt(index);
        }

        internal bool RemoveChar(int index)
        {
            if (index == -1)
                return false;

            _characters.RemoveAt(index);
            return true;
        }

        public override string ToString()
        {
            return new string(_characters.ToArray());
        }
    }
}