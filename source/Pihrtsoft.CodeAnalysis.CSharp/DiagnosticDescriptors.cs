// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor DeclareExplicitType = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DeclareExplicitType,
            title: "Declare explicit type (when the type is not obvious).",
            messageFormat: "Consider declaring explicit type (when the type is not obvious).",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor DeclareExplicitTypeEvenIfObvious = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DeclareExplicitTypeEvenIfObvious,
            title: "Declare explicit type (even if the type is obvious).",
            messageFormat: "Consider declaring explicit type (even if the type is obvious).",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor DeclareExplicitTypeInForEach = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DeclareExplicitTypeInForEach,
            title: "Declare explicit type in foreach (when the type is not obvious).",
            messageFormat: "Consider declaring explicit type in foreach (when the type is not obvious).",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor DeclareImplicitType = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DeclareImplicitType,
            title: "Declare implicit type (when the type is obvious).",
            messageFormat: "Consider declaring implicit type (when the type is obvious).",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AddBracesToStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddBracesToStatement,
            title: "Add braces to a statement.",
            messageFormat: "Consider adding braces to a statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveBracesFromStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveBracesFromStatement,
            title: "Remove braces from a statement.",
            messageFormat: "Consider removing braces from a statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveBracesFromStatementFadeOut = RemoveBracesFromStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor AddBracesToIfElseChain = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddBracesToIfElseChain,
            title: "Add braces to if-else chain.",
            messageFormat: "Consider adding braces to if-else chain.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveBracesFromIfElseChain = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveBracesFromIfElseChain,
            title: "Remove braces from if-else chain.",
            messageFormat: "Consider removing braces from if-else chain.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RenamePrivateFieldAccordingToCamelCaseWithUnderscore = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RenamePrivateFieldAccordingToCamelCaseWithUnderscore,
            title: "Rename private field according to camel case with underscore.",
            messageFormat: "Consider renaming private field to camel case with underscore.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
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

        public static readonly DiagnosticDescriptor RemoveEmptyAttributeArgumentList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyAttributeArgumentList,
            title: "Remove empty attribute's argument list.",
            messageFormat: "Consider removing empty attribute's argument list.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBooleanLiteral = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantBooleanLiteral,
            title: "Remove redundant boolean literal.",
            messageFormat: "Consider removing redundant boolean literal.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantBooleanLiteralFadeOut = RemoveRedundantBooleanLiteral.CreateFadeOut();

        public static readonly DiagnosticDescriptor ReorderModifiers = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReorderModifiers,
            title: "Reorder modifiers.",
            messageFormat: "Consider reordering modifiers.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AddAccessModifier = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddAccessModifier,
            title: "Add access modifier.",
            messageFormat: "Consider adding access modifier.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
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

        public static readonly DiagnosticDescriptor AddConstructorArgumentList = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddConstructorArgumentList,
            title: "Add constructor's argument list.",
            messageFormat: "Consider adding constructor's argument list.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AvoidEmbeddedStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidEmbeddedStatement,
            title: "Avoid embedded statement.",
            messageFormat: "Consider adding braces to a statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatBlock = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatBlock,
            title: "Format block.",
            messageFormat: "Consider formatting block.",
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
            title: "Simplify lambda expression's parameter list.",
            messageFormat: "Consider simplifying lambda expression's parameter list.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor SimplifyLambdaExpressionParameterListFadeOut = SimplifyLambdaExpressionParameterList.CreateFadeOut();

        public static readonly DiagnosticDescriptor UseLambdaExpressionInsteadOfAnonymousMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseLambdaExpressionInsteadOfAnonymousMethod,
            title: "Use lambda expression instead of anonymous method.",
            messageFormat: "Consider using lambda expression instead of anonymous method.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseLambdaExpressionInsteadOfAnonymousMethodFadeOut = UseLambdaExpressionInsteadOfAnonymousMethod.CreateFadeOut();

        public static readonly DiagnosticDescriptor UsePredefinedType = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UsePredefinedType,
            title: "Use predefined type.",
            messageFormat: "Consider using predefined type.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidImplicitArrayCreation = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidImplicitArrayCreation,
            title: "Avoid implicit array creation.",
            messageFormat: "Consider declaring explicit type when creating an array.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseNameOfOperator = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseNameOfOperator,
            title: "Use nameof operator.",
            messageFormat: "Consider using nameof operator.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveEmptyObjectInitializer = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyObjectInitializer,
            title: "Remove empty object initializer.",
            messageFormat: "Consider removing empty object initializer.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor RemoveRedundantParentheses = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveRedundantParentheses,
            title: "Remove redundant parentheses.",
            messageFormat: "Consider removing redundant parentheses.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveRedundantParenthesesFadeOut = RemoveRedundantParentheses.CreateFadeOut();

        public static readonly DiagnosticDescriptor FormatEachStatementOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEachStatementOnSeparateLine,
            title: "Format each statement on separate line.",
            messageFormat: "Consider formatting each statement on separate line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatCaseLabelStatementOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatCaseLabelStatementOnSeparateLine,
            title: "Format switch section's statement on separate line.",
            messageFormat: "Consider formatting switch section's statement on separate line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatEachEnumMemberOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEachEnumMemberOnSeparateLine,
            title: "Format each enum member on separate line.",
            messageFormat: "Consider formatting each enum member on separate line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
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

        public static readonly DiagnosticDescriptor RemoveTrailingWhitespace = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveTrailingWhitespace,
            title: "Remove trailing white-space.",
            messageFormat: "Consider removing trailing white-space.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
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

        public static readonly DiagnosticDescriptor RemoveEmptyElseClause = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEmptyElseClause,
            title: "Remove empty else clause.",
            messageFormat: "Consider removing empty else clause.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor SimplifyBooleanComparison = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyBooleanComparison,
            title: "Simplify boolean comparison.",
            messageFormat: "Consider simplifying boolean comparison.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyBooleanComparisonFadeOut = SimplifyBooleanComparison.CreateFadeOut();

        public static readonly DiagnosticDescriptor FormatEmbeddedStatementOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEmbeddedStatementOnSeparateLine,
            title: "Format embedded statement on separate line.",
            messageFormat: "Consider formatting embedded statement on separate line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor AddEmptyLineAfterEmbeddedStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddEmptyLineAfterEmbeddedStatement,
            title: "Add empty line after embedded statement.",
            messageFormat: "Consider adding empty line after embedded statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
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

        public static readonly DiagnosticDescriptor AddParenthesesToConditionalExpressionCondition = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddParenthesesToConditionalExpressionCondition,
            title: "Add parentheses to conditional expression's condition.",
            messageFormat: "Consider adding parentheses to conditional expression's condition.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor ConvertForEachToFor = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ConvertForEachToFor,
            title: "Convert foreach to for.",
            messageFormat: "Consider converting foreach to for.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor ConvertForEachToForFadeOut = ConvertForEachToFor.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidMultilineExpressionBody = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidMultilineExpressionBody,
            title: "Avoid multiline expression body.",
            messageFormat: "Consider expanding expression-bodied member.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

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
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor RemoveEnumDefaultUnderlyingType = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.RemoveEnumDefaultUnderlyingType,
            title: "Remove enum's default underlying type.",
            messageFormat: "Consider removing enum's default underlying type.",
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

        public static readonly DiagnosticDescriptor UseExpressionBodiedMember = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseExpressionBodiedMember,
            title: "Use expression-bodied member.",
            messageFormat: "Consider using expression bodied member.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseExpressionBodiedMemberFadeOut = UseExpressionBodiedMember.CreateFadeOut();

        public static readonly DiagnosticDescriptor MergeLocalDeclarationWithReturnStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeLocalDeclarationWithReturnStatement,
            title: "Merge local declaration with return statement.",
            messageFormat: "Consider merging local declaration with return statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeLocalDeclarationWithReturnStatementFadeOut = MergeLocalDeclarationWithReturnStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyNestedUsingStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyNestedUsingStatement,
            title: "Simplify nested using statement.",
            messageFormat: "Consider simplifying nested using statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyNestedUsingStatementFadeOut = SimplifyNestedUsingStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor AsyncMethodShouldHaveAsyncSuffix = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AsyncMethodShouldHaveAsyncSuffix,
            title: "Add 'Async' suffix to asynchronous method name.",
            messageFormat: "Consider adding 'Async' suffix to asynchronous method name.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor NonAsyncMethodShouldNotHaveAsyncSuffix = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.NonAsyncMethodShouldNotHaveAsyncSuffix,
            title: "Remove 'Async' suffix from non-asynchronous method name.",
            messageFormat: "Consider removing 'Async' suffix from non-asynchronous method name.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor NonAsyncMethodShouldNotHaveAsyncSuffixFadeOut = NonAsyncMethodShouldNotHaveAsyncSuffix.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyElseClauseContainingIfStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyElseClauseContainingIfStatement,
            title: "Simplify else clause containing only if statement.",
            messageFormat: "Consider simplifying else clause containing only if statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyElseClauseContainingIfStatementFadeOut = SimplifyElseClauseContainingIfStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidSemicolonAtEndOfDeclaration = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidSemicolonAtEndOfDeclaration,
            title: "Avoid semicolon at the end of declaration.",
            messageFormat: "Consider removing semicolon from the end of declaration.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor AvoidUsingAliasDirective = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsingAliasDirective,
            title: "Avoid using alias directive.",
            messageFormat: "Avoid using alias directive.", //TODO: improve messageFormat for AvoidUsingAliasDirective
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
            messageFormat: "Avoid locking on publicly accessible instance.", //TODO: Improve messageFormat for AvoidLockingOnPubliclyAccessibleInstance
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor DeclareEachTypeInSeparateFile = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.DeclareEachTypeInSeparateFile,
            title: "Declare each type in separate file.",
            messageFormat: "Consider declaring each type in separate file.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeIfStatementWithContainedIfStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeIfStatementWithContainedIfStatement,
            title: "Merge if statement with contained if statement.",
            messageFormat: "Consider merging if statement with contained if statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeIfStatementWithContainedIfStatementFadeOut = MergeIfStatementWithContainedIfStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor UseStringLiteralInsteadOfInterpolatedString = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseStringLiteralInsteadOfInterpolatedString,
            title: "Use string literal instead of interpolated string.",
            messageFormat: "Consider using string literal instead of interpolated string.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceInterpolatedStringWithStringLiteralFadeOut = UseStringLiteralInsteadOfInterpolatedString.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidUsageOfDoStatementToCreateInfiniteLoop = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsageOfDoStatementToCreateInfiniteLoop,
            title: "Avoid usage of do statement to create an infinite loop.",
            messageFormat: "Consider using while statement to create an infinite loop.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseWhileStatementToCreateInfiniteLoop = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseWhileStatementToCreateInfiniteLoop,
            title: "Use while statement to create an infinite loop.",
            messageFormat: "Consider using while statement to create an infinite loop.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor UseForStatementToCreateInfiniteLoop = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseForStatementToCreateInfiniteLoop,
            title: "Use for statement to create an infinite loop.",
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

        public static readonly DiagnosticDescriptor SimplifyIfStatementToReturnStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyIfStatementToReturnStatement,
            title: "Simplify if statement to return statement.",
            messageFormat: "Consider simplifying if statement to return statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyIfStatementToReturnStatementFadeOut = SimplifyIfStatementToReturnStatement.CreateFadeOut();

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

        public static readonly DiagnosticDescriptor AvoidUsageOfStringEmpty = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidUsageOfStringEmpty,
            title: "Avoid usage of string.Empty.",
            messageFormat: "Consider replacing string.Empty with empty string literal.",
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

        public static readonly DiagnosticDescriptor UseCountOrLengthPropertyInsteadOfAnyMethod = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfAnyMethod,
            title: "Use 'Count' or 'Length' property instead of 'Any' method.",
            messageFormat: "Consider using '{0}' instead of 'Any' method.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

#if DEBUG
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

        public static readonly DiagnosticDescriptor UseSpacesInsteadOfTab = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseSpacesInsteadOfTab,
            title: "Use spaces instead of tab.",
            messageFormat: "Consider using spaces instead of tab.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );
#endif
    }
}
