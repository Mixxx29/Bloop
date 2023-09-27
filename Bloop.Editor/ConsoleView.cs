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

        private ImmutableArray<string> _lines;

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
            var cursorLeft = Console.CursorLeft;
            var cursorTop = Console.CursorTop;

            Console.CursorVisible = false;
            Console.CursorLeft = 0;
            Console.CursorTop = _document.Lines.Count + 2;



            Console.CursorLeft = cursorLeft;
            Console.CursorTop = cursorTop;
            Console.CursorVisible = true;
        }

        public void Print(Compilation compilation)
        {
            var result = compilation.Evaluate();

            if (!result.Diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($" {result.Value}");
                Console.ResetColor();
            }
            else
            {
                var syntaxTree = compilation.SyntaxTree;
                foreach (var diagnostic in result.Diagnostics)
                {
                    var lineIndex = syntaxTree.SourceText.GetLineIndex(diagnostic.Span.Start);
                    var line = syntaxTree.SourceText.Lines[lineIndex];
                    var lineNumber = lineIndex + 1;
                    var errorPosition = diagnostic.Span.Start - line.Start + 1;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($" ({lineNumber}, {errorPosition}): ");
                    Console.WriteLine($"{diagnostic}");
                    Console.ResetColor();

                    var prefixSpan = TextSpan.FromBounds(line.Span.Start, diagnostic.Span.Start);
                    var sufixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                    var prefix = syntaxTree.SourceText.ToString(prefixSpan);
                    var error = syntaxTree.SourceText.ToString(diagnostic.Span);
                    var suffix = syntaxTree.SourceText.ToString(sufixSpan);

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

            Render();
        }
    }
}
