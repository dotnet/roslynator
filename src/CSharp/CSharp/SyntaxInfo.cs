// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp
{
    /// <summary>
    /// Serves as a factory for types in Roslynator.CSharp.Syntax namespace.
    /// </summary>
    public static class SyntaxInfo
    {
        /// <summary>
        /// Creates a new <see cref="Syntax.AsExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static AsExpressionInfo AsExpressionInfo(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.AsExpressionInfo.Create(
                node,
                walkDownParentheses,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.AsExpressionInfo"/> from the specified binary expression.
        /// </summary>
        /// <param name="binaryExpression"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static AsExpressionInfo AsExpressionInfo(
            BinaryExpressionSyntax binaryExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.AsExpressionInfo.Create(
                binaryExpression,
                walkDownParentheses,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.AssignmentExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static AssignmentExpressionInfo AssignmentExpressionInfo(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.AssignmentExpressionInfo.Create(node, walkDownParentheses, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.AssignmentExpressionInfo"/> from the specified assignment expression.
        /// </summary>
        /// <param name="assignmentExpression"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static AssignmentExpressionInfo AssignmentExpressionInfo(
            AssignmentExpressionSyntax assignmentExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.AssignmentExpressionInfo.Create(assignmentExpression, walkDownParentheses, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.BinaryExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static BinaryExpressionInfo BinaryExpressionInfo(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.BinaryExpressionInfo.Create(
                node,
                walkDownParentheses,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.BinaryExpressionInfo"/> from the specified binary expression.
        /// </summary>
        /// <param name="binaryExpression"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static BinaryExpressionInfo BinaryExpressionInfo(
            BinaryExpressionSyntax binaryExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.BinaryExpressionInfo.Create(
                binaryExpression,
                walkDownParentheses,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ConditionalExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static ConditionalExpressionInfo ConditionalExpressionInfo(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.ConditionalExpressionInfo.Create(
                node,
                walkDownParentheses,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ConditionalExpressionInfo"/> from the specified conditional expression.
        /// </summary>
        /// <param name="conditionalExpression"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static ConditionalExpressionInfo ConditionalExpressionInfo(
            ConditionalExpressionSyntax conditionalExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.ConditionalExpressionInfo.Create(
                conditionalExpression,
                walkDownParentheses,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.GenericInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static GenericInfo GenericInfo(SyntaxNode node)
        {
            return Syntax.GenericInfo.Create(node);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.GenericInfo"/> from the specified type parameter constraint.
        /// </summary>
        /// <param name="typeParameterConstraint"></param>
        /// <returns></returns>
        public static GenericInfo GenericInfo(TypeParameterConstraintSyntax typeParameterConstraint)
        {
            return Syntax.GenericInfo.Create(typeParameterConstraint);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.GenericInfo"/> from the specified constraint clause.
        /// </summary>
        /// <param name="constraintClause"></param>
        /// <returns></returns>
        public static GenericInfo GenericInfo(TypeParameterConstraintClauseSyntax constraintClause)
        {
            return Syntax.GenericInfo.Create(constraintClause);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.GenericInfo"/> from the specified type parameter.
        /// </summary>
        /// <param name="typeParameter"></param>
        /// <returns></returns>
        public static GenericInfo GenericInfo(TypeParameterSyntax typeParameter)
        {
            return Syntax.GenericInfo.Create(typeParameter);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.GenericInfo"/> from the specified type parameter list.
        /// </summary>
        /// <param name="typeParameterList"></param>
        /// <returns></returns>
        public static GenericInfo GenericInfo(TypeParameterListSyntax typeParameterList)
        {
            return Syntax.GenericInfo.Create(typeParameterList);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.GenericInfo"/> from the specified type declaration.
        /// </summary>
        /// <param name="typeDeclaration"></param>
        /// <returns></returns>
        public static GenericInfo GenericInfo(TypeDeclarationSyntax typeDeclaration)
        {
            return Syntax.GenericInfo.Create(typeDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.GenericInfo"/> from the specified delegate declaration.
        /// </summary>
        /// <param name="delegateDeclaration"></param>
        /// <returns></returns>
        public static GenericInfo GenericInfo(DelegateDeclarationSyntax delegateDeclaration)
        {
            return Syntax.GenericInfo.Create(delegateDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.GenericInfo"/> from the specified local function.
        /// </summary>
        /// <param name="localFunctionStatement"></param>
        /// <returns></returns>
        public static GenericInfo GenericInfo(LocalFunctionStatementSyntax localFunctionStatement)
        {
            return Syntax.GenericInfo.Create(localFunctionStatement);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.GenericInfo"/> from the specified method declaration.
        /// </summary>
        /// <param name="methodDeclaration"></param>
        /// <returns></returns>
        public static GenericInfo GenericInfo(MethodDeclarationSyntax methodDeclaration)
        {
            return Syntax.GenericInfo.Create(methodDeclaration);
        }

        internal static HexNumericLiteralExpressionInfo HexNumericLiteralExpressionInfo(SyntaxNode node, bool walkDownParentheses = true)
        {
            return Syntax.HexNumericLiteralExpressionInfo.Create(node, walkDownParentheses);
        }

        internal static HexNumericLiteralExpressionInfo HexNumericLiteralExpressionInfo(LiteralExpressionSyntax literalExpression)
        {
            return Syntax.HexNumericLiteralExpressionInfo.Create(literalExpression);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.IsExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static IsExpressionInfo IsExpressionInfo(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.IsExpressionInfo.Create(
                node,
                walkDownParentheses,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.IsExpressionInfo"/> from the specified binary expression.
        /// </summary>
        /// <param name="binaryExpression"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static IsExpressionInfo IsExpressionInfo(
            BinaryExpressionSyntax binaryExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.IsExpressionInfo.Create(
                binaryExpression,
                walkDownParentheses,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.LocalDeclarationStatementInfo"/> from the specified local declaration statement.
        /// </summary>
        /// <param name="localDeclarationStatement"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static LocalDeclarationStatementInfo LocalDeclarationStatementInfo(
            LocalDeclarationStatementSyntax localDeclarationStatement,
            bool allowMissing = false)
        {
            return Syntax.LocalDeclarationStatementInfo.Create(localDeclarationStatement, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.LocalDeclarationStatementInfo"/> from the specified expression.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static LocalDeclarationStatementInfo LocalDeclarationStatementInfo(
            ExpressionSyntax value,
            bool allowMissing = false)
        {
            return Syntax.LocalDeclarationStatementInfo.Create(value, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.MemberDeclarationListInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static MemberDeclarationListInfo MemberDeclarationListInfo(SyntaxNode node)
        {
            return Syntax.MemberDeclarationListInfo.Create(node);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.MemberDeclarationListInfo"/> from the specified compilation unit.
        /// </summary>
        /// <param name="compilationUnit"></param>
        /// <returns></returns>
        public static MemberDeclarationListInfo MemberDeclarationListInfo(CompilationUnitSyntax compilationUnit)
        {
            return Syntax.MemberDeclarationListInfo.Create(compilationUnit);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.MemberDeclarationListInfo"/> from the specified declaration.
        /// </summary>
        /// <param name="declaration"></param>
        /// <returns></returns>
        public static MemberDeclarationListInfo MemberDeclarationListInfo(NamespaceDeclarationSyntax declaration)
        {
            return Syntax.MemberDeclarationListInfo.Create(declaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.MemberDeclarationListInfo"/> from the specified declaration.
        /// </summary>
        /// <param name="declaration"></param>
        /// <returns></returns>
        public static MemberDeclarationListInfo MemberDeclarationListInfo(ClassDeclarationSyntax declaration)
        {
            return Syntax.MemberDeclarationListInfo.Create(declaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.MemberDeclarationListInfo"/> from the specified declaration.
        /// </summary>
        /// <param name="declaration"></param>
        /// <returns></returns>
        public static MemberDeclarationListInfo MemberDeclarationListInfo(StructDeclarationSyntax declaration)
        {
            return Syntax.MemberDeclarationListInfo.Create(declaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.MemberDeclarationListInfo"/> from the specified declaration.
        /// </summary>
        /// <param name="declaration"></param>
        /// <returns></returns>
        public static MemberDeclarationListInfo MemberDeclarationListInfo(InterfaceDeclarationSyntax declaration)
        {
            return Syntax.MemberDeclarationListInfo.Create(declaration);
        }

        internal static MemberDeclarationListInfo MemberDeclarationListInfo(MemberDeclarationListSelection selectedMembers)
        {
            return Syntax.MemberDeclarationListInfo.Create(selectedMembers);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleMemberInvocationExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleMemberInvocationExpressionInfo SimpleMemberInvocationExpressionInfo(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.SimpleMemberInvocationExpressionInfo.Create(
                node,
                walkDownParentheses,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleMemberInvocationExpressionInfo"/> from the specified invocation expression.
        /// </summary>
        /// <param name="invocationExpression"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleMemberInvocationExpressionInfo SimpleMemberInvocationExpressionInfo(
            InvocationExpressionSyntax invocationExpression,
            bool allowMissing = false)
        {
            return Syntax.SimpleMemberInvocationExpressionInfo.Create(
                invocationExpression,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleMemberInvocationStatementInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleMemberInvocationStatementInfo SimpleMemberInvocationStatementInfo(
            SyntaxNode node,
            bool allowMissing = false)
        {
            return Syntax.SimpleMemberInvocationStatementInfo.Create(
                node,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleMemberInvocationStatementInfo"/> from the specified expression statement.
        /// </summary>
        /// <param name="expressionStatement"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleMemberInvocationStatementInfo SimpleMemberInvocationStatementInfo(
            ExpressionStatementSyntax expressionStatement,
            bool allowMissing = false)
        {
            return Syntax.SimpleMemberInvocationStatementInfo.Create(
                expressionStatement,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleMemberInvocationStatementInfo"/> from the specified invocation expression.
        /// </summary>
        /// <param name="invocationExpression"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleMemberInvocationStatementInfo SimpleMemberInvocationStatementInfo(
            InvocationExpressionSyntax invocationExpression,
            bool allowMissing = false)
        {
            return Syntax.SimpleMemberInvocationStatementInfo.Create(
                invocationExpression,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(SyntaxNode node)
        {
            return Syntax.ModifierListInfo.Create(node);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified class declaration.
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(ClassDeclarationSyntax classDeclaration)
        {
            return Syntax.ModifierListInfo.Create(classDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified constructor declaration.
        /// </summary>
        /// <param name="constructorDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(ConstructorDeclarationSyntax constructorDeclaration)
        {
            return Syntax.ModifierListInfo.Create(constructorDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified conversion operator declaration.
        /// </summary>
        /// <param name="conversionOperatorDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration)
        {
            return Syntax.ModifierListInfo.Create(conversionOperatorDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified delegate declaration.
        /// </summary>
        /// <param name="delegateDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(DelegateDeclarationSyntax delegateDeclaration)
        {
            return Syntax.ModifierListInfo.Create(delegateDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified destructor declaration.
        /// </summary>
        /// <param name="destructorDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(DestructorDeclarationSyntax destructorDeclaration)
        {
            return Syntax.ModifierListInfo.Create(destructorDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified enum declaration.
        /// </summary>
        /// <param name="enumDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(EnumDeclarationSyntax enumDeclaration)
        {
            return Syntax.ModifierListInfo.Create(enumDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified event declaration.
        /// </summary>
        /// <param name="eventDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(EventDeclarationSyntax eventDeclaration)
        {
            return Syntax.ModifierListInfo.Create(eventDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified event field declaration.
        /// </summary>
        /// <param name="eventFieldDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            return Syntax.ModifierListInfo.Create(eventFieldDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified field declaration.
        /// </summary>
        /// <param name="fieldDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(FieldDeclarationSyntax fieldDeclaration)
        {
            return Syntax.ModifierListInfo.Create(fieldDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified indexer declaration.
        /// </summary>
        /// <param name="indexerDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(IndexerDeclarationSyntax indexerDeclaration)
        {
            return Syntax.ModifierListInfo.Create(indexerDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified interface declaration.
        /// </summary>
        /// <param name="interfaceDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            return Syntax.ModifierListInfo.Create(interfaceDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified method declaration.
        /// </summary>
        /// <param name="methodDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(MethodDeclarationSyntax methodDeclaration)
        {
            return Syntax.ModifierListInfo.Create(methodDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified operator declaration.
        /// </summary>
        /// <param name="operatorDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(OperatorDeclarationSyntax operatorDeclaration)
        {
            return Syntax.ModifierListInfo.Create(operatorDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified property declaration.
        /// </summary>
        /// <param name="propertyDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(PropertyDeclarationSyntax propertyDeclaration)
        {
            return Syntax.ModifierListInfo.Create(propertyDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified struct declaration.
        /// </summary>
        /// <param name="structDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(StructDeclarationSyntax structDeclaration)
        {
            return Syntax.ModifierListInfo.Create(structDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified incomplete member.
        /// </summary>
        /// <param name="incompleteMember"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(IncompleteMemberSyntax incompleteMember)
        {
            return Syntax.ModifierListInfo.Create(incompleteMember);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified accessor declaration.
        /// </summary>
        /// <param name="accessorDeclaration"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(AccessorDeclarationSyntax accessorDeclaration)
        {
            return Syntax.ModifierListInfo.Create(accessorDeclaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified local declaration statement.
        /// </summary>
        /// <param name="localDeclarationStatement"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(LocalDeclarationStatementSyntax localDeclarationStatement)
        {
            return Syntax.ModifierListInfo.Create(localDeclarationStatement);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified local function.
        /// </summary>
        /// <param name="localFunctionStatement"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(LocalFunctionStatementSyntax localFunctionStatement)
        {
            return Syntax.ModifierListInfo.Create(localFunctionStatement);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.ModifierListInfo"/> from the specified parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static ModifierListInfo ModifierListInfo(ParameterSyntax parameter)
        {
            return Syntax.ModifierListInfo.Create(parameter);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.NullCheckExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="allowedStyles"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// 
        /// <returns></returns>
        public static NullCheckExpressionInfo NullCheckExpressionInfo(
            SyntaxNode node,
            NullCheckStyles allowedStyles = NullCheckStyles.ComparisonToNull | NullCheckStyles.IsPattern,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.NullCheckExpressionInfo.Create(
                node,
                allowedStyles,
                walkDownParentheses,
                allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.NullCheckExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="semanticModel"></param>
        /// <param name="allowedStyles"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static NullCheckExpressionInfo NullCheckExpressionInfo(
            SyntaxNode node,
            SemanticModel semanticModel,
            NullCheckStyles allowedStyles = NullCheckStyles.All,
            bool walkDownParentheses = true,
            bool allowMissing = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Syntax.NullCheckExpressionInfo.Create(
                node,
                semanticModel,
                allowedStyles,
                walkDownParentheses,
                allowMissing,
                cancellationToken);
        }

        internal static ParameterInfo ParameterInfo(
            ConstructorDeclarationSyntax constructorDeclaration,
            bool allowMissing = false)
        {
            return Syntax.ParameterInfo.Create(constructorDeclaration, allowMissing);
        }

        internal static ParameterInfo ParameterInfo(
            MethodDeclarationSyntax methodDeclaration,
            bool allowMissing = false)
        {
            return Syntax.ParameterInfo.Create(methodDeclaration, allowMissing);
        }

        internal static ParameterInfo ParameterInfo(
            OperatorDeclarationSyntax operatorDeclaration,
            bool allowMissing = false)
        {
            return Syntax.ParameterInfo.Create(operatorDeclaration, allowMissing);
        }

        internal static ParameterInfo ParameterInfo(
            ConversionOperatorDeclarationSyntax conversionOperatorDeclaration,
            bool allowMissing = false)
        {
            return Syntax.ParameterInfo.Create(conversionOperatorDeclaration, allowMissing);
        }

        internal static ParameterInfo ParameterInfo(
            DelegateDeclarationSyntax delegateDeclaration,
            bool allowMissing = false)
        {
            return Syntax.ParameterInfo.Create(delegateDeclaration, allowMissing);
        }

        internal static ParameterInfo ParameterInfo(
            LocalFunctionStatementSyntax localFunction,
            bool allowMissing = false)
        {
            return Syntax.ParameterInfo.Create(localFunction, allowMissing);
        }

        internal static ParameterInfo ParameterInfo(
            IndexerDeclarationSyntax indexerDeclaration,
            bool allowMissing = false)
        {
            return Syntax.ParameterInfo.Create(indexerDeclaration, allowMissing);
        }

        internal static ParameterInfo ParameterInfo(
            SimpleLambdaExpressionSyntax simpleLambda,
            bool allowMissing = false)
        {
            return Syntax.ParameterInfo.Create(simpleLambda, allowMissing);
        }

        internal static ParameterInfo ParameterInfo(
            ParenthesizedLambdaExpressionSyntax parenthesizedLambda,
            bool allowMissing = false)
        {
            return Syntax.ParameterInfo.Create(parenthesizedLambda, allowMissing);
        }

        internal static ParameterInfo ParameterInfo(
            AnonymousMethodExpressionSyntax anonymousMethod,
            bool allowMissing = false)
        {
            return Syntax.ParameterInfo.Create(anonymousMethod, allowMissing);
        }

        internal static RegionInfo RegionInfo(SyntaxNode node)
        {
            return Syntax.RegionInfo.Create(node);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.RegionInfo"/> from the specified region directive.
        /// </summary>
        /// <param name="regionDirective"></param>
        /// <returns></returns>
        public static RegionInfo RegionInfo(RegionDirectiveTriviaSyntax regionDirective)
        {
            return Syntax.RegionInfo.Create(regionDirective);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.RegionInfo"/> from the specified endregion directive.
        /// </summary>
        /// <param name="endRegionDirective"></param>
        /// <returns></returns>
        public static RegionInfo RegionInfo(EndRegionDirectiveTriviaSyntax endRegionDirective)
        {
            return Syntax.RegionInfo.Create(endRegionDirective);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleAssignmentExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleAssignmentExpressionInfo SimpleAssignmentExpressionInfo(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.SimpleAssignmentExpressionInfo.Create(node, walkDownParentheses, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleAssignmentExpressionInfo"/> from the specified assignment expression.
        /// </summary>
        /// <param name="assignmentExpression"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleAssignmentExpressionInfo SimpleAssignmentExpressionInfo(
            AssignmentExpressionSyntax assignmentExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.SimpleAssignmentExpressionInfo.Create(assignmentExpression, walkDownParentheses, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleAssignmentStatementInfo"/> from the specified statement.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleAssignmentStatementInfo SimpleAssignmentStatementInfo(
            StatementSyntax statement,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.SimpleAssignmentStatementInfo.Create(statement, walkDownParentheses, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleAssignmentStatementInfo"/> from the specified assignment expression.
        /// </summary>
        /// <param name="assignmentExpression"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleAssignmentStatementInfo SimpleAssignmentStatementInfo(
            AssignmentExpressionSyntax assignmentExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.SimpleAssignmentStatementInfo.Create(assignmentExpression, walkDownParentheses, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleAssignmentStatementInfo"/> from the specified expression statement.
        /// </summary>
        /// <param name="expressionStatement"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleAssignmentStatementInfo SimpleAssignmentStatementInfo(
            ExpressionStatementSyntax expressionStatement,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.SimpleAssignmentStatementInfo.Create(expressionStatement, walkDownParentheses, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleIfElseInfo"/> from the specified if statement.
        /// </summary>
        /// <param name="ifStatement"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleIfElseInfo SimpleIfElseInfo(
            IfStatementSyntax ifStatement,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.SimpleIfElseInfo.Create(ifStatement, walkDownParentheses, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleIfStatementInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleIfStatementInfo SimpleIfStatementInfo(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.SimpleIfStatementInfo.Create(node, walkDownParentheses, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SimpleIfStatementInfo"/> from the specified if statement.
        /// </summary>
        /// <param name="ifStatement"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SimpleIfStatementInfo SimpleIfStatementInfo(
            IfStatementSyntax ifStatement,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.SimpleIfStatementInfo.Create(ifStatement, walkDownParentheses, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SingleLocalDeclarationStatementInfo"/> from the specified local declaration statement.
        /// </summary>
        /// <param name="localDeclarationStatement"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SingleLocalDeclarationStatementInfo SingleLocalDeclarationStatementInfo(
            LocalDeclarationStatementSyntax localDeclarationStatement,
            bool allowMissing = false)
        {
            return Syntax.SingleLocalDeclarationStatementInfo.Create(localDeclarationStatement, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SingleLocalDeclarationStatementInfo"/> from the specified variable declaration.
        /// </summary>
        /// <param name="variableDeclaration"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SingleLocalDeclarationStatementInfo SingleLocalDeclarationStatementInfo(
            VariableDeclarationSyntax variableDeclaration,
            bool allowMissing = false)
        {
            return Syntax.SingleLocalDeclarationStatementInfo.Create(variableDeclaration, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SingleLocalDeclarationStatementInfo"/> from the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SingleLocalDeclarationStatementInfo SingleLocalDeclarationStatementInfo(ExpressionSyntax value)
        {
            return Syntax.SingleLocalDeclarationStatementInfo.Create(value);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SingleParameterLambdaExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SingleParameterLambdaExpressionInfo SingleParameterLambdaExpressionInfo(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Syntax.SingleParameterLambdaExpressionInfo.Create(node, walkDownParentheses, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.SingleParameterLambdaExpressionInfo"/> from the specified lambda expression.
        /// </summary>
        /// <param name="lambdaExpression"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        public static SingleParameterLambdaExpressionInfo SingleParameterLambdaExpressionInfo(
            LambdaExpressionSyntax lambdaExpression,
            bool allowMissing = false)
        {
            return Syntax.SingleParameterLambdaExpressionInfo.Create(lambdaExpression, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.StatementListInfo"/> from the specified statement.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public static StatementListInfo StatementListInfo(StatementSyntax statement)
        {
            return Syntax.StatementListInfo.Create(statement);
        }

        internal static StatementListInfo StatementListInfo(BlockSyntax block)
        {
            return Syntax.StatementListInfo.Create(block);
        }

        internal static StatementListInfo StatementListInfo(SwitchSectionSyntax switchSection)
        {
            return Syntax.StatementListInfo.Create(switchSection);
        }

        internal static StatementListInfo StatementListInfo(StatementListSelection selectedStatements)
        {
            return Syntax.StatementListInfo.Create(selectedStatements);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.StringConcatenationExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="semanticModel"></param>
        /// <param name="walkDownParentheses"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static StringConcatenationExpressionInfo StringConcatenationExpressionInfo(
            SyntaxNode node,
            SemanticModel semanticModel,
            bool walkDownParentheses = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Syntax.StringConcatenationExpressionInfo.Create(node, semanticModel, walkDownParentheses, cancellationToken);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.StringConcatenationExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="binaryExpression"></param>
        /// <param name="semanticModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static StringConcatenationExpressionInfo StringConcatenationExpressionInfo(
            BinaryExpressionSyntax binaryExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Syntax.StringConcatenationExpressionInfo.Create(binaryExpression, semanticModel, cancellationToken);
        }

        internal static StringConcatenationExpressionInfo StringConcatenationExpressionInfo(
            BinaryExpressionSelection binaryExpressionSelection,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Syntax.StringConcatenationExpressionInfo.Create(binaryExpressionSelection, semanticModel, cancellationToken);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.StringLiteralExpressionInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="walkDownParentheses"></param>
        /// <returns></returns>
        public static StringLiteralExpressionInfo StringLiteralExpressionInfo(
            SyntaxNode node,
            bool walkDownParentheses = true)
        {
            return Syntax.StringLiteralExpressionInfo.Create(node, walkDownParentheses);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.StringLiteralExpressionInfo"/> from the specified literal expression.
        /// </summary>
        /// <param name="literalExpression"></param>
        /// <returns></returns>
        public static StringLiteralExpressionInfo StringLiteralExpressionInfo(LiteralExpressionSyntax literalExpression)
        {
            return Syntax.StringLiteralExpressionInfo.Create(literalExpression);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.TypeParameterConstraintInfo"/> from the specified constraint.
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="allowMissing"></param>
        /// <returns></returns>
        internal static TypeParameterConstraintInfo TypeParameterConstraintInfo(TypeParameterConstraintSyntax constraint, bool allowMissing = false)
        {
            return Syntax.TypeParameterConstraintInfo.Create(constraint, allowMissing);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.TypeParameterInfo"/> from the specified type parameter.
        /// </summary>
        /// <param name="typeParameter"></param>
        /// <returns></returns>
        internal static TypeParameterInfo TypeParameterInfo(TypeParameterSyntax typeParameter)
        {
            return Syntax.TypeParameterInfo.Create(typeParameter);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.UsingDirectiveListInfo"/> from the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static UsingDirectiveListInfo UsingDirectiveListInfo(SyntaxNode node)
        {
            return Syntax.UsingDirectiveListInfo.Create(node);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.UsingDirectiveListInfo"/> from the specified compilation unit.
        /// </summary>
        /// <param name="compilationUnit"></param>
        /// <returns></returns>
        public static UsingDirectiveListInfo UsingDirectiveListInfo(CompilationUnitSyntax compilationUnit)
        {
            return Syntax.UsingDirectiveListInfo.Create(compilationUnit);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.UsingDirectiveListInfo"/> from the specified declaration.
        /// </summary>
        /// <param name="declaration"></param>
        /// <returns></returns>
        public static UsingDirectiveListInfo UsingDirectiveListInfo(NamespaceDeclarationSyntax declaration)
        {
            return Syntax.UsingDirectiveListInfo.Create(declaration);
        }

        /// <summary>
        /// Creates a new <see cref="Syntax.XmlElementInfo"/> from the specified xml node.
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public static XmlElementInfo XmlElementInfo(XmlNodeSyntax xmlNode)
        {
            return Syntax.XmlElementInfo.Create(xmlNode);
        }
    }
}