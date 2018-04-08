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
            if (methodDeclaration.HasDocumentationComment())
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            DocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(methodDeclaration, semanticModel, context.CancellationToken);

            if (!data.Success)
                return;

            RegisterRefactoring(context, methodDeclaration, data);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.HasDocumentationComment())
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            DocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(propertyDeclaration, semanticModel, context.CancellationToken);

            if (!data.Success)
                return;

            RegisterRefactoring(context, propertyDeclaration, data);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration.HasDocumentationComment())
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            DocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(indexerDeclaration, semanticModel, context.CancellationToken);

            if (!data.Success)
                return;

            RegisterRefactoring(context, indexerDeclaration, data);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, EventDeclarationSyntax eventDeclaration)
        {
            if (eventDeclaration.HasDocumentationComment())
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            DocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(eventDeclaration, semanticModel, context.CancellationToken);

            if (!data.Success)
                return;

            RegisterRefactoring(context, eventDeclaration, data);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (eventFieldDeclaration.HasDocumentationComment())
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            DocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(eventFieldDeclaration, semanticModel, context.CancellationToken);

            if (!data.Success)
                return;

            RegisterRefactoring(context, eventFieldDeclaration, data);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (constructorDeclaration.HasDocumentationComment())
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            DocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(constructorDeclaration, semanticModel, context.CancellationToken);

            if (!data.Success)
                return;

            RegisterRefactoring(context, constructorDeclaration, data);
        }

        private static void RegisterRefactoring(RefactoringContext context, MemberDeclarationSyntax memberDeclaration, DocumentationCommentData data)
        {
            context.RegisterRefactoring(
                GetTitle(memberDeclaration, data.Origin),
                cancellationToken => RefactorAsync(context.Document, memberDeclaration, data.Comment, cancellationToken));
        }

        private static string GetTitle(MemberDeclarationSyntax memberDeclaration, DocumentationCommentOrigin origin)
        {
            string s;

            if (origin == DocumentationCommentOrigin.BaseMember)
            {
                s = "base";
            }
            else if (origin == DocumentationCommentOrigin.InterfaceMember)
            {
                s = "interface";
            }
            else
            {
                Debug.Fail(origin.ToString());
                s = "base";
            }

            return $"Add comment from {s} {CSharpFacts.GetTitle(memberDeclaration)}";
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