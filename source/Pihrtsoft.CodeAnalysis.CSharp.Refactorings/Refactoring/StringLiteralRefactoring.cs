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
    internal static class StringLiteralRefactoring
    {
        public static bool CanConvertStringLiteralToStringEmpty(LiteralExpressionSyntax literalExpression)
        {
            return literalExpression.IsKind(SyntaxKind.StringLiteralExpression)
                && literalExpression.Token.ValueText.Length == 0;
        }

        public static async Task<Document> ConvertStringLiteralToStringEmptyAsync(
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

        public static async Task<Document> ConvertStringLiteralToInterpolatedStringAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var interpolatedString = (InterpolatedStringExpressionSyntax)ParseExpression("$" + literalExpression.ToString())
                .WithTriviaFrom(literalExpression);

            SyntaxNode newRoot = oldRoot.ReplaceNode(literalExpression, interpolatedString);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
