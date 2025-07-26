﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp;

internal static class CSharpUtility
{
    public static bool IsInUnsafeContext(SyntaxNode? node)
    {
        while (node is not null)
        {
            switch (node)
            {
                case UnsafeStatementSyntax:
                    return true;
                case MemberDeclarationSyntax memberDeclarationSyntax:
                {
                    if (memberDeclarationSyntax.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
                        return true;

                    break;
                }
                case LocalFunctionStatementSyntax localFunctionStatement:
                {
                    if (localFunctionStatement.Modifiers.Contains(SyntaxKind.UnsafeKeyword))
                        return true;

                    break;
                }
            }

            node = node.Parent;
        }

        return false;
    }

    public static bool IsNullableReferenceType(
        TypeSyntax type,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        if (type.IsKind(SyntaxKind.NullableType))
        {
            ITypeSymbol? typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

            return typeSymbol?.IsKind(SymbolKind.ErrorType) == false
                && typeSymbol.IsReferenceType;
        }

        return false;
    }

    public static string? GetCountOrLengthPropertyName(
        ExpressionSyntax expression,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        ITypeSymbol? typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

        if (typeSymbol is null)
            return null;

        return SymbolUtility.GetCountOrLengthPropertyName(typeSymbol, semanticModel, expression.SpanStart);
    }

    public static bool IsNamespaceInScope(
        SyntaxNode node,
        INamespaceSymbol namespaceSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        if (namespaceSymbol is null)
            throw new ArgumentNullException(nameof(namespaceSymbol));

        if (semanticModel is null)
            throw new ArgumentNullException(nameof(semanticModel));

        foreach (SyntaxNode ancestor in node.Ancestors())
        {
            switch (ancestor)
            {
                case NamespaceDeclarationSyntax namespaceDeclaration:
                {
                    if (IsNamespace(namespaceSymbol, namespaceDeclaration.Name, semanticModel, cancellationToken)
                        || IsNamespace(namespaceSymbol, namespaceDeclaration.Usings, semanticModel, cancellationToken))
                    {
                        return true;
                    }

                    break;
                }
                case CompilationUnitSyntax compilationUnit:
                {
                    if (IsNamespace(namespaceSymbol, compilationUnit.Usings, semanticModel, cancellationToken))
                        return true;

                    break;
                }
            }
        }

        return false;
    }

    private static bool IsNamespace(
        INamespaceSymbol namespaceSymbol,
        SyntaxList<UsingDirectiveSyntax> usings,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        foreach (UsingDirectiveSyntax usingDirective in usings)
        {
            if (!usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)
                && usingDirective.Alias is null
                && IsNamespace(namespaceSymbol, usingDirective.Name, semanticModel, cancellationToken))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsNamespace(
        INamespaceSymbol namespaceSymbol,
        NameSyntax? name,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        if (name is not null)
        {
            ISymbol? symbol = semanticModel.GetSymbol(name, cancellationToken);

            if (symbol?.Kind == SymbolKind.Namespace)
            {
                string namespaceText = namespaceSymbol.ToString();

                if (string.Equals(namespaceText, symbol.ToString(), StringComparison.Ordinal))
                {
                    return true;
                }
                else if (name.IsParentKind(SyntaxKind.NamespaceDeclaration))
                {
                    INamespaceSymbol containingNamespace = symbol.ContainingNamespace;

                    while (containingNamespace is not null)
                    {
                        if (string.Equals(namespaceText, containingNamespace.ToString(), StringComparison.Ordinal))
                            return true;

                        containingNamespace = containingNamespace.ContainingNamespace;
                    }
                }
            }
        }

        return false;
    }

    public static bool IsStaticClassInScope(
        SyntaxNode node,
        INamedTypeSymbol staticClassSymbol,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        if (staticClassSymbol is null)
            throw new ArgumentNullException(nameof(staticClassSymbol));

        if (semanticModel is null)
            throw new ArgumentNullException(nameof(semanticModel));

        foreach (SyntaxNode ancestor in node.Ancestors())
        {
            foreach (UsingDirectiveSyntax usingDirective in SyntaxInfo.UsingDirectiveListInfo(ancestor).Usings)
            {
                if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                {
                    NameSyntax? name = usingDirective.Name;

                    if (name is not null
                        && SymbolEqualityComparer.Default.Equals(staticClassSymbol, semanticModel.GetSymbol(name, cancellationToken)))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool IsEmptyStringExpression(
        ExpressionSyntax expression,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        if (expression is null)
            throw new ArgumentNullException(nameof(expression));

        if (semanticModel is null)
            throw new ArgumentNullException(nameof(semanticModel));

        SyntaxKind kind = expression.Kind();

        if (kind == SyntaxKind.StringLiteralExpression)
        {
            return ((LiteralExpressionSyntax)expression).Token.ValueText.Length == 0;
        }
        else if (kind == SyntaxKind.InterpolatedStringExpression)
        {
            return !((InterpolatedStringExpressionSyntax)expression).Contents.Any();
        }
        else if (kind == SyntaxKind.SimpleMemberAccessExpression)
        {
            var memberAccess = (MemberAccessExpressionSyntax)expression;

            if (memberAccess.Name?.Identifier.ValueText == "Empty")
            {
                ISymbol? symbol = semanticModel.GetSymbol(memberAccess, cancellationToken);

                if (symbol?.Kind == SymbolKind.Field)
                {
                    var fieldSymbol = (IFieldSymbol)symbol;

                    if (SymbolUtility.IsPublicStaticReadOnly(fieldSymbol, "Empty")
                        && fieldSymbol.ContainingType?.SpecialType == SpecialType.System_String
                        && fieldSymbol.Type.IsString())
                    {
                        return true;
                    }
                }
            }
        }

        Optional<object?> optional = semanticModel.GetConstantValue(expression, cancellationToken);

        if (optional.HasValue)
        {
            var value = optional.Value as string;

            return value?.Length == 0;
        }

        return false;
    }

    public static bool IsNameOfExpression(
        SyntaxNode node,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        return node.IsKind(SyntaxKind.InvocationExpression)
            && IsNameOfExpression((InvocationExpressionSyntax)node, semanticModel, cancellationToken);
    }

    public static bool IsNameOfExpression(
        InvocationExpressionSyntax invocationExpression,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        ExpressionSyntax expression = invocationExpression.Expression;

        if (expression?.Kind() == SyntaxKind.IdentifierName)
        {
            var identifierName = (IdentifierNameSyntax)expression;

            if (string.Equals(identifierName.Identifier.ValueText, "nameof", StringComparison.Ordinal)
                && semanticModel.GetSymbol(invocationExpression, cancellationToken) is null)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsStringConcatenation(BinaryExpressionSyntax addExpression, SemanticModel semanticModel, CancellationToken cancellationToken = default)
    {
        return addExpression.Kind() == SyntaxKind.AddExpression
            && SymbolUtility.IsStringAdditionOperator(semanticModel.GetMethodSymbol(addExpression, cancellationToken));
    }

    public static bool IsStringLiteralConcatenation(BinaryExpressionSyntax binaryExpression)
    {
        if (binaryExpression?.Kind() != SyntaxKind.AddExpression)
            return false;

        while (true)
        {
            if (binaryExpression.Right?.WalkDownParentheses().Kind() != SyntaxKind.StringLiteralExpression)
                return false;

            ExpressionSyntax? left = binaryExpression.Left?.WalkDownParentheses();

            switch (left?.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                {
                    return true;
                }
                case SyntaxKind.AddExpression:
                {
                    binaryExpression = (BinaryExpressionSyntax)left;
                    break;
                }
                default:
                {
                    return false;
                }
            }
        }
    }

    public static bool IsStringExpression(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        if (expression is null)
            return false;

        if (expression
            .WalkDownParentheses()
            .Kind()
            .Is(SyntaxKind.StringLiteralExpression, SyntaxKind.InterpolatedStringExpression))
        {
            return true;
        }

        return semanticModel.GetTypeInfo(expression, cancellationToken)
            .Type?
            .SpecialType == SpecialType.System_String;
    }

    public static SyntaxToken GetIdentifier(SyntaxNode node)
    {
        switch (node.Kind())
        {
            case SyntaxKind.ClassDeclaration:
                return ((ClassDeclarationSyntax)node).Identifier;
            case SyntaxKind.StructDeclaration:
                return ((StructDeclarationSyntax)node).Identifier;
            case SyntaxKind.InterfaceDeclaration:
                return ((InterfaceDeclarationSyntax)node).Identifier;
            case SyntaxKind.EnumDeclaration:
                return ((EnumDeclarationSyntax)node).Identifier;
            case SyntaxKind.DelegateDeclaration:
                return ((DelegateDeclarationSyntax)node).Identifier;
            case SyntaxKind.MethodDeclaration:
                return ((MethodDeclarationSyntax)node).Identifier;
            case SyntaxKind.ConstructorDeclaration:
                return ((ConstructorDeclarationSyntax)node).Identifier;
            case SyntaxKind.PropertyDeclaration:
                return ((PropertyDeclarationSyntax)node).Identifier;
            case SyntaxKind.IndexerDeclaration:
                return ((IndexerDeclarationSyntax)node).ThisKeyword;
            case SyntaxKind.EventDeclaration:
                return ((EventDeclarationSyntax)node).Identifier;
            case SyntaxKind.EventFieldDeclaration:
                return ((EventFieldDeclarationSyntax)node).Declaration?.Variables.FirstOrDefault()?.Identifier ?? default;
            case SyntaxKind.FieldDeclaration:
                return ((FieldDeclarationSyntax)node).Declaration?.Variables.FirstOrDefault()?.Identifier ?? default;
            case SyntaxKind.VariableDeclarator:
                return ((VariableDeclaratorSyntax)node).Identifier;
            case SyntaxKind.RecordDeclaration:
#if ROSLYN_4_0
            case SyntaxKind.RecordStructDeclaration:
                return ((RecordDeclarationSyntax)node).Identifier;
#endif
            case SyntaxKind.Parameter:
                return ((ParameterSyntax)node).Identifier;
            case SyntaxKind.TypeParameter:
                return ((TypeParameterSyntax)node).Identifier;
            case SyntaxKind.EnumMemberDeclaration:
                return ((EnumMemberDeclarationSyntax)node).Identifier;
            case SyntaxKind.ForEachStatement:
                return ((ForEachStatementSyntax)node).Identifier;
            case SyntaxKind.CatchDeclaration:
                return ((CatchDeclarationSyntax)node).Identifier;
            case SyntaxKind.SingleVariableDesignation:
                return ((SingleVariableDesignationSyntax)node).Identifier;
            case SyntaxKind.LocalFunctionStatement:
                return ((LocalFunctionStatementSyntax)node).Identifier;
        }

        return default;
    }

    public static bool IsPartOfExpressionThatMustBeConstant(ExpressionSyntax expression)
    {
        for (SyntaxNode? parent = expression.Parent; parent is not null; parent = parent.Parent)
        {
            switch (parent.Kind())
            {
                case SyntaxKind.AttributeArgument:
                case SyntaxKind.Parameter:
                case SyntaxKind.CaseSwitchLabel:
                case SyntaxKind.ConstantPattern:
                    return true;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)parent).Modifiers.Contains(SyntaxKind.ConstKeyword);
                case SyntaxKind.LocalDeclarationStatement:
                    return ((LocalDeclarationStatementSyntax)parent).Modifiers.Contains(SyntaxKind.ConstKeyword);
                case SyntaxKind.Block:
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.EmptyStatement:
                case SyntaxKind.LabeledStatement:
                case SyntaxKind.GotoStatement:
                case SyntaxKind.GotoCaseStatement:
                case SyntaxKind.GotoDefaultStatement:
                case SyntaxKind.BreakStatement:
                case SyntaxKind.ContinueStatement:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.YieldBreakStatement:
                case SyntaxKind.ThrowStatement:
                case SyntaxKind.WhileStatement:
                case SyntaxKind.DoStatement:
                case SyntaxKind.ForStatement:
                case SyntaxKind.ForEachStatement:
                case SyntaxKind.ForEachVariableStatement:
                case SyntaxKind.UsingStatement:
                case SyntaxKind.FixedStatement:
                case SyntaxKind.CheckedStatement:
                case SyntaxKind.UncheckedStatement:
                case SyntaxKind.UnsafeStatement:
                case SyntaxKind.LockStatement:
                case SyntaxKind.IfStatement:
                case SyntaxKind.SwitchStatement:
                case SyntaxKind.TryStatement:
                case SyntaxKind.LocalFunctionStatement:
                case SyntaxKind.GlobalStatement:
                case SyntaxKind.NamespaceDeclaration:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
#if ROSLYN_4_0
                case SyntaxKind.RecordStructDeclaration:
#endif
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.EnumMemberDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.UnknownAccessorDeclaration:
                case SyntaxKind.IncompleteMember:
                case SyntaxKind.ArrowExpressionClause:
                case SyntaxKind.ThisConstructorInitializer:
                case SyntaxKind.BaseConstructorInitializer:
                case SyntaxKind.SwitchExpressionArm:
                case SyntaxKind.AnonymousObjectMemberDeclarator:
                    return false;
#if DEBUG
                default:
                {
                    if (parent is ExpressionSyntax)
                        break;

                    switch (parent.Kind())
                    {
                        case SyntaxKind.Argument:
                        case SyntaxKind.ArgumentList:
                        case SyntaxKind.BracketedArgumentList:
                        case SyntaxKind.EqualsValueClause:
                        case SyntaxKind.Interpolation:
                        case SyntaxKind.VariableDeclaration:
                        case SyntaxKind.VariableDeclarator:
                        {
                            break;
                        }
                        default:
                        {
                            SyntaxDebug.Fail(parent);
                            break;
                        }
                    }

                    break;
                }
#endif
            }
        }

        return false;
    }

    public static ArrowExpressionClauseSyntax? GetExpressionBody(SyntaxNode node)
    {
        switch (node.Kind())
        {
            case SyntaxKind.MethodDeclaration:
                return ((MethodDeclarationSyntax)node).ExpressionBody;
            case SyntaxKind.PropertyDeclaration:
                return ((PropertyDeclarationSyntax)node).ExpressionBody;
            case SyntaxKind.IndexerDeclaration:
                return ((IndexerDeclarationSyntax)node).ExpressionBody;
            case SyntaxKind.OperatorDeclaration:
                return ((OperatorDeclarationSyntax)node).ExpressionBody;
            case SyntaxKind.ConversionOperatorDeclaration:
                return ((ConversionOperatorDeclarationSyntax)node).ExpressionBody;
            case SyntaxKind.ConstructorDeclaration:
                return ((ConstructorDeclarationSyntax)node).ExpressionBody;
            case SyntaxKind.DestructorDeclaration:
                return ((DestructorDeclarationSyntax)node).ExpressionBody;
            case SyntaxKind.GetAccessorDeclaration:
            case SyntaxKind.SetAccessorDeclaration:
            case SyntaxKind.AddAccessorDeclaration:
            case SyntaxKind.RemoveAccessorDeclaration:
                return ((AccessorDeclarationSyntax)node).ExpressionBody;
            case SyntaxKind.LocalFunctionStatement:
                return ((LocalFunctionStatementSyntax)node).ExpressionBody;
        }

        SyntaxDebug.Assert(!CSharpFacts.CanHaveExpressionBody(node.Kind()), node);

        return null;
    }

    public static bool IsConditionallyAccessed(SyntaxNode node)
    {
        SyntaxNode prev = node;

        for (SyntaxNode? parent = node.Parent; parent is not null; parent = parent.Parent)
        {
            switch (parent.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.ElementAccessExpression:
                case SyntaxKind.InvocationExpression:
                {
                    break;
                }
                case SyntaxKind.ConditionalAccessExpression:
                {
                    if (((ConditionalAccessExpressionSyntax)parent).WhenNotNull == prev)
                        return true;

                    break;
                }
                default:
                {
                    return false;
                }
            }

            prev = parent;
        }

        return false;
    }

    public static ExpressionSyntax GetTopmostExpressionInCallChain(ExpressionSyntax expression)
    {
        return (ExpressionSyntax)expression.WalkUp(f => f.IsKind(
            SyntaxKind.ConditionalAccessExpression,
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxKind.ElementAccessExpression,
            SyntaxKind.MemberBindingExpression,
            SyntaxKind.InvocationExpression));
    }

    internal static IFieldSymbol? FindEnumDefaultField(INamedTypeSymbol enumSymbol)
    {
        if (enumSymbol is null)
            throw new ArgumentNullException(nameof(enumSymbol));

        if (enumSymbol.EnumUnderlyingType is null)
            throw new ArgumentException($"'{enumSymbol}' is not an enumeration.", nameof(enumSymbol));

        foreach (ISymbol symbol in enumSymbol.GetMembers())
        {
            if (symbol.Kind == SymbolKind.Field)
            {
                var fieldSymbol = (IFieldSymbol)symbol;

                if (fieldSymbol.HasConstantValue
                    && SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, enumSymbol) == 0)
                {
                    return fieldSymbol;
                }
            }
        }

        return default;
    }

    public static TypeSyntax? GetTypeOrReturnType(SyntaxNode node)
    {
        switch (node.Kind())
        {
            case SyntaxKind.MethodDeclaration:
                return ((MethodDeclarationSyntax)node).ReturnType;
            case SyntaxKind.OperatorDeclaration:
                return ((OperatorDeclarationSyntax)node).ReturnType;
            case SyntaxKind.ConversionOperatorDeclaration:
                return ((ConversionOperatorDeclarationSyntax)node).Type;
            case SyntaxKind.PropertyDeclaration:
                return ((PropertyDeclarationSyntax)node).Type;
            case SyntaxKind.IndexerDeclaration:
                return ((IndexerDeclarationSyntax)node).Type;
            case SyntaxKind.FieldDeclaration:
                return ((FieldDeclarationSyntax)node).Declaration.Type;
            case SyntaxKind.EventDeclaration:
                return ((EventDeclarationSyntax)node).Type;
            case SyntaxKind.EventFieldDeclaration:
                return ((EventFieldDeclarationSyntax)node).Declaration.Type;
            case SyntaxKind.LocalFunctionStatement:
                return ((LocalFunctionStatementSyntax)node).ReturnType;
            default:
                return null;
        }
    }

    public static bool ContainsOutArgumentWithLocalOrParameter(
        ExpressionSyntax expression,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        foreach (SyntaxNode node in expression.DescendantNodes())
        {
            if (node is ArgumentSyntax argument
                && argument.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword))
            {
                ExpressionSyntax argumentExpression = argument.Expression;

                if (argumentExpression?.IsMissing == false
                    && semanticModel.GetSymbol(argumentExpression, cancellationToken)?.IsKind(SymbolKind.Local, SymbolKind.Parameter) == true)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static SeparatedSyntaxList<ParameterSyntax> GetParameters(SyntaxNode declaration)
    {
        return GetParameterList(declaration)?.Parameters ?? default;
    }

    public static BaseParameterListSyntax? GetParameterList(SyntaxNode declaration)
    {
        switch (declaration.Kind())
        {
            case SyntaxKind.MethodDeclaration:
                return ((MethodDeclarationSyntax)declaration).ParameterList;
            case SyntaxKind.ConstructorDeclaration:
                return ((ConstructorDeclarationSyntax)declaration).ParameterList;
            case SyntaxKind.OperatorDeclaration:
                return ((OperatorDeclarationSyntax)declaration).ParameterList;
            case SyntaxKind.ConversionOperatorDeclaration:
                return ((ConversionOperatorDeclarationSyntax)declaration).ParameterList;
            case SyntaxKind.DelegateDeclaration:
                return ((DelegateDeclarationSyntax)declaration).ParameterList;
            case SyntaxKind.IndexerDeclaration:
                return ((IndexerDeclarationSyntax)declaration).ParameterList;
            case SyntaxKind.LocalFunctionStatement:
                return ((LocalFunctionStatementSyntax)declaration).ParameterList;
            case SyntaxKind.RecordDeclaration:
#if ROSLYN_4_0
            case SyntaxKind.RecordStructDeclaration:
#endif
                return ((RecordDeclarationSyntax)declaration).ParameterList;
#if ROSLYN_4_7
            case SyntaxKind.ClassDeclaration:
            case SyntaxKind.StructDeclaration:
            case SyntaxKind.InterfaceDeclaration:
                return ((TypeDeclarationSyntax)declaration).ParameterList;
#endif
            default:
                return null;
        }
    }

    public static SeparatedSyntaxList<TypeParameterSyntax> GetTypeParameters(SyntaxNode declaration)
    {
        return GetTypeParameterList(declaration)?.Parameters ?? default;
    }

    public static TypeParameterListSyntax? GetTypeParameterList(SyntaxNode declaration)
    {
        switch (declaration.Kind())
        {
            case SyntaxKind.ClassDeclaration:
                return ((ClassDeclarationSyntax)declaration).TypeParameterList;
            case SyntaxKind.InterfaceDeclaration:
                return ((InterfaceDeclarationSyntax)declaration).TypeParameterList;
            case SyntaxKind.StructDeclaration:
                return ((StructDeclarationSyntax)declaration).TypeParameterList;
            case SyntaxKind.MethodDeclaration:
                return ((MethodDeclarationSyntax)declaration).TypeParameterList;
            case SyntaxKind.DelegateDeclaration:
                return ((DelegateDeclarationSyntax)declaration).TypeParameterList;
            case SyntaxKind.LocalFunctionStatement:
                return ((LocalFunctionStatementSyntax)declaration).TypeParameterList;
            case SyntaxKind.RecordDeclaration:
#if ROSLYN_4_0
            case SyntaxKind.RecordStructDeclaration:
#endif
                return ((RecordDeclarationSyntax)declaration).TypeParameterList;
            default:
                return null;
        }
    }

    public static bool IsSymbolObsolete(ISymbol symbol)
    {
        return symbol.HasAttribute(MetadataNames.System_ObsoleteAttribute);
    }
}
