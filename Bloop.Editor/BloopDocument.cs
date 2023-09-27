using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Bloop.Editor
{
    public class BloopDocument
    {
        private const int _columnOffset = 8;
        private const int _rowOffset = 1;

        private readonly ObservableCollection<DocumentLine> _lines;

        private int _currentLineIndex;

        public BloopDocument()
        {
            _lines = new ObservableCollection<DocumentLine>();
            AddLine("");
        }

        public ObservableCollection<DocumentLine> Lines => _lines;

        public int CurrentLineIndex 
        { 
            get => _currentLineIndex;
            set
            {
                if (_currentLineIndex != value)
                {
                    _currentLineIndex = value;
                    UpdateCursor();
                }
            } 
        }

        public DocumentLine CurrentLine => _lines[_currentLineIndex];

        public delegate void LineChangedHandler(object? sender, NotifyCollectionChangedEventArgs e);
        public event LineChangedHandler? LineChanged;

        public void Subscribe(DocumentSubscriber subscriber)
        {
            Lines.CollectionChanged += subscriber.OnDocumentChanged;
            LineChanged += subscriber.OnLineChanged;
        }

        public void UpdateCursor()
        {
            var column = _columnOffset + CurrentLine.CurrentCharacterIndex;
            var row = _rowOffset + _currentLineIndex;
            Console.SetCursorPosition(column, row);
        }

        internal void NewLine()
        {
            var slicedText = CurrentLine.Slice();
            AddLine(slicedText, CurrentLineIndex + 1);
            ++CurrentLineIndex;
            CurrentLine.CurrentCharacterIndex = 0;
        }

        private void AddLine(string text)
        {
            var line = new DocumentLine(this, text);
            Lines.Add(line);
        }

        private void AddLine(string text, int index)
        {
            var line = new DocumentLine(this, text);
            Lines.Insert(index, line);
        }

        internal void DeleteCharacter()
        {
            if (!CurrentLine.DeleteCharacter())
                JoinWithPreviousLine();
        }

        private void JoinWithPreviousLine()
        {
            if (_currentLineIndex == 0)
            {
                Console.CursorLeft++;
                return;
            }

            var previousLine = _lines[_currentLineIndex - 1];
            var end = previousLine.Length;

            var textToJoin = CurrentLine.ToString();
            previousLine.AddText(textToJoin);
            previousLine.CurrentCharacterIndex = end;
            DeleteLine();
        }

        private void DeleteLine()
        {
            _lines.RemoveAt(CurrentLineIndex--);
        }

        internal void MoveCursorUp()
        {
            if (CurrentLineIndex - 1 < 0)
                return;

            Console.CursorVisible = false;

            var characterIndex = CurrentLine.CurrentCharacterIndex;
            --CurrentLineIndex;
            CurrentLine.SetCharacterIndex(characterIndex);

            Console.CursorVisible = true;
        }

        internal void MoveCursorDown()
        {
            if (CurrentLineIndex + 1 >= _lines.Count)
                return;

            Console.CursorVisible = false;

            var characterIndex = CurrentLine.CurrentCharacterIndex;
            ++CurrentLineIndex;
            CurrentLine.SetCharacterIndex(characterIndex);

            Console.CursorVisible = true;
        }

        internal void MoveCursorRight()
        {
            if (CurrentLine.CurrentCharacterIndex + 1 > CurrentLine.Length)
                return;

            Console.CursorVisible = false;

            CurrentLine.SetCharacterIndex(CurrentLine.CurrentCharacterIndex + 1);
            
            Console.CursorVisible = true;
        }

        internal void MoveCursorLeft()
        {
            if (CurrentLine.CurrentCharacterIndex - 1 < 0)
                return;

            Console.CursorVisible = false;
            
            CurrentLine.SetCharacterIndex(CurrentLine.CurrentCharacterIndex - 1);
            
            Console.CursorVisible = true;
        }

        public void AddText(string text)
        {
            Lines[_currentLineIndex].AddText(text);
        }

        internal void OnLineChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            LineChanged?.Invoke(this, e);
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