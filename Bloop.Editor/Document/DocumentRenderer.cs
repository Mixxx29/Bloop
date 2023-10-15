using Bloop.CodeAnalysis.Syntax;
using Bloop.Editor.Window;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Bloop.Editor.Document
{
    internal class DocumentRenderer : DocumentSubscriber
    {
        private readonly BloopDocument _document;
        private readonly WindowFrame _frame;
        private readonly int _offset = 6;

        public DocumentRenderer(BloopDocument document, WindowFrame frame)
        {
            _document = document;
            _document.Subscribe(this);

            _frame = frame;
        }

        public int Offset => _offset;

        public void OnDocumentChanged(int lineIndex)
        {
            RenderDocument(lineIndex);
        }

        public void OnLineChanged(int lineIndex, int charIndex)
        {
            RenderLine(lineIndex, charIndex);
        }

        public void Render()
        {
            RenderDocument(0);
        }

        private void RenderDocument(int lineIndex)
        {
            var rect = new Rect()
            {
                X = _frame.Left + 2,
                Y = _frame.Top + lineIndex + 2,
                Width = _frame.Width - 3,
                Height = _document.Lines.Count - lineIndex + 1,
            };

            var builder = ImmutableArray.CreateBuilder<CharInfo>();
            DrawLines(lineIndex, builder);
            ConsoleManager.Write(builder.ToImmutable(), rect);
        }

        private void RenderLine(int lineIndex, int charIndex)
        {
            var rect = new Rect()
            {
                X = _frame.Left + 2,
                Y = _frame.Top + lineIndex + 2,
                Width = _frame.Width - 3,
                Height = 1,
            };

            var builder = ImmutableArray.CreateBuilder<CharInfo>();
            DrawLine(lineIndex, builder);

            ConsoleManager.Write(builder.ToImmutable(), rect);
        }

        private void DrawLines(int startIndex, ImmutableArray<CharInfo>.Builder builder)
        {
            for (var i = startIndex; i < _document.Lines.Count; i++)
                DrawLine(i, builder);

            builder.AddRange(
                CharInfo.FromText(" ".PadRight(_frame.Width - 3), ConsoleColor.White)
            );
        }

        private void DrawLine(int lineIndex, ImmutableArray<CharInfo>.Builder builder)
        {
            DrawLineNumber(lineIndex + 1, builder);
            DrawLineContent(lineIndex, builder);
        }

        private void DrawLineNumber(int lineNumber, ImmutableArray<CharInfo>.Builder builder)
        {
            builder.AddRange(CharInfo.FromText(lineNumber.ToString().PadRight(_offset), ConsoleColor.DarkGray));
        }

        private void DrawLineContent(int lineIndex, ImmutableArray<CharInfo>.Builder builder)
        {
            var content = _document.Lines[lineIndex].ToString();
            var tokens = SyntaxTree.ParseTokens(content);

            foreach (var token in tokens)
                DrawToken(token, builder);

            builder.AddRange(
                CharInfo.FromText(
                    "".PadRight(_frame.Width - _offset - content.Length - 3), 
                    ConsoleColor.White
                )
            );
        }

        private void DrawToken(SyntaxToken token, ImmutableArray<CharInfo>.Builder builder)
        {
            var color = _frame.InFocus 
                ? SyntaxFacts.GetColor(token.Type) 
                : ConsoleColor.DarkGray;

            builder.AddRange(CharInfo.FromText(token.Text, color));
        }
    }
}
