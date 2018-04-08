// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFacts;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCompoundAssignmentRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            AssignmentExpressionSyntax assignmentExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxToken operatorToken = assignmentExpression.OperatorToken;

            var binaryExpression = (BinaryExpressionSyntax)assignmentExpression.Right;

            SyntaxKind kind = GetCompoundAssignmentKind(binaryExpression.Kind());

            SyntaxTriviaList trailingTrivia = binaryExpression
                .Left
                .DescendantTrivia()
                .Concat(binaryExpression.OperatorToken.LeadingAndTrailingTrivia())
                .ToSyntaxTriviaList()
                .EmptyIfWhitespace();

            AssignmentExpressionSyntax newNode = AssignmentExpression(
                kind,
                assignmentExpression.Left,
                Token(operatorToken.LeadingTrivia, GetCompoundAssignmentOperatorKind(kind), operatorToken.TrailingTrivia.AddRange(trailingTrivia)),
                binaryExpression.Right);

            newNode = newNode.WithFormatterAnnotation();

            return document.ReplaceNodeAsync(assignmentExpression, newNode, cancellationToken);
        }
    }
}
