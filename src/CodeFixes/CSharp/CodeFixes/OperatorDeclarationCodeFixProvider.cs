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

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OperatorDeclarationCodeFixProvider))]
    [Shared]
    public class OperatorDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.OperatorRequiresMatchingOperatorToAlsoBeDefined); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.DefineMatchingOperator))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out OperatorDeclarationSyntax operatorDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.OperatorRequiresMatchingOperatorToAlsoBeDefined:
                        {
                            SyntaxToken token = operatorDeclaration.OperatorToken;

                            SyntaxKind matchingKind = GetMatchingOperatorToken(token);

                            if (matchingKind == SyntaxKind.None)
                                break;

                            SyntaxToken newToken = SyntaxFactory.Token(token.LeadingTrivia, matchingKind, token.TrailingTrivia);

                            if (operatorDeclaration.BodyOrExpressionBody() == null)
                                break;

                            if (!(operatorDeclaration.Parent is TypeDeclarationSyntax typeDeclaration))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                $"Generate {newToken} operator",
                                cancellationToken =>
                                {
                                    OperatorDeclarationSyntax newNode = operatorDeclaration.WithOperatorToken(newToken);

                                    return context.Document.InsertNodeAfterAsync(operatorDeclaration, newNode, cancellationToken);
                                },
                                EquivalenceKey.Create(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
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
