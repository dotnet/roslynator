// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp
{
    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor AddBraces = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.AddBraces,
             title: "Add braces.",
             messageFormat: "Consider adding braces to {0}.",
             category: DiagnosticCategories.General,
             defaultSeverity: DiagnosticSeverity.Info,
             isEnabledByDefault: true
         );

        public static readonly DiagnosticDescriptor RemoveBraces = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveBraces,
            title: "Remove braces.",
            messageFormat: "Consider removing braces from {0}.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor RemoveBracesFadeOut = RemoveBraces.CreateFadeOut();

        public static readonly DiagnosticDescriptor AddBracesToIfElse = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddBracesToIfElse,
            title: "Add braces to if-else.",
            messageFormat: "Consider adding braces to if-else.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveBracesFromIfElse = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveBracesFromIfElse,
            title: "Remove braces from if-else.",
            messageFormat: "Consider removing braces from if-else.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor RemoveBracesFromIfElseFadeOut = RemoveBracesFromIfElse.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyNestedUsingStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyNestedUsingStatement,
            title: "Simplify nested using statement.",
            messageFormat: "Consider simplifying nested using statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyNestedUsingStatementFadeOut = SimplifyNestedUsingStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor MergeElseClauseWithNestedIfStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeElseClauseWithNestedIfStatement,
            title: "Merge else clause with nested if statement.",
            messageFormat: "Consider merging else clause with nested if statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeElseClauseWithNestedIfStatementFadeOut = MergeElseClauseWithNestedIfStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidEmbeddedStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidEmbeddedStatement,
            title: "Avoid embedded statement.",
            messageFormat: "Consider adding braces to {0}.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseExplicitTypeInsteadOfVar = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseExplicitTypeInsteadOfVar,
            title: "Use explicit type instead of 'var' (when the type is not obvious).",
            messageFormat: "Consider using explicit type instead of 'var'.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor UseExplicitTypeInsteadOfVarInForEach = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarInForEach,
            title: "Use explicit type instead of 'var' (foreach variable).",
            messageFormat: "Consider using explicit type instead of 'var'.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor UseVarInsteadOfExplicitType = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseVarInsteadOfExplicitType,
            title: "Use 'var' instead of explicit type (when the type is obvious).",
            messageFormat: "Consider using 'var' instead of explicit type.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor UseExplicitTypeInsteadOfVarEvenIfObvious = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarEvenIfObvious,
            title: "Use explicit type instead of 'var' (even if the type is obvious).",
            messageFormat: "Consider using explicit type instead of 'var'.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor UsePredefinedType = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UsePredefinedType,
            title: "Use predefined type.",
            messageFormat: "Consider using predefined type.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidImplicitlyTypedArray = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidImplicitlyTypedArray,
            title: "Avoid implicitly-typed array.",
            messageFormat: "Consider declaring explicit type when creating an array.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseNameOfOperator = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseNameOfOperator,
            title: "Use 'nameof' operator.",
            messageFormat: "Consider using 'nameof' operator.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseNameOfOperatorFadeOut = UseNameOfOperator.CreateFadeOut();

        public static readonly DiagnosticDescriptor UseExpressionBodiedMember = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseExpressionBodiedMember,
            title: "Use expression-bodied member.",
            messageFormat: "Consider using expression bodied member.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseExpressionBodiedMemberFadeOut = UseExpressionBodiedMember.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidMultilineExpressionBody = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidMultilineExpressionBody,
            title: "Avoid multiline expression body.",
            messageFormat: "Consider expanding expression-bodied member.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AddDefaultAccessModifier = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddDefaultAccessModifier,
            title: "Add default access modifier.",
            messageFormat: "Consider adding default access modifier.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReorderModifiers = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReorderModifiers,
            title: "Reorder modifiers.",
            messageFormat: "Consider reordering modifiers.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor SimplifyNullableOfT = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyNullableOfT,
            title: "Simplify Nullable<T> to T?.",
            messageFormat: "Consider simplifying Nullable<T> to T?.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor SimplifyLambdaExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyLambdaExpression,
            title: "Simplify lambda expression.",
            messageFormat: "Consider simplifying lambda expression.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyLambdaExpressionFadeOut = SimplifyLambdaExpression.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyLambdaExpressionParameterList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyLambdaExpressionParameterList,
            title: "Simplify lambda expression parameter list.",
            messageFormat: "Consider simplifying lambda expression parameter list.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor SimplifyLambdaExpressionParameterListFadeOut = SimplifyLambdaExpressionParameterList.CreateFadeOut();

        public static readonly DiagnosticDescriptor FormatEmptyBlock = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEmptyBlock,
            title: "Format empty block.",
            messageFormat: "Consider formatting empty block.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor FormatAccessorList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatAccessorList,
            title: "Format accessor list.",
            messageFormat: "Consider formatting accessor list.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor FormatEachEnumMemberOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEachEnumMemberOnSeparateLine,
            title: "Format each enum member on a separate line.",
            messageFormat: "Consider formatting each enum member on a separate line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatEachStatementOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEachStatementOnSeparateLine,
            title: "Format each statement on a separate line.",
            messageFormat: "Consider formatting each statement on a separate line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatEmbeddedStatementOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEmbeddedStatementOnSeparateLine,
            title: "Format embedded statement on a separate line.",
            messageFormat: "Consider formatting embedded statement on a separate line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatSwitchSectionStatementOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatSwitchSectionStatementOnSeparateLine,
            title: "Format switch section's statement on a separate line.",
            messageFormat: "Consider formatting switch section's statement on a separate line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatBinaryOperatorOnNextLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatBinaryOperatorOnNextLine,
            title: "Format binary operator on next line.",
            messageFormat: "Consider formatting binary operator on next line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AddEmptyLineAfterEmbeddedStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddEmptyLineAfterEmbeddedStatement,
            title: "Add empty line after embedded statement.",
            messageFormat: "Consider adding empty line after embedded statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBraces = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantBraces,
            title: "Remove redundant braces.",
            messageFormat: "Consider removing redundant braces.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBracesFadeOut = RemoveRedundantBraces.CreateFadeOut();

        public static readonly DiagnosticDescriptor RemoveRedundantParentheses = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantParentheses,
            title: "Remove redundant parentheses.",
            messageFormat: "Consider removing redundant parentheses.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantParenthesesFadeOut = RemoveRedundantParentheses.CreateFadeOut();

        public static readonly DiagnosticDescriptor RemoveRedundantBooleanLiteral = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantBooleanLiteral,
            title: "Remove redundant boolean literal.",
            messageFormat: "Consider removing redundant boolean literal.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBooleanLiteralFadeOut = RemoveRedundantBooleanLiteral.CreateFadeOut();

        public static readonly DiagnosticDescriptor RemoveRedundantSealedModifier = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantSealedModifier,
            title: "Remove redundant 'sealed' modifier.",
            messageFormat: "Consider removing redundant 'sealed' modifier.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantCommaInInitializer = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantCommaInInitializer,
            title: "Remove redundant comma in initializer.",
            messageFormat: "Consider removing redundant comma in initializer.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantEmptyLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantEmptyLine,
            title: "Remove redundant empty line.",
            messageFormat: "Consider removing redundant empty line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveTrailingWhitespace = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveTrailingWhitespace,
            title: "Remove trailing white-space.",
            messageFormat: "Consider removing trailing white-space.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveEmptyStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyStatement,
            title: "Remove empty statement.",
            messageFormat: "Consider removing empty statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEmptyAttributeArgumentList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyAttributeArgumentList,
            title: "Remove empty attribute argument list.",
            messageFormat: "Consider removing empty attribute argument list.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEmptyElseClause = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyElseClause,
            title: "Remove empty else clause.",
            messageFormat: "Consider removing empty else clause.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEmptyInitializer = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyInitializer,
            title: "Remove empty initializer.",
            messageFormat: "Consider removing empty initializer.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEnumDefaultUnderlyingType = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEnumDefaultUnderlyingType,
            title: "Remove enum default underlying type.",
            messageFormat: "Consider removing enum default underlying type.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemovePartialModifierFromTypeWithSinglePart = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemovePartialModifierFromTypeWithSinglePart,
            title: "Remove 'partial' modifier from type with a single part.",
            messageFormat: "Consider removing 'partial' modifier from type with a single part.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveOriginalExceptionFromThrowStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveOriginalExceptionFromThrowStatement,
            title: "Remove original exception from throw statement.",
            messageFormat: "Consider removing original exception from throw statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RenamePrivateFieldAccordingToCamelCaseWithUnderscore = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RenamePrivateFieldAccordingToCamelCaseWithUnderscore,
            title: "Rename private field according to camel case with underscore.",
            messageFormat: "Consider renaming private field to camel case with underscore.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AsynchronousMethodNameShouldEndWithAsync = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync,
            title: "Asynchronous method name should end with 'Async'.",
            messageFormat: "Consider adding 'Async' to asynchronous method name.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor NonAsynchronousMethodNameShouldNotEndWithAsync = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync,
            title: "Non-asynchronous method name should not end with 'Async'.",
            messageFormat: "Consider removing 'Async' from non-asynchronous method name.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor NonAsynchronousMethodNameShouldNotEndWithAsyncFadeOut = NonAsynchronousMethodNameShouldNotEndWithAsync.CreateFadeOut();

        public static readonly DiagnosticDescriptor ReplaceAnonymousMethodWithLambdaExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceAnonymousMethodWithLambdaExpression,
            title: "Replace anonymous method with lambda expression.",
            messageFormat: "Consider replacing with anonymous method lambda expression.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceAnonymousMethodWithLambdaExpressionFadeOut = ReplaceAnonymousMethodWithLambdaExpression.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyBooleanComparison = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyBooleanComparison,
            title: "Simplify boolean comparison.",
            messageFormat: "Consider simplifying boolean comparison.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyBooleanComparisonFadeOut = SimplifyBooleanComparison.CreateFadeOut();

        public static readonly DiagnosticDescriptor AddConstructorArgumentList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddConstructorArgumentList,
            title: "Add constructor argument list.",
            messageFormat: "Consider adding constructor argument list.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor WrapConditionalExpressionConditionInParentheses = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.WrapConditionalExpressionConditionInParentheses,
            title: "Wrap conditional expression condition in parentheses.",
            messageFormat: "Consider wrapping conditional expression condition in parentheses.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor DeclareEachAttributeSeparately = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DeclareEachAttributeSeparately,
            title: "Declare each attribute separately.",
            messageFormat: "Consider declaring each attribute separately.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor MergeLocalDeclarationWithReturnStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeLocalDeclarationWithReturnStatement,
            title: "Merge local declaration with return statement.",
            messageFormat: "Consider merging local declaration with return statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeLocalDeclarationWithReturnStatementFadeOut = MergeLocalDeclarationWithReturnStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidSemicolonAtEndOfDeclaration = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidSemicolonAtEndOfDeclaration,
            title: "Avoid semicolon at the end of declaration.",
            messageFormat: "Consider removing semicolon from the end of declaration.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidUsageOfUsingAliasDirective = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsageOfUsingAliasDirective,
            title: "Avoid usage of using alias directive.",
            messageFormat: "Avoid usage of using alias directive.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AddEmptyLineBetweenDeclarations = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations,
            title: "Add empty line between declarations.",
            messageFormat: "Consider adding empty line between declarations.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyAssignmentExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyAssignmentExpression,
            title: "Simplify assignment expression.",
            messageFormat: "Consider simplifying assignment expression.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyAssignmentExpressionFadeOut = SimplifyAssignmentExpression.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidLockingOnPubliclyAccessibleInstance = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidLockingOnPubliclyAccessibleInstance,
            title: "Avoid locking on publicly accessible instance.",
            messageFormat: "Consider locking on private field instead of locking on '{0}'.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor DeclareEachTypeInSeparateFile = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DeclareEachTypeInSeparateFile,
            title: "Declare each type in separate file.",
            messageFormat: "Consider declaring each type in separate file.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor MergeIfStatementWithNestedIfStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeIfStatementWithNestedIfStatement,
            title: "Merge if statement with nested if statement.",
            messageFormat: "Consider merging if statement with nested if statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeIfStatementWithNestedIfStatementFadeOut = MergeIfStatementWithNestedIfStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidInterpolatedStringWithNoInterpolation = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidInterpolatedStringWithNoInterpolation,
            title: "Avoid interpolated string with no interpolation.",
            messageFormat: "Consider removing '$' from interpolated string with no interpolation.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidUsageOfDoStatementToCreateInfiniteLoop = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsageOfDoStatementToCreateInfiniteLoop,
            title: "Avoid usage of do statement to create an infinite loop.",
            messageFormat: "Consider using while statement to create an infinite loop.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AvoidUsageOfForStatementToCreateInfiniteLoop = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsageOfForStatementToCreateInfiniteLoop,
            title: "Avoid usage of for statement to create an infinite loop.",
            messageFormat: "Consider using while statement to create an infinite loop.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AvoidUsageOfWhileStatementToCreateInfiniteLoop = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsageOfWhileStatementToCreateInfiniteLoop,
            title: "Avoid usage of while statement to create an inifinite loop.",
            messageFormat: "Consider using for statement to create an infinite loop.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor RemoveEmptyFinallyClause = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyFinallyClause,
            title: "Remove empty finally clause.",
            messageFormat: "Consider removing empty finally clause.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEmptyArgumentList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyArgumentList,
            title: "Remove empty argument list.",
            messageFormat: "Consider removing empty argument list.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor SimplifyLogicalNotExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyLogicalNotExpression,
            title: "Simplify logical not expression.",
            messageFormat: "Consider simplifying logical not expression.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveUnnecessaryCaseLabel = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveUnnecessaryCaseLabel,
            title: "Remove unnecessary case label.",
            messageFormat: "Consider removing unnecessary case label.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantDefaultSwitchSection = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantDefaultSwitchSection,
            title: "Remove redundant default switch section.",
            messageFormat: "Consider removing redundant default switch section.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBaseConstructorCall = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantBaseConstructorCall,
            title: "Remove redundant base constructor call.",
            messageFormat: "Consider removing redundant base constructor call.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEmptyNamespaceDeclaration = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyNamespaceDeclaration,
            title: "Remove empty namespace declaration.",
            messageFormat: "Consider removing empty namespace declaration.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor ReplaceIfStatementWithReturnStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceIfStatementWithReturnStatement,
            title: "Replace if statement with return statement.",
            messageFormat: "Consider replacing if statement with return statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor ReplaceIfStatementWithReturnStatementFadeOut = ReplaceIfStatementWithReturnStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor RemoveRedundantConstructor = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantConstructor,
            title: "Remove redundant constructor.",
            messageFormat: "Consider removing redundant constructor.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidEmptyCatchClauseThatCatchesSystemException = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidEmptyCatchClauseThatCatchesSystemException,
            title: "Avoid empty catch clause that catches System.Exception.",
            messageFormat: "Avoid empty catch clause that catches System.Exception.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor FormatDeclarationBraces = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatDeclarationBraces,
            title: "Format declaration braces.",
            messageFormat: "Consider formatting declaration braces.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyLinqMethodChain = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyLinqMethodChain,
            title: "Simplify LINQ method chain.",
            messageFormat: "Consider simplifying LINQ method chain.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor ReplaceStringEmptyWithEmptyStringLiteral = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceStringEmptyWithEmptyStringLiteral,
            title: "Replace string.Empty with \"\".",
            messageFormat: "Consider replacing string.Empty with \"\".",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor ThrowingOfNewNotImplementedException = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ThrowingOfNewNotImplementedException,
            title: "Throwing of new NotImplementedException.",
            messageFormat: "Consider implementing the functionality instead of throwing new NotImplementedException.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceAnyMethodWithCountOrLengthProperty = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceAnyMethodWithCountOrLengthProperty,
            title: "Replace 'Any' method with 'Count' or 'Length' property.",
            messageFormat: "Consider replacing 'Any' method with '{0}' property.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SplitVariableDeclaration = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SplitVariableDeclaration,
            title: "Split variable declaration.",
            messageFormat: "Consider splitting variable declaration.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor ReplaceCountMethodWithCountOrLengthProperty = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceCountMethodWithCountOrLengthProperty,
            title: "Replace 'Count' method with 'Count' or 'Length' property.",
            messageFormat: "Consider replacing 'Count' method with '{0}' property.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceCountMethodWithAnyMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceCountMethodWithAnyMethod,
            title: "Replace 'Count' method with 'Any' method.",
            messageFormat: "Consider replacing 'Count' method with 'Any' method.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceConditionalExpressionWithCoalesceExpression = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceConditionalExpressionWithCoalesceExpression,
            title: "Replace conditional expression with coalesce expression.",
            messageFormat: "Consider replacing conditional expression with coalesce expression.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplacePropertyWithAutoImplementedProperty = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplacePropertyWithAutoImplementedProperty,
            title: "Replace property with auto-implemented property.",
            messageFormat: "Consider replacing property with auto-implemented property.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplacePropertyWithAutoImplementedPropertyFadeOut = ReplacePropertyWithAutoImplementedProperty.CreateFadeOut();

        public static readonly DiagnosticDescriptor UseLinefeedAsNewLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseLinefeedAsNewLine,
            title: "Use linefeed as newline.",
            messageFormat: "Consider using linefeed as newline.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseCarriageReturnAndLinefeedAsNewLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseCarriageReturnAndLinefeedAsNewLine,
            title: "Use carriage return + linefeed as newline.",
            messageFormat: "Consider using carriage return + linefeed as newline.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AvoidUsageOfTab = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsageOfTab,
            title: "Avoid usage of tab.",
            messageFormat: "Consider replacing tab with spaces.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UsePostfixUnaryOperatorInsteadOfAssignment = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UsePostfixUnaryOperatorInsteadOfAssignment,
            title: "Use postfix unary operator instead of assignment.",
            messageFormat: "Consider using {0} operator instead of assignment.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor AddConfigureAwait = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.AddConfigureAwait,
             title: "Add 'ConfigureAwait(false)' to awaitable expression.",
             messageFormat: "Consider adding 'ConfigureAwait(false) to awaitable expression.",
             category: DiagnosticCategories.General,
             defaultSeverity: DiagnosticSeverity.Info,
             isEnabledByDefault: true
         );

        public static readonly DiagnosticDescriptor RemoveEmptyRegion = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.RemoveEmptyRegion,
             title: "Remove empty region.",
             messageFormat: "Consider removing empty region.",
             category: DiagnosticCategories.General,
             defaultSeverity: DiagnosticSeverity.Info,
             isEnabledByDefault: true
         );

        public static readonly DiagnosticDescriptor AddEmptyLineAfterLastStatementInDoStatement = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.AddEmptyLineAfterLastStatementInDoStatement,
             title: "Add empty line after last statement in do statement.",
             messageFormat: "Consider adding empty line after last statement in do statement.",
             category: DiagnosticCategories.General,
             defaultSeverity: DiagnosticSeverity.Info,
             isEnabledByDefault: false
         );

        public static readonly DiagnosticDescriptor RemoveFileWithNoCode = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.RemoveFileWithNoCode,
             title: "Remove file with no code.",
             messageFormat: "Consider removing file with no code.",
             category: DiagnosticCategories.General,
             defaultSeverity: DiagnosticSeverity.Info,
             isEnabledByDefault: true
         );

        public static readonly DiagnosticDescriptor DeclareUsingDirectiveOnTopLevel = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.DeclareUsingDirectiveOnTopLevel,
             title: "Declare using directive on top level.",
             messageFormat: "Consider declaring using directive on top level.",
             category: DiagnosticCategories.General,
             defaultSeverity: DiagnosticSeverity.Warning,
             isEnabledByDefault: false
         );
     }
}
