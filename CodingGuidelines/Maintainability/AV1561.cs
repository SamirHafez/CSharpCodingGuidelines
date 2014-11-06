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
    class AV1561 : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        public const string DiagnosticId = "AV1561";
        internal const string Description = "Don’t allow methods and constructors with more than three parameters";
        internal const string MessageFormat = "Don’t allow methods and constructors with more than three parameters";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest { get { return ImmutableArray.Create(SyntaxKind.ParameterList); } }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            var parameterList = (ParameterListSyntax)node;

            if (parameterList.Parameters.Count > 3)
                addDiagnostic(Diagnostic.Create(Rule, parameterList.GetLocation()));
        }
    }
}
