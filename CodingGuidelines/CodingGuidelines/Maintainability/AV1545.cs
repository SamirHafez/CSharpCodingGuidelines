using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AV1545 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1545";
        internal const string Description = "Don’t use if-else statements instead of a simple (conditional) assignment";
        internal const string MessageFormat = "Don’t use if-else statements instead of a simple (conditional) assignment";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.IfStatement);
        }

        public void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var ifStatementSyntax = context.Node as IfStatementSyntax;

            if (ifStatementSyntax?.Else != null)
            {
                var ifExpression = GetStatementSingle(ifStatementSyntax.Statement);

                var elseExpression = GetStatementSingle(ifStatementSyntax.Else.Statement);

                if (ifExpression != null && elseExpression != null)
                {
                    if (ifExpression is ReturnStatementSyntax && elseExpression is ReturnStatementSyntax)
                        context.ReportDiagnostic(Diagnostic.Create(Rule, ifStatementSyntax.GetLocation()));
                    else if (ifExpression is ExpressionStatementSyntax && elseExpression is ExpressionStatementSyntax)
                    {
                        var ifExpressionStatement = ((ExpressionStatementSyntax)ifExpression).Expression;
                        var elseExpressionStatement = ((ExpressionStatementSyntax)elseExpression).Expression;

                        if (ifExpressionStatement.IsKind(SyntaxKind.SimpleAssignmentExpression) &&
                            elseExpressionStatement.IsKind(SyntaxKind.SimpleAssignmentExpression))
                        {
                            var ifAssignmentVariable = ((BinaryExpressionSyntax)ifExpressionStatement).Left as IdentifierNameSyntax;
                            var elseAssignmentVariable = ((BinaryExpressionSyntax)elseExpressionStatement).Left as IdentifierNameSyntax;

                            if (ifAssignmentVariable != null && elseAssignmentVariable != null &&
                               ifAssignmentVariable.Identifier.Text == elseAssignmentVariable.Identifier.Text)
                                context.ReportDiagnostic(Diagnostic.Create(Rule, ifStatementSyntax.GetLocation()));
                        }
                    }
                }
            }
        }

        private SyntaxNode GetStatementSingle(StatementSyntax statement)
        {
            if (statement is BlockSyntax)
            {
                var block = (BlockSyntax)statement;
                if (block.Statements.Count == 1)
                    return block.Statements[0];

                return null;
            }
            else
                return statement;
        }
    }
}
