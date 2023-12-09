// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Configuration;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp;

internal static class CodeStyleExtensions
{
    public static bool TryGetTabLength(this AnalyzerConfigOptions configOptions, out int tabLength)
    {
        if (configOptions.TryGetValue(ConfigOptionKeys.TabLength, out string tabLengthStr)
            && int.TryParse(tabLengthStr, NumberStyles.None, CultureInfo.InvariantCulture, out tabLength))
        {
            return true;
        }

        tabLength = 0;
        return false;
    }

    public static bool TryGetIndentSize(this AnalyzerConfigOptions configOptions, out int indentSize)
    {
        if (configOptions.TryGetValue("indent_size", out string indentSizeStr)
            && int.TryParse(indentSizeStr, NumberStyles.None, CultureInfo.InvariantCulture, out indentSize))
        {
            return true;
        }

        indentSize = 0;
        return false;
    }

    public static bool TryGetIndentStyle(this AnalyzerConfigOptions configOptions, out IndentStyle indentStyle)
    {
        if (configOptions.TryGetValue("indent_style", out string indentStyleStr)
            && Enum.TryParse(indentStyleStr, ignoreCase: true, out indentStyle))
        {
            return true;
        }

        indentStyle = IndentStyle.Space;
        return false;
    }

    public static bool GetPrefixFieldIdentifierWithUnderscore(this AnalyzerConfigOptions configOptions)
    {
        if (configOptions.TryGetValueAsBool(ConfigOptions.PrefixFieldIdentifierWithUnderscore, out bool value))
            return value;

        if (CodeAnalysisConfig.Instance.PrefixFieldIdentifierWithUnderscore is not null)
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

        if (CodeAnalysisConfig.Instance.MaxLineLength is not null)
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

        return NewLineStyle.None;
    }

    public static NewLinePosition GetBinaryOperatorNewLinePosition(this AnalyzerConfigOptions configOptions)
    {
        if (TryGetNewLinePosition(configOptions, ConfigOptions.BinaryOperatorNewLine, out NewLinePosition newLinePosition))
            return newLinePosition;

        return NewLinePosition.None;
    }

    public static NewLinePosition GetConditionalOperatorNewLinePosition(this AnalyzerConfigOptions configOptions)
    {
        if (TryGetNewLinePosition(configOptions, ConfigOptions.ConditionalOperatorNewLine, out NewLinePosition newLinePosition))
            return newLinePosition;

        return NewLinePosition.None;
    }

    public static NewLinePosition GetArrowTokenNewLinePosition(this AnalyzerConfigOptions configOptions)
    {
        if (TryGetNewLinePosition(configOptions, ConfigOptions.ArrowTokenNewLine, out NewLinePosition newLinePosition))
            return newLinePosition;

        return NewLinePosition.None;
    }

    public static NewLinePosition GetEqualsTokenNewLinePosition(this AnalyzerConfigOptions configOptions)
    {
        if (TryGetNewLinePosition(configOptions, ConfigOptions.EqualsTokenNewLine, out NewLinePosition newLinePosition))
            return newLinePosition;

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

        return EnumFlagOperationStyle.None;
    }

    public static ConfigureAwaitStyle GetConfigureAwaitStyle(this SyntaxNodeAnalysisContext context)
    {
        AnalyzerConfigOptions configOptions = context.GetConfigOptions();

        if (ConfigOptions.TryGetValueAsBool(configOptions, ConfigOptions.ConfigureAwait, out bool value))
        {
            return (value) ? ConfigureAwaitStyle.Include : ConfigureAwaitStyle.Omit;
        }

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

        return AccessibilityModifierStyle.None;
    }

