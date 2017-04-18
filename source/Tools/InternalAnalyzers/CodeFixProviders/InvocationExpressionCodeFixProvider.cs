// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Internal.DiagnosticAnalyzers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Internal.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InvocationExpressionCodeFixProvider))]
    [Shared]
    public class InvocationExpressionCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.ReplaceIsKindMethodInvocation); }
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
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
                    case DiagnosticIdentifiers.ReplaceIsKindMethodInvocation:
                        {
                            SimpleNameSyntax name = InvocationExpressionDiagnosticAnalyzer.GetMethodName(invocation.Expression);
                            string oldValue = invocation.ToString().Substring(name.SpanStart - invocation.SpanStart);
                            string newName = diagnostic.Properties["MethodName"];

                            CodeAction codeAction = CodeAction.Create(
                                $"Replace '{oldValue}' with '{newName}()'",
                                cancellationToken =>
                                {
                                    return RefactorAsync(
                                        context.Document,
                                        invocation,
                                        newName,
                                        cancellationToken);
                                },
                                DiagnosticIdentifiers.ReplaceIsKindMethodInvocation);

                            context.RegisterCodeFix(codeAction, diagnostic);

                            break;
                        }
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            string newName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            InvocationExpressionSyntax newNode = GetNewNode(invocation, newName)
                .WithTriviaFrom(invocation);

            root = root.ReplaceNode(invocation, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static InvocationExpressionSyntax GetNewNode(
            InvocationExpressionSyntax invocation,
            string newName)
        {
            ExpressionSyntax expression = invocation.Expression;

            switch (expression.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)expression;

                        return invocation
                            .WithExpression(
                                memberAccess.WithName(IdentifierName(newName)))
                            .WithArgumentList(ArgumentList());
                    }
                case SyntaxKind.MemberBindingExpression:
                    {
                        var memberBinding = (MemberBindingExpressionSyntax)expression;

                        return invocation
                            .WithExpression(
                                memberBinding.WithName(IdentifierName(newName)))
                            .WithArgumentList(ArgumentList());
                    }
            }

            return invocation;
        }
    }
}
