// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveEmptySyntaxCodeFixProvider))]
[Shared]
public sealed class RemoveEmptySyntaxCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveEmptySyntax); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(
            root,
            context.Span,
            out SyntaxNode node,
            findInsideTrivia: true,
            predicate: f =>
            {
                switch (f.Kind())
                {
                    case SyntaxKind.DestructorDeclaration:
                    case SyntaxKind.ElseClause:
                    case SyntaxKind.EmptyStatement:
                    case SyntaxKind.FinallyClause:
                    case SyntaxKind.NamespaceDeclaration:
                    case SyntaxKind.ObjectCreationExpression:
                    case SyntaxKind.RegionDirectiveTrivia:
                        return true;
                    default:
                        return false;
                }
            }))
        {
            return;
        }

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (node)
        {
            case DestructorDeclarationSyntax destructorDeclaration:
                {
                    CodeAction codeAction = CodeActionFactory.RemoveMemberDeclaration(
                        document,
                        destructorDeclaration,
                        title: "Remove empty destructor",
                        equivalenceKey: GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case ElseClauseSyntax elseClause:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Remove empty 'else' clause",
                        ct => ElseClauseCodeFixProvider.RemoveEmptyElseClauseAsync(document, elseClause, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case FinallyClauseSyntax finallyClause:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Remove empty 'finally' clause",
                        ct => FinallyClauseCodeFixProvider.RemoveEmptyFinallyClauseAsync(document, finallyClause, ct),
                        equivalenceKey: GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case ObjectCreationExpressionSyntax objectCreationExpression:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Remove empty initializer",
                        ct => RemoveEmptyInitializerRefactoring.RefactorAsync(document, objectCreationExpression, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case NamespaceDeclarationSyntax namespaceDeclaration:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Remove empty namespace declaration",
                        ct => RemoveEmptyNamespaceDeclarationRefactoring.RefactorAsync(document, namespaceDeclaration, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case RegionDirectiveTriviaSyntax regionDirective:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Remove empty region",
                        ct => RemoveEmptyRegionRefactoring.RefactorAsync(document, SyntaxInfo.RegionInfo(regionDirective), ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case EmptyStatementSyntax emptyStatement:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Remove empty statement",
                        ct => RemoveEmptyStatementRefactoring.RefactorAsync(document, emptyStatement, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
        }
    }
}
