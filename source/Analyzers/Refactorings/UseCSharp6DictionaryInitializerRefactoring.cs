// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCSharp6DictionaryInitializerRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SeparatedSyntaxList<ExpressionSyntax> expressions = initializer.Expressions;

            ImplicitElementAccessSyntax implicitElementAccess = ImplicitElementAccess(
                BracketedArgumentList(
                    OpenBracketToken().WithTriviaFrom(initializer.OpenBraceToken),
                    SingletonSeparatedList(Argument(expressions[0]).WithFormatterAnnotation()),
                    CloseBracketToken()));

            AssignmentExpressionSyntax assignment = SimpleAssignmentExpression(
                implicitElementAccess,
                EqualsToken().WithTriviaFrom(initializer.ChildTokens().FirstOrDefault()),
                expressions[1]
                    .AppendTrailingTrivia(initializer.CloseBraceToken.GetLeadingAndTrailingTrivia())
                    .WithFormatterAnnotation());

            SyntaxNode newRoot = root.ReplaceNode(initializer, assignment);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}