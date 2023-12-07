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

    public override void AnalyzeExplicitCreation(SyntaxNodeAnalysisContext context)
    {
        ObjectCreationTypeStyle style = GetTypeStyle(context);

        if (style != ObjectCreationTypeStyle.Implicit
            && style != ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious)
        {
            return;
        }

        if (context.Node.ContainsDiagnostics)
            return;

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
                                AnalyzeExplicitObvious(context);
                            }
                            else if (variableDeclaration.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement))
                            {
                                bool isVar = variableDeclaration.Type.IsVar;

                                if (!AnalyzeExplicit(context, isObvious: !isVar, canUseCollectionExpression: !isVar)
                                    && context.UseVarInsteadOfImplicitObjectCreation() == false)
                                {
                                    if (isVar)
                                    {
                                        ReportExplicitToImplicit(context);
                                    }
                                    else
                                    {
                                        AnalyzeType(context, arrayCreation, variableDeclaration.Type);
                                    }
                                }
                            }
                        }
                    }
                    else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                    {
                        AnalyzeExplicitObvious(context);
                    }

                    break;
                }
            case SyntaxKind.ArrowExpressionClause:
                {
                    TypeSyntax type = DetermineReturnType(parent.Parent);

                    if (type is not null)
                        AnalyzeExplicitObvious(context);

                    break;
                }
            case SyntaxKind.ReturnStatement:
                {
                    if (style == ObjectCreationTypeStyle.Implicit
                        || IsSingleReturnStatement(parent))
                    {
                        for (SyntaxNode node = parent.Parent; node is not null; node = node.Parent)
                        {
                            if (CSharpFacts.IsAnonymousFunctionExpression(node.Kind()))
                                return;

                            TypeSyntax type = DetermineReturnType(node);

                            if (type is not null)
                            {
                                AnalyzeExplicit(context, isObvious: IsSingleReturnStatement(parent));
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
                    AnalyzeExplicitNotObvious(context);
                    break;
                }
            case SyntaxKind.CoalesceExpression:
                {
                    if (style == ObjectCreationTypeStyle.Implicit)
                    {
                        AnalyzeExplicitNotObvious(context);
                    }
                    else if (style == ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious
                        && parent.IsParentKind(SyntaxKind.EqualsValueClause))
                    {
                        if (parent.Parent.Parent is VariableDeclaratorSyntax variableDeclarator)
                        {
                            if (variableDeclarator.Parent is VariableDeclarationSyntax variableDeclaration
                                && variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration))
                            {
                                AnalyzeExplicitObvious(context);
                            }
                        }
                        else if (parent.Parent.Parent is PropertyDeclarationSyntax)
                        {
                            AnalyzeExplicitObvious(context);
                        }
                    }

                    break;
                }
        }
    }

    private void AnalyzeExplicitObvious(SyntaxNodeAnalysisContext context, bool canUseCollectionExpression = true)
    {
        AnalyzeExplicit(context, isObvious: true, canUseCollectionExpression: canUseCollectionExpression);
    }

    private void AnalyzeExplicitNotObvious(SyntaxNodeAnalysisContext context, bool canUseCollectionExpression = true)
    {
        AnalyzeExplicit(context, isObvious: false, canUseCollectionExpression: canUseCollectionExpression);
    }

    private bool AnalyzeExplicit(SyntaxNodeAnalysisContext context, bool isObvious, bool canUseCollectionExpression = true)
    {
        ObjectCreationTypeStyle style = GetTypeStyle(context);

        if (style == ObjectCreationTypeStyle.Implicit)
        {
            if (canUseCollectionExpression
                && UseCollectionExpression(context))
            {
                ReportExplicitToCollectionExpression(context);
                return true;
            }
            else
            {
                ReportExplicitToImplicit(context);
                return true;
            }
        }
        else if (style == ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious)
        {
            if (isObvious
                && canUseCollectionExpression
                && UseCollectionExpression(context))
            {
                ReportExplicitToCollectionExpression(context);
                return true;
            }
            else if (isObvious
                || IsInitializerObvious(context))
            {
                ReportExplicitToImplicit(context);
                return true;
            }
        }

        return false;
    }

    public override void AnalyzeImplicitCreation(SyntaxNodeAnalysisContext context)
    {
        var implicitArrayCreation = (ImplicitArrayCreationExpressionSyntax)context.Node;

        if (implicitArrayCreation.ContainsDiagnostics
            || implicitArrayCreation.NewKeyword.ContainsDirectives
            || implicitArrayCreation.OpenBracketToken.ContainsDirectives
            || implicitArrayCreation.CloseBracketToken.ContainsDirectives)
        {
            return;
        }

        if (UseExplicit(context))
        {
            var arrayTypeSymbol = context.SemanticModel.GetTypeSymbol(implicitArrayCreation, context.CancellationToken) as IArrayTypeSymbol;

            if ((arrayTypeSymbol?.ElementType.SupportsExplicitDeclaration()) == true)
                ReportImplicitToExplicit(context);

            return;
        }

        base.AnalyzeImplicitCreation(context);
    }

    protected override bool IsInitializerObvious(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is ArrayCreationExpressionSyntax arrayCreation)
            return IsInitializerObvious(context, arrayCreation, arrayCreation.Initializer);

        if (context.Node is ImplicitArrayCreationExpressionSyntax implicitArrayCreation)
            return IsInitializerObvious(context, implicitArrayCreation, implicitArrayCreation.Initializer);

        if (context.Node is CollectionExpressionSyntax collectionExpression)
            return IsInitializerObvious(context, collectionExpression);

        return false;
    }

    private static bool IsInitializerObvious(SyntaxNodeAnalysisContext context, ExpressionSyntax creationExpression, InitializerExpressionSyntax initializer)
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

    public override ObjectCreationTypeStyle GetTypeStyle(SyntaxNodeAnalysisContext context)
    {
        return context.GetArrayCreationTypeStyle();
    }

    protected override void ReportExplicitToImplicit(SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            GetLocationFromExplicit(context),
            "implicitly typed array");
    }

    protected override void ReportExplicitToCollectionExpression(SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            GetLocationFromExplicit(context),
            properties: _explicitToCollectionExpression,
            "collection expression");
    }

    private static Location GetLocationFromExplicit(SyntaxNodeAnalysisContext context)
    {
        return ((ArrayCreationExpressionSyntax)context.Node).Type.ElementType.GetLocation();
    }

    protected override void ReportImplicitToExplicit(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is CollectionExpressionSyntax collectionExpression)
        {
            foreach (CollectionElementSyntax element in collectionExpression.Elements)
            {
                if (element is SpreadElementSyntax)
                    return;
            }
        }

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            GetLocationFromImplicit(context),
            "explicitly typed array");
    }

    protected override void ReportImplicitToCollectionExpression(SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            GetLocationFromImplicit(context),
            properties: _implicitToCollectionExpression,
            "collection expression");
    }

    private static Location GetLocationFromImplicit(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is ImplicitArrayCreationExpressionSyntax implicitCreation)
        {
            return Location.Create(
                implicitCreation.SyntaxTree,
                TextSpan.FromBounds(implicitCreation.NewKeyword.SpanStart, implicitCreation.CloseBracketToken.Span.End));
        }

        return context.Node.GetLocation();
    }

    protected override void ReportCollectionExpressionToImplicit(SyntaxNodeAnalysisContext context)
    {
        var collectionExpression = (CollectionExpressionSyntax)context.Node;

        foreach (CollectionElementSyntax element in collectionExpression.Elements)
        {
            if (element is SpreadElementSyntax)
                return;
        }

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            context.Node.GetLocation(),
            properties: _collectionExpressionToImplicit,
            "implicitly typed array");
    }
}
