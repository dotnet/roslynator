// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
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
                        DiagnosticRules.UseBlockBodyOrExpressionBody,
                        DiagnosticRules.UseAnonymousFunctionOrMethodGroup,
                        DiagnosticRules.UseHasFlagMethodOrBitwiseOperator,
                        DiagnosticRules.ConfigureAwait,
                        DiagnosticRules.UseEmptyStringLiteralOrStringEmpty,
                        DiagnosticRules.NormalizeNullCheck,
                        DiagnosticRules.AddOrRemoveParenthesesFromConditionInConditionalOperator,
                        DiagnosticRules.IncludeParenthesesWhenCreatingNewObject,
                        DiagnosticRules.AddOrRemoveAccessibilityModifiers,
                        DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
                        DiagnosticRules.UseBlockBodyOrExpressionBody,
                        DiagnosticRules.RemoveUnusedMemberDeclaration,
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

                    if (DiagnosticRules.UseBlockBodyOrExpressionBody.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.UseBlockBodyOrExpressionBody, ConfigOptions.BodyStyle);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, ConfigOptions.PreferBlockBodyWhenDeclarationSpansOverMultipleLines, "true");
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, ConfigOptions.PreferBlockBodyWhenExpressionSpansOverMultipleLines, "true");
                    }

                    if (DiagnosticRules.UseAnonymousFunctionOrMethodGroup.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.UseAnonymousFunctionOrMethodGroup, ConfigOptions.AnonymousFunctionOrMethodGroup);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.ConvertMethodGroupToAnonymousFunction, ConfigOptions.AnonymousFunctionOrMethodGroup, ConfigOptionValues.AnonymousFunctionOrMethodGroup_AnonymousFunction);
                    }

                    if (DiagnosticRules.UseHasFlagMethodOrBitwiseOperator.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.UseHasFlagMethodOrBitwiseOperator, ConfigOptions.EnumHasFlagStyle);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.ConvertBitwiseOperationToHasFlagCall, ConfigOptions.EnumHasFlagStyle, ConfigOptionValues.EnumHasFlagStyle_Method);
                    }

                    if (DiagnosticRules.ConfigureAwait.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.ConfigureAwait, ConfigOptions.ConfigureAwait);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.RemoveCallToConfigureAwait, ConfigOptions.ConfigureAwait, "false");
                    }

                    if (DiagnosticRules.UseEmptyStringLiteralOrStringEmpty.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.UseEmptyStringLiteralOrStringEmpty, ConfigOptions.EmptyStringStyle);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.UseStringEmptyInsteadOfEmptyStringLiteral, ConfigOptions.EmptyStringStyle, ConfigOptionValues.EmptyStringStyle_Field);
                    }

                    if (DiagnosticRules.NormalizeNullCheck.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.NormalizeNullCheck, ConfigOptions.NullCheckStyle);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.UseComparisonInsteadPatternMatchingToCheckForNull, ConfigOptions.NullCheckStyle, ConfigOptionValues.NullCheckStyle_EqualityOperator);
                    }

                    if (DiagnosticRules.AddOrRemoveParenthesesFromConditionInConditionalOperator.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.AddOrRemoveParenthesesFromConditionInConditionalOperator, ConfigOptions.ConditionInConditionalOperatorParenthesesStyle);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.RemoveParenthesesFromConditionOfConditionalExpressionWhenExpressionIsSingleToken, ConfigOptions.ConditionInConditionalOperatorParenthesesStyle, ConfigOptionValues.ConditionInConditionalExpressionParenthesesStyle_OmitWhenConditionIsSingleToken);
                    }

                    if (DiagnosticRules.IncludeParenthesesWhenCreatingNewObject.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.IncludeParenthesesWhenCreatingNewObject, ConfigOptions.ObjectCreationParenthesesStyle);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.RemoveParenthesesWhenCreatingNewObject, ConfigOptions.ObjectCreationParenthesesStyle, ConfigOptionValues.ObjectCreationParenthesesStyle_Omit);
                    }

                    if (DiagnosticRules.AddOrRemoveAccessibilityModifiers.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.AddOrRemoveAccessibilityModifiers, ConfigOptions.AccessibilityModifiers);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.RemoveAccessibilityModifiers, ConfigOptions.AccessibilityModifiers, ConfigOptionValues.AccessibilityModifiers_Implicit);
                    }

                    if (DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray, ConfigOptions.ArrayCreationTypeStyle);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.UseImplicitlyTypedArray, ConfigOptions.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Implicit);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.UseImplicitlyTypedArrayWhenTypeIsObvious, ConfigOptions.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious);
                    }

                    if (DiagnosticRules.UseBlockBodyOrExpressionBody.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportMissingRequiredOption(configOptions, DiagnosticRules.UseBlockBodyOrExpressionBody, ConfigOptions.BodyStyle);
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.ConvertExpressionBodyToBlockBody, ConfigOptions.BodyStyle, ConfigOptionValues.BodyStyle_Block);
                    }

                    if (DiagnosticRules.RemoveRedundantEmptyLine.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.RemoveEmptyLineBetweenClosingBraceAndSwitchSection, ConfigOptions.BlankLineBetweenClosingBraceAndSwitchSection, "false");
                    }

                    if (DiagnosticRules.RemoveUnusedMemberDeclaration.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                    {
                        context.ReportObsoleteOption(configOptions, LegacyConfigOptions.SuppressUnityScriptMethods, ConfigOptions.SuppressUnityScriptMethods, "true");
                    }
                });
            });
        }
    }
}
