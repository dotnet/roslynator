// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
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
    internal static class GenerateBaseConstructorsRefactoring
    {
        public static List<IMethodSymbol> GetMissingBaseConstructors(
            ClassDeclarationSyntax classDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            INamedTypeSymbol symbol = semanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken);

            if (symbol?.IsStatic == false)
            {
                INamedTypeSymbol baseSymbol = symbol.BaseType;

                if (baseSymbol?.IsObject() == false)
                    return GetMissingBaseConstructors(symbol, baseSymbol);
            }

            return null;
        }

        private static List<IMethodSymbol> GetMissingBaseConstructors(INamedTypeSymbol symbol, INamedTypeSymbol baseSymbol)
        {
            ImmutableArray<IMethodSymbol> constructors = symbol.InstanceConstructors.RemoveAll(f => f.IsImplicitlyDeclared);

            List<IMethodSymbol> missing = null;

            foreach (IMethodSymbol baseConstructor in GetBaseConstructors(baseSymbol))
            {
                if (IsAccessibleFromDerivedClass(baseConstructor)
                    && constructors.IndexOf(baseConstructor, ConstructorComparer.Instance) == -1)
                {
                    (missing ?? (missing = new List<IMethodSymbol>())).Add(baseConstructor);
                }
            }

            return missing;
        }

        public static bool IsAnyBaseConstructorMissing(INamedTypeSymbol symbol, INamedTypeSymbol baseSymbol)
        {
            ImmutableArray<IMethodSymbol> constructors = symbol.InstanceConstructors;

            foreach (IMethodSymbol baseConstructor in GetBaseConstructors(baseSymbol))
            {
                if (!baseConstructor.IsImplicitlyDeclared
                    && IsAccessibleFromDerivedClass(baseConstructor)
                    && constructors.IndexOf(baseConstructor, ConstructorComparer.Instance) == -1)
                {
                    return true;
                }
            }

            return false;
        }

        private static ImmutableArray<IMethodSymbol> GetBaseConstructors(INamedTypeSymbol baseSymbol)
        {
            ImmutableArray<IMethodSymbol> constructors = baseSymbol.InstanceConstructors;

            while (constructors.Length == 1
                && constructors[0].IsImplicitlyDeclared
                && baseSymbol.BaseType?.IsObject() == false)
            {
                baseSymbol = baseSymbol.BaseType;

                constructors = baseSymbol.InstanceConstructors;
            }

            return constructors;
        }

        private static bool IsAccessibleFromDerivedClass(IMethodSymbol methodSymbol)
        {
            Accessibility accessibility = methodSymbol.DeclaredAccessibility;

            return accessibility == Accessibility.Public
                || accessibility == Accessibility.Protected
                || accessibility == Accessibility.ProtectedOrInternal;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            IMethodSymbol[] constructorSymbols,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxList<MemberDeclarationSyntax> members = classDeclaration.Members;

            string className = classDeclaration.Identifier.ValueText;

            bool isSealedClass = classDeclaration.Modifiers.Contains(SyntaxKind.SealedKeyword);

            int insertIndex = MemberDeclarationComparer.ByKind.GetInsertIndex(members, SyntaxKind.ConstructorDeclaration);

            int position = (insertIndex == 0)
                ? classDeclaration.OpenBraceToken.FullSpan.End
                : members[insertIndex - 1].FullSpan.End;

            IEnumerable<ConstructorDeclarationSyntax> constructors = constructorSymbols
                .Select(symbol => CreateConstructor(symbol, className, isSealedClass, semanticModel, position));

            ClassDeclarationSyntax newClassDeclaration = classDeclaration
                .WithMembers(members.InsertRange(insertIndex, constructors));

            return document.ReplaceNodeAsync(classDeclaration, newClassDeclaration, cancellationToken);
        }

        private static ConstructorDeclarationSyntax CreateConstructor(
            IMethodSymbol methodSymbol,
            string className,
            bool isSealedClass,
            SemanticModel semanticModel,
            int position)
        {
            var parameters = new List<ParameterSyntax>();
            var arguments = new List<ArgumentSyntax>();

            foreach (IParameterSymbol parameterSymbol in methodSymbol.Parameters)
            {
                EqualsValueClauseSyntax @default = null;

                if (parameterSymbol.HasExplicitDefaultValue)
                {
                    ExpressionSyntax defaultValue = parameterSymbol.GetDefaultValueSyntax();

                    if (defaultValue != null)
                        @default = EqualsValueClause(defaultValue.WithSimplifierAnnotation());
                }

                parameters.Add(Parameter(
                    default(SyntaxList<AttributeListSyntax>),
                    Modifiers.FromAccessibility(parameterSymbol.DeclaredAccessibility),
                    parameterSymbol.Type.ToMinimalTypeSyntax(semanticModel, position),
                    Identifier(parameterSymbol.Name),
                    @default));

                arguments.Add(Argument(IdentifierName(parameterSymbol.Name)));
            }

            Accessibility accessibility = methodSymbol.DeclaredAccessibility;

            if (isSealedClass)
            {
                if (accessibility == Accessibility.ProtectedOrInternal)
                {
                    accessibility = Accessibility.Internal;
                }
                else if (accessibility == Accessibility.Protected)
                {
                    accessibility = Accessibility.Private;
                }
            }

            ConstructorDeclarationSyntax constructor = ConstructorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                Modifiers.FromAccessibility(accessibility),
                Identifier(className),
                ParameterList(SeparatedList(parameters)),
                BaseConstructorInitializer(ArgumentList(arguments.ToArray())),
                Block());

            return constructor.WithFormatterAnnotation();
        }

        private class ConstructorComparer : EqualityComparer<IMethodSymbol>
        {
            public static ConstructorComparer Instance { get; } = new ConstructorComparer();

            public override bool Equals(IMethodSymbol x, IMethodSymbol y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null || y == null)
                    return false;

                ImmutableArray<IParameterSymbol> parameters1 = x.Parameters;
                ImmutableArray<IParameterSymbol> parameters2 = y.Parameters;

                if (parameters1.Length != parameters2.Length)
                    return false;

                for (int i = 0; i < parameters1.Length; i++)
                {
                    if (!parameters1[i].Type.Equals(parameters2[i].Type))
                        return false;
                }

                return true;
            }

            public override int GetHashCode(IMethodSymbol obj)
            {
                return 0;
            }
        }
    }
}