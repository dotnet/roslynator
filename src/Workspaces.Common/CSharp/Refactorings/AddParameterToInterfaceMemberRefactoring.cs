// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddParameterToInterfaceMemberRefactoring
    {
        public static CodeAction ComputeRefactoringForExplicitImplementation(
            CommonFixContext context,
            MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    {
                        return ComputeRefactoringForExplicitImplementation(
                            context,
                            methodDeclaration,
                            methodDeclaration.ExplicitInterfaceSpecifier,
                            methodDeclaration.ParameterList?.Parameters ?? default);
                    }
                case IndexerDeclarationSyntax indexerDeclaration:
                    {
                        return ComputeRefactoringForExplicitImplementation(
                            context,
                            indexerDeclaration,
                            indexerDeclaration.ExplicitInterfaceSpecifier,
                            indexerDeclaration.ParameterList?.Parameters ?? default);
                    }
            }

            return default;
        }

        public static OneOrMany<CodeAction> ComputeRefactoringForImplicitImplementation(
            CommonFixContext context,
            MethodDeclarationSyntax methodDeclaration)
        {
            return ComputeRefactoringForImplicitImplementation(
                context,
                methodDeclaration,
                methodDeclaration.Modifiers,
                methodDeclaration.ExplicitInterfaceSpecifier,
                methodDeclaration.ParameterList?.Parameters ?? default);
        }

        public static OneOrMany<CodeAction> ComputeRefactoringForImplicitImplementation(
            CommonFixContext context,
            IndexerDeclarationSyntax indexerDeclaration)
        {
            return ComputeRefactoringForImplicitImplementation(
                context,
                indexerDeclaration,
                indexerDeclaration.Modifiers,
                indexerDeclaration.ExplicitInterfaceSpecifier,
                indexerDeclaration.ParameterList?.Parameters ?? default);
        }

        private static OneOrMany<CodeAction> ComputeRefactoringForImplicitImplementation(
            CommonFixContext context,
            MemberDeclarationSyntax memberDeclaration,
            SyntaxTokenList modifiers,
            ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier,
            SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            if (!parameters.Any())
                return default;

            if (modifiers.Contains(SyntaxKind.StaticKeyword))
                return default;

            if (explicitInterfaceSpecifier != null)
                return default;

            return ComputeRefactoringForImplicitImplementation(
                context,
                memberDeclaration,
                modifiers);
        }

        public static CodeAction ComputeRefactoringForExplicitImplementation(
            CommonFixContext context,
            MemberDeclarationSyntax memberDeclaration,
            ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier,
            SeparatedSyntaxList<ParameterSyntax> parameters)
        {
            if (!parameters.Any())
                return default;

            NameSyntax explicitInterfaceName = explicitInterfaceSpecifier?.Name;

            if (explicitInterfaceName == null)
                return default;

            var interfaceSymbol = (INamedTypeSymbol)context.SemanticModel.GetTypeSymbol(explicitInterfaceName, context.CancellationToken);

            if (interfaceSymbol?.TypeKind != TypeKind.Interface)
                return default;

            if (!(interfaceSymbol.GetSyntaxOrDefault(context.CancellationToken) is InterfaceDeclarationSyntax _))
                return default;

            ISymbol memberSymbol = context.SemanticModel.GetDeclaredSymbol(memberDeclaration, context.CancellationToken);

            if (memberSymbol == null)
                return default;

            ISymbol interfaceMemberSymbol = FindInterfaceMember(memberSymbol, interfaceSymbol);

            if (interfaceMemberSymbol == null)
                return default;

            return ComputeCodeAction(context, memberDeclaration, memberSymbol, interfaceMemberSymbol);
        }

        private static OneOrMany<CodeAction> ComputeRefactoringForImplicitImplementation(
            CommonFixContext context,
            MemberDeclarationSyntax memberDeclaration,
            SyntaxTokenList modifiers)
        {
            if (!modifiers.Contains(SyntaxKind.PublicKeyword))
                return default;

            BaseListSyntax baseList = GetBaseList(memberDeclaration.Parent);

            if (baseList == null)
                return default;

            SeparatedSyntaxList<BaseTypeSyntax> baseTypes = baseList.Types;

            ISymbol memberSymbol = context.SemanticModel.GetDeclaredSymbol(memberDeclaration, context.CancellationToken);

            if (memberSymbol == null)
                return default;

            if (memberSymbol.ImplementsInterfaceMember())
                return default;

            int count = 0;

            CodeAction singleCodeAction = null;
            List<CodeAction> codeActions = default;

            for (int i = 0; i < baseTypes.Count; i++)
            {
                BaseTypeSyntax baseType = baseTypes[i];

                Diagnostic diagnostic = context.SemanticModel.GetDiagnostic("CS0535", baseType.Type.Span, context.CancellationToken);

                if (diagnostic?.Location.SourceSpan != baseType.Type.Span)
                    continue;

                var interfaceSymbol = context.SemanticModel.GetTypeSymbol(baseType.Type, context.CancellationToken) as INamedTypeSymbol;

                if (interfaceSymbol?.TypeKind != TypeKind.Interface)
                    continue;

                if (!(interfaceSymbol.GetSyntaxOrDefault(context.CancellationToken) is InterfaceDeclarationSyntax _))
                    continue;

                ISymbol interfaceMemberSymbol = FindInterfaceMember(memberSymbol, interfaceSymbol);

                if (interfaceMemberSymbol != null)
                {
                    CodeAction codeAction = ComputeCodeAction(context, memberDeclaration, memberSymbol, interfaceMemberSymbol);

                    if (singleCodeAction == null)
                    {
                        singleCodeAction = codeAction;
                    }
                    else
                    {
                        (codeActions ??= new List<CodeAction>() { singleCodeAction }).Add(codeAction);
                    }

                    count++;

                    if (count == 10)
                        break;
                }
            }

            if (codeActions != null)
            {
                return OneOrMany.Create(codeActions.ToImmutableArray());
            }
            else if (singleCodeAction != null)
            {
                return OneOrMany.Create(singleCodeAction);
            }

            return default;
        }

        private static ISymbol FindInterfaceMember(
            ISymbol memberSymbol,
            INamedTypeSymbol interfaceSymbol)
        {
            switch (memberSymbol.Kind)
            {
                case SymbolKind.Method:
                    {
                        return FindInterfaceMethod((IMethodSymbol)memberSymbol, interfaceSymbol);
                    }
                case SymbolKind.Property:
                    {
                        var propertySymbol = (IPropertySymbol)memberSymbol;

                        if (propertySymbol.IsIndexer)
                            return FindInterfaceIndexer(propertySymbol, interfaceSymbol);

                        break;
                    }
            }

            return null;
        }

        private static ISymbol FindInterfaceMethod(
            IMethodSymbol methodSymbol,
            INamedTypeSymbol interfaceSymbol)
        {
            ImmutableArray<ISymbol> members = interfaceSymbol.GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                ISymbol memberSymbol = members[i];

                if (memberSymbol.Kind != SymbolKind.Method)
                    continue;

                var methodSymbol2 = (IMethodSymbol)memberSymbol;

                if (methodSymbol2.MethodKind != MethodKind.Ordinary)
                    continue;

                if (methodSymbol.MethodKind == MethodKind.ExplicitInterfaceImplementation)
                {
                    int dotIndex = methodSymbol.Name.LastIndexOf('.');

                    if (string.Compare(methodSymbol.Name, dotIndex + 1, methodSymbol2.Name, 0, methodSymbol2.Name.Length) != 0)
                        continue;
                }
                else if (methodSymbol.Name != methodSymbol2.Name)
                {
                    continue;
                }

                if (methodSymbol.TypeParameters.Length != methodSymbol2.TypeParameters.Length)
                    continue;

                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;
                ImmutableArray<IParameterSymbol> parameters2 = methodSymbol2.Parameters;

                if (parameters.Length != parameters2.Length + 1)
                    continue;

                if (!SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, methodSymbol2.ReturnType))
                    continue;

                if (!ParametersEqual(parameters, parameters2))
                    continue;

                return memberSymbol;
            }

            return null;
        }

        private static ISymbol FindInterfaceIndexer(
            IPropertySymbol propertySymbol,
            INamedTypeSymbol interfaceSymbol)
        {
            ImmutableArray<ISymbol> members = interfaceSymbol.GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                ISymbol memberSymbol = members[i];

                if (memberSymbol.Kind != SymbolKind.Property)
                    continue;

                var propertySymbol2 = (IPropertySymbol)memberSymbol;

                if (!propertySymbol2.IsIndexer)
                    continue;

                ImmutableArray<IParameterSymbol> parameters = propertySymbol.Parameters;
                ImmutableArray<IParameterSymbol> parameters2 = propertySymbol2.Parameters;

                if (parameters.Length != parameters2.Length + 1)
                    continue;

                if (!SymbolEqualityComparer.Default.Equals(propertySymbol.Type, propertySymbol2.Type))
                    continue;

                if (!ParametersEqual(parameters, parameters2))
                    continue;

                return memberSymbol;
            }

            return null;
        }

        private static bool ParametersEqual(ImmutableArray<IParameterSymbol> parameters, ImmutableArray<IParameterSymbol> parameters2)
        {
            for (int j = 0; j < parameters.Length - 1; j++)
            {
                if (parameters[j].RefKind != parameters2[j].RefKind)
                    return false;

                if (!SymbolEqualityComparer.Default.Equals(parameters[j].Type, parameters2[j].Type))
                    return false;
            }

            return true;
        }

        private static CodeAction ComputeCodeAction(
            CommonFixContext context,
            MemberDeclarationSyntax memberDeclaration,
            ISymbol memberSymbol,
            ISymbol interfaceMemberSymbol)
        {
            IParameterSymbol parameterSymbol = memberSymbol.GetParameters().Last();

            string title = $"Add parameter '{parameterSymbol.Name}' to '{SymbolDisplay.ToMinimalDisplayString(interfaceMemberSymbol.OriginalDefinition, context.SemanticModel, memberDeclaration.SpanStart, SymbolDisplayFormat.CSharpShortErrorMessageFormat)}'";

            string equivalenceKey = EquivalenceKey.Join(context.EquivalenceKey, interfaceMemberSymbol.OriginalDefinition.GetDocumentationCommentId());

            var interfaceMemberDeclaration = (MemberDeclarationSyntax)interfaceMemberSymbol.GetSyntax();

            if (memberDeclaration.SyntaxTree == interfaceMemberDeclaration.SyntaxTree)
            {
                return CodeAction.Create(
                    title,
                    ct =>
                    {
                        MemberDeclarationSyntax newNode = AddParameter(interfaceMemberDeclaration, parameterSymbol).WithFormatterAnnotation();

                        return context.Document.ReplaceNodeAsync(interfaceMemberDeclaration, newNode, ct);
                    },
                    equivalenceKey);
            }
            else
            {
                return CodeAction.Create(
                    title,
                    ct =>
                    {
                        MemberDeclarationSyntax newNode = AddParameter(interfaceMemberDeclaration, parameterSymbol).WithFormatterAnnotation();

                        return context.Document.Solution().ReplaceNodeAsync(interfaceMemberDeclaration, newNode, ct);
                    },
                    equivalenceKey);
            }
        }

        private static MemberDeclarationSyntax AddParameter(MemberDeclarationSyntax memberDeclaration, IParameterSymbol parameterSymbol)
        {
            ParameterSyntax parameter = CreateParameter(parameterSymbol);

            switch (memberDeclaration)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    return methodDeclaration.AddParameterListParameters(parameter);
                case IndexerDeclarationSyntax indexerDeclaration:
                    return indexerDeclaration.AddParameterListParameters(parameter);
                default:
                    throw new InvalidOperationException();
            }
        }

        private static BaseListSyntax GetBaseList(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).BaseList;
                case SyntaxKind.RecordDeclaration:
                    return ((RecordDeclarationSyntax)node).BaseList;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).BaseList;
                default:
                    return null;
            }
        }

        private static ParameterSyntax CreateParameter(IParameterSymbol parameterSymbol)
        {
            ExpressionSyntax defaultValue = (parameterSymbol.HasExplicitDefaultValue)
                ? parameterSymbol.GetDefaultValueSyntax()
                : null;

            return Parameter(
                default(SyntaxList<AttributeListSyntax>),
                GetModifiers(),
                parameterSymbol.Type.ToTypeSyntax().WithSimplifierAnnotation(),
                Identifier(parameterSymbol.Name),
                (defaultValue != null) ? EqualsValueClause(defaultValue) : default);

            SyntaxTokenList GetModifiers()
            {
                switch (parameterSymbol.RefKind)
                {
                    case RefKind.Ref:
                        return TokenList(Token(SyntaxKind.RefKeyword));
                    case RefKind.Out:
                        return TokenList(Token(SyntaxKind.OutKeyword));
                    case RefKind.None:
                        return default;
                }

                Debug.Fail(parameterSymbol.RefKind.ToString());

                return default;
            }
        }
    }
}
