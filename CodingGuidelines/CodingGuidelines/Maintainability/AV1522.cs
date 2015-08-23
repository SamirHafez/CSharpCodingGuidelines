using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace CodingGuidelines.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AV1522 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1522";
        internal const string Description = "Assign each variable in a separate statement";
        internal const string MessageFormat = "Assign each variable in a separate statement";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest { get { return ImmutableArray.Create(SyntaxKind.LogicalNotExpression); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SimpleAssignmentExpression);
        }

        public void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var assignmentExpression = context.Node as AssignmentExpressionSyntax;

            if (assignmentExpression?.Right is AssignmentExpressionSyntax)
                context.ReportDiagnostic(Diagnostic.Create(Rule, assignmentExpression.GetLocation()));
        }
    }
} 