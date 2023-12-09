// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis;

internal class ImplicitOrExpressionArrayCreationAnalysis : ImplicitOrExplicitCreationAnalysis
{
    public static ImplicitOrExpressionArrayCreationAnalysis Instance { get; } = new();

    public override void AnalyzeExplicitCreation(ref SyntaxNodeAnalysisContext context)
    {
        if (context.Node.ContainsDiagnostics)
            return;

        TypeStyle style = GetTypeStyle(ref context);

        if (style != TypeStyle.Implicit
            && style != TypeStyle.ImplicitWhenTypeIsObvious)
        {
            return;
        }

        var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;

        ArrayTypeSyntax arrayType = arrayCreation.Type;

        if (arrayType.ContainsDirectives)
            return;

        SeparatedSyntaxList<ExpressionSyntax> expressions = arrayCreation.Initializer?.Expressions ?? default;

        if (!expressions.Any())
            return;

        SyntaxNode parent = arrayCreation.Parent;

        switch (parent.Kind())
        {
            case SyntaxKind.EqualsValueClause:
                {
                    parent = parent.Parent;

                    SyntaxDebug.Assert(parent.IsKind(SyntaxKind.VariableDeclarator, SyntaxKind.PropertyDeclaration), parent);

                    if (parent.IsKind(SyntaxKind.VariableDeclarator))
                    {
                        parent = parent.Parent;

                        if (parent is VariableDeclarationSyntax variableDeclaration)
                        {
                            SyntaxDebug.Assert(variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement), variableDeclaration.Parent);

                            if (variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration))
                            {
                                AnalyzeExplicitObvious(ref context);
                            }
                            else if (variableDeclaration.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement))
                            {
                                if (context.UseVarInsteadOfImplicitObjectCreation() != true)
                                {
                                    bool isVar = variableDeclaration.Type.IsVar;
                                    AnalyzeExplicit(ref context, isObvious: !isVar, allowCollectionExpression: !isVar);
                                }
                            }
                        }
                    }
                    else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                    {
                        AnalyzeExplicitObvious(ref context);
                    }

