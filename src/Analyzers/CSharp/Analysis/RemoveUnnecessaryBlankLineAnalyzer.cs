// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveUnnecessaryBlankLineAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.Obsolete_RemoveUnnecessaryBlankLine);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        UnnecessaryBlankLineAnalysis.Instance.Initialize(context);
    }

    private class UnnecessaryBlankLineAnalysis : Roslynator.CSharp.Analysis.UnnecessaryBlankLineAnalysis
    {
        public static UnnecessaryBlankLineAnalysis Instance { get; } = new();

        protected override DiagnosticDescriptor Descriptor => DiagnosticRules.Obsolete_RemoveUnnecessaryBlankLine;
    }
}
