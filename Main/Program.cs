

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

            var currentLineNumber = 0;
            while (true)
            {
                if (textBuilder.Length == 0)
                {
                    Console.WriteLine(">");
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
                }

                textBuilder.AppendLine(input);
                var text = textBuilder.ToString();

                var syntaxTree = SyntaxTree.Parse(text);

                if (!isBlank)
                    continue;

                Console.CursorTop--;
                Console.WriteLine("> ");

                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);

                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(syntaxTree.Node);
                    Console.ResetColor();
                }

                if (!result.Diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($" {result.Value}");
                    Console.ResetColor();
                }
                else
                {
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        var lineIndex = syntaxTree.SourceText.GetLineIndex(diagnostic.TextSpan.Start);
                        var line = syntaxTree.SourceText.Lines[lineIndex];
                        var lineNumber = lineIndex + 1;
                        var errorPosition = diagnostic.TextSpan.Start - line.Start + 1;

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"({lineNumber}, {errorPosition}): ");
                        Console.WriteLine($"{diagnostic}");
                        Console.ResetColor();

                        var prefixSpan = TextSpan.FromBounds(line.Span.Start, diagnostic.TextSpan.Start);
                        var sufixSpan = TextSpan.FromBounds(diagnostic.TextSpan.End, line.End);

                        var prefix = syntaxTree.SourceText.ToString(prefixSpan);
                        var error = syntaxTree.SourceText.ToString(diagnostic.TextSpan);
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
                currentLineNumber = 0;
            }
        }
    }
}