using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class AV1570 : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        public const string DiagnosticId = "AV1570";
        internal const string Description = "Always check the result of an as operation";
        internal const string MessageFormat = "Always check the result of an as operation";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest { get { return ImmutableArray.Create(SyntaxKind.AsExpression); } }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            var asExpression = (BinaryExpressionSyntax)node;

            var identifier = ((VariableDeclaratorSyntax)asExpression.Parent.Parent).Identifier;

            SyntaxNode auxNode = asExpression.Parent;
            while (!(auxNode is BlockSyntax))
                auxNode = auxNode.Parent;

            var parentBlock = (BlockSyntax)auxNode;
            var nextStatements = parentBlock.Statements.Where(s => s.SpanStart > asExpression.Span.End).ToList();

            if (nextStatements.Count == 0)
                addDiagnostic(Diagnostic.Create(Rule, asExpression.GetLocation()));
            else
                foreach (var statement in nextStatements)
                {
                    var nodes = statement.DescendantNodes().
                        OfType<MemberAccessExpressionSyntax>().
                        Where(memberNode => memberNode.Expression is IdentifierNameSyntax && ((IdentifierNameSyntax)memberNode.Expression).Identifier.Text == identifier.Text).
                        ToList();

                    foreach (var memberNode in nodes)
                    {
                        if (!memberNode.Ancestors().
                            Where(ancestorNode => ancestorNode is IfStatementSyntax || ancestorNode is ConditionalExpressionSyntax).
                            Select(expression => expression.GetType().GetProperty("Condition").GetValue(expression)).
                            Cast<BinaryExpressionSyntax>().
                            Any(ancestorBinary => ancestorBinary.IsKind(SyntaxKind.NotEqualsExpression) &&
                                                 (ancestorBinary.Left.IsKind(SyntaxKind.NullLiteralExpression) || ancestorBinary.Right.IsKind(SyntaxKind.NullLiteralExpression) &&
                                                 ((ancestorBinary.Left is IdentifierNameSyntax && ((IdentifierNameSyntax)ancestorBinary.Left).Identifier.Text == identifier.Text) ||
                                                  (ancestorBinary.Right is IdentifierNameSyntax && ((IdentifierNameSyntax)ancestorBinary.Right).Identifier.Text == identifier.Text)))))
                            addDiagnostic(Diagnostic.Create(Rule, memberNode.GetLocation()));
                    }
                }
        }
    }
}
