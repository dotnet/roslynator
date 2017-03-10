// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Documentation;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CopyDocumentationCommentFromBaseMemberRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (!methodDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                BaseDocumentationCommentInfo info = DocumentationCommentGenerator.GenerateFromBase(methodDeclaration, semanticModel, context.CancellationToken);

                if (info.Success)
                    RegisterRefactoring(context, methodDeclaration, info);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (!propertyDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                BaseDocumentationCommentInfo info = DocumentationCommentGenerator.GenerateFromBase(propertyDeclaration, semanticModel, context.CancellationToken);

                if (info.Success)
                    RegisterRefactoring(context, propertyDeclaration, info);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (!indexerDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                BaseDocumentationCommentInfo info = DocumentationCommentGenerator.GenerateFromBase(indexerDeclaration, semanticModel, context.CancellationToken);

                if (info.Success)
                    RegisterRefactoring(context, indexerDeclaration, info);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, EventDeclarationSyntax eventDeclaration)
        {
            if (!eventDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                BaseDocumentationCommentInfo info = DocumentationCommentGenerator.GenerateFromBase(eventDeclaration, semanticModel, context.CancellationToken);

                if (info.Success)
                    RegisterRefactoring(context, eventDeclaration, info);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (!eventFieldDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                BaseDocumentationCommentInfo info = DocumentationCommentGenerator.GenerateFromBase(eventFieldDeclaration, semanticModel, context.CancellationToken);

                if (info.Success)
                    RegisterRefactoring(context, eventFieldDeclaration, info);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (!constructorDeclaration.HasSingleLineDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                BaseDocumentationCommentInfo info = DocumentationCommentGenerator.GenerateFromBase(constructorDeclaration, semanticModel, context.CancellationToken);

                if (info.Success)
                    RegisterRefactoring(context, constructorDeclaration, info);
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, MemberDeclarationSyntax memberDeclaration, BaseDocumentationCommentInfo info)
        {
            context.RegisterRefactoring(
                GetTitle(memberDeclaration, info.Origin),
                cancellationToken => RefactorAsync(context.Document, memberDeclaration, info.Trivia, cancellationToken));
        }

        private static string GetTitle(MemberDeclarationSyntax memberDeclaration, BaseDocumentationCommentOrigin origin)
        {
            const string s = "Add comment from ";

            if (origin == BaseDocumentationCommentOrigin.BaseMember)
            {
                switch (memberDeclaration.Kind())
                {
                    case SyntaxKind.ConstructorDeclaration:
                        return s + "base constructor";
                    case SyntaxKind.MethodDeclaration:
                        return s + "base method";
                    case SyntaxKind.PropertyDeclaration:
                        return s + "base property";
                    case SyntaxKind.IndexerDeclaration:
                        return s + "base indexer";
                    case SyntaxKind.EventDeclaration:
                    case SyntaxKind.EventFieldDeclaration:
                        return s + "base event";
                    default:
                        {
                            Debug.Assert(false, memberDeclaration.Kind().ToString());
                            return s + "base member";
                        }
                }
            }
            else if (origin == BaseDocumentationCommentOrigin.InterfaceMember)
            {
                switch (memberDeclaration.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        return s + "interface method";
                    case SyntaxKind.PropertyDeclaration:
                        return s + "interface property";
                    case SyntaxKind.IndexerDeclaration:
                        return s + "interface indexer";
                    case SyntaxKind.EventDeclaration:
                    case SyntaxKind.EventFieldDeclaration:
                        return s + "interface event";
                    default:
                        {
                            Debug.Assert(false, memberDeclaration.Kind().ToString());
                            return s + "interface member";
                        }
                }
            }

            Debug.Assert(false, origin.ToString());

            return s + "base member";
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            SyntaxTrivia commentTrivia,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newMemberDeclaration = Inserter.InsertDocumentationComment(memberDeclaration, commentTrivia, indent: true);

            return document.ReplaceNodeAsync(memberDeclaration, newMemberDeclaration, cancellationToken);
        }
    }
}