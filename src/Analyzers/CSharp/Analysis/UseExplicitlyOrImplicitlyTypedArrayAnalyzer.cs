// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseExplicitlyOrImplicitlyTypedArrayAnalyzer : BaseDiagnosticAnalyzer
{
    private static readonly ImmutableDictionary<string, string> _diagnosticProperties = ImmutableDictionary.CreateRange(new[]
        {
            new KeyValuePair<string, string>(DiagnosticPropertyKeys.ConvertImplicitToImplicit, null)
        });

    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(c => AnalyzeImplicitArrayCreationExpression(c), SyntaxKind.ImplicitArrayCreationExpression);
        context.RegisterSyntaxNodeAction(c => AnalyzeArrayCreationExpression(c), SyntaxKind.ArrayCreationExpression);
        context.RegisterSyntaxNodeAction(c => AnalyzeCollectionExpression(c), SyntaxKind.CollectionExpression);
    }

    private static void AnalyzeImplicitArrayCreationExpression(SyntaxNodeAnalysisContext context)
    {
        ArrayCreationTypeStyle style = context.GetArrayCreationTypeStyle();

        var useExplicit = false;

        if (style == ArrayCreationTypeStyle.Explicit
            || style == ArrayCreationTypeStyle.ImplicitWhenTypeIsObvious)
        {
            useExplicit = AnalyzeImplicitArrayCreationExpression(context, style);
        }

        if (!useExplicit
            && context.UseCollectionExpression() == true)
        {
            var arrayTypeSymbol = context.SemanticModel.GetTypeSymbol(context.Node, context.CancellationToken) as IArrayTypeSymbol;

            if (arrayTypeSymbol?.Rank == 1)
            {
                var expression = (ImplicitArrayCreationExpressionSyntax)context.Node;

                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
                    Location.Create(
                        expression.SyntaxTree,
                        TextSpan.FromBounds(expression.NewKeyword.SpanStart, expression.CloseBracketToken.Span.End)),
                    _diagnosticProperties,
                    "implicitly");
            }
        }
    }

    private static bool AnalyzeImplicitArrayCreationExpression(SyntaxNodeAnalysisContext context, ArrayCreationTypeStyle kind)
    {
        var expression = (ImplicitArrayCreationExpressionSyntax)context.Node;

        if (expression.ContainsDiagnostics
            || expression.NewKeyword.ContainsDirectives
            || expression.OpenBracketToken.ContainsDirectives
            || expression.CloseBracketToken.ContainsDirectives)
        {
            return false;
        }

        IArrayTypeSymbol arrayTypeSymbol = null;

        if (kind == ArrayCreationTypeStyle.ImplicitWhenTypeIsObvious)
        {
            InitializerExpressionSyntax initializer = expression.Initializer;

            if (initializer is not null)
            {
                var isObvious = false;

                foreach (ExpressionSyntax expression2 in initializer.Expressions)
                {
                    if (arrayTypeSymbol is null)
                    {
                        arrayTypeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken) as IArrayTypeSymbol;

                        if (arrayTypeSymbol?.ElementType.SupportsExplicitDeclaration() != true)
                            return false;
                    }

                    isObvious = CSharpTypeAnalysis.IsTypeObvious(expression2, arrayTypeSymbol.ElementType, includeNullability: true, context.SemanticModel, context.CancellationToken);

                    if (!isObvious)
                        break;
                }

                if (isObvious)
                    return false;
            }
        }

        if (arrayTypeSymbol is null)
        {
            arrayTypeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken) as IArrayTypeSymbol;

            if (arrayTypeSymbol?.ElementType.SupportsExplicitDeclaration() != true)
                return false;
        }

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            Location.Create(
                expression.SyntaxTree,
                TextSpan.FromBounds(expression.NewKeyword.SpanStart, expression.CloseBracketToken.Span.End)),
            "explicitly");

        return true;
    }

    private static void AnalyzeCollectionExpression(SyntaxNodeAnalysisContext context)
    {
        var collectionExpression = (CollectionExpressionSyntax)context.Node;

        if (collectionExpression.ContainsDiagnostics)
            return;

        foreach (CollectionElementSyntax element in collectionExpression.Elements)
        {
            if (element is SpreadElementSyntax)
                return;
        }

        ArrayCreationTypeStyle style = context.GetArrayCreationTypeStyle();

        var useExplicit = false;

        if (style == ArrayCreationTypeStyle.Explicit
            || style == ArrayCreationTypeStyle.ImplicitWhenTypeIsObvious)
        {
            useExplicit = AnalyzeCollectionExpression(context, collectionExpression, style);
        }

        if (!useExplicit
            && context.UseCollectionExpression() == true)
        {
            var arrayTypeSymbol = context.SemanticModel.GetTypeSymbol(context.Node, context.CancellationToken) as IArrayTypeSymbol;

            if (arrayTypeSymbol?.Rank == 1)
            {
                var expression = (ImplicitArrayCreationExpressionSyntax)context.Node;

                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
                    Location.Create(
                        expression.SyntaxTree,
                        TextSpan.FromBounds(expression.NewKeyword.SpanStart, expression.CloseBracketToken.Span.End)),
                    _diagnosticProperties,
                    "implicitly");
            }
        }
    }

    private static bool AnalyzeCollectionExpression(
        SyntaxNodeAnalysisContext context,
        CollectionExpressionSyntax collectionExpression,
        ArrayCreationTypeStyle kind)
    {
        IArrayTypeSymbol arrayTypeSymbol = null;

        if (kind == ArrayCreationTypeStyle.ImplicitWhenTypeIsObvious)
        {
            var isObvious = false;

            foreach (CollectionElementSyntax element in collectionExpression.Elements)
            {
                if (arrayTypeSymbol is null)
                {
                    arrayTypeSymbol = context.SemanticModel.GetTypeSymbol(collectionExpression, context.CancellationToken) as IArrayTypeSymbol;

                    if (arrayTypeSymbol?.ElementType.SupportsExplicitDeclaration() != true)
                        return false;
                }

                isObvious = CSharpTypeAnalysis.IsTypeObvious(((ExpressionElementSyntax)element).Expression, arrayTypeSymbol.ElementType, includeNullability: true, context.SemanticModel, context.CancellationToken);

                if (!isObvious)
                    break;
            }

            if (isObvious)
                return false;
        }

        if (arrayTypeSymbol is null)
        {
            arrayTypeSymbol = context.SemanticModel.GetTypeSymbol(collectionExpression, context.CancellationToken) as IArrayTypeSymbol;

            if (arrayTypeSymbol?.ElementType.SupportsExplicitDeclaration() != true)
                return false;
        }

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            collectionExpression,
            "explicitly");

        return true;
    }

    private static void AnalyzeArrayCreationExpression(SyntaxNodeAnalysisContext context)
    {
        ArrayCreationTypeStyle style = context.GetArrayCreationTypeStyle();

        if (style != ArrayCreationTypeStyle.Implicit
            && style != ArrayCreationTypeStyle.ImplicitWhenTypeIsObvious)
        {
            return;
        }

        var arrayCreation = (ArrayCreationExpressionSyntax)context.Node;

        if (arrayCreation.ContainsDiagnostics)
            return;

        ArrayTypeSyntax arrayType = arrayCreation.Type;

        if (arrayType.ContainsDirectives)
            return;

        SeparatedSyntaxList<ExpressionSyntax> expressions = arrayCreation.Initializer?.Expressions ?? default;

        if (!expressions.Any())
            return;

        ITypeSymbol typeSymbol = null;

        if (style == ArrayCreationTypeStyle.ImplicitWhenTypeIsObvious)
        {
            foreach (ExpressionSyntax expression in expressions)
            {
                if (typeSymbol is null)
                {
                    typeSymbol = context.SemanticModel.GetTypeSymbol(arrayCreation.Type.ElementType, context.CancellationToken);

                    if (typeSymbol?.IsErrorType() != false)
                        return;
                }

                if (!CSharpTypeAnalysis.IsTypeObvious(expression, typeSymbol, includeNullability: true, context.SemanticModel, context.CancellationToken))
                    return;
            }
        }

        TypeSyntax elementType = arrayType.ElementType;
        SyntaxList<ArrayRankSpecifierSyntax> rankSpecifiers = arrayType.RankSpecifiers;

        TextSpan textSpan = TextSpan.FromBounds(
            elementType.SpanStart,
            ((rankSpecifiers.Count > 1) ? rankSpecifiers.LastButOne() : (SyntaxNode)elementType).Span.End);

        Location location = Location.Create(arrayCreation.SyntaxTree, textSpan);

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
            location,
            "implicitly");
    }
}
