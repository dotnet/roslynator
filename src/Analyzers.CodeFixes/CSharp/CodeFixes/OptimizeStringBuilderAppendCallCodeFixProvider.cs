// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptimizeStringBuilderAppendCallCodeFixProvider))]
    [Shared]
    public class OptimizeStringBuilderAppendCallCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.OptimizeStringBuilderAppendCall); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                getInnermostNodeForTie: false,
                predicate: f => f.IsKind(SyntaxKind.Argument, SyntaxKind.InvocationExpression)))
            {
                return;
            }

            Diagnostic diagnostic = context.Diagnostics[0];

            if (node is ArgumentSyntax argument)
            {
                SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo((InvocationExpressionSyntax)argument.Parent.Parent);

                CodeAction codeAction = CodeAction.Create(
                    $"Optimize '{invocationInfo.NameText}' call",
                    cancellationToken => OptimizeStringBuilderAppendCallRefactoring.RefactorAsync(context.Document, argument, invocationInfo, cancellationToken),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else if (node is InvocationExpressionSyntax invocationExpression)
            {
                SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

                CodeAction codeAction = CodeAction.Create(
                    $"Optimize '{invocationInfo.NameText}' call",
                    cancellationToken => OptimizeStringBuilderAppendCallRefactoring.RefactorAsync(context.Document, invocationInfo, cancellationToken),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }
    }
}
