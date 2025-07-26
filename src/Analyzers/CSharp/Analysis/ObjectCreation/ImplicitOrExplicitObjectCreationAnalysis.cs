// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis;

internal class ImplicitOrExplicitObjectCreationAnalysis : ImplicitOrExplicitCreationAnalysis
{
    public static ImplicitOrExplicitObjectCreationAnalysis Instance { get; } = new();

    public override TypeStyle GetTypeStyle(ref SyntaxNodeAnalysisContext context)
    {
        return context.GetObjectCreationTypeStyle();
    }

    public override void AnalyzeCollectionExpression(ref SyntaxNodeAnalysisContext context)
    {
        if (context.SemanticModel.GetTypeInfo(context.Node, context.CancellationToken).ConvertedType?.TypeKind != TypeKind.Array)
            AnalyzeImplicit(ref context);
    }

    protected override void ReportExplicitToImplicit(ref SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            objectCreation.Type.GetLocation(),
            "Simplify object creation");
    }

    protected override void ReportImplicitToExplicit(ref SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node,
            "Use explicit object creation");
    }

    protected override void ReportVarToExplicit(ref SyntaxNodeAnalysisContext context, TypeSyntax type)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node.GetLocation(),
            properties: _varToExplicit,
            "Use explicit object creation");
    }

#if ROSLYN_4_7
    protected override bool UseCollectionExpressionFromImplicit(ref SyntaxNodeAnalysisContext context)
    {
        var implicitObjectCreation = (ImplicitObjectCreationExpressionSyntax)context.Node;

        return implicitObjectCreation.ArgumentList?.Arguments.Any() != true
            && implicitObjectCreation.Initializer?.Expressions
                .Any(f => f.IsKind(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxKind.ComplexElementInitializerExpression)) != true
            && UseCollectionExpression(ref context);
    }

    protected override void ReportExplicitToCollectionExpression(ref SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            objectCreation.Type.GetLocation(),
            properties: _explicitToCollectionExpression,
            "Simplify object creation");
    }

    protected override void ReportImplicitToCollectionExpression(ref SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node.GetLocation(),
            properties: _implicitToCollectionExpression,
            "Simplify object creation");
    }

    protected override void ReportCollectionExpressionToImplicit(ref SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node.GetLocation(),
            properties: _collectionExpressionToImplicit,
            "Simplify object creation");
    }
#endif
}
