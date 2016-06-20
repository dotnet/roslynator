// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;
using Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ArgumentRefactoring
    {
        private static readonly SymbolDisplayFormat _symbolDisplayFormat = new SymbolDisplayFormat(
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers,
            parameterOptions: SymbolDisplayParameterOptions.IncludeName);

        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ArgumentSyntax argument)
        {
            if (argument.Expression?.IsMissing == false
                && context.SupportsSemanticModel)
            {
                await AddCastAccordingToParameterTypeAsync(context, argument);
            }
        }

        private static async Task AddCastAccordingToParameterTypeAsync(RefactoringContext context, ArgumentSyntax argument)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync();

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(argument.Expression).ConvertedType;

            if (typeSymbol?.IsKind(SymbolKind.ErrorType) == false)
            {
                IParameterSymbol parameterSymbol = argument.DetermineParameter(
                    semanticModel,
                    allowParams: false,
                    allowCandidate: true,
                    cancellationToken: context.CancellationToken);

                if (parameterSymbol?.Type?.Equals(typeSymbol) == false)
                    AddCastRefactoring.Refactor(context, argument.Expression, parameterSymbol.Type);
            }
        }

        public static void AddOrRemoveParameterName(
            RefactoringContext context,
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

        public static async Task AddOrRemoveParameterNameAsync(RefactoringContext context, ArgumentListSyntax argumentList)
        {
            List<ArgumentSyntax> arguments = null;

            foreach (ArgumentSyntax argument in argumentList.Arguments)
            {
                if (argument.Expression != null && context.Span.Contains(argument.Expression.Span))
                {
                    if (arguments == null)
                        arguments = new List<ArgumentSyntax>();

                    arguments.Add(argument);
                }
            }

            if (arguments?.Count > 0)
                await AddOrRemoveParameterNameAsync(context, argumentList, arguments.ToArray());
        }

        private static async Task AddOrRemoveParameterNameAsync(
            RefactoringContext context,
            ArgumentListSyntax argumentList,
            ArgumentSyntax[] arguments)
        {
            if (await CanAddParameterNameAsync(context, arguments))
            {
                context.RegisterRefactoring(
                    "Add parameter name",
                    cancellationToken =>
                    {
                        return AddParameterNameToEachArgumentAsync(
                            context.Document,
                            argumentList,
                            arguments,
                            cancellationToken);
                    });
            }

            if (arguments.Any(f => f.NameColon != null))
            {
                context.RegisterRefactoring(
                    "Remove parameter name",
                    cancellationToken =>
                    {
                        return RemoveParameterNameFromArgumentsAsync(
                            context.Document,
                            argumentList,
                            arguments,
                            cancellationToken);
                    });
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
            ArgumentSyntax[] arguments,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            ArgumentListSyntax newArgumentList = AddParameterNameSyntaxRewriter.VisitNode(argumentList, arguments, semanticModel)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(argumentList, newArgumentList);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> RemoveParameterNameFromArgumentsAsync(
            Document document,
            ArgumentListSyntax argumentList,
            ArgumentSyntax[] arguments,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ArgumentListSyntax newArgumentList = ParameterNameRemover.VisitNode(argumentList, arguments)
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

        private static async Task<bool> CanAddParameterNameAsync(
            RefactoringContext context,
            ArgumentSyntax[] arguments)
        {
            foreach (ArgumentSyntax argument in arguments)
            {
                if (argument.NameColon == null || argument.NameColon.IsMissing)
                {
                    IParameterSymbol parameterSymbol = argument.DetermineParameter(
                        await context.GetSemanticModelAsync(),
                        allowParams: false,
                        cancellationToken: context.CancellationToken);

                    if (parameterSymbol != null)
                        return true;
                }
            }

            return false;
        }
    }
}
