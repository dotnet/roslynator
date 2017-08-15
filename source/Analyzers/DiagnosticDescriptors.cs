// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp
{
    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor AddBraces = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.AddBraces,
             title: "Add braces.",
             messageFormat: "Add braces to {0}.",
             category: DiagnosticCategories.Style,
             defaultSeverity: DiagnosticSeverity.Info,
             isEnabledByDefault: true
         );

        public static readonly DiagnosticDescriptor RemoveBraces = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveBraces,
            title: "Remove braces.",
            messageFormat: "Remove braces from {0}.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor RemoveBracesFadeOut = RemoveBraces.CreateFadeOut();

        public static readonly DiagnosticDescriptor AddBracesToIfElse = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddBracesToIfElse,
            title: "Add braces to if-else.",
            messageFormat: "Add braces to if-else.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveBracesFromIfElse = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveBracesFromIfElse,
            title: "Remove braces from if-else.",
            messageFormat: "Remove braces from if-else.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor RemoveBracesFromIfElseFadeOut = RemoveBracesFromIfElse.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyNestedUsingStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyNestedUsingStatement,
            title: "Simplify nested using statement.",
            messageFormat: "Simplify nested using statement.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyNestedUsingStatementFadeOut = SimplifyNestedUsingStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor MergeElseClauseWithNestedIfStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeElseClauseWithNestedIfStatement,
            title: "Merge else clause with nested if statement.",
            messageFormat: "Merge else clause with nested if statement.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeElseClauseWithNestedIfStatementFadeOut = MergeElseClauseWithNestedIfStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidEmbeddedStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidEmbeddedStatement,
            title: "Avoid embedded statement.",
            messageFormat: "Add braces to {0}.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious,
            title: "Use explicit type instead of 'var' (when the type is not obvious).",
            messageFormat: "Use explicit type instead of 'var'.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseExplicitTypeInsteadOfVarInForEach = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarInForEach,
            title: "Use explicit type instead of 'var' (foreach variable).",
            messageFormat: "Use explicit type instead of 'var'.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseVarInsteadOfExplicitTypeWhenTypeIsObvious = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsObvious,
            title: "Use 'var' instead of explicit type (when the type is obvious).",
            messageFormat: "Use 'var' instead of explicit type.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor UseExplicitTypeInsteadOfVarWhenTypeIsObvious = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarWhenTypeIsObvious,
            title: "Use explicit type instead of 'var' (when the type is obvious).",
            messageFormat: "Use explicit type instead of 'var'.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UsePredefinedType = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UsePredefinedType,
            title: "Use predefined type.",
            messageFormat: "Use predefined type.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidImplicitlyTypedArray = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidImplicitlyTypedArray,
            title: "Avoid implicitly-typed array.",
            messageFormat: "Declare explicit type when creating an array.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseNameOfOperator = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseNameOfOperator,
            title: "Use nameof operator.",
            messageFormat: "Use nameof operator.",
            category: DiagnosticCategories.Maintainability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseNameOfOperatorFadeOut = UseNameOfOperator.CreateFadeOut();

        public static readonly DiagnosticDescriptor UseExpressionBodiedMember = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseExpressionBodiedMember,
            title: "Use expression-bodied member.",
            messageFormat: "Use expression bodied member.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseExpressionBodiedMemberFadeOut = UseExpressionBodiedMember.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidMultilineExpressionBody = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidMultilineExpressionBody,
            title: "Avoid multiline expression body.",
            messageFormat: "Expand expression body with multiline expression.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AddDefaultAccessModifier = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddDefaultAccessModifier,
            title: "Add default access modifier.",
            messageFormat: "Add default access modifier.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReorderModifiers = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReorderModifiers,
            title: "Reorder modifiers.",
            messageFormat: "Reorder modifiers.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor SimplifyNullableOfT = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyNullableOfT,
            title: "Simplify Nullable<T> to T?.",
            messageFormat: "Simplify Nullable<T> to T?.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor SimplifyLambdaExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyLambdaExpression,
            title: "Simplify lambda expression.",
            messageFormat: "Simplify lambda expression.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyLambdaExpressionFadeOut = SimplifyLambdaExpression.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyLambdaExpressionParameterList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyLambdaExpressionParameterList,
            title: "Simplify lambda expression parameter list.",
            messageFormat: "Simplify lambda expression parameter list.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor SimplifyLambdaExpressionParameterListFadeOut = SimplifyLambdaExpressionParameterList.CreateFadeOut();

        public static readonly DiagnosticDescriptor FormatEmptyBlock = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEmptyBlock,
            title: "Format empty block.",
            messageFormat: "Format empty block.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatAccessorList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatAccessorList,
            title: "Format accessor list.",
            messageFormat: "Format accessor list.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatEachEnumMemberOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEachEnumMemberOnSeparateLine,
            title: "Format each enum member on a separate line.",
            messageFormat: "Format each enum member on a separate line.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatEachStatementOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEachStatementOnSeparateLine,
            title: "Format each statement on a separate line.",
            messageFormat: "Format each statement on a separate line.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatEmbeddedStatementOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEmbeddedStatementOnSeparateLine,
            title: "Format embedded statement on a separate line.",
            messageFormat: "Format embedded statement on a separate line.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatSwitchSectionStatementOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatSwitchSectionStatementOnSeparateLine,
            title: "Format switch section's statement on a separate line.",
            messageFormat: "Format switch section's statement on a separate line.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatBinaryOperatorOnNextLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatBinaryOperatorOnNextLine,
            title: "Format binary operator on next line.",
            messageFormat: "Format binary operator on next line.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AddEmptyLineAfterEmbeddedStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddEmptyLineAfterEmbeddedStatement,
            title: "Add empty line after embedded statement.",
            messageFormat: "Add empty line after embedded statement.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor RemoveRedundantParentheses = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantParentheses,
            title: "Remove redundant parentheses.",
            messageFormat: "Remove redundant parentheses.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantParenthesesFadeOut = RemoveRedundantParentheses.CreateFadeOut();

        public static readonly DiagnosticDescriptor RemoveRedundantBooleanLiteral = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantBooleanLiteral,
            title: "Remove redundant boolean literal.",
            messageFormat: "Remove redundant '{0}'.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantSealedModifier = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantSealedModifier,
            title: "Remove redundant 'sealed' modifier.",
            messageFormat: "Remove redundant 'sealed' modifier.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantCommaInInitializer = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantCommaInInitializer,
            title: "Remove redundant comma in initializer.",
            messageFormat: "Remove redundant comma in initializer.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantEmptyLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantEmptyLine,
            title: "Remove redundant empty line.",
            messageFormat: "Remove redundant empty line.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveTrailingWhitespace = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveTrailingWhitespace,
            title: "Remove trailing white-space.",
            messageFormat: "Remove trailing white-space.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveEmptyStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyStatement,
            title: "Remove empty statement.",
            messageFormat: "Remove empty statement.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEmptyAttributeArgumentList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyAttributeArgumentList,
            title: "Remove empty attribute argument list.",
            messageFormat: "Remove empty attribute argument list.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEmptyElseClause = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyElseClause,
            title: "Remove empty else clause.",
            messageFormat: "Remove empty else clause.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEmptyInitializer = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyInitializer,
            title: "Remove empty initializer.",
            messageFormat: "Remove empty initializer.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEnumDefaultUnderlyingType = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEnumDefaultUnderlyingType,
            title: "Remove enum default underlying type.",
            messageFormat: "Remove enum default underlying type.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemovePartialModifierFromTypeWithSinglePart = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemovePartialModifierFromTypeWithSinglePart,
            title: "Remove 'partial' modifier from type with a single part.",
            messageFormat: "Remove 'partial' modifier from type with a single part.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveOriginalExceptionFromThrowStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveOriginalExceptionFromThrowStatement,
            title: "Remove original exception from throw statement.",
            messageFormat: "Remove original exception from throw statement.",
            category: DiagnosticCategories.Maintainability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RenamePrivateFieldAccordingToCamelCaseWithUnderscore = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RenamePrivateFieldAccordingToCamelCaseWithUnderscore,
            title: "Rename private field according to camel case with underscore.",
            messageFormat: "Rename private field to camel case with underscore.",
            category: DiagnosticCategories.Naming,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AsynchronousMethodNameShouldEndWithAsync = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync,
            title: "Asynchronous method name should end with 'Async'.",
            messageFormat: "Add suffix 'Async' to asynchronous method name.",
            category: DiagnosticCategories.Naming,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor NonAsynchronousMethodNameShouldNotEndWithAsync = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync,
            title: "Non-asynchronous method name should not end with 'Async'.",
            messageFormat: "Remove suffix 'Async' from non-asynchronous method name.",
            category: DiagnosticCategories.Naming,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor NonAsynchronousMethodNameShouldNotEndWithAsyncFadeOut = NonAsynchronousMethodNameShouldNotEndWithAsync.CreateFadeOut();

        public static readonly DiagnosticDescriptor UseLambdaExpressionInsteadOfAnonymousMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseLambdaExpressionInsteadOfAnonymousMethod,
            title: "Use lambda expression instead of anonymous method.",
            messageFormat: "Use lambda expression instead of anonymous method.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseLambdaExpressionInsteadOfAnonymousMethodFadeOut = UseLambdaExpressionInsteadOfAnonymousMethod.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyBooleanComparison = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyBooleanComparison,
            title: "Simplify boolean comparison.",
            messageFormat: "Simplify boolean comparison.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyBooleanComparisonFadeOut = SimplifyBooleanComparison.CreateFadeOut();

        public static readonly DiagnosticDescriptor AddConstructorArgumentList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddConstructorArgumentList,
            title: "Add constructor argument list.",
            messageFormat: "Add constructor argument list.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor ParenthesizeConditionInConditionalExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ParenthesizeConditionInConditionalExpression,
            title: "Parenthesize condition in conditional expression.",
            messageFormat: "Parenthesize condition in conditional expression.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor DeclareEachAttributeSeparately = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DeclareEachAttributeSeparately,
            title: "Declare each attribute separately.",
            messageFormat: "Declare each attribute separately.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AvoidSemicolonAtEndOfDeclaration = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidSemicolonAtEndOfDeclaration,
            title: "Avoid semicolon at the end of declaration.",
            messageFormat: "Remove semicolon from the end of declaration.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidUsageOfUsingAliasDirective = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsageOfUsingAliasDirective,
            title: "Avoid usage of using alias directive.",
            messageFormat: "Avoid usage of using alias directive.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AddEmptyLineBetweenDeclarations = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations,
            title: "Add empty line between declarations.",
            messageFormat: "Add empty line between declarations.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseCompoundAssignment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseCompoundAssignment,
            title: "Use compound assignment.",
            messageFormat: "Use compound assignment.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseCompoundAssignmentFadeOut = UseCompoundAssignment.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidLockingOnPubliclyAccessibleInstance = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidLockingOnPubliclyAccessibleInstance,
            title: "Avoid locking on publicly accessible instance.",
            messageFormat: "Lock on private field instead of locking on '{0}'.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor DeclareEachTypeInSeparateFile = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DeclareEachTypeInSeparateFile,
            title: "Declare each type in separate file.",
            messageFormat: "Declare each type in separate file.",
            category: DiagnosticCategories.Maintainability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor MergeIfStatementWithNestedIfStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeIfStatementWithNestedIfStatement,
            title: "Merge if statement with nested if statement.",
            messageFormat: "Merge if statement with nested if statement.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeIfStatementWithNestedIfStatementFadeOut = MergeIfStatementWithNestedIfStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidInterpolatedStringWithNoInterpolation = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidInterpolatedStringWithNoInterpolation,
            title: "Avoid interpolated string with no interpolation.",
            messageFormat: "Remove '$' from interpolated string with no interpolation.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidUsageOfDoStatementToCreateInfiniteLoop = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsageOfDoStatementToCreateInfiniteLoop,
            title: "Avoid usage of do statement to create an infinite loop.",
            messageFormat: "Use while statement to create an infinite loop.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AvoidUsageOfForStatementToCreateInfiniteLoop = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsageOfForStatementToCreateInfiniteLoop,
            title: "Avoid usage of for statement to create an infinite loop.",
            messageFormat: "Use while statement to create an infinite loop.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AvoidUsageOfWhileStatementToCreateInfiniteLoop = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsageOfWhileStatementToCreateInfiniteLoop,
            title: "Avoid usage of while statement to create an inifinite loop.",
            messageFormat: "Use for statement to create an infinite loop.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor RemoveEmptyFinallyClause = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyFinallyClause,
            title: "Remove empty finally clause.",
            messageFormat: "Remove empty finally clause.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEmptyArgumentList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyArgumentList,
            title: "Remove empty argument list.",
            messageFormat: "Remove empty argument list.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor SimplifyLogicalNotExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyLogicalNotExpression,
            title: "Simplify logical not expression.",
            messageFormat: "Simplify logical not expression.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveUnnecessaryCaseLabel = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveUnnecessaryCaseLabel,
            title: "Remove unnecessary case label.",
            messageFormat: "Remove unnecessary case label.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantDefaultSwitchSection = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantDefaultSwitchSection,
            title: "Remove redundant default switch section.",
            messageFormat: "Remove redundant default switch section.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBaseConstructorCall = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantBaseConstructorCall,
            title: "Remove redundant base constructor call.",
            messageFormat: "Remove redundant base constructor call.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEmptyNamespaceDeclaration = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyNamespaceDeclaration,
            title: "Remove empty namespace declaration.",
            messageFormat: "Remove empty namespace declaration.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor ReplaceIfStatementWithReturnStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceIfStatementWithReturnStatement,
            title: "Replace if statement with return statement.",
            messageFormat: "Replace if statement with return statement.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor ReplaceIfStatementWithReturnStatementFadeOut = ReplaceIfStatementWithReturnStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor RemoveRedundantConstructor = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantConstructor,
            title: "Remove redundant constructor.",
            messageFormat: "Remove redundant constructor.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidEmptyCatchClauseThatCatchesSystemException = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidEmptyCatchClauseThatCatchesSystemException,
            title: "Avoid empty catch clause that catches System.Exception.",
            messageFormat: "Avoid empty catch clause that catches System.Exception.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor FormatDeclarationBraces = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatDeclarationBraces,
            title: "Format declaration braces.",
            messageFormat: "Format declaration braces.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyLinqMethodChain = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyLinqMethodChain,
            title: "Simplify LINQ method chain.",
            messageFormat: "Simplify LINQ method chain.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseEmptyStringLiteralInsteadOfStringEmpty = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseEmptyStringLiteralInsteadOfStringEmpty,
            title: "Use \"\" instead of string.Empty.",
            messageFormat: "Use \"\" instead of string.Empty",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor ThrowingOfNewNotImplementedException = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ThrowingOfNewNotImplementedException,
            title: "Throwing of new NotImplementedException.",
            messageFormat: "Implement the functionality instead of throwing new NotImplementedException.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseCountOrLengthPropertyInsteadOfAnyMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfAnyMethod,
            title: "Use 'Count/Length' property instead of 'Any' method.",
            messageFormat: "Use '{0}' property instead of 'Any' method.",
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SplitVariableDeclaration = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SplitVariableDeclaration,
            title: "Split variable declaration.",
            messageFormat: "Split variable declaration.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseCountOrLengthPropertyInsteadOfCountMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfCountMethod,
            title: "Use 'Count/Length' property instead of 'Count' method.",
            messageFormat: "Use '{0}' property instead of 'Count' method.",
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseAnyMethodInsteadOfCountMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseAnyMethodInsteadOfCountMethod,
            title: "Use 'Any' method instead of 'Count' method.",
            messageFormat: "Use 'Any' method instead of 'Count' method.",
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseCoalesceExpressionInsteadOfConditionalExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseCoalesceExpressionInsteadOfConditionalExpression,
            title: "Use coalesce expression instead of conditional expression.",
            messageFormat: "Use coalesce expression instead of conditional expression.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseAutoProperty = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseAutoProperty,
            title: "Use auto-implemented property.",
            messageFormat: "Use auto-implemented property.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseAutoPropertyFadeOut = UseAutoProperty.CreateFadeOut();

        public static readonly DiagnosticDescriptor UseLinefeedAsNewLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseLinefeedAsNewLine,
            title: "Use linefeed as newline.",
            messageFormat: "Use linefeed as newline.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseCarriageReturnAndLinefeedAsNewLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseCarriageReturnAndLinefeedAsNewLine,
            title: "Use carriage return + linefeed as newline.",
            messageFormat: "Use carriage return + linefeed as newline.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseSpacesInsteadOfTab = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseSpacesInsteadOfTab,
            title: "Use space(s) instead of tab.",
            messageFormat: "Use space(s) instead of tab.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UsePostfixUnaryOperatorInsteadOfAssignment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UsePostfixUnaryOperatorInsteadOfAssignment,
            title: "Use --/++ operator instead of assignment.",
            messageFormat: "Use {0} operator instead of assignment.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UsePostfixUnaryOperatorInsteadOfAssignmentFadeOut = UsePostfixUnaryOperatorInsteadOfAssignment.CreateFadeOut();

        public static readonly DiagnosticDescriptor CallConfigureAwait = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.CallConfigureAwait,
             title: "Call 'ConfigureAwait(false)'.",
             messageFormat: "Call 'ConfigureAwait(false).",
             category: DiagnosticCategories.Design,
             defaultSeverity: DiagnosticSeverity.Info,
             isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveEmptyRegion = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.RemoveEmptyRegion,
             title: "Remove empty region.",
             messageFormat: "Remove empty region.",
             category: DiagnosticCategories.Redundancy,
             defaultSeverity: DiagnosticSeverity.Hidden,
             isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveEmptyRegionFadeOut = RemoveEmptyRegion.CreateFadeOut();

        public static readonly DiagnosticDescriptor AddEmptyLineAfterLastStatementInDoStatement = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.AddEmptyLineAfterLastStatementInDoStatement,
             title: "Add empty line after last statement in do statement.",
             messageFormat: "Add empty line after last statement in do statement.",
             category: DiagnosticCategories.Formatting,
             defaultSeverity: DiagnosticSeverity.Info,
             isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor RemoveFileWithNoCode = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.RemoveFileWithNoCode,
             title: "Remove file with no code.",
             messageFormat: "Remove file with no code.",
             category: DiagnosticCategories.Redundancy,
             defaultSeverity: DiagnosticSeverity.Info,
             isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor DeclareUsingDirectiveOnTopLevel = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.DeclareUsingDirectiveOnTopLevel,
             title: "Declare using directive on top level.",
             messageFormat: "Declare using directive on top level.",
             category: DiagnosticCategories.Readability,
             defaultSeverity: DiagnosticSeverity.Warning,
             isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseCSharp6DictionaryInitializer = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseCSharp6DictionaryInitializer,
            title: "Use C# 6.0 dictionary initializer.",
            messageFormat: "Use C# 6.0 dictionary initializer.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseBitwiseOperationInsteadOfCallingHasFlag = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag,
            title: "Use bitwise operation instead of calling 'HasFlag'.",
            messageFormat: "Use bitwise operation instead of calling 'HasFlag'.",
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantToStringCall = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantToStringCall,
            title: "Remove redundant 'ToString' call.",
            messageFormat: "Remove redundant 'ToString' call.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidNullLiteralExpressionOnLeftSideOfBinaryExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidNullLiteralExpressionOnLeftSideOfBinaryExpression,
            title: "Avoid 'null' on the left side of a binary expression.",
            messageFormat: "'null' should be on the right side of a binary expression.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor DefaultLabelShouldBeLastLabelInSwitchSection = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DefaultLabelShouldBeLastLabelInSwitchSection,
            title: "Default label should be last label in switch section.",
            messageFormat: "Move default label to last position in switch section.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor FormatDocumentationSummaryOnSingleLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatDocumentationSummaryOnSingleLine,
            title: "Format documentation summary on a single line.",
            messageFormat: "Format documentation summary on a single line.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatDocumentationSummaryOnMultipleLines = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatDocumentationSummaryOnMultipleLines,
            title: "Format documentation summary on multiple lines.",
            messageFormat: "Format documentation summary on multiple lines.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor MakeClassStatic = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MakeClassStatic,
            title: "Make class static.",
            messageFormat: "Make class static.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceIfStatementWithAssignment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceIfStatementWithAssignment,
            title: "Replace if statement with assignment.",
            messageFormat: "Replace if statement with assignment.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyConditionalExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyConditionalExpression,
            title: "Simplify conditional expression.",
            messageFormat: "Simplify conditional expression.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeInterpolationIntoInterpolatedString = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeInterpolationIntoInterpolatedString,
            title: "Merge interpolation into interpolated string.",
            messageFormat: "Merge interpolation into interpolated string.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveEmptyDestructor = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyDestructor,
            title: "Remove empty destructor.",
            messageFormat: "Remove empty destructor.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantStringToCharArrayCall = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantStringToCharArrayCall,
            title: "Remove redundant 'ToCharArray' call.",
            messageFormat: "Remove redundant 'ToCharArray' call.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AddStaticModifierToAllPartialClassDeclarations = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddStaticModifierToAllPartialClassDeclarations,
            title: "Add 'static' modifier to all partial class declarations.",
            messageFormat: "Add 'static' modifier to all partial class declarations.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor CallCastInsteadOfSelect = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.CallCastInsteadOfSelect,
            title: "Call 'Enumerable.Cast' instead of 'Enumerable.Select'.",
            messageFormat: "Call 'Enumerable.Cast' instead of 'Enumerable.Select'.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor DeclareTypeInsideNamespace = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DeclareTypeInsideNamespace,
            title: "Declare type inside namespace.",
            messageFormat: "Declare '{0}' inside namespace.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AddBracesToSwitchSectionWithMultipleStatements = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddBracesToSwitchSectionWithMultipleStatements,
            title: "Add braces to switch section with multiple statements.",
            messageFormat: "Add braces to switch section with multiple statements.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor CombineEnumerableWhereMethodChain = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.CombineEnumerableWhereMethodChain,
            title: "Combine 'Enumerable.Where' method chain.",
            messageFormat: "Combine 'Enumerable.Where' method chain.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor CombineEnumerableWhereMethodChainFadeOut = CombineEnumerableWhereMethodChain.CreateFadeOut();

        public static readonly DiagnosticDescriptor UseStringIsNullOrEmptyMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseStringIsNullOrEmptyMethod,
            title: "Use 'string.IsNullOrEmpty' method.",
            messageFormat: "Use 'string.IsNullOrEmpty' method.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantDelegateCreation = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantDelegateCreation,
            title: "Remove redundant delegate creation.",
            messageFormat: "Remove redundant delegate creation.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantDelegateCreationFadeOut = RemoveRedundantDelegateCreation.CreateFadeOut();

        public static readonly DiagnosticDescriptor MarkLocalVariableAsConst = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MarkLocalVariableAsConst,
            title: "Mark local variable as const.",
            messageFormat: "Mark local variable as const.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor CallFindInsteadOfFirstOrDefault = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.CallFindInsteadOfFirstOrDefault,
            title: "Call 'Find' instead of 'FirstOrDefault'.",
            messageFormat: "Call 'Find' instead of 'FirstOrDefault'.",
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseElementAccessInsteadOfElementAt = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseElementAccessInsteadOfElementAt,
            title: "Use [] instead of calling 'ElementAt'.",
            messageFormat: "Use [] instead of calling 'ElementAt'.",
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseElementAccessInsteadOfFirst = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseElementAccessInsteadOfFirst,
            title: "Use [] instead of calling 'First'.",
            messageFormat: "Use [] instead of calling 'First'.",
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AddParenthesesAccordingToOperatorPrecedence = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddParenthesesAccordingToOperatorPrecedence,
            title: "Add parentheses according to operator precedence.",
            messageFormat: "Add parentheses according to operator precedence.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor InlineLocalVariable = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.InlineLocalVariable,
            title: "Inline local variable.",
            messageFormat: "Inline local variable.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor InlineLocalVariableFadeOut = InlineLocalVariable.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidEmbeddedStatementInIfElse = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidEmbeddedStatementInIfElse,
            title: "Avoid embedded statement in if-else.",
            messageFormat: "Add braces to {0}.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor MergeLocalDeclarationWithAssignment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeLocalDeclarationWithAssignment,
            title: "Merge local declaration with assignment.",
            messageFormat: "Merge local declaration with assignment.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeLocalDeclarationWithAssignmentFadeOut = MergeLocalDeclarationWithAssignment.CreateFadeOut();

        public static readonly DiagnosticDescriptor UseCoalesceExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseCoalesceExpression,
            title: "Use coalesce expression.",
            messageFormat: "Use coalesce expression.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantFieldInitialization = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantFieldInitialization,
            title: "Remove redundant field initalization.",
            messageFormat: "Remove redundant field initialization.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor BitwiseOperationOnEnumWithoutFlagsAttribute = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.BitwiseOperationOnEnumWithoutFlagsAttribute,
            title: "Bitwise operation on enum without Flags attribute.",
            messageFormat: "Bitwise operation on enum without Flags attribute.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantOverridingMember = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantOverridingMember,
            title: "Remove redundant overriding member.",
            messageFormat: "Remove redundant overriding {0}.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantDisposeOrCloseCall = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantDisposeOrCloseCall,
            title: "Remove redundant Dispose/Close call.",
            messageFormat: "Remove redundant '{0}' call.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantStatement,
            title: "Remove redundant statement.",
            messageFormat: "Remove redundant statement.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor DeclareEnumMemberWithZeroValue = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DeclareEnumMemberWithZeroValue,
            title: "Declare enum member with zero value (when enum has FlagsAttribute).",
            messageFormat: "Declare enum member with zero value (when enum has FlagsAttribute).",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeSwitchSectionsWithEquivalentContent = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeSwitchSectionsWithEquivalentContent,
            title: "Merge switch sections with equivalent content.",
            messageFormat: "Merge switch sections with equivalent content.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AddSummaryToDocumentationComment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddSummaryToDocumentationComment,
            title: "Add summary to documentation comment.",
            messageFormat: "Add summary to documentation comment.",
            category: DiagnosticCategories.Maintainability,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AddSummaryElementToDocumentationComment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddSummaryElementToDocumentationComment,
            title: "Add summary element to documentation comment.",
            messageFormat: "Add summary element to documentation comment.",
            category: DiagnosticCategories.Maintainability,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AddExceptionToDocumentationComment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddExceptionToDocumentationComment,
            title: "Add exception to documentation comment.",
            messageFormat: "Add exception to documentation comment.",
            category: DiagnosticCategories.Maintainability,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AddParameterToDocumentationComment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddParameterToDocumentationComment,
            title: "Add parameter to documentation comment.",
            messageFormat: "Add parameter to documentation comment.",
            category: DiagnosticCategories.Maintainability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AddTypeParameterToDocumentationComment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddTypeParameterToDocumentationComment,
            title: "Add type parameter to documentation comment.",
            messageFormat: "Add type parameter to documentation comment.",
            category: DiagnosticCategories.Maintainability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyCoalesceExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyCoalesceExpression,
            title: "Simplify coalesce expression.",
            messageFormat: "Simplify coalesce expression.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantAsOperator = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantAsOperator,
            title: "Remove redundant 'as' operator.",
            messageFormat: "Remove redundant 'as' operator.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor UseConditionalAccess = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseConditionalAccess,
            title: "Use conditional access.",
            messageFormat: "Use conditional access.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor CallStringConcatInsteadOfStringJoin = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.CallStringConcatInsteadOfStringJoin,
            title: "Call string.Concat instead of string.Join.",
            messageFormat: "Call string.Concat instead of string.Join.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantCast = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantCast,
            title: "Remove redundant cast.",
            messageFormat: "Remove redundant cast.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AddEmptyLineAfterClosingBrace = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddEmptyLineAfterClosingBrace,
            title: "Add empty line after closing brace.",
            messageFormat: "Add empty line after closing brace.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor SortEnumMembers = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SortEnumMembers,
            title: "Sort enum members.",
            messageFormat: "Sort '{0}' members.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseStringComparison = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseStringComparison,
            title: "Use StringComparison when comparing strings.",
            messageFormat: "Use StringComparison when comparing strings.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseStringLengthInsteadOfComparisonWithEmptyString = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseStringLengthInsteadOfComparisonWithEmptyString,
            title: "Use string.Length instead of comparison with empty string.",
            messageFormat: "Use string.Length instead of comparison with empty string.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor CompositeEnumValueContainsUndefinedFlag = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.CompositeEnumValueContainsUndefinedFlag,
            title: "Composite enum value contains undefined flag.",
            messageFormat: "Composite enum value contains undefined flag {0}.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor StaticMemberInGenericTypeShouldUseTypeParameter = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.StaticMemberInGenericTypeShouldUseTypeParameter,
            title: "Static member in generic type should use a type parameter.",
            messageFormat: "Static member in generic type should use a type parameter.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseGenericEventHandler = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseGenericEventHandler,
            title: "Use EventHandler<T>.",
            messageFormat: "Use EventHandler<T>.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AbstractTypeShouldNotHavePublicConstructors = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AbstractTypeShouldNotHavePublicConstructors,
            title: "Abstract type should not have public constructors.",
            messageFormat: "Abstract type should not have public constructors.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor EnumMemberShouldDeclareExplicitValue = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.EnumMemberShouldDeclareExplicitValue,
            title: "Enum member should declare explicit value.",
            messageFormat: "Enum member should declare explicit value.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AvoidChainOfAssignments = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidChainOfAssignments,
            title: "Avoid chain of assignments.",
            messageFormat: "Avoid chain of assignments.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UnusedParameter = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UnusedParameter,
            title: "Unused parameter.",
            messageFormat: "Unused parameter '{0}'.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor UnusedTypeParameter = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UnusedTypeParameter,
            title: "Unused type parameter.",
            messageFormat: "Unused type parameter '{0}'.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor UnconstrainedTypeParameterCheckedForNull = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UnconstrainedTypeParameterCheckedForNull,
            title: "Unconstrained type parameter checked for null.",
            messageFormat: "Unconstrained type parameter checked for null.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ValueTypeObjectIsNeverEqualToNull = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ValueTypeObjectIsNeverEqualToNull,
            title: "Value type object is never equal to null.",
            messageFormat: "Value type object is never equal to null.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ParameterNameDiffersFromBase = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ParameterNameDiffersFromBase,
            title: "Parameter name differs from base name.",
            messageFormat: "Parameter name '{0}' differs from base name '{1}'.",
            category: DiagnosticCategories.Maintainability,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MarkFieldAsReadOnly = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MarkFieldAsReadOnly,
            title: "Mark field as read-only.",
            messageFormat: "Mark field as read-only.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseReadOnlyAutoProperty = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseReadOnlyAutoProperty,
            title: "Use read-only auto-implemented property.",
            messageFormat: "Use read-only auto-implemented property.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor SimplifyLazilyInitializedProperty = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyLazilyInitializedProperty,
            title: "Simplify lazily initialized property.",
            messageFormat: "Simplify lazily initialized property.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseIsOperatorInsteadOfAsOperator = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseIsOperatorInsteadOfAsOperator,
            title: "Use is operator instead of as operator.",
            messageFormat: "Use is operator instead of as operator.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseCoalesceExpressionInsteadOfIf = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseCoalesceExpressionInsteadOfIf,
            title: "Use coalesce expression instead of if.",
            messageFormat: "Use coalesce expression instead of if.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantAsyncAwait = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantAsyncAwait,
            title: "Remove redundant async/await.",
            messageFormat: "Remove redundant async/await.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantAsyncAwaitFadeOut = RemoveRedundantAsyncAwait.CreateFadeOut();

        public static readonly DiagnosticDescriptor UnusedThisParameter = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UnusedThisParameter,
            title: "Unused this parameter.",
            messageFormat: "Unused this parameter '{0}'.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious,
            title: "Use 'var' instead of explicit type (when the type is not obvious).",
            messageFormat: "Use 'var' instead of explicit type.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseVarInsteadOfExplicitTypeInForEach = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeInForEach,
            title: "Use 'var' instead of explicit type (in foreach).",
            messageFormat: "Use 'var' instead of explicit type.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor CallDebugFailInsteadOfDebugAssert = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.CallDebugFailInsteadOfDebugAssert,
            title: "Call Debug.Fail instead of Debug.Assert.",
            messageFormat: "Call Debug.Fail instead of Debug.Assert.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseReturnInsteadOfAssignment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseReturnInsteadOfAssignment,
            title: "Use return instead of assignment.",
            messageFormat: "Use return instead of assignment.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor InlineLazyInitialization = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.InlineLazyInitialization,
            title: "Inline lazy initialization.",
            messageFormat: "Inline lazy initialization.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceCommentWithDocumentationComment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceCommentWithDocumentationComment,
            title: "Replace comment with documentation comment.",
            messageFormat: "Replace comment with documentation comment.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBaseInterface = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantBaseInterface,
            title: "Remove redundant base interface.",
            messageFormat: "Interface '{0}' is already implemented by '{1}'.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor FormatInitializerWithSingleExpressionOnSingleLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatInitializerWithSingleExpressionOnSingleLine,
            title: "Format initializer with single expression on single line.",
            messageFormat: "Format initializer with single expression on single line.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor FormatConditionalExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatConditionalExpression,
            title: "Format conditional expression (format ? and : on next line).",
            messageFormat: "Format conditional expression.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AvoidSingleLineBlock = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidSingleLineBlock,
            title: "Avoid single-line block.",
            messageFormat: "Avoid single-line block.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseRegexInstanceInsteadOfStaticMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseRegexInstanceInsteadOfStaticMethod,
            title: "Use Regex instance instead of static method.",
            messageFormat: "Use Regex instance instead of static method.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseConstantInsteadOfField = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseConstantInsteadOfField,
            title: "Use constant instead of field.",
            messageFormat: "Use constant instead of field.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantAutoPropertyInitialization = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantAutoPropertyInitialization,
            title: "Remove redundant auto-property initialization.",
            messageFormat: "Remove redundant auto-property initialization.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AddOrRemoveRegionName = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddOrRemoveRegionName,
            title: "Add or remove region name.",
            messageFormat: "{0} region name {1} #endregion.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor JoinStringExpressions = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.JoinStringExpressions,
            title: "Join string expressions.",
            messageFormat: "Join string expressions.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor DeclareEnumValueAsCombinationOfNames = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DeclareEnumValueAsCombinationOfNames,
            title: "Declare enum value as combination of names.",
            messageFormat: "Declare enum value as combination of names.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseRegularStringLiteralInsteadOfVerbatimStringLiteral = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseRegularStringLiteralInsteadOfVerbatimStringLiteral,
            title: "Use regular string literal instead of verbatim string literal.",
            messageFormat: "Use regular string literal instead of verbatim string literal.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor OverridingMemberCannotChangeParamsModifier = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.OverridingMemberCannotChangeParamsModifier,
            title: "Overriding member cannot change 'params' modifier.",
            messageFormat: "Overriding member cannot change 'params' modifier.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ImplementExceptionConstructors = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ImplementExceptionConstructors,
            title: "Implement exception constructors.",
            messageFormat: "Implement exception constructors.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseExclusiveOrOperator = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseExclusiveOrOperator,
            title: "Use ^ operator.",
            messageFormat: "Use ^ operator.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor CallExtensionMethodAsInstanceMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.CallExtensionMethodAsInstanceMethod,
            title: "Call extension method as instance method.",
            messageFormat: "Call extension method as instance method.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor OptimizeStringBuilderAppendCall = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.OptimizeStringBuilderAppendCall,
            title: "Optimize StringBuilder.Append/AppendLine call.",
            messageFormat: "Optimize StringBuilder.{0} call.",
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AvoidBoxingOfValueType = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidBoxingOfValueType,
            title: "Avoid unnecessary boxing of value type.",
            messageFormat: "Avoid unnecessary boxing of value type.",
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor SimplifyBooleanExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyBooleanExpression,
            title: "Simplify boolean expression.",
            messageFormat: "Simplify boolean expression.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor CallThenByInsteadOfOrderBy = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.CallThenByInsteadOfOrderBy,
            title: "Call 'Enumerable.ThenBy' instead of 'Enumerable.OrderBy'.",
            messageFormat: "Call 'Enumerable.ThenBy{0}' instead of 'Enumerable.OrderBy{0}'.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseMethodChaining = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseMethodChaining,
            title: "Use method chaining.",
            messageFormat: "Use method chaining.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AvoidNullReferenceException = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidNullReferenceException,
            title: "Avoid NullReferenceException.",
            messageFormat: "Avoid NullReferenceException.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseAttributeUsageAttribute = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseAttributeUsageAttribute,
            title: "Use AttributeUsageAttribute.",
            messageFormat: "Use AttributeUsageAttribute.",
            category: DiagnosticCategories.Design,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseEventArgsEmpty = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseEventArgsEmpty,
            title: "Use EventArgs.Empty.",
            messageFormat: "Use EventArgs.Empty.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReorderNamedArguments = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReorderNamedArguments,
            title: "Reorder named arguments according to the order of parameters.",
            messageFormat: "Reorder named arguments according to the order of parameters.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseConditionalAccessInsteadOfConditionalExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseConditionalAccessInsteadOfConditionalExpression,
            title: "Use conditional access instead of conditional expression.",
            messageFormat: "Use conditional access instead of conditional expression.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseMethodGroupInsteadOfAnonymousFunction = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction,
            title: "Use method group instead of anonymous function.",
            messageFormat: "Use method group instead of anonymous function.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseMethodGroupInsteadOfAnonymousFunctionFadeOut = UseMethodGroupInsteadOfAnonymousFunction.CreateFadeOut();

        public static readonly DiagnosticDescriptor ReduceIfNesting = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReduceIfNesting,
            title: "Reduce if nesting.",
            messageFormat: "Reduce if nesting.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );
    }
}