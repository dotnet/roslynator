// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Documentation;
using Roslynator.Documentation;
using static Roslynator.CSharp.Documentation.DocumentationCommentGenerator;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CopyDocumentationCommentFromBaseMemberRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel)
        {
            DocumentationCommentData data = GenerateFromBase(methodDeclaration, semanticModel, context.CancellationToken);

            if (!data.Success)
                return;

            RegisterRefactoring(context, methodDeclaration, data, semanticModel);
        }

        public static void ComputeRefactoring(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration, SemanticModel semanticModel)
        {
            DocumentationCommentData data = GenerateFromBase(propertyDeclaration, semanticModel, context.CancellationToken);

            if (!data.Success)
                return;

            RegisterRefactoring(context, propertyDeclaration, data, semanticModel);
        }

        public static void ComputeRefactoring(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration, SemanticModel semanticModel)
        {
            DocumentationCommentData data = GenerateFromBase(indexerDeclaration, semanticModel, context.CancellationToken);

            if (!data.Success)
                return;

            RegisterRefactoring(context, indexerDeclaration, data, semanticModel);
        }

        public static void ComputeRefactoring(RefactoringContext context, EventDeclarationSyntax eventDeclaration, SemanticModel semanticModel)
        {
            DocumentationCommentData data = GenerateFromBase(eventDeclaration, semanticModel, context.CancellationToken);

            if (!data.Success)
                return;

            RegisterRefactoring(context, eventDeclaration, data, semanticModel);
        }

        public static void ComputeRefactoring(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration, SemanticModel semanticModel)
        {
            DocumentationCommentData data = GenerateFromBase(eventFieldDeclaration, semanticModel, context.CancellationToken);

            if (!data.Success)
                return;

            RegisterRefactoring(context, eventFieldDeclaration, data, semanticModel);
        }

        public static void ComputeRefactoring(RefactoringContext context, ConstructorDeclarationSyntax constructorDeclaration, SemanticModel semanticModel)
        {
            DocumentationCommentData data = GenerateFromBase(constructorDeclaration, semanticModel, context.CancellationToken);

            if (!data.Success)
                return;

            RegisterRefactoring(context, constructorDeclaration, data, semanticModel);
        }

        private static void RegisterRefactoring(RefactoringContext context, MemberDeclarationSyntax memberDeclaration, DocumentationCommentData data, SemanticModel semanticModel)
        {
            context.RegisterRefactoring(
                GetTitle(),
                cancellationToken => RefactorAsync(context.Document, memberDeclaration, data, semanticModel, cancellationToken),
                RefactoringIdentifiers.CopyDocumentationCommentFromBaseMember);

            string GetTitle()
            {
                string s;
                DocumentationCommentOrigin origin = data.Origin;

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
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            in DocumentationCommentData data,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SyntaxTrivia commentTrivia = DocumentationCommentTriviaFactory.Parse(data.RawXml, semanticModel, memberDeclaration.SpanStart);

            MemberDeclarationSyntax newMemberDeclaration = memberDeclaration.WithDocumentationComment(commentTrivia, indent: true);

            return document.ReplaceNodeAsync(memberDeclaration, newMemberDeclaration, cancellationToken);
        }
    }
}