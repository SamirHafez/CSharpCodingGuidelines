using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.Collections.Generic;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AV1540 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1540";
        internal const string Description = "Be reluctant with multiple return statements";
        internal const string MessageFormat = "Be reluctant with multiple return statements";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }

        public void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            IList<SyntaxNode> returnStatements = methodDeclaration.Body.DescendantNodes().
                Where(n => n.IsKind(SyntaxKind.ReturnStatement)).
                ToList();

            if(returnStatements.Count > 1)
                foreach(var returnStatement in returnStatements)
                    context.ReportDiagnostic(Diagnostic.Create(Rule, returnStatement.GetLocation()));
        }
    }
}
