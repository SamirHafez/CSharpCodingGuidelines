using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class AV1561 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1561";
        internal const string Description = "Don’t allow methods and constructors with more than three parameters";
        internal const string MessageFormat = "Don’t allow methods and constructors with more than three parameters";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ParameterList);
        }

        public void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var parameterList = context.Node as ParameterListSyntax;

            if (parameterList?.Parameters.Count > 3)
                context.ReportDiagnostic(Diagnostic.Create(Rule, parameterList.GetLocation()));
        }
    }
}
