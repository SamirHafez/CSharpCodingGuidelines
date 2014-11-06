using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    class AV1564 : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        public const string DiagnosticId = "AV1564";
        internal const string Description = "Avoid methods that take a bool flag";
        internal const string MessageFormat = "Avoid methods that take a bool flag";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest { get { return ImmutableArray.Create(SyntaxKind.Parameter); } }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            var parameter = (ParameterSyntax)node;

            if (parameter.Type is PredefinedTypeSyntax)
            {
                var predefinedType = (PredefinedTypeSyntax)parameter.Type;
                if (predefinedType.Keyword.IsKind(SyntaxKind.BoolKeyword))
                    addDiagnostic(Diagnostic.Create(Rule, parameter.Type.GetLocation()));
            }

        }
    }
}
