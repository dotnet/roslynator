// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Rename;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
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
                  DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync,
                  DiagnosticIdentifiers.AddReturnStatementThatReturnsDefaultValue);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Document document = context.Document;
            CancellationToken cancellationToken = context.CancellationToken;

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            MethodDeclarationSyntax methodDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MethodDeclarationSyntax>();

            if (methodDeclaration == null)
                return;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

            Debug.Assert(methodSymbol != null, $"{nameof(methodSymbol)} is null");

            if (methodSymbol != null)
            {
                foreach (Diagnostic diagnostic in context.Diagnostics)
                {
                    switch (diagnostic.Id)
                    {
                        case DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync:
                            {
                                string newName = methodDeclaration.Identifier.ValueText;

                                newName = await Identifier.EnsureUniqueAsyncMethodNameAsync(
                                    methodSymbol,
                                    newName,
                                    document.Project.Solution,
                                    cancellationToken).ConfigureAwait(false);

                                CodeAction codeAction = CodeAction.Create(
                                    $"Rename method to '{newName}'",
                                    c => Renamer.RenameSymbolAsync(document, methodSymbol, newName, c),
                                    diagnostic.Id + EquivalenceKeySuffix);

                                context.RegisterCodeFix(codeAction, diagnostic);
                                break;
                            }
                        case DiagnosticIdentifiers.AddReturnStatementThatReturnsDefaultValue:
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Add return statement that returns default value",
                                    c => AddReturnStatementThatReturnsDefaultValueRefactoring.RefactorAsync(document, methodDeclaration, c),
                                    diagnostic.Id + EquivalenceKeySuffix);

                                context.RegisterCodeFix(codeAction, diagnostic);
                                break;
                            }
                        case DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync:
                            {
                                string name = methodDeclaration.Identifier.ValueText;
                                string newName = name.Remove(name.Length - AsyncSuffix.Length);

                                newName = await Identifier.EnsureUniqueMemberNameAsync(
                                    methodSymbol,
                                    newName,
                                    document.Project.Solution,
                                    cancellationToken).ConfigureAwait(false);

                                CodeAction codeAction = CodeAction.Create(
                                    $"Rename method to '{newName}'",
                                    c => Renamer.RenameSymbolAsync(document, methodSymbol, newName, c),
                                    diagnostic.Id + EquivalenceKeySuffix);

                                context.RegisterCodeFix(codeAction, diagnostic);
                                break;
                            }
                    }
                }
            }
        }
    }
}
