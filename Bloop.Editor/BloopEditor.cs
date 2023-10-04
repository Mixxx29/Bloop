

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
                var key = Console.ReadKey(true);
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

            CompileDocument(document, false);
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
                if (!document.CurrentLine.ToString().Split(" ")[0].StartsWith("{"))
                {
                    document.MoveCursorLeft();
                    document.NewLine();
                    document.MoveCursorRight();
                }
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
            var currentChar = document.CurrentLine.GetChar();
            var previousChar = document.CurrentLine.GetChar(-1);

            if ((currentChar == ')' && previousChar == '(') ||
                (currentChar == '}' && previousChar == '{') ||
                (currentChar == '"' && previousChar == '"'))
            {
                document.MoveCursorRight();
                document.DeleteCharacter();
            }

            document.DeleteCharacter();
        }

        private void HandleTab(BloopDocument document)
        {
            if (_suggestedText != null && _suggestedText != "")
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

        private void CompileDocument(BloopDocument document, bool invoke = true)
        {
            var syntaxTree = SyntaxTree.Parse(document.ToString());
            var result = _compilation.Compile(syntaxTree, invoke);
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

            Match(document);

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

        private void Match(BloopDocument document)
        {
            var currentLineText = document.CurrentLine.ToString();
            if (currentLineText == null)
                return;

            if (MatchForStatement(currentLineText))
                return;

            var lastWord = currentLineText.Split(" ").Last();
            if (lastWord == null || lastWord == "")
                return;

            if (MatchIdentifiers(lastWord))
                return;
        }

        private bool MatchForStatement(string currentLine)
        {
            string pattern = @"for\s+i\s*=\s*\d+\s+$";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(currentLine);

            if (match.Success)
            {
                _suggestedText = "to ";
                return true;
            }

            pattern = @"for\s+$";
            regex = new Regex(pattern);
            match = regex.Match(currentLine);

            if (match.Success)
            {
                _suggestedText = "i = ";
                return true;
            }

            return false;
        }

        private bool MatchIdentifiers(string lastWord)
        {
            if (MatchVariable(lastWord))
                return true;

            if (MatchKeywords(lastWord))
                return true;

            if (MatchBuiltinFunctions(lastWord))
                return true;

            return false;
        }

        private bool MatchBuiltinFunctions(string lastWord)
        {
            foreach (var function in BuiltinFunctions.GetAll())
            {
                if (function.Name.StartsWith(lastWord))
                {
                    _suggestedText = function.Name.Substring(lastWord.Length) + "()";
                    return true;
                }
            }

            return false;
        }

        private bool MatchVariable(string lastWord)
        {
            if (_compilation.Variables == null)
                return false;

            foreach (var variable in _compilation.Variables.Keys)
            {
                if (variable.Name.StartsWith(lastWord.Trim()))
                {
                    _suggestedText = variable.Name.Substring(lastWord.Trim().Length);
                    return true;
                }
            }

            return false;
        }

        private bool MatchKeywords(string lastWord)
        {
            foreach (var enumValue in Enum.GetValues(typeof(SyntaxType)).Cast<SyntaxType>())
            {
                var enumText = enumValue.ToString();
                if (!enumText.EndsWith("_KEYWORD"))
                    continue;

                var text = SyntaxFacts.GetText(enumValue);
                if (text.StartsWith(lastWord.Trim()))
                {
                    _suggestedText = text.Substring(lastWord.Trim().Length);
                    return true;
                }
            }

            return false;
        }

        private void ClearSuggestedText()
        {
            if (_suggestedText == "")
                return;

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