    public static TrailingCommaStyle GetTrailingCommaStyle(this SyntaxNodeAnalysisContext context)
    {
        AnalyzerConfigOptions configOptions = context.GetConfigOptions();

        if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.TrailingCommaStyle, out string rawValue))
        {
            if (string.Equals(rawValue, ConfigOptionValues.TrailingCommaStyle_Include, StringComparison.OrdinalIgnoreCase))
            {
                return TrailingCommaStyle.Include;
            }
            else if (string.Equals(rawValue, ConfigOptionValues.TrailingCommaStyle_Omit, StringComparison.OrdinalIgnoreCase))
            {
                return TrailingCommaStyle.Omit;
            }
            else if (string.Equals(rawValue, ConfigOptionValues.TrailingCommaStyle_OmitWhenSingleLine, StringComparison.OrdinalIgnoreCase))
            {
                return TrailingCommaStyle.OmitWhenSingleLine;
            }
        }

        return TrailingCommaStyle.None;
    }

    public static TypeStyle GetObjectCreationTypeStyle(this SyntaxNodeAnalysisContext context)
    {
        AnalyzerConfigOptions configOptions = context.GetConfigOptions();

        if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.ObjectCreationTypeStyle, out string rawValue))
        {
            if (string.Equals(rawValue, ConfigOptionValues.ObjectCreationTypeStyle_Implicit, StringComparison.OrdinalIgnoreCase))
            {
                return TypeStyle.Implicit;
            }
            else if (string.Equals(rawValue, ConfigOptionValues.ObjectCreationTypeStyle_Explicit, StringComparison.OrdinalIgnoreCase))
            {
                return TypeStyle.Explicit;
            }
            else if (string.Equals(rawValue, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious, StringComparison.OrdinalIgnoreCase))
            {
                return TypeStyle.ImplicitWhenTypeIsObvious;
            }
        }

        return TypeStyle.None;
    }

    public static bool? UseCollectionExpression(this SyntaxNodeAnalysisContext context)
    {
        return UseCollectionExpression(context.GetConfigOptions());
    }

    public static bool? UseCollectionExpression(this AnalyzerConfigOptions configOptions)
    {
        return ConfigOptions.GetValueAsBool(configOptions, ConfigOptions.UseCollectionExpression);
    }

    public static TypeStyle GetArrayCreationTypeStyle(this SyntaxNodeAnalysisContext context)
    {
        AnalyzerConfigOptions configOptions = context.GetConfigOptions();

        if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.ArrayCreationTypeStyle, out string rawValue))
        {
            if (string.Equals(rawValue, ConfigOptionValues.ArrayCreationTypeStyle_Implicit, StringComparison.OrdinalIgnoreCase))
            {
                return TypeStyle.Implicit;
            }
            else if (string.Equals(rawValue, ConfigOptionValues.ArrayCreationTypeStyle_Explicit, StringComparison.OrdinalIgnoreCase))
            {
                return TypeStyle.Explicit;
            }
            else if (string.Equals(rawValue, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious, StringComparison.OrdinalIgnoreCase))
            {
                return TypeStyle.ImplicitWhenTypeIsObvious;
            }
        }

        return TypeStyle.None;
    }

    public static InfiniteLoopStyle GetInfiniteLoopStyle(this SyntaxNodeAnalysisContext context)
    {
        return GetInfiniteLoopStyle(context.GetConfigOptions());
    }

    public static InfiniteLoopStyle GetInfiniteLoopStyle(this AnalyzerConfigOptions configOptions)
    {
        if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.InfiniteLoopStyle, out string rawValue))
        {
            if (string.Equals(rawValue, ConfigOptionValues.InfiniteLoopStyle_For, StringComparison.OrdinalIgnoreCase))
            {
                return InfiniteLoopStyle.ForStatement;
            }
            else if (string.Equals(rawValue, ConfigOptionValues.InfiniteLoopStyle_While, StringComparison.OrdinalIgnoreCase))
            {
                return InfiniteLoopStyle.WhileStatement;
            }
        }

        return InfiniteLoopStyle.None;
    }

    public static DocCommentSummaryStyle GetDocCommentSummaryStyle(this SyntaxNodeAnalysisContext context)
    {
        AnalyzerConfigOptions configOptions = context.GetConfigOptions();

        if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.DocCommentSummaryStyle, out string rawValue))
        {
            if (string.Equals(rawValue, ConfigOptionValues.DocCommentSummaryStyle_MultiLine, StringComparison.OrdinalIgnoreCase))
            {
                return DocCommentSummaryStyle.MultiLine;
            }
            else if (string.Equals(rawValue, ConfigOptionValues.DocCommentSummaryStyle_SingleLine, StringComparison.OrdinalIgnoreCase))
            {
                return DocCommentSummaryStyle.SingleLine;
            }
        }

        return DocCommentSummaryStyle.None;
    }

    public static EnumFlagValueStyle GetEnumFlagValueStyle(this SyntaxNodeAnalysisContext context)
    {
        return GetEnumFlagValueStyle(context.GetConfigOptions());
    }

    public static EnumFlagValueStyle GetEnumFlagValueStyle(this AnalyzerConfigOptions configOptions)
    {
        if (ConfigOptions.TryGetValue(configOptions, ConfigOptions.EnumFlagValueStyle, out string rawValue))
        {
            if (string.Equals(rawValue, ConfigOptionValues.EnumFlagValueStyle_DecimalNumber, StringComparison.OrdinalIgnoreCase))
            {
                return EnumFlagValueStyle.DecimalNumber;
            }
            else if (string.Equals(rawValue, ConfigOptionValues.EnumFlagValueStyle_ShiftOperator, StringComparison.OrdinalIgnoreCase))
            {
                return EnumFlagValueStyle.ShiftOperator;
            }
        }

        return EnumFlagValueStyle.None;
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

        return null;
    }

    public static BlankLineStyle GetBlankLineAfterFileScopedNamespaceDeclaration(this SyntaxNodeAnalysisContext context)
    {
        if (ConfigOptions.TryGetValueAsBool(context.GetConfigOptions(), ConfigOptions.BlankLineAfterFileScopedNamespaceDeclaration, out bool value))
        {
            return (value) ? BlankLineStyle.Add : BlankLineStyle.Remove;
        }

        return BlankLineStyle.None;
    }

    public static bool? IsUnityCodeAnalysisEnabled(this SyntaxNodeAnalysisContext context)
    {
        if (ConfigOptions.TryGetValueAsBool(context.GetConfigOptions(), ConfigOptions.UnityCodeAnalysisEnabled, out bool value))
            return value;

#pragma warning disable CS0618 // Type or member is obsolete
        if (ConfigOptions.TryGetValueAsBool(context.GetConfigOptions(), ConfigOptions.SuppressUnityScriptMethods, out value))
            return value;
#pragma warning restore CS0618 // Type or member is obsolete

        return null;
    }

    public static BlankLineBetweenSwitchSections GetBlankLineBetweenSwitchSections(this SyntaxNodeAnalysisContext context)
    {
        if (ConfigOptions.TryGetValue(context.GetConfigOptions(), ConfigOptions.BlankLineBetweenSwitchSections, out string rawValue))
        {
            if (string.Equals(rawValue, ConfigOptionValues.BlankLineBetweenSwitchSections_Include, StringComparison.OrdinalIgnoreCase))
                return BlankLineBetweenSwitchSections.Include;

            if (string.Equals(rawValue, ConfigOptionValues.BlankLineBetweenSwitchSections_Omit, StringComparison.OrdinalIgnoreCase))
                return BlankLineBetweenSwitchSections.Omit;

            if (string.Equals(rawValue, ConfigOptionValues.BlankLineBetweenSwitchSections_OmitAfterBlock, StringComparison.OrdinalIgnoreCase))
                return BlankLineBetweenSwitchSections.OmitAfterBlock;
        }

        return BlankLineBetweenSwitchSections.None;
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
