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

            SplitAttributesRefactoring.Refactor(context, memberDeclaration);
            MergeAttributesRefactoring.Refactor(context, memberDeclaration);

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.NamespaceDeclaration:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                    {
                        if (MemberDeclarationRefactoring.CanBeRemoved(context, memberDeclaration))
                        {
                            context.RegisterRefactoring(
                                "Remove " + SyntaxHelper.GetSyntaxNodeName(memberDeclaration),
                                cancellationToken => MemberDeclarationRefactoring.RemoveMemberAsync(context.Document, memberDeclaration, cancellationToken));
                        }

                        if (MemberDeclarationRefactoring.CanBeDuplicated(context, memberDeclaration))
                        {
                            context.RegisterRefactoring(
                                "Duplicate " + SyntaxHelper.GetSyntaxNodeName(memberDeclaration),
                                cancellationToken => MemberDeclarationRefactoring.DuplicateMemberAsync(context.Document, memberDeclaration, cancellationToken));
                        }

                        break;
                    }
            }

            if (root.FindTrivia(context.Span.Start).IsWhitespaceOrEndOfLine())
                SwapMembersRefactoring.Refactor(context, memberDeclaration);

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        await ComputeRefactoringsAsync(context, (MethodDeclarationSyntax)memberDeclaration);
                        break;
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        ComputeRefactorings(context, (IndexerDeclarationSyntax)memberDeclaration);
                        break;
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        await ComputeRefactoringsAsync(context, (PropertyDeclarationSyntax)memberDeclaration);
                        break;
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        await ComputeRefactoringsAsync(context, (EventFieldDeclarationSyntax)memberDeclaration);
                        break;
                    }
            }
        }

        private async Task ComputeRefactoringsAsync(CodeRefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration.HeaderSpan().Contains(context.Span))
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
            }

            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                MethodDeclarationRefactoring.RenameAccordingToTypeName(methodDeclaration, context, semanticModel);
            }
        }

        private void ComputeRefactorings(CodeRefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration.HeaderSpan().Contains(context.Span)
                && MakeMemberAbstractRefactoring.CanRefactor(context, indexerDeclaration))
            {
                context.RegisterRefactoring(
                    "Make indexer abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, indexerDeclaration, cancellationToken));
            }
        }

        private async Task ComputeRefactoringsAsync(CodeRefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.HeaderSpan().Contains(context.Span)
                && PropertyDeclarationRefactoring.CanConvertToMethod(propertyDeclaration))
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

                NotifyPropertyChangedRefactoring.Refactor(propertyDeclaration, context, semanticModel);
            }

            if (propertyDeclaration.HeaderSpan().Contains(context.Span)
                && MakeMemberAbstractRefactoring.CanRefactor(context, propertyDeclaration))
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

        private async Task ComputeRefactoringsAsync(CodeRefactoringContext context, EventFieldDeclarationSyntax eventDeclaration)
        {
            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                if (EventFieldDeclarationRefactoring.CanExpand(eventDeclaration, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Expand event",
                        cancellationToken =>
                        {
                            return EventFieldDeclarationRefactoring.ExpandAsync(
                                context.Document,
                                eventDeclaration,
                                cancellationToken);
                        });
                }
            }
        }
    }
}