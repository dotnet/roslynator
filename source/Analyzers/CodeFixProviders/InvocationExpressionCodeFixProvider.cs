// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Refactorings.UseInsteadOfCountMethod;

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
                    DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfAnyMethod,
                    DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfCountMethod,
                    DiagnosticIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag,
                    DiagnosticIdentifiers.RemoveRedundantToStringCall,
                    DiagnosticIdentifiers.RemoveRedundantStringToCharArrayCall,
                    DiagnosticIdentifiers.UseCastMethodInsteadOfSelectMethod,
                    DiagnosticIdentifiers.CombineEnumerableWhereMethodChain,
                    DiagnosticIdentifiers.CallFindMethodInsteadOfFirstOrDefaultMethod,
                    DiagnosticIdentifiers.UseElementAccessInsteadOfElementAt,
                    DiagnosticIdentifiers.UseElementAccessInsteadOfFirst,
                    DiagnosticIdentifiers.CallStringConcatInsteadOfStringJoin);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

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

                            string name = memberAccess.Name.Identifier.ValueText;

                            if (name == "Cast")
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Simplify method chain",
                                    cancellationToken => CallOfTypeInsteadOfWhereAndCastRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                    diagnostic.Id + EquivalenceKeySuffix);

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }
                            else if (name == "Any"
                                && invocation.ArgumentList.Arguments.Count == 1)
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Simplify method chain",
                                    cancellationToken => CombineEnumerableWhereAndAnyRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                    diagnostic.Id + EquivalenceKeySuffix);

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }
                            else
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Simplify method chain",
                                    cancellationToken => SimplifyLinqMethodChainRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                    diagnostic.Id + EquivalenceKeySuffix);

                                context.RegisterCodeFix(codeAction, diagnostic);
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
                    case DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfAnyMethod:
                        {
                            string propertyName = diagnostic.Properties["PropertyName"];

                            CodeAction codeAction = CodeAction.Create(
                                $"Use '{propertyName}' property instead of calling 'Any'",
                                cancellationToken => UseCountOrLengthPropertyInsteadOfAnyMethodRefactoring.RefactorAsync(context.Document, invocation, propertyName, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfCountMethod:
                        {
                            string propertyName = diagnostic.Properties["PropertyName"];

                            CodeAction codeAction = CodeAction.Create(
                                $"Use '{propertyName}' property instead of calling 'Count'",
                                cancellationToken => UseCountOrLengthPropertyInsteadOfCountMethodRefactoring.RefactorAsync(context.Document, invocation, diagnostic.Properties["PropertyName"], cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                UseBitwiseOperationInsteadOfCallingHasFlagRefactoring.Title,
                                cancellationToken => UseBitwiseOperationInsteadOfCallingHasFlagRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
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
                                cancellationToken => UseCastMethodInsteadOfSelectMethodRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
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
                    case DiagnosticIdentifiers.CallFindMethodInsteadOfFirstOrDefaultMethod:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Call 'Find' instead of 'FirstOrDefault'",
                                cancellationToken => CallFindMethodInsteadOfFirstOrDefaultMethodRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseElementAccessInsteadOfElementAt:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use [] instead of calling 'ElementAt'",
                                cancellationToken => UseElementAccessInsteadOfElementAtRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseElementAccessInsteadOfFirst:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use [] instead of calling 'First'",
                                cancellationToken => UseElementAccessInsteadOfFirstRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.CallStringConcatInsteadOfStringJoin:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Call 'Concat' instead of 'Join'",
                                cancellationToken => CallStringConcatInsteadOfStringJoinRefactoring.RefactorAsync(context.Document, invocation, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
