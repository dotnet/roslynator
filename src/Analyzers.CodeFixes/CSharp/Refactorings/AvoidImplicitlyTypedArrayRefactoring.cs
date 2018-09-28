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
            ImplicitArrayCreationExpressionSyntax implicitArrayCreation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(implicitArrayCreation, cancellationToken);

            var arrayType = (ArrayTypeSyntax)typeSymbol.ToTypeSyntax().WithSimplifierAnnotation();

            SyntaxToken newKeyword = implicitArrayCreation.NewKeyword;

            if (!newKeyword.HasTrailingTrivia)
                newKeyword = newKeyword.WithTrailingTrivia(SyntaxFactory.Space);

            ArrayCreationExpressionSyntax newNode = SyntaxFactory.ArrayCreationExpression(
                newKeyword,
                arrayType
                    .WithLeadingTrivia(implicitArrayCreation.OpenBracketToken.LeadingTrivia)
                    .WithTrailingTrivia(implicitArrayCreation.CloseBracketToken.TrailingTrivia),
                implicitArrayCreation.Initializer);

            return await document.ReplaceNodeAsync(implicitArrayCreation, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
