// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.CodeFixes;
using Roslynator.CSharp.Helpers.ModifierHelpers;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ModifiersRefactoring
    {
        public static void AddModifier(
            CodeFixContext context,
            Diagnostic diagnostic,
            MemberDeclarationSyntax memberDeclaration,
            SyntaxKind kind)
        {
            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                $"Add '{ModifierHelper.GetModifierName(kind)}' modifier",
                cancellationToken => AddModifier(document, memberDeclaration, kind, cancellationToken),
                EquivalenceKeyProvider.GetEquivalenceKey(diagnostic, kind.ToString()));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static Task<Document> AddModifier(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            SyntaxKind kind,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newMemberDeclaration = AddModifier(memberDeclaration, kind);

            return document.ReplaceNodeAsync(memberDeclaration, newMemberDeclaration, cancellationToken);
        }

        private static MemberDeclarationSyntax AddModifier(
            MemberDeclarationSyntax memberDeclaration,
            SyntaxKind kind,
            IModifierComparer comparer = null)
        {
            switch (kind)
            {
                case SyntaxKind.AbstractKeyword:
                    {
                        memberDeclaration = memberDeclaration
                            .RemoveModifier(SyntaxKind.VirtualKeyword)
                            .RemoveModifier(SyntaxKind.OverrideKeyword);

                        break;
                    }
                case SyntaxKind.VirtualKeyword:
                    {
                        memberDeclaration = memberDeclaration
                            .RemoveModifier(SyntaxKind.AbstractKeyword)
                            .RemoveModifier(SyntaxKind.OverrideKeyword);

                        break;
                    }
                case SyntaxKind.OverrideKeyword:
                    {
                        memberDeclaration = memberDeclaration
                            .RemoveModifier(SyntaxKind.AbstractKeyword)
                            .RemoveModifier(SyntaxKind.VirtualKeyword);

                        break;
                    }
            }

            return memberDeclaration.InsertModifier(kind, comparer);
        }
    }
}