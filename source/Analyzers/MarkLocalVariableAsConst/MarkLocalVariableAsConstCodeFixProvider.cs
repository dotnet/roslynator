// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.CodeFixes;

namespace Roslynator.CSharp.Analyzers.MarkLocalVariableAsConst
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MarkLocalVariableAsConstCodeFixProvider))]
    [Shared]
    public class MarkLocalVariableAsConstCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.MarkLocalVariableAsConst); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out LocalDeclarationStatementSyntax localDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.MarkLocalVariableAsConst:
                        {
                            string names = GetNames(localDeclaration);

                            CodeAction codeAction = CodeAction.Create(
                                $"Mark {names} as const",
                                cancellationToken => MarkLocalVariableAsConstRefactoring.RefactorAsync(context.Document, localDeclaration, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static string GetNames(LocalDeclarationStatementSyntax localDeclaration)
        {
            VariableDeclarationSyntax declaration = localDeclaration.Declaration;

            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

            if (variables.Count == 1)
            {
                return $"'{variables.First().Identifier.ValueText}'";
            }
            else
            {
                return string.Join(", ", variables.Select(f => $"'{f.Identifier.ValueText}'"));
            }
        }
    }
}
