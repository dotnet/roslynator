﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class AnalyzerOptionIsObsoleteAnalyzer : AbstractAnalyzerOptionIsObsoleteAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, CommonDiagnosticRules.AnalyzerOptionIsObsolete);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterCompilationStartAction(compilationContext =>
        {
            var flags = Flags.None;

            CompilationOptions compilationOptions = compilationContext.Compilation.Options;

            compilationContext.RegisterSyntaxTreeAction(context =>
            {
                // files generated by source generator have relative path
                if (!Path.IsPathRooted(context.Tree.FilePath))
                    return;

                AnalyzerConfigOptions configOptions = context.GetConfigOptions();

                Validate(ref context, compilationOptions, configOptions, Flags.AddOrRemoveNewLineBeforeWhileInDoStatement, ref flags, DiagnosticRules.AddOrRemoveNewLineBeforeWhileInDoStatement, ConfigOptions.NewLineBeforeWhileInDoStatement, LegacyConfigOptions.RemoveNewLineBetweenClosingBraceAndWhileKeyword, "false");
                Validate(ref context, compilationOptions, configOptions, Flags.BlankLineBetweenSingleLineAccessors, ref flags, DiagnosticRules.BlankLineBetweenSingleLineAccessors, ConfigOptions.BlankLineBetweenSingleLineAccessors, LegacyConfigOptions.RemoveEmptyLineBetweenSingleLineAccessors, "false");
                Validate(ref context, compilationOptions, configOptions, Flags.BlankLineBetweenUsingDirectives, ref flags, DiagnosticRules.BlankLineBetweenUsingDirectives, ConfigOptions.BlankLineBetweenUsingDirectives, LegacyConfigOptions.RemoveEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace, ConfigOptionValues.BlankLineBetweenUsingDirectives_Never);
                Validate(ref context, compilationOptions, configOptions, Flags.PlaceNewLineAfterOrBeforeArrowToken, ref flags, DiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken, ConfigOptions.ArrowTokenNewLine, LegacyConfigOptions.AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt, "after");
                Validate(ref context, compilationOptions, configOptions, Flags.PlaceNewLineAfterOrBeforeConditionalOperator, ref flags, DiagnosticRules.PlaceNewLineAfterOrBeforeConditionalOperator, ConfigOptions.ConditionalOperatorNewLine, LegacyConfigOptions.AddNewLineAfterConditionalOperatorInsteadOfBeforeIt, "after");
                Validate(ref context, compilationOptions, configOptions, Flags.PlaceNewLineAfterOrBeforeBinaryOperator, ref flags, DiagnosticRules.PlaceNewLineAfterOrBeforeBinaryOperator, ConfigOptions.BinaryOperatorNewLine, LegacyConfigOptions.AddNewLineAfterBinaryOperatorInsteadOfBeforeIt, "after");
                Validate(ref context, compilationOptions, configOptions, Flags.PlaceNewLineAfterOrBeforeEqualsToken, ref flags, DiagnosticRules.PlaceNewLineAfterOrBeforeEqualsToken, ConfigOptions.EqualsTokenNewLine, LegacyConfigOptions.AddNewLineAfterEqualsSignInsteadOfBeforeIt, "after");
            });
        });
    }

    private static void Validate(
        ref SyntaxTreeAnalysisContext context,
        CompilationOptions compilationOptions,
        AnalyzerConfigOptions configOptions,
        Flags flag,
        ref Flags flags,
        DiagnosticDescriptor analyzer,
        ConfigOptionDescriptor option,
        LegacyConfigOptionDescriptor legacyOption,
        string newValue)
    {
        if (!flags.HasFlag(flag)
            && analyzer.IsEffective(context.Tree, compilationOptions, context.CancellationToken)
            && TryReportObsoleteOption(context, configOptions, legacyOption, option, newValue))
        {
            flags |= flag;
        }
    }

    [Flags]
    private enum Flags
    {
        None,
        AddOrRemoveNewLineBeforeWhileInDoStatement,
        BlankLineBetweenSingleLineAccessors,
        BlankLineBetweenUsingDirectives,
        PlaceNewLineAfterOrBeforeArrowToken,
        PlaceNewLineAfterOrBeforeBinaryOperator,
        PlaceNewLineAfterOrBeforeConditionalOperator,
        PlaceNewLineAfterOrBeforeEqualsToken,
    }
}
