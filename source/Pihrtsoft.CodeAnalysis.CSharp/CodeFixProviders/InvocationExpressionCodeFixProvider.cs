// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders.CodeFixes;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InvocationExpressionCodeFixProvider))]
    [Shared]
    public class InvocationExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.SimplifyLinqMethodChain,
                    DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfAnyMethod);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            InvocationExpressionSyntax invocation = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InvocationExpressionSyntax>();

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.SimplifyLinqMethodChain:
                        {
                            SimplifyLinqMethodChainCodeFix.Register(context, diagnostic, invocation);
                            break;
                        }
                    case DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfAnyMethod:
                        {
                            UseCountOrLengthPropertyInsteadOfAnyMethodCodeFix.Register(context, diagnostic, invocation);
                            break;
                        }
                }
            }
        }
    }
}
