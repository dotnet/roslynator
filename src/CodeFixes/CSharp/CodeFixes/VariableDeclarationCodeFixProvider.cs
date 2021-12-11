// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(VariableDeclarationCodeFixProvider))]
    [Shared]
    public sealed class VariableDeclarationCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0819_ImplicitlyTypedVariablesCannotHaveMultipleDeclarators,
                    CompilerDiagnosticIdentifiers.CS0822_ImplicitlyTypedVariablesCannotBeConstant,
                    CompilerDiagnosticIdentifiers.CS0128_LocalVariableOrFunctionIsAlreadyDefinedInThisScope,
                    CompilerDiagnosticIdentifiers.CS0136_LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f => f.IsKind(
                    SyntaxKind.VariableDeclaration,
                    SyntaxKind.ForEachStatement,
                    SyntaxKind.Parameter,
                    SyntaxKind.DeclarationPattern,
                    SyntaxKind.DeclarationExpression,
                    SyntaxKind.LocalFunctionStatement)))
            {
                return;
            }

            if (node.IsKind(SyntaxKind.ForEachStatement, SyntaxKind.Parameter, SyntaxKind.DeclarationPattern, SyntaxKind.DeclarationExpression, SyntaxKind.LocalFunctionStatement))
                return;

            var variableDeclaration = (VariableDeclarationSyntax)node;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS0819_ImplicitlyTypedVariablesCannotHaveMultipleDeclarators:
                    case CompilerDiagnosticIdentifiers.CS0822_ImplicitlyTypedVariablesCannotBeConstant:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.UseExplicitTypeInsteadOfVar, context.Document, root.SyntaxTree))
                                return;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            TypeSyntax type = variableDeclaration.Type;

                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                            if (typeSymbol?.SupportsExplicitDeclaration() == true)
                            {
                                CodeAction codeAction = CodeActionFactory.ChangeType(context.Document, type, typeSymbol, semanticModel, equivalenceKey: GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0128_LocalVariableOrFunctionIsAlreadyDefinedInThisScope:
                    case CompilerDiagnosticIdentifiers.CS0136_LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceVariableDeclarationWithAssignment, context.Document, root.SyntaxTree))
                                return;

                            if (variableDeclaration.Parent is not LocalDeclarationStatementSyntax localDeclaration)
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
                                    ct =>
                                    {
                                        ExpressionStatementSyntax newNode = CSharpFactory.SimpleAssignmentStatement(
                                            SyntaxFactory.IdentifierName(variableDeclarator.Identifier),
                                            value);

                                        newNode = newNode
                                            .WithTriviaFrom(localDeclaration)
                                            .WithFormatterAnnotation();

                                        return context.Document.ReplaceNodeAsync(localDeclaration, newNode, ct);
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
                if (descendant is VariableDeclaratorSyntax variableDeclarator
                    && string.Equals(name, variableDeclarator.Identifier.ValueText, StringComparison.Ordinal))
                {
                    return variableDeclarator;
                }
            }

            return null;
        }
    }
}
