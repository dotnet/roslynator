// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(CommaTokenCodeCodeRefactoringProvider))]
    public class CommaTokenCodeCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            SyntaxToken commaToken = root.FindToken(context.Span.Start);

            if (!commaToken.IsKind(SyntaxKind.CommaToken))
                return;

            if (!context.Document.SupportsSemanticModel)
                return;

            if (context.Span.Start == 0)
                return;

            if (commaToken.Parent?.IsKind(SyntaxKind.ParameterList) == true)
            {
                ParameterSyntax parameter = root
                    .FindNode(new TextSpan(context.Span.Start - 1, 1))?
                    .FirstAncestorOrSelf<ParameterSyntax>();

                if (parameter == null)
                    return;

                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                RenameParameterAccordingToTypeNameRefactoring.Refactor(context, parameter, semanticModel);

                AddParameterNullCheckRefactoring.Refactor(context, parameter, semanticModel);
            }
        }
    }
}