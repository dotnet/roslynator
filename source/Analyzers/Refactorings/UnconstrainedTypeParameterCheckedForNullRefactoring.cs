// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UnconstrainedTypeParameterCheckedForNullRefactoring
    {
        public static void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            Analyze(context, (BinaryExpressionSyntax)context.Node, NullCheckKind.EqualsToNull);
        }

        public static void AnalyzeNotEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            Analyze(context, (BinaryExpressionSyntax)context.Node, NullCheckKind.NotEqualsToNull);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binaryExpression, NullCheckKind allowedKinds)
        {
            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(binaryExpression, allowedKinds: allowedKinds);
            if (nullCheck.Success
                && IsUnconstrainedTypeParameter(context.SemanticModel.GetTypeSymbol(nullCheck.Expression, context.CancellationToken))
                && !binaryExpression.SpanContainsDirectives())
            {
                context.ReportDiagnostic(DiagnosticDescriptors.UnconstrainedTypeParameterCheckedForNull, binaryExpression);
            }
        }

        private static bool IsUnconstrainedTypeParameter(ITypeSymbol typeSymbol)
        {
            return typeSymbol?.IsTypeParameter() == true
                && ((ITypeParameterSymbol)typeSymbol).VerifyConstraint(allowReference: false, allowValueType: false, allowConstructor: true);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            INamedTypeSymbol equalityComparerSymbol = semanticModel
                .GetTypeByMetadataName(MetadataNames.System_Collections_Generic_EqualityComparer_T)
                .Construct(typeSymbol);

            ExpressionSyntax newNode = InvocationExpression(
                SimpleMemberAccessExpression(
                    SimpleMemberAccessExpression(equalityComparerSymbol.ToMinimalTypeSyntax(semanticModel, binaryExpression.SpanStart), IdentifierName("Default")), IdentifierName("Equals")),
                ArgumentList(
                    Argument(binaryExpression.Left.WithoutTrivia()),
                    Argument(DefaultExpression(typeSymbol.ToTypeSyntax()))));

            if (binaryExpression.IsKind(SyntaxKind.NotEqualsExpression))
                newNode = LogicalNotExpression(newNode);

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
