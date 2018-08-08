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
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.SyntaxRewriters;

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
                    CompilerDiagnosticIdentifiers.MissingXmlCommentForPubliclyVisibleTypeOrMember,
                    CompilerDiagnosticIdentifiers.MethodReturnTypeMustMatchOverriddenMethodReturnType,
                    CompilerDiagnosticIdentifiers.MemberTypeMustMatchOverriddenMemberType,
                    CompilerDiagnosticIdentifiers.MissingPartialModifier,
                    CompilerDiagnosticIdentifiers.PartialMethodMustBeDeclaredInPartialClassOrPartialStruct,
                    CompilerDiagnosticIdentifiers.MemberIsAbstractButItIsContainedInNonAbstractClass,
                    CompilerDiagnosticIdentifiers.StaticConstructorMustBeParameterless,
                    CompilerDiagnosticIdentifiers.PartialMethodsMustHaveVoidReturnType,
                    CompilerDiagnosticIdentifiers.ExplicitInterfaceDeclarationCanOnlyBeDeclaredInClassOrStruct,
                    CompilerDiagnosticIdentifiers.InterfacesCannotContainFields,
                    CompilerDiagnosticIdentifiers.InterfacesCannotContainOperators,
                    CompilerDiagnosticIdentifiers.InterfacesCannotDeclareTypes,
                    CompilerDiagnosticIdentifiers.OnlyClassTypesCanContainDestructors,
                    CompilerDiagnosticIdentifiers.StructsCannotContainExplicitParameterlessConstructors,
                    CompilerDiagnosticIdentifiers.NameOfDestructorMustMatchNameOfClass,
                    CompilerDiagnosticIdentifiers.CannotChangeTupleElementNameWhenOverridingInheritedMember);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddDocumentationComment)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeMethodReturnType)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.MemberTypeMustMatchOverriddenMemberType)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddPartialModifier)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.MakeContainingClassAbstract)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveParametersFromStaticConstructor)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveMemberDeclaration)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RenameDestructorToMatchClassName)
                && !Settings.IsCodeFixEnabled(CodeFixIdentifiers.RenameTupleElement))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberDeclarationSyntax memberDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
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
                    case CompilerDiagnosticIdentifiers.MethodReturnTypeMustMatchOverriddenMethodReturnType:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeMethodReturnType))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            var methodSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(memberDeclaration, context.CancellationToken);

                            ITypeSymbol typeSymbol = methodSymbol.OverriddenMethod.ReturnType;

                            CodeFixRegistrator.ChangeMemberDeclarationType(context, diagnostic, memberDeclaration, typeSymbol, semanticModel);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.PartialMethodsMustHaveVoidReturnType:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeMethodReturnType))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;

                            MethodDeclarationSyntax otherPart = semanticModel.GetOtherPart(methodDeclaration, context.CancellationToken);

                            if (otherPart == null)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Change return type to 'void'",
                                cancellationToken =>
                                {
                                    return context.Document.Solution().ReplaceNodesAsync(
                                        new MethodDeclarationSyntax[] { methodDeclaration, otherPart },
                                        (node, _) => node.WithReturnType(CSharpFactory.VoidType().WithTriviaFrom(node.ReturnType)),
                                        cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
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

                            CodeFixRegistrator.ChangeMemberDeclarationType(context, diagnostic, memberDeclaration, typeSymbol, semanticModel);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MissingPartialModifier:
                    case CompilerDiagnosticIdentifiers.PartialMethodMustBeDeclaredInPartialClassOrPartialStruct:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddPartialModifier))
                                break;

                            SyntaxNode node = null;

                            switch (memberDeclaration.Kind())
                            {
                                case SyntaxKind.MethodDeclaration:
                                    {
                                        if (memberDeclaration.IsParentKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
                                            node = memberDeclaration.Parent;

                                        break;
                                    }
                                case SyntaxKind.ClassDeclaration:
                                case SyntaxKind.StructDeclaration:
                                case SyntaxKind.InterfaceDeclaration:
                                    {
                                        node = memberDeclaration;
                                        break;
                                    }
                            }

                            Debug.Assert(node != null, memberDeclaration.ToString());

                            if (node == null)
                                break;

                            ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, node, SyntaxKind.PartialKeyword);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.MemberIsAbstractButItIsContainedInNonAbstractClass:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.MakeContainingClassAbstract))
                                break;

                            if (!memberDeclaration.IsParentKind(SyntaxKind.ClassDeclaration))
                                break;

                            ModifiersCodeFixRegistrator.AddModifier(
                                context,
                                diagnostic,
                                memberDeclaration.Parent,
                                SyntaxKind.AbstractKeyword,
                                title: "Make containing class abstract");

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
                    case CompilerDiagnosticIdentifiers.ExplicitInterfaceDeclarationCanOnlyBeDeclaredInClassOrStruct:
                    case CompilerDiagnosticIdentifiers.InterfacesCannotContainFields:
                    case CompilerDiagnosticIdentifiers.InterfacesCannotContainOperators:
                    case CompilerDiagnosticIdentifiers.InterfacesCannotDeclareTypes:
                    case CompilerDiagnosticIdentifiers.OnlyClassTypesCanContainDestructors:
                    case CompilerDiagnosticIdentifiers.StructsCannotContainExplicitParameterlessConstructors:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RemoveMemberDeclaration))
                                break;

                            CodeFixRegistrator.RemoveMember(context, diagnostic, memberDeclaration);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.NameOfDestructorMustMatchNameOfClass:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RenameDestructorToMatchClassName))
                                break;

                            if (!(memberDeclaration is DestructorDeclarationSyntax destructorDeclaration))
                                break;

                            if (!(memberDeclaration.Parent is ClassDeclarationSyntax classDeclaration))
                                break;

                            if (classDeclaration.Identifier.ValueText.Length == 0)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Rename destructor to match class name",
                                cancellationToken =>
                                {
                                    DestructorDeclarationSyntax newNode = destructorDeclaration.WithIdentifier(classDeclaration.Identifier.WithTriviaFrom(destructorDeclaration.Identifier));

                                    return context.Document.ReplaceNodeAsync(destructorDeclaration, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CannotChangeTupleElementNameWhenOverridingInheritedMember:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.RenameTupleElement))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            if (memberDeclaration is MethodDeclarationSyntax methodDeclaration)
                            {
                                IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

                                if (!(methodSymbol.ReturnType is INamedTypeSymbol tupleType))
                                    break;

                                if (!tupleType.IsTupleType)
                                    break;

                                if (!(methodSymbol.OverriddenMethod?.ReturnType is INamedTypeSymbol baseTupleType))
                                    break;

                                if (!baseTupleType.IsTupleType)
                                    break;

                                ImmutableArray<IFieldSymbol> elements = tupleType.TupleElements;
                                ImmutableArray<IFieldSymbol> baseElements = baseTupleType.TupleElements;

                                if (elements.Length != baseElements.Length)
                                    break;

                                int i = 0;
                                while (i < elements.Length)
                                {
                                    if (elements[i].Name != baseElements[i].Name)
                                        break;

                                    i++;
                                }

                                if (i == elements.Length)
                                    break;

                                TupleElementSyntax tupleElement = ((TupleTypeSyntax)methodDeclaration.ReturnType).Elements[i];

                                CodeAction codeAction = CodeAction.Create(
                                    $"Rename '{elements[i].Name}' to '{baseElements[i].Name}'",
                                    ct => RenameTupleElementAsync(context.Document, methodDeclaration, tupleElement, elements[i], baseElements[i].Name, semanticModel, ct),
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }
                            else if (memberDeclaration is PropertyDeclarationSyntax propertyDeclaration)
                            {
                                IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

                                if (!(propertySymbol.Type is INamedTypeSymbol tupleType))
                                    break;

                                if (!tupleType.IsTupleType)
                                    break;

                                if (!(propertySymbol.OverriddenProperty?.Type is INamedTypeSymbol baseTupleType))
                                    break;

                                if (!baseTupleType.IsTupleType)
                                    break;

                                ImmutableArray<IFieldSymbol> elements = tupleType.TupleElements;
                                ImmutableArray<IFieldSymbol> baseElements = baseTupleType.TupleElements;

                                if (elements.Length != baseElements.Length)
                                    break;

                                int i = 0;
                                while (i < elements.Length)
                                {
                                    if (elements[i].Name != baseElements[i].Name)
                                        break;

                                    i++;
                                }

                                if (i == elements.Length)
                                    break;

                                TupleElementSyntax tupleElement = ((TupleTypeSyntax)propertyDeclaration.Type).Elements[i];

                                CodeAction codeAction = CodeAction.Create(
                                    $"Rename '{elements[i].Name}' to '{baseElements[i].Name}'",
                                    ct => RenameTupleElementAsync(context.Document, propertyDeclaration, tupleElement, elements[i], baseElements[i].Name, semanticModel, ct),
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }

                            break;
                        }
                }
            }
        }

        private static Task<Document> RenameTupleElementAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            TupleElementSyntax tupleElement,
            IFieldSymbol fieldSymbol,
            string newName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            MethodDeclarationSyntax newNode = methodDeclaration;

            var returnType = (TupleTypeSyntax)methodDeclaration.ReturnType;

            SyntaxToken newIdentifier = SyntaxFactory.Identifier(newName).WithTriviaFrom(tupleElement.Identifier);

            SeparatedSyntaxList<TupleElementSyntax> newElements = returnType.Elements.Replace(tupleElement, tupleElement.WithIdentifier(newIdentifier));

            newNode = methodDeclaration.WithReturnType(returnType.WithElements(newElements));

            var rewriter = new RenameRewriter(fieldSymbol, newName, semanticModel, cancellationToken);

            if (methodDeclaration.Body is BlockSyntax body)
            {
                newNode = newNode.WithBody((BlockSyntax)rewriter.VisitBlock(body));
            }
            else if (methodDeclaration.ExpressionBody is ArrowExpressionClauseSyntax expressionBody)
            {
                newNode = newNode.WithExpressionBody((ArrowExpressionClauseSyntax)rewriter.VisitArrowExpressionClause(expressionBody));
            }

            return document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken);
        }

        private static Task<Document> RenameTupleElementAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            TupleElementSyntax tupleElement,
            IFieldSymbol fieldSymbol,
            string newName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            PropertyDeclarationSyntax newNode = propertyDeclaration;

            var type = (TupleTypeSyntax)propertyDeclaration.Type;

            SyntaxToken newIdentifier = SyntaxFactory.Identifier(newName).WithTriviaFrom(tupleElement.Identifier);

            SeparatedSyntaxList<TupleElementSyntax> newElements = type.Elements.Replace(tupleElement, tupleElement.WithIdentifier(newIdentifier));

            newNode = propertyDeclaration.WithType(type.WithElements(newElements));

            var rewriter = new RenameRewriter(fieldSymbol, newName, semanticModel, cancellationToken);

            var newAccessorList = (AccessorListSyntax)rewriter.VisitAccessorList(propertyDeclaration.AccessorList);

            newNode = newNode.WithAccessorList(newAccessorList);

            return document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
        }
    }
}
