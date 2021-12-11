// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberDeclarationCodeFixProvider))]
    [Shared]
    public sealed class MemberDeclarationCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS1591_MissingXmlCommentForPubliclyVisibleTypeOrMember,
                    CompilerDiagnosticIdentifiers.CS0508_MethodReturnTypeMustMatchOverriddenMethodReturnType,
                    CompilerDiagnosticIdentifiers.CS1715_MemberTypeMustMatchOverriddenMemberType,
                    CompilerDiagnosticIdentifiers.CS0260_MissingPartialModifier,
                    CompilerDiagnosticIdentifiers.CS0751_PartialMethodMustBeDeclaredInPartialClassOrPartialStruct,
                    CompilerDiagnosticIdentifiers.CS0513_MemberIsAbstractButItIsContainedInNonAbstractClass,
                    CompilerDiagnosticIdentifiers.CS0132_StaticConstructorMustBeParameterless,
                    CompilerDiagnosticIdentifiers.CS0766_PartialMethodsMustHaveVoidReturnType,
                    CompilerDiagnosticIdentifiers.CS0541_ExplicitInterfaceDeclarationCanOnlyBeDeclaredInClassOrStruct,
                    CompilerDiagnosticIdentifiers.CS0525_InterfacesCannotContainFields,
                    CompilerDiagnosticIdentifiers.CS0567_InterfacesCannotContainOperators,
                    CompilerDiagnosticIdentifiers.CS0524_InterfacesCannotDeclareTypes,
                    CompilerDiagnosticIdentifiers.CS0575_OnlyClassTypesCanContainDestructors,
                    CompilerDiagnosticIdentifiers.CS0568_StructsCannotContainExplicitParameterlessConstructors,
                    CompilerDiagnosticIdentifiers.CS0574_NameOfDestructorMustMatchNameOfClass,
                    CompilerDiagnosticIdentifiers.CS8139_CannotChangeTupleElementNameWhenOverridingInheritedMember,
                    CompilerDiagnosticIdentifiers.CS3000_MethodsWithVariableArgumentsAreNotCLSCompliant,
                    CompilerDiagnosticIdentifiers.CS3001_ArgumentTypeIsNotCLSCompliant,
                    CompilerDiagnosticIdentifiers.CS3002_ReturnTypeIsNotCLSCompliant,
                    CompilerDiagnosticIdentifiers.CS3003_TypeOfVariableIsNotCLSCompliant,
                    CompilerDiagnosticIdentifiers.CS3005_IdentifierDifferingOnlyInCaseIsNotCLSCompliant,
                    CompilerDiagnosticIdentifiers.CS3006_OverloadedMethodDifferingOnlyInRefOrOutOrInArrayRankIsNotCLSCompliant,
                    CompilerDiagnosticIdentifiers.CS3007_OverloadedMethodDifferingOnlyByUnnamedArrayTypesIsNotCLSCompliant,
                    CompilerDiagnosticIdentifiers.CS3008_IdentifierIsNotCLSCompliant,
                    CompilerDiagnosticIdentifiers.CS3009_BaseTypeIsNotCLSCompliant,
                    CompilerDiagnosticIdentifiers.CS3016_ArraysAsAttributeArgumentsIsNotCLSCompliant,
                    CompilerDiagnosticIdentifiers.CS3024_ConstraintTypeIsNotCLSCompliant,
                    CompilerDiagnosticIdentifiers.CS3027_TypeIsNotCLSCompliantBecauseBaseInterfaceIsNotCLSCompliant,
                    CompilerDiagnosticIdentifiers.CS0539_ExplicitInterfaceDeclarationIsNotMemberOfInterface);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out MemberDeclarationSyntax memberDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CS1591_MissingXmlCommentForPubliclyVisibleTypeOrMember:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddDocumentationComment, context.Document, root.SyntaxTree))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Add documentation comment",
                                ct => AddDocumentationCommentRefactoring.RefactorAsync(context.Document, memberDeclaration, false, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);

                            CodeAction codeAction2 = CodeAction.Create(
                                "Add documentation comment (copy from base if available)",
                                ct => AddDocumentationCommentRefactoring.RefactorAsync(context.Document, memberDeclaration, true, ct),
                                GetEquivalenceKey(diagnostic, "CopyFromBaseIfAvailable"));

                            context.RegisterCodeFix(codeAction2, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0508_MethodReturnTypeMustMatchOverriddenMethodReturnType:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeMethodReturnType, context.Document, root.SyntaxTree))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            var methodSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(memberDeclaration, context.CancellationToken);

                            ITypeSymbol typeSymbol = methodSymbol.OverriddenMethod.ReturnType;

                            CodeFixRegistrator.ChangeTypeOrReturnType(context, diagnostic, memberDeclaration, typeSymbol, semanticModel);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0766_PartialMethodsMustHaveVoidReturnType:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeMethodReturnType, context.Document, root.SyntaxTree))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;

                            MethodDeclarationSyntax otherPart = semanticModel.GetOtherPart(methodDeclaration, context.CancellationToken);

                            if (otherPart == null)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Change return type to 'void'",
                                ct =>
                                {
                                    return context.Document.Solution().ReplaceNodesAsync(
                                        new MethodDeclarationSyntax[] { methodDeclaration, otherPart },
                                        (node, _) => node.WithReturnType(CSharpFactory.VoidType().WithTriviaFrom(node.ReturnType)),
                                        ct);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS1715_MemberTypeMustMatchOverriddenMemberType:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.MemberTypeMustMatchOverriddenMemberType, context.Document, root.SyntaxTree))
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
                                        VariableDeclaratorSyntax declarator = ((EventFieldDeclarationSyntax)memberDeclaration).Declaration.Variables[0];

                                        var eventSymbol = (IEventSymbol)semanticModel.GetDeclaredSymbol(declarator, context.CancellationToken);

                                        typeSymbol = eventSymbol.OverriddenEvent.Type;
                                        break;
                                    }
                            }

                            CodeFixRegistrator.ChangeTypeOrReturnType(context, diagnostic, memberDeclaration, typeSymbol, semanticModel);

                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0260_MissingPartialModifier:
                    case CompilerDiagnosticIdentifiers.CS0751_PartialMethodMustBeDeclaredInPartialClassOrPartialStruct:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddPartialModifier, context.Document, root.SyntaxTree))
                                break;

                            SyntaxNode node = null;

                            switch (memberDeclaration.Kind())
                            {
                                case SyntaxKind.MethodDeclaration:
                                    {
                                        if (memberDeclaration.IsParentKind(
                                            SyntaxKind.ClassDeclaration,
                                            SyntaxKind.StructDeclaration,
                                            SyntaxKind.RecordDeclaration,
                                            SyntaxKind.RecordStructDeclaration))
                                        {
                                            node = memberDeclaration.Parent;
                                        }

                                        break;
                                    }
                                case SyntaxKind.ClassDeclaration:
                                case SyntaxKind.StructDeclaration:
                                case SyntaxKind.RecordStructDeclaration:
                                case SyntaxKind.InterfaceDeclaration:
                                case SyntaxKind.RecordDeclaration:
                                    {
                                        node = memberDeclaration;
                                        break;
                                    }
                            }

                            SyntaxDebug.Assert(node != null, memberDeclaration);

                            if (node == null)
                                break;

                            ModifiersCodeFixRegistrator.AddModifier(context, diagnostic, node, SyntaxKind.PartialKeyword, title: $"Make {CSharpFacts.GetTitle(node)} partial");
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0513_MemberIsAbstractButItIsContainedInNonAbstractClass:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.MakeContainingClassAbstract, context.Document, root.SyntaxTree))
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
                    case CompilerDiagnosticIdentifiers.CS0132_StaticConstructorMustBeParameterless:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveParametersFromStaticConstructor, context.Document, root.SyntaxTree))
                                break;

                            var constructorDeclaration = (ConstructorDeclarationSyntax)memberDeclaration;

                            CodeAction codeAction = CodeAction.Create(
                                "Remove parameters",
                                ct =>
                                {
                                    ParameterListSyntax parameterList = constructorDeclaration.ParameterList;

                                    ParameterListSyntax newParameterList = parameterList
                                        .WithParameters(default(SeparatedSyntaxList<ParameterSyntax>))
                                        .WithOpenParenToken(parameterList.OpenParenToken.WithoutTrailingTrivia())
                                        .WithCloseParenToken(parameterList.CloseParenToken.WithoutLeadingTrivia());

                                    ConstructorDeclarationSyntax newNode = constructorDeclaration.WithParameterList(newParameterList);

                                    return context.Document.ReplaceNodeAsync(constructorDeclaration, newNode, ct);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0541_ExplicitInterfaceDeclarationCanOnlyBeDeclaredInClassOrStruct:
                    case CompilerDiagnosticIdentifiers.CS0525_InterfacesCannotContainFields:
                    case CompilerDiagnosticIdentifiers.CS0567_InterfacesCannotContainOperators:
                    case CompilerDiagnosticIdentifiers.CS0524_InterfacesCannotDeclareTypes:
                    case CompilerDiagnosticIdentifiers.CS0575_OnlyClassTypesCanContainDestructors:
                    case CompilerDiagnosticIdentifiers.CS0568_StructsCannotContainExplicitParameterlessConstructors:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveMemberDeclaration, context.Document, root.SyntaxTree))
                                break;

                            CodeFixRegistrator.RemoveMemberDeclaration(context, diagnostic, memberDeclaration);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0574_NameOfDestructorMustMatchNameOfClass:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RenameDestructorToMatchClassName, context.Document, root.SyntaxTree))
                                break;

                            if (memberDeclaration is not DestructorDeclarationSyntax destructorDeclaration)
                                break;

                            if (memberDeclaration.Parent is not ClassDeclarationSyntax classDeclaration)
                                break;

                            if (classDeclaration.Identifier.ValueText.Length == 0)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Rename destructor to match class name",
                                ct =>
                                {
                                    DestructorDeclarationSyntax newNode = destructorDeclaration.WithIdentifier(classDeclaration.Identifier.WithTriviaFrom(destructorDeclaration.Identifier));

                                    return context.Document.ReplaceNodeAsync(destructorDeclaration, newNode, ct);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS8139_CannotChangeTupleElementNameWhenOverridingInheritedMember:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RenameTupleElement, context.Document, root.SyntaxTree))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            if (memberDeclaration is MethodDeclarationSyntax methodDeclaration)
                            {
                                IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

                                if (methodSymbol.ReturnType is not INamedTypeSymbol tupleType)
                                    break;

                                if (!tupleType.IsTupleType)
                                    break;

                                if (methodSymbol.OverriddenMethod?.ReturnType is not INamedTypeSymbol baseTupleType)
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

                                if (propertySymbol.Type is not INamedTypeSymbol tupleType)
                                    break;

                                if (!tupleType.IsTupleType)
                                    break;

                                if (propertySymbol.OverriddenProperty?.Type is not INamedTypeSymbol baseTupleType)
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
                    case CompilerDiagnosticIdentifiers.CS3000_MethodsWithVariableArgumentsAreNotCLSCompliant:
                    case CompilerDiagnosticIdentifiers.CS3001_ArgumentTypeIsNotCLSCompliant:
                    case CompilerDiagnosticIdentifiers.CS3002_ReturnTypeIsNotCLSCompliant:
                    case CompilerDiagnosticIdentifiers.CS3003_TypeOfVariableIsNotCLSCompliant:
                    case CompilerDiagnosticIdentifiers.CS3005_IdentifierDifferingOnlyInCaseIsNotCLSCompliant:
                    case CompilerDiagnosticIdentifiers.CS3006_OverloadedMethodDifferingOnlyInRefOrOutOrInArrayRankIsNotCLSCompliant:
                    case CompilerDiagnosticIdentifiers.CS3007_OverloadedMethodDifferingOnlyByUnnamedArrayTypesIsNotCLSCompliant:
                    case CompilerDiagnosticIdentifiers.CS3008_IdentifierIsNotCLSCompliant:
                    case CompilerDiagnosticIdentifiers.CS3009_BaseTypeIsNotCLSCompliant:
                    case CompilerDiagnosticIdentifiers.CS3016_ArraysAsAttributeArgumentsIsNotCLSCompliant:
                    case CompilerDiagnosticIdentifiers.CS3024_ConstraintTypeIsNotCLSCompliant:
                    case CompilerDiagnosticIdentifiers.CS3027_TypeIsNotCLSCompliantBecauseBaseInterfaceIsNotCLSCompliant:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.MarkDeclarationAsNonCLSCompliant, context.Document, root.SyntaxTree))
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                $"Mark {CSharpFacts.GetTitle(memberDeclaration)} as non-CLS-compliant",
                                ct => MarkDeclarationAsNonCLSCompliantAsync(context.Document, memberDeclaration, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.CS0539_ExplicitInterfaceDeclarationIsNotMemberOfInterface:
                        {
                            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddParameterToExplicitlyImplementedInterfaceMember, context.Document, root.SyntaxTree))
                                break;

                            var context2 = new CommonFixContext(
                                context.Document,
                                GetEquivalenceKey(diagnostic),
                                await context.GetSemanticModelAsync().ConfigureAwait(false),
                                context.CancellationToken);

                            CodeAction codeAction = AddParameterToInterfaceMemberRefactoring.ComputeRefactoringForExplicitImplementation(context2, memberDeclaration);

                            if (codeAction != null)
                                context.RegisterCodeFix(codeAction, diagnostic);

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
            var returnType = (TupleTypeSyntax)methodDeclaration.ReturnType;

            SyntaxToken newIdentifier = Identifier(newName).WithTriviaFrom(tupleElement.Identifier);

            SeparatedSyntaxList<TupleElementSyntax> newElements = returnType.Elements.Replace(tupleElement, tupleElement.WithIdentifier(newIdentifier));

            MethodDeclarationSyntax newNode = methodDeclaration.WithReturnType(returnType.WithElements(newElements));

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
            var type = (TupleTypeSyntax)propertyDeclaration.Type;

            SyntaxToken newIdentifier = Identifier(newName).WithTriviaFrom(tupleElement.Identifier);

            SeparatedSyntaxList<TupleElementSyntax> newElements = type.Elements.Replace(tupleElement, tupleElement.WithIdentifier(newIdentifier));
            PropertyDeclarationSyntax newNode = propertyDeclaration.WithType(type.WithElements(newElements));

            var rewriter = new RenameRewriter(fieldSymbol, newName, semanticModel, cancellationToken);

            var newAccessorList = (AccessorListSyntax)rewriter.VisitAccessorList(propertyDeclaration.AccessorList);

            newNode = newNode.WithAccessorList(newAccessorList);

            return document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
        }

        private static Task<Document> MarkDeclarationAsNonCLSCompliantAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
            AttributeListSyntax attributeList = AttributeList(
                Attribute(
                    ParseName("global::System.CLSCompliantAttribute").WithSimplifierAnnotation(),
                    AttributeArgument(FalseLiteralExpression())))
                .WithFormatterAnnotation();

            MemberDeclarationSyntax newMemberDeclaration = SyntaxRefactorings.AddAttributeLists(memberDeclaration, keepDocumentationCommentOnTop: true, attributeList);

            return document.ReplaceNodeAsync(memberDeclaration, newMemberDeclaration, cancellationToken);
        }
    }
}
