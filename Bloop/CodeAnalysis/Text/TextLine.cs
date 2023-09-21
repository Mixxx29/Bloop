namespace Bloop.CodeAnalysis.Text
{
    public sealed class TextLine
    {
        public TextLine(SourceText text, int start, int lenght, int totalLength)
        {
            Text = text;
            Start = start;
            Lenght = lenght;
            TotalLength = totalLength;
        }

        public SourceText Text { get; }
        public int Start { get; }
        public int Lenght { get; }
        public int TotalLength { get; }

        public int End => Start + Lenght;
        public TextSpan Span => new TextSpan(Start, Lenght);
        public TextSpan TotalSpan => new TextSpan(Start, TotalLength);

        public override string ToString() => Text.ToString(Span); 
    }
}
