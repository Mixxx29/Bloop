

using Bloop.Editor;
using Bloop.CodeAnalysis;
using Bloop.CodeAnalysis.Syntax;
using Bloop.CodeAnalysis.Symbol;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace Bloop
{
    public partial class BloopEditor
    {
        private Dictionary<VariableSymbol, object> _variables = new Dictionary<VariableSymbol, object>();

        private Compilation _compilation;

        private bool _processing = true;
        private DocumentView _documentView;
        private ConsoleView _consoleView;
        private string _suggestedText;

        public void Run()
        {
            var document = CreateDocument();
            ProcessDocument(document);
        }

        private BloopDocument CreateDocument()
        {
            var document = new BloopDocument();
            _documentView = new DocumentView(document);

            _compilation = new Compilation();
            _consoleView = new ConsoleView(document, _compilation);

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
                    case ConsoleKey.Escape:
                        HandleEscape(document);
                        break;

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
                //throw new Exception("Invalid input");
            }
        }

        private void HandleEscape(BloopDocument document)
        {
            _processing = false;
        }

        private void HandleEnter(BloopDocument document)
        {
            var currentChar = document.CurrentLine.GetChar();
            var previousChar = document.CurrentLine.GetChar(-1);

            if (currentChar == '}' && previousChar == '{')
            {
                document.NewLine();
                document.NewLine();
                document.MoveCursorUp();
                HandleTab(document);
                return;
            }

            document.NewLine();
        }

        private void HandleBackspace(BloopDocument document)
        {
            document.DeleteCharacter();
        }

        private void HandleTab(BloopDocument document)
        {
            if (_suggestedText != "")
            {
                document.AddText(_suggestedText);
                _suggestedText = "";
                return;
            }
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
            var result = _compilation.Compile(syntaxTree);
        }

        private void HandleTyping(BloopDocument document, string text)
        {
            ClearSuggestedText();

            document.AddText(text);
            if (text == "(")
            {
                document.AddText(")");
            }
            else if (text == "\"")
            {
                document.AddText("\"");
            }
            else if (text == "{")
            {
                document.AddText("}");
            }
            else
            {
                SuggestText(document);
                return;
            }

            document.MoveCursorLeft();
        }

        private void SuggestText(BloopDocument document)
        {
            Console.CursorVisible = false;
            var cursorLeft = Console.CursorLeft;
            var cursorTop = Console.CursorTop;

            var curretLine = document.CurrentLine.ToString();
            MatchForStatement(curretLine);

            if (_suggestedText != "")
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(_suggestedText);
                Console.ResetColor();
            }

            Console.CursorLeft = cursorLeft;
            Console.CursorTop = cursorTop;
            Console.CursorVisible = true;
        }

        private bool MatchForStatement(string line)
        {
            string pattern = @"for\s+i\s*=\s*\d+\s+$";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(line);

            if (match.Success)
            {
                _suggestedText = "to ";
                return true;
            }

            pattern = @"for\s+$";
            regex = new Regex(pattern);
            match = regex.Match(line);

            if (match.Success)
            {
                _suggestedText = "i = ";
                return true;
            }

            return false;
        }

        private void ClearSuggestedText()
        {
            _suggestedText = "";

            Console.CursorVisible = false;
            var cursorLeft = Console.CursorLeft;

            var blankspace = new string(' ', Console.BufferWidth - cursorLeft);
            Console.Write(blankspace);

            Console.CursorLeft = cursorLeft;
            Console.CursorVisible = true;
        }
    }
}