// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DocumentCodeFixProvider))]
    [Shared]
    public class DocumentCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveFileWithNoCode); }
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.RemoveFileWithNoCode:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove file with no code",
                                cancellationToken => RemoveDocumentAsync(context.Document, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }

            var tcs = new TaskCompletionSource<object>();

            tcs.SetResult(null);

            return tcs.Task;
        }

        private static Task<Solution> RemoveDocumentAsync(
            Document document,
            CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Solution>();

            Solution newSolution = document.Project.Solution.RemoveDocument(document.Id);

            tcs.SetResult(newSolution);

            return tcs.Task;
        }
    }
}
