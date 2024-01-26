// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxWalkers;

internal class TypeCSharpSyntaxWalker : CSharpSyntaxWalker2
{
    protected virtual void VisitType(TypeSyntax? node)
    {
        if (node is null)
            return;

        switch (node.Kind())
        {
            case SyntaxKind.AliasQualifiedName:
                VisitAliasQualifiedName((AliasQualifiedNameSyntax)node);
                break;
            case SyntaxKind.ArrayType:
                VisitArrayType((ArrayTypeSyntax)node);
                break;
            case SyntaxKind.FunctionPointerType:
                VisitFunctionPointerType((FunctionPointerTypeSyntax)node);
                break;
            case SyntaxKind.GenericName:
                VisitGenericName((GenericNameSyntax)node);
                break;
            case SyntaxKind.IdentifierName:
                VisitIdentifierName((IdentifierNameSyntax)node);
                break;
            case SyntaxKind.NullableType:
                VisitNullableType((NullableTypeSyntax)node);
                break;
            case SyntaxKind.OmittedTypeArgument:
                VisitOmittedTypeArgument((OmittedTypeArgumentSyntax)node);
                break;
            case SyntaxKind.PointerType:
                VisitPointerType((PointerTypeSyntax)node);
                break;
            case SyntaxKind.PredefinedType:
                VisitPredefinedType((PredefinedTypeSyntax)node);
                break;
            case SyntaxKind.QualifiedName:
                VisitQualifiedName((QualifiedNameSyntax)node);
                break;
            case SyntaxKind.RefType:
                VisitRefType((RefTypeSyntax)node);
                break;
            case SyntaxKind.ScopedType:
                VisitScopedType((ScopedTypeSyntax)node);
                break;
            case SyntaxKind.TupleType:
                VisitTupleType((TupleTypeSyntax)node);
                break;
            default:
                Debug.Fail($"Unrecognized kind '{node.Kind()}'.");
                base.Visit(node);
                break;
        }
    }

    public override void VisitArrayType(ArrayTypeSyntax node)
    {
        VisitType(node.ElementType);
        base.VisitArrayType(node);
    }

    public override void VisitAttribute(AttributeSyntax node)
    {
        VisitType(node.Name);
        base.VisitAttribute(node);
    }

    public override void VisitCastExpression(CastExpressionSyntax node)
    {
        VisitType(node.Type);
        base.VisitCastExpression(node);
    }

    public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
    {
        VisitType(node.Type);
        base.VisitCatchDeclaration(node);
    }

    public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
    {
        VisitType(node.Type);
        base.VisitConversionOperatorDeclaration(node);
    }

