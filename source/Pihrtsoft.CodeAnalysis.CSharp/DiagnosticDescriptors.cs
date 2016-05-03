// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    //TODO: add parameter names
    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor DeclareExplicitType = new DiagnosticDescriptor(
            DiagnosticIdentifiers.DeclareExplicitType,
            "Declare explicit type (when the type is not obvious).",
            "Consider declaring explicit type (when the type is not obvious).",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor DeclareExplicitTypeEvenIfObvious = new DiagnosticDescriptor(
            DiagnosticIdentifiers.DeclareExplicitTypeEvenIfObvious,
            "Declare explicit type (even if the type is obvious).",
            "Consider declaring explicit type (even if the type is obvious).",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor DeclareExplicitTypeInForEach = new DiagnosticDescriptor(
            DiagnosticIdentifiers.DeclareExplicitTypeInForEach,
            "Declare explicit type in foreach (when the type is not obvious).",
            "Consider declaring explicit type in foreach (when the type is not obvious).",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor DeclareImplicitType = new DiagnosticDescriptor(
            DiagnosticIdentifiers.DeclareImplicitType,
            "Declare implicit type (when the type is obvious).",
            "Consider declaring implicit type (when the type is obvious).",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AddBracesToStatement = new DiagnosticDescriptor(
            DiagnosticIdentifiers.AddBracesToStatement,
            "Add braces to a statement.",
            "Consider adding braces to a statement.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveBracesFromStatement = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveBracesFromStatement,
            "Remove braces from a statement.",
            "Consider removing braces from a statement.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveBracesFromStatementFadeOut = RemoveBracesFromStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor AddBracesToIfElseChain = new DiagnosticDescriptor(
            DiagnosticIdentifiers.AddBracesToIfElseChain,
            "Add braces to if-else chain.",
            "Consider adding braces to if-else chain.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveBracesFromIfElseChain = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveBracesFromIfElseChain,
            "Remove braces from if-else chain.",
            "Consider removing braces from if-else chain.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RenamePrivateFieldAccordingToCamelCaseWithUnderscore = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RenamePrivateFieldAccordingToCamelCaseWithUnderscore,
            "Rename private field according to camel case with underscore.",
            "Consider renaming private field to camel case with underscore.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor DeclareEachAttributeSeparately = new DiagnosticDescriptor(
            DiagnosticIdentifiers.DeclareEachAttributeSeparately,
            "Declare each attribute separately.",
            "Consider declaring each attribute separately.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor RemoveEmptyAttributeArgumentList = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveEmptyAttributeArgumentList,
            "Remove empty attribute's argument list.",
            "Consider removing empty attribute's argument list.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBooleanComparison = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveRedundantBooleanComparison,
            "Remove redundant boolean comparison.",
            "Consider removing redundant boolean comparison.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBooleanComparisonFadeOut = RemoveRedundantBooleanComparison.CreateFadeOut();

        public static readonly DiagnosticDescriptor ReorderModifiers = new DiagnosticDescriptor(
            DiagnosticIdentifiers.ReorderModifiers,
            "Reorder modifiers.",
            "Consider reordering modifiers.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AddAccessModifier = new DiagnosticDescriptor(
            DiagnosticIdentifiers.AddAccessModifier,
            "Add access modifier.",
            "Consider adding access modifier.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor FormatBinaryOperatorOnNextLine = new DiagnosticDescriptor(
            DiagnosticIdentifiers.FormatBinaryOperatorOnNextLine,
            "Format binary operator on next line.",
            "Consider formatting binary operator on next line.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AddConstructorArgumentList = new DiagnosticDescriptor(
            DiagnosticIdentifiers.AddConstructorArgumentList,
            "Add constructor's argument list.",
            "Consider adding constructor's argument list.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AvoidEmbeddedStatement = new DiagnosticDescriptor(
            DiagnosticIdentifiers.AvoidEmbeddedStatement,
            "Avoid embedded statement.",
            "Consider adding braces to a statement.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatBlock = new DiagnosticDescriptor(
            DiagnosticIdentifiers.FormatBlock,
            "Format block.",
            "Consider formatting block.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor FormatAccessorList = new DiagnosticDescriptor(
            DiagnosticIdentifiers.FormatAccessorList,
            "Format accessor list.",
            "Consider formatting accessor list.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyLambdaExpression = new DiagnosticDescriptor(
            DiagnosticIdentifiers.SimplifyLambdaExpression,
            "Simplify lambda expression.",
            "Consider simplifying lambda expression.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyLambdaExpressionFadeOut = SimplifyLambdaExpression.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyLambdaExpressionParameterList = new DiagnosticDescriptor(
            DiagnosticIdentifiers.SimplifyLambdaExpressionParameterList,
            "Simplify lambda expression's parameter list.",
            "Consider simplifying lambda expression's parameter list.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor SimplifyLambdaExpressionParameterListFadeOut = SimplifyLambdaExpressionParameterList.CreateFadeOut();

        public static readonly DiagnosticDescriptor UseLambdaExpressionInsteadOfAnonymousMethod = new DiagnosticDescriptor(
            DiagnosticIdentifiers.UseLambdaExpressionInsteadOfAnonymousMethod,
            "Use lambda expression instead of anonymous method.",
            "Consider using lambda expression instead of anonymous method.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseLambdaExpressionInsteadOfAnonymousMethodFadeOut = UseLambdaExpressionInsteadOfAnonymousMethod.CreateFadeOut();

        public static readonly DiagnosticDescriptor UsePredefinedType = new DiagnosticDescriptor(
            DiagnosticIdentifiers.UsePredefinedType,
            "Use predefined type.",
            "Consider using predefined type.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidImplicitArrayCreation = new DiagnosticDescriptor(
            DiagnosticIdentifiers.AvoidImplicitArrayCreation,
            "Avoid implicit array creation.",
            "Consider declaring explicit type when creating an array.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseNameOfOperator = new DiagnosticDescriptor(
            DiagnosticIdentifiers.UseNameOfOperator,
            "Use nameof operator.",
            "Consider using nameof operator.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEmptyObjectInitializer = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveEmptyObjectInitializer,
            "Remove empty object initializer.",
            "Consider removing empty object initializer.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantParentheses = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveRedundantParentheses,
            "Remove redundant parentheses.",
            "Consider removing redundant parentheses.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantParenthesesFadeOut = RemoveRedundantParentheses.CreateFadeOut();

        public static readonly DiagnosticDescriptor FormatEachStatementOnSeparateLine = new DiagnosticDescriptor(
            DiagnosticIdentifiers.FormatEachStatementOnSeparateLine,
            "Format each statement on separate line.",
            "Consider formatting each statement on separate line.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatCaseLabelStatementOnSeparateLine = new DiagnosticDescriptor(
            DiagnosticIdentifiers.FormatCaseLabelStatementOnSeparateLine,
            "Format switch section's statement on separate line.",
            "Consider formatting switch section's statement on separate line.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatEachEnumMemberOnSeparateLine = new DiagnosticDescriptor(
            DiagnosticIdentifiers.FormatEachEnumMemberOnSeparateLine,
            "Format each enum member on separate line.",
            "Consider formatting each enum member on separate line.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveEmptyStatement = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveEmptyStatement,
            "Remove empty statement.",
            "Consider removing empty statement.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveTrailingWhitespace = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveTrailingWhitespace,
            "Remove trailing white-space.",
            "Consider removing trailing white-space.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBraces = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveRedundantBraces,
            "Remove redundant braces.",
            "Consider removing redundant braces.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBracesFadeOut = RemoveRedundantBraces.CreateFadeOut();

        public static readonly DiagnosticDescriptor RemoveEmptyElseClause = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveEmptyElseClause,
            "Remove empty else clause.",
            "Consider removing empty else clause.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor SimplifyBooleanComparison = new DiagnosticDescriptor(
            DiagnosticIdentifiers.SimplifyBooleanComparison,
            "Simplify boolean comparison.",
            "Consider simplifying boolean comparison.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyBooleanComparisonFadeOut = SimplifyBooleanComparison.CreateFadeOut();

        public static readonly DiagnosticDescriptor FormatEmbeddedStatementOnSeparateLine = new DiagnosticDescriptor(
            DiagnosticIdentifiers.FormatEmbeddedStatementOnSeparateLine,
            "Format embedded statement on separate line.",
            "Consider formatting embedded statement on separate line.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AddEmptyLineAfterEmbeddedStatement = new DiagnosticDescriptor(
            DiagnosticIdentifiers.AddEmptyLineAfterEmbeddedStatement,
            "Add empty line after embedded statement.",
            "Consider adding empty line after embedded statement.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor SimplifyNullableOfT = new DiagnosticDescriptor(
            DiagnosticIdentifiers.SimplifyNullableOfT,
            "Simplify Nullable<T> to T?.",
            "Consider simplifying Nullable<T> to T?.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AddParenthesesToConditionalExpressionCondition = new DiagnosticDescriptor(
            DiagnosticIdentifiers.AddParenthesesToConditionalExpressionCondition,
            "Add parentheses to conditional expression's condition.",
            "Consider adding parentheses to conditional expression's condition.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor ConvertForEachToFor = new DiagnosticDescriptor(
            DiagnosticIdentifiers.ConvertForEachToFor,
            "Convert foreach to for.",
            "Consider converting foreach to for.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor ConvertForEachToForFadeOut = ConvertForEachToFor.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidMultilineExpressionBody = new DiagnosticDescriptor(
            DiagnosticIdentifiers.AvoidMultilineExpressionBody,
            "Avoid multiline expression body.",
            "Consider expanding expression-bodied member.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor RemoveRedundantSealedModifier = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveRedundantSealedModifier,
            "Remove redundant 'sealed' modifier.",
            "Consider removing redundant 'sealed' modifier.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantCommaInInitializer = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveRedundantCommaInInitializer,
            "Remove redundant comma in initializer.",
            "Consider removing redundant comma in initializer.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantEmptyLine = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveRedundantEmptyLine,
            "Remove redundant empty line.",
            "Consider removing redundant empty line.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveEnumDefaultUnderlyingType = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveEnumDefaultUnderlyingType,
            "Remove enum's default underlying type.",
            "Consider removing enum's default underlying type.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemovePartialModifierFromTypeWithSinglePart = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemovePartialModifierFromTypeWithSinglePart,
            "Remove 'partial' modifier from type with a single part.",
            "Consider removing 'partial' modifier from type with a single part.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveOriginalExceptionFromThrowStatement = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveOriginalExceptionFromThrowStatement,
            "Remove original exception from throw statement.",
            "Consider removing original exception from throw statement.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor UseExpressionBodiedMember = new DiagnosticDescriptor(
            DiagnosticIdentifiers.UseExpressionBodiedMember,
            "Use expression-bodied member.",
            "Consider using expression bodied member.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseExpressionBodiedMemberFadeOut = UseExpressionBodiedMember.CreateFadeOut();

        public static readonly DiagnosticDescriptor MergeLocalDeclarationWithReturnStatement = new DiagnosticDescriptor(
            DiagnosticIdentifiers.MergeLocalDeclarationWithReturnStatement,
            "Merge local declaration with return statement.",
            "Consider merging local declaration with return statement.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeLocalDeclarationWithReturnStatementFadeOut = MergeLocalDeclarationWithReturnStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyNestedUsingStatement = new DiagnosticDescriptor(
            DiagnosticIdentifiers.SimplifyNestedUsingStatement,
            "Simplify nested using statement.",
            "Consider simplifying nested using statement.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyNestedUsingStatementFadeOut = SimplifyNestedUsingStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor AsyncMethodShouldHaveAsyncSuffix = new DiagnosticDescriptor(
            DiagnosticIdentifiers.AsyncMethodShouldHaveAsyncSuffix,
            "Add 'Async' suffix to asynchronous method name.",
            "Consider adding 'Async' suffix to asynchronous method name.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor NonAsyncMethodShouldNotHaveAsyncSuffix = new DiagnosticDescriptor(
            DiagnosticIdentifiers.NonAsyncMethodShouldNotHaveAsyncSuffix,
            "Remove 'Async' suffix from non-asynchronous method name.",
            "Consider removing 'Async' suffix from non-asynchronous method name.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor NonAsyncMethodShouldNotHaveAsyncSuffixFadeOut = NonAsyncMethodShouldNotHaveAsyncSuffix.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyElseClauseContainingIfStatement = new DiagnosticDescriptor(
            DiagnosticIdentifiers.SimplifyElseClauseContainingIfStatement,
            "Simplify else clause containing if statement.",
            "Consider simplifying else clause containing if statement.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyElseClauseContainingIfStatementFadeOut = SimplifyElseClauseContainingIfStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor RemoveSemicolonFromDeclaration = new DiagnosticDescriptor(
            DiagnosticIdentifiers.RemoveSemicolonFromDeclaration,
            "Remove semicolon from declaration.",
            "Consider removing semicolon from declaration.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidUsingAliasDirective = new DiagnosticDescriptor(
            DiagnosticIdentifiers.AvoidUsingAliasDirective,
            "Avoid using alias directive.",
            "Avoid using alias directive.", //TODO: improve messageFormat for AvoidUsingAliasDirective
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

#if DEBUG
        public static readonly DiagnosticDescriptor UseLinefeedAsNewLine = new DiagnosticDescriptor(
            DiagnosticIdentifiers.UseLinefeedAsNewLine,
            "Use linefeed as newline.",
            "Consider using linefeed as newline.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseCarriageReturnAndLinefeedAsNewLine = new DiagnosticDescriptor(
            DiagnosticIdentifiers.UseCarriageReturnAndLinefeedAsNewLine,
            "Use carriage return + linefeed as newline.",
            "Consider using carriage return + linefeed as newline.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseSpacesInsteadOfTab = new DiagnosticDescriptor(
            DiagnosticIdentifiers.UseSpacesInsteadOfTab,
            "Use spaces instead of tab.",
            "Consider using spaces instead of tab.",
            DiagnosticCategories.General,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );
#endif
    }
}
