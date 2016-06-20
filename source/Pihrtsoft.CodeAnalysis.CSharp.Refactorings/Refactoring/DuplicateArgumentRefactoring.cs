// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class DuplicateArgumentRefactoring
    {
        public static void Refactor(RefactoringContext context, ArgumentListSyntax argumentList)
        {
            ArgumentSyntax argument = GetArgument(context, argumentList);

            if (argument != null)
            {
                context.RegisterRefactoring(
                    "Duplicate argument",
                    cancellationToken => RefactorAsync(context.Document, argument, cancellationToken));
            }
        }

        private static ArgumentSyntax GetArgument(RefactoringContext context, ArgumentListSyntax argumentList)
        {
            if (context.Span.IsEmpty)
            {
                SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

                foreach (ArgumentSyntax argument in arguments)
                {
                    if (argument.IsMissing
                        && context.Span.Contains(argument.Span))
                    {
                        int index = arguments.IndexOf(argument);

                        if (index > 0
                            && !arguments[index - 1].IsMissing)
                        {
                            return argument;
                        }
                    }
                }
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ArgumentSyntax argument,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var argumentList = (ArgumentListSyntax)argument.Parent;

            int index = argumentList.Arguments.IndexOf(argument);

            ArgumentSyntax previousArgument = argumentList.Arguments[index - 1]
                .WithTriviaFrom(argument);

            SyntaxNode newRoot = oldRoot.ReplaceNode(argument, previousArgument);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
