// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

        public static readonly DiagnosticDescriptor UseExplicitTypeInsteadOfVar = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseExplicitTypeInsteadOfVar,
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

        public static readonly DiagnosticDescriptor UseVarInsteadOfExplicitType = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseVarInsteadOfExplicitType,
            title: "Use 'var' instead of explicit type (when the type is obvious).",
            messageFormat: "Use 'var' instead of explicit type.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor UseExplicitTypeInsteadOfVarEvenIfObvious = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarEvenIfObvious,
            title: "Use explicit type instead of 'var' (even if the type is obvious).",
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
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor FormatAccessorList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatAccessorList,
            title: "Format accessor list.",
            messageFormat: "Format accessor list.",
            category: DiagnosticCategories.Formatting,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
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

        public static readonly DiagnosticDescriptor RemoveRedundantBraces = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantBraces,
            title: "Remove redundant braces.",
            messageFormat: "Remove redundant braces.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBracesFadeOut = RemoveRedundantBraces.CreateFadeOut();

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
            messageFormat: "Remove redundant boolean literal.",
            category: DiagnosticCategories.Redundancy,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantSealedModifier = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantSealedModifier,
            title: "Remove redundant sealed modifier.",
            messageFormat: "Remove redundant sealed modifier.",
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
            title: "Remove partial modifier from type with a single part.",
            messageFormat: "Remove partial modifier from type with a single part.",
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

        public static readonly DiagnosticDescriptor ReplaceAnonymousMethodWithLambdaExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceAnonymousMethodWithLambdaExpression,
            title: "Replace anonymous method with lambda expression.",
            messageFormat: "Replace anonymous method with lambda expression.",
            category: DiagnosticCategories.Usage,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceAnonymousMethodWithLambdaExpressionFadeOut = ReplaceAnonymousMethodWithLambdaExpression.CreateFadeOut();

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

        public static readonly DiagnosticDescriptor MergeLocalDeclarationWithReturnStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeLocalDeclarationWithReturnStatement,
            title: "Merge local declaration with return statement.",
            messageFormat: "Merge local declaration with return statement.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeLocalDeclarationWithReturnStatementFadeOut = MergeLocalDeclarationWithReturnStatement.CreateFadeOut();

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
            messageFormat: "Use {0} operator.",
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

        public static readonly DiagnosticDescriptor ReplaceStringEmptyWithEmptyStringLiteral = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceStringEmptyWithEmptyStringLiteral,
            title: "Replace string.Empty with \"\".",
            messageFormat: "Replace string.Empty with \"\".",
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

        public static readonly DiagnosticDescriptor ReplaceAnyMethodWithCountOrLengthProperty = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceAnyMethodWithCountOrLengthProperty,
            title: "Replace 'Any' method with 'Count' or 'Length' property.",
            messageFormat: "Replace 'Any' method with '{0}' property.",
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

        public static readonly DiagnosticDescriptor ReplaceCountMethodWithCountOrLengthProperty = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceCountMethodWithCountOrLengthProperty,
            title: "Replace 'Count' method with 'Count' or 'Length' property.",
            messageFormat: "Replace 'Count' method with '{0}' property.",
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceCountMethodWithAnyMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceCountMethodWithAnyMethod,
            title: "Replace 'Count' method with 'Any' method.",
            messageFormat: "Replace 'Count' method with 'Any' method.",
            category: DiagnosticCategories.Performance,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceConditionalExpressionWithCoalesceExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceConditionalExpressionWithCoalesceExpression,
            title: "Replace conditional expression with coalesce expression.",
            messageFormat: "Replace conditional expression with coalesce expression.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplacePropertyWithAutoImplementedProperty = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplacePropertyWithAutoImplementedProperty,
            title: "Replace property with auto-implemented property.",
            messageFormat: "Replace property with auto-implemented property.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplacePropertyWithAutoImplementedPropertyFadeOut = ReplacePropertyWithAutoImplementedProperty.CreateFadeOut();

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

        public static readonly DiagnosticDescriptor AvoidUsageOfTab = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsageOfTab,
            title: "Avoid usage of tab.",
            messageFormat: "Replace tab with spaces.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UsePostfixUnaryOperatorInsteadOfAssignment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UsePostfixUnaryOperatorInsteadOfAssignment,
            title: "Use postfix unary operator instead of assignment.",
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
             defaultSeverity: DiagnosticSeverity.Info,
             isEnabledByDefault: true
        );

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

        public static readonly DiagnosticDescriptor UseBitwiseOperationInsteadOfHasFlagMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseBitwiseOperationInsteadOfHasFlagMethod,
            title: "Use bitwise operation instead of 'HasFlag' method.",
            messageFormat: "Use bitwise operation instead of 'HasFlag' method.",
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
            messageFormat: "Swap the left and right part of a binary expression so that 'null' is on the right side.",
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

        public static readonly DiagnosticDescriptor MarkClassAsStatic = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MarkClassAsStatic,
            title: "Mark class as static.",
            messageFormat: "Mark class as static.",
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
            title: "Add static modifier to all partial class declarations.",
            messageFormat: "Add static modifier to all partial class declarations.",
            category: DiagnosticCategories.Readability,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseCastMethodInsteadOfSelectMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseCastMethodInsteadOfSelectMethod,
            title: "Use 'Cast' method instead of 'Select' method.",
            messageFormat: "Use 'Cast' method instead of 'Select' method.",
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

        public static readonly DiagnosticDescriptor ReplaceReturnStatementWithExpressionStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceReturnStatementWithExpressionStatement,
            title: "Replace yield/return statement with expression statement.",
            messageFormat: "Replace {0} statement with expression statement.",
            category: DiagnosticCategories.ErrorFix,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AddBreakStatementToSwitchSection = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddBreakStatementToSwitchSection,
            title: "Add break statement to switch section.",
            messageFormat: "Add break statement to switch section.",
            category: DiagnosticCategories.ErrorFix,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AddReturnStatementThatReturnsDefaultValue = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddReturnStatementThatReturnsDefaultValue,
            title: "Add return statement that returns default value.",
            messageFormat: "Add return statement that returns default value.",
            category: DiagnosticCategories.ErrorFix,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MarkLocalVariableAsConst = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MarkLocalVariableAsConst,
            title: "Mark local variable as const.",
            messageFormat: "Mark local variable as const.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor CallFindMethodInsteadOfFirstOrDefaultMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.CallFindMethodInsteadOfFirstOrDefaultMethod,
            title: "Call 'Find' method instead of 'FirstOrDefault' method.",
            messageFormat: "Call 'Find' method instead of 'FirstOrDefault' method.",
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

        public static readonly DiagnosticDescriptor AddMissingSemicolon = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddMissingSemicolon,
            title: "Add missing semicolon.",
            messageFormat: "Add missing semicolon.",
            category: DiagnosticCategories.ErrorFix,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
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

        public static readonly DiagnosticDescriptor MarkMemberAsStatic = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MarkMemberAsStatic,
            title: "Mark member as static.",
            messageFormat: "Mark member as static.",
            category: DiagnosticCategories.ErrorFix,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AvoidEmbeddedStatementInIfElse = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidEmbeddedStatementInIfElse,
            title: "Avoid embedded statement in if-else.",
            messageFormat: "Add braces to {0}.",
            category: DiagnosticCategories.Style,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor MergeLocalDeclarationWithInitialization = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeLocalDeclarationWithInitialization,
            title: "Merge local declaration with initialization.",
            messageFormat: "Merge local declaration with initialization.",
            category: DiagnosticCategories.Simplification,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeLocalDeclarationWithInitializationFadeOut = MergeLocalDeclarationWithInitialization.CreateFadeOut();

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

        public static readonly DiagnosticDescriptor ReplaceReturnWithYieldReturn = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceReturnWithYieldReturn,
            title: "Replace return with yield return.",
            messageFormat: "Replace return with yield return.",
            category: DiagnosticCategories.ErrorFix,
            defaultSeverity: DiagnosticSeverity.Hidden,
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

        public static readonly DiagnosticDescriptor RemoveRedundantContinueStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantContinueStatement,
            title: "Remove redundant continue statement.",
            messageFormat: "Remove redundant continue statement.",
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

        public static readonly DiagnosticDescriptor AddDocumentationComment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddDocumentationComment,
            title: "Add documentation comment to publicly visible type or member.",
            messageFormat: "Add documentation comment to publicly visible type or member.",
            category: DiagnosticCategories.Maintainability,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
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
            defaultSeverity: DiagnosticSeverity.Info,
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

        public static readonly DiagnosticDescriptor MarkContainingClassAsAbstract = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MarkContainingClassAsAbstract,
            title: "Mark containing class as abstract.",
            messageFormat: "Mark containing class as abstract.",
            category: DiagnosticCategories.ErrorFix,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
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
    }
}