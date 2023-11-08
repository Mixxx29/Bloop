using Bloop.Editor.Window;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Bloop.Editor.Model
{
    internal class BloopDocument : BloopModel
    {
        private readonly List<DocumentLine> _lines;

        public BloopDocument(string name, string path) : base(name, path)
        {
            _lines = new List<DocumentLine>();

            if (File.Exists(Filepath))
            {
                LoadFile();
            }
            else
            {
                AddLine("", 0);
                ToFile();
            }

        }

        public BloopDocument(FileInfo info) : base(info.Name, info.FullName)
        {
            _lines = new List<DocumentLine>();

            if (File.Exists(Filepath))
            {
                LoadFile();
            }
            else
            {
                AddLine("", 0);
                ToFile();
            }

        }

        public string Filepath => Path + "\\" + Name;

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

        internal void Unsubscribe(DocumentSubscriber subscriber)
        {
            DocumentChanged -= subscriber.OnDocumentChanged;
            LineChanged -= subscriber.OnLineChanged;
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

        public override void Save()
        {
            ToFile();
        }

        private void LoadFile()
        {
            try
            {
                string[] linse = File.ReadAllLines(Filepath);
                foreach (var line in linse.Reverse())
                    AddLine(line, 0);
            }
            catch (Exception e)
            {

            }
        }

        private void ToFile()
        {
            using (var stream = File.Create(Filepath))
            {
                var text = ToString();
                var textBytes = Encoding.UTF8.GetBytes(text);
                stream.Write(textBytes, 0, textBytes.Length);
            }
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