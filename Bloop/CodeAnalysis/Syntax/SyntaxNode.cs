using System.Reflection;
using Bloop.CodeAnalysis.Text;

namespace Bloop.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxType Type { get; }

        public virtual TextSpan Span
        { 
            get
            {
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }

        public IEnumerable<SyntaxNode?> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (SyntaxNode?) property.GetValue(this);
                    yield return child;
                }
                else if (typeof(SeparatedSyntaxList).IsAssignableFrom(property.PropertyType))
                {
                    var list = (SeparatedSyntaxList?)property.GetValue(this);
                    if (list == null)
                        continue;

                    foreach (var child in list.GetAll())
                        yield return child;
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>?)property.GetValue(this);
                    if (children == null)
                        continue;

                    foreach (var child in children)
                        yield return child;
                }
            }
        }

        private void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        private void PrettyPrint(TextWriter writer, SyntaxNode? node, string indent = "", bool isLast = true)
        {
            if (node == null)
                return;

            var marker = isLast ? "└──" : "├──";

            writer.Write(indent);
            writer.Write(marker);
            writer.Write(node.Type);

            if (node is SyntaxToken token && token.Value != null)
            {
                writer.Write($" '{token.Value}'");
            }

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (SyntaxNode? child in node.GetChildren())
            {
                PrettyPrint(writer, child, indent, child == lastChild);
            }
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}
