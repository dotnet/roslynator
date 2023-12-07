// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis;

internal class ImplicitOrExpressionObjectCreationAnalysis : ImplicitOrExplicitCreationAnalysis
{
    public static ImplicitOrExpressionObjectCreationAnalysis Instance { get; } = new();

    public override ObjectCreationTypeStyle GetTypeStyle(ref SyntaxNodeAnalysisContext context)
    {
        return context.GetObjectCreationTypeStyle();
    }

    protected override bool PreferCollectionExpressionFromImplicit(ref SyntaxNodeAnalysisContext context)
    {
        return ((ImplicitObjectCreationExpressionSyntax)context.Node).ArgumentList?.Arguments.Any() != true
            && PreferCollectionExpression(ref context);
    }

    protected override void ReportExplicitToImplicit(ref SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            objectCreation.Type.GetLocation(),
            "implicit object creation");
    }

    protected override void ReportExplicitToCollectionExpression(ref SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            objectCreation.Type.GetLocation(),
            properties: _explicitToCollectionExpression,
            "collection expression");
    }

    protected override void ReportImplicitToExplicit(ref SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node,
            "explicit object creation");
    }

    protected override void ReportImplicitToCollectionExpression(ref SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node.GetLocation(),
            properties: _implicitToCollectionExpression,
            "collection expression");
    }

    protected override void ReportCollectionExpressionToImplicit(ref SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node.GetLocation(),
            properties: _collectionExpressionToImplicit,
            "implicit object creation");
    }
}
