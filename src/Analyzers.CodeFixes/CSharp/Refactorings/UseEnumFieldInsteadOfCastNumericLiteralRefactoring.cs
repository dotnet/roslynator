// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseEnumFieldInsteadOfCastNumericLiteralRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            CastExpressionSyntax castExpression,
            CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
            var namedTypeSymbol = (INamedTypeSymbol)semanticModel.GetTypeSymbol(castExpression.Type, cancellationToken);

            var expression = (LiteralExpressionSyntax)castExpression.Expression;
            var numericLiteralValue = SymbolUtility.GetEnumValueAsUInt64(expression.Token.Value, namedTypeSymbol);

            var enumField = GetEnumFieldWithSpecifiedValue(namedTypeSymbol, numericLiteralValue);

            var newNode = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, castExpression.Type, SyntaxFactory.IdentifierName(enumField.Name))
                .WithOperatorToken(SyntaxFactory.Token(SyntaxKind.DotToken));

            return await document.ReplaceNodeAsync(castExpression, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static EnumFieldSymbolInfo GetEnumFieldWithSpecifiedValue(INamedTypeSymbol enumSymbol, ulong value)
        {
            foreach (var fieldSymbol in enumSymbol.GetMembers().Where(f => f.Kind == SymbolKind.Field).Cast<IFieldSymbol>())
            {
                var fieldInfo = EnumFieldSymbolInfo.Create(fieldSymbol);
                if (fieldInfo.Value == value)
                {
                    return fieldInfo;
                }
            }

            Debug.Fail("Enum must have field of this value as the analyser flagged this");

            return default;
        }
    }
}
