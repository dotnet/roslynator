// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator;

internal static class CodeActionFactory
{
    public static void CreateAndRegisterCodeActionForBlankLine(CodeFixContext context, SyntaxNode root, Diagnostic diagnostic)
    {
        TextChange textChange = TriviaBetweenAnalysis.GetTextChangeForBlankLine(root, context.Span.Start);

        CodeAction codeAction = CodeAction.Create(
            (textChange.NewText.Length == 0) ? "Remove blank line" : "Add blank line",
            ct => context.Document.WithTextChangeAsync(textChange, ct),
            EquivalenceKey.Create(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }
}
