// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseImplicitOrExplicitObjectCreationAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UseImplicitOrExplicitObjectCreation);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterCompilationStartAction(startContext =>
        {
            if (((CSharpCompilation)startContext.Compilation).LanguageVersion >= LanguageVersion.CSharp9)
            {
                startContext.RegisterSyntaxNodeAction(f => AnalyzeExplicit(f), SyntaxKind.ObjectCreationExpression);
            }

            startContext.RegisterSyntaxNodeAction(c => AnalyzeImplicit(c), SyntaxKind.ImplicitObjectCreationExpression);
            startContext.RegisterSyntaxNodeAction(c => AnalyzeImplicit(c), SyntaxKind.CollectionExpression);
        });
    }

    private static void AnalyzeExplicit(SyntaxNodeAnalysisContext context)
    {
        ImplicitOrExpressionObjectCreationAnalysis.Instance.AnalyzeExplicitCreation(ref context);
    }

    private static void AnalyzeImplicit(SyntaxNodeAnalysisContext context)
    {
        ImplicitOrExpressionObjectCreationAnalysis.Instance.AnalyzeImplicit(ref context);
    }
}
