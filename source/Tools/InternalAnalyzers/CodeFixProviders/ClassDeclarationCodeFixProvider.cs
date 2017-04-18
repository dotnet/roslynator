// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;

namespace Roslynator.CSharp.Internal.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassDeclarationCodeFixProvider))]
    [Shared]
    public class ClassDeclarationCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                  DiagnosticIdentifiers.AddDiagnosticAnalyzerSuffix,
                  DiagnosticIdentifiers.AddCodeFixProviderSuffix,
                  DiagnosticIdentifiers.AddCodeRefactoringProviderSuffix);
            }
        }

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            ClassDeclarationSyntax classDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ClassDeclarationSyntax>();

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.AddDiagnosticAnalyzerSuffix:
                    case DiagnosticIdentifiers.AddCodeFixProviderSuffix:
                    case DiagnosticIdentifiers.AddCodeRefactoringProviderSuffix:
                        {
                            await RegisterCodeFixAsync(context, classDeclaration, diagnostic).ConfigureAwait(false);
                            break;
                        }
                }
            }
        }

        private static async Task RegisterCodeFixAsync(
            CodeFixContext context,
            ClassDeclarationSyntax classDeclaration,
            Diagnostic diagnostic)
        {
            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            ISymbol symbol = semanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);

            string oldName = classDeclaration.Identifier.ValueText;
            string newName = oldName + GetSuffix(diagnostic);

            CodeAction codeAction = CodeAction.Create(
                $"Rename '{oldName}' to '{newName}'",
                cancellationToken => Renamer.RenameSymbolAsync(context.Document.Project.Solution, symbol, newName, default(OptionSet), cancellationToken),
                DiagnosticIdentifiers.AddCodeFixProviderSuffix);

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static string GetSuffix(Diagnostic diagnostic)
        {
            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.AddDiagnosticAnalyzerSuffix:
                    return nameof(DiagnosticAnalyzer);
                case DiagnosticIdentifiers.AddCodeFixProviderSuffix:
                    return nameof(CodeFixProvider);
                case DiagnosticIdentifiers.AddCodeRefactoringProviderSuffix:
                    return nameof(CodeRefactoringProvider);
                default:
                    return null;
            }
        }
    }
}
