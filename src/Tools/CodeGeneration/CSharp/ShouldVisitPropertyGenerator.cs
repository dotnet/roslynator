// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CodeGeneration.CSharp.CSharpFactory2;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class ShouldVisitPropertyGenerator
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
                        Modifiers.Public_Partial(),
                        Identifier("CSharpSyntaxWalkerGenerator"),
                        default(TypeParameterListSyntax),
                        default(BaseListSyntax),
                        default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                        SingletonList<MemberDeclarationSyntax>(GenerateMethodDeclaration()))));
        }

        private static MethodDeclarationSyntax GenerateMethodDeclaration()
        {
            return MethodDeclaration(
                Modifiers.Protected_Virtual(),
                PredefinedBoolType(),
                Identifier("ShouldVisit"),
                ParameterList(Parameter(IdentifierName("IPropertySymbol"), "propertySymbol")),
                Block(
                    SwitchStatement(
                        SimpleMemberAccessExpression(
                            SimpleMemberAccessExpression(
                                IdentifierName("propertySymbol"),
                                IdentifierName("ContainingType")), IdentifierName("Name")),
                        GenerateSections().ToSyntaxList().Add(DefaultSwitchSection(Block(ThrowNewInvalidOperationException(ParseExpression(@"$""Unrecognized type '{propertySymbol.ContainingType.Name}'"""))))))));

            IEnumerable<SwitchSectionSyntax> GenerateSections()
            {
                foreach (INamedTypeSymbol typeSymbol in Symbols.SyntaxSymbols)
                {
                    if (typeSymbol.IsAbstract)
                        continue;

                    SyntaxList<SwitchSectionSyntax> sections = default;

                    foreach (IGrouping<bool, IPropertySymbol> grouping in Symbols.GetPropertySymbols(typeSymbol, skipObsolete: false)
                        .GroupBy(f => f.HasAttribute(MetadataNames.System_ObsoleteAttribute)
                            || (typeSymbol.Name == "AnonymousMethodExpressionSyntax" && f.Name == "Block"))
                        .OrderBy(f => f.Key))
                    {
                        if (!grouping.Key)
                        {
                            sections = sections.Add(SwitchSection(GenerateLabels(grouping).ToSyntaxList(), ReturnStatement(TrueLiteralExpression())));
                        }
                        else
                        {
                            sections = sections.Add(SwitchSection(GenerateLabels(grouping).ToSyntaxList(), ReturnStatement(FalseLiteralExpression())));
                        }
                    }

                    Debug.Assert(sections.Any());

                    sections = sections.Add(DefaultSwitchSection(ThrowNewInvalidOperationException(ParseExpression(@"$""Unrecognized property '{propertySymbol.Name}'"""))));

                    yield return SwitchSection(
                        CaseSwitchLabel(StringLiteralExpression(typeSymbol.Name)),
                        Block(
                            SwitchStatement(
                                SimpleMemberAccessExpression(IdentifierName("propertySymbol"), IdentifierName("Name")),
                                sections)));
                }

                IEnumerable<SwitchLabelSyntax> GenerateLabels(IEnumerable<IPropertySymbol> propertySymbols)
                {
                    foreach (IPropertySymbol propertySymbol in propertySymbols)
                    {
                        yield return CaseSwitchLabel(StringLiteralExpression(propertySymbol.Name));
                    }
                }
            }
        }
    }
}
