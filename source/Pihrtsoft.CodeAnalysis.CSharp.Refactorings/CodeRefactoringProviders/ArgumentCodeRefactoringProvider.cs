// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

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

            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                if (argument.Expression?.IsMissing == false)
                    AddCastAccordingToParameterType(context, argument, semanticModel);

                ArgumentRefactoring.AddOrRemoveArgumentName(context, argument, semanticModel);

                ArgumentListSyntax argumentList = root
                    .FindNode(context.Span, getInnermostNodeForTie: true)?
                    .FirstAncestorOrSelf<ArgumentListSyntax>();

                if (argumentList != null)
                    ArgumentRefactoring.AddOrRemoveArgumentName(context, argumentList, semanticModel);
            }
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
                cancellationToken =>
                {
                    return AddCastRefactoring.RefactorAsync(
                        context.Document,
                        argument.Expression,
                        parameterSymbol.Type,
                        cancellationToken);
                });
        }
    }
}
