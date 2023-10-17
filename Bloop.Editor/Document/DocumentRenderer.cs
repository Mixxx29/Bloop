using Bloop.CodeAnalysis.Syntax;
using Bloop.Editor.Window;
using System.Collections.Immutable;

namespace Bloop.Editor.Document
{
    internal class DocumentRenderer : DocumentSubscriber
    {
        private readonly BloopDocument _document;
        private readonly WindowFrame _frame;
        private readonly ScrollBarUI _scrollBar;
        private readonly int _offset = 6;

        private int _lineOffset;

        public DocumentRenderer(BloopDocument document, WindowFrame frame)
        {
            _document = document;
            _document.Subscribe(this);
            _frame = frame;

            _scrollBar = new ScrollBarUI(_frame);
        }

        public int Offset => _offset;
        public int LineOffset => _lineOffset;

        public int ViewportWidth => _frame.Width - 6;
        public int ViewportHeight => _frame.Height - 4;
        public int VisibleLinesCount => Math.Min(_document.LinesCount, ViewportHeight);

        public bool IsAtBottom => _lineOffset + ViewportHeight == _document.LinesCount;

        public void OnDocumentChanged(int lineIndex)
        {
            RenderDocument(lineIndex);
        }

        public void OnLineChanged(int lineIndex, int charIndex)
        {
            //RenderLine(lineIndex, charIndex);
            RenderDocument(_lineOffset);
        }

        public void Render()
        {
            RenderDocument(_lineOffset);
        }

        private void RenderDocument(int lineIndex)
        {
            UpdateLineOffset();

            var rect = new Rect()
            {
                X = _frame.Left + 2,
                Y = _frame.Top + 2,
                Width = ViewportWidth,
                Height = ViewportHeight,
            };

            var builder = ImmutableArray.CreateBuilder<CharInfo>();
            DrawLines(_lineOffset, builder);

            if (VisibleLinesCount < ViewportHeight)
            {
                builder.AddRange(
                    CharInfo.FromText(
                        new string(' ', ViewportWidth * (ViewportHeight - VisibleLinesCount)),
                        ConsoleColor.White
                    )
                );
            }

            ConsoleManager.Write(builder.ToImmutable(), rect);
            UpdateScrollBar();
        }

        private void UpdateLineOffset()
        {
            if (_document.LinesCount < ViewportHeight)
            {
                _lineOffset = 0;
                return;
            }

            if (_lineOffset > _document.LinesCount - ViewportHeight)
                _lineOffset = _document.LinesCount - ViewportHeight;
        }

        private void RenderLine(int lineIndex, int charIndex)
        {
            var rect = new Rect()
            {
                X = _frame.Left + 2,
                Y = _frame.Top + lineIndex - _lineOffset + 2,
                Width = _frame.Width - 3,
                Height = 1,
            };

            var builder = ImmutableArray.CreateBuilder<CharInfo>();
            DrawLine(lineIndex, builder);

            ConsoleManager.Write(builder.ToImmutable(), rect);
        }

        private void DrawLines(int startIndex, ImmutableArray<CharInfo>.Builder builder)
        {
            var i = _lineOffset;
            while (i - _lineOffset < VisibleLinesCount )
                DrawLine(i++, builder);
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
                    new string(' ', ViewportWidth - _offset - content.Length), 
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

        private void UpdateScrollBar()
        {
            var scrollBarOffset = (float)_lineOffset / _document.LinesCount;
            var scrollBarLength = (float)VisibleLinesCount / _document.LinesCount;
            _scrollBar.Update(scrollBarOffset, scrollBarLength);
        }

        public bool ScrollUp()
        {
            if (_document.LinesCount <= ViewportHeight || 
                _lineOffset == _document.LinesCount - ViewportHeight)
                return false;

            ++_lineOffset;
            RenderDocument(_lineOffset);
            return true;
        }

        public bool ScrollDown()
        {
            if (_document.LinesCount <= ViewportHeight || _lineOffset == 0)
                return false;

            --_lineOffset;
            RenderDocument(_lineOffset);
            return true;
        }
    }
}
