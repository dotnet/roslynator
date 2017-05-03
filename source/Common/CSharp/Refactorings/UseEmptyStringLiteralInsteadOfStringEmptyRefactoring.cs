// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseEmptyStringLiteralInsteadOfStringEmptyRefactoring
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

            if (!memberAccess.IsParentKind(SyntaxKind.SimpleMemberAccessExpression)
                && memberAccess.Expression != null
                && memberAccess.Name?.Identifier.ValueText == "Empty")
            {
                var fieldSymbol = semanticModel.GetSymbol(memberAccess.Name, cancellationToken) as IFieldSymbol;

                return fieldSymbol?.IsPublic() == true
                    && fieldSymbol.IsReadOnly
                    && fieldSymbol.IsStatic
                    && fieldSymbol.ContainingType?.IsString() == true;
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberAccessExpressionSyntax node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            LiteralExpressionSyntax newNode = CSharpFactory.StringLiteralExpression("")
                .WithTriviaFrom(node);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }
    }
}
