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
    public class AV1532 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1532";
        internal const string Description = "Avoid nested loops";
        internal const string MessageFormat = "Avoid nested loops";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ForStatement);
        }

        public void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            if (node.Ancestors().
                Any(ancestorNode => ancestorNode is ForStatementSyntax || ancestorNode is ForEachStatementSyntax))
                context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
        }
    }
}
