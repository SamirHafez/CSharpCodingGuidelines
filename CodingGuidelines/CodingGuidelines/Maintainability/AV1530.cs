using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AV1530 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1530";
        internal const string Description = "Don’t change a loop variable inside a for or foreach loop";
        internal const string MessageFormat = "Don’t change a loop variable inside a for or foreach loop";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ForStatement | SyntaxKind.ForEachStatement);
        }

        public void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {

        }
    }
} 