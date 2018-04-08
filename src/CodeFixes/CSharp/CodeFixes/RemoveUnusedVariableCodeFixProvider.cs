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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveUnusedVariableCodeFixProvider))]
    [Shared]
    public class RemoveUnusedVariableCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.VariableIsDeclaredButNeverUsed,
                    CompilerDiagnosticIdentifiers.VariableIsAssignedButItsValueIsNeverUsed);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveUnusedVariable))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken token))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.VariableIsDeclaredButNeverUsed:
                    case CompilerDiagnosticIdentifiers.VariableIsAssignedButItsValueIsNeverUsed:
                        {
                            switch (token.Parent.Kind())
                            {
                                case SyntaxKind.VariableDeclarator:
                                    {
                                        var variableDeclarator = (VariableDeclaratorSyntax)token.Parent;

                                        var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

                                        if (variableDeclaration.Variables.Count == 1)
                                        {
                                            var localDeclarationStatement = (LocalDeclarationStatementSyntax)variableDeclaration.Parent;

                                            if (!localDeclarationStatement.IsEmbedded()
                                                && !localDeclarationStatement.SpanContainsDirectives())
                                            {
                                                CodeAction codeAction = CodeAction.Create(
                                                    "Remove unused variable",
                                                    cancellationToken => context.Document.RemoveNodeAsync(localDeclarationStatement, cancellationToken),
                                                    GetEquivalenceKey(diagnostic));

                                                context.RegisterCodeFix(codeAction, diagnostic);
                                            }
                                        }
                                        else if (!variableDeclarator.SpanContainsDirectives())
                                        {
                                            CodeAction codeAction = CodeAction.Create(
                                                "Remove unused variable",
                                                cancellationToken => context.Document.RemoveNodeAsync(variableDeclarator, cancellationToken),
                                                GetEquivalenceKey(diagnostic));

                                            context.RegisterCodeFix(codeAction, diagnostic);
                                        }

                                        break;
                                    }
                                case SyntaxKind.CatchDeclaration:
                                    {
                                        var catchDeclaration = (CatchDeclarationSyntax)token.Parent;

                                        CodeAction codeAction = CodeAction.Create(
                                            "Remove unused variable",
                                            cancellationToken =>
                                            {
                                                CatchDeclarationSyntax newNode = catchDeclaration
                                                    .WithIdentifier(default(SyntaxToken))
                                                    .WithCloseParenToken(catchDeclaration.CloseParenToken.PrependToLeadingTrivia(token.GetAllTrivia()))
                                                    .WithFormatterAnnotation();

                                                return context.Document.ReplaceNodeAsync(catchDeclaration, newNode, cancellationToken);
                                            },
                                            GetEquivalenceKey(diagnostic));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                        break;
                                    }
                                case SyntaxKind.LocalFunctionStatement:
                                    {
                                        var localFunction = (LocalFunctionStatementSyntax)token.Parent;

                                        CodeAction codeAction = CodeAction.Create(
                                            "Remove unused local function",
                                            cancellationToken => context.Document.RemoveStatementAsync(localFunction, cancellationToken),
                                            GetEquivalenceKey(diagnostic, "LocalFunction"));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                        break;
                                    }
                            }

                            break;
                        }
                }
            }
        }
    }
}
