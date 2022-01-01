// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class OptionValidationAnalyzer : BaseDiagnosticAnalyzer
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
                        DiagnosticRules.BlankLineBetweenUsingDirectiveGroups,
                        DiagnosticRules.BlankLineBetweenSingleLineAccessors,
                        DiagnosticRules.AddEmptyLineBetweenAccessors,
                        DiagnosticRules.PlaceNewLineAfterOrBeforeEqualsToken,
                        DiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken,
                        DiagnosticRules.PlaceNewLineAfterOrBeforeConditionalOperator,
                        DiagnosticRules.PlaceNewLineAfterOrBeforeBinaryOperator,
                        CommonDiagnosticRules.AnalyzerOptionIsObsolete,
                        CommonDiagnosticRules.RequiredConfigOptionNotSet);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                CompilationOptions compilationOptions = startContext.Compilation.Options;

                startContext.RegisterSyntaxTreeAction(context =>
                {
                    AnalyzerConfigOptions configOptions = context.GetConfigOptions();

                    if (DiagnosticRules.AddOrRemoveNewLineBeforeWhileInDoStatement.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.AddOrRemoveNewLineBeforeWhileInDoStatement, ConfigOptions.NewLineBeforeWhileInDoStatement);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.RemoveNewLineBetweenClosingBraceAndWhileKeyword, ConfigOptions.NewLineBeforeWhileInDoStatement, "false");
                    }

                    if (DiagnosticRules.BlankLineBetweenUsingDirectiveGroups.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.BlankLineBetweenUsingDirectiveGroups, ConfigOptions.BlankLineBetweenUsingDirectiveGroups);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.RemoveEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace, ConfigOptions.BlankLineBetweenUsingDirectiveGroups, "false");
                    }

                    if (DiagnosticRules.BlankLineBetweenSingleLineAccessors.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.BlankLineBetweenSingleLineAccessors, ConfigOptions.BlankLineBetweenSingleLineAccessors);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.RemoveEmptyLineBetweenSingleLineAccessors, ConfigOptions.BlankLineBetweenSingleLineAccessors, "false");
                    }

                    if (DiagnosticRules.PlaceNewLineAfterOrBeforeEqualsToken.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.PlaceNewLineAfterOrBeforeEqualsToken, ConfigOptions.EqualsTokenNewLine);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.AddNewLineAfterEqualsSignInsteadOfBeforeIt, ConfigOptions.EqualsTokenNewLine, "after");
                    }

                    if (DiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken, ConfigOptions.ArrowTokenNewLine);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt, ConfigOptions.ArrowTokenNewLine, "after");
                    }

                    if (DiagnosticRules.PlaceNewLineAfterOrBeforeConditionalOperator.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.PlaceNewLineAfterOrBeforeConditionalOperator, ConfigOptions.ConditionalOperatorNewLine);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.AddNewLineAfterConditionalOperatorInsteadOfBeforeIt, ConfigOptions.ConditionalOperatorNewLine, "after");
                    }

                    if (DiagnosticRules.PlaceNewLineAfterOrBeforeBinaryOperator.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.PlaceNewLineAfterOrBeforeBinaryOperator, ConfigOptions.BinaryOperatorNewLine);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.AddNewLineAfterBinaryOperatorInsteadOfBeforeIt, ConfigOptions.BinaryOperatorNewLine, "after");
                    }
                });
            });
        }
    }
}
