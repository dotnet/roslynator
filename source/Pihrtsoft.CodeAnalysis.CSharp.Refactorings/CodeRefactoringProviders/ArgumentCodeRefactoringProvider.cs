// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;
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

            if (!context.Document.SupportsSemanticModel)
                return;

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

            AddCastAccordingToParameterType(context, argument, semanticModel);

            ArgumentRefactoring.AddOrRemoveArgumentName(context, argument, semanticModel);

            ArgumentListSyntax argumentList = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ArgumentListSyntax>();

            if (argumentList != null)
                ArgumentRefactoring.AddOrRemoveArgumentName(context, argumentList, semanticModel);

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
