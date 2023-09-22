using Bloop.CodeAnalysis.Syntax;
using Bloop.CodeAnalysis.Text;
using System.Collections;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Bloop.CodeAnalysis
{
    public sealed class DiagnosticsPool : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Report(TextSpan textSpan, string message)
        {
            var diagnostic = new Diagnostic(textSpan, message);
            _diagnostics.Add(diagnostic);
        }

        public void AddRange(DiagnosticsPool diagnostics)
        {
            _diagnostics.AddRange(diagnostics);
        }

        public void ReportInvalidNumber(TextSpan textSpan, string text, Type type)
        {
            var message = $"The number {text} isn't valid {type}";
            Report(textSpan, message);
        }

        public void ReportInvalidCharacter(int position, char character)
        {
            var textSpan = new TextSpan(position, 1);
            var message = $"Invalid character: {character}";
            Report(textSpan, message);
        }

        internal void ReportUnexpectedToken(TextSpan textSpan, SyntaxType type, SyntaxType expectedType)
        {
            var messsage = $"Unexpected token <{type}>, expected <{expectedType}>";
            Report(textSpan, messsage);
        }

        internal void ReportUndefinedUnaryOperator(TextSpan textSpan, string text, Type operandType)
        {
            var message = $"Unary operator '{text}' is not defined for type {operandType}";
            Report(textSpan, message);
        }

        internal void ReportUndefinedBinaryOperator(TextSpan textSpan, string text, Type firstOperandType, Type secondOperandType)
        {
            var message = $"Binary operator '{text}' is not defined for types {firstOperandType} and {secondOperandType}";
            Report(textSpan, message);
        }

        internal void ReportUndefinedVariable(TextSpan textSpan, string name)
        {
            var message = $"Variable '{name}' is not defined";
            Report(textSpan, message);
        }

        internal void ReportVariableAlreadyDeclared(TextSpan textSpan, string name)
        {
            var message = $"Variable '{name}' is already defined";
            Report(textSpan, message);
        }

        internal void ReportInvalidConversion(TextSpan textSpan, Type type1, Type type2)
        {
            var message = $"Cannot convert type '{type1}' to type '{type2}'";
            Report(textSpan, message);
        }
    }
}
