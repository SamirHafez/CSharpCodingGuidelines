using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AV1500 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1500";
        internal const string Description = "Methods should not exceed 7 statements";
        internal const string MessageFormat = "Method '{0}' should not exceed 7 statements";
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

            if (methodDeclaration.Body.Statements.Count > 7)
            {
                Diagnostic diagnostic = Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation(), methodDeclaration.Identifier.Text);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
