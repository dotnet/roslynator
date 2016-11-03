// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MarkAllMembersAsStaticRefactoring
    {
        public static void RegisterRefactoring(RefactoringContext context, ClassDeclarationSyntax classDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MarkAllMembersAsStatic))
            {
                context.RegisterRefactoring(
                    "Mark all members as static",
                    cancellationToken => RefactorAsync(context.Document, classDeclaration, cancellationToken));
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var rewriter = new SyntaxRewriter(classDeclaration);

            SyntaxNode newNode = rewriter.Visit(classDeclaration);

            root = root.ReplaceNode(classDeclaration, newNode);

            return document.WithSyntaxRoot(root);
        }

        private class SyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly ClassDeclarationSyntax _classDeclaration;

            public SyntaxRewriter(ClassDeclarationSyntax classDeclaration)
            {
                _classDeclaration = classDeclaration;
            }

            public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                if (_classDeclaration.Members.IndexOf(node) != -1)
                    return MarkMemberAsStaticRefactoring.AddStaticModifier(node);

                return base.VisitFieldDeclaration(node);
            }

            public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                if (_classDeclaration.Members.IndexOf(node) != -1)
                    return MarkMemberAsStaticRefactoring.AddStaticModifier(node);

                return base.VisitMethodDeclaration(node);
            }

            public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            {
                if (_classDeclaration.Members.IndexOf(node) != -1)
                    return MarkMemberAsStaticRefactoring.AddStaticModifier(node);

                return base.VisitPropertyDeclaration(node);
            }

            public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
            {
                if (_classDeclaration.Members.IndexOf(node) != -1)
                    return MarkMemberAsStaticRefactoring.AddStaticModifier(node);

                return base.VisitEventDeclaration(node);
            }

            public override SyntaxNode VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
            {
                if (_classDeclaration.Members.IndexOf(node) != -1)
                    return MarkMemberAsStaticRefactoring.AddStaticModifier(node);

                return base.VisitEventFieldDeclaration(node);
            }

            public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
            {
                if (_classDeclaration.Members.IndexOf(node) != -1)
                    return MarkMemberAsStaticRefactoring.AddStaticModifier(node);

                return base.VisitConstructorDeclaration(node);
            }
        }
    }
}
