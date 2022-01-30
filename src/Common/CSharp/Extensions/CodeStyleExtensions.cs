// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Configuration;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp
{
    internal static class CodeStyleExtensions
    {
        public static bool GetPrefixFieldIdentifierWithUnderscore(this AnalyzerConfigOptions configOptions)
        {
            if (configOptions.TryGetValueAsBool(ConfigOptions.PrefixFieldIdentifierWithUnderscore, out bool value))
                return value;

            if (CodeAnalysisConfig.Instance.PrefixFieldIdentifierWithUnderscore != null)
                return CodeAnalysisConfig.Instance.PrefixFieldIdentifierWithUnderscore.Value;

            if (configOptions.TryGetValueAsBool(LegacyConfigOptions.PrefixFieldIdentifierWithUnderscore, out value))
                return value;

            return ConfigOptionDefaultValues.PrefixFieldIdentifierWithUnderscore;
        }

        public static int GetMaxLineLength(this AnalyzerConfigOptions configOptions)
        {
            if (configOptions.TryGetValue(ConfigOptionKeys.MaxLineLength, out string rawValue)
                && int.TryParse(rawValue, out int value))
            {
                return value;
            }

            if (CodeAnalysisConfig.Instance.MaxLineLength != null)
                return CodeAnalysisConfig.Instance.MaxLineLength.Value;

            if (configOptions.TryGetValue(LegacyConfigOptions.MaxLineLength.Key, out rawValue)
                && int.TryParse(rawValue, out value))
            {
                return value;
            }

            return ConfigOptionDefaultValues.MaxLineLength;
        }

        public static bool? UseVarInsteadOfImplicitObjectCreation(this SyntaxNodeAnalysisContext context)
        {
            return ConfigOptions.GetValueAsBool(context.GetConfigOptions(), ConfigOptions.UseVarInsteadOfImplicitObjectCreation);
        }

        public static bool? PreferNewLineAtEndOfFile(this SyntaxNodeAnalysisContext context)
        {
            return context.GetConfigOptions().PreferNewLineAtEndOfFile();
        }

        public static bool? PreferNewLineAtEndOfFile(this AnalyzerConfigOptions configOptions)
        {
            return ConfigOptions.GetValueAsBool(configOptions, ConfigOptions.NewLineAtEndOfFile);
        }

        public static NewLineStyle GetNewLineBeforeWhileInDoStatement(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValueAsBool(configOptions, ConfigOptions.NewLineBeforeWhileInDoStatement, out bool addNewLine))
                return (addNewLine) ? NewLineStyle.Add : NewLineStyle.Remove;

            if (configOptions.TryGetValueAsBool(LegacyConfigOptions.RemoveNewLineBetweenClosingBraceAndWhileKeyword, out bool removeLine))
                return (removeLine) ? NewLineStyle.Remove : NewLineStyle.Add;

            return NewLineStyle.None;
        }

        public static NewLinePosition GetBinaryOperatorNewLinePosition(this AnalyzerConfigOptions configOptions)
        {
            if (TryGetNewLinePosition(configOptions, ConfigOptions.BinaryOperatorNewLine, out NewLinePosition newLinePosition))
                return newLinePosition;

            if (configOptions.IsEnabled(LegacyConfigOptions.AddNewLineAfterBinaryOperatorInsteadOfBeforeIt))
                return NewLinePosition.After;

            return NewLinePosition.None;
        }

        public static NewLinePosition GetConditionalOperatorNewLinePosition(this AnalyzerConfigOptions configOptions)
        {
            if (TryGetNewLinePosition(configOptions, ConfigOptions.ConditionalOperatorNewLine, out NewLinePosition newLinePosition))
                return newLinePosition;

            if (configOptions.IsEnabled(LegacyConfigOptions.AddNewLineAfterConditionalOperatorInsteadOfBeforeIt))
                return NewLinePosition.After;

            return NewLinePosition.None;
        }

        public static NewLinePosition GetArrowTokenNewLinePosition(this AnalyzerConfigOptions configOptions)
        {
            if (TryGetNewLinePosition(configOptions, ConfigOptions.ArrowTokenNewLine, out NewLinePosition newLinePosition))
                return newLinePosition;

            if (configOptions.IsEnabled(LegacyConfigOptions.AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt))
                return NewLinePosition.After;

            return NewLinePosition.None;
        }

        public static NewLinePosition GetEqualsTokenNewLinePosition(this AnalyzerConfigOptions configOptions)
        {
            if (TryGetNewLinePosition(configOptions, ConfigOptions.EqualsTokenNewLine, out NewLinePosition newLinePosition))
                return newLinePosition;

            if (configOptions.IsEnabled(LegacyConfigOptions.AddNewLineAfterEqualsSignInsteadOfBeforeIt))
                return NewLinePosition.After;

            return NewLinePosition.None;
        }

        public static NewLinePosition GetNullConditionalOperatorNewLinePosition(this AnalyzerConfigOptions configOptions, NewLinePosition defaultValue = NewLinePosition.None)
        {
            return (TryGetNewLinePosition(configOptions, ConfigOptions.NullConditionalOperatorNewLine, out NewLinePosition newLinePosition))
                ? newLinePosition
                : defaultValue;
        }

        public static UsingDirectiveBlankLineStyle GetBlankLineBetweenUsingDirectives(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.BlankLineBetweenUsingDirectives, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.BlankLineBetweenUsingDirectives_Never, StringComparison.OrdinalIgnoreCase))
                {
                    return UsingDirectiveBlankLineStyle.Never;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.BlankLineBetweenUsingDirectives_SeparateGroups, StringComparison.OrdinalIgnoreCase))
                {
                    return UsingDirectiveBlankLineStyle.SeparateGroups;
                }
            }

            if (ConfigOptions.TryGetValueAsBool(configOptions, LegacyConfigOptions.RemoveEmptyLineBetweenUsingDirectivesWithDifferentRootNamespace, out bool removeLine))
                return (removeLine) ? UsingDirectiveBlankLineStyle.Never : UsingDirectiveBlankLineStyle.SeparateGroups;

            return UsingDirectiveBlankLineStyle.None;
        }

        public static AccessorBracesStyle GetAccessorBracesStyle(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.AccessorBracesStyle, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.AccessorBracesStyle_MultiLine, StringComparison.OrdinalIgnoreCase))
                {
                    return AccessorBracesStyle.MultiLine;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.AccessorBracesStyle_SingleLineWhenExpressionIsOnSingleLine, StringComparison.OrdinalIgnoreCase))
                {
                    return AccessorBracesStyle.SingleLineWhenExpressionIsOnSingleLine;
                }
            }

            return AccessorBracesStyle.None;
        }

        public static BlockBracesStyle GetBlockBracesStyle(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.BlockBracesStyle, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.BlockBracesStyle_MultiLine, StringComparison.OrdinalIgnoreCase))
                {
                    return BlockBracesStyle.MultiLine;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.BlockBracesStyle_SingleLineWhenEmpty, StringComparison.OrdinalIgnoreCase))
                {
                    return BlockBracesStyle.SingleLineWhenEmpty;
                }
            }

            return BlockBracesStyle.None;
        }

        public static BlankLineStyle GetBlankLineBetweenSingleLineAccessors(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValueAsBool(configOptions, ConfigOptions.BlankLineBetweenSingleLineAccessors, out bool addLine))
                return (addLine) ? BlankLineStyle.Add : BlankLineStyle.Remove;

            if (ConfigOptions.TryGetValueAsBool(configOptions, LegacyConfigOptions.RemoveEmptyLineBetweenSingleLineAccessors, out bool removeLine))
                return (removeLine) ? BlankLineStyle.Remove : BlankLineStyle.Add;

            return BlankLineStyle.None;
        }

        public static bool? PreferAnonymousFunctionOrMethodGroup(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.UseAnonymousFunctionOrMethodGroup, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.UseAnonymousFunctionOrMethodGroup_AnonymousFunction, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.UseAnonymousFunctionOrMethodGroup_MethodGroup, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (configOptions.IsEnabled(LegacyConfigOptions.ConvertMethodGroupToAnonymousFunction))
                return true;

            return null;
        }

        public static EnumFlagOperationStyle GetEnumHasFlagStyle(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.EnumHasFlagStyle, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.EnumHasFlagStyle_Method, StringComparison.OrdinalIgnoreCase))
                {
                    return EnumFlagOperationStyle.HasFlagMethod;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.EnumHasFlagStyle_Operator, StringComparison.OrdinalIgnoreCase))
                {
                    return EnumFlagOperationStyle.BitwiseOperator;
                }
            }

            if (configOptions.IsEnabled(LegacyConfigOptions.ConvertBitwiseOperationToHasFlagCall))
                return EnumFlagOperationStyle.HasFlagMethod;

            return EnumFlagOperationStyle.None;
        }

        public static ConfigureAwaitStyle GetConfigureAwaitStyle(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValueAsBool(configOptions, ConfigOptions.ConfigureAwait, out bool value))
            {
                return (value) ? ConfigureAwaitStyle.Include : ConfigureAwaitStyle.Omit;
            }

            if (configOptions.IsEnabled(LegacyConfigOptions.RemoveCallToConfigureAwait))
                return ConfigureAwaitStyle.Omit;

            return ConfigureAwaitStyle.None;
        }

        public static EmptyStringStyle GetEmptyStringStyle(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.EmptyStringStyle, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.EmptyStringStyle_Field, StringComparison.OrdinalIgnoreCase))
                    return EmptyStringStyle.Field;

                if (string.Equals(rawValue, ConfigOptionValues.EmptyStringStyle_Literal, StringComparison.OrdinalIgnoreCase))
                    return EmptyStringStyle.Literal;
            }

            if (configOptions.IsEnabled(LegacyConfigOptions.UseStringEmptyInsteadOfEmptyStringLiteral))
                return EmptyStringStyle.Field;

            return EmptyStringStyle.None;
        }

        public static NullCheckStyle GetNullCheckStyle(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.NullCheckStyle, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.NullCheckStyle_EqualityOperator, StringComparison.OrdinalIgnoreCase))
                    return NullCheckStyle.EqualityOperator;

                if (string.Equals(rawValue, ConfigOptionValues.NullCheckStyle_PatternMatching, StringComparison.OrdinalIgnoreCase))
                    return NullCheckStyle.PatternMatching;
            }

            if (configOptions.IsEnabled(LegacyConfigOptions.UseComparisonInsteadPatternMatchingToCheckForNull))
                return NullCheckStyle.EqualityOperator;

            return NullCheckStyle.None;
        }

        public static ConditionalExpressionParenthesesStyle GetConditionalExpressionParenthesesStyle(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.ConditionalOperatorConditionParenthesesStyle, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.ConditionalOperatorConditionParenthesesStyle_Include, StringComparison.OrdinalIgnoreCase))
                {
                    return ConditionalExpressionParenthesesStyle.Include;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.ConditionalOperatorConditionParenthesesStyle_Omit, StringComparison.OrdinalIgnoreCase))
                {
                    return ConditionalExpressionParenthesesStyle.Omit;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.ConditionalOperatorConditionParenthesesStyle_OmitWhenConditionIsSingleToken, StringComparison.OrdinalIgnoreCase))
                {
                    return ConditionalExpressionParenthesesStyle.OmitWhenConditionIsSingleToken;
                }
            }

            if (configOptions.IsEnabled(LegacyConfigOptions.RemoveParenthesesFromConditionOfConditionalExpressionWhenExpressionIsSingleToken))
                return ConditionalExpressionParenthesesStyle.OmitWhenConditionIsSingleToken;

            return ConditionalExpressionParenthesesStyle.None;
        }

        public static ObjectCreationParenthesesStyle GetObjectCreationParenthesesStyle(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.ObjectCreationParenthesesStyle, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.ObjectCreationParenthesesStyle_Include, StringComparison.OrdinalIgnoreCase))
                {
                    return ObjectCreationParenthesesStyle.Include;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.ObjectCreationParenthesesStyle_Omit, StringComparison.OrdinalIgnoreCase))
                {
                    return ObjectCreationParenthesesStyle.Omit;
                }
            }

            if (context.IsEnabled(LegacyConfigOptions.RemoveParenthesesWhenCreatingNewObject))
                return ObjectCreationParenthesesStyle.Omit;

            return ObjectCreationParenthesesStyle.None;
        }

        public static AccessibilityModifierStyle GetAccessModifiersStyle(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.AccessibilityModifiers, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.AccessibilityModifiers_Explicit, StringComparison.OrdinalIgnoreCase))
                {
                    return AccessibilityModifierStyle.Explicit;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.AccessibilityModifiers_Implicit, StringComparison.OrdinalIgnoreCase))
                {
                    return AccessibilityModifierStyle.Implicit;
                }
            }

            if (ConfigOptions.TryGetValueAsBool(configOptions, LegacyConfigOptions.RemoveAccessibilityModifiers, out bool useImplicit))
                return (useImplicit) ? AccessibilityModifierStyle.Implicit : AccessibilityModifierStyle.Explicit;

            return AccessibilityModifierStyle.None;
        }

        public static ObjectCreationTypeStyle GetObjectCreationTypeStyle(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.ObjectCreationTypeStyle, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.ObjectCreationTypeStyle_Implicit, StringComparison.OrdinalIgnoreCase))
                {
                    return ObjectCreationTypeStyle.Implicit;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.ObjectCreationTypeStyle_Explicit, StringComparison.OrdinalIgnoreCase))
                {
                    return ObjectCreationTypeStyle.Explicit;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious, StringComparison.OrdinalIgnoreCase))
                {
                    return ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious;
                }
            }

            return ObjectCreationTypeStyle.None;
        }

        public static ArrayCreationTypeStyle GetArrayCreationTypeStyle(this SyntaxNodeAnalysisContext context)
        {
            AnalyzerConfigOptions configOptions = context.GetConfigOptions();

            if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.ArrayCreationTypeStyle, out string rawValue))
            {
                if (string.Equals(rawValue, ConfigOptionValues.ArrayCreationTypeStyle_Implicit, StringComparison.OrdinalIgnoreCase))
                {
                    return ArrayCreationTypeStyle.Implicit;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.ArrayCreationTypeStyle_Explicit, StringComparison.OrdinalIgnoreCase))
                {
                    return ArrayCreationTypeStyle.Explicit;
                }
                else if (string.Equals(rawValue, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious, StringComparison.OrdinalIgnoreCase))
                {
                    return ArrayCreationTypeStyle.ImplicitWhenTypeIsObvious;
                }
            }

            if (context.IsEnabled(LegacyConfigOptions.UseImplicitlyTypedArrayWhenTypeIsObvious))
                return ArrayCreationTypeStyle.ImplicitWhenTypeIsObvious;

            if (context.IsEnabled(LegacyConfigOptions.UseImplicitlyTypedArray))
                return ArrayCreationTypeStyle.Implicit;

            return ArrayCreationTypeStyle.None;
        }

        public static BodyStyle GetBodyStyle(this SyntaxNodeAnalysisContext context)
        {
            return BodyStyle.Create(context);
        }

        public static bool? GetBlankLineBetweenClosingBraceAndSwitchSection(this SyntaxNodeAnalysisContext context)
        {
            if (ConfigOptions.TryGetValueAsBool(context.GetConfigOptions(), ConfigOptions.BlankLineBetweenClosingBraceAndSwitchSection, out bool value))
            {
                return value;
            }

            if (context.TryGetOptionAsBool(LegacyConfigOptions.RemoveEmptyLineBetweenClosingBraceAndSwitchSection, out value))
                return !value;

            return null;
        }

        public static bool? GetSuppressUnityScriptMethods(this SyntaxNodeAnalysisContext context)
        {
            if (ConfigOptions.TryGetValueAsBool(context.GetConfigOptions(), ConfigOptions.SuppressUnityScriptMethods, out bool value))
            {
                return value;
            }

            if (context.TryGetOptionAsBool(LegacyConfigOptions.SuppressUnityScriptMethods, out value))
                return value;

            return null;
        }

        public static NewLinePosition GetEqualsSignNewLinePosition(this SyntaxNodeAnalysisContext context)
        {
            return context.GetConfigOptions().GetEqualsTokenNewLinePosition();
        }

        public static NewLinePosition GetArrowTokenNewLinePosition(this SyntaxNodeAnalysisContext context)
        {
            return context.GetConfigOptions().GetArrowTokenNewLinePosition();
        }

        public static NewLinePosition GetConditionalExpressionNewLinePosition(this SyntaxNodeAnalysisContext context)
        {
            return context.GetConfigOptions().GetConditionalOperatorNewLinePosition();
        }

        public static NewLinePosition GetBinaryExpressionNewLinePosition(this SyntaxNodeAnalysisContext context)
        {
            return context.GetConfigOptions().GetBinaryOperatorNewLinePosition();
        }

        public static NewLinePosition GetNullConditionalOperatorNewLinePosition(this SyntaxNodeAnalysisContext context, NewLinePosition defaultValue = NewLinePosition.None)
        {
            return context.GetConfigOptions().GetNullConditionalOperatorNewLinePosition(defaultValue);
        }

        private static bool TryGetNewLinePosition(
            AnalyzerConfigOptions configOptions,
            ConfigOptionDescriptor option,
            out NewLinePosition newLinePosition)
        {
            if (ConfigOptions.TryGetValue(configOptions, option, out string rawValue))
            {
                if (string.Equals(rawValue, "before", StringComparison.OrdinalIgnoreCase))
                {
                    newLinePosition = NewLinePosition.Before;
                    return true;
                }
                else if (string.Equals(rawValue, "after", StringComparison.OrdinalIgnoreCase))
                {
                    newLinePosition = NewLinePosition.After;
                    return true;
                }
            }

            newLinePosition = NewLinePosition.None;
            return false;
        }
    }
}
