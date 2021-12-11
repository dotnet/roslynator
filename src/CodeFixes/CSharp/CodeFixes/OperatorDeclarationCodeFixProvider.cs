// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OperatorDeclarationCodeFixProvider))]
    [Shared]
    public sealed class OperatorDeclarationCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS0216_OperatorRequiresMatchingOperatorToAlsoBeDefined); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.DefineMatchingOperator, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out OperatorDeclarationSyntax operatorDeclaration))
                return;

            SyntaxToken token = operatorDeclaration.OperatorToken;

            SyntaxKind matchingKind = GetMatchingOperatorToken(token);

            if (matchingKind == SyntaxKind.None)
                return;

            SyntaxToken newToken = SyntaxFactory.Token(token.LeadingTrivia, matchingKind, token.TrailingTrivia);

            if (operatorDeclaration.BodyOrExpressionBody() == null)
                return;

            if (operatorDeclaration.Parent is not TypeDeclarationSyntax typeDeclaration)
                return;

            CodeAction codeAction = CodeAction.Create(
                $"Generate {newToken} operator",
                ct =>
                {
                    OperatorDeclarationSyntax newNode = operatorDeclaration.WithOperatorToken(newToken);

                    return context.Document.InsertNodeAfterAsync(operatorDeclaration, newNode, ct);
                },
                EquivalenceKey.Create(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static SyntaxKind GetMatchingOperatorToken(SyntaxToken operatorToken)
        {
            switch (operatorToken.Kind())
            {
                case SyntaxKind.TrueKeyword:
                    return SyntaxKind.FalseKeyword;
                case SyntaxKind.FalseKeyword:
                    return SyntaxKind.TrueKeyword;
                case SyntaxKind.EqualsEqualsToken:
                    return SyntaxKind.ExclamationEqualsToken;
                case SyntaxKind.ExclamationEqualsToken:
                    return SyntaxKind.EqualsEqualsToken;
            }

            return SyntaxKind.None;
        }
    }
}
