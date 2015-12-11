using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Collections.Generic;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AV1530 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1530";
        internal const string Description = "Don’t change a loop variable inside a for or foreach loop";
        internal const string MessageFormat = "Don’t change a loop variable inside a for or foreach loop";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ForStatement);
        }

        public void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode node = context.Node;

            if (node is ForEachStatementSyntax)
            {
                var forEach = (ForEachStatementSyntax)node;
                SyntaxToken identifier = forEach.Identifier;

                if (identifier != null)
                    FindViolatingAssignments(context, forEach, identifier);
            }
            else if (node is ForStatementSyntax)
            {
                var @for = (ForStatementSyntax)node;
                SeparatedSyntaxList<VariableDeclaratorSyntax>? declarationVariables = @for.Declaration?.Variables;

                if (declarationVariables.HasValue)
                    foreach (var declarationVariable in declarationVariables)
                        FindViolatingAssignments(context, @for, declarationVariable.Identifier);
            }
        }

        private static void FindViolatingAssignments(SyntaxNodeAnalysisContext context, SyntaxNode syntaxNode, SyntaxToken identifier)
        {
            IEnumerable<AssignmentExpressionSyntax> assignments = syntaxNode.DescendantNodes().OfType<AssignmentExpressionSyntax>();

            foreach (var violatingAssignment in assignments.Where(ass => ass.Left is IdentifierNameSyntax
                                             && ((IdentifierNameSyntax)ass.Left).Identifier.Value == identifier.Value))
                context.ReportDiagnostic(Diagnostic.Create(Rule, violatingAssignment.GetLocation()));
        }
    }
}