// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;
using Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ArgumentCodeRefactoringProvider))]
    public class ArgumentCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            ArgumentSyntax argument = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ArgumentSyntax>();

            if (argument == null)
                return;

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

            if (semanticModel == null)
                return;

            AddCastAccordingToParameterType(context, argument, semanticModel);

            if (argument.NameColon == null)
            {
                AddParameterName(context, argument, semanticModel);
            }
            else if (!argument.NameColon.IsMissing)
            {
                context.RegisterRefactoring(
                    "Remove parameter name",
                    cancellationToken => RemoveParameterNameAsync(context.Document, argument, cancellationToken));
            }

            ArgumentListSyntax argumentList = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ArgumentListSyntax>();

            if (argumentList == null)
                return;

            if (argumentList.Arguments.Count <= 1)
                return;

            if (argumentList.Arguments.Any(f => f.NameColon == null))
            {
                context.RegisterRefactoring(
                    "Add parameter name to each argument",
                    cancellationToken => AddParameterNameToEachArgumentAsync(context.Document, argumentList, semanticModel, cancellationToken));
            }
            else if (argumentList.Arguments.All(f => f.NameColon != null))
            {
                context.RegisterRefactoring(
                    "Remove parameter name from each argument",
                    cancellationToken => RemoveParameterNameFromEachArgumentAsync(context.Document, argumentList, cancellationToken));
            }
        }

        private static void AddParameterName(
            CodeRefactoringContext context,
            ArgumentSyntax argument,
            SemanticModel semanticModel)
        {
            IParameterSymbol parameterSymbol = argument.DetermineParameter(
                semanticModel,
                allowParams: false,
                cancellationToken: context.CancellationToken);

            if (parameterSymbol == null)
                return;

            context.RegisterRefactoring(
                "Add parameter name",
                cancellationToken => AddParameterNameAsync(context.Document, argument, parameterSymbol, cancellationToken));
        }

        private static void AddCastAccordingToParameterType(
            CodeRefactoringContext context,
            ArgumentSyntax argument,
            SemanticModel semanticModel)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(argument.Expression).ConvertedType;

            if (typeSymbol == null)
                return;

            IParameterSymbol parameterSymbol = argument.DetermineParameter(
                semanticModel,
                allowParams: false,
                allowCandidate: true,
                cancellationToken: context.CancellationToken);

            if (parameterSymbol == null)
                return;

            if (typeSymbol.Equals(parameterSymbol.Type))
                return;

            context.RegisterRefactoring(
                $"Add cast to '{parameterSymbol.Type.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                cancellationToken => AddCastAsync(context.Document, argument.Expression, parameterSymbol.Type, cancellationToken));
        }

        private static async Task<Document> AddParameterNameAsync(
            Document document,
            ArgumentSyntax argument,
            IParameterSymbol parameterSymbol,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ArgumentSyntax newNode = argument
                .WithNameColon(NameColon(parameterSymbol.Name))
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

        private static async Task<Document> AddCastAsync(
            Document document,
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            TypeSyntax type = TypeSyntaxRefactoring.CreateTypeSyntax(typeSymbol)
                .WithAdditionalAnnotations(Simplifier.Annotation);

            CastExpressionSyntax castExpression = CastExpression(type, expression)
                .WithTriviaFrom(expression);

            SyntaxNode newRoot = oldRoot.ReplaceNode(expression, castExpression);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
