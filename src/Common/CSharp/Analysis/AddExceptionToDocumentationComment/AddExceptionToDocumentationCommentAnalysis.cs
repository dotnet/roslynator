﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.AddExceptionToDocumentationComment;

internal static class AddExceptionToDocumentationCommentAnalysis
{
    private static AddExceptionToDocumentationCommentAnalysisResult Fail { get; } = new();

    public static AddExceptionToDocumentationCommentAnalysisResult Analyze(
        ThrowStatementSyntax throwStatement,
        INamedTypeSymbol exceptionSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        ExpressionSyntax expression = throwStatement.Expression;

        if (expression?.IsMissing == false)
        {
            return Analyze(throwStatement, expression, exceptionSymbol, semanticModel, cancellationToken);
        }
        else
        {
            return Fail;
        }
    }

    public static AddExceptionToDocumentationCommentAnalysisResult Analyze(
        ThrowExpressionSyntax throwExpression,
        INamedTypeSymbol exceptionSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        ExpressionSyntax expression = throwExpression.Expression;

        if (expression?.IsMissing == false)
        {
            return Analyze(throwExpression, expression, exceptionSymbol, semanticModel, cancellationToken);
        }
        else
        {
            return Fail;
        }
    }

    private static AddExceptionToDocumentationCommentAnalysisResult Analyze(
        SyntaxNode node,
        ExpressionSyntax expression,
        INamedTypeSymbol exceptionSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        if (semanticModel.GetTypeSymbol(expression, cancellationToken) is not INamedTypeSymbol typeSymbol)
            return Fail;

        if (!InheritsFromException(typeSymbol, exceptionSymbol))
            return Fail;

        if (IsExceptionTypeCaughtInMethod(node, typeSymbol, semanticModel, cancellationToken))
            return Fail;

        ISymbol declarationSymbol = GetDeclarationSymbol(node.SpanStart, semanticModel, cancellationToken);

        if (declarationSymbol?.GetSyntax(cancellationToken) is not MemberDeclarationSyntax containingMember)
            return Fail;

        DocumentationCommentTriviaSyntax comment = containingMember.GetSingleLineDocumentationComment();

        if (comment is null)
            return Fail;

        if (!CanAddExceptionToComment(comment, typeSymbol, semanticModel, cancellationToken))
            return Fail;

        ThrowInfo throwInfo = ThrowInfo.Create(node, typeSymbol, declarationSymbol);

        return new AddExceptionToDocumentationCommentAnalysisResult(throwInfo, comment.ParentTrivia);
    }

    private static bool CanAddExceptionToComment(
        DocumentationCommentTriviaSyntax comment,
        INamedTypeSymbol exceptionSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var containsException = false;
        var containsIncludeOrExclude = false;
        var isFirst = true;

        foreach (XmlNodeSyntax node in comment.Content)
        {
            XmlElementInfo info = SyntaxInfo.XmlElementInfo(node);
            if (info.Success)
            {
                switch (info.GetTag())
                {
                    case XmlTag.Include:
                    case XmlTag.Exclude:
                    {
                        if (isFirst)
                            containsIncludeOrExclude = true;

                        break;
                    }
                    case XmlTag.InheritDoc:
                    {
                        return false;
                    }
                    case XmlTag.Exception:
                    {
                        if (!containsException)
                        {
                            if (info.IsEmptyElement)
                            {
                                containsException = ContainsException((XmlEmptyElementSyntax)info.Element, exceptionSymbol, semanticModel, cancellationToken);
                            }
                            else
                            {
                                containsException = ContainsException((XmlElementSyntax)info.Element, exceptionSymbol, semanticModel, cancellationToken);
                            }
                        }

                        break;
                    }
                }

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    containsIncludeOrExclude = false;
                }
            }
        }

        return !containsIncludeOrExclude
            && !containsException;
    }

    private static bool ContainsException(XmlElementSyntax xmlElement, INamedTypeSymbol exceptionSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        XmlElementStartTagSyntax startTag = xmlElement.StartTag;

        return startTag is not null
            && ContainsException(startTag.Attributes, exceptionSymbol, semanticModel, cancellationToken);
    }

    private static bool ContainsException(XmlEmptyElementSyntax xmlEmptyElement, INamedTypeSymbol exceptionSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        return ContainsException(xmlEmptyElement.Attributes, exceptionSymbol, semanticModel, cancellationToken);
    }

    private static bool ContainsException(SyntaxList<XmlAttributeSyntax> attributes, INamedTypeSymbol exceptionSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        foreach (XmlAttributeSyntax xmlAttribute in attributes)
        {
            if (xmlAttribute.Kind() == SyntaxKind.XmlCrefAttribute)
            {
                var xmlCrefAttribute = (XmlCrefAttributeSyntax)xmlAttribute;

                CrefSyntax cref = xmlCrefAttribute.Cref;

                if (cref is not null
                    && (semanticModel.GetSymbol(cref, cancellationToken) is INamedTypeSymbol symbol))
                {
                    if (SymbolEqualityComparer.Default.Equals(exceptionSymbol, symbol))
                        return true;

                    // https://github.com/dotnet/roslyn/issues/22923
                    if (exceptionSymbol.IsGenericType
                        && symbol.IsGenericType
                        && SymbolEqualityComparer.Default.Equals(exceptionSymbol.ConstructedFrom, symbol.ConstructedFrom))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    internal static ISymbol GetDeclarationSymbol(
        int position,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        ISymbol symbol = semanticModel.GetEnclosingSymbol(position, cancellationToken);

        return GetDeclarationSymbol(symbol);
    }

    private static ISymbol GetDeclarationSymbol(ISymbol symbol)
    {
        if (symbol is not IMethodSymbol methodSymbol)
            return null;

        MethodKind methodKind = methodSymbol.MethodKind;

        if (methodKind == MethodKind.Ordinary)
            return methodSymbol.PartialImplementationPart ?? methodSymbol;

        if (methodKind == MethodKind.LocalFunction)
            return GetDeclarationSymbol(methodSymbol.ContainingSymbol);

        return methodSymbol.AssociatedSymbol;
    }

    private static bool InheritsFromException(ITypeSymbol typeSymbol, INamedTypeSymbol exceptionSymbol)
    {
        return typeSymbol?.TypeKind == TypeKind.Class
            && typeSymbol.BaseType?.IsObject() == false
            && typeSymbol.InheritsFrom(exceptionSymbol);
    }

    /// <summary>
    /// Walk upwards from throw statement and find all try statements in method and see if any of them catches the thrown exception type
    /// </summary>
    private static bool IsExceptionTypeCaughtInMethod(SyntaxNode node, ITypeSymbol exceptionSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        SyntaxNode parent = node.Parent;
        while (parent is not null)
        {
            if (parent is TryStatementSyntax tryStatement)
            {
                foreach (CatchClauseSyntax catchClause in tryStatement.Catches)
                {
                    TypeSyntax exceptionType = catchClause.Declaration?.Type;

                    if (exceptionType is null
                        || SymbolEqualityComparer.Default.Equals(
                            exceptionSymbol,
                            semanticModel.GetTypeSymbol(exceptionType, cancellationToken)))
                    {
                        return true;
                    }
                }
            }

            if (parent is MemberDeclarationSyntax or LocalFunctionStatementSyntax)
            {
                // We don't care if it's caught outside of the current method
                // Since the exception should be documented in this method
                return false;
            }
            else
            {
                parent = parent.Parent;
            }
        }

        return false;
    }
}
