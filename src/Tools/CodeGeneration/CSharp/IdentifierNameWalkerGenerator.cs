// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CodeGeneration.CSharp.Symbols;
using static Roslynator.CodeGeneration.CSharp.CSharpFactory2;

namespace Roslynator.CodeGeneration.CSharp
{
    public class IdentifierNameWalkerGenerator : CSharpSyntaxWalkerGenerator
    {
        public IdentifierNameWalkerGenerator(
            SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node,
            bool useCustomVisitMethod = false,
            bool eliminateDefaultVisit = false) : base(depth, useCustomVisitMethod, eliminateDefaultVisit)
        {
        }

        public static CompilationUnitSyntax Generate()
        {
            var generator = new IdentifierNameWalkerGenerator(
                depth: SyntaxWalkerDepth.Node,
                useCustomVisitMethod: true,
                eliminateDefaultVisit: true);

            return CompilationUnit(
                UsingDirectives(
                    "System",
                    "Microsoft.CodeAnalysis",
                    "Microsoft.CodeAnalysis.CSharp",
                    "Microsoft.CodeAnalysis.CSharp.Syntax"),
                NamespaceDeclaration("Roslynator.CSharp.SyntaxWalkers",
                    ClassDeclaration(
                        default(SyntaxList<AttributeListSyntax>),
                        Modifiers.Public_Abstract(),
                        Identifier("IdentifierNameWalker"),
                        default(TypeParameterListSyntax),
                        default(BaseListSyntax),
                        default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                        generator.CreateMemberDeclarations().ToSyntaxList())));
        }

        public override ConstructorDeclarationSyntax CreateConstructorDeclaration(SyntaxWalkerDepth depth)
        {
            return ConstructorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                Modifiers.Protected(),
                Identifier("IdentifierNameWalker"),
                ParameterList(),
                default,
                Block());
        }

        protected override MethodDeclarationSyntax CreateVisitMethodDeclaration(MethodGenerationContext context)
        {
            MethodDeclarationSyntax methodDeclaration = base.CreateVisitMethodDeclaration(context);

            if (context.ParameterType.Name == "IdentifierNameSyntax")
            {
                return methodDeclaration.WithModifiers(Modifiers.Public_Virtual());
            }
            else
            {
                return methodDeclaration.WithModifiers(Modifiers.Public());
            }
        }

        internal override MethodDeclarationSyntax CreateVisitAbstractSyntaxMethodDeclaration(MetadataName metadataName)
        {
            MethodDeclarationSyntax methodDeclaration = base.CreateVisitAbstractSyntaxMethodDeclaration(metadataName);

            if (metadataName != MetadataNames2.Microsoft_CodeAnalysis_CSharp_Syntax_BaseTypeSyntax)
            {
                methodDeclaration = methodDeclaration.WithModifiers(Modifiers.Private());
            }

            return methodDeclaration;
        }

        public override MethodDeclarationSyntax CreateVisitNodeMethodDeclaration()
        {
            SwitchStatementSyntax switchStatement = SwitchStatement(
                SimpleMemberInvocationExpression(IdentifierName("node"), IdentifierName("Kind")),
                CreateSections().ToSyntaxList());

            return MethodDeclaration(
                Modifiers.Public(),
                VoidType(),
                Identifier("Visit"),
                ParameterList(Parameter(IdentifierName("SyntaxNode"), "node")),
                Block(switchStatement));

            IEnumerable<SwitchSectionSyntax> CreateSections()
            {
                foreach (INamedTypeSymbol typeSymbol in SyntaxSymbols.Where(f => !f.IsAbstract))
                {
                    string name = typeSymbol.Name;

                    SyntaxList<SwitchLabelSyntax> labels = GetKinds(typeSymbol)
                        .Select(f => CaseSwitchLabel(SimpleMemberAccessExpression(IdentifierName("SyntaxKind"), IdentifierName(f.ToString()))))
                        .ToSyntaxList<SwitchLabelSyntax>();

                    yield return SwitchSection(
                        labels,
                        List(new StatementSyntax[]
                        {
                            ExpressionStatement(
                                InvocationExpression(
                                    IdentifierName("Visit" + name.Remove(name.Length - 6)),
                                    ArgumentList(Argument(CastExpression(IdentifierName(name), IdentifierName("node")))))),

                            BreakStatement()
                        }));
                }

                yield return DefaultSwitchSection(ThrowNewArgumentException(ParseExpression(@"$""Unrecognized node '{node.Kind()}'."""), "node"));
            }
        }
    }
}
