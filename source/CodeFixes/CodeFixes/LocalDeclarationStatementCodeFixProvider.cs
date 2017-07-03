// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalDeclarationStatementCodeFixProvider))]
    [Shared]
    public class LocalDeclarationStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.LocalVariableOrFunctionIsAlreadyDefinedInThisScope,
                    CompilerDiagnosticIdentifiers.LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReplaceVariableDeclarationWithAssignment))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            LocalDeclarationStatementSyntax localDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<LocalDeclarationStatementSyntax>();

            if (localDeclaration == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.LocalVariableOrFunctionIsAlreadyDefinedInThisScope:
                    case CompilerDiagnosticIdentifiers.LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReplaceVariableDeclarationWithAssignment))
                                break;

                            VariableDeclaratorSyntax variableDeclarator = localDeclaration.Declaration?.SingleVariableOrDefault();

                            if (variableDeclarator == null)
                                break;

                            ExpressionSyntax value = variableDeclarator.Initializer?.Value;

                            if (value == null)
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            VariableDeclaratorSyntax variableDeclarator2 = FindVariableDeclarator(
                                variableDeclarator.Identifier.ValueText,
                                semanticModel.GetEnclosingSymbolSyntax(localDeclaration.SpanStart, context.CancellationToken));

                            if (variableDeclarator2?.SpanStart < localDeclaration.SpanStart)
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Replace variable declaration with assignment",
                                    cancellationToken =>
                                    {
                                        ExpressionStatementSyntax newNode = CSharpFactory.SimpleAssignmentStatement(
                                            SyntaxFactory.IdentifierName(variableDeclarator.Identifier),
                                            value);

                                        newNode = newNode
                                            .WithTriviaFrom(localDeclaration)
                                            .WithFormatterAnnotation();

                                        return context.Document.ReplaceNodeAsync(localDeclaration, newNode, context.CancellationToken);
                                    },
                                    GetEquivalenceKey(diagnostic));
                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                }
            }
        }

        private static VariableDeclaratorSyntax FindVariableDeclarator(string name, SyntaxNode node)
        {
            foreach (SyntaxNode descendant in node.DescendantNodes())
            {
                if (descendant.IsKind(SyntaxKind.VariableDeclarator))
                {
                    var variableDeclarator = (VariableDeclaratorSyntax)descendant;

                    if (string.Equals(name, variableDeclarator.Identifier.ValueText, StringComparison.Ordinal))
                        return variableDeclarator;
                }
            }

            return null;
        }
    }
}