    public override void VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node)
    {
        VisitType(node.Type);
        base.VisitConversionOperatorMemberCref(node);
    }

    public override void VisitCrefParameter(CrefParameterSyntax node)
    {
        VisitType(node.Type);
        base.VisitCrefParameter(node);
    }

    public override void VisitDeclarationExpression(DeclarationExpressionSyntax node)
    {
        VisitType(node.Type);
        base.VisitDeclarationExpression(node);
    }

    public override void VisitDeclarationPattern(DeclarationPatternSyntax node)
    {
        VisitType(node.Type);
        base.VisitDeclarationPattern(node);
    }

    public override void VisitDefaultExpression(DefaultExpressionSyntax node)
    {
        VisitType(node.Type);
        base.VisitDefaultExpression(node);
    }

    public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
    {
        VisitType(node.ReturnType);
        base.VisitDelegateDeclaration(node);
    }

    public override void VisitEventDeclaration(EventDeclarationSyntax node)
    {
        VisitType(node.Type);
        base.VisitEventDeclaration(node);
    }

    public override void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
    {
        VisitType(node.Name);
        base.VisitExplicitInterfaceSpecifier(node);
    }

    public override void VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
    {
        VisitType(node.Name);
        base.VisitFileScopedNamespaceDeclaration(node);
    }

    public override void VisitForEachStatement(ForEachStatementSyntax node)
    {
        VisitType(node.Type);
        base.VisitForEachStatement(node);
    }

    public override void VisitFromClause(FromClauseSyntax node)
    {
        VisitType(node.Type);
        base.VisitFromClause(node);
    }

    public override void VisitFunctionPointerParameter(FunctionPointerParameterSyntax node)
    {
        VisitType(node.Type);
        base.VisitFunctionPointerParameter(node);
    }

    public override void VisitIncompleteMember(IncompleteMemberSyntax node)
    {
        VisitType(node.Type);
        base.VisitIncompleteMember(node);
    }

    public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
    {
        VisitType(node.Type);
        base.VisitIndexerDeclaration(node);
    }

    public override void VisitJoinClause(JoinClauseSyntax node)
    {
        VisitType(node.Type);
        base.VisitJoinClause(node);
    }

    public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
    {
        VisitType(node.ReturnType);
        base.VisitLocalFunctionStatement(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        VisitType(node.ReturnType);
        base.VisitMethodDeclaration(node);
    }

    public override void VisitNameMemberCref(NameMemberCrefSyntax node)
    {
        VisitType(node.Name);
        base.VisitNameMemberCref(node);
    }

    public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
    {
        VisitType(node.Name);
        base.VisitNamespaceDeclaration(node);
    }

    public override void VisitNullableType(NullableTypeSyntax node)
    {
        VisitType(node.ElementType);
        base.VisitNullableType(node);
    }

    public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        VisitType(node.Type);
        base.VisitObjectCreationExpression(node);
    }
    public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
    {
        VisitType(node.ReturnType);
        base.VisitOperatorDeclaration(node);
    }

    public override void VisitParameter(ParameterSyntax node)
    {
        VisitType(node.Type);
        base.VisitParameter(node);
    }

    public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
    {
        VisitType(node.ReturnType);
        base.VisitParenthesizedLambdaExpression(node);
    }

    public override void VisitPointerType(PointerTypeSyntax node)
    {
        VisitType(node.ElementType);
        base.VisitPointerType(node);
    }

    public override void VisitPrimaryConstructorBaseType(PrimaryConstructorBaseTypeSyntax node)
    {
        VisitType(node.Type);
        base.VisitPrimaryConstructorBaseType(node);
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        VisitType(node.Type);
        base.VisitPropertyDeclaration(node);
    }

    public override void VisitQualifiedCref(QualifiedCrefSyntax node)
    {
        VisitType(node.Container);
        base.VisitQualifiedCref(node);
    }

    public override void VisitQualifiedName(QualifiedNameSyntax node)
    {
        VisitType(node.Left);
        base.VisitQualifiedName(node);
    }

    public override void VisitRecursivePattern(RecursivePatternSyntax node)
    {
        VisitType(node.Type);
        base.VisitRecursivePattern(node);
    }

    public override void VisitRefType(RefTypeSyntax node)
    {
        VisitType(node.Type);
        base.VisitRefType(node);
    }

    public override void VisitRefValueExpression(RefValueExpressionSyntax node)
    {
        VisitType(node.Type);
        base.VisitRefValueExpression(node);
    }

    public override void VisitScopedType(ScopedTypeSyntax node)
    {
        VisitType(node.Type);
        base.VisitScopedType(node);
    }

    public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
    {
        VisitType(node.Type);
        base.VisitSimpleBaseType(node);
    }

    public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
    {
        VisitType(node.Type);
        base.VisitSizeOfExpression(node);
    }

    public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
    {
        VisitType(node.Type);
        base.VisitStackAllocArrayCreationExpression(node);
    }

    public override void VisitTupleElement(TupleElementSyntax node)
    {
        VisitType(node.Type);
        base.VisitTupleElement(node);
    }

    public override void VisitTypeArgumentList(TypeArgumentListSyntax node)
    {
        foreach (TypeSyntax type in node.Arguments)
            VisitType(type);

        base.VisitTypeArgumentList(node);
    }

    public override void VisitTypeConstraint(TypeConstraintSyntax node)
    {
        VisitType(node.Type);
        base.VisitTypeConstraint(node);
    }

    public override void VisitTypeCref(TypeCrefSyntax node)
    {
        VisitType(node.Type);
        base.VisitTypeCref(node);
    }

    public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
    {
        VisitType(node.Type);
        base.VisitTypeOfExpression(node);
    }

    public override void VisitTypePattern(TypePatternSyntax node)
    {
        VisitType(node.Type);
        base.VisitTypePattern(node);
    }

    public override void VisitUsingDirective(UsingDirectiveSyntax node)
    {
        VisitType(node.Name);
        VisitType(node.NamespaceOrType);
        base.VisitUsingDirective(node);
    }

    public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
    {
        VisitType(node.Type);
        base.VisitVariableDeclaration(node);
    }
}
