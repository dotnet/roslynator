// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class StringEmptyRefactoring
    {
        public static bool CanConvertStringEmptyToEmptyStringLiteral(
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
                    && fieldSymbol.DeclaredAccessibility == Accessibility.Public
                    && fieldSymbol.IsReadOnly
                    && fieldSymbol.IsStatic
                    && fieldSymbol.ContainingType?.SpecialType == SpecialType.System_String;
            }

            return false;
        }

        public static async Task<Document> ConvertStringEmptyToEmptyStringLiteralAsync(
            Document document,
            MemberAccessExpressionSyntax node,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            LiteralExpressionSyntax newNode = LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(string.Empty))
                .WithTriviaFrom(node);

            SyntaxNode newRoot = oldRoot.ReplaceNode(node, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
