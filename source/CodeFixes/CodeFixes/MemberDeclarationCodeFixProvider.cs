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
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
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
                    CompilerDiagnosticIdentifiers.CannotChangeAccessModifiersWhenOverridingInheritedMember,
                    CompilerDiagnosticIdentifiers.MissingXmlCommentForPubliclyVisibleTypeOrMember,
                    CompilerDiagnosticIdentifiers.MemberReturnTypeMustMatchOverriddenMemberReturnType,
                    CompilerDiagnosticIdentifiers.MemberTypeMustMatchOverriddenMemberType,
                    CompilerDiagnosticIdentifiers.MissingPartialModifier,
                    CompilerDiagnosticIdentifiers.PartialMethodMayNotHaveMultipleDefiningDeclarations,
                    CompilerDiagnosticIdentifiers.PartialMethodMustBeDeclaredWithinPartialClassOrPartialStruct,
                    CompilerDiagnosticIdentifiers.CannotDeclareInstanceMembersInStaticClass,
                    CompilerDiagnosticIdentifiers.StaticClassesCannotHaveInstanceConstructors,
                    CompilerDiagnosticIdentifiers.MemberIsAbstractButItIsContainedInNonAbstractClass,
                    CompilerDiagnosticIdentifiers.ObjectReferenceIsRequiredForNonStaticMember,
                    CompilerDiagnosticIdentifiers.StaticConstructorMustBeParameterless);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.OverridingMemberCannotChangeAccessModifiers)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddDocumentationComment)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.MemberReturnTypeMustMatchOverriddenMemberReturnType)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.MemberTypeMustMatchOverriddenMemberType)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddPartialModifier)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddMethodBody)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddStaticModifier)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.MakeContainingClassAbstract)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.MakeMemberNonStatic)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveParametersFromStaticConstructor))
            {
                return;
            }

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
                    case CompilerDiagnosticIdentifiers.CannotChangeAccessModifiersWhenOverridingInheritedMember:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.OverridingMemberCannotChangeAccessModifiers))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            OverrideInfo overrideInfo = OverridingMemberCannotChangeAccessModifiersRefactoring.GetOverrideInfo(memberDeclaration, semanticModel, context.CancellationToken);

                            string title = $"Change accessibility to '{overrideInfo.DeclaredAccessibilityText}'";

                            CodeAction codeAction = CodeAction.Create(
                                title,
                                cancellationToken => OverridingMemberCannotChangeAccessModifiersRefactoring.RefactorAsync(context.Document, memberDeclaration, overrideInfo, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MissingXmlCommentForPubliclyVisibleTypeOrMember:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddDocumentationComment))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                           "Add documentation comment",
                           cancellationToken => AddDocumentationCommentRefactoring.RefactorAsync(context.Document, memberDeclaration, false, cancellationToken),
                           GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);

                            CodeAction codeAction2 = CodeAction.Create(
                                "Add documentation comment (copy from base if available)",
                                cancellationToken => AddDocumentationCommentRefactoring.RefactorAsync(context.Document, memberDeclaration, true, cancellationToken),
                                GetEquivalenceKey(diagnostic, "CopyFromBaseIfAvailable"));

                            context.RegisterCodeFix(codeAction2, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MemberReturnTypeMustMatchOverriddenMemberReturnType:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.MemberReturnTypeMustMatchOverriddenMemberReturnType))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            var methodSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(memberDeclaration, context.CancellationToken);

                            ITypeSymbol typeSymbol = methodSymbol.OverriddenMethod.ReturnType;

                            if (typeSymbol?.IsErrorType() == false)
                            {
                                TypeSyntax newType = typeSymbol.ToMinimalTypeSyntax(semanticModel, memberDeclaration.SpanStart);

                                CodeAction codeAction = CodeAction.Create(
                                    $"Change return type to '{SymbolDisplay.GetMinimalString(typeSymbol, semanticModel, memberDeclaration.SpanStart)}'",
                                    cancellationToken => MemberTypeMustMatchOverriddenMemberTypeRefactoring.RefactorAsync(context.Document, memberDeclaration, typeSymbol, semanticModel, cancellationToken),
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MemberTypeMustMatchOverriddenMemberType:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.MemberTypeMustMatchOverriddenMemberType))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ITypeSymbol typeSymbol = null;

                            switch (memberDeclaration.Kind())
                            {
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

                            if (typeSymbol?.IsErrorType() == false)
                            {
                                string title = $"Change type to '{SymbolDisplay.GetMinimalString(typeSymbol, semanticModel, memberDeclaration.SpanStart)}'";

                                CodeAction codeAction = CodeAction.Create(
                                    title,
                                    cancellationToken => MemberTypeMustMatchOverriddenMemberTypeRefactoring.RefactorAsync(context.Document, memberDeclaration, typeSymbol, semanticModel, cancellationToken),
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MissingPartialModifier:
                    case CompilerDiagnosticIdentifiers.PartialMethodMustBeDeclaredWithinPartialClassOrPartialStruct:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddPartialModifier))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Add 'partial' modifier",
                                cancellationToken =>
                                {
                                    if (memberDeclaration.IsKind(SyntaxKind.MethodDeclaration)
                                        && memberDeclaration.IsParentKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
                                    {
                                        var parentMember = (MemberDeclarationSyntax)memberDeclaration.Parent;

                                        SyntaxTokenList newModifiers = parentMember.GetModifiers().InsertModifier(SyntaxKind.PartialKeyword, ModifierComparer.Instance);

                                        MemberDeclarationSyntax newNode = parentMember.WithModifiers(newModifiers);

                                        return context.Document.ReplaceNodeAsync(parentMember, newNode, context.CancellationToken);
                                    }
                                    else if (memberDeclaration.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration))
                                    {
                                        SyntaxTokenList newModifiers = memberDeclaration.GetModifiers().InsertModifier(SyntaxKind.PartialKeyword, ModifierComparer.Instance);

                                        MemberDeclarationSyntax newNode = memberDeclaration.WithModifiers(newModifiers);

                                        return context.Document.ReplaceNodeAsync(memberDeclaration, newNode, context.CancellationToken);
                                    }

                                    return Task.FromResult(context.Document);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.PartialMethodMayNotHaveMultipleDefiningDeclarations:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddMethodBody))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Add body",
                                cancellationToken =>
                                {
                                    var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;

                                    ParameterListSyntax parameterList = methodDeclaration.ParameterList ?? SyntaxFactory.ParameterList();

                                    MethodDeclarationSyntax newNode = methodDeclaration
                                        .WithParameterList(parameterList.AppendToTrailingTrivia(methodDeclaration.SemicolonToken.GetLeadingAndTrailingTrivia()))
                                        .WithSemicolonToken(default(SyntaxToken))
                                        .WithBody(SyntaxFactory.Block())
                                        .WithFormatterAnnotation();

                                    return context.Document.ReplaceNodeAsync(memberDeclaration, newNode, context.CancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CannotDeclareInstanceMembersInStaticClass:
                    case CompilerDiagnosticIdentifiers.StaticClassesCannotHaveInstanceConstructors:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddStaticModifier))
                                break;

                            if (memberDeclaration.IsKind(SyntaxKind.ConstructorDeclaration)
                                && ((ConstructorDeclarationSyntax)memberDeclaration).ParameterList?.Parameters.Any() == true)
                            {
                                break;
                            }

                            CodeAction codeAction = CodeAction.Create(
                                "Add 'static' modifier",
                                cancellationToken =>
                                {
                                    SyntaxTokenList modifiers = memberDeclaration.GetModifiers();

                                    if (memberDeclaration.IsKind(SyntaxKind.ConstructorDeclaration))
                                        modifiers = modifiers.RemoveAccessModifiers();

                                    SyntaxTokenList newModifiers = modifiers.InsertModifier(SyntaxKind.StaticKeyword, ModifierComparer.Instance);

                                    MemberDeclarationSyntax newNode = memberDeclaration.WithModifiers(newModifiers);

                                    return context.Document.ReplaceNodeAsync(memberDeclaration, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MemberIsAbstractButItIsContainedInNonAbstractClass:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.MakeContainingClassAbstract))
                                break;

                            if (!memberDeclaration.IsParentKind(SyntaxKind.ClassDeclaration))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Make containing class abstract",
                                cancellationToken =>
                                {
                                    var classDeclaration = (ClassDeclarationSyntax)memberDeclaration.Parent;

                                    SyntaxTokenList newModifiers = classDeclaration.GetModifiers().InsertModifier(SyntaxKind.AbstractKeyword, ModifierComparer.Instance);

                                    MemberDeclarationSyntax newNode = classDeclaration.WithModifiers(newModifiers);

                                    return context.Document.ReplaceNodeAsync(classDeclaration, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.ObjectReferenceIsRequiredForNonStaticMember:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.MakeMemberNonStatic))
                                break;

                            SyntaxTokenList modifiers = memberDeclaration.GetModifiers();

                            Debug.Assert(modifiers.Contains(SyntaxKind.StaticKeyword), memberDeclaration.ToString());

                            if (!modifiers.Contains(SyntaxKind.StaticKeyword))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                $"Make containing {memberDeclaration.GetTitle()} non-static",
                                cancellationToken =>
                                {
                                    MemberDeclarationSyntax newNode = memberDeclaration.RemoveModifier(SyntaxKind.StaticKeyword);

                                    return context.Document.ReplaceNodeAsync(memberDeclaration, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.StaticConstructorMustBeParameterless:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveParametersFromStaticConstructor))
                                break;

                            var constructorDeclaration = (ConstructorDeclarationSyntax)memberDeclaration;

                            CodeAction codeAction = CodeAction.Create(
                                "Remove parameters",
                                cancellationToken =>
                                {
                                    ParameterListSyntax parameterList = constructorDeclaration.ParameterList;

                                    ParameterListSyntax newParameterList = parameterList
                                        .WithParameters(default(SeparatedSyntaxList<ParameterSyntax>))
                                        .WithOpenParenToken(parameterList.OpenParenToken.WithoutTrailingTrivia())
                                        .WithCloseParenToken(parameterList.CloseParenToken.WithoutLeadingTrivia());

                                    ConstructorDeclarationSyntax newNode = constructorDeclaration.WithParameterList(newParameterList);

                                    return context.Document.ReplaceNodeAsync(constructorDeclaration, newNode, cancellationToken);
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
