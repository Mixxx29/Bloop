using System.Collections;
using System.Collections.Immutable;

namespace Bloop.CodeAnalysis.Syntax
{
    public abstract class SeparatedSyntaxList
    {
        public abstract ImmutableArray<SyntaxNode> GetAll();
    }

    public sealed class SeparatedSyntaxList<T> : SeparatedSyntaxList, IEnumerable<T>
        where T : SyntaxNode
    {
        private readonly ImmutableArray<SyntaxNode> _nodes;

        public SeparatedSyntaxList(ImmutableArray<SyntaxNode> nodes)
        {
            _nodes = nodes;
        }

        public int Count => (_nodes.Length + 1) / 2;

        public T this[int index] => (T) _nodes[index * 2];

        public SyntaxToken GetSeparator(int index) => (SyntaxToken)_nodes[index + 1 * 2];

        public override ImmutableArray<SyntaxNode> GetAll() => _nodes;

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
