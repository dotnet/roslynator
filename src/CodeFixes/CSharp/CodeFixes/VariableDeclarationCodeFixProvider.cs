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
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(VariableDeclarationCodeFixProvider))]
    [Shared]
    public class VariableDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.ImplicitlyTypedVariablesCannotHaveMultipleDeclarators,
                    CompilerDiagnosticIdentifiers.ImplicitlyTypedVariablesCannotBeConstant,
                    CompilerDiagnosticIdentifiers.LocalVariableOrFunctionIsAlreadyDefinedInThisScope,
                    CompilerDiagnosticIdentifiers.LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.UseExplicitTypeInsteadOfVar,
                CodeFixIdentifiers.ReplaceVariableDeclarationWithAssignment))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f => f.IsKind(SyntaxKind.VariableDeclaration, SyntaxKind.ForEachStatement, SyntaxKind.Parameter, SyntaxKind.DeclarationPattern)))
            {
                return;
            }

            if (node.IsKind(SyntaxKind.Parameter, SyntaxKind.DeclarationPattern))
                return;

            if (!(node is VariableDeclarationSyntax variableDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.ImplicitlyTypedVariablesCannotHaveMultipleDeclarators:
                    case CompilerDiagnosticIdentifiers.ImplicitlyTypedVariablesCannotBeConstant:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.UseExplicitTypeInsteadOfVar))
                                return;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            TypeSyntax type = variableDeclaration.Type;

                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                            if (typeSymbol?.SupportsExplicitDeclaration() == true)
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    $"Change type to '{SymbolDisplay.ToMinimalDisplayString(typeSymbol, semanticModel, type.SpanStart, SymbolDisplayFormats.Default)}'",
                                    cancellationToken => ChangeTypeRefactoring.ChangeTypeAsync(context.Document, type, typeSymbol, cancellationToken),
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.LocalVariableOrFunctionIsAlreadyDefinedInThisScope:
                    case CompilerDiagnosticIdentifiers.LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ReplaceVariableDeclarationWithAssignment))
                                return;

                            if (!(variableDeclaration.Parent is LocalDeclarationStatementSyntax localDeclaration))
                                return;

                            VariableDeclaratorSyntax variableDeclarator = variableDeclaration.Variables.SingleOrDefault(shouldThrow: false);

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

                                        return context.Document.ReplaceNodeAsync(localDeclaration, newNode, cancellationToken);
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
