// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveAllMemberDeclarationsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSyntax member)
        {
            if (!CanRefactor(member, context.Span))
                return;

            context.RegisterRefactoring(
                "Remove all members",
                cancellationToken => RefactorAsync(context.Document, member, cancellationToken));
        }

        public static bool CanRefactor(MemberDeclarationSyntax member, TextSpan span)
        {
            switch (member.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var declaration = (NamespaceDeclarationSyntax)member;

                        return declaration.Members.Any()
                            && (declaration.OpenBraceToken.Span.Contains(span)
                                || declaration.CloseBraceToken.Span.Contains(span));
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var declaration = (ClassDeclarationSyntax)member;

                        return declaration.Members.Any()
                            && (declaration.OpenBraceToken.Span.Contains(span)
                                || declaration.CloseBraceToken.Span.Contains(span));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var declaration = (StructDeclarationSyntax)member;

                        return declaration.Members.Any()
                            && (declaration.OpenBraceToken.Span.Contains(span)
                                || declaration.CloseBraceToken.Span.Contains(span));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var declaration = (InterfaceDeclarationSyntax)member;

                        return declaration.Members.Any()
                            && (declaration.OpenBraceToken.Span.Contains(span)
                                || declaration.CloseBraceToken.Span.Contains(span));
                    }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MemberDeclarationListInfo info = SyntaxInfo.MemberDeclarationListInfo(member);

            SyntaxNode newNode = info
                .WithMembers(default(SyntaxList<MemberDeclarationSyntax>))
                .Parent
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(info.Parent, newNode, cancellationToken);
        }
    }
}
