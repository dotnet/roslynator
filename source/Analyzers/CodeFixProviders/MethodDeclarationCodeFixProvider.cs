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

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
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
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            MethodDeclarationSyntax methodDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MethodDeclarationSyntax>();

            if (methodDeclaration == null)
                return;

            if (!context.Document.SupportsSemanticModel)
                return;

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            if (methodSymbol == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync:
                    case DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync:
                        {
                            string newName = GetNewName(methodDeclaration, diagnostic);

                            CodeAction codeAction = CodeAction.Create(
                                $"Rename method to '{newName}'",
                                cancellationToken => SymbolRenamer.RenameAsync(context.Document, methodSymbol, newName, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);

                            break;
                        }
                }
            }
        }

        private static string GetNewName(MethodDeclarationSyntax methodDeclaration, Diagnostic diagnostic)
        {
            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync:
                    {
                        return methodDeclaration.Identifier + AsyncSuffix;
                    }
                case DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync:
                    {
                        string name = methodDeclaration.Identifier.ValueText;
                        return name.Remove(name.Length - AsyncSuffix.Length);
                    }
                default:
                    {
                        Debug.Assert(false, diagnostic.Id);
                        return null;
                    }
            }
        }
    }
}
