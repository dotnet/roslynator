// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class ReplaceStringEmptyWithEmptyStringLiteralRefactoring
    {
        public static bool CanRefactor(
            MemberAccessExpressionSyntax memberAccess,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (memberAccess == null)
                throw new ArgumentNullException(nameof(memberAccess));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (memberAccess.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) != true
                && memberAccess.Expression != null
                && memberAccess.Name?.Identifier.ValueText == "Empty")
            {
                var fieldSymbol = semanticModel
                    .GetSymbolInfo(memberAccess.Name, cancellationToken)
                    .Symbol as IFieldSymbol;

                return fieldSymbol != null
                    && fieldSymbol.IsPublic()
                    && fieldSymbol.IsReadOnly
                    && fieldSymbol.IsStatic
                    && fieldSymbol.ContainingType?.IsString() == true;
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberAccessExpressionSyntax node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            LiteralExpressionSyntax newNode = LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(string.Empty))
                .WithTriviaFrom(node);

            SyntaxNode newRoot = oldRoot.ReplaceNode(node, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
