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
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddSemicolonCodeFixProvider))]
    [Shared]
    public class AddSemicolonCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.SemicolonExpected); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddSemicolon))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => CanHaveSemicolon(f)))
                return;

            if (node.SpanStart == context.Span.Start
                && !TryFindFirstAncestorOrSelf(root, new TextSpan(node.FullSpan.Start - 1, 0), out node, predicate: f => CanHaveSemicolon(f)))
            {
                return;
            }

            SyntaxToken semicolon = GetSemicolonToken(node);

            if (semicolon.IsKind(SyntaxKind.None))
                return;

            TextSpan textSpan = GetMissingSemicolonSpan(node, semicolon);

            if (textSpan == default(TextSpan))
                return;

            CodeAction codeAction = CodeAction.Create(
                "Add semicolon",
                cancellationToken =>
                {
                    var textChange = new TextChange(textSpan, ";");

                    return context.Document.WithTextChangeAsync(textChange, cancellationToken);
                },
                GetEquivalenceKey(CompilerDiagnosticIdentifiers.SemicolonExpected));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static bool CanHaveSemicolon(SyntaxNode node)
        {
            return node.IsKind(SyntaxKind.UsingDirective, SyntaxKind.ExternAliasDirective)
                || node is StatementSyntax
                || node is MemberDeclarationSyntax
                || node is AccessorDeclarationSyntax;
        }

        private SyntaxToken GetSemicolonToken(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;

                        if (methodDeclaration.ExpressionBody != null)
                            return methodDeclaration.SemicolonToken;

                        return default(SyntaxToken);
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)node;

                        if (constructorDeclaration.ExpressionBody != null)
                            return constructorDeclaration.SemicolonToken;

                        return default(SyntaxToken);
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        var destructorDeclaration = (DestructorDeclarationSyntax)node;

                        if (destructorDeclaration.ExpressionBody != null)
                            return destructorDeclaration.SemicolonToken;

                        return default(SyntaxToken);
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)node;

                        if (operatorDeclaration.ExpressionBody != null)
                            return operatorDeclaration.SemicolonToken;

                        return default(SyntaxToken);
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)node;

                        if (conversionOperatorDeclaration.ExpressionBody != null)
                            return conversionOperatorDeclaration.SemicolonToken;

                        return default(SyntaxToken);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)node;

                        if (propertyDeclaration.ExpressionBody != null)
                            return propertyDeclaration.SemicolonToken;

                        return default(SyntaxToken);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;

                        if (indexerDeclaration.ExpressionBody != null)
                            return indexerDeclaration.SemicolonToken;

                        return default(SyntaxToken);
                    }
                case SyntaxKind.FieldDeclaration:
                    {
                        return ((FieldDeclarationSyntax)node).SemicolonToken;
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        return ((EventFieldDeclarationSyntax)node).SemicolonToken;
                    }
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    {
                        var accessor = (AccessorDeclarationSyntax)node;

                        if (accessor.ExpressionBody != null)
                            return accessor.SemicolonToken;

                        return default(SyntaxToken);
                    }
                case SyntaxKind.BreakStatement:
                    return ((BreakStatementSyntax)node).SemicolonToken;
                case SyntaxKind.ContinueStatement:
                    return ((ContinueStatementSyntax)node).SemicolonToken;
                case SyntaxKind.DoStatement:
                    return ((DoStatementSyntax)node).SemicolonToken;
                case SyntaxKind.ExpressionStatement:
                    return ((ExpressionStatementSyntax)node).SemicolonToken;
                case SyntaxKind.GotoStatement:
                case SyntaxKind.GotoCaseStatement:
                case SyntaxKind.GotoDefaultStatement:
                    return ((GotoStatementSyntax)node).SemicolonToken;
                case SyntaxKind.LocalDeclarationStatement:
                    return ((LocalDeclarationStatementSyntax)node).SemicolonToken;
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)node).SemicolonToken;
                case SyntaxKind.ThrowStatement:
                    return ((ThrowStatementSyntax)node).SemicolonToken;
                case SyntaxKind.YieldBreakStatement:
                case SyntaxKind.YieldReturnStatement:
                    return ((YieldStatementSyntax)node).SemicolonToken;
                case SyntaxKind.UsingDirective:
                    return ((UsingDirectiveSyntax)node).SemicolonToken;
                case SyntaxKind.ExternAliasDirective:
                    return ((ExternAliasDirectiveSyntax)node).SemicolonToken;
            }

            Debug.Fail(node.Kind().ToString());

            return default(SyntaxToken);
        }

        private static TextSpan GetMissingSemicolonSpan(SyntaxNode node, SyntaxToken semicolon)
        {
            if (semicolon.IsMissing)
            {
                SyntaxToken token = node.GetLastToken();

                if (!token.IsKind(SyntaxKind.None))
                {
                    SyntaxTriviaList trailingTrivia = token.TrailingTrivia;

                    if (trailingTrivia.Any())
                        return new TextSpan(trailingTrivia.Span.Start, 0);
                }
            }

            return default(TextSpan);
        }
    }
}
