

using Bloop.CodeAnalysis;
using Bloop.CodeAnalysis.Syntax;
using Bloop.CodeAnalysis.Text;
using System.Text;

namespace Bloop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();
            var textBuilder = new StringBuilder();

            Compilation? previous = null;

            var currentLineNumber = 0;
            while (true)
            {
                if (textBuilder.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(">                        ");
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"{++currentLineNumber}. ");
                Console.ResetColor();

                var input = Console.ReadLine();
                var isBlank = string.IsNullOrWhiteSpace(input);

                if (textBuilder.Length == 0)
                {
                    if (isBlank)
                    {
                        Console.Write("> ");
                        break;
                    }
                    else if (input == "#showTree")
                    {
                        showTree = !showTree;
                        Console.WriteLine(showTree ? "Show tree enabled" : "Show tree disabled");
                        continue;
                    }
                    else if (input == "#cls")
                    {
                        Console.Clear();
                        continue;
                    }
                    else if (input == "#reset")
                    {
                        previous = null;
                        continue;
                    }
                }

                textBuilder.AppendLine(input);

                if (!isBlank)
                    continue;

                Console.CursorTop--;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(">                        ");

                var text = textBuilder.ToString();
                var syntaxTree = SyntaxTree.Parse(text);
                var compilation = previous == null 
                                    ? new Compilation(syntaxTree)
                                    : previous.ContinueWith(syntaxTree);

                var result = compilation.Evaluate(variables);

                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(syntaxTree.Root);
                    Console.ResetColor();
                }

                if (!result.Diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($" {result.Value}");
                    Console.ResetColor();

                    previous = compilation;
                }
                else
                {
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        var lineIndex = syntaxTree.SourceText.GetLineIndex(diagnostic.Span.Start);
                        var line = syntaxTree.SourceText.Lines[lineIndex];
                        var lineNumber = lineIndex + 1;
                        var errorPosition = diagnostic.Span.Start - line.Start + 1;

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"({lineNumber}, {errorPosition}): ");
                        Console.WriteLine($"{diagnostic}");
                        Console.ResetColor();

                        var prefixSpan = TextSpan.FromBounds(line.Span.Start, diagnostic.Span.Start);
                        var sufixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                        var prefix = syntaxTree.SourceText.ToString(prefixSpan);
                        var error = syntaxTree.SourceText.ToString(diagnostic.Span);
                        var suffix = syntaxTree.SourceText.ToString(sufixSpan);

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" └── ");
                        Console.ResetColor();

                        Console.Write(prefix);

                        Console.ForegroundColor = ConsoleColor.Red;
                        if (error == " ") Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write(error);
                        Console.ResetColor();

                        Console.WriteLine(suffix);
                    }
                }

                textBuilder.Clear();
                Console.ResetColor();
                currentLineNumber = 0;
            }
        }
    }
}