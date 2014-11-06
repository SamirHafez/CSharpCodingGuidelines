using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AV1502 : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        public const string DiagnosticId = "AV1502";
        internal const string Description = "Avoid conditions with double negatives";
        internal const string MessageFormat = "Avoid conditions with double negatives";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest { get { return ImmutableArray.Create(SyntaxKind.LogicalNotExpression); } }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            var logicalNotExpression = (PrefixUnaryExpressionSyntax)node;

            ExpressionSyntax expression = logicalNotExpression.Operand;
            if (logicalNotExpression.Operand is ParenthesizedExpressionSyntax)
                expression = ((ParenthesizedExpressionSyntax)logicalNotExpression.Operand).Expression; 

            if (expression.IsKind(SyntaxKind.NotEqualsExpression))
                addDiagnostic(Diagnostic.Create(Rule, logicalNotExpression.GetLocation()));
        }
    }
}
