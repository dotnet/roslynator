// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class StringEmptyRefactoring
    {
        public static bool CanConvertStringEmptyToEmptyStringLiteral(
            MemberAccessExpressionSyntax node,
            SemanticModel semanticModel)
        {
            var namedTypeSymbol = semanticModel.GetSymbolInfo(node.Expression).Symbol as INamedTypeSymbol;

            if (namedTypeSymbol?.SpecialType == SpecialType.System_String)
            {
                var identifierName = node.Name as IdentifierNameSyntax;

                return identifierName?.Identifier.ValueText == "Empty";
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
