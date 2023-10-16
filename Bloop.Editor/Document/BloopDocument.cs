using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Bloop.Editor.Document
{
    internal class BloopDocument
    {
        private readonly List<DocumentLine> _lines;

        public BloopDocument(string name = "NewBloopDocument")
        {
            Name = name;

            _lines = new List<DocumentLine>();
            AddLine("", 0);
        }

        public string Name { get; }

        public List<DocumentLine> Lines => _lines;
        public int LinesCount => _lines.Count;

        public delegate void DocumentChangedHandler(int lineIndex);
        public event DocumentChangedHandler? DocumentChanged;

        public delegate void LineChangedHandler(int lineIndex, int charIndex);
        public event LineChangedHandler? LineChanged;

        public void Subscribe(DocumentSubscriber subscriber)
        {
            DocumentChanged += subscriber.OnDocumentChanged;
            LineChanged += subscriber.OnLineChanged;
        }

        public void NewLine(int lineIndex, int charIndex)
        {
            var slicedText = _lines[lineIndex].Slice(charIndex);
            AddLine(slicedText, lineIndex + 1);
            DocumentChanged?.Invoke(lineIndex);
        }

        private void AddLine(string text, int index)
        {
            var line = new DocumentLine(this, text);
            Lines.Insert(index, line);
        }

        public void DeleteCharacter(int lineIndex, int charIndex)
        {
            if (_lines[lineIndex].RemoveChar(charIndex))
            {
                LineChanged?.Invoke(lineIndex, charIndex);
                return;
            }

            JoinWithPreviousLine(lineIndex);
            DocumentChanged?.Invoke(lineIndex - 1);
        }

        private void JoinWithPreviousLine(int index)
        {
            if (index == 0)
                return;

            var previousLine = _lines[index - 1];

            var textToJoin = _lines[index].ToString();
            previousLine.AddText(textToJoin);

            DeleteLine(index);
        }

        private void DeleteLine(int index)
        {
            _lines.RemoveAt(index);
        }

        public void AddText(int lineIndex, int charIndex, string text)
        {
            Lines[lineIndex].AddText(charIndex, text);
            LineChanged?.Invoke(lineIndex, charIndex);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var line in _lines)
                builder.AppendLine(line.ToString());

            return builder.ToString();
        }
    }
}