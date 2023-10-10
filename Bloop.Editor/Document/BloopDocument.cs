using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Bloop.Editor.Document
{
    internal class BloopDocument
    {
        private readonly ObservableCollection<DocumentLine> _lines;

        public BloopDocument(string name = "NewBloopDocument")
        {
            Name = name;

            _lines = new ObservableCollection<DocumentLine>();
            AddLine("");
        }

        public string Name { get; }

        public ObservableCollection<DocumentLine> Lines => _lines;

        public delegate void DocumentChangedHandler(int lineIndex);
        public event DocumentChangedHandler? DocumentChanged;

        public delegate void LineChangedHandler(int charIndex);
        public event LineChangedHandler? LineChanged;

        public void Subscribe(DocumentSubscriber subscriber)
        {
            Lines.CollectionChanged += OnCollectionChanged;
            DocumentChanged += subscriber.OnDocumentChanged;
            LineChanged += subscriber.OnLineChanged;
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                DocumentChanged?.Invoke(e.OldStartingIndex);
                return;
            }

            DocumentChanged?.Invoke(e.NewStartingIndex);
        }

        internal void OnLineChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            LineChanged?.Invoke(e.NewStartingIndex);
        }

        public void NewLine(WindowCursor cursor)
        {
            var slicedText = _lines[cursor.Top].Slice(cursor);
            AddLine(slicedText, cursor.Top + 1);
            cursor.MoveDown();
            cursor.ResetLeft();
        }

        private void AddLine(string text, int index = 0)
        {
            var line = new DocumentLine(this, text);
            Lines.Insert(index, line);
        }

        public void DeleteCharacter(WindowCursor cursor)
        {
            if (!_lines[cursor.Top].DeleteText(cursor))
                JoinWithPreviousLine(cursor);
        }

        private void JoinWithPreviousLine(WindowCursor cursor)
        {
            if (cursor.Top == 0)
                return;

            var previousLine = _lines[cursor.Top - 1];
            var textToJoin = _lines[cursor.Top].ToString();
            previousLine.AddText(textToJoin);

            DeleteLine(cursor.Top);
            cursor.MoveUp();
        }

        private void DeleteLine(int index)
        {
            _lines.RemoveAt(index);
        }

        public void AddText(int index, string text)
        {
            Lines[index].AddText(text);
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