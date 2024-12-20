// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseRegexInstanceInsteadOfStaticMethodCodeFixProvider))]
[Shared]
public sealed class UseRegexInstanceInsteadOfStaticMethodCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIds.UseRegexInstanceInsteadOfStaticMethod); }
    }

    public override FixAllProvider GetFixAllProvider()
    {
#if DEBUG
        return WellKnownFixAllProviders.BatchFixer;
#else
        return null;
#endif
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out InvocationExpressionSyntax invocation))
            return;

        CodeAction codeAction = CodeAction.Create(
            "Use Regex instance",
            ct => UseRegexInstanceInsteadOfStaticMethodRefactoring.RefactorAsync(context.Document, invocation, ct),
            GetEquivalenceKey(DiagnosticIds.UseRegexInstanceInsteadOfStaticMethod));

        context.RegisterCodeFix(codeAction, context.Diagnostics);
    }
}
