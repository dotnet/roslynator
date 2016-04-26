// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class EmptyStringRefactoring
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

        public static bool CanConvertEmptyStringLiteralToStringEmpty(LiteralExpressionSyntax literalExpression)
        {
            return literalExpression.IsKind(SyntaxKind.StringLiteralExpression)
                && literalExpression.Token.ValueText.Length == 0;
        }

        public static async Task<Document> ConvertEmptyStringLiteralToStringEmptyAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            MemberAccessExpressionSyntax newNode = MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    PredefinedType(Token(SyntaxKind.StringKeyword)),
                    IdentifierName("Empty"))
                .WithTriviaFrom(literalExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(literalExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
