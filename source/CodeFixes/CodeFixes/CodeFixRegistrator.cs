// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    internal static class CodeFixRegistrator
    {
        public static void RemoveMember(CodeFixContext context, Diagnostic diagnostic, MemberDeclarationSyntax memberDeclaration, string additionalKey = null)
        {
            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                $"Remove {memberDeclaration.GetTitle()}",
                cancellationToken => document.RemoveMemberAsync(memberDeclaration, cancellationToken),
                EquivalenceKeyProvider.GetEquivalenceKey(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static void RemoveStatement(
            CodeFixContext context,
            Diagnostic diagnostic,
            StatementSyntax statement,
            string title = null,
            string additionalKey = null)
        {
            if (statement.IsEmbedded())
                return;

            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                title ?? $"Remove {statement.GetTitle()}",
                cancellationToken => document.RemoveStatementAsync(statement, cancellationToken),
                EquivalenceKeyProvider.GetEquivalenceKey(diagnostic, additionalKey));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
