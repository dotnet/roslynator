// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class OptionValidationAnalyzer : AbstractOptionValidationAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(
                        ref _supportedDiagnostics,
                        DiagnosticRules.AddOrRemoveNewLineBeforeWhileInDoStatement,
                        DiagnosticRules.BlankLineBetweenSingleLineAccessors,
                        DiagnosticRules.BlankLineBetweenUsingDirectiveGroups,
                        DiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken,
                        DiagnosticRules.PlaceNewLineAfterOrBeforeBinaryOperator,
                        DiagnosticRules.PlaceNewLineAfterOrBeforeConditionalOperator,
                        DiagnosticRules.PlaceNewLineAfterOrBeforeEqualsToken,
                        CommonDiagnosticRules.AnalyzerOptionIsObsolete,
                        CommonDiagnosticRules.RequiredConfigOptionNotSet);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var missingFlags = Flags.None;
                var obsoleteFlags = Flags.None;

                CompilationOptions compilationOptions = compilationContext.Compilation.Options;

                compilationContext.RegisterSyntaxTreeAction(context =>
                {
                    SyntaxTree tree = context.Tree;

                    if (compilationOptions.SyntaxTreeOptionsProvider.IsGenerated(tree, compilationContext.CancellationToken) == GeneratedKind.MarkedGenerated)
                        return;

                    AnalyzerConfigOptions configOptions = context.GetConfigOptions();

                    ValidateMissing(ref context, compilationOptions, configOptions, Flags.AddOrRemoveNewLineBeforeWhileInDoStatement, ref missingFlags, DiagnosticRules.AddOrRemoveNewLineBeforeWhileInDoStatement, ConfigOptions.NewLineBeforeWhileInDoStatement);
                    ValidateMissing(ref context, compilationOptions, configOptions, Flags.BlankLineBetweenSingleLineAccessors, ref missingFlags, DiagnosticRules.BlankLineBetweenSingleLineAccessors, ConfigOptions.BlankLineBetweenSingleLineAccessors);
                    ValidateMissing(ref context, compilationOptions, configOptions, Flags.BlankLineBetweenUsingDirectiveGroups, ref missingFlags, DiagnosticRules.BlankLineBetweenUsingDirectiveGroups, ConfigOptions.BlankLineBetweenUsingDirectiveGroups);
                    ValidateMissing(ref context, compilationOptions, configOptions, Flags.PlaceNewLineAfterOrBeforeArrowToken, ref missingFlags, DiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken, ConfigOptions.ArrowTokenNewLine);
                    ValidateMissing(ref context, compilationOptions, configOptions, Flags.PlaceNewLineAfterOrBeforeConditionalOperator, ref missingFlags, DiagnosticRules.PlaceNewLineAfterOrBeforeConditionalOperator, ConfigOptions.ConditionalOperatorNewLine);
                    ValidateMissing(ref context, compilationOptions, configOptions, Flags.PlaceNewLineAfterOrBeforeBinaryOperator, ref missingFlags, DiagnosticRules.PlaceNewLineAfterOrBeforeBinaryOperator, ConfigOptions.BinaryOperatorNewLine);
                    ValidateMissing(ref context, compilationOptions, configOptions, Flags.PlaceNewLineAfterOrBeforeEqualsToken, ref missingFlags, DiagnosticRules.PlaceNewLineAfterOrBeforeEqualsToken, ConfigOptions.EqualsTokenNewLine);

                    ValidateObsolete(ref context, compilationOptions, configOptions, Flags.AddOrRemoveNewLineBeforeWhileInDoStatement, ref obsoleteFlags, DiagnosticRules.AddOrRemoveNewLineBeforeWhileInDoStatement, ConfigOptions.NewLineBeforeWhileInDoStatement, LegacyConfigOptions.RemoveNewLineBetweenClosingBraceAndWhileKeyword, "false");
                    ValidateObsolete(ref context, compilationOptions, configOptions, Flags.BlankLineBetweenSingleLineAccessors, ref obsoleteFlags, DiagnosticRules.BlankLineBetweenSingleLineAccessors, ConfigOptions.BlankLineBetweenSingleLineAccessors, LegacyConfigOptions.RemoveEmptyLineBetweenSingleLineAccessors, "false");
                    ValidateObsolete(ref context, compilationOptions, configOptions, Flags.BlankLineBetweenUsingDirectiveGroups, ref obsoleteFlags, DiagnosticRules.BlankLineBetweenUsingDirectiveGroups, ConfigOptions.BlankLineBetweenUsingDirectiveGroups, LegacyConfigOptions.RemoveEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace, "false");
                    ValidateObsolete(ref context, compilationOptions, configOptions, Flags.PlaceNewLineAfterOrBeforeArrowToken, ref obsoleteFlags, DiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken, ConfigOptions.ArrowTokenNewLine, LegacyConfigOptions.AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt, "after");
                    ValidateObsolete(ref context, compilationOptions, configOptions, Flags.PlaceNewLineAfterOrBeforeConditionalOperator, ref obsoleteFlags, DiagnosticRules.PlaceNewLineAfterOrBeforeConditionalOperator, ConfigOptions.ConditionalOperatorNewLine, LegacyConfigOptions.AddNewLineAfterConditionalOperatorInsteadOfBeforeIt, "after");
                    ValidateObsolete(ref context, compilationOptions, configOptions, Flags.PlaceNewLineAfterOrBeforeBinaryOperator, ref obsoleteFlags, DiagnosticRules.PlaceNewLineAfterOrBeforeBinaryOperator, ConfigOptions.BinaryOperatorNewLine, LegacyConfigOptions.AddNewLineAfterBinaryOperatorInsteadOfBeforeIt, "after");
                    ValidateObsolete(ref context, compilationOptions, configOptions, Flags.PlaceNewLineAfterOrBeforeEqualsToken, ref obsoleteFlags, DiagnosticRules.PlaceNewLineAfterOrBeforeEqualsToken, ConfigOptions.EqualsTokenNewLine, LegacyConfigOptions.AddNewLineAfterEqualsSignInsteadOfBeforeIt, "after");
                });
            });
        }

        private static void ValidateMissing(
            ref SyntaxTreeAnalysisContext context,
            CompilationOptions compilationOptions,
            AnalyzerConfigOptions configOptions,
            Flags flag,
            ref Flags flags,
            DiagnosticDescriptor analyzer,
            ConfigOptionDescriptor option)
        {
            if (!flags.HasFlag(flag)
                && analyzer.IsEffective(context.Tree, compilationOptions, context.CancellationToken)
                && TryReportMissingRequiredOption(context, configOptions, analyzer, option))
            {
                flags |= flag;
            }
        }

        private static void ValidateObsolete(
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
            BlankLineBetweenUsingDirectiveGroups,
            PlaceNewLineAfterOrBeforeArrowToken,
            PlaceNewLineAfterOrBeforeBinaryOperator,
            PlaceNewLineAfterOrBeforeConditionalOperator,
            PlaceNewLineAfterOrBeforeEqualsToken,
        }
    }
}
