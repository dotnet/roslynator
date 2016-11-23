// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Refactorings.ReplaceCountMethod;

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
                    DiagnosticIdentifiers.RemoveRedundantToStringCall,
                    DiagnosticIdentifiers.RemoveRedundantStringToCharArrayCall,
                    DiagnosticIdentifiers.UseCastMethodInsteadOfSelectMethod,
                    DiagnosticIdentifiers.CombineEnumerableWhereMethodChain);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            InvocationExpressionSyntax invocation = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InvocationExpressionSyntax>();

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.SimplifyLinqMethodChain:
                        {
                            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                            switch (memberAccess.Name.Identifier.ValueText)
                            {
                                case "Cast":
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Simplify method chain",
                                            cancellationToken => ReplaceWhereAndCastWithOfTypeRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                            diagnostic.Id + EquivalenceKeySuffix);

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                        break;
                                    }
                                default:
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Simplify method chain",
                                            cancellationToken => SimplifyLinqMethodChainRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                            diagnostic.Id + EquivalenceKeySuffix);

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                        break;
                                    }
                            }

                            break;
                        }
                    case DiagnosticIdentifiers.CombineEnumerableWhereMethodChain:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Combine 'Where' method chain",
                                cancellationToken => CombineEnumerableWhereMethodChainRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.ReplaceAnyMethodWithCountOrLengthProperty:
                        {
                            string propertyName = diagnostic.Properties["PropertyName"];
                            string sign = (invocation.Parent?.IsKind(SyntaxKind.LogicalNotExpression) == true) ? "==" : ">";

                            CodeAction codeAction = CodeAction.Create(
                                $"Replace 'Any' with '{propertyName} {sign} 0'",
                                cancellationToken => ReplaceAnyMethodWithCountOrLengthPropertyRefactoring.RefactorAsync(context.Document, invocation, propertyName, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.ReplaceCountMethodWithCountOrLengthProperty:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Replace 'Count()' with '{diagnostic.Properties["PropertyName"]}'",
                                cancellationToken => ReplaceCountMethodWithCountOrLengthPropertyRefactoring.RefactorAsync(context.Document, invocation, diagnostic.Properties["PropertyName"], cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseBitwiseOperationInsteadOfHasFlagMethod:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                ReplaceHasFlagWithBitwiseOperationRefactoring.Title,
                                cancellationToken => ReplaceHasFlagWithBitwiseOperationRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantToStringCall:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant 'ToString' call",
                                cancellationToken => RemoveRedundantCallRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseCastMethodInsteadOfSelectMethod:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Call 'Cast' instead of 'Select'",
                                cancellationToken => ReplaceSelectWithCastRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantStringToCharArrayCall:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant 'ToCharArray' call",
                                cancellationToken => RemoveRedundantCallRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
