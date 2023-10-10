

using Bloop.Editor;
using Bloop.CodeAnalysis;
using Bloop.CodeAnalysis.Syntax;
using Bloop.CodeAnalysis.Symbol;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Bloop
{
    public partial class BloopEditor
    {
        private Compilation _compilation;

        private bool _processing = true;

        private DocumentView _documentView;
        private ConsoleView _consoleView;

        private SuggestionGenerator _suggestionGenerator;
        private string? _suggestedText;

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

            _suggestionGenerator = new SuggestionGenerator(document);

            return document;
        }

        private void ProcessDocument(BloopDocument document)
        {
            while (_processing)
            {
                var key = Console.ReadKey(true);
                //SuggestionWindow.Instance.CloseWindow(document);
                HandleKey(key, document);
            }
        }

        private void HandleKey(ConsoleKeyInfo key, BloopDocument document)
        {
            /*if (key.Modifiers == default(ConsoleModifiers))
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
            }*/
        }

        private void HandleEscape(BloopDocument document)
        {
            if (SuggestionWindow.Instance.Visible)
            {
                SuggestionWindow.Instance.CloseWindow(document);
                return;
            }

            _processing = false;
        }

        /*private void HandleEnter(BloopDocument document)
        {
            if (SuggestionWindow.Instance.Visible)
            {
                var suggestion = SuggestionWindow.Instance.Enter();
                if (suggestion != null)
                    document.AddText(suggestion);

                SuggestionWindow.Instance.CloseWindow(document);
                return;
            }

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
            if (SuggestionWindow.Instance.Visible)
            {
                SuggestionWindow.Instance.CloseWindow(document);
            }

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
            if (SuggestionWindow.Instance.Visible)
            {
                var suggestion = SuggestionWindow.Instance.Enter();
                if (suggestion != null)
                    document.AddText(suggestion);

                SuggestionWindow.Instance.CloseWindow(document);
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
            if (SuggestionWindow.Instance.Visible)
            {
                SuggestionWindow.Instance.Up();
                return;
            }

            document.MoveCursorUp();
        }

        private void HandleDownArrow(BloopDocument document)
        {
            if (SuggestionWindow.Instance.Visible)
            {
                SuggestionWindow.Instance.Down();
                return;
            }

            document.MoveCursorDown();
        }

        private void HandleRightArrow(BloopDocument document)
        {
            if (SuggestionWindow.Instance.Visible)
                SuggestionWindow.Instance.CloseWindow(document);

            document.MoveCursorRight();
        }

        private void HandleLeftArrow(BloopDocument document)
        {
            if (SuggestionWindow.Instance.Visible)
                SuggestionWindow.Instance.CloseWindow(document);

            document.MoveCursorLeft();
        }

        private void CompileDocument(BloopDocument document)
        {
            if (SuggestionWindow.Instance.Visible)
                SuggestionWindow.Instance.CloseWindow(document);

            var syntaxTree = SyntaxTree.Parse(document.ToString());
            _compilation.Compile(syntaxTree);
        }

        private void HandleTyping(BloopDocument document, string text)
        {
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
                if (SuggestionWindow.Instance.Visible)
                    SuggestionWindow.Instance.CloseWindow(document);

                SuggestText();
                return;
            }

            document.MoveCursorLeft();
        }

        private void SuggestText()
        {
            var cursorLeft = Console.CursorLeft;

            _suggestionGenerator.Suggest();
            if (_suggestedText == null)
                return;

            Console.CursorVisible = false;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(_suggestedText);
            Console.ResetColor();

            Console.CursorLeft = cursorLeft;
            Console.CursorVisible = true;
        }*/
    }
}