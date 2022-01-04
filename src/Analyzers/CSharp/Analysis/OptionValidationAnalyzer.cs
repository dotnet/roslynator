// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
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
                        DiagnosticRules.AddOrRemoveAccessibilityModifiers,
                        DiagnosticRules.AddOrRemoveParenthesesFromConditionInConditionalOperator,
                        DiagnosticRules.ConfigureAwait,
                        DiagnosticRules.IncludeParenthesesWhenCreatingNewObject,
                        DiagnosticRules.NormalizeNullCheck,
                        DiagnosticRules.RemoveUnusedMemberDeclaration,
                        DiagnosticRules.UseAnonymousFunctionOrMethodGroup,
                        DiagnosticRules.UseBlockBodyOrExpressionBody,
                        DiagnosticRules.UseEmptyStringLiteralOrStringEmpty,
                        DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
                        DiagnosticRules.UseHasFlagMethodOrBitwiseOperator,
                        DiagnosticRules.UseImplicitOrExplicitObjectCreation,
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
                var missingFlags = MissingFlags.None;
                var obsoleteFlags = ObsoleteFlags.None;

                CompilationOptions compilationOptions = compilationContext.Compilation.Options;

                compilationContext.RegisterSyntaxTreeAction(context =>
                {
                    if (compilationOptions.SyntaxTreeOptionsProvider.IsGenerated(context.Tree, compilationContext.CancellationToken) == GeneratedKind.MarkedGenerated)
                        return;

                    AnalyzerConfigOptions options = context.GetConfigOptions();

                    ValidateMissing(ref context, compilationOptions, options, MissingFlags.AddOrRemoveAccessibilityModifiers, ref missingFlags, DiagnosticRules.AddOrRemoveAccessibilityModifiers, ConfigOptions.AccessibilityModifiers);
                    ValidateMissing(ref context, compilationOptions, options, MissingFlags.AddOrRemoveParenthesesFromConditionInConditionalOperator, ref missingFlags, DiagnosticRules.AddOrRemoveParenthesesFromConditionInConditionalOperator, ConfigOptions.ConditionInConditionalOperatorParenthesesStyle);
                    ValidateMissing(ref context, compilationOptions, options, MissingFlags.ConfigureAwait, ref missingFlags, DiagnosticRules.ConfigureAwait, ConfigOptions.ConfigureAwait);
                    ValidateMissing(ref context, compilationOptions, options, MissingFlags.IncludeParenthesesWhenCreatingNewObject, ref missingFlags, DiagnosticRules.IncludeParenthesesWhenCreatingNewObject, ConfigOptions.ObjectCreationParenthesesStyle);
                    ValidateMissing(ref context, compilationOptions, options, MissingFlags.NormalizeNullCheck, ref missingFlags, DiagnosticRules.NormalizeNullCheck, ConfigOptions.NullCheckStyle);
                    ValidateMissing(ref context, compilationOptions, options, MissingFlags.UseAnonymousFunctionOrMethodGroup, ref missingFlags, DiagnosticRules.UseAnonymousFunctionOrMethodGroup, ConfigOptions.AnonymousFunctionOrMethodGroup);
                    ValidateMissing(ref context, compilationOptions, options, MissingFlags.UseBlockBodyOrExpressionBody, ref missingFlags, DiagnosticRules.UseBlockBodyOrExpressionBody, ConfigOptions.BodyStyle);
                    ValidateMissing(ref context, compilationOptions, options, MissingFlags.UseEmptyStringLiteralOrStringEmpty, ref missingFlags, DiagnosticRules.UseEmptyStringLiteralOrStringEmpty, ConfigOptions.EmptyStringStyle);
                    ValidateMissing(ref context, compilationOptions, options, MissingFlags.UseExplicitlyOrImplicitlyTypedArray, ref missingFlags, DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray, ConfigOptions.ArrayCreationTypeStyle);
                    ValidateMissing(ref context, compilationOptions, options, MissingFlags.UseHasFlagMethodOrBitwiseOperator, ref missingFlags, DiagnosticRules.UseHasFlagMethodOrBitwiseOperator, ConfigOptions.EnumHasFlagStyle);
                    ValidateMissing(ref context, compilationOptions, options, MissingFlags.UseImplicitOrExplicitObjectCreation, ref missingFlags, DiagnosticRules.UseImplicitOrExplicitObjectCreation, ConfigOptions.ObjectCreationTypeStyle);

                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.ConvertBitwiseOperationToHasFlagCall, ref obsoleteFlags, DiagnosticRules.UseHasFlagMethodOrBitwiseOperator, ConfigOptions.EnumHasFlagStyle, LegacyConfigOptions.ConvertBitwiseOperationToHasFlagCall, ConfigOptionValues.EnumHasFlagStyle_Method);
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.ConvertExpressionBodyToBlockBody, ref obsoleteFlags, DiagnosticRules.UseBlockBodyOrExpressionBody, ConfigOptions.BodyStyle, LegacyConfigOptions.ConvertExpressionBodyToBlockBody, ConfigOptionValues.BodyStyle_Block);
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, ref obsoleteFlags, DiagnosticRules.UseBlockBodyOrExpressionBody, ConfigOptions.BodyStyle, LegacyConfigOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, "true");
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, ref obsoleteFlags, DiagnosticRules.UseBlockBodyOrExpressionBody, ConfigOptions.BodyStyle, LegacyConfigOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, "true");
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.ConvertMethodGroupToAnonymousFunction, ref obsoleteFlags, DiagnosticRules.UseAnonymousFunctionOrMethodGroup, ConfigOptions.AnonymousFunctionOrMethodGroup, LegacyConfigOptions.ConvertMethodGroupToAnonymousFunction, ConfigOptionValues.AnonymousFunctionOrMethodGroup_AnonymousFunction);
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.RemoveCallToConfigureAwait, ref obsoleteFlags, DiagnosticRules.ConfigureAwait, ConfigOptions.ConfigureAwait, LegacyConfigOptions.RemoveCallToConfigureAwait, "false");
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.RemoveAccessibilityModifiers, ref obsoleteFlags, DiagnosticRules.AddOrRemoveAccessibilityModifiers, ConfigOptions.AccessibilityModifiers, LegacyConfigOptions.RemoveAccessibilityModifiers, ConfigOptionValues.AccessibilityModifiers_Implicit);
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.RemoveEmptyLineBetweenClosingBraceAndSwitchSection, ref obsoleteFlags, DiagnosticRules.RemoveRedundantEmptyLine, ConfigOptions.BlankLineBetweenClosingBraceAndSwitchSection, LegacyConfigOptions.RemoveEmptyLineBetweenClosingBraceAndSwitchSection, "false");
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.RemoveParenthesesFromConditionOfConditionalExpressionWhenExpressionIsSingleToken, ref obsoleteFlags, DiagnosticRules.AddOrRemoveParenthesesFromConditionInConditionalOperator, ConfigOptions.ConditionInConditionalOperatorParenthesesStyle, LegacyConfigOptions.RemoveParenthesesFromConditionOfConditionalExpressionWhenExpressionIsSingleToken, ConfigOptionValues.ConditionInConditionalExpressionParenthesesStyle_OmitWhenConditionIsSingleToken);
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.RemoveParenthesesWhenCreatingNewObject, ref obsoleteFlags, DiagnosticRules.UseImplicitOrExplicitObjectCreation, ConfigOptions.ObjectCreationParenthesesStyle, LegacyConfigOptions.RemoveParenthesesWhenCreatingNewObject, ConfigOptionValues.ObjectCreationParenthesesStyle_Omit);
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.SuppressUnityScriptMethods, ref obsoleteFlags, DiagnosticRules.RemoveUnusedMemberDeclaration, ConfigOptions.SuppressUnityScriptMethods, LegacyConfigOptions.SuppressUnityScriptMethods, "true");
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.UseComparisonInsteadPatternMatchingToCheckForNull, ref obsoleteFlags, DiagnosticRules.NormalizeNullCheck, ConfigOptions.NullCheckStyle, LegacyConfigOptions.UseComparisonInsteadPatternMatchingToCheckForNull, ConfigOptionValues.NullCheckStyle_EqualityOperator);
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.UseImplicitlyTypedArray, ref obsoleteFlags, DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray, ConfigOptions.ArrayCreationTypeStyle, LegacyConfigOptions.UseImplicitlyTypedArray, ConfigOptionValues.ArrayCreationTypeStyle_Implicit);
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.UseImplicitlyTypedArrayWhenTypeIsObvious, ref obsoleteFlags, DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray, ConfigOptions.ArrayCreationTypeStyle, LegacyConfigOptions.UseImplicitlyTypedArrayWhenTypeIsObvious, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious);
                    ValidateObsolete(ref context, compilationOptions, options, ObsoleteFlags.UseStringEmptyInsteadOfEmptyStringLiteral, ref obsoleteFlags, DiagnosticRules.UseEmptyStringLiteralOrStringEmpty, ConfigOptions.EmptyStringStyle, LegacyConfigOptions.UseStringEmptyInsteadOfEmptyStringLiteral, ConfigOptionValues.EmptyStringStyle_Field);
                });
            });
        }

        private static void ValidateMissing(
            ref SyntaxTreeAnalysisContext context,
            CompilationOptions compilationOptions,
            AnalyzerConfigOptions configOptions,
            MissingFlags flag,
            ref MissingFlags flags,
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
            ObsoleteFlags flag,
            ref ObsoleteFlags flags,
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
        private enum MissingFlags
        {
            None,
            AddOrRemoveAccessibilityModifiers,
            AddOrRemoveParenthesesFromConditionInConditionalOperator,
            ConfigureAwait,
            IncludeParenthesesWhenCreatingNewObject,
            NormalizeNullCheck,
            RemoveUnusedMemberDeclaration,
            UseAnonymousFunctionOrMethodGroup,
            UseBlockBodyOrExpressionBody,
            UseEmptyStringLiteralOrStringEmpty,
            UseExplicitlyOrImplicitlyTypedArray,
            UseHasFlagMethodOrBitwiseOperator,
            UseImplicitOrExplicitObjectCreation,
        }

        [Flags]
        private enum ObsoleteFlags
        {
            None,
            ConvertBitwiseOperationToHasFlagCall,
            ConvertExpressionBodyToBlockBody,
            ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine,
            ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine,
            ConvertMethodGroupToAnonymousFunction,
            RemoveAccessibilityModifiers,
            RemoveCallToConfigureAwait,
            RemoveEmptyLineBetweenClosingBraceAndSwitchSection,
            RemoveParenthesesFromConditionOfConditionalExpressionWhenExpressionIsSingleToken,
            RemoveParenthesesWhenCreatingNewObject,
            SuppressUnityScriptMethods,
            UseComparisonInsteadPatternMatchingToCheckForNull,
            UseImplicitlyTypedArray,
            UseImplicitlyTypedArrayWhenTypeIsObvious,
            UseStringEmptyInsteadOfEmptyStringLiteral,
        }
    }
}
