// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidImplicitlyTypedArrayRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ImplicitArrayCreationExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            var arrayType = (ArrayTypeSyntax)typeSymbol.ToMinimalTypeSyntax(semanticModel, expression.SpanStart);

            SyntaxToken newKeyword = expression.NewKeyword;

            if (!newKeyword.HasTrailingTrivia)
                newKeyword = newKeyword.WithTrailingTrivia(SyntaxFactory.Space);

            ArrayCreationExpressionSyntax newNode = SyntaxFactory.ArrayCreationExpression(
                newKeyword,
                arrayType
                    .WithLeadingTrivia(expression.OpenBracketToken.LeadingTrivia)
                    .WithTrailingTrivia(expression.CloseBracketToken.TrailingTrivia),
                expression.Initializer);

            newNode = newNode.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(expression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
