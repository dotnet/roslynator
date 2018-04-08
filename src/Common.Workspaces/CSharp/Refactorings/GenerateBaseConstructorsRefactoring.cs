// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenerateBaseConstructorsRefactoring
    {
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

            int insertIndex = MemberDeclarationInserter.Default.GetInsertIndex(members, SyntaxKind.ConstructorDeclaration);

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
                    ExpressionSyntax defaultValue = parameterSymbol.GetDefaultValueMinimalSyntax(semanticModel, position);

                    if (defaultValue != null)
                        @default = EqualsValueClause(defaultValue.WithSimplifierAnnotation());
                }

                parameters.Add(Parameter(
                    default(SyntaxList<AttributeListSyntax>),
                    Modifiers.FromParameterSymbol(parameterSymbol),
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
                else if (accessibility == Accessibility.Protected
                    || accessibility == Accessibility.ProtectedAndInternal)
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

        private class ParametersComparer : EqualityComparer<IMethodSymbol>
        {
            public static ParametersComparer Instance { get; } = new ParametersComparer();

            public override bool Equals(IMethodSymbol x, IMethodSymbol y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
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