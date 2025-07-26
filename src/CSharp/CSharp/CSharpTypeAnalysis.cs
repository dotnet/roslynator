﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp;

internal static class CSharpTypeAnalysis
{
    public static TypeAnalysis AnalyzeType(
        VariableDeclarationSyntax variableDeclaration,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        TypeSyntax type = variableDeclaration.Type;

        Debug.Assert(type is not null);

        if (type is null)
            return default;

        SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;

        Debug.Assert(variables.Any());

        if (!variables.Any())
            return default;

        if (variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.EventFieldDeclaration, SyntaxKind.FixedStatement))
            return default;

        ExpressionSyntax? expression = variables[0].Initializer?.Value?.WalkDownParentheses();

        if (expression is null)
            return default;
#if ROSLYN_4_7
        if (expression.IsKind(SyntaxKind.CollectionExpression))
            return default;
#endif
        ITypeSymbol? typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

        if (typeSymbol is null)
            return default;

        SymbolKind kind = typeSymbol.Kind;

        if (kind == SymbolKind.ErrorType)
            return default;

        if (kind == SymbolKind.DynamicType)
            return new TypeAnalysis(typeSymbol, TypeAnalysisFlags.Dynamic);

        var flags = TypeAnalysisFlags.None;

        if (type.IsVar)
        {
            flags |= TypeAnalysisFlags.Implicit;

            if (typeSymbol.SupportsExplicitDeclaration())
                flags |= TypeAnalysisFlags.SupportsExplicit;
        }
        else
        {
            flags |= TypeAnalysisFlags.Explicit;

            if (variables.Count == 1
                && (variableDeclaration.Parent as LocalDeclarationStatementSyntax)?.IsConst != true
                && !expression.IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultLiteralExpression))
            {
                flags |= TypeAnalysisFlags.SupportsImplicit;
            }
        }

        if (IsTypeObvious(expression, typeSymbol, includeNullability: false, semanticModel, cancellationToken))
            flags |= TypeAnalysisFlags.TypeObvious;

        return new TypeAnalysis(typeSymbol, flags);
    }

    public static bool IsImplicitThatCanBeExplicit(
        VariableDeclarationSyntax variableDeclaration,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        return IsImplicitThatCanBeExplicit(variableDeclaration, semanticModel, TypeAppearance.None, cancellationToken);
    }

    public static bool IsImplicitThatCanBeExplicit(
        VariableDeclarationSyntax variableDeclaration,
        SemanticModel semanticModel,
        TypeAppearance typeAppearance,
        CancellationToken cancellationToken = default)
    {
        TypeSyntax type = variableDeclaration.Type;

        Debug.Assert(type is not null);

        if (type is null)
            return false;

        if (!type.IsVar)
            return false;

        if (variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.EventFieldDeclaration, SyntaxKind.FixedStatement))
            return false;

        Debug.Assert(variableDeclaration.Variables.Any());

        ExpressionSyntax? expression = variableDeclaration
            .Variables
            .FirstOrDefault()?
            .Initializer?
            .Value?
            .WalkDownParentheses();

        if (expression is null)
            return false;

        ITypeSymbol? typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

        if (typeSymbol is null)
            return false;

        if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
            return false;

        if (!typeSymbol.SupportsExplicitDeclaration())
            return false;

        switch (typeAppearance)
        {
            case TypeAppearance.Obvious:
                return IsTypeObvious(expression, typeSymbol, includeNullability: false, semanticModel, cancellationToken);
            case TypeAppearance.NotObvious:
                return !IsTypeObvious(expression, typeSymbol, includeNullability: false, semanticModel, cancellationToken);
        }

        Debug.Assert(typeAppearance == TypeAppearance.None, typeAppearance.ToString());

        return true;
    }

    public static bool IsExplicitThatCanBeImplicit(
        VariableDeclarationSyntax variableDeclaration,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        return IsExplicitThatCanBeImplicit(variableDeclaration, semanticModel, TypeAppearance.None, cancellationToken);
    }

    public static bool IsExplicitThatCanBeImplicit(
        VariableDeclarationSyntax variableDeclaration,
        SemanticModel semanticModel,
        TypeAppearance typeAppearance,
        CancellationToken cancellationToken = default)
    {
        TypeSyntax type = variableDeclaration.Type;

        Debug.Assert(type is not null);

        if (type is null)
            return false;

        if (type.IsVar)
            return false;

        switch (variableDeclaration.Parent?.Kind())
        {
            case SyntaxKind.FieldDeclaration:
            case SyntaxKind.EventFieldDeclaration:
            case SyntaxKind.FixedStatement:
            {
                return false;
            }
            case SyntaxKind.LocalDeclarationStatement:
            {
                if (((LocalDeclarationStatementSyntax)variableDeclaration.Parent).IsConst)
                    return false;

                break;
            }
            case null:
            {
                return false;
            }
        }

        Debug.Assert(variableDeclaration.Variables.Any());

        ExpressionSyntax? expression = variableDeclaration
            .Variables
            .SingleOrDefault(shouldThrow: false)?
            .Initializer?
            .Value?
            .WalkDownParentheses();

        if (expression is null)
            return false;

        if (expression.IsKind(
            SyntaxKind.NullLiteralExpression,
            SyntaxKind.DefaultLiteralExpression,
#if ROSLYN_4_7
            SyntaxKind.CollectionExpression,
#endif
            SyntaxKind.ImplicitObjectCreationExpression))
        {
            return false;
        }

        if (expression.IsKind(SyntaxKind.SuppressNullableWarningExpression)
            && expression is PostfixUnaryExpressionSyntax postfixUnary
            && postfixUnary.Operand.IsKind(SyntaxKind.DefaultLiteralExpression))
        {
            return false;
        }

        ITypeSymbol? typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

        if (typeSymbol is null)
            return false;

        if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
            return false;

        if (expression.IsKind(SyntaxKind.StackAllocArrayCreationExpression)
            && typeSymbol.HasMetadataName(MetadataNames.System_Span_T))
        {
            return false;
        }

        ITypeSymbol? expressionType = semanticModel.GetTypeSymbol(expression, cancellationToken);

        if (!SymbolEqualityComparer.Default.Equals(typeSymbol, expressionType)
            || (type.IsKind(SyntaxKind.NullableType)
                && typeSymbol.IsReferenceType
                && expressionType.NullableAnnotation == NullableAnnotation.NotAnnotated))
        {
            return false;
        }

        switch (typeAppearance)
        {
            case TypeAppearance.Obvious:
                return IsTypeObvious(expression, typeSymbol, includeNullability: true, semanticModel, cancellationToken);
            case TypeAppearance.NotObvious:
                return !IsTypeObvious(expression, typeSymbol, includeNullability: true, semanticModel, cancellationToken);
            case TypeAppearance.None:
                return GetEqualityComparer(includeNullability: true).Equals(
                    typeSymbol,
                    semanticModel.GetTypeSymbol(expression, cancellationToken));
            default:
                throw new InvalidOperationException($"Unknow enum value '{typeAppearance}'");
        }
    }

    public static bool IsTypeObvious(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        return IsTypeObvious(expression, typeSymbol: null, includeNullability: false, semanticModel, cancellationToken);
    }

    public static bool IsTypeObvious(ExpressionSyntax expression, ITypeSymbol? typeSymbol, bool includeNullability, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        switch (expression.Kind())
        {
            case SyntaxKind.StringLiteralExpression:
            case SyntaxKind.CharacterLiteralExpression:
            case SyntaxKind.TrueLiteralExpression:
            case SyntaxKind.FalseLiteralExpression:
            case SyntaxKind.ThisExpression:
            case SyntaxKind.ObjectCreationExpression:
            case SyntaxKind.ArrayCreationExpression:
            case SyntaxKind.CastExpression:
            case SyntaxKind.AsExpression:
            case SyntaxKind.DefaultExpression:
            {
                return typeSymbol is null
                    || GetEqualityComparer(includeNullability).Equals(
                        typeSymbol,
                        semanticModel.GetTypeSymbol(expression, cancellationToken));
            }
            case SyntaxKind.ImplicitArrayCreationExpression:
            {
                var implicitArrayCreation = (ImplicitArrayCreationExpressionSyntax)expression;

                SeparatedSyntaxList<ExpressionSyntax> expressions = implicitArrayCreation.Initializer?.Expressions ?? default;

                if (!expressions.Any())
                    return false;

                if (typeSymbol is not null)
                {
                    var arrayTypeSymbol = semanticModel.GetTypeSymbol(implicitArrayCreation, cancellationToken) as IArrayTypeSymbol;

                    if (!GetEqualityComparer(includeNullability).Equals(typeSymbol, arrayTypeSymbol))
                        return false;

                    typeSymbol = arrayTypeSymbol.ElementType;
                }

                foreach (ExpressionSyntax expression2 in expressions)
                {
                    if (!IsTypeObvious(expression2, typeSymbol, includeNullability, semanticModel, cancellationToken))
                        return false;
                }

                return true;
            }
            case SyntaxKind.SimpleMemberAccessExpression:
            {
                ISymbol? symbol = semanticModel.GetSymbol(expression, cancellationToken);

                return symbol?.Kind == SymbolKind.Field
                    && symbol.ContainingType?.TypeKind == TypeKind.Enum;
            }
            case SyntaxKind.InvocationExpression:
            {
                if (typeSymbol is not null)
                {
                    var invocationExpression = (InvocationExpressionSyntax)expression;
                    if (invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                    {
                        ISymbol? symbol = semanticModel.GetSymbol(expression, cancellationToken);

                        if (symbol?.IsStatic == true
                            && string.Equals(symbol.Name, "Parse", StringComparison.Ordinal)
                            && GetEqualityComparer(includeNullability).Equals(
                                ((IMethodSymbol)symbol).ReturnType,
                                typeSymbol))
                        {
                            var simpleMemberAccess = (MemberAccessExpressionSyntax)invocationExpression.Expression;

                            ISymbol? symbol2 = semanticModel.GetSymbol(simpleMemberAccess.Expression, cancellationToken);

                            if (SymbolEqualityComparer.Default.Equals(symbol2, typeSymbol)
                                && semanticModel.GetAliasInfo(simpleMemberAccess.Expression, cancellationToken) is null)
                            {
                                return true;
                            }
                        }
                    }
                }

                break;
            }
        }

        return false;
    }

    private static SymbolEqualityComparer GetEqualityComparer(bool includeNullability)
    {
        return (includeNullability) ? SymbolEqualityComparer.IncludeNullability : SymbolEqualityComparer.Default;
    }

    public static TypeAnalysis AnalyzeType(
        DeclarationExpressionSyntax declarationExpression,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        TypeSyntax type = declarationExpression.Type;

        if (type is null)
            return default;

        if (declarationExpression.Designation is not SingleVariableDesignationSyntax singleVariableDesignation)
            return default;

        if (semanticModel.GetDeclaredSymbol(singleVariableDesignation, cancellationToken) is not ILocalSymbol localSymbol)
            return default;

        ITypeSymbol typeSymbol = localSymbol.Type;

        Debug.Assert(typeSymbol is not null);

        if (typeSymbol is null)
            return default;

        SymbolKind kind = typeSymbol.Kind;

        if (kind == SymbolKind.ErrorType)
            return default;

        if (kind == SymbolKind.DynamicType)
            return new TypeAnalysis(typeSymbol, TypeAnalysisFlags.Dynamic);

        var flags = TypeAnalysisFlags.None;

        if (type.IsVar)
        {
            flags |= TypeAnalysisFlags.Implicit;

            if (typeSymbol.SupportsExplicitDeclaration())
                flags |= TypeAnalysisFlags.SupportsExplicit;
        }
        else
        {
            flags |= TypeAnalysisFlags.Explicit;
            flags |= TypeAnalysisFlags.SupportsImplicit;
        }

        return new TypeAnalysis(typeSymbol, flags);
    }

    public static bool IsImplicitThatCanBeExplicit(
        DeclarationExpressionSyntax declarationExpression,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        return IsImplicitThatCanBeExplicit(declarationExpression, semanticModel, TypeAppearance.None, cancellationToken);
    }

    public static bool IsImplicitThatCanBeExplicit(
        DeclarationExpressionSyntax declarationExpression,
        SemanticModel semanticModel,
        TypeAppearance typeAppearance,
        CancellationToken cancellationToken = default)
    {
        TypeSyntax type = declarationExpression.Type;

        Debug.Assert(type is not null);

        if (type is null)
            return false;

        if (!type.IsVar)
            return false;

        if (declarationExpression.Parent is AssignmentExpressionSyntax assignment)
        {
            ITypeSymbol? typeSymbol = semanticModel.GetTypeSymbol(assignment.Right, cancellationToken);

            return typeSymbol?.SupportsExplicitDeclaration() == true;
        }

        switch (declarationExpression.Designation)
        {
            case SingleVariableDesignationSyntax singleVariableDesignation:
            {
                return IsLocalThatSupportsExplicitDeclaration(singleVariableDesignation);
            }
            case DiscardDesignationSyntax discardDesignation:
            {
                return IsLocalThatSupportsExplicitDeclaration(discardDesignation);
            }
            case ParenthesizedVariableDesignationSyntax parenthesizedVariableDesignation:
            {
                foreach (VariableDesignationSyntax variableDesignation in parenthesizedVariableDesignation.Variables)
                {
                    if (variableDesignation is not SingleVariableDesignationSyntax)
                        return false;

                    if (!IsLocalThatSupportsExplicitDeclaration(variableDesignation))
                        return false;
                }

                if (declarationExpression.Parent is AssignmentExpressionSyntax assignmentExpression
                    && declarationExpression == assignmentExpression.Left)
                {
                    ExpressionSyntax expression = assignmentExpression.Right;

                    if (expression is not null)
                    {
                        switch (typeAppearance)
                        {
                            case TypeAppearance.Obvious:
                                return !IsTypeObvious(expression, semanticModel, cancellationToken);
                            case TypeAppearance.NotObvious:
                                return IsTypeObvious(expression, semanticModel, cancellationToken);
                        }

                        Debug.Assert(typeAppearance == TypeAppearance.None, typeAppearance.ToString());
                    }
                }

                return true;
            }
            default:
            {
                SyntaxDebug.Fail(declarationExpression.Designation);
                return false;
            }
        }

        bool IsLocalThatSupportsExplicitDeclaration(VariableDesignationSyntax variableDesignation)
        {
            if (semanticModel.GetDeclaredSymbol(variableDesignation, cancellationToken) is not ILocalSymbol localSymbol)
                return false;

            ITypeSymbol typeSymbol = localSymbol.Type;

            if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
                return false;

            return typeSymbol.SupportsExplicitDeclaration();
        }
    }

    public static bool IsExplicitThatCanBeImplicit(
        DeclarationExpressionSyntax declarationExpression,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        TypeSyntax type = declarationExpression.Type;

        if (type is null)
            return false;

        if (type.IsVar)
            return false;

        if (declarationExpression.Designation is not SingleVariableDesignationSyntax singleVariableDesignation)
            return false;

        if (semanticModel.GetDeclaredSymbol(singleVariableDesignation, cancellationToken) is not ILocalSymbol localSymbol)
            return false;

        ITypeSymbol typeSymbol = localSymbol.Type;

        Debug.Assert(typeSymbol is not null);

        if (typeSymbol is null)
            return false;

        if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
            return false;

        if (declarationExpression.Parent is ArgumentSyntax argument)
            return AnalyzeArgument(argument, semanticModel, cancellationToken);

        return true;

        static bool AnalyzeArgument(ArgumentSyntax argument, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            IParameterSymbol? parameterSymbol = semanticModel.DetermineParameter(argument, cancellationToken: cancellationToken);

            if (parameterSymbol is null)
                return false;

            if (SymbolEqualityComparer.Default.Equals(parameterSymbol.Type, parameterSymbol.OriginalDefinition.Type))
                return true;

            if (parameterSymbol.ContainingSymbol is IMethodSymbol methodSymbol)
            {
                ImmutableArray<ITypeSymbol> typeParameterList = methodSymbol.TypeArguments;

                ITypeParameterSymbol? typeParameterSymbol = null;
                for (int i = 0; i < typeParameterList.Length; i++)
                {
                    if (SymbolEqualityComparer.Default.Equals(typeParameterList[i], parameterSymbol.Type))
                    {
                        typeParameterSymbol = methodSymbol.TypeParameters[i];
                        break;
                    }
                }

                if (typeParameterSymbol is not null
                    && argument.Parent is ArgumentListSyntax argumentList
                    && argumentList.Parent is InvocationExpressionSyntax invocation)
                {
                    switch (invocation.Expression.Kind())
                    {
                        case SyntaxKind.IdentifierName:
                            return false;
                        case SyntaxKind.GenericName:
                            return true;
                        case SyntaxKind.SimpleMemberAccessExpression:
                        {
                            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                            if (memberAccess.Name.IsKind(SyntaxKind.IdentifierName))
                                return false;

                            if (memberAccess.Name.IsKind(SyntaxKind.GenericName))
                                return true;

                            Debug.Fail(memberAccess.Name.Kind().ToString());
                            break;
                        }
                        default:
                        {
                            Debug.Fail(invocation.Expression.Kind().ToString());
                            break;
                        }
                    }
                }
            }

            return false;
        }
    }

    public static bool IsExplicitThatCanBeImplicit(
        TupleExpressionSyntax tupleExpression,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        return IsExplicitThatCanBeImplicit(tupleExpression, semanticModel, TypeAppearance.None, cancellationToken);
    }

    public static bool IsExplicitThatCanBeImplicit(
        TupleExpressionSyntax tupleExpression,
        SemanticModel semanticModel,
        TypeAppearance typeAppearance,
        CancellationToken cancellationToken = default)
    {
        switch (tupleExpression.Parent?.Kind())
        {
            case SyntaxKind.SimpleAssignmentExpression:
            {
                var assignment = (AssignmentExpressionSyntax)tupleExpression.Parent;

                return IsExplicitThatCanBeImplicit(tupleExpression, assignment, typeAppearance, semanticModel, cancellationToken);
            }
            case SyntaxKind.ForEachVariableStatement:
            {
                var forEachStatement = (ForEachVariableStatementSyntax)tupleExpression.Parent;

                return IsExplicitThatCanBeImplicit(tupleExpression, forEachStatement, semanticModel);
            }
#if DEBUG
            case SyntaxKind.Argument:
            case SyntaxKind.ArrayInitializerExpression:
            case SyntaxKind.ArrowExpressionClause:
            case SyntaxKind.CollectionInitializerExpression:
            case SyntaxKind.EqualsValueClause:
            case SyntaxKind.ParenthesizedLambdaExpression:
            case SyntaxKind.ReturnStatement:
            case SyntaxKind.SimpleLambdaExpression:
            case SyntaxKind.SimpleMemberAccessExpression:
            case SyntaxKind.SwitchExpression:
            case SyntaxKind.SwitchExpressionArm:
            case SyntaxKind.YieldReturnStatement:
            case SyntaxKind.ConditionalExpression:
            case SyntaxKind.ComplexElementInitializerExpression:
            {
                SyntaxDebug.Assert(tupleExpression.ContainsDiagnostics || !tupleExpression.Arguments.Any(f => f.Expression.IsKind(SyntaxKind.DeclarationExpression)), tupleExpression);
                return false;
            }
#endif
            case null:
            {
                return false;
            }
            default:
            {
                SyntaxDebug.Fail(tupleExpression.Parent);
                return false;
            }
        }
    }

    private static bool IsExplicitThatCanBeImplicit(
        TupleExpressionSyntax tupleExpression,
        AssignmentExpressionSyntax assignment,
        TypeAppearance typeAppearance,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        ExpressionSyntax expression = assignment.Right.WalkDownParentheses();

        if (expression?.IsMissing != false)
            return false;

        if (expression.IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultLiteralExpression))
            return false;

        ITypeSymbol? tupleTypeSymbol = semanticModel.GetTypeSymbol(tupleExpression, cancellationToken);

        if (tupleTypeSymbol is null)
            return false;

        ITypeSymbol? expressionTypeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

        if (tupleTypeSymbol.IsTupleType
            && expressionTypeSymbol?.IsTupleType == true)
        {
            var tupleNamedTypeSymbol = (INamedTypeSymbol)tupleTypeSymbol;
            var expressionNamedTypeSymbol = (INamedTypeSymbol)expressionTypeSymbol;

            if (!SymbolEqualityComparer.Default.Equals(
                tupleNamedTypeSymbol.TupleUnderlyingType ?? tupleNamedTypeSymbol,
                expressionNamedTypeSymbol.TupleUnderlyingType ?? expressionNamedTypeSymbol))
            {
                return false;
            }
        }
        else if (!SymbolEqualityComparer.Default.Equals(tupleTypeSymbol, expressionTypeSymbol))
        {
            return false;
        }

        foreach (ArgumentSyntax argument in tupleExpression.Arguments)
        {
            if (argument.Expression is not DeclarationExpressionSyntax declarationExpression)
                return false;

            TypeSyntax type = declarationExpression.Type;

            if (type is null)
                return false;

            if (declarationExpression.Designation is not SingleVariableDesignationSyntax singleVariableDesignation)
                return false;

            if (semanticModel.GetDeclaredSymbol(singleVariableDesignation, cancellationToken) is not ILocalSymbol localSymbol)
                return false;

            ITypeSymbol typeSymbol = localSymbol.Type;

            Debug.Assert(typeSymbol is not null);

            if (typeSymbol is null)
                return false;

            if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
                return false;
        }

        switch (typeAppearance)
        {
            case TypeAppearance.Obvious:
                return IsTypeObvious(expression, semanticModel, cancellationToken);
            case TypeAppearance.NotObvious:
                return !IsTypeObvious(expression, semanticModel, cancellationToken);
        }

        Debug.Assert(typeAppearance == TypeAppearance.None, typeAppearance.ToString());

        return true;
    }

    public static TypeAnalysis AnalyzeType(ForEachStatementSyntax forEachStatement, SemanticModel semanticModel)
    {
        TypeSyntax type = forEachStatement.Type;

        Debug.Assert(type is not null);

        if (type is null)
            return default;

        ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

        ITypeSymbol? typeSymbol = info.ElementType;

        if (typeSymbol is null)
            return default;

        SymbolKind kind = typeSymbol.Kind;

        if (kind == SymbolKind.ErrorType)
            return default;

        if (kind == SymbolKind.DynamicType)
            return new TypeAnalysis(typeSymbol, TypeAnalysisFlags.Dynamic);

        var flags = TypeAnalysisFlags.None;

        if (type.IsVar)
        {
            flags |= TypeAnalysisFlags.Implicit;

            if (typeSymbol.SupportsExplicitDeclaration())
                flags |= TypeAnalysisFlags.SupportsExplicit;
        }
        else
        {
            flags |= TypeAnalysisFlags.Explicit;

            if (info.ElementConversion.IsIdentity)
                flags |= TypeAnalysisFlags.SupportsImplicit;
        }

        return new TypeAnalysis(typeSymbol, flags);
    }

    public static TypeAnalysis AnalyzeType(ForEachVariableStatementSyntax forEachStatement, SemanticModel semanticModel)
    {
        var flags = TypeAnalysisFlags.None;

        switch (forEachStatement.Variable)
        {
            case DeclarationExpressionSyntax declarationExpression:
            {
                TypeSyntax type = declarationExpression.Type;

                Debug.Assert(type is not null);

                if (type is null)
                    return default;

                SyntaxDebug.Assert(type.IsVar, type);

                if (type.IsVar)
                    flags |= TypeAnalysisFlags.Implicit;

                break;
            }
            case TupleExpressionSyntax tupleExpression:
            {
                foreach (ArgumentSyntax argument in tupleExpression.Arguments)
                {
                    SyntaxDebug.Assert(argument.Expression.IsKind(SyntaxKind.DeclarationExpression), argument.Expression);

                    if (argument.Expression is DeclarationExpressionSyntax declarationExpression)
                    {
                        TypeSyntax type = declarationExpression.Type;

                        if (type.IsVar)
                        {
                            flags |= TypeAnalysisFlags.Implicit;
                        }
                        else
                        {
                            flags |= TypeAnalysisFlags.Explicit;
                        }
                    }
                }

                break;
            }
            default:
            {
                SyntaxDebug.Fail(forEachStatement.Variable);
                return default;
            }
        }

        ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

        ITypeSymbol? typeSymbol = info.ElementType;

        if (typeSymbol is null)
            return default;

        SymbolKind kind = typeSymbol.Kind;

        if (kind == SymbolKind.ErrorType)
            return default;

        if (kind == SymbolKind.DynamicType)
            return new TypeAnalysis(typeSymbol, TypeAnalysisFlags.Dynamic);

        if ((flags & TypeAnalysisFlags.Implicit) != 0
            && typeSymbol.SupportsExplicitDeclaration())
        {
            flags |= TypeAnalysisFlags.SupportsExplicit;
        }

        if ((flags & TypeAnalysisFlags.Explicit) != 0
            && info.ElementConversion.IsIdentity)
        {
            flags |= TypeAnalysisFlags.SupportsImplicit;
        }

        return new TypeAnalysis(typeSymbol, flags);
    }

    public static bool IsImplicitThatCanBeExplicit(ForEachStatementSyntax forEachStatement, SemanticModel semanticModel)
    {
        TypeSyntax type = forEachStatement.Type;

        Debug.Assert(type is not null);

        if (type is null)
            return false;

        if (!type.IsVar)
            return false;

        ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

        ITypeSymbol? typeSymbol = info.ElementType;

        if (typeSymbol is null)
            return false;

        if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
            return false;

        return typeSymbol.SupportsExplicitDeclaration();
    }

    public static bool IsImplicitThatCanBeExplicit(ForEachVariableStatementSyntax forEachStatement, SemanticModel semanticModel)
    {
        ExpressionSyntax variable = forEachStatement.Variable;

        if (variable is not DeclarationExpressionSyntax declarationExpression)
            return false;

        TypeSyntax type = declarationExpression.Type;

        SyntaxDebug.Assert(type.IsVar, type);

        if (!type.IsVar)
            return false;

        ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

        ITypeSymbol? typeSymbol = info.ElementType;

        if (typeSymbol is null)
            return false;

        if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
            return false;

        return typeSymbol.SupportsExplicitDeclaration();
    }

    public static bool IsExplicitThatCanBeImplicit(ForEachStatementSyntax forEachStatement, SemanticModel semanticModel)
    {
        TypeSyntax type = forEachStatement.Type;

        Debug.Assert(type is not null);

        if (type is null)
            return false;

        if (type.IsVar)
            return false;

        ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

        ITypeSymbol? typeSymbol = info.ElementType;

        if (typeSymbol is null)
            return false;

        if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
            return false;

        return info.ElementConversion.IsIdentity;
    }

    public static bool IsExplicitThatCanBeImplicit(ForEachVariableStatementSyntax forEachStatement, SemanticModel semanticModel)
    {
        ExpressionSyntax variable = forEachStatement.Variable;

        if (variable is not TupleExpressionSyntax tupleExpression)
            return false;

        return IsExplicitThatCanBeImplicit(tupleExpression, forEachStatement, semanticModel);
    }

    private static bool IsExplicitThatCanBeImplicit(
        TupleExpressionSyntax tupleExpression,
        ForEachVariableStatementSyntax forEachStatement,
        SemanticModel semanticModel)
    {
        var isAllVar = true;

        foreach (ArgumentSyntax argument in tupleExpression.Arguments)
        {
            if (argument.Expression is not DeclarationExpressionSyntax declarationExpression)
                return false;

            TypeSyntax type = declarationExpression.Type;

            if (type is null)
                return false;

            if (!type.IsVar)
            {
                isAllVar = false;
                break;
            }
        }

        if (isAllVar)
            return false;

        ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

        ITypeSymbol? typeSymbol = info.ElementType;

        if (typeSymbol is null)
            return false;

        if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
            return false;

        return info.ElementConversion.IsIdentity;
    }
}
