// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MemberDeclarationCodeRefactoringProvider))]
    public class MemberDeclarationCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            MemberDeclarationSyntax memberDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            if (memberDeclaration == null)
                return;

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    {
                        MemberDeclarationRefactoring.Remove(context, memberDeclaration);
                        MemberDeclarationRefactoring.Duplicate(context, memberDeclaration);
                        break;
                    }
            }

            if (memberDeclaration.IsKind(SyntaxKind.MethodDeclaration))
            {
                await ComputeRefactoringsAsync(context, (MethodDeclarationSyntax)memberDeclaration);
            }
            else if (memberDeclaration.IsKind(SyntaxKind.IndexerDeclaration))
            {
                ComputeRefactorings(context, (IndexerDeclarationSyntax)memberDeclaration);
            }
            else if (memberDeclaration.IsKind(SyntaxKind.PropertyDeclaration))
            {
                await ComputeRefactoringsAsync(context, (PropertyDeclarationSyntax)memberDeclaration);
            }
        }

        private async Task ComputeRefactoringsAsync(CodeRefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (MethodDeclarationRefactoring.CanConvertToReadOnlyProperty(methodDeclaration))
            {
                context.RegisterRefactoring(
                    "Convert to read-only property",
                    cancellationToken => MethodDeclarationRefactoring.ConvertToReadOnlyPropertyAsync(context.Document, methodDeclaration, cancellationToken));
            }

            if (MakeMemberAbstractRefactoring.CanRefactor(context, methodDeclaration))
            {
                context.RegisterRefactoring(
                    "Make method abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, methodDeclaration, cancellationToken));
            }

            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                MethodDeclarationRefactoring.RenameAccordingToTypeName(methodDeclaration, context, semanticModel);
            }
        }

        private void ComputeRefactorings(CodeRefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (MakeMemberAbstractRefactoring.CanRefactor(context, indexerDeclaration))
            {
                context.RegisterRefactoring(
                    "Make indexer abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, indexerDeclaration, cancellationToken));
            }
        }

        private async Task ComputeRefactoringsAsync(CodeRefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (PropertyDeclarationRefactoring.CanConvertToMethod(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Convert to method",
                    cancellationToken => PropertyDeclarationRefactoring.ConvertToMethodAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                if (PropertyDeclarationRefactoring.CanExpand(propertyDeclaration, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Expand property",
                        cancellationToken => PropertyDeclarationRefactoring.ExpandPropertyAsync(context.Document, propertyDeclaration, cancellationToken));

                    context.RegisterRefactoring(
                        "Expand property and add backing field",
                        cancellationToken => PropertyDeclarationRefactoring.ExpandPropertyAndAddBackingFieldAsync(context.Document, propertyDeclaration, cancellationToken));
                }
            }

            if (MakeMemberAbstractRefactoring.CanRefactor(context, propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Make property abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                PropertyDeclarationRefactoring.RenameAccordingToTypeName(propertyDeclaration, context, semanticModel);
            }

            if (propertyDeclaration.Initializer != null)
            {
                context.RegisterRefactoring(
                    "Remove initializer",
                    cancellationToken => PropertyDeclarationRefactoring.RemoveInitializerAsync(context.Document, propertyDeclaration, cancellationToken));
            }
        }
    }
}