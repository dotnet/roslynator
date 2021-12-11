// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(YieldStatementCodeFixProvider))]
    [Shared]
    public sealed class YieldStatementCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS1621_YieldStatementCannotBeUsedInsideAnonymousMethodOrLambdaExpression); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveYieldKeyword, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out YieldStatementSyntax yieldStatement))
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove 'yield'",
                ct =>
                {
                    SyntaxToken yieldKeyword = yieldStatement.YieldKeyword;
                    SyntaxToken returnKeyword = yieldStatement.ReturnOrBreakKeyword;

                    SyntaxTriviaList leadingTrivia = yieldKeyword.LeadingTrivia
                        .AddRange(yieldKeyword.TrailingTrivia.EmptyIfWhitespace())
                        .AddRange(returnKeyword.LeadingTrivia.EmptyIfWhitespace());

                    ReturnStatementSyntax newNode = ReturnStatement(
                        returnKeyword.WithLeadingTrivia(leadingTrivia),
                        yieldStatement.Expression,
                        yieldStatement.SemicolonToken);

                    return context.Document.ReplaceNodeAsync(yieldStatement, newNode, ct);
                },
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
