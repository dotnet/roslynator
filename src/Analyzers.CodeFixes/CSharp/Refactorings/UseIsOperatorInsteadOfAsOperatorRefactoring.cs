// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseIsOperatorInsteadOfAsOperatorRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(node);

            AsExpressionInfo asExpressionInfo = SyntaxInfo.AsExpressionInfo(nullCheck.Expression);

            ExpressionSyntax newNode = IsExpression(asExpressionInfo.Expression, asExpressionInfo.Type);

            if (nullCheck.IsCheckingNull)
                newNode = LogicalNotExpression(newNode.WithoutTrivia().Parenthesize()).WithTriviaFrom(newNode);

            newNode = newNode
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }
    }
}
