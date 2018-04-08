// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
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
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ISymbol symbol = semanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);

                            ImmutableArray<SyntaxReference> syntaxReferences = symbol.DeclaringSyntaxReferences;

                            if (!syntaxReferences.Any())
                                break;

                            ModifiersCodeFixRegistrator.AddModifier(
                                context,
                                diagnostic,
                                ImmutableArray.CreateRange(syntaxReferences, f => f.GetSyntax()),
                                SyntaxKind.StaticKeyword,
                                title: "Make class static");

                            break;
                        }
                    case DiagnosticIdentifiers.AddStaticModifierToAllPartialClassDeclarations:
                        {
                            ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, classDeclaration, SyntaxKind.StaticKeyword);
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
