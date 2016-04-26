// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(SimpleLambdaExpressionCodeCodeRefactoringProvider))]
    public class SimpleLambdaExpressionCodeCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            SimpleLambdaExpressionSyntax simpleLambda = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<SimpleLambdaExpressionSyntax>();

            if (simpleLambda == null)
                return;

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

            if (semanticModel == null)
                return;

            RenameParameterAccordingToTypeName(context, semanticModel, simpleLambda);
        }

        private static void RenameParameterAccordingToTypeName(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            SimpleLambdaExpressionSyntax simpleLambda)
        {
            if (simpleLambda.Parameter == null)
                return;

            IParameterSymbol parameterSymbol = semanticModel.GetDeclaredSymbol(simpleLambda.Parameter, context.CancellationToken);

            if (parameterSymbol?.Type == null)
                return;

            string newName = NamingHelper.CreateIdentifierName(
                parameterSymbol.Type,
                firstCharToLower: true);

            if (string.IsNullOrEmpty(newName))
                return;

            if (string.Equals(simpleLambda.Parameter.Identifier.ToString(), newName, StringComparison.Ordinal))
                return;

            context.RegisterRefactoring(
                $"Rename parameter to '{newName}'",
                cancellationToken => parameterSymbol.RenameAsync(newName, context.Document, cancellationToken));
        }
    }
}