// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NewLineCodeFixProvider))]
    [Shared]
    public class NewLineCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.UseLinefeedAsNewLine,
                    DiagnosticIdentifiers.UseCarriageReturnAndLinefeedAsNewLine);
            }
        }

        public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case  DiagnosticIdentifiers.UseLinefeedAsNewLine:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use linefeed as newline",
                                cancellationToken => UseLinefeedAsNewLineRefactoring.RefactorAsync(context.Document, context.Span, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case  DiagnosticIdentifiers.UseCarriageReturnAndLinefeedAsNewLine:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use carriage return + linefeed as newline",
                                cancellationToken => UseCarriageReturnAndLinefeedAsNewLineRefactoring.RefactorAsync(context.Document, context.Span, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }

            return Task.CompletedTask;
        }
    }
}

