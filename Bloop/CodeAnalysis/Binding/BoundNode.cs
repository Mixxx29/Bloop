using System.Collections.Immutable;
using System.Reflection;

namespace Bloop.CodeAnalysis.Binding
{
    public abstract class BoundNode
    {
        public abstract BoundNodeType NodeType { get; }

        public IEnumerable<BoundNode?> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (BoundNode?)property.GetValue(this);
                    yield return child;
                }
                else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<BoundNode>?)property.GetValue(this);
                    if (children == null)
                        continue;

                    foreach (var child in children)
                        yield return child;
                }
            }
        }

        public IEnumerable<(string Name, object Value)> GetProperties()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (property.Name == nameof(NodeType) ||
                    property.Name == nameof(BoundBinaryExpression.Op))
                    continue;

                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType) ||
                    typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                    continue;

                var value = property.GetValue(this);
                if (value != null) 
                    yield return (property.Name, value);
            }
        }

        private void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        private static void PrettyPrint(TextWriter writer, BoundNode? node, string indent = "", bool isLast = true)
        {
            if (node == null)
                return;

            var marker = isLast ? "└──" : "├──";

            writer.Write(indent);
            writer.Write(marker);

            Console.ForegroundColor = ConsoleColor.Green;
            writer.Write(node.GetText() + " ");

            var isFirstPropery = true;
            foreach (var property in node.GetProperties())
            {
                if (isFirstPropery)
                    isFirstPropery = false;
                else
                    writer.Write(", ");

                Console.ForegroundColor = ConsoleColor.Green;
                writer.Write(property.Name);

                Console.ForegroundColor = ConsoleColor.Yellow;
                writer.Write(" = ");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                writer.Write(property.Value);

                Console.ResetColor();
            }



            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (BoundNode? child in node.GetChildren())
            {
                PrettyPrint(writer, child, indent, child == lastChild);
            }
        }

        private string GetText()
        {
            if (this is BoundBinaryExpression binaryExpression)
                return binaryExpression.Op.Type.ToString() + "_EXPRESSION";

            if (this is BoundUnaryExpression unaryExpression)
                return unaryExpression.Op.Type.ToString() + "_EXPRESSION";

            return NodeType.ToString();    
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
