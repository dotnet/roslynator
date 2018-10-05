// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeGeneration.CSharp
{
    public static class SymbolsGetKindsGenerator
    {
        public static CompilationUnitSyntax Generate()
        {
            return CompilationUnit(
                UsingDirectives(
                    "System.Collections.Generic",
                    "Microsoft.CodeAnalysis",
                    "Microsoft.CodeAnalysis.CSharp"),
                NamespaceDeclaration("Roslynator.CodeGeneration.CSharp",
                    ClassDeclaration(
                        default(SyntaxList<AttributeListSyntax>),
                        Modifiers.Internal_Static_Partial(),
                        Identifier("Symbols"),
                        default(TypeParameterListSyntax),
                        default(BaseListSyntax),
                        default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                        SingletonList<MemberDeclarationSyntax>(GenerateMethodDeclaration()))));
        }

        private static MethodDeclarationSyntax GenerateMethodDeclaration()
        {
            var enumNames = new HashSet<string>();

            foreach (string enumName in Symbols
                .Compilation
                .GetTypeByMetadataName("Microsoft.CodeAnalysis.CSharp.SyntaxKind")
                .GetMembers()
                .Where(f => f.DeclaredAccessibility == Accessibility.Public)
                .Select(f => f.Name)
                .Where(f => f != "None" && !f.EndsWith("Keyword") && !f.EndsWith("Token"))
                .OrderBy(f => f))
            {
                enumNames.Add(enumName);
            }

            MethodDeclarationSyntax methodDeclaration = MethodDeclaration(
                Modifiers.Public_Static(),
                ParseTypeName("IEnumerable<SyntaxKind>"),
                Identifier("GetKinds"),
                ParameterList(Parameter(IdentifierName("INamedTypeSymbol"), "syntaxSymbol")),
                Block(
                    SwitchStatement(
                        SimpleMemberAccessExpression(IdentifierName("syntaxSymbol"), IdentifierName("Name")),
                        GenerateSections().ToSyntaxList())));

            foreach (string enumName in enumNames)
            {
                switch (enumName)
                {
                    case "ConflictMarkerTrivia":
                    case "DisabledTextTrivia":
                    case "DocumentationCommentExteriorTrivia":
                    case "EndOfLineTrivia":
                    case "MultiLineCommentTrivia":
                    case "PreprocessingMessageTrivia":
                    case "SingleLineCommentTrivia":
                    case "WhitespaceTrivia":
                    case "value__":
                    case "List":
                        break;
                    default:
                        throw new InvalidOperationException($"Unrecognized enum value '{enumName}'.");
                }
            }

            return methodDeclaration;

            IEnumerable<SwitchSectionSyntax> GenerateSections()
            {
                foreach (INamedTypeSymbol symbol in Symbols.SyntaxSymbols)
                {
                    if (symbol.IsAbstract)
                        continue;

                    string name = symbol.Name;

                    SyntaxList<StatementSyntax> statements = default;

                    foreach (SyntaxKind kind in GetSyntaxKinds(symbol))
                    {
                        statements = statements.Add(
                            YieldReturnStatement(
                                SimpleMemberAccessExpression(
                                    IdentifierName("SyntaxKind"),
                                    IdentifierName(kind.ToString()))));

                        enumNames.Remove(kind.ToString());
                    }

                    statements = statements.Add(BreakStatement());

                    yield return SwitchSection(
                        CaseSwitchLabel(StringLiteralExpression(name)),
                        Block(statements));
                }

                IEnumerable<SyntaxKind> GetSyntaxKinds(INamedTypeSymbol syntaxSymbol)
                {
                    switch (syntaxSymbol.Name)
                    {
                        case "AccessorDeclarationSyntax":
                            {
                                yield return SyntaxKind.GetAccessorDeclaration;
                                yield return SyntaxKind.SetAccessorDeclaration;
                                yield return SyntaxKind.AddAccessorDeclaration;
                                yield return SyntaxKind.RemoveAccessorDeclaration;
                                yield return SyntaxKind.UnknownAccessorDeclaration;
                                break;
                            }

                        case "AccessorListSyntax":
                            {
                                yield return SyntaxKind.AccessorList;
                                break;
                            }

                        case "AliasQualifiedNameSyntax":
                            {
                                yield return SyntaxKind.AliasQualifiedName;
                                break;
                            }

                        case "AnonymousMethodExpressionSyntax":
                            {
                                yield return SyntaxKind.AnonymousMethodExpression;
                                break;
                            }

                        case "AnonymousObjectCreationExpressionSyntax":
                            {
                                yield return SyntaxKind.AnonymousObjectCreationExpression;
                                break;
                            }

                        case "AnonymousObjectMemberDeclaratorSyntax":
                            {
                                yield return SyntaxKind.AnonymousObjectMemberDeclarator;
                                break;
                            }

                        case "ArgumentListSyntax":
                            {
                                yield return SyntaxKind.ArgumentList;
                                break;
                            }

                        case "ArgumentSyntax":
                            {
                                yield return SyntaxKind.Argument;
                                break;
                            }

                        case "ArrayCreationExpressionSyntax":
                            {
                                yield return SyntaxKind.ArrayCreationExpression;
                                break;
                            }

                        case "ArrayRankSpecifierSyntax":
                            {
                                yield return SyntaxKind.ArrayRankSpecifier;
                                break;
                            }

                        case "ArrayTypeSyntax":
                            {
                                yield return SyntaxKind.ArrayType;
                                break;
                            }

                        case "ArrowExpressionClauseSyntax":
                            {
                                yield return SyntaxKind.ArrowExpressionClause;
                                break;
                            }

                        case "AssignmentExpressionSyntax":
                            {
                                yield return SyntaxKind.SimpleAssignmentExpression;
                                yield return SyntaxKind.AddAssignmentExpression;
                                yield return SyntaxKind.SubtractAssignmentExpression;
                                yield return SyntaxKind.MultiplyAssignmentExpression;
                                yield return SyntaxKind.DivideAssignmentExpression;
                                yield return SyntaxKind.ModuloAssignmentExpression;
                                yield return SyntaxKind.AndAssignmentExpression;
                                yield return SyntaxKind.ExclusiveOrAssignmentExpression;
                                yield return SyntaxKind.OrAssignmentExpression;
                                yield return SyntaxKind.LeftShiftAssignmentExpression;
                                yield return SyntaxKind.RightShiftAssignmentExpression;
                                break;
                            }

                        case "AttributeArgumentListSyntax":
                            {
                                yield return SyntaxKind.AttributeArgumentList;
                                break;
                            }

                        case "AttributeArgumentSyntax":
                            {
                                yield return SyntaxKind.AttributeArgument;
                                break;
                            }

                        case "AttributeListSyntax":
                            {
                                yield return SyntaxKind.AttributeList;
                                break;
                            }

                        case "AttributeSyntax":
                            {
                                yield return SyntaxKind.Attribute;
                                break;
                            }

                        case "AttributeTargetSpecifierSyntax":
                            {
                                yield return SyntaxKind.AttributeTargetSpecifier;
                                break;
                            }

                        case "AwaitExpressionSyntax":
                            {
                                yield return SyntaxKind.AwaitExpression;
                                break;
                            }

                        case "BadDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.BadDirectiveTrivia;
                                break;
                            }

                        case "BaseExpressionSyntax":
                            {
                                yield return SyntaxKind.BaseExpression;
                                break;
                            }

                        case "BaseListSyntax":
                            {
                                yield return SyntaxKind.BaseList;
                                break;
                            }

                        case "BinaryExpressionSyntax":
                            {
                                yield return SyntaxKind.AddExpression;
                                yield return SyntaxKind.SubtractExpression;
                                yield return SyntaxKind.MultiplyExpression;
                                yield return SyntaxKind.DivideExpression;
                                yield return SyntaxKind.ModuloExpression;
                                yield return SyntaxKind.LeftShiftExpression;
                                yield return SyntaxKind.RightShiftExpression;
                                yield return SyntaxKind.LogicalOrExpression;
                                yield return SyntaxKind.LogicalAndExpression;
                                yield return SyntaxKind.BitwiseOrExpression;
                                yield return SyntaxKind.BitwiseAndExpression;
                                yield return SyntaxKind.ExclusiveOrExpression;
                                yield return SyntaxKind.EqualsExpression;
                                yield return SyntaxKind.NotEqualsExpression;
                                yield return SyntaxKind.LessThanExpression;
                                yield return SyntaxKind.LessThanOrEqualExpression;
                                yield return SyntaxKind.GreaterThanExpression;
                                yield return SyntaxKind.GreaterThanOrEqualExpression;
                                yield return SyntaxKind.IsExpression;
                                yield return SyntaxKind.AsExpression;
                                yield return SyntaxKind.CoalesceExpression;
                                break;
                            }

                        case "BlockSyntax":
                            {
                                yield return SyntaxKind.Block;
                                break;
                            }

                        case "BracketedArgumentListSyntax":
                            {
                                yield return SyntaxKind.BracketedArgumentList;
                                break;
                            }

                        case "BracketedParameterListSyntax":
                            {
                                yield return SyntaxKind.BracketedParameterList;
                                break;
                            }

                        case "BreakStatementSyntax":
                            {
                                yield return SyntaxKind.BreakStatement;
                                break;
                            }

                        case "CasePatternSwitchLabelSyntax":
                            {
                                yield return SyntaxKind.CasePatternSwitchLabel;
                                break;
                            }

                        case "CaseSwitchLabelSyntax":
                            {
                                yield return SyntaxKind.CaseSwitchLabel;
                                break;
                            }

                        case "CastExpressionSyntax":
                            {
                                yield return SyntaxKind.CastExpression;
                                break;
                            }

                        case "CatchClauseSyntax":
                            {
                                yield return SyntaxKind.CatchClause;
                                break;
                            }

                        case "CatchDeclarationSyntax":
                            {
                                yield return SyntaxKind.CatchDeclaration;
                                break;
                            }

                        case "CatchFilterClauseSyntax":
                            {
                                yield return SyntaxKind.CatchFilterClause;
                                break;
                            }

                        case "ClassDeclarationSyntax":
                            {
                                yield return SyntaxKind.ClassDeclaration;
                                break;
                            }

                        case "ClassOrStructConstraintSyntax":
                            {
                                yield return SyntaxKind.ClassConstraint;
                                yield return SyntaxKind.StructConstraint;
                                break;
                            }

                        case "CompilationUnitSyntax":
                            {
                                yield return SyntaxKind.CompilationUnit;
                                break;
                            }

                        case "ConditionalAccessExpressionSyntax":
                            {
                                yield return SyntaxKind.ConditionalAccessExpression;
                                break;
                            }

                        case "ConditionalExpressionSyntax":
                            {
                                yield return SyntaxKind.ConditionalExpression;
                                break;
                            }

                        case "ConstantPatternSyntax":
                            {
                                yield return SyntaxKind.ConstantPattern;
                                break;
                            }

                        case "ConstructorConstraintSyntax":
                            {
                                yield return SyntaxKind.ConstructorConstraint;
                                break;
                            }

                        case "ConstructorDeclarationSyntax":
                            {
                                yield return SyntaxKind.ConstructorDeclaration;
                                break;
                            }

                        case "ConstructorInitializerSyntax":
                            {
                                yield return SyntaxKind.BaseConstructorInitializer;
                                yield return SyntaxKind.ThisConstructorInitializer;
                                break;
                            }

                        case "ContinueStatementSyntax":
                            {
                                yield return SyntaxKind.ContinueStatement;
                                break;
                            }

                        case "ConversionOperatorDeclarationSyntax":
                            {
                                yield return SyntaxKind.ConversionOperatorDeclaration;
                                break;
                            }

                        case "ConversionOperatorMemberCrefSyntax":
                            {
                                yield return SyntaxKind.ConversionOperatorMemberCref;
                                break;
                            }

                        case "CrefBracketedParameterListSyntax":
                            {
                                yield return SyntaxKind.CrefBracketedParameterList;
                                break;
                            }

                        case "CrefParameterListSyntax":
                            {
                                yield return SyntaxKind.CrefParameterList;
                                break;
                            }

                        case "CrefParameterSyntax":
                            {
                                yield return SyntaxKind.CrefParameter;
                                break;
                            }

                        case "DeclarationExpressionSyntax":
                            {
                                yield return SyntaxKind.DeclarationExpression;
                                break;
                            }

                        case "DeclarationPatternSyntax":
                            {
                                yield return SyntaxKind.DeclarationPattern;
                                break;
                            }

                        case "DefaultExpressionSyntax":
                            {
                                yield return SyntaxKind.DefaultExpression;
                                break;
                            }

                        case "DefaultSwitchLabelSyntax":
                            {
                                yield return SyntaxKind.DefaultSwitchLabel;
                                break;
                            }

                        case "DefineDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.DefineDirectiveTrivia;
                                break;
                            }

                        case "DelegateDeclarationSyntax":
                            {
                                yield return SyntaxKind.DelegateDeclaration;
                                break;
                            }

                        case "DestructorDeclarationSyntax":
                            {
                                yield return SyntaxKind.DestructorDeclaration;
                                break;
                            }

                        case "DiscardDesignationSyntax":
                            {
                                yield return SyntaxKind.DiscardDesignation;
                                break;
                            }

                        case "DocumentationCommentTriviaSyntax":
                            {
                                yield return SyntaxKind.SingleLineDocumentationCommentTrivia;
                                yield return SyntaxKind.MultiLineDocumentationCommentTrivia;
                                break;
                            }

                        case "DoStatementSyntax":
                            {
                                yield return SyntaxKind.DoStatement;
                                break;
                            }

                        case "ElementAccessExpressionSyntax":
                            {
                                yield return SyntaxKind.ElementAccessExpression;
                                break;
                            }

                        case "ElementBindingExpressionSyntax":
                            {
                                yield return SyntaxKind.ElementBindingExpression;
                                break;
                            }

                        case "ElifDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.ElifDirectiveTrivia;
                                break;
                            }

                        case "ElseClauseSyntax":
                            {
                                yield return SyntaxKind.ElseClause;
                                break;
                            }

                        case "ElseDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.ElseDirectiveTrivia;
                                break;
                            }

                        case "EmptyStatementSyntax":
                            {
                                yield return SyntaxKind.EmptyStatement;
                                break;
                            }

                        case "EndIfDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.EndIfDirectiveTrivia;
                                break;
                            }

                        case "EndRegionDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.EndRegionDirectiveTrivia;
                                break;
                            }

                        case "EnumDeclarationSyntax":
                            {
                                yield return SyntaxKind.EnumDeclaration;
                                break;
                            }

                        case "EnumMemberDeclarationSyntax":
                            {
                                yield return SyntaxKind.EnumMemberDeclaration;
                                break;
                            }

                        case "EqualsValueClauseSyntax":
                            {
                                yield return SyntaxKind.EqualsValueClause;
                                break;
                            }

                        case "ErrorDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.ErrorDirectiveTrivia;
                                break;
                            }

                        case "EventDeclarationSyntax":
                            {
                                yield return SyntaxKind.EventDeclaration;
                                break;
                            }

                        case "EventFieldDeclarationSyntax":
                            {
                                yield return SyntaxKind.EventFieldDeclaration;
                                break;
                            }

                        case "ExplicitInterfaceSpecifierSyntax":
                            {
                                yield return SyntaxKind.ExplicitInterfaceSpecifier;
                                break;
                            }

                        case "ExpressionStatementSyntax":
                            {
                                yield return SyntaxKind.ExpressionStatement;
                                break;
                            }

                        case "ExternAliasDirectiveSyntax":
                            {
                                yield return SyntaxKind.ExternAliasDirective;
                                break;
                            }

                        case "FieldDeclarationSyntax":
                            {
                                yield return SyntaxKind.FieldDeclaration;
                                break;
                            }

                        case "FinallyClauseSyntax":
                            {
                                yield return SyntaxKind.FinallyClause;
                                break;
                            }

                        case "FixedStatementSyntax":
                            {
                                yield return SyntaxKind.FixedStatement;
                                break;
                            }

                        case "ForEachStatementSyntax":
                            {
                                yield return SyntaxKind.ForEachStatement;
                                break;
                            }

                        case "ForEachVariableStatementSyntax":
                            {
                                yield return SyntaxKind.ForEachVariableStatement;
                                break;
                            }

                        case "ForStatementSyntax":
                            {
                                yield return SyntaxKind.ForStatement;
                                break;
                            }

                        case "FromClauseSyntax":
                            {
                                yield return SyntaxKind.FromClause;
                                break;
                            }

                        case "GenericNameSyntax":
                            {
                                yield return SyntaxKind.GenericName;
                                break;
                            }

                        case "GlobalStatementSyntax":
                            {
                                yield return SyntaxKind.GlobalStatement;
                                break;
                            }

                        case "GotoStatementSyntax":
                            {
                                yield return SyntaxKind.GotoStatement;
                                yield return SyntaxKind.GotoCaseStatement;
                                yield return SyntaxKind.GotoDefaultStatement;
                                break;
                            }

                        case "GroupClauseSyntax":
                            {
                                yield return SyntaxKind.GroupClause;
                                break;
                            }

                        case "CheckedExpressionSyntax":
                            {
                                yield return SyntaxKind.CheckedExpression;
                                yield return SyntaxKind.UncheckedExpression;
                                break;
                            }

                        case "CheckedStatementSyntax":
                            {
                                yield return SyntaxKind.CheckedStatement;
                                yield return SyntaxKind.UncheckedStatement;
                                break;
                            }

                        case "IdentifierNameSyntax":
                            {
                                yield return SyntaxKind.IdentifierName;
                                break;
                            }

                        case "IfDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.IfDirectiveTrivia;
                                break;
                            }

                        case "IfStatementSyntax":
                            {
                                yield return SyntaxKind.IfStatement;
                                break;
                            }

                        case "ImplicitArrayCreationExpressionSyntax":
                            {
                                yield return SyntaxKind.ImplicitArrayCreationExpression;
                                break;
                            }

                        case "ImplicitElementAccessSyntax":
                            {
                                yield return SyntaxKind.ImplicitElementAccess;
                                break;
                            }

                        case "IncompleteMemberSyntax":
                            {
                                yield return SyntaxKind.IncompleteMember;
                                break;
                            }

                        case "IndexerDeclarationSyntax":
                            {
                                yield return SyntaxKind.IndexerDeclaration;
                                break;
                            }

                        case "IndexerMemberCrefSyntax":
                            {
                                yield return SyntaxKind.IndexerMemberCref;
                                break;
                            }

                        case "InitializerExpressionSyntax":
                            {
                                yield return SyntaxKind.ArrayInitializerExpression;
                                yield return SyntaxKind.CollectionInitializerExpression;
                                yield return SyntaxKind.ComplexElementInitializerExpression;
                                yield return SyntaxKind.ObjectInitializerExpression;
                                break;
                            }

                        case "InterfaceDeclarationSyntax":
                            {
                                yield return SyntaxKind.InterfaceDeclaration;
                                break;
                            }

                        case "InterpolatedStringExpressionSyntax":
                            {
                                yield return SyntaxKind.InterpolatedStringExpression;
                                break;
                            }

                        case "InterpolatedStringTextSyntax":
                            {
                                yield return SyntaxKind.InterpolatedStringText;
                                break;
                            }

                        case "InterpolationAlignmentClauseSyntax":
                            {
                                yield return SyntaxKind.InterpolationAlignmentClause;
                                break;
                            }

                        case "InterpolationFormatClauseSyntax":
                            {
                                yield return SyntaxKind.InterpolationFormatClause;
                                break;
                            }

                        case "InterpolationSyntax":
                            {
                                yield return SyntaxKind.Interpolation;
                                break;
                            }

                        case "InvocationExpressionSyntax":
                            {
                                yield return SyntaxKind.InvocationExpression;
                                break;
                            }

                        case "IsPatternExpressionSyntax":
                            {
                                yield return SyntaxKind.IsPatternExpression;
                                break;
                            }

                        case "JoinClauseSyntax":
                            {
                                yield return SyntaxKind.JoinClause;
                                break;
                            }

                        case "JoinIntoClauseSyntax":
                            {
                                yield return SyntaxKind.JoinIntoClause;
                                break;
                            }

                        case "LabeledStatementSyntax":
                            {
                                yield return SyntaxKind.LabeledStatement;
                                break;
                            }

                        case "LetClauseSyntax":
                            {
                                yield return SyntaxKind.LetClause;
                                break;
                            }

                        case "LineDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.LineDirectiveTrivia;
                                break;
                            }

                        case "LiteralExpressionSyntax":
                            {
                                yield return SyntaxKind.ArgListExpression;
                                yield return SyntaxKind.NumericLiteralExpression;
                                yield return SyntaxKind.StringLiteralExpression;
                                yield return SyntaxKind.CharacterLiteralExpression;
                                yield return SyntaxKind.TrueLiteralExpression;
                                yield return SyntaxKind.FalseLiteralExpression;
                                yield return SyntaxKind.NullLiteralExpression;
                                yield return SyntaxKind.DefaultLiteralExpression;
                                break;
                            }

                        case "LoadDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.LoadDirectiveTrivia;
                                break;
                            }

                        case "LocalDeclarationStatementSyntax":
                            {
                                yield return SyntaxKind.LocalDeclarationStatement;
                                break;
                            }

                        case "LocalFunctionStatementSyntax":
                            {
                                yield return SyntaxKind.LocalFunctionStatement;
                                break;
                            }

                        case "LockStatementSyntax":
                            {
                                yield return SyntaxKind.LockStatement;
                                break;
                            }

                        case "MakeRefExpressionSyntax":
                            {
                                yield return SyntaxKind.MakeRefExpression;
                                break;
                            }

                        case "MemberAccessExpressionSyntax":
                            {
                                yield return SyntaxKind.PointerMemberAccessExpression;
                                yield return SyntaxKind.SimpleMemberAccessExpression;
                                break;
                            }

                        case "MemberBindingExpressionSyntax":
                            {
                                yield return SyntaxKind.MemberBindingExpression;
                                break;
                            }

                        case "MethodDeclarationSyntax":
                            {
                                yield return SyntaxKind.MethodDeclaration;
                                break;
                            }

                        case "NameColonSyntax":
                            {
                                yield return SyntaxKind.NameColon;
                                break;
                            }

                        case "NameEqualsSyntax":
                            {
                                yield return SyntaxKind.NameEquals;
                                break;
                            }

                        case "NameMemberCrefSyntax":
                            {
                                yield return SyntaxKind.NameMemberCref;
                                break;
                            }

                        case "NamespaceDeclarationSyntax":
                            {
                                yield return SyntaxKind.NamespaceDeclaration;
                                break;
                            }

                        case "NullableTypeSyntax":
                            {
                                yield return SyntaxKind.NullableType;
                                break;
                            }

                        case "ObjectCreationExpressionSyntax":
                            {
                                yield return SyntaxKind.ObjectCreationExpression;
                                break;
                            }

                        case "OmittedArraySizeExpressionSyntax":
                            {
                                yield return SyntaxKind.OmittedArraySizeExpression;
                                break;
                            }

                        case "OmittedTypeArgumentSyntax":
                            {
                                yield return SyntaxKind.OmittedTypeArgument;
                                break;
                            }

                        case "OperatorDeclarationSyntax":
                            {
                                yield return SyntaxKind.OperatorDeclaration;
                                break;
                            }

                        case "OperatorMemberCrefSyntax":
                            {
                                yield return SyntaxKind.OperatorMemberCref;
                                break;
                            }

                        case "OrderByClauseSyntax":
                            {
                                yield return SyntaxKind.OrderByClause;
                                break;
                            }

                        case "OrderingSyntax":
                            {
                                yield return SyntaxKind.AscendingOrdering;
                                yield return SyntaxKind.DescendingOrdering;
                                break;
                            }

                        case "ParameterListSyntax":
                            {
                                yield return SyntaxKind.ParameterList;
                                break;
                            }

                        case "ParameterSyntax":
                            {
                                yield return SyntaxKind.Parameter;
                                break;
                            }

                        case "ParenthesizedExpressionSyntax":
                            {
                                yield return SyntaxKind.ParenthesizedExpression;
                                break;
                            }

                        case "ParenthesizedLambdaExpressionSyntax":
                            {
                                yield return SyntaxKind.ParenthesizedLambdaExpression;
                                break;
                            }

                        case "ParenthesizedVariableDesignationSyntax":
                            {
                                yield return SyntaxKind.ParenthesizedVariableDesignation;
                                break;
                            }

                        case "PointerTypeSyntax":
                            {
                                yield return SyntaxKind.PointerType;
                                break;
                            }

                        case "PostfixUnaryExpressionSyntax":
                            {
                                yield return SyntaxKind.PostDecrementExpression;
                                yield return SyntaxKind.PostIncrementExpression;
                                break;
                            }

                        case "PragmaChecksumDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.PragmaChecksumDirectiveTrivia;
                                break;
                            }

                        case "PragmaWarningDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.PragmaWarningDirectiveTrivia;
                                break;
                            }

                        case "PredefinedTypeSyntax":
                            {
                                yield return SyntaxKind.PredefinedType;
                                break;
                            }

                        case "PrefixUnaryExpressionSyntax":
                            {
                                yield return SyntaxKind.UnaryPlusExpression;
                                yield return SyntaxKind.UnaryMinusExpression;
                                yield return SyntaxKind.BitwiseNotExpression;
                                yield return SyntaxKind.LogicalNotExpression;
                                yield return SyntaxKind.PreIncrementExpression;
                                yield return SyntaxKind.PreDecrementExpression;
                                yield return SyntaxKind.AddressOfExpression;
                                yield return SyntaxKind.PointerIndirectionExpression;
                                break;
                            }

                        case "PropertyDeclarationSyntax":
                            {
                                yield return SyntaxKind.PropertyDeclaration;
                                break;
                            }

                        case "QualifiedCrefSyntax":
                            {
                                yield return SyntaxKind.QualifiedCref;
                                break;
                            }

                        case "QualifiedNameSyntax":
                            {
                                yield return SyntaxKind.QualifiedName;
                                break;
                            }

                        case "QueryBodySyntax":
                            {
                                yield return SyntaxKind.QueryBody;
                                break;
                            }

                        case "QueryContinuationSyntax":
                            {
                                yield return SyntaxKind.QueryContinuation;
                                break;
                            }

                        case "QueryExpressionSyntax":
                            {
                                yield return SyntaxKind.QueryExpression;
                                break;
                            }

                        case "ReferenceDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.ReferenceDirectiveTrivia;
                                break;
                            }

                        case "RefExpressionSyntax":
                            {
                                yield return SyntaxKind.RefExpression;
                                break;
                            }

                        case "RefTypeExpressionSyntax":
                            {
                                yield return SyntaxKind.RefTypeExpression;
                                break;
                            }

                        case "RefTypeSyntax":
                            {
                                yield return SyntaxKind.RefType;
                                break;
                            }

                        case "RefValueExpressionSyntax":
                            {
                                yield return SyntaxKind.RefValueExpression;
                                break;
                            }

                        case "RegionDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.RegionDirectiveTrivia;
                                break;
                            }

                        case "ReturnStatementSyntax":
                            {
                                yield return SyntaxKind.ReturnStatement;
                                break;
                            }

                        case "SelectClauseSyntax":
                            {
                                yield return SyntaxKind.SelectClause;
                                break;
                            }

                        case "ShebangDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.ShebangDirectiveTrivia;
                                break;
                            }

                        case "SimpleBaseTypeSyntax":
                            {
                                yield return SyntaxKind.SimpleBaseType;
                                break;
                            }

                        case "SimpleLambdaExpressionSyntax":
                            {
                                yield return SyntaxKind.SimpleLambdaExpression;
                                break;
                            }

                        case "SingleVariableDesignationSyntax":
                            {
                                yield return SyntaxKind.SingleVariableDesignation;
                                break;
                            }

                        case "SizeOfExpressionSyntax":
                            {
                                yield return SyntaxKind.SizeOfExpression;
                                break;
                            }

                        case "SkippedTokensTriviaSyntax":
                            {
                                yield return SyntaxKind.SkippedTokensTrivia;
                                break;
                            }

                        case "StackAllocArrayCreationExpressionSyntax":
                            {
                                yield return SyntaxKind.StackAllocArrayCreationExpression;
                                break;
                            }

                        case "StructDeclarationSyntax":
                            {
                                yield return SyntaxKind.StructDeclaration;
                                break;
                            }

                        case "SwitchSectionSyntax":
                            {
                                yield return SyntaxKind.SwitchSection;
                                break;
                            }

                        case "SwitchStatementSyntax":
                            {
                                yield return SyntaxKind.SwitchStatement;
                                break;
                            }

                        case "ThisExpressionSyntax":
                            {
                                yield return SyntaxKind.ThisExpression;
                                break;
                            }

                        case "ThrowExpressionSyntax":
                            {
                                yield return SyntaxKind.ThrowExpression;
                                break;
                            }

                        case "ThrowStatementSyntax":
                            {
                                yield return SyntaxKind.ThrowStatement;
                                break;
                            }

                        case "TryStatementSyntax":
                            {
                                yield return SyntaxKind.TryStatement;
                                break;
                            }

                        case "TupleElementSyntax":
                            {
                                yield return SyntaxKind.TupleElement;
                                break;
                            }

                        case "TupleExpressionSyntax":
                            {
                                yield return SyntaxKind.TupleExpression;
                                break;
                            }

                        case "TupleTypeSyntax":
                            {
                                yield return SyntaxKind.TupleType;
                                break;
                            }

                        case "TypeArgumentListSyntax":
                            {
                                yield return SyntaxKind.TypeArgumentList;
                                break;
                            }

                        case "TypeConstraintSyntax":
                            {
                                yield return SyntaxKind.TypeConstraint;
                                break;
                            }

                        case "TypeCrefSyntax":
                            {
                                yield return SyntaxKind.TypeCref;
                                break;
                            }

                        case "TypeOfExpressionSyntax":
                            {
                                yield return SyntaxKind.TypeOfExpression;
                                break;
                            }

                        case "TypeParameterConstraintClauseSyntax":
                            {
                                yield return SyntaxKind.TypeParameterConstraintClause;
                                break;
                            }

                        case "TypeParameterListSyntax":
                            {
                                yield return SyntaxKind.TypeParameterList;
                                break;
                            }

                        case "TypeParameterSyntax":
                            {
                                yield return SyntaxKind.TypeParameter;
                                break;
                            }

                        case "UndefDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.UndefDirectiveTrivia;
                                break;
                            }

                        case "UnsafeStatementSyntax":
                            {
                                yield return SyntaxKind.UnsafeStatement;
                                break;
                            }

                        case "UsingDirectiveSyntax":
                            {
                                yield return SyntaxKind.UsingDirective;
                                break;
                            }

                        case "UsingStatementSyntax":
                            {
                                yield return SyntaxKind.UsingStatement;
                                break;
                            }

                        case "VariableDeclarationSyntax":
                            {
                                yield return SyntaxKind.VariableDeclaration;
                                break;
                            }

                        case "VariableDeclaratorSyntax":
                            {
                                yield return SyntaxKind.VariableDeclarator;
                                break;
                            }

                        case "WarningDirectiveTriviaSyntax":
                            {
                                yield return SyntaxKind.WarningDirectiveTrivia;
                                break;
                            }

                        case "WhenClauseSyntax":
                            {
                                yield return SyntaxKind.WhenClause;
                                break;
                            }

                        case "WhereClauseSyntax":
                            {
                                yield return SyntaxKind.WhereClause;
                                break;
                            }

                        case "WhileStatementSyntax":
                            {
                                yield return SyntaxKind.WhileStatement;
                                break;
                            }

                        case "XmlCDataSectionSyntax":
                            {
                                yield return SyntaxKind.XmlCDataSection;
                                break;
                            }

                        case "XmlCommentSyntax":
                            {
                                yield return SyntaxKind.XmlComment;
                                break;
                            }

                        case "XmlCrefAttributeSyntax":
                            {
                                yield return SyntaxKind.XmlCrefAttribute;
                                break;
                            }

                        case "XmlElementEndTagSyntax":
                            {
                                yield return SyntaxKind.XmlElementEndTag;
                                break;
                            }

                        case "XmlElementStartTagSyntax":
                            {
                                yield return SyntaxKind.XmlElementStartTag;
                                break;
                            }

                        case "XmlElementSyntax":
                            {
                                yield return SyntaxKind.XmlElement;
                                break;
                            }

                        case "XmlEmptyElementSyntax":
                            {
                                yield return SyntaxKind.XmlEmptyElement;
                                break;
                            }

                        case "XmlNameAttributeSyntax":
                            {
                                yield return SyntaxKind.XmlNameAttribute;
                                break;
                            }

                        case "XmlNameSyntax":
                            {
                                yield return SyntaxKind.XmlName;
                                break;
                            }

                        case "XmlPrefixSyntax":
                            {
                                yield return SyntaxKind.XmlPrefix;
                                break;
                            }

                        case "XmlProcessingInstructionSyntax":
                            {
                                yield return SyntaxKind.XmlProcessingInstruction;
                                break;
                            }

                        case "XmlTextAttributeSyntax":
                            {
                                yield return SyntaxKind.XmlTextAttribute;
                                break;
                            }

                        case "XmlTextSyntax":
                            {
                                yield return SyntaxKind.XmlText;
                                break;
                            }

                        case "YieldStatementSyntax":
                            {
                                yield return SyntaxKind.YieldBreakStatement;
                                yield return SyntaxKind.YieldReturnStatement;
                                break;
                            }
                    }
                }
            }
        }
    }
}
