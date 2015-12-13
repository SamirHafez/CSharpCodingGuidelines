using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AV1570 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1570";
        internal const string Description = "Always check the result of an as operation";
        internal const string MessageFormat = "Always check the result of an as operation";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.AsExpression);
        }

        public void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var asExpression = (BinaryExpressionSyntax)context.Node;

            SyntaxToken identifier = ((VariableDeclaratorSyntax)asExpression.Parent.Parent).Identifier;

            SyntaxNode auxNode = asExpression.Parent;
            while (!(auxNode is BlockSyntax))
                auxNode = auxNode.Parent;

            var parentBlock = (BlockSyntax)auxNode;
            IList<StatementSyntax> nextStatements = parentBlock.Statements.Where(s => s.SpanStart > asExpression.Span.End).ToList();

            if (nextStatements.Count == 0)
                return;
            else
                foreach (var statement in nextStatements)
                {
                    if (IsCut(statement, identifier))
                        break;

                    IList<MemberAccessExpressionSyntax> nodes = statement.DescendantNodes().
                        OfType<MemberAccessExpressionSyntax>().
                        Where(memberNode => memberNode.Expression is IdentifierNameSyntax && ((IdentifierNameSyntax)memberNode.Expression).Identifier.Text == identifier.Text).
                        ToList();

                    foreach (var memberNode in nodes)
                    {
                        if (!memberNode.Ancestors().
                            Where(ancestorNode => ancestorNode is IfStatementSyntax || ancestorNode is ConditionalExpressionSyntax).
                            Select(expression => expression.GetType().GetRuntimeProperty("Condition").GetValue(expression)).
                            OfType<BinaryExpressionSyntax>().
                            Any(ancestorBinary => ancestorBinary.IsKind(SyntaxKind.NotEqualsExpression) &&
                                                 (ancestorBinary.Left.IsKind(SyntaxKind.NullLiteralExpression) || ancestorBinary.Right.IsKind(SyntaxKind.NullLiteralExpression) &&
                                                 ((ancestorBinary.Left is IdentifierNameSyntax && ((IdentifierNameSyntax)ancestorBinary.Left).Identifier.Text == identifier.Text) ||
                                                  (ancestorBinary.Right is IdentifierNameSyntax && ((IdentifierNameSyntax)ancestorBinary.Right).Identifier.Text == identifier.Text)))))
                            context.ReportDiagnostic(Diagnostic.Create(Rule, memberNode.GetLocation()));
                    }
                }
        }

        private bool IsCut(StatementSyntax statement, SyntaxToken identifier)
        {
            bool result = false;
            if (statement is IfStatementSyntax)
            {
                var ifStatement = (IfStatementSyntax)statement;
                if (ifStatement.Condition is BinaryExpressionSyntax)
                {
                    BinaryExpressionSyntax condition = (BinaryExpressionSyntax)ifStatement.Condition;

                    if (condition.Left.IsKind(SyntaxKind.NullLiteralExpression) ||
                        condition.Right.IsKind(SyntaxKind.NullLiteralExpression))
                    {
                        if (condition.Left is IdentifierNameSyntax)
                            result = ((IdentifierNameSyntax)condition.Left).Identifier.Text == identifier.Text;
                        else if (condition.Right is IdentifierNameSyntax)
                            result = ((IdentifierNameSyntax)condition.Right).Identifier.Text == identifier.Text;
                    }
                }
            }
            return result;
        }
    }
}
