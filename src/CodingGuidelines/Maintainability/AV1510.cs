using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AV1510 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1510";
        internal const string Description = "Use using statements instead of fully qualified type name";
        internal const string MessageFormat = "Use using statements instead of fully qualified type name";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.QualifiedName);
        }

        public void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            if(context.Node.Parent is UsingDirectiveSyntax)
                return;

            Diagnostic diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation());

            context.ReportDiagnostic(diagnostic);
        }
    }
}
