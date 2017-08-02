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
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MethodDeclarationCodeFixProvider))]
    [Shared]
    public class MethodDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        private const string AsyncSuffix = "Async";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                  DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync,
                  DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out MethodDeclarationSyntax methodDeclaration))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            Debug.Assert(methodSymbol != null, $"{nameof(methodSymbol)} is null");

            if (methodSymbol != null)
            {
                foreach (Diagnostic diagnostic in context.Diagnostics)
                {
                    switch (diagnostic.Id)
                    {
                        case DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync:
                            {
                                string oldName = methodDeclaration.Identifier.ValueText;

                                string newName = await NameGenerators.AsyncMethod.EnsureUniqueMemberNameAsync(
                                    oldName,
                                    methodSymbol,
                                    context.Solution(),
                                    cancellationToken: context.CancellationToken).ConfigureAwait(false);

                                CodeAction codeAction = CodeAction.Create(
                                    $"Rename '{oldName}' to '{newName}'",
                                    c => Renamer.RenameSymbolAsync(context.Solution(), methodSymbol, newName, default(OptionSet), c),
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                                break;
                            }
                        case DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync:
                            {
                                string name = methodDeclaration.Identifier.ValueText;
                                string newName = name.Remove(name.Length - AsyncSuffix.Length);

                                newName = await NameGenerator.Default.EnsureUniqueMemberNameAsync(
                                    newName,
                                    methodSymbol,
                                    context.Solution(),
                                    cancellationToken: context.CancellationToken).ConfigureAwait(false);

                                CodeAction codeAction = CodeAction.Create(
                                    $"Rename '{name}' to '{newName}'",
                                    c => Renamer.RenameSymbolAsync(context.Solution(), methodSymbol, newName, default(OptionSet), c),
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                                break;
                            }
                    }
                }
            }
        }
    }
}
