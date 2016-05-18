// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SwapArgumentsRefactoring
    {
        private static ArgumentSyntax GetArgumentToSwap(CodeRefactoringContext context, ArgumentListSyntax argumentList)
        {
            if (argumentList == null)
                throw new ArgumentNullException(nameof(argumentList));

            if (!context.Span.IsEmpty)
            {
                SeparatedSyntaxList<ArgumentSyntax>.Enumerator en = argumentList.Arguments.GetEnumerator();

                while (en.MoveNext())
                {
                    if (context.Span.Contains(en.Current.Span))
                    {
                        ArgumentSyntax argument = en.Current;

                        if (en.MoveNext()
                            && context.Span.Contains(en.Current.Span)
                            && (!en.MoveNext() || !context.Span.IntersectsWith(en.Current.Span)))
                        {
                            return argument;
                        }

                        break;
                    }
                    else if (context.Span.IntersectsWith(en.Current.Span))
                    {
                        break;
                    }
                }
            }

            return null;
        }

        public static void Refactor(CodeRefactoringContext context, ArgumentListSyntax argumentList)
        {
            ArgumentSyntax argument = GetArgumentToSwap(context, argumentList);

            if (argument != null)
            {
                context.RegisterRefactoring(
                    "Swap arguments",
                    cancellationToken =>
                    {
                        return RefactorAsync(
                            context.Document,
                            argument,
                            argumentList,
                            cancellationToken);
                    });
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ArgumentSyntax argument,
            ArgumentListSyntax argumentList,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            int index = arguments.IndexOf(argument);

            ArgumentSyntax nextArgument = arguments[index + 1]
                .WithTriviaFrom(argument);

            arguments = arguments
                .Replace(argument, nextArgument);

            arguments = arguments
                .Replace(arguments[index + 1], argument.WithTriviaFrom(nextArgument));

            ArgumentListSyntax newNode = argumentList.WithArguments(arguments);

            SyntaxNode newRoot = oldRoot.ReplaceNode(argumentList, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
