using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class AV1564 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1564";
        internal const string Description = "Avoid methods that take a bool flag";
        internal const string MessageFormat = "Avoid methods that take a bool flag";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.Parameter);
        }

        public void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var parameter = context.Node as ParameterSyntax;

            if (parameter?.Type is PredefinedTypeSyntax)
            {
                var predefinedType = (PredefinedTypeSyntax)parameter.Type;
                if (predefinedType.Keyword.IsKind(SyntaxKind.BoolKeyword))
                    context.ReportDiagnostic(Diagnostic.Create(Rule, parameter.Type.GetLocation()));
            } 
        }
    }
}
