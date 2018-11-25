// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
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
                    CreateModifiers(parameterSymbol),
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
                TokenList(accessibility),
                Identifier(className),
                ParameterList(SeparatedList(parameters)),
                BaseConstructorInitializer(ArgumentList(arguments.ToArray())),
                Block());

            return constructor.WithFormatterAnnotation();
        }

        private static SyntaxTokenList CreateModifiers(IParameterSymbol parameterSymbol)
        {
            if (parameterSymbol.IsParams)
                return TokenList(SyntaxKind.ParamsKeyword);

            switch (parameterSymbol.RefKind)
            {
                case RefKind.None:
                    return default(SyntaxTokenList);
                case RefKind.Ref:
                    return TokenList(SyntaxKind.RefKeyword);
                case RefKind.Out:
                    return TokenList(SyntaxKind.OutKeyword);
            }

            Debug.Fail(parameterSymbol.RefKind.ToString());

            return default;
        }
    }
}