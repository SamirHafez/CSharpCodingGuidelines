using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace DiagnosticAnalyzerAndCodeFix.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AV1520 : ISyntaxNodeAnalyzer<SyntaxKind>
    {
        public const string DiagnosticId = "AV1520";
        internal const string Description = "Only use var when the type is very obvious";
        internal const string MessageFormat = "Only use var when the type is very obvious";
        internal const string Category = "Maintainability";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest { get { return ImmutableArray.Create(SyntaxKind.VariableDeclaration); } }

        public void AnalyzeNode(SyntaxNode node, SemanticModel semanticModel, Action<Diagnostic> addDiagnostic, AnalyzerOptions options, CancellationToken cancellationToken)
        {
            var b = string.Empty;

            var variableDeclaration = (VariableDeclarationSyntax)node;

            if (!variableDeclaration.Type.IsVar)
                return;

            // IT'S MISSING PREDEFINED TYPES (E.G. int.Parse/string.empy)
            foreach(ExpressionSyntax expression in variableDeclaration.Variables.
                Where(declarator => declarator.Initializer != null &&
                                    declarator.Initializer.Value != null &&
                                    !(declarator.Initializer.Value is ObjectCreationExpressionSyntax) &&
                                    !(declarator.Initializer.Value is CastExpressionSyntax) &&
                                    !(declarator.Initializer.Value is BinaryExpressionSyntax && declarator.Initializer.Value.IsKind(SyntaxKind.AsExpression)) &&
                                    !(declarator.Initializer.Value is LiteralExpressionSyntax)).
                Select(declarator => declarator.Initializer.Value))
            {
                var diagnostic = Diagnostic.Create(Rule, expression.GetLocation());

                addDiagnostic(diagnostic);
            }
        }
    }
}
