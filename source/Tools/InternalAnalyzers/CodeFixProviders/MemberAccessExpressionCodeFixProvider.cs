// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Internal.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberAccessExpressionCodeFixProvider))]
    [Shared]
    public class MemberAccessExpressionCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.SimplifyGetTypeInfoInvocation); }
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

            MemberAccessExpressionSyntax memberAccess = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberAccessExpressionSyntax>();

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.SimplifyGetTypeInfoInvocation:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Replace 'GetTypeInfo' with '{GetMethodName(memberAccess)}'",
                                cancellationToken =>
                                {
                                    return RefactorAsync(
                                        context.Document,
                                        memberAccess,
                                        cancellationToken);
                                },
                                DiagnosticIdentifiers.SimplifyGetTypeInfoInvocation);

                            context.RegisterCodeFix(codeAction, diagnostic);

                            break;
                        }
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            MemberAccessExpressionSyntax memberAccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var invocation = (InvocationExpressionSyntax)memberAccess.Expression;

            var memberAccess2 = (MemberAccessExpressionSyntax)invocation.Expression;

            InvocationExpressionSyntax newInvocation = invocation
                .WithExpression(
                    memberAccess2
                        .WithName(IdentifierName(GetMethodName(memberAccess)).WithTriviaFrom(memberAccess2.Name)));

            SyntaxNode newRoot = root.ReplaceNode(
                memberAccess,
                newInvocation.WithTriviaFrom(memberAccess));

            return document.WithSyntaxRoot(newRoot);
        }

        private static string GetMethodName(MemberAccessExpressionSyntax memberAccess)
        {
            switch (memberAccess.Name.Identifier.ValueText)
            {
                case "Type":
                    return "GetTypeSymbol";
                case "ConvertedType":
                    return "GetConvertedTypeSymbol";
                default:
                    {
                        Debug.Assert(false, memberAccess.Name.Identifier.ValueText);
                        return "";
                    }
            }
        }
    }
}
