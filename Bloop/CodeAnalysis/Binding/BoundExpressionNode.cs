namespace Bloop.CodeAnalysis.Binding
{
    internal abstract class BoundExpressionNode : BoundNode
    {
        public abstract Type Type { get; }
    }
}
