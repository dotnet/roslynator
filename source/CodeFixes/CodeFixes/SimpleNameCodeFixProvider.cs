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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SimpleNameCodeFixProvider))]
    [Shared]
    public class SimpleNameCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CannotConvertMethodGroupToNonDelegateType); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddArgumentList))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SimpleNameSyntax simpleName = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<SimpleNameSyntax>();

            Debug.Assert(simpleName != null, $"{nameof(simpleName)} is null");

            if (simpleName == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CannotConvertMethodGroupToNonDelegateType:
                        {
                            if (!simpleName.IsParentKind(SyntaxKind.SimpleMemberAccessExpression))
                                break;

                            var memberAccess = (MemberAccessExpressionSyntax)simpleName.Parent;

                            CodeAction codeAction = CodeAction.Create(
                                "Add argument list",
                                cancellationToken =>
                                {
                                    InvocationExpressionSyntax invocationExpression = InvocationExpression(
                                        memberAccess.WithoutTrailingTrivia(),
                                        ArgumentList().WithTrailingTrivia(memberAccess.GetTrailingTrivia()));

                                    return context.Document.ReplaceNodeAsync(memberAccess, invocationExpression, cancellationToken);
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
