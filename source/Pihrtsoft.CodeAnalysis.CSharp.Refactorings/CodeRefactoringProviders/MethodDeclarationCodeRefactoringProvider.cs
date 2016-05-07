// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MethodDeclarationCodeRefactoringProvider))]
    public class MethodDeclarationCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            MethodDeclarationSyntax methodDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MethodDeclarationSyntax>();

            if (methodDeclaration == null)
                return;

            if (methodDeclaration.Identifier.Span.Contains(context.Span)
                && MethodDeclarationRefactoring.CanConvertToReadOnlyProperty(methodDeclaration))
            {
                context.RegisterRefactoring(
                    "Convert to read-only property",
                    cancellationToken => MethodDeclarationRefactoring.ConvertToReadOnlyPropertyAsync(context.Document, methodDeclaration, cancellationToken));
            }

            if (MakeMemberAbstractRefactoring.CanRefactor(context, methodDeclaration))
            {
                context.RegisterRefactoring(
                    $"Make method abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, methodDeclaration, cancellationToken));
            }

            if (!context.Document.SupportsSemanticModel)
                return;

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

            RenameMethodAccordingToTypeName(context, semanticModel, methodDeclaration);
        }

        private static void RenameMethodAccordingToTypeName(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.ReturnType?.IsVoid() == false
                && methodDeclaration.Identifier.Span.Contains(context.Span))
            {
                string newName = NamingHelper.CreateIdentifierName(methodDeclaration.ReturnType, semanticModel);

                Debug.Assert(!string.IsNullOrEmpty(newName), methodDeclaration.ReturnType.ToString());

                if (!string.IsNullOrEmpty(newName))
                {
                    newName = "Get" + newName;

                    if (!string.Equals(newName, methodDeclaration.Identifier.ToString(), StringComparison.Ordinal))
                    {
                        ISymbol symbol = semanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

                        context.RegisterRefactoring(
                            $"Rename method to '{newName}'",
                            cancellationToken => symbol.RenameAsync(newName, context.Document, cancellationToken));
                    }
                }
            }
        }
    }
}