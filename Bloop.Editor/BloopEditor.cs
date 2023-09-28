

using Bloop.Editor;
using Bloop.CodeAnalysis;
using Bloop.CodeAnalysis.Syntax;
using Bloop.CodeAnalysis.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Bloop
{
    public partial class BloopEditor
    {
        private Dictionary<VariableSymbol, object> _variables = new Dictionary<VariableSymbol, object>();

        private bool _processing = true;
        private DocumentView _documentView;
        private ConsoleView _consoleView;

        public void Run()
        {
            var document = CreateDocument();
            ProcessDocument(document);
        }

        private BloopDocument CreateDocument()
        {
            var document = new BloopDocument();
            _documentView = new DocumentView(document);
            _consoleView = new ConsoleView(document);

            return document;
        }

        private void ProcessDocument(BloopDocument document)
        {
            while (_processing)
            {
                var key = Console.ReadKey();
                HandleKey(key, document);
            }
        }

        private void HandleKey(ConsoleKeyInfo key, BloopDocument document)
        {
            if (key.Modifiers == default(ConsoleModifiers))
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleEnter(document);
                        break;

                    case ConsoleKey.Backspace:
                        HandleBackspace(document);
                        break;

                    case ConsoleKey.Tab:
                        HandleTab(document);
                        break;

                    case ConsoleKey.UpArrow:
                        HandleUpArrow(document);
                        break;

                    case ConsoleKey.DownArrow:
                        HandleDownArrow(document);
                        break;
                    
                    case ConsoleKey.RightArrow:
                        HandleRightArrow(document);
                        break;

                    case ConsoleKey.LeftArrow:
                        HandleLeftArrow(document);
                        break;

                    case ConsoleKey.F5:
                        CompileDocument(document);
                        break;

                    default:
                        HandleTyping(document, key.KeyChar.ToString());
                        break;
                }
            }
            else if (key.Modifiers == ConsoleModifiers.Shift)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Tab:
                        HandleShiftTab(document);
                        break;

                    default:
                        HandleTyping(document, key.KeyChar.ToString());
                        break;
                }
            }
            else
            {
                throw new Exception("Invalid input");
            }
        }

        private void HandleEnter(BloopDocument document)
        {
            document.NewLine();
        }

        private void HandleBackspace(BloopDocument document)
        {
            document.DeleteCharacter();
        }

        private void HandleTab(BloopDocument document)
        {
            document.AddText("    ");
        }

        private void HandleShiftTab(BloopDocument document)
        {
            document.DeleteCharacter();
            document.DeleteCharacter();
            document.DeleteCharacter();
            document.DeleteCharacter();
        }

        private void HandleUpArrow(BloopDocument document)
        {
            document.MoveCursorUp();
        }

        private void HandleDownArrow(BloopDocument document)
        {
            document.MoveCursorDown();
        }

        private void HandleRightArrow(BloopDocument document)
        {
            document.MoveCursorRight();
        }

        private void HandleLeftArrow(BloopDocument document)
        {
            document.MoveCursorLeft();
        }

        private void CompileDocument(BloopDocument document)
        {
            var syntaxTree = SyntaxTree.Parse(document.ToString());
            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate();
            _consoleView.Print(result, syntaxTree);
        }

        private void HandleTyping(BloopDocument document, string text)
        {
            document.AddText(text);
        }
    }
}