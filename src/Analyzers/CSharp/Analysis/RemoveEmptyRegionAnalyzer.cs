// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveEmptyRegionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveEmptyRegion,
                    DiagnosticDescriptors.RemoveEmptyRegionFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeRegionDirective, SyntaxKind.RegionDirectiveTrivia);
        }

        public static void AnalyzeRegionDirective(SyntaxNodeAnalysisContext context)
        {
            var regionDirective = (RegionDirectiveTriviaSyntax)context.Node;

            RegionInfo region = SyntaxInfo.RegionInfo(regionDirective);

            if (!region.Success)
                return;

            if (!region.IsEmpty)
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.RemoveEmptyRegion,
                regionDirective.GetLocation(),
                additionalLocations: ImmutableArray.Create(region.EndDirective.GetLocation()));

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveEmptyRegionFadeOut, regionDirective.GetLocation());
            context.ReportDiagnostic(DiagnosticDescriptors.RemoveEmptyRegionFadeOut, region.EndDirective.GetLocation());
        }
    }
}
