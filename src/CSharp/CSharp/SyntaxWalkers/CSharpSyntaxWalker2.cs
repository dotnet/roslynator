// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxWalkers;

internal class CSharpSyntaxWalker2 : CSharpSyntaxWalker
{
    public override void DefaultVisit(SyntaxNode node)
    {
        if (!ShouldVisit)
            return;

        if (node is TypeSyntax type)
            VisitType(type);

        base.DefaultVisit(node);
    }

    protected virtual bool ShouldVisit => true;

    protected virtual void VisitType(TypeSyntax node)
    {
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
}
