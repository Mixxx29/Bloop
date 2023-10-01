using Bloop.CodeAnalysis.Syntax;
using Bloop.CodeAnalysis.Text;
using Bloop.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace Bloop.Editor
{
    internal class ConsoleView : DocumentSubscriber
    {
        private readonly BloopDocument _document;

        private int _lastLineDrawn;

        private EvaluationResult _result;
        private Compilation _compilation;

        public ConsoleView(BloopDocument document)
        {
            _document = document;
            _document.Subscribe(this);
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
            if (_result == null)
                return;

            var cursorLeft = Console.CursorLeft;
            var cursorTop = Console.CursorTop;

            Clear();

            Console.CursorVisible = false;

            Console.CursorLeft = 0;
            Console.CursorTop = _document.Lines.Count + 2;

            if (!_result.Diagnostics.Any())
                PrintResult();
            else
                PrintDiagnostics();

            if (_result.Root != null)
                _result.Root.ToConsole();

            _lastLineDrawn = Console.CursorTop;

            Console.CursorLeft = cursorLeft;
            Console.CursorTop = cursorTop;
            Console.CursorVisible = true;
        }

        private void PrintResult()
        {
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

        public void Print(EvaluationResult result, Compilation compilation)
        {
            _result = result;
            _compilation = compilation;
            Render();
        }

        private void PrintText(string text)
        {
            Console.CursorLeft = 0;

            Console.Write(text);

            var unusedSpaceLength = Console.BufferWidth - Console.CursorLeft;
            var unusedSpace = new string(' ', unusedSpaceLength);
            Console.WriteLine(unusedSpace);
        }

        private void Clear()
        {
            var start = _document.Lines.Count + 2;
            while (_lastLineDrawn > start)
            {
                Console.CursorTop = _lastLineDrawn--;
                var blankSpace = new string(' ', Console.BufferWidth);
                Console.WriteLine(blankSpace);
            }
        }
    }
}
