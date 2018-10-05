// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CodeGeneration.CSharp.CSharpFactory2;
using static Roslynator.CodeGeneration.CSharp.MetadataNames2;
using static Roslynator.CodeGeneration.CSharp.Symbols;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public partial class CSharpSyntaxWalkerGenerator
    {
        private static readonly SymbolDisplayFormat _parameterSymbolDisplayFormat = SymbolDisplayFormats.Default.WithParameterOptions(
            SymbolDisplayParameterOptions.IncludeDefaultValue
                | SymbolDisplayParameterOptions.IncludeExtensionThis
                | SymbolDisplayParameterOptions.IncludeName
                | SymbolDisplayParameterOptions.IncludeParamsRefOut
                | SymbolDisplayParameterOptions.IncludeType);

        public CSharpSyntaxWalkerGenerator(
            SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node,
            bool useCustomVisitMethod = false,
            bool eliminateDefaultVisit = false)
        {
            Depth = depth;
            UseCustomVisitMethod = useCustomVisitMethod;
            EliminateDefaultVisit = eliminateDefaultVisit;
        }

        public SyntaxWalkerDepth Depth { get; }

        public bool UseCustomVisitMethod { get; }

        public bool EliminateDefaultVisit { get; }

        public List<MemberDeclarationSyntax> CreateMemberDeclarations()
        {
            var members = new List<MemberDeclarationSyntax>();

            AddIfNotNull(CreateConstructorDeclaration(Depth));
            AddIfNotNull(CreateShouldVisitPropertyDeclaration());
            AddIfNotNull(CreateVisitNodeMethodDeclaration());

            if (!EliminateDefaultVisit
                && Depth >= SyntaxWalkerDepth.Node)
            {
                AddIfNotNull(CreateVisitListMethodDeclaration());
                AddIfNotNull(CreateVisitSeparatedListMethodDeclaration());
            }

            if (Depth >= SyntaxWalkerDepth.Token)
                AddIfNotNull(CreateVisitTokenListMethodDeclaration());

            var context = new MethodGenerationContext();

            foreach (ISymbol symbol in VisitMethodSymbols)
            {
                context.MethodSymbol = (IMethodSymbol)symbol;

                AddIfNotNull(CreateVisitMethodDeclaration(context));

                context.MethodSymbol = null;
                context.Statements.Clear();
                context.LocalNames.Clear();
            }

            if (EliminateDefaultVisit)
            {
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_BaseTypeSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_CrefSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_ExpressionSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_InterpolatedStringContentSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_MemberCrefSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_PatternSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_QueryClauseSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_SelectOrGroupClauseSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_SimpleNameSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_StatementSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_SwitchLabelSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_TypeParameterConstraintSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_TypeSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_VariableDesignationSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_XmlAttributeSyntax));
                AddIfNotNull(CreateVisitAbstractSyntaxMethodDeclaration(Microsoft_CodeAnalysis_CSharp_Syntax_XmlNodeSyntax));
            }

            return members;

            void AddIfNotNull(MemberDeclarationSyntax memberDeclaration)
            {
                if (memberDeclaration != null)
                    members.Add(memberDeclaration);
            }
        }

        protected virtual MethodDeclarationSyntax CreateVisitMethodDeclaration(MethodGenerationContext context)
        {
            CreateMethodStatements(context);

            return MethodDeclaration(
                Modifiers.Public_Override(),
                VoidType(),
                Identifier(context.MethodName),
                ParseParameterList($"({context.ParameterSymbol.ToDisplayString(_parameterSymbolDisplayFormat)})"),
                Block(context.Statements));
        }

        protected virtual void CreateMethodStatements(MethodGenerationContext context)
        {
            foreach (IPropertySymbol propertySymbol in GetPropertySymbols(context.ParameterType))
            {
                if (ShouldVisit(propertySymbol))
                {
                    context.PropertySymbol = propertySymbol;

                    CreateVisitStatements(context);

                    context.PropertySymbol = null;
                }
            }
        }

        protected virtual void CreateVisitStatements(MethodGenerationContext context)
        {
            string parameterName = context.ParameterName;
            ITypeSymbol propertyType = context.PropertyType;
            string propertyName = context.PropertyName;

            if (propertyType.OriginalDefinition.Equals(SyntaxListSymbol))
            {
                if (UseCustomVisitMethod)
                {
                    CreateVisitListStatements(context, isSeparatedList: false);
                }
                else
                {
                    context.AddStatement(VisitStatement("VisitList", parameterName, propertyName));
                }
            }
            else if (propertyType.OriginalDefinition.Equals(SeparatedSyntaxListSymbol))
            {
                if (UseCustomVisitMethod)
                {
                    CreateVisitListStatements(context, isSeparatedList: true);
                }
                else
                {
                    context.AddStatement(VisitStatement("VisitSeparatedList", parameterName, propertyName));
                }
            }
            else if (propertyType.Equals(SyntaxTokenListSymbol))
            {
                if (Depth >= SyntaxWalkerDepth.Token)
                {
                    context.AddStatement(VisitStatement("VisitTokenList", parameterName, propertyName));
                }
            }
            else if (propertyType.Equals(SyntaxTokenSymbol))
            {
                if (Depth >= SyntaxWalkerDepth.Token)
                {
                    context.AddStatement(VisitStatement("VisitToken", parameterName, propertyName));
                }
            }
            else if (propertyType.EqualsOrInheritsFrom(SyntaxNodeSymbol))
            {
                switch (propertyType.Name)
                {
                    case "AccessorListSyntax":
                    case "ArgumentListSyntax":
                    case "ArrayTypeSyntax":
                    case "ArrowExpressionClauseSyntax":
                    case "AttributeArgumentListSyntax":
                    case "AttributeTargetSpecifierSyntax":
                    case "BaseListSyntax":
                    case "BlockSyntax":
                    case "BracketedArgumentListSyntax":
                    case "BracketedParameterListSyntax":
                    case "CatchDeclarationSyntax":
                    case "CatchFilterClauseSyntax":
                    case "ConstructorInitializerSyntax":
                    case "CrefBracketedParameterListSyntax":
                    case "CrefParameterListSyntax":
                    case "CrefSyntax":
                    case "ElseClauseSyntax":
                    case "EqualsValueClauseSyntax":
                    case "ExplicitInterfaceSpecifierSyntax":
                    case "ExpressionSyntax":
                    case "FinallyClauseSyntax":
                    case "FromClauseSyntax":
                    case "IdentifierNameSyntax":
                    case "InitializerExpressionSyntax":
                    case "InterpolationAlignmentClauseSyntax":
                    case "InterpolationFormatClauseSyntax":
                    case "JoinIntoClauseSyntax":
                    case "MemberCrefSyntax":
                    case "NameColonSyntax":
                    case "NameEqualsSyntax":
                    case "NameSyntax":
                    case "ParameterListSyntax":
                    case "ParameterSyntax":
                    case "PatternSyntax":
                    case "QueryBodySyntax":
                    case "QueryContinuationSyntax":
                    case "SelectOrGroupClauseSyntax":
                    case "SimpleNameSyntax":
                    case "StatementSyntax":
                    case "TypeArgumentListSyntax":
                    case "TypeParameterListSyntax":
                    case "TypeSyntax":
                    case "VariableDeclarationSyntax":
                    case "VariableDesignationSyntax":
                    case "WhenClauseSyntax":
                    case "XmlElementEndTagSyntax":
                    case "XmlElementStartTagSyntax":
                    case "XmlNameSyntax":
                    case "XmlPrefixSyntax":
                        {
                            if (UseCustomVisitMethod)
                            {
                                CreateTypeVisitStatements(context);
                            }
                            else
                            {
                                context.AddStatement(VisitStatement("Visit", parameterName, propertyName));
                            }

                            break;
                        }
                    case "CSharpSyntaxNode":
                        {
                            if (!UseCustomVisitMethod)
                            {
                                context.AddStatement(VisitStatement("Visit", parameterName, propertyName));
                                break;
                            }

                            if (EliminateDefaultVisit
                                && propertyName == "Body"
                                && context.ParameterType.InheritsFrom(Microsoft_CodeAnalysis_CSharp_Syntax_AnonymousFunctionExpressionSyntax))
                            {
                                CreateVisitAnonymousFunctionStatements(context);
                            }
                            else
                            {
                                CreateTypeVisitStatements(context);
                            }

                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException($"Unrecognized property type '{propertyType.ToDisplayString()}'.");
                        }
                }
            }
            else if (!propertyType.SpecialType.Is(SpecialType.System_Int32, SpecialType.System_Boolean))
            {
                throw new InvalidOperationException();
            }
        }

        private static void CreateVisitAnonymousFunctionStatements(MethodGenerationContext context)
        {
            string variableName = context.CreateVariableName(context.PropertyName);

            context.AddStatement(LocalDeclarationStatement(context.PropertyType, variableName, context.ParameterName, context.PropertyName));

            IfStatementSyntax ifStatement = IfStatement(
                IsPatternExpression(
                    IdentifierName(variableName),
                    DeclarationPattern(IdentifierName("ExpressionSyntax"), SingleVariableDesignation(Identifier("expression")))),
                Block(VisitStatement("VisitExpression", "expression")),
                ElseClause(
                    IfStatement(
                        IsPatternExpression(
                            IdentifierName(variableName),
                            DeclarationPattern(IdentifierName("StatementSyntax"), SingleVariableDesignation(Identifier("statement")))),
                        Block(VisitStatement("VisitStatement", "statement")),
                        ElseClause(Block(VisitStatement("Visit", variableName))))));

            context.AddStatement(ifStatement);
        }

        protected virtual void CreateVisitListStatements(MethodGenerationContext context, bool isSeparatedList)
        {
            ITypeSymbol typeSymbol = ((INamedTypeSymbol)context.PropertyType).TypeArguments.Single();

            IMethodSymbol methodSymbol = FindVisitMethod(typeSymbol);

            string methodName = null;

            if (methodSymbol != null)
            {
                methodName = methodSymbol.Name;
            }
            else if (EliminateDefaultVisit)
            {
                methodName = GetMethodName(typeSymbol);
            }

            if (methodName != null)
            {
                string typeName = typeSymbol.Name;

                string variableName = context.CreateVariableName(typeName.Remove(typeName.Length - 6));

                ForEachStatementSyntax forEachStatement = ForEachVisitStatement(
                    typeName,
                    variableName,
                    SimpleMemberAccessExpression(IdentifierName(context.ParameterName), IdentifierName(context.PropertyName)),
                    VisitStatement(methodName, variableName),
                    checkShouldVisit: true);

                context.AddStatement(forEachStatement);
            }
            else
            {
                methodName = (isSeparatedList) ? "VisitSeparatedList" : "VisitList";

                context.AddStatement(VisitStatement(methodName, context.ParameterName, context.PropertyName));
            }
        }

        protected virtual void CreateTypeVisitStatements(MethodGenerationContext context)
        {
            ITypeSymbol propertyType = context.PropertyType;
            string propertyName = context.PropertyName;
            string parameterName = context.ParameterName;

            IMethodSymbol methodSymbol = FindVisitMethod(propertyType);

            if (methodSymbol == null)
            {
                if (EliminateDefaultVisit)
                {
                    context.AddStatement(IfNotShouldVisitReturnStatement());

                    string variableName = context.CreateVariableName(propertyName);

                    context.AddStatement(LocalDeclarationStatement(propertyType, variableName, parameterName, propertyName));

                    string methodName = GetMethodName(propertyType);

                    IfStatementSyntax ifStatement = IfNotEqualsToNullStatement(
                        variableName,
                        VisitStatement(methodName, variableName));

                    context.AddStatement(ifStatement);
                }
                else
                {
                    context.AddStatement(VisitStatement("Visit", parameterName, propertyName));
                }
            }
            else
            {
                string variableName = context.CreateVariableName(propertyName);

                context.AddStatement(IfNotShouldVisitReturnStatement());

                context.AddStatement(LocalDeclarationStatement(propertyType, variableName, parameterName, propertyName));

                context.AddStatement(IfNotEqualsToNullStatement(variableName, VisitStatement(methodSymbol.Name, variableName)));
            }
        }

        private void CreateVisitListSyntaxStatements(MethodGenerationContext context)
        {
            string variableName = context.CreateVariableName(context.PropertyName);

            context.AddStatement(LocalDeclarationStatement(context.PropertyType, variableName, context.ParameterName, context.PropertyName));

            IPropertySymbol listPropertySymbol = FindListPropertySymbol(context.PropertySymbol);

            ITypeSymbol typeSymbol = ((INamedTypeSymbol)listPropertySymbol.Type).TypeArguments.Single();

            IMethodSymbol methodSymbol = FindVisitMethod(typeSymbol);

            string methodName = null;

            if (methodSymbol != null)
            {
                methodName = methodSymbol.Name;
            }
            else if (EliminateDefaultVisit)
            {
                methodName = GetMethodName(typeSymbol);
            }

            StatementSyntax statement;

            if (methodName != null)
            {
                string forEachVariableName = context.CreateVariableName(typeSymbol.Name.Remove(typeSymbol.Name.Length - 6));

                statement = ForEachVisitStatement(
                    typeSymbol.Name,
                    forEachVariableName,
                    SimpleMemberAccessExpression(
                        IdentifierName(variableName),
                        IdentifierName(listPropertySymbol.Name)),
                    VisitStatement(methodName, forEachVariableName), checkShouldVisit: true);
            }
            else
            {
                methodName = (listPropertySymbol.Type.OriginalDefinition.Equals(SyntaxListSymbol)) ? "VisitList" : "VisitSeparatedList";

                statement = VisitStatement(methodName, variableName, listPropertySymbol.Name);
            }

            context.AddStatement(IfNotEqualsToNullStatement(variableName, statement));
        }

        private static string GetMethodName(ITypeSymbol typeSymbol)
        {
            do
            {
                string name = typeSymbol.MetadataName;

                switch (name)
                {
                    case "BaseTypeSyntax":
                    case "CrefSyntax":
                    case "ExpressionSyntax":
                    case "InterpolatedStringContentSyntax":
                    case "MemberCrefSyntax":
                    case "MemberDeclarationSyntax":
                    case "PatternSyntax":
                    case "QueryClauseSyntax":
                    case "SelectOrGroupClauseSyntax":
                    case "SimpleNameSyntax":
                    case "StatementSyntax":
                    case "SwitchLabelSyntax":
                    case "TypeSyntax":
                    case "TypeParameterConstraintSyntax":
                    case "VariableDesignationSyntax":
                    case "XmlAttributeSyntax":
                    case "XmlNodeSyntax":
                        {
                            if (typeSymbol
                                .ContainingNamespace
                                .HasMetadataName(Microsoft_CodeAnalysis_CSharp_Syntax))
                            {
                                return "Visit" + name.Remove(name.Length - 6);
                            }

                            break;
                        }
                }

                typeSymbol = typeSymbol.BaseType;

            } while (typeSymbol != null);

            throw new ArgumentException("", nameof(typeSymbol));
        }

        public virtual ConstructorDeclarationSyntax CreateConstructorDeclaration(SyntaxWalkerDepth depth)
        {
            return ConstructorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                Modifiers.Protected(),
                Identifier("CSharpSyntaxNodeWalker"),
                ParameterList(),
                BaseConstructorInitializer(
                    ArgumentList(
                        Argument(
                            NameColon("depth"),
                            SimpleMemberAccessExpression(IdentifierName("SyntaxWalkerDepth"), IdentifierName(depth.ToString()))))),
                Block());
        }

        public virtual PropertyDeclarationSyntax CreateShouldVisitPropertyDeclaration()
        {
            return PropertyDeclaration(
                Modifiers.Protected_Virtual(),
                PredefinedBoolType(),
                Identifier("ShouldVisit"),
                AccessorList(GetAccessorDeclaration(Block(ReturnStatement(TrueLiteralExpression())))));
        }

        public virtual MethodDeclarationSyntax CreateVisitNodeMethodDeclaration()
        {
            return MethodDeclaration(
                Modifiers.Public_Override(),
                VoidType(),
                Identifier("Visit"),
                ParameterList(Parameter(IdentifierName("SyntaxNode"), "node")),
                Block(
                    IfNotShouldVisitReturnStatement(),
                    ExpressionStatement(
                        SimpleMemberInvocationExpression(
                            BaseExpression(),
                            IdentifierName("Visit"),
                            ArgumentList(Argument(IdentifierName("node")))))));
        }

        public virtual MethodDeclarationSyntax CreateVisitListMethodDeclaration()
        {
            return MethodDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                Modifiers.Private(),
                VoidType(),
                default(ExplicitInterfaceSpecifierSyntax),
                Identifier("VisitList"),
                TypeParameterList(TypeParameter("TNode")),
                ParameterList(Parameter(GenericName("SyntaxList", IdentifierName("TNode")), "list")),
                SingletonList(TypeParameterConstraintClause("TNode", TypeConstraint(IdentifierName("SyntaxNode")))),
                Block(ForEachVisitStatement(
                    "TNode",
                    "node",
                    IdentifierName("list"),
                    VisitStatement("Visit", "node"),
                    checkShouldVisit: true)),
                default(ArrowExpressionClauseSyntax));
        }

        public virtual MethodDeclarationSyntax CreateVisitSeparatedListMethodDeclaration()
        {
            return MethodDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                Modifiers.Private(),
                VoidType(),
                default(ExplicitInterfaceSpecifierSyntax),
                Identifier("VisitSeparatedList"),
                TypeParameterList(TypeParameter("TNode")),
                ParameterList(Parameter(GenericName("SeparatedSyntaxList", IdentifierName("TNode")), "list")),
                SingletonList(TypeParameterConstraintClause("TNode", TypeConstraint(IdentifierName("SyntaxNode")))),
                Block(ForEachVisitStatement("TNode", "node", IdentifierName("list"),
                    VisitStatement("Visit", "node"), checkShouldVisit: true)),
                default(ArrowExpressionClauseSyntax));
        }

        public virtual MethodDeclarationSyntax CreateVisitTokenListMethodDeclaration()
        {
            return MethodDeclaration(
                Modifiers.Private(),
                VoidType(),
                Identifier("VisitTokenList"),
                ParameterList(Parameter(IdentifierName("SyntaxTokenList"), "list")),
                Block(
                    IfStatement(
                        SimpleMemberInvocationExpression(IdentifierName("list"), IdentifierName("Any")),
                        Block(
                            ForEachVisitStatement(
                                "SyntaxToken",
                                "token",
                                IdentifierName("list"),
                                VisitStatement("VisitToken", "token"), checkShouldVisit: true)))));
        }

        internal virtual MethodDeclarationSyntax CreateVisitAbstractSyntaxMethodDeclaration(MetadataName metadataName)
        {
            string name = metadataName.Name;

            return MethodDeclaration(
                Modifiers.Protected_Virtual(),
                VoidType(),
                Identifier($"Visit{name.Remove(name.Length - 6)}"),
                ParameterList(Parameter(IdentifierName(name), "node")),
                Block(
                    SwitchStatement(
                        SimpleMemberInvocationExpression(IdentifierName("node"), IdentifierName("Kind")),
                        CreateSections().ToSyntaxList())));

            IEnumerable<SwitchSectionSyntax> CreateSections()
            {
                foreach (INamedTypeSymbol typeSymbol2 in SyntaxSymbols.Where(f => !f.IsAbstract && f.InheritsFrom(metadataName)))
                {
                    string name2 = typeSymbol2.Name;

                    SyntaxList<SwitchLabelSyntax> labels = GetKinds(typeSymbol2)
                        .Select(f => CaseSwitchLabel(SimpleMemberAccessExpression(IdentifierName("SyntaxKind"), IdentifierName(f.ToString()))))
                        .ToSyntaxList<SwitchLabelSyntax>();

                    yield return SwitchSection(
                        labels,
                        List(new StatementSyntax[]
                        {
                            ExpressionStatement(
                                InvocationExpression(
                                    IdentifierName("Visit" + name2.Remove(name2.Length - 6)),
                                    ArgumentList(Argument(CastExpression(IdentifierName(name2), IdentifierName("node")))))),
                            BreakStatement()
                        }));
                }

                yield return DefaultSwitchSection(
                    List(new StatementSyntax[]
                    {
                        ExpressionStatement(
                            SimpleMemberInvocationExpression(
                                IdentifierName("Debug"),
                                IdentifierName("Fail"),
                                ArgumentList(
                                    Argument(
                                        ParseExpression(@"$""Unrecognized kind '{node.Kind()}'."""))))),
                        ExpressionStatement(
                            SimpleMemberInvocationExpression(
                                BaseExpression(),
                                IdentifierName("Visit"),
                                ArgumentList(Argument(IdentifierName("node"))))),
                        BreakStatement()
                    }));
            }
        }
    }
}
