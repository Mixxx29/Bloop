﻿using Bloop.CodeAnalysis.Symbol;
using Bloop.CodeAnalysis.Syntax;
using Bloop.CodeAnalysis.Text;
using System;
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
            var message = $"Invalid character: '{character}'";
            Report(textSpan, message);
        }

        internal void ReportUnexpectedToken(TextSpan textSpan, SyntaxType type, SyntaxType expectedType)
        {
            var messsage = $"Unexpected token <{type}>, expected <{expectedType}>";
            Report(textSpan, messsage);
        }

        internal void ReportUndefinedUnaryOperator(TextSpan textSpan, string text, TypeSymbol operandType)
        {
            var message = $"Unary operator '{text}' is not defined for type '{operandType}'";
            Report(textSpan, message);
        }

        internal void ReportUndefinedBinaryOperator(TextSpan textSpan, string text, TypeSymbol firstOperandType, TypeSymbol secondOperandType)
        {
            var message = $"Binary operator '{text}' is not defined for types '{firstOperandType}' and '{secondOperandType}'";
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

        internal void ReportInvalidConversion(TextSpan textSpan, TypeSymbol type1, TypeSymbol type2)
        {
            var message = $"Cannot convert type '{type1}' to '{type2}'";
            Report(textSpan, message);
        }

        internal void ReportReadOnly(TextSpan textSpan, string name)
        {
            var message = $"Cannot assign value. Variable '{name}' is read-only";
            Report(textSpan, message);
        }

        internal void ReportMissingExpression(TextSpan textSpan)
        {
            var message = $"Missing expression";
            Report(textSpan, message);
        }

        internal void ReportUnterminatedString(TextSpan textSpan)
        {
            var message = $"Unterminated string literal";
            Report(textSpan, message);
        }

        internal void ReportUndefinedFunction(TextSpan textSpan, string name)
        {
            var message = $"Function '{name}' is not defined";
            Report(textSpan, message);
        }

        internal void ReportInvalidArgumentCount(TextSpan textSpan, string name, int given, int expected)
        {
            var message = $"Function '{name}' requires {expected} arguments, but was given {given}";
            Report(textSpan, message);
        }

        internal void ReportInvalidArgumentType(TextSpan textSpan, string name, TypeSymbol given, TypeSymbol expected)
        {
            var message = $"Argument '{name}' requires value of type {expected}, but was given {given}";
            Report(textSpan, message);
        }

        internal void ReportUnexpectedVoidExpression(TextSpan textSpan)
        {
            var message = "Unexpected void expression";
            Report(textSpan, message);
        }

        internal void ReportUndefinedType(TextSpan textSpan, string name)
        {
            var message = $"Undefined type '{name}'";
            Report(textSpan, message);
        }

        internal void ReportParameterAlreadyDeclared(TextSpan textSpan, string name)
        {
            var message = $"Parameter with name '{name}' is already used";
            Report(textSpan, message);
        }

        internal void ReportFunctionAlreadyDeclared(TextSpan textSpan, string name)
        {
            var message = $"Function with name '{name}' is already defined";
            Report(textSpan, message);
        }

        internal void ReportMainMissing()
        {
            var message = "Main function missing";
            Report(TextSpan.FromBounds(-1, 0), message);
        }

        internal void ReportStatementNotAllowed(TextSpan textSpan)
        {
            var message = "Only variable and function declarations are allowed";
            Report(textSpan, message);
        }
    }
}
