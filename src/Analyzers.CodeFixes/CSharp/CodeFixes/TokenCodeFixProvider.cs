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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TokenCodeFixProvider))]
    [Shared]
    public sealed class TokenCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UnnecessaryNullForgivingOperator); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f => f.IsKind(SyntaxKind.EqualsValueClause, SyntaxKind.SuppressNullableWarningExpression)))
            {
                return;
            }

            Diagnostic diagnostic = context.Diagnostics[0];
            Document document = context.Document;

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.UnnecessaryNullForgivingOperator:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Remove null-forgiving operator",
                            ct =>
                            {
                                if (node.Parent is PropertyDeclarationSyntax propertyDeclaration)
                                {
                                    SyntaxToken semicolonToken = propertyDeclaration.SemicolonToken;

                                    SyntaxToken token = propertyDeclaration.Initializer.GetFirstToken().GetPreviousToken();

                                    SyntaxTriviaList newTrivia = token.TrailingTrivia
                                        .AddRange(node.GetLeadingAndTrailingTrivia())
                                        .AddRange(semicolonToken.LeadingTrivia)
                                        .EmptyIfWhitespace()
                                        .AddRange(semicolonToken.TrailingTrivia);

                                    PropertyDeclarationSyntax newNode = propertyDeclaration
                                        .ReplaceToken(token, token.WithTrailingTrivia(newTrivia))
                                        .WithInitializer(null)
                                        .WithSemicolonToken(default);

                                    return document.ReplaceNodeAsync(propertyDeclaration, newNode, ct);
                                }
                                else if (node.Parent.Parent.IsParentKind(SyntaxKind.FieldDeclaration))
                                {
                                    var variableDeclarator = (VariableDeclaratorSyntax)node.Parent;

                                    SyntaxToken token = variableDeclarator.Initializer.GetFirstToken().GetPreviousToken();

                                    SyntaxTriviaList newTrivia = token.TrailingTrivia
                                        .AddRange(node.GetLeadingAndTrailingTrivia())
                                        .EmptyIfWhitespace();

                                    VariableDeclaratorSyntax newNode = variableDeclarator
                                        .ReplaceToken(token, token.WithTrailingTrivia(newTrivia))
                                        .WithInitializer(null);

                                    return document.ReplaceNodeAsync(variableDeclarator, newNode, ct);
                                }
                                else
                                {
                                    var expression = (PostfixUnaryExpressionSyntax)node;

                                    ExpressionSyntax newExpression = expression.Operand
                                        .AppendToTrailingTrivia(expression.OperatorToken.LeadingAndTrailingTrivia());

                                    return document.ReplaceNodeAsync(expression, newExpression, ct);
                                }
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }
    }
}
