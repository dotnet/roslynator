// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassDeclarationCodeFixProvider))]
    [Shared]
    public class ClassDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.MakeClassStatic,
                    DiagnosticIdentifiers.AddStaticModifierToAllPartialClassDeclarations,
                    DiagnosticIdentifiers.ImplementExceptionConstructors,
                    DiagnosticIdentifiers.UseAttributeUsageAttribute);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ClassDeclarationSyntax classDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.MakeClassStatic:
                        {
                            CodeAction codeAction = null;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ISymbol symbol = semanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);

                            ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

                            if (syntaxReferences.Length == 1)
                            {
                                codeAction = CodeAction.Create(
                                    $"Make '{classDeclaration.Identifier.ValueText}' static",
                                    cancellationToken =>
                                    {
                                        return MakeClassStaticRefactoring.RefactorAsync(
                                            context.Document,
                                            classDeclaration,
                                            cancellationToken);
                                    },
                                    GetEquivalenceKey(diagnostic));
                            }
                            else
                            {
                                ImmutableArray<ClassDeclarationSyntax> classDeclarations = syntaxReferences
                                    .Select(f => (ClassDeclarationSyntax)f.GetSyntax(context.CancellationToken))
                                    .ToImmutableArray();

                                codeAction = CodeAction.Create(
                                    $"Make '{classDeclaration.Identifier.ValueText}' static",
                                    cancellationToken =>
                                    {
                                        return MakeClassStaticRefactoring.RefactorAsync(
                                            context.Solution(),
                                            classDeclarations,
                                            cancellationToken);
                                    },
                                    GetEquivalenceKey(diagnostic));
                            }

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AddStaticModifierToAllPartialClassDeclarations:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Add 'static' modifier",
                                cancellationToken =>
                                {
                                    return AddStaticModifierToAllPartialClassDeclarationsRefactoring.RefactorAsync(
                                        context.Document,
                                        classDeclaration,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.ImplementExceptionConstructors:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Generate exception constructors",
                                cancellationToken =>
                                {
                                    return ImplementExceptionConstructorsRefactoring.RefactorAsync(
                                        context.Document,
                                        classDeclaration,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseAttributeUsageAttribute:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use AttributeUsageAttribute",
                                cancellationToken =>
                                {
                                    return UseAttributeUsageAttributeRefactoring.RefactorAsync(
                                        context.Document,
                                        classDeclaration,
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
