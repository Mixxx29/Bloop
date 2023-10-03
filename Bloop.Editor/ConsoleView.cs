using Bloop.CodeAnalysis.Text;
using Bloop.CodeAnalysis;
using System.Collections.Specialized;
using System.Text;

namespace Bloop.Editor
{
    public class ConsoleView : DocumentSubscriber, CompilationSubscriber
    {
        private readonly BloopDocument _document;

        private readonly List<List<string>> _lines = new List<List<string>>();
        private readonly List<List<ConsoleColor>> _colors = new List<List<ConsoleColor>>();

        private int _lastLineDrawn;

        private EvaluationResult _result;
        private Compilation _compilation;

        public ConsoleView(BloopDocument document, Compilation compilation)
        {
            _document = document;
            _document.Subscribe(this);

            _compilation = compilation;
            _compilation.Subscribe(this);
        }

        public void OnDocumentChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Render();
        }

        public void OnLineChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            
        }

        private void Render()
        {
            var cursorLeft = Console.CursorLeft;
            var cursorTop = Console.CursorTop;

            Clear();

            Console.CursorVisible = false;

            Console.CursorLeft = 1;
            Console.CursorTop = _document.Lines.Count + 2;

            if (_result != null && _result.Diagnostics.Any())
                PrintDiagnostics();

            //Console.WriteLine(_compilation.SyntaxTree.Root.ToString());

            /*if (_result.Root != null)
                _result.Root.ToConsole();*/

            _lastLineDrawn = Console.CursorTop;

            Console.CursorLeft = cursorLeft;
            Console.CursorTop = cursorTop;
            Console.CursorVisible = true;
        }

        private void PrintResult()
        {
            if (_result.Value == null)
                return;

            Console.ForegroundColor = ConsoleColor.Blue;
            PrintText($" {_result.Value}");
            Console.ResetColor();
        }

        private void PrintDiagnostics()
        {
            var sourceText = _compilation.SyntaxTree.SourceText;
            foreach (var diagnostic in _result.Diagnostics)
            {
                var lineIndex = sourceText.GetLineIndex(diagnostic.Span.Start);
                var line = sourceText.Lines[lineIndex];
                var lineNumber = lineIndex + 1;
                var errorPosition = diagnostic.Span.Start - line.Start + 1;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($" ({lineNumber}, {errorPosition}): ");
                Console.WriteLine($"{diagnostic}");
                Console.ResetColor();

                var prefixSpan = TextSpan.FromBounds(line.Span.Start, diagnostic.Span.Start);
                var sufixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                var prefix = sourceText.ToString(prefixSpan);
                var error = sourceText.ToString(diagnostic.Span);
                var suffix = sourceText.ToString(sufixSpan);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("  └── ");
                Console.ResetColor();

                Console.Write(prefix);

                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write(error);
                Console.ResetColor();

                Console.WriteLine(suffix);
            }
        }

        public void OnCompile()
        {
            Clear();
            _lastLineDrawn = _document.Lines.Count + 2;
        }

        public void OnPrint(string text)
        {
            var cursorLeft = Console.CursorLeft;
            var cursorTop = Console.CursorTop;

            Console.CursorVisible = false;
            Console.CursorTop = _lastLineDrawn;

            var color = ConsoleColor.Blue;
            Console.ForegroundColor = color;
            PrintText(text);
            Console.ResetColor();

            _lastLineDrawn = Console.CursorTop;

            Console.CursorLeft = cursorLeft;
            Console.CursorTop = cursorTop;
            Console.CursorVisible = true;
        }

        public string OnRead()
        {
            var cursorLeft = Console.CursorLeft;
            var cursorTop = Console.CursorTop;

            Console.CursorVisible = false;
            Console.CursorLeft = 1;
            Console.CursorTop = _lastLineDrawn;
            Console.CursorVisible = true;

            Console.ForegroundColor = ConsoleColor.Cyan;
            var input = Console.ReadLine();
            Console.ResetColor();

            _lastLineDrawn = Console.CursorTop;

            Console.CursorVisible = false;
            Console.CursorLeft = cursorLeft;
            Console.CursorTop = cursorTop;
            Console.CursorVisible = true;

            return input ?? "";
        }

        private void PrintText(string text)
        {
            Console.CursorLeft = 1;

            Console.Write(text);

            var unusedSpaceLength = Console.BufferWidth - Console.CursorLeft;
            var unusedSpace = new string(' ', unusedSpaceLength);
            Console.WriteLine(unusedSpace);
        }

        private void Clear()
        {
            Console.CursorVisible = false;
            var cursorLeft = Console.CursorLeft;
            var cursorTop = Console.CursorTop;

            var start = _document.Lines.Count + 2;

            Console.CursorLeft = 0;
            while (_lastLineDrawn > start)
            {
                Console.CursorTop = _lastLineDrawn--;
                var blankSpace = new string(' ', Console.BufferWidth);
                Console.WriteLine(blankSpace);
            }

            Console.CursorLeft = cursorLeft;
            Console.CursorTop = cursorTop;
            Console.CursorVisible = true;
        }
    }
}
