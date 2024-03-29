﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseExplicitlyOrImplicitlyTypedArrayAnalyzer : BaseDiagnosticAnalyzer
{
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

        context.RegisterSyntaxNodeAction(c => AnalyzeArrayCreationExpression(c), SyntaxKind.ArrayCreationExpression);
        context.RegisterSyntaxNodeAction(c => AnalyzeImplicitArrayCreationExpression(c), SyntaxKind.ImplicitArrayCreationExpression);
#if ROSLYN_4_7
        context.RegisterSyntaxNodeAction(c => AnalyzeCollectionExpression(c), SyntaxKind.CollectionExpression);
#endif
    }

    private static void AnalyzeArrayCreationExpression(SyntaxNodeAnalysisContext context)
    {
        ImplicitOrExplicitArrayCreationAnalysis.Instance.AnalyzeExplicitCreation(ref context);
    }

    private static void AnalyzeImplicitArrayCreationExpression(SyntaxNodeAnalysisContext context)
    {
        ImplicitOrExplicitArrayCreationAnalysis.Instance.AnalyzeImplicitCreation(ref context);
    }

    private static void AnalyzeCollectionExpression(SyntaxNodeAnalysisContext context)
    {
        ImplicitOrExplicitArrayCreationAnalysis.Instance.AnalyzeCollectionExpression(ref context);
    }
}
