// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Refactorings.MakeMemberReadOnly;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberDeclarationCodeFixProvider))]
    [Shared]
    public class MemberDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.FormatDeclarationBraces,
                    DiagnosticIdentifiers.MarkMemberAsStatic,
                    DiagnosticIdentifiers.RemoveRedundantOverridingMember,
                    DiagnosticIdentifiers.AddDocumentationComment,
                    DiagnosticIdentifiers.MarkContainingClassAsAbstract,
                    DiagnosticIdentifiers.MemberTypeMustMatchOverriddenMemberType,
                    DiagnosticIdentifiers.AddDefaultAccessModifier,
                    DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations,
                    DiagnosticIdentifiers.RemoveRedundantSealedModifier,
                    DiagnosticIdentifiers.AvoidSemicolonAtEndOfDeclaration,
                    DiagnosticIdentifiers.ReorderModifiers,
                    DiagnosticIdentifiers.OverridingMemberCannotChangeAccessModifiers,
                    DiagnosticIdentifiers.MarkFieldAsReadOnly,
                    DiagnosticIdentifiers.UseReadOnlyAutoProperty);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            MemberDeclarationSyntax memberDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            Debug.Assert(memberDeclaration != null, $"{nameof(memberDeclaration)} is null");

            if (memberDeclaration == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.FormatDeclarationBraces:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Format braces",
                                cancellationToken => FormatDeclarationBracesRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.MarkMemberAsStatic:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Mark {memberDeclaration.GetTitle()} as static",
                                cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AddDocumentationComment:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Add documentation comment",
                                cancellationToken => AddDocumentationCommentRefactoring.RefactorAsync(context.Document, memberDeclaration, false, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);

                            CodeAction codeAction2 = CodeAction.Create(
                                "Add documentation comment (copy from base if available)",
                                cancellationToken => AddDocumentationCommentRefactoring.RefactorAsync(context.Document, memberDeclaration, true, cancellationToken),
                                diagnostic.Id + "CopyFromBaseIfAvailable" + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction2, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantOverridingMember:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Remove redundant overridding {memberDeclaration.GetTitle()}",
                                cancellationToken => context.Document.RemoveMemberAsync(memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.MarkContainingClassAsAbstract:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Mark containing class as abstract",
                                cancellationToken => MarkContainingClassAsAbstractRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.MemberTypeMustMatchOverriddenMemberType:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ITypeSymbol typeSymbol = null;

                            switch (memberDeclaration.Kind())
                            {
                                case SyntaxKind.MethodDeclaration:
                                    {
                                        var methodSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(memberDeclaration, context.CancellationToken);
                                        typeSymbol = methodSymbol.OverriddenMethod.ReturnType;
                                        break;
                                    }
                                case SyntaxKind.PropertyDeclaration:
                                case SyntaxKind.IndexerDeclaration:
                                    {
                                        var propertySymbol = (IPropertySymbol)semanticModel.GetDeclaredSymbol(memberDeclaration, context.CancellationToken);
                                        typeSymbol = propertySymbol.OverriddenProperty.Type;
                                        break;
                                    }
                                case SyntaxKind.EventDeclaration:
                                    {
                                        var eventSymbol = (IEventSymbol)semanticModel.GetDeclaredSymbol(memberDeclaration, context.CancellationToken);
                                        typeSymbol = eventSymbol.OverriddenEvent.Type;
                                        break;
                                    }
                                case SyntaxKind.EventFieldDeclaration:
                                    {
                                        VariableDeclaratorSyntax declarator = ((EventFieldDeclarationSyntax)memberDeclaration).Declaration.Variables.First();

                                        var eventSymbol = (IEventSymbol)semanticModel.GetDeclaredSymbol(declarator, context.CancellationToken);
                                        typeSymbol = eventSymbol.OverriddenEvent.Type;
                                        break;
                                    }
                            }

                            int position = memberDeclaration.SpanStart;

                            TypeSyntax newType = typeSymbol.ToMinimalTypeSyntax(semanticModel, position);

                            string title = (memberDeclaration.IsKind(SyntaxKind.MethodDeclaration))
                                ? "Change return type to "
                                : "Change type to ";

                            title += $"'{SymbolDisplay.GetMinimalString(typeSymbol, semanticModel, position)}'";

                            CodeAction codeAction = CodeAction.Create(
                                title,
                                cancellationToken => MemberTypeMustMatchOverriddenMemberTypeRefactoring.RefactorAsync(context.Document, memberDeclaration, newType, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AddDefaultAccessModifier:
                        {
                            var accessibility = (Accessibility)Enum.Parse(
                                typeof(Accessibility),
                                context.Diagnostics[0].Properties[nameof(Accessibility)]);

                            CodeAction codeAction = CodeAction.Create(
                                "Add default access modifier",
                                cancellationToken => AddDefaultAccessModifierRefactoring.RefactorAsync(context.Document, memberDeclaration, accessibility, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Add empty line",
                                cancellationToken => AddEmptyLineBetweenDeclarationsRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantSealedModifier:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove sealed modifier",
                                cancellationToken => RemoveRedundantSealedModifierRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AvoidSemicolonAtEndOfDeclaration:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove unnecessary semicolon",
                                cancellationToken => AvoidSemicolonAtEndOfDeclarationRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.ReorderModifiers:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Reorder modifiers",
                                cancellationToken => ReorderModifiersRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.OverridingMemberCannotChangeAccessModifiers:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            OverrideInfo overrideInfo = OverridingMemberCannotChangeAccessModifiersRefactoring.GetOverrideInfo(memberDeclaration, semanticModel, context.CancellationToken);

                            string title = $"Change accessibility to '{OverridingMemberCannotChangeAccessModifiersRefactoring.GetAccessibilityText(overrideInfo.OverridenSymbol.DeclaredAccessibility)}'";

                            CodeAction codeAction = CodeAction.Create(
                                title,
                                cancellationToken => OverridingMemberCannotChangeAccessModifiersRefactoring.RefactorAsync(context.Document, memberDeclaration, overrideInfo, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.MarkFieldAsReadOnly:
                        {
                            var fieldDeclaration = (FieldDeclarationSyntax)memberDeclaration;

                            SeparatedSyntaxList<VariableDeclaratorSyntax> declarators = fieldDeclaration.Declaration.Variables;

                            string title = (declarators.Count == 1)
                                ? $"Mark '{declarators[0].Identifier.ValueText}' as read-only"
                                : "Mark fields as read-only";

                            CodeAction codeAction = CodeAction.Create(
                                title,
                                cancellationToken => MarkFieldAsReadOnlyRefactoring.RefactorAsync(context.Document, fieldDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseReadOnlyAutoProperty:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use read-only auto-property",
                                cancellationToken => UseReadOnlyAutoPropertyRefactoring.RefactorAsync(context.Document, (PropertyDeclarationSyntax)memberDeclaration, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
