using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AV1502 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1502";
        internal const string Description = "Avoid conditions with double negatives";
        internal const string MessageFormat = "Avoid conditions with double negatives";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest { get { return ImmutableArray.Create(SyntaxKind.LogicalNotExpression); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.LogicalNotExpression);
        }

        public void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var logicalNotExpression = context.Node as PrefixUnaryExpressionSyntax;

            if (logicalNotExpression != null)
            {
                ExpressionSyntax expression = logicalNotExpression.Operand;
                if (logicalNotExpression.Operand is ParenthesizedExpressionSyntax)
                    expression = ((ParenthesizedExpressionSyntax)logicalNotExpression.Operand).Expression;

                if (expression.IsKind(SyntaxKind.NotEqualsExpression))
                    context.ReportDiagnostic(Diagnostic.Create(Rule, logicalNotExpression.GetLocation()));
            }
        }
    }
}
