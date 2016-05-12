// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class ArgumentRefactoring
    {
        private static readonly SymbolDisplayFormat _symbolDisplayFormat = new SymbolDisplayFormat(
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers,
            parameterOptions: SymbolDisplayParameterOptions.IncludeName);

        public static void AddOrRemoveArgumentName(
            CodeRefactoringContext context,
            ArgumentSyntax argument,
            SemanticModel semanticModel)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (argument.NameColon == null)
            {
                IParameterSymbol parameterSymbol = argument.DetermineParameter(
                    semanticModel,
                    allowParams: false,
                    cancellationToken: context.CancellationToken);

                if (parameterSymbol != null)
                {
                    context.RegisterRefactoring(
                        "Add parameter name",
                        cancellationToken => AddParameterNameAsync(context.Document, argument, parameterSymbol, cancellationToken));
                }
            }
            else if (!argument.NameColon.IsMissing)
            {
                context.RegisterRefactoring(
                    "Remove parameter name",
                    cancellationToken => RemoveParameterNameAsync(context.Document, argument, cancellationToken));
            }
        }

        public static void AddOrRemoveArgumentName(
            CodeRefactoringContext context,
            ArgumentListSyntax argumentList,
            SemanticModel semanticModel)
        {
            if (argumentList == null)
                throw new ArgumentNullException(nameof(argumentList));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (argumentList.Arguments.Count > 1)
            {
                if (argumentList.Arguments.Any(f => f.NameColon == null))
                {
                    context.RegisterRefactoring(
                        "Add parameter name to each argument",
                        cancellationToken =>
                        {
                            return AddParameterNameToEachArgumentAsync(
                                context.Document,
                                argumentList,
                                semanticModel,
                                cancellationToken);
                        });
                }
                else if (argumentList.Arguments.All(f => f.NameColon != null))
                {
                    context.RegisterRefactoring(
                        "Remove parameter name from each argument",
                        cancellationToken =>
                        {
                            return RemoveParameterNameFromEachArgumentAsync(
                                context.Document,
                                argumentList,
                                cancellationToken);
                        });
                }
            }
        }

        private static async Task<Document> AddParameterNameAsync(
            Document document,
            ArgumentSyntax argument,
            IParameterSymbol parameterSymbol,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ArgumentSyntax newNode = argument
                .WithNameColon(NameColon(parameterSymbol.ToDisplayString(_symbolDisplayFormat)))
                .WithTriviaFrom(argument)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(argument, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> RemoveParameterNameAsync(
            Document document,
            ArgumentSyntax argument,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ArgumentSyntax newArgument = argument
                .WithNameColon(null)
                .WithTriviaFrom(argument)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(argument, newArgument);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> AddParameterNameToEachArgumentAsync(
            Document document,
            ArgumentListSyntax argumentList,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ArgumentListSyntax newArgumentList = AddParameterNameSyntaxRewriter.VisitNode(argumentList, semanticModel)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(argumentList, newArgumentList);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> RemoveParameterNameFromEachArgumentAsync(
            Document document,
            ArgumentListSyntax argumentList,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ArgumentListSyntax newArgumentList = RemoveParameterNameSyntaxRewriter.VisitNode(argumentList)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(argumentList, newArgumentList);

            return document.WithSyntaxRoot(newRoot);
        }

        public static ArgumentSyntax AddParameterName(
            ArgumentSyntax argument,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (argument.NameColon != null && !argument.NameColon.IsMissing)
                return argument;

            IParameterSymbol parameterSymbol = argument.DetermineParameter(
                semanticModel,
                allowParams: false,
                cancellationToken: cancellationToken);

            if (parameterSymbol == null)
                return argument;

            return argument
                .WithNameColon(
                    NameColon(parameterSymbol.ToDisplayString(_symbolDisplayFormat))
                        .WithTrailingTrivia(Space))
                .WithTriviaFrom(argument);
        }
    }
}
