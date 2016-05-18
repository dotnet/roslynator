// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ParameterCodeRefactoringProvider))]
    public class ParameterCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            ParameterSyntax parameter = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ParameterSyntax>();

            if (parameter == null)
                return;

            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                RenameParameterAccordingToTypeNameRefactoring.Refactor(context, parameter, semanticModel);

                AddParameterNullCheckRefactoring.Refactor(context, parameter, semanticModel);
            }
        }
    }
}