// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
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
                    DiagnosticIdentifiers.ReplaceAnyMethodWithCountOrLengthProperty,
                    DiagnosticIdentifiers.ReplaceCountMethodWithCountOrLengthProperty,
                    DiagnosticIdentifiers.UseBitwiseOperationInsteadOfHasFlagMethod,
                    DiagnosticIdentifiers.RemoveRedundantToStringCall);
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
                            SimplifyLinqMethodChainRefactoring.RegisterCodeFix(context, diagnostic, invocation);
                            break;
                        }
                    case DiagnosticIdentifiers.ReplaceAnyMethodWithCountOrLengthProperty:
                        {
                            ReplaceAnyMethodWithCountOrLengthPropertyRefactoring.RegisterCodeFix(context, diagnostic, invocation);
                            break;
                        }
                    case DiagnosticIdentifiers.ReplaceCountMethodWithCountOrLengthProperty:
                        {
                            ReplaceCountMethodWithCountOrLengthPropertyRefactoring.RegisterCodeFix(context, diagnostic, invocation);
                            break;
                        }
                    case DiagnosticIdentifiers.UseBitwiseOperationInsteadOfHasFlagMethod:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                ReplaceHasFlagWithBitwiseOperationRefactoring.Title,
                                cancellationToken =>
                                {
                                    return ReplaceHasFlagWithBitwiseOperationRefactoring.RefactorAsync(
                                        context.Document,
                                        invocation,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantToStringCall:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant 'ToString' call",
                                cancellationToken =>
                                {
                                    return RemoveRedundantToStringCallRefactoring.RefactorAsync(
                                        context.Document,
                                        invocation,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
