// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseEventArgsEmptyRefactoring
    {
        public static void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol eventArgsSymbol)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

            if (objectCreation.ArgumentList?.Arguments.Count == 0
                && objectCreation.Initializer == null)
            {
                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(objectCreation, context.CancellationToken);

                if (typeSymbol?.Equals(eventArgsSymbol) == true)
                    context.ReportDiagnostic(DiagnosticDescriptors.UseEventArgsEmpty, objectCreation);
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ObjectCreationExpressionSyntax objectCreationExpression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            TypeSyntax type = semanticModel
                .GetTypeByMetadataName(MetadataNames.System_EventArgs)
                .ToMinimalTypeSyntax(semanticModel, objectCreationExpression.SpanStart);

            MemberAccessExpressionSyntax newNode = CSharpFactory.SimpleMemberAccessExpression(type, SyntaxFactory.IdentifierName("Empty"))
                .WithTriviaFrom(objectCreationExpression);

            return await document.ReplaceNodeAsync(objectCreationExpression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
