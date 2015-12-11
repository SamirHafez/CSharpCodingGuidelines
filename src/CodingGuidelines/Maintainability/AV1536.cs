using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CodingGuidelines.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AV1536 : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AV1536";
        internal const string Description = "Always add a default block after the last case in a switch statement";
        internal const string MessageFormat = "Always add a default block after the last case in a switch statement";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.SwitchStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;

            bool? hasDefaultLabel = switchStatement.Sections
                .LastOrDefault()
                ?.Labels
                .OfType<DefaultSwitchLabelSyntax>()
                .Any();

            if (!hasDefaultLabel.GetValueOrDefault())
                context.ReportDiagnostic(Diagnostic.Create(Rule, switchStatement.GetLocation()));
        }
    }
}