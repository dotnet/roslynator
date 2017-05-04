// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Documentation;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CopyDocumentationCommentFromBaseMemberRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (!methodDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                BaseDocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(methodDeclaration, semanticModel, context.CancellationToken);

                if (data.Success)
                    RegisterRefactoring(context, methodDeclaration, data);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (!propertyDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                BaseDocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(propertyDeclaration, semanticModel, context.CancellationToken);

                if (data.Success)
                    RegisterRefactoring(context, propertyDeclaration, data);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (!indexerDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                BaseDocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(indexerDeclaration, semanticModel, context.CancellationToken);

                if (data.Success)
                    RegisterRefactoring(context, indexerDeclaration, data);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, EventDeclarationSyntax eventDeclaration)
        {
            if (!eventDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                BaseDocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(eventDeclaration, semanticModel, context.CancellationToken);

                if (data.Success)
                    RegisterRefactoring(context, eventDeclaration, data);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (!eventFieldDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                BaseDocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(eventFieldDeclaration, semanticModel, context.CancellationToken);

                if (data.Success)
                    RegisterRefactoring(context, eventFieldDeclaration, data);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (!constructorDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                BaseDocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(constructorDeclaration, semanticModel, context.CancellationToken);

                if (data.Success)
                    RegisterRefactoring(context, constructorDeclaration, data);
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, MemberDeclarationSyntax memberDeclaration, BaseDocumentationCommentData data)
        {
            context.RegisterRefactoring(
                GetTitle(memberDeclaration, data.Origin),
                cancellationToken => RefactorAsync(context.Document, memberDeclaration, data.Comment, cancellationToken));
        }

        private static string GetTitle(MemberDeclarationSyntax memberDeclaration, BaseDocumentationCommentOrigin origin)
        {
            string s;

            if (origin == BaseDocumentationCommentOrigin.BaseMember)
            {
                s = "base";
            }
            else if (origin == BaseDocumentationCommentOrigin.InterfaceMember)
            {
                s = "interface";
            }
            else
            {
                Debug.Fail(origin.ToString());
                s = "base";
            }

            return $"Add comment from {s} {memberDeclaration.GetTitle()}";
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            SyntaxTrivia commentTrivia,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newMemberDeclaration = memberDeclaration.WithDocumentationComment(commentTrivia, indent: true);

            return document.ReplaceNodeAsync(memberDeclaration, newMemberDeclaration, cancellationToken);
        }
    }
}