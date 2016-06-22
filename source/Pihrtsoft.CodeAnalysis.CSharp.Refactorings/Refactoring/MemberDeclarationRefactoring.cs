// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class MemberDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
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
                        if (CanBeRemoved(context, memberDeclaration))
                        {
                            context.RegisterRefactoring(
                                "Remove " + SyntaxHelper.GetSyntaxNodeName(memberDeclaration),
                                cancellationToken => RemoveMemberDeclarationRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken));
                        }

                        if (CanBeDuplicated(context, memberDeclaration))
                        {
                            context.RegisterRefactoring(
                                "Duplicate " + SyntaxHelper.GetSyntaxNodeName(memberDeclaration),
                                cancellationToken => DuplicateMemberDeclarationRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken));
                        }
#if DEBUG
                        if (RemoveAllStatementsRefactoring.CanRefactor(context, memberDeclaration))
                        {
                            context.RegisterRefactoring(
                                "Remove all statements",
                                cancellationToken => RemoveAllStatementsRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken));
                        }
#endif

                        break;
                    }
            }

            if (context.Root.FindTrivia(context.Span.Start).IsWhitespaceOrEndOfLine())
                SwapMembersRefactoring.Refactor(context, memberDeclaration);

            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        await ComputeRefactoringsAsync(context, (MethodDeclarationSyntax)memberDeclaration);
                        break;
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        ComputeRefactorings(context, (ConstructorDeclarationSyntax)memberDeclaration);
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
                case SyntaxKind.OperatorDeclaration:
                    {
                        ComputeRefactorings(context, (OperatorDeclarationSyntax)memberDeclaration);
                        break;
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        ComputeRefactorings(context, (ConversionOperatorDeclarationSyntax)memberDeclaration);
                        break;
                    }
                case SyntaxKind.FieldDeclaration:
                    {
                        await FieldDeclarationRefactoring.ComputeRefactoringsAsync(context, (FieldDeclarationSyntax)memberDeclaration);
                        break;
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        ComputeRefactorings(context, (EventDeclarationSyntax)memberDeclaration);
                        break;
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        await ComputeRefactoringsAsync(context, (EventFieldDeclarationSyntax)memberDeclaration);
                        break;
                    }
            }
        }

        private static async Task ComputeRefactoringsAsync(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (MarkMemberAsStaticRefactoring.CanRefactor(methodDeclaration))
            {
                context.RegisterRefactoring(
                    "Mark method as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, methodDeclaration, cancellationToken));

                MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)methodDeclaration.Parent);
            }

            if (methodDeclaration.HeaderSpan().Contains(context.Span)
                && MethodDeclarationRefactoring.CanConvertToReadOnlyProperty(methodDeclaration))
            {
                context.RegisterRefactoring(
                    "Convert to read-only property",
                    cancellationToken => MethodDeclarationRefactoring.ConvertToReadOnlyPropertyAsync(context.Document, methodDeclaration, cancellationToken));
            }

            if (methodDeclaration.Body?.Span.Contains(context.Span) == true
                && UseExpressionBodiedMemberRefactoring.CanRefactor(methodDeclaration))
            {
                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, methodDeclaration, cancellationToken));
            }

            if (methodDeclaration.HeaderSpan().Contains(context.Span)
                && MakeMemberAbstractRefactoring.CanRefactor(context, methodDeclaration))
            {
                context.RegisterRefactoring(
                    "Make method abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, methodDeclaration, cancellationToken));
            }

            if (context.SupportsSemanticModel)
            {
                await MethodDeclarationRefactoring.RenameAccordingToTypeNameAsync(context, methodDeclaration);
            }
        }

        private static void ComputeRefactorings(RefactoringContext context, ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (MarkMemberAsStaticRefactoring.CanRefactor(constructorDeclaration))
            {
                context.RegisterRefactoring(
                    "Mark constructor as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, constructorDeclaration, cancellationToken));

                MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)constructorDeclaration.Parent);
            }
        }

        private static void ComputeRefactorings(RefactoringContext context, OperatorDeclarationSyntax operatorDeclaration)
        {
            if (operatorDeclaration.Body?.Span.Contains(context.Span) == true
                && UseExpressionBodiedMemberRefactoring.CanRefactor(operatorDeclaration))
            {
                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, operatorDeclaration, cancellationToken));
            }
        }

        private static void ComputeRefactorings(RefactoringContext context, ConversionOperatorDeclarationSyntax operatorDeclaration)
        {
            if (operatorDeclaration.Body?.Span.Contains(context.Span) == true
                && UseExpressionBodiedMemberRefactoring.CanRefactor(operatorDeclaration))
            {
                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, operatorDeclaration, cancellationToken));
            }
        }

        private static void ComputeRefactorings(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration.AccessorList?.Span.Contains(context.Span) == true
                && UseExpressionBodiedMemberRefactoring.CanRefactor(indexerDeclaration))
            {
                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, indexerDeclaration, cancellationToken));
            }

            if (indexerDeclaration.HeaderSpan().Contains(context.Span)
                && MakeMemberAbstractRefactoring.CanRefactor(context, indexerDeclaration))
            {
                context.RegisterRefactoring(
                    "Make indexer abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, indexerDeclaration, cancellationToken));
            }
        }

        private static async Task ComputeRefactoringsAsync(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (MarkMemberAsStaticRefactoring.CanRefactor(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Mark property as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));

                MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)propertyDeclaration.Parent);
            }

            if (propertyDeclaration.HeaderSpan().Contains(context.Span)
                && PropertyDeclarationRefactoring.CanConvertToMethod(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Convert to method",
                    cancellationToken => PropertyDeclarationRefactoring.ConvertToMethodAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (propertyDeclaration.AccessorList?.Span.Contains(context.Span) == true
                && UseExpressionBodiedMemberRefactoring.CanRefactor(propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (propertyDeclaration.Initializer != null
                && propertyDeclaration.Initializer.Span.Contains(context.Span))
            {
                context.RegisterRefactoring(
                    "Remove property initializer",
                    cancellationToken => PropertyDeclarationRefactoring.RemoveInitializerAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.SupportsSemanticModel)
            {
                if (propertyDeclaration.Span.Contains(context.Span)
                    && PropertyDeclarationRefactoring.CanExpand(propertyDeclaration))
                {
                    context.RegisterRefactoring(
                        "Expand property",
                        cancellationToken => PropertyDeclarationRefactoring.ExpandPropertyAsync(context.Document, propertyDeclaration, cancellationToken));

                    context.RegisterRefactoring(
                        "Expand property and add backing field",
                        cancellationToken => PropertyDeclarationRefactoring.ExpandPropertyAndAddBackingFieldAsync(context.Document, propertyDeclaration, cancellationToken));
                }

                await NotifyPropertyChangedRefactoring.RefactorAsync(context, propertyDeclaration);
            }

            if (propertyDeclaration.HeaderSpan().Contains(context.Span)
                && MakeMemberAbstractRefactoring.CanRefactor(context, propertyDeclaration))
            {
                context.RegisterRefactoring(
                    "Make property abstract",
                    cancellationToken => MakeMemberAbstractRefactoring.RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
            }

            if (context.SupportsSemanticModel)
                await PropertyDeclarationRefactoring.RenameAccordingToTypeNameAsync(context, propertyDeclaration);
        }

        private static void ComputeRefactorings(RefactoringContext context, EventDeclarationSyntax eventDeclaration)
        {
            if (MarkMemberAsStaticRefactoring.CanRefactor(eventDeclaration))
            {
                context.RegisterRefactoring(
                    "Mark event as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, eventDeclaration, cancellationToken));

                MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)eventDeclaration.Parent);
            }
        }

        private static async Task ComputeRefactoringsAsync(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (MarkMemberAsStaticRefactoring.CanRefactor(eventFieldDeclaration))
            {
                context.RegisterRefactoring(
                    "Mark event as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, eventFieldDeclaration, cancellationToken));

                MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)eventFieldDeclaration.Parent);
            }

            if (eventFieldDeclaration.Span.Contains(context.Span)
                && context.SupportsSemanticModel
                && EventFieldDeclarationRefactoring.CanExpand(eventFieldDeclaration, await context.GetSemanticModelAsync(), context.CancellationToken))
            {
                context.RegisterRefactoring(
                    "Expand event",
                    cancellationToken =>
                    {
                        return EventFieldDeclarationRefactoring.ExpandAsync(
                            context.Document,
                            eventFieldDeclaration,
                            cancellationToken);
                    });
            }
        }

        public static bool CanBeRemoved(RefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return CanBeRemovedOrDuplicated(context, memberDeclaration);
        }

        public static bool CanBeDuplicated(RefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return CanBeRemovedOrDuplicated(context, memberDeclaration);
        }

        private static bool CanBeRemovedOrDuplicated(RefactoringContext context, MemberDeclarationSyntax member)
        {
            if (member.Parent?.IsAnyKind(
                    SyntaxKind.NamespaceDeclaration,
                    SyntaxKind.ClassDeclaration,
                    SyntaxKind.StructDeclaration,
                    SyntaxKind.InterfaceDeclaration) != true)
            {
                return false;
            }

            switch (member.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var declaration = (MethodDeclarationSyntax)member;

                        return declaration.Body != null
                            && (declaration.Body.OpenBraceToken.Span.Contains(context.Span)
                                || declaration.Body.CloseBraceToken.Span.Contains(context.Span));
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var declaration = (IndexerDeclarationSyntax)member;

                        return declaration.AccessorList != null
                            && (declaration.AccessorList.OpenBraceToken.Span.Contains(context.Span)
                                || declaration.AccessorList.CloseBraceToken.Span.Contains(context.Span));
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var declaration = (OperatorDeclarationSyntax)member;

                        return declaration.Body != null
                            && (declaration.Body.OpenBraceToken.Span.Contains(context.Span)
                                || declaration.Body.CloseBraceToken.Span.Contains(context.Span));
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var declaration = (ConversionOperatorDeclarationSyntax)member;

                        return declaration.Body != null
                            && (declaration.Body.OpenBraceToken.Span.Contains(context.Span)
                                || declaration.Body.CloseBraceToken.Span.Contains(context.Span));
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var declaration = (ConstructorDeclarationSyntax)member;

                        return declaration.Body != null
                            && (declaration.Body.OpenBraceToken.Span.Contains(context.Span)
                                || declaration.Body.CloseBraceToken.Span.Contains(context.Span));
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var declaration = (PropertyDeclarationSyntax)member;

                        return declaration.AccessorList != null
                            && (declaration.AccessorList.OpenBraceToken.Span.Contains(context.Span)
                                || declaration.AccessorList.CloseBraceToken.Span.Contains(context.Span));
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var declaration = (EventDeclarationSyntax)member;

                        return declaration.AccessorList != null
                            && (declaration.AccessorList.OpenBraceToken.Span.Contains(context.Span)
                                || declaration.AccessorList.CloseBraceToken.Span.Contains(context.Span));
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var declaration = (NamespaceDeclarationSyntax)member;

                        return declaration.OpenBraceToken.Span.Contains(context.Span)
                            || declaration.CloseBraceToken.Span.Contains(context.Span);
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var declaration = (ClassDeclarationSyntax)member;

                        return declaration.OpenBraceToken.Span.Contains(context.Span)
                            || declaration.CloseBraceToken.Span.Contains(context.Span);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var declaration = (StructDeclarationSyntax)member;

                        return declaration.OpenBraceToken.Span.Contains(context.Span)
                            || declaration.CloseBraceToken.Span.Contains(context.Span);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var declaration = (InterfaceDeclarationSyntax)member;

                        return declaration.OpenBraceToken.Span.Contains(context.Span)
                            || declaration.CloseBraceToken.Span.Contains(context.Span);
                    }
            }

            return false;
        }
    }
}