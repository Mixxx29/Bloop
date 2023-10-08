using Bloop.CodeAnalysis.Text;
using Bloop.CodeAnalysis;
using System.Collections.Specialized;
using System.Text;

namespace Bloop.Editor
{
    public class ConsoleView : DocumentSubscriber, CompilationSubscriber
    {
        private readonly BloopDocument _document;

        private int _lastLineDrawn;

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

        public void OnDocumentChanged()
        {
            Render();
        }

        public void OnLineChanged()
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

            if (_compilation.Diagnostics != null && _compilation.Diagnostics.Any())
                PrintDiagnostics();

            /*if (_compilation.SyntaxTree != null)
                Console.WriteLine(_compilation.SyntaxTree.Root.ToString());*/

            /*if (_compilation.BoundRoot != null)
                _compilation.BoundRoot.ToConsole();*/

            _lastLineDrawn = Console.CursorTop;

            Console.CursorLeft = cursorLeft;
            Console.CursorTop = cursorTop;
            Console.CursorVisible = true;
        }

        private void PrintDiagnostics()
        {
            Console.CursorLeft = 0;

            var sourceText = _compilation.SyntaxTree.SourceText;
            foreach (var diagnostic in _compilation.Diagnostics)
            {
                if (diagnostic.Span.Start == -1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($" (0, 0): ");
                    Console.WriteLine($"{diagnostic}");
                    Console.ResetColor();
                    continue;
                }

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
            Render();
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
