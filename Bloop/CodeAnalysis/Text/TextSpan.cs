namespace Bloop.CodeAnalysis.Text
{
    public struct TextSpan
    {
        public TextSpan(int start, int lenght)
        {
            Start = start;
            Lenght = lenght;
        }

        public int Start { get; }
        public int Lenght { get; }
        public int End => Start + Lenght;

        public static TextSpan FromBounds(int start, int end)
        {
            var lenght = end - start;
            return new TextSpan(start, lenght);
        }
    }
}
