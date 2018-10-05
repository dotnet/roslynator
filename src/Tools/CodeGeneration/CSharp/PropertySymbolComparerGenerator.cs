// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CodeGeneration.CSharp.CSharpFactory2;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class PropertySymbolComparerGenerator
    {
        public static CompilationUnitSyntax Generate()
        {
            return CompilationUnit(
                UsingDirectives(
                    "System",
                    "Microsoft.CodeAnalysis"),
                NamespaceDeclaration("Roslynator.CodeGeneration.CSharp",
                    ClassDeclaration(
                        default(SyntaxList<AttributeListSyntax>),
                        Modifiers.Internal_Static(),
                        Identifier("PropertySymbolComparer"),
                        default(TypeParameterListSyntax),
                        default(BaseListSyntax),
                        default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                        SingletonList<MemberDeclarationSyntax>(GenerateMethodDeclaration()))));
        }

        private static MethodDeclarationSyntax GenerateMethodDeclaration()
        {
            return MethodDeclaration(
                Modifiers.Public_Static(),
                PredefinedIntType(),
                Identifier("GetRank"),
                ParameterList(Parameter(IdentifierName("IPropertySymbol"), "x")),
                Block(
                    SwitchStatement(
                        SimpleMemberAccessExpression(
                            SimpleMemberAccessExpression(
                                IdentifierName("x"),
                                IdentifierName("ContainingType")), IdentifierName("Name")),
                        GenerateSections().ToSyntaxList().Add(DefaultSwitchSection(Block(ThrowNewInvalidOperationException()))))));

            IEnumerable<SwitchSectionSyntax> GenerateSections()
            {
                foreach (INamedTypeSymbol typeSymbol in Symbols.SyntaxSymbols)
                {
                    SyntaxList<SwitchSectionSyntax> sections = GenerateSections2(typeSymbol).ToSyntaxList();

                    if (sections.Count > 1)
                    {
                        yield return SwitchSection(
                            CaseSwitchLabel(StringLiteralExpression(typeSymbol.Name)),
                            Block(
                                SwitchStatement(
                                    SimpleMemberAccessExpression(IdentifierName("x"), IdentifierName("Name")),
                                    sections.Add(DefaultSwitchSection(Block(ThrowNewInvalidOperationException()))))));
                    }
                }

                IEnumerable<SwitchSectionSyntax> GenerateSections2(INamedTypeSymbol typeSymbol)
                {
                    int i = 0;

                    foreach (IPropertySymbol propertySymbol in Symbols.GetPropertySymbols(typeSymbol))
                    {
                        yield return SwitchSection(
                            CaseSwitchLabel(StringLiteralExpression(propertySymbol.Name)),
                            ReturnStatement(NumericLiteralExpression(i)));

                        i++;
                    }
                }
            }
        }
    }
}
