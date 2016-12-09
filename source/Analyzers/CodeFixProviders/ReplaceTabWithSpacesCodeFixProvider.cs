// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReplaceTabWithSpacesCodeFixProvider))]
    [Shared]
    public class ReplaceTabWithSpacesCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AvoidUsageOfTab); }
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            CodeAction codeAction = CodeAction.Create(
                "Replace tab with spaces",
                cancellationToken => AvoidUsageOfTabRefactoring.RefactorAsync(context.Document, context.Span, cancellationToken),
                DiagnosticIdentifiers.AvoidUsageOfTab + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);

            var tcs = new TaskCompletionSource<object>();

            tcs.SetResult(null);

            return tcs.Task;
        }
    }
}
