// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddMemberToInterfaceRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            MethodDeclarationSyntax methodDeclaration,
            SemanticModel semanticModel)
        {
            ComputeRefactoring(context, methodDeclaration, methodDeclaration.Modifiers, methodDeclaration.ExplicitInterfaceSpecifier, semanticModel);
        }

        public static void ComputeRefactoring(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration, SemanticModel semanticModel)
        {
            ComputeRefactoring(context, propertyDeclaration, propertyDeclaration.Modifiers, propertyDeclaration.ExplicitInterfaceSpecifier, semanticModel);
        }

        public static void ComputeRefactoring(RefactoringContext context, IndexerDeclarationSyntax indexerDeclaration, SemanticModel semanticModel)
        {
            ComputeRefactoring(context, indexerDeclaration, indexerDeclaration.Modifiers, indexerDeclaration.ExplicitInterfaceSpecifier, semanticModel);
        }

        public static void ComputeRefactoring(RefactoringContext context, EventDeclarationSyntax eventDeclaration, SemanticModel semanticModel)
        {
            ComputeRefactoring(context, eventDeclaration, eventDeclaration.Modifiers, eventDeclaration.ExplicitInterfaceSpecifier, semanticModel);
        }

        internal static void ComputeRefactoring(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration, SemanticModel semanticModel)
        {
            ComputeRefactoring(context, eventFieldDeclaration, eventFieldDeclaration.Modifiers, null, semanticModel);
        }

        private static void ComputeRefactoring(
            RefactoringContext context,
            MemberDeclarationSyntax memberDeclaration,
            SyntaxTokenList modifiers,
            ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier,
            SemanticModel semanticModel)
        {
            if (!modifiers.Contains(SyntaxKind.PublicKeyword))
                return;

            if (modifiers.Contains(SyntaxKind.StaticKeyword))
                return;

            BaseListSyntax baseList = GetBaseList(memberDeclaration.Parent);

            if (baseList == null)
                return;

            SeparatedSyntaxList<BaseTypeSyntax> types = baseList.Types;

            if (!types.Any())
                return;

            NameSyntax explicitInterfaceName = explicitInterfaceSpecifier?.Name;

            ITypeSymbol explicitInterfaceSymbol = (explicitInterfaceName != null)
                ? semanticModel.GetTypeSymbol(explicitInterfaceName, context.CancellationToken)?.OriginalDefinition
                : null;

            ISymbol memberSymbol = (memberDeclaration is EventFieldDeclarationSyntax eventFieldDeclaration)
                ? semanticModel.GetDeclaredSymbol(eventFieldDeclaration.Declaration.Variables.First())
                : semanticModel.GetDeclaredSymbol(memberDeclaration);

            if (memberSymbol == null)
                return;

            foreach (BaseTypeSyntax baseType in types)
                ComputeRefactoring(context, memberDeclaration, baseType, explicitInterfaceSymbol, memberSymbol, semanticModel);
        }

        private static void ComputeRefactoring(
            RefactoringContext context,
            MemberDeclarationSyntax memberDeclaration,
            BaseTypeSyntax baseType,
            ITypeSymbol explicitInterfaceSymbol,
            ISymbol memberSymbol,
            SemanticModel semanticModel)
        {
            TypeSyntax type = baseType.Type;

            if (type == null)
                return;

            var interfaceSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken) as INamedTypeSymbol;

            if (interfaceSymbol?.TypeKind != TypeKind.Interface)
                return;

            interfaceSymbol = interfaceSymbol.OriginalDefinition;

            if (!(interfaceSymbol.GetSyntaxOrDefault(context.CancellationToken) is InterfaceDeclarationSyntax interfaceDeclaration))
                return;

            if (interfaceSymbol.Equals(explicitInterfaceSymbol))
                return;

            ImmutableArray<ISymbol> members = interfaceSymbol.GetMembers();

            SyntaxKind kind = memberDeclaration.Kind();

            for (int i = 0; i < members.Length; i++)
            {
                if (CheckKind(members[i], kind))
                {
                    ISymbol symbol = memberSymbol.ContainingType.FindImplementationForInterfaceMember(members[i]);

                    if (memberSymbol.OriginalDefinition.Equals(symbol?.OriginalDefinition)
                        && CheckTypeParameters(memberDeclaration, interfaceSymbol))
                    {
                        return;
                    }
                }
            }

            string displayName = SymbolDisplay.GetMinimalString(interfaceSymbol, semanticModel, type.SpanStart);

            context.RegisterRefactoring(
                $"Add to interface '{displayName}'",
                cancellationToken => RefactorAsync(context.Document, memberDeclaration, interfaceDeclaration, cancellationToken),
                RefactoringIdentifiers.AddMemberToInterface + "." + displayName);
        }

        private static bool CheckTypeParameters(
            MemberDeclarationSyntax memberDeclaration,
            INamedTypeSymbol interfaceSymbol)
        {
            if (!(memberDeclaration is MethodDeclarationSyntax methodDeclaration))
                return true;

            TypeParameterListSyntax typeParameterList = methodDeclaration.TypeParameterList;

            if (typeParameterList == null)
                return true;

            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = typeParameterList.Parameters;

            if (typeParameters.Count == 0)
                return true;

            return typeParameters.Count == interfaceSymbol.TypeParameters.Length;
        }

        private static bool CheckKind(ISymbol symbol, SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        return symbol.Kind == SymbolKind.Method;
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        return symbol.Kind == SymbolKind.Property
                            && !((IPropertySymbol)symbol).IsIndexer;
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        return symbol.Kind == SymbolKind.Property
                            && ((IPropertySymbol)symbol).IsIndexer;
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        return symbol.Kind == SymbolKind.Event;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        private static BaseListSyntax GetBaseList(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax classDeclaration)
                return classDeclaration.BaseList;

            if (node is StructDeclarationSyntax structDeclaration)
                return structDeclaration.BaseList;

            return null;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            InterfaceDeclarationSyntax interfaceDeclaration,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax interfaceMember = CreateInterfaceMemberDeclaration(memberDeclaration).WithFormatterAnnotation();

            InterfaceDeclarationSyntax newNode = interfaceDeclaration.InsertMember(interfaceMember, MemberDeclarationComparer.ByKind);

            return document.ReplaceNodeAsync(interfaceDeclaration, newNode, cancellationToken);
        }

        private static MemberDeclarationSyntax CreateInterfaceMemberDeclaration(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    {
                        return MethodDeclaration(
                            default(SyntaxList<AttributeListSyntax>),
                            default(SyntaxTokenList),
                            methodDeclaration.ReturnType.WithoutTrivia(),
                            default(ExplicitInterfaceSpecifierSyntax),
                            methodDeclaration.Identifier.WithoutTrivia(),
                            methodDeclaration.TypeParameterList?.WithoutTrivia(),
                            methodDeclaration.ParameterList.WithoutTrivia(),
                            default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                            default(BlockSyntax),
                            default(ArrowExpressionClauseSyntax),
                            SemicolonToken());
                    }
                case PropertyDeclarationSyntax propertyDeclaration:
                    {
                        return PropertyDeclaration(
                            default(SyntaxList<AttributeListSyntax>),
                            default(SyntaxTokenList),
                            propertyDeclaration.Type.WithoutTrivia(),
                            default(ExplicitInterfaceSpecifierSyntax),
                            propertyDeclaration.Identifier.WithoutTrivia(),
                            CreateInterfaceAccessorList(propertyDeclaration.AccessorList));
                    }
                case IndexerDeclarationSyntax indexerDeclaration:
                    {
                        return IndexerDeclaration(
                            default(SyntaxList<AttributeListSyntax>),
                            default(SyntaxTokenList),
                            indexerDeclaration.Type.WithoutTrivia(),
                            default(ExplicitInterfaceSpecifierSyntax),
                            indexerDeclaration.ParameterList.WithoutTrivia(),
                            CreateInterfaceAccessorList(indexerDeclaration.AccessorList));
                    }
                case EventDeclarationSyntax eventDeclaration:
                    {
                        return EventFieldDeclaration(
                            default(SyntaxList<AttributeListSyntax>),
                            default(SyntaxTokenList),
                            EventKeyword(),
                            VariableDeclaration(eventDeclaration.Type.WithoutTrivia(), eventDeclaration.Identifier.WithoutTrivia()),
                            SemicolonToken());
                    }
                case EventFieldDeclarationSyntax eventFieldDeclaration:
                    {
                        return EventFieldDeclaration(
                            default(SyntaxList<AttributeListSyntax>),
                            default(SyntaxTokenList),
                            EventKeyword(),
                            eventFieldDeclaration.Declaration.WithoutTrivia(),
                            SemicolonToken());
                    }
                default:
                    {
                        throw new ArgumentException("", nameof(memberDeclaration));
                    }
            }
        }

        private static AccessorListSyntax CreateInterfaceAccessorList(AccessorListSyntax accessorList)
        {
            if (accessorList != null)
            {
                return AccessorList(accessorList
                    .Accessors
                    .Select(f =>
                    {
                        return AccessorDeclaration(
                            f.Kind(),
                            default(SyntaxList<AttributeListSyntax>),
                            default(SyntaxTokenList),
                            f.Keyword.WithoutTrivia(),
                            default(ArrowExpressionClauseSyntax),
                            SemicolonToken());
                    })
                    .ToSyntaxList());
            }
            else
            {
                return AccessorList(
                    AccessorDeclaration(
                        SyntaxKind.GetAccessorDeclaration,
                        default(SyntaxList<AttributeListSyntax>),
                        default(SyntaxTokenList),
                        GetKeyword(),
                        default(ArrowExpressionClauseSyntax),
                        SemicolonToken()));
            }
        }
    }
}
