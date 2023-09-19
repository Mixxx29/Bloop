

using Bloop.CodeAnalysis;
using Bloop.CodeAnalysis.Syntax;

namespace Bloop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();

            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;

                if (line == "#showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Show tree enabled" : "Show tree disabled");
                    continue;
                }

                if (line == "#cls")
                {
                    Console.Clear();
                    continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);

                var diagnostics = result.Diagnostics;

                if (showTree)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    PrettyPrint(syntaxTree.Node);
                    Console.ForegroundColor = color;
                }

                if (!diagnostics.Any())
                {
                    Console.WriteLine(result.Value);
                }
                else
                {
                    foreach (var diagnostic in diagnostics)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[Error]: {diagnostic}");
                        Console.ResetColor();

                        var prefix = line.Substring(0, diagnostic.TextSpan.Start);

                        var error = " ";
                        var suffix = "";
                        if (diagnostic.TextSpan.Start < line.Length)
                        {
                            error = line.Substring(diagnostic.TextSpan.Start, diagnostic.TextSpan.Lenght);
                            suffix = line.Substring(diagnostic.TextSpan.End);
                        }

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("   └── ");
                        Console.ResetColor();

                        Console.Write(prefix);

                        Console.ForegroundColor = ConsoleColor.Red;
                        if (error == " ") Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write(error);
                        Console.ResetColor();

                        Console.WriteLine(suffix);
                    }
                }
            }
        }

        private static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Type);

            if (node is SyntaxToken token && token.Value != null)
            {
                Console.Write(" " + token.Value);
            }

            Console.WriteLine();

            indent += isLast ? "   " : "│  ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (SyntaxNode child in node.GetChildren())
            {
                PrettyPrint(child, indent, child == lastChild);
            }
        }
    }
}