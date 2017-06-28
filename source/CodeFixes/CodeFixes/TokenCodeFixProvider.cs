// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.Helpers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TokenCodeFixProvider))]
    [Shared]
    public class TokenCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperandOfType,
                    CompilerDiagnosticIdentifiers.PartialModifierCanOnlyAppearImmediatelyBeforeClassStructInterfaceOrVoid);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.AddArgumentList,
                CodeFixIdentifiers.ReorderModifiers))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxToken token = root.FindToken(context.Span.Start);

            Debug.Assert(!token.IsKind(SyntaxKind.None), $"{nameof(token)} is none");

            SyntaxKind kind = token.Kind();

            if (kind == SyntaxKind.None)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperandOfType:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddArgumentList))
                                break;

                            if (kind != SyntaxKind.QuestionToken)
                                break;

                            if (!token.IsParentKind(SyntaxKind.ConditionalAccessExpression))
                                break;

                            var conditionalAccess = (ConditionalAccessExpressionSyntax)token.Parent;

                            CodeAction codeAction = CodeAction.Create(
                                "Add argument list",
                                cancellationToken =>
                                {
                                    InvocationExpressionSyntax invocationExpression = InvocationExpression(
                                        conditionalAccess.WithoutTrailingTrivia(),
                                        ArgumentList().WithTrailingTrivia(conditionalAccess.GetTrailingTrivia()));

                                    return context.Document.ReplaceNodeAsync(conditionalAccess, invocationExpression, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.PartialModifierCanOnlyAppearImmediatelyBeforeClassStructInterfaceOrVoid:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReorderModifiers))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Move 'partial' modifier",
                                cancellationToken =>
                                {
                                    SyntaxNode node = token.Parent;

                                    SyntaxNode newNode = RemoveModifierHelper.RemoveModifier(node, token);

                                    newNode = InsertModifierHelper.InsertModifier(newNode, SyntaxKind.PartialKeyword, ModifierComparer.Instance);

                                    return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
