// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;

namespace Roslynator.CSharp.Refactorings;

[ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(RoslynatorCodeRefactoringProvider))]
public sealed class RoslynatorCodeRefactoringProvider : CodeRefactoringProvider
{
    public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
    {
        SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
#if DEBUG
        try
        {
            var refactoringContext = new RefactoringContext(context, root);
            await refactoringContext.ComputeRefactoringsAsync().ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Debug.Fail(nameof(RoslynatorCodeRefactoringProvider));
            throw;
        }
#else
        var refactoringContext = new RefactoringContext(context, root);
        await refactoringContext.ComputeRefactoringsAsync().ConfigureAwait(false);
#endif
    }
}
