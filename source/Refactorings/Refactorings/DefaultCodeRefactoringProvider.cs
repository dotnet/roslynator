// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;

namespace Roslynator.CSharp.Refactorings
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(DefaultCodeRefactoringProvider))]
    public class DefaultCodeRefactoringProvider : CodeRefactoringProvider
    {
        public static RefactoringSettings DefaultSettings { get; } = new RefactoringSettings();

        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);
#if DEBUG
            try
            {
#endif
                await ComputeRefactoringsAsync(new RefactoringContext(context, root, DefaultSettings)).ConfigureAwait(false);
#if DEBUG
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Debug.Assert(false, nameof(DefaultCodeRefactoringProvider));
                throw;
            }
#endif
        }

        private static async Task ComputeRefactoringsAsync(RefactoringContext context)
        {
            await context.ComputeRefactoringsAsync().ConfigureAwait(false);
        }
    }
}
