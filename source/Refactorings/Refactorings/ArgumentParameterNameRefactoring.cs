// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ArgumentParameterNameRefactoring
    {
        private static readonly SymbolDisplayFormat _symbolDisplayFormat = new SymbolDisplayFormat(
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers,
            parameterOptions: SymbolDisplayParameterOptions.IncludeName);

        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ArgumentListSyntax argumentList)
        {
            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.AddParameterNameToArgument,
                    RefactoringIdentifiers.RemoveParameterNameFromArgument)
                && !context.Span.IsEmpty
                && context.SupportsSemanticModel)
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
                    await AddOrRemoveParameterNameAsync(context, argumentList, arguments.ToImmutableArray()).ConfigureAwait(false);
            }
        }

        private static async Task AddOrRemoveParameterNameAsync(
            RefactoringContext context,
            ArgumentListSyntax argumentList,
            ImmutableArray<ArgumentSyntax> arguments)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddParameterNameToArgument)
                && await CanAddParameterNameAsync(context, arguments).ConfigureAwait(false))
            {
                context.RegisterRefactoring(
                    "Add parameter name",
                    cancellationToken =>
                    {
                        return AddParameterNameToArgumentsAsync(
                            context.Document,
                            argumentList,
                            arguments,
                            cancellationToken);
                    });
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveParameterNameFromArgument)
                && arguments.Any(f => f.NameColon != null))
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

        private static async Task<Document> AddParameterNameToArgumentsAsync(
            Document document,
            ArgumentListSyntax argumentList,
            ImmutableArray<ArgumentSyntax> arguments,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ArgumentListSyntax newArgumentList = AddParameterNameSyntaxRewriter.VisitNode(argumentList, arguments, semanticModel)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(argumentList, newArgumentList);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> RemoveParameterNameFromArgumentsAsync(
            Document document,
            ArgumentListSyntax argumentList,
            ImmutableArray<ArgumentSyntax> arguments,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ArgumentListSyntax newArgumentList = SyntaxRemover.RemoveNameColon(argumentList, arguments)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(argumentList, newArgumentList);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ArgumentSyntax AddParameterName(
            ArgumentSyntax argument,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (argument.NameColon == null || argument.NameColon.IsMissing)
            {
                IParameterSymbol parameterSymbol = argument.DetermineParameter(
                    semanticModel,
                    allowParams: false,
                    cancellationToken: cancellationToken);

                if (parameterSymbol != null)
                {
                    return argument
                        .WithNameColon(
                            NameColon(parameterSymbol.ToDisplayString(_symbolDisplayFormat))
                                .WithTrailingTrivia(Space))
                        .WithTriviaFrom(argument);
                }
            }

            return argument;
        }

        private static async Task<bool> CanAddParameterNameAsync(
            RefactoringContext context,
            ImmutableArray<ArgumentSyntax> arguments)
        {
            foreach (ArgumentSyntax argument in arguments)
            {
                if (argument.NameColon == null || argument.NameColon.IsMissing)
                {
                    IParameterSymbol parameterSymbol = argument.DetermineParameter(
                        await context.GetSemanticModelAsync().ConfigureAwait(false),
                        allowParams: false,
                        cancellationToken: context.CancellationToken);

                    if (parameterSymbol != null)
                        return true;
                }
            }

            return false;
        }

        private class AddParameterNameSyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly SemanticModel _semanticModel;
            private readonly ImmutableArray<ArgumentSyntax> _arguments;

            private AddParameterNameSyntaxRewriter(ImmutableArray<ArgumentSyntax> arguments, SemanticModel semanticModel)
            {
                _arguments = arguments;
                _semanticModel = semanticModel;
            }

            public static ArgumentListSyntax VisitNode(
                ArgumentListSyntax argumentList,
                ImmutableArray<ArgumentSyntax> arguments,
                SemanticModel semanticModel)
            {
                return (ArgumentListSyntax)new AddParameterNameSyntaxRewriter(arguments, semanticModel).Visit(argumentList);
            }

            public override SyntaxNode VisitArgument(ArgumentSyntax node)
            {
                if (_arguments.Contains(node))
                    return AddParameterName(node, _semanticModel);

                return base.VisitArgument(node);
            }
        }
    }
}