                    break;
                }
            case SyntaxKind.ArrowExpressionClause:
                {
                    TypeSyntax type = DetermineReturnType(parent.Parent);

                    if (type is not null)
                        AnalyzeExplicitObvious(ref context);

                    break;
                }
            case SyntaxKind.ReturnStatement:
                {
                    if (style == TypeStyle.Implicit
                        || IsSingleReturnStatement(parent))
                    {
                        for (SyntaxNode node = parent.Parent; node is not null; node = node.Parent)
                        {
                            if (CSharpFacts.IsAnonymousFunctionExpression(node.Kind()))
                                return;

                            TypeSyntax type = DetermineReturnType(node);

                            if (type is not null)
                            {
                                AnalyzeExplicit(ref context, isObvious: IsSingleReturnStatement(parent));
                                return;
                            }
                        }
                    }

                    break;
                }
            case SyntaxKind.SimpleAssignmentExpression:
            case SyntaxKind.CoalesceAssignmentExpression:
            case SyntaxKind.AddAssignmentExpression:
            case SyntaxKind.SubtractAssignmentExpression:
                {
                    AnalyzeExplicitNotObvious(ref context);
                    break;
                }
            case SyntaxKind.CoalesceExpression:
                {
                    if (style == TypeStyle.Implicit)
                    {
                        AnalyzeExplicitNotObvious(ref context);
                    }
                    else if (style == TypeStyle.ImplicitWhenTypeIsObvious
                        && parent.IsParentKind(SyntaxKind.EqualsValueClause))
                    {
                        if (parent.Parent.Parent is VariableDeclaratorSyntax variableDeclarator)
                        {
                            if (variableDeclarator.Parent is VariableDeclarationSyntax variableDeclaration
                                && variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration))
                            {
                                AnalyzeExplicitObvious(ref context);
                            }
                        }
                        else if (parent.Parent.Parent is PropertyDeclarationSyntax)
                        {
                            AnalyzeExplicitObvious(ref context);
                        }
                    }

                    break;
                }
        }
    }

    private void AnalyzeExplicitObvious(ref SyntaxNodeAnalysisContext context, bool allowCollectionExpression = true)
    {
        AnalyzeExplicit(ref context, isObvious: true, allowCollectionExpression: allowCollectionExpression);
    }

    private void AnalyzeExplicitNotObvious(ref SyntaxNodeAnalysisContext context, bool allowCollectionExpression = true)
    {
        AnalyzeExplicit(ref context, isObvious: false, allowCollectionExpression: allowCollectionExpression);
    }

    private bool AnalyzeExplicit(ref SyntaxNodeAnalysisContext context, bool isObvious, bool allowCollectionExpression = true)
    {
        TypeStyle style = GetTypeStyle(ref context);

        if (style == TypeStyle.Implicit)
        {
            if (allowCollectionExpression
                && UseCollectionExpression(ref context))
            {
                ReportExplicitToCollectionExpression(ref context);
                return true;
            }
            else
            {
                ReportExplicitToImplicit(ref context);
                return true;
            }
        }
        else if (style == TypeStyle.ImplicitWhenTypeIsObvious)
        {
            if (isObvious
                && allowCollectionExpression
                && UseCollectionExpression(ref context))
            {
                ReportExplicitToCollectionExpression(ref context);
                return true;
            }
            else if (isObvious
                || IsInitializerObvious(ref context))
            {
                ReportExplicitToImplicit(ref context);
                return true;
            }
        }

        return false;
    }

    public override void AnalyzeImplicitCreation(ref SyntaxNodeAnalysisContext context)
    {
        var implicitArrayCreation = (ImplicitArrayCreationExpressionSyntax)context.Node;

        if (implicitArrayCreation.ContainsDiagnostics
            || implicitArrayCreation.NewKeyword.ContainsDirectives
            || implicitArrayCreation.OpenBracketToken.ContainsDirectives
            || implicitArrayCreation.CloseBracketToken.ContainsDirectives)
        {
            return;
        }

        if (GetTypeStyle(ref context) == TypeStyle.Explicit)
        {
            var arrayTypeSymbol = context.SemanticModel.GetTypeSymbol(implicitArrayCreation, context.CancellationToken) as IArrayTypeSymbol;

            if ((arrayTypeSymbol?.ElementType.SupportsExplicitDeclaration()) == true)
                ReportImplicitToExplicit(ref context);

            return;
        }

        base.AnalyzeImplicitCreation(ref context);
    }

    protected override bool IsInitializerObvious(ref SyntaxNodeAnalysisContext context)
    {
        if (context.Node is ArrayCreationExpressionSyntax arrayCreation)
            return IsInitializerObvious(ref context, arrayCreation, arrayCreation.Initializer);

        if (context.Node is ImplicitArrayCreationExpressionSyntax implicitArrayCreation)
            return IsInitializerObvious(ref context, implicitArrayCreation, implicitArrayCreation.Initializer);

        if (context.Node is CollectionExpressionSyntax collectionExpression)
            return IsInitializerObvious(ref context, collectionExpression);

        return false;
    }

    private static bool IsInitializerObvious(
        ref SyntaxNodeAnalysisContext context,
        ExpressionSyntax creationExpression,
        InitializerExpressionSyntax initializer)
    {
        if (initializer is not null)
        {
            IArrayTypeSymbol arrayTypeSymbol = null;
            var isObvious = false;

            foreach (ExpressionSyntax expression in initializer.Expressions)
            {
                if (arrayTypeSymbol is null)
                {
                    arrayTypeSymbol = context.SemanticModel.GetTypeSymbol(creationExpression, context.CancellationToken) as IArrayTypeSymbol;

                    if (arrayTypeSymbol?.ElementType.SupportsExplicitDeclaration() != true)
                        return true;
                }

                isObvious = CSharpTypeAnalysis.IsTypeObvious(expression, arrayTypeSymbol.ElementType, includeNullability: true, context.SemanticModel, context.CancellationToken);

                if (!isObvious)
                    return false;
            }

            return isObvious;
        }

        return false;
    }

    private static bool IsInitializerObvious(ref SyntaxNodeAnalysisContext context, CollectionExpressionSyntax collectionExpression)
    {
        SeparatedSyntaxList<CollectionElementSyntax> elements = collectionExpression.Elements;

        IArrayTypeSymbol arrayTypeSymbol = null;
        var isObvious = false;

        foreach (CollectionElementSyntax element in elements)
        {
            if (element is not ExpressionElementSyntax expressionElement)
                return false;

            if (arrayTypeSymbol is null)
            {
                ITypeSymbol type = context.SemanticModel.GetTypeInfo(collectionExpression, context.CancellationToken).ConvertedType;

                arrayTypeSymbol = type as IArrayTypeSymbol;

                if (arrayTypeSymbol?.ElementType.SupportsExplicitDeclaration() != true)
                    return true;
            }

            isObvious = CSharpTypeAnalysis.IsTypeObvious(expressionElement.Expression, arrayTypeSymbol.ElementType, includeNullability: true, context.SemanticModel, context.CancellationToken);

            if (!isObvious)
                return false;
        }

        return isObvious;
    }

    protected override bool UseCollectionExpressionFromImplicit(ref SyntaxNodeAnalysisContext context)
    {
        return UseCollectionExpression(ref context);
    }

    public override TypeStyle GetTypeStyle(ref SyntaxNodeAnalysisContext context)
    {
        return context.GetArrayCreationTypeStyle();
    }

    protected override void ReportExplicitToImplicit(ref SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            GetLocationFromExplicit(ref context),
            "Simplify array creation");
    }

    protected override void ReportExplicitToCollectionExpression(ref SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            GetLocationFromExplicit(ref context),
            properties: _explicitToCollectionExpression,
            "Simplify array creation");
    }

    private static Location GetLocationFromExplicit(ref SyntaxNodeAnalysisContext context)
    {
        return ((ArrayCreationExpressionSyntax)context.Node).Type.ElementType.GetLocation();
    }

    protected override void ReportImplicitToExplicit(ref SyntaxNodeAnalysisContext context)
    {
        if (context.Node is CollectionExpressionSyntax collectionExpression
            && !CanConvertCollectionExpression(ref context, collectionExpression))
        {
            return;
        }

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            GetLocationFromImplicit(ref context),
            "Use explicitly typed array");
    }

    protected override void ReportImplicitToCollectionExpression(ref SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            GetLocationFromImplicit(ref context),
            properties: _implicitToCollectionExpression,
            "Simplify array creation");
    }

    private static Location GetLocationFromImplicit(ref SyntaxNodeAnalysisContext context)
    {
        if (context.Node is ImplicitArrayCreationExpressionSyntax implicitCreation)
        {
            return Location.Create(
                implicitCreation.SyntaxTree,
                TextSpan.FromBounds(implicitCreation.NewKeyword.SpanStart, implicitCreation.CloseBracketToken.Span.End));
        }

        return context.Node.GetLocation();
    }

    protected override void ReportCollectionExpressionToImplicit(ref SyntaxNodeAnalysisContext context)
    {
        var collectionExpression = (CollectionExpressionSyntax)context.Node;

        if (!CanConvertCollectionExpression(ref context, collectionExpression))
            return;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            context.Node.GetLocation(),
            properties: _collectionExpressionToImplicit,
            "Simplify array creation");
    }

    private static bool CanConvertCollectionExpression(ref SyntaxNodeAnalysisContext context, CollectionExpressionSyntax collectionExpression)
    {
        foreach (CollectionElementSyntax element in collectionExpression.Elements)
        {
            if (element is SpreadElementSyntax)
                return false;
        }

        return context.SemanticModel
            .GetTypeInfo(context.Node, context.CancellationToken)
            .ConvertedType?
            .IsKind(SymbolKind.ArrayType) == true;
    }
}
