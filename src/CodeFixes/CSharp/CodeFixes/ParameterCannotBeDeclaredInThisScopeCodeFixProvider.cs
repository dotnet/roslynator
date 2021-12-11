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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParameterCannotBeDeclaredInThisScopeCodeFixProvider))]
    [Shared]
    public sealed class ParameterCannotBeDeclaredInThisScopeCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS0136_LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveParameter, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f => f.IsKind(
                    SyntaxKind.Parameter,
                    SyntaxKind.VariableDeclaration,
                    SyntaxKind.ForEachStatement,
                    SyntaxKind.DeclarationPattern,
                    SyntaxKind.DeclarationExpression,
                    SyntaxKind.LocalFunctionStatement)))
            {
                return;
            }

            if (node.IsKind(SyntaxKind.VariableDeclaration, SyntaxKind.ForEachStatement, SyntaxKind.DeclarationPattern, SyntaxKind.DeclarationExpression, SyntaxKind.LocalFunctionStatement))
                return;

            var parameter = (ParameterSyntax)node;

            if (parameter.IsParentKind(SyntaxKind.ParameterList)
                && parameter.Parent.IsParentKind(SyntaxKind.LocalFunctionStatement))
            {
                CodeAction codeAction = CodeAction.Create(
                    $"Remove parameter '{parameter.Identifier.ValueText}'",
                    ct => context.Document.RemoveNodeAsync(parameter, ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }
    }
}
