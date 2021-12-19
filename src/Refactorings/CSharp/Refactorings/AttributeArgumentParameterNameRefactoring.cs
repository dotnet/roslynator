// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AttributeArgumentParameterNameRefactoring
    {
        private static readonly SymbolDisplayFormat _symbolDisplayFormat = new(
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers,
            parameterOptions: SymbolDisplayParameterOptions.IncludeName);

        public static async Task ComputeRefactoringsAsync(RefactoringContext context, AttributeArgumentListSyntax argumentList)
        {
            if (!context.IsAnyRefactoringEnabled(
                RefactoringDescriptors.AddArgumentName,
                RefactoringDescriptors.RemoveArgumentName))
            {
                return;
            }

            if (context.Span.IsEmpty)
                return;

            List<AttributeArgumentSyntax> list = null;

            foreach (AttributeArgumentSyntax argument in argumentList.Arguments)
            {
                if (argument.Expression != null
                    && context.Span.Contains(argument.Expression.Span))
                {
                    (list ??= new List<AttributeArgumentSyntax>()).Add(argument);
                }
            }

            if (list == null)
                return;

            AttributeArgumentSyntax[] arguments = list.ToArray();

            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddArgumentName)
                && await CanAddParameterNameAsync(context, arguments).ConfigureAwait(false))
            {
                context.RegisterRefactoring(
                    "Add argument name",
                    ct => AddArgumentNameAsync(context.Document, argumentList, arguments, ct),
                    RefactoringDescriptors.AddArgumentName);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveArgumentName)
                && arguments.Any(f => f.NameColon != null))
            {
                context.RegisterRefactoring(
                    "Remove argument name",
                    ct => RemoveArgumentNameAsync(context.Document, argumentList, arguments, ct),
                    RefactoringDescriptors.RemoveArgumentName);
            }
        }

        private static async Task<Document> AddArgumentNameAsync(
            Document document,
            AttributeArgumentListSyntax argumentList,
            AttributeArgumentSyntax[] arguments,
            CancellationToken cancellationToken = default)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            AttributeArgumentListSyntax newArgumentList = AddParameterNameSyntaxRewriter.VisitNode(argumentList, arguments, semanticModel)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(argumentList, newArgumentList, cancellationToken).ConfigureAwait(false);
        }

        private static Task<Document> RemoveArgumentNameAsync(
            Document document,
            AttributeArgumentListSyntax argumentList,
            AttributeArgumentSyntax[] arguments,
            CancellationToken cancellationToken = default)
        {
            AttributeArgumentListSyntax newArgumentList = RemoveParameterNameSyntaxRewriter.VisitNode(argumentList, arguments)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(argumentList, newArgumentList, cancellationToken);
        }

        private static AttributeArgumentSyntax AddParameterName(
            AttributeArgumentSyntax argument,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            if (argument.NameColon?.IsMissing != false)
            {
                IParameterSymbol parameterSymbol = semanticModel.DetermineParameter(
                    argument,
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
            AttributeArgumentSyntax[] arguments)
        {
            foreach (AttributeArgumentSyntax argument in arguments)
            {
                if (argument.NameColon?.IsMissing != false)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    IParameterSymbol parameterSymbol = semanticModel.DetermineParameter(
                        argument,
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
            private readonly AttributeArgumentSyntax[] _arguments;

            private AddParameterNameSyntaxRewriter(AttributeArgumentSyntax[] arguments, SemanticModel semanticModel)
            {
                _arguments = arguments;
                _semanticModel = semanticModel;
            }

            public static AttributeArgumentListSyntax VisitNode(
                AttributeArgumentListSyntax argumentList,
                AttributeArgumentSyntax[] arguments,
                SemanticModel semanticModel)
            {
                return (AttributeArgumentListSyntax)new AddParameterNameSyntaxRewriter(arguments, semanticModel).Visit(argumentList);
            }

            public override SyntaxNode VisitAttributeArgument(AttributeArgumentSyntax node)
            {
                if (Array.IndexOf(_arguments, node) != -1)
                    return AddParameterName(node, _semanticModel);

                return base.VisitAttributeArgument(node);
            }
        }

        private class RemoveParameterNameSyntaxRewriter : CSharpSyntaxRewriter
        {
            private static readonly RemoveParameterNameSyntaxRewriter _instance = new();

            private readonly AttributeArgumentSyntax[] _arguments;

            private RemoveParameterNameSyntaxRewriter(AttributeArgumentSyntax[] arguments = null)
            {
                _arguments = arguments;
            }

            public static AttributeArgumentListSyntax VisitNode(AttributeArgumentListSyntax argumentList, AttributeArgumentSyntax[] arguments = null)
            {
                if (arguments == null)
                {
                    return (AttributeArgumentListSyntax)_instance.Visit(argumentList);
                }
                else
                {
                    var instance = new RemoveParameterNameSyntaxRewriter(arguments);
                    return (AttributeArgumentListSyntax)instance.Visit(argumentList);
                }
            }

            public override SyntaxNode VisitAttributeArgument(AttributeArgumentSyntax node)
            {
                if (_arguments == null || Array.IndexOf(_arguments, node) != -1)
                {
                    return node
                        .WithNameColon(null)
                        .WithTriviaFrom(node);
                }

                return base.VisitAttributeArgument(node);
            }
        }
    }
}
