// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnnecessaryEnumFlagCodeFixProvider))]
[Shared]
public sealed class UnnecessaryEnumFlagCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.UnnecessaryEnumFlag); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberAccessExpressionSyntax memberAccessExpression))
            return;

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case DiagnosticIdentifiers.UnnecessaryEnumFlag:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Remove unnecessary flag",
                        ct =>
                        {
                            var bitwiseOr = (BinaryExpressionSyntax)memberAccessExpression.Parent;

                            ExpressionSyntax newExpression = (bitwiseOr.Left == memberAccessExpression)
                                ? bitwiseOr.Right
                                : bitwiseOr.Left;

                            newExpression = newExpression.WithTriviaFrom(bitwiseOr);

                            return document.ReplaceNodeAsync(bitwiseOr, newExpression, ct);
                        },
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
        }
    }
}
