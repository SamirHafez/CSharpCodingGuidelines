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
    public class AV1532 : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        public const string DiagnosticId = "AV1532";
        internal const string Description = "Avoid nested loops";
        internal const string MessageFormat = "Avoid nested loops";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest { get { return ImmutableArray.Create(SyntaxKind.ForEachStatement, SyntaxKind.ForStatement); } }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            if (node.Ancestors().
                Any(ancestorNode => ancestorNode is ForStatementSyntax || ancestorNode is ForEachStatementSyntax))
                addDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
        }
    }
}
