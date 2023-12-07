// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis;

internal class ImplicitOrExpressionObjectCreationAnalysis : ImplicitOrExplicitCreationAnalysis
{
    public static ImplicitOrExpressionObjectCreationAnalysis Instance { get; } = new();

    public override ObjectCreationTypeStyle GetTypeStyle(SyntaxNodeAnalysisContext context)
    {
        return context.GetObjectCreationTypeStyle();
    }

    protected override void ReportExplicitToImplicit(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            objectCreation.Type.GetLocation(),
            "implicit object creation");
    }

    protected override void ReportExplicitToCollectionExpression(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            objectCreation.Type.GetLocation(),
            properties: _explicitToCollectionExpression,
            "collection expression");
    }

    protected override void ReportImplicitToExplicit(SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node,
            "explicit object creation");
    }

    protected override void ReportImplicitToCollectionExpression(SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node.GetLocation(),
            properties: _implicitToCollectionExpression,
            "collection expression");
    }

    protected override void ReportCollectionExpressionToImplicit(SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node.GetLocation(),
            properties: _collectionExpressionToImplicit,
            "implicit object creation");
    }
}
