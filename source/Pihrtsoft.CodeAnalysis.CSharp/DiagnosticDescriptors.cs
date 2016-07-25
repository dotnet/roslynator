// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor ReplaceEmbeddedStatementWithBlock = new DiagnosticDescriptor(
             id: DiagnosticIdentifiers.ReplaceEmbeddedStatementWithBlock,
             title: "Replace embedded statement with block.",
             messageFormat: "Consider replacing embedded statement with block.",
             category: DiagnosticCategories.General,
             defaultSeverity: DiagnosticSeverity.Info,
             isEnabledByDefault: true
         );

        public static readonly DiagnosticDescriptor ReplaceBlockWithEmbeddedStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceBlockWithEmbeddedStatement,
            title: "Replace block with embedded statement.",
            messageFormat: "Consider replacing block with embedded statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceBlockWithEmbeddedStatementFadeOut = ReplaceBlockWithEmbeddedStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor ReplaceEmbeddedStatementWithBlockInIfElse = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceEmbeddedStatementWithBlockInIfElse,
            title: "Replace embedded statement with block (in if-else).",
            messageFormat: "Consider replacing embedded statement with block (in if-else).",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceBlockWithEmbeddedStatementInIfElse = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceBlockWithEmbeddedStatementInIfElse,
            title: "Replace block with embedded statement (in if-else).",
            messageFormat: "Consider replacing block with embedded statement (in if-else).",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceBlockWithEmbeddedStatementInIfElseFadeOut = ReplaceBlockWithEmbeddedStatementInIfElse.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyNestedUsingStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyNestedUsingStatement,
            title: "Simplify nested using statement.",
            messageFormat: "Consider simplifying nested using statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyNestedUsingStatementFadeOut = SimplifyNestedUsingStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor SimplifyElseClauseContainingOnlyIfStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.SimplifyElseClauseContainingOnlyIfStatement,
            title: "Simplify else clause containing only if statement.",
            messageFormat: "Consider simplifying else clause containing only if statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor SimplifyElseClauseContainingOnlyIfStatementFadeOut = SimplifyElseClauseContainingOnlyIfStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor AvoidEmbeddedStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AvoidEmbeddedStatement,
            title: "Avoid embedded statement.",
            messageFormat: "Consider replacing embedded statement with block.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor ReplaceVarWithExplicitType = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceVarWithExplicitType,
            title: "Replace 'var' with explicit type (when the type is not obvious).",
            messageFormat: "Consider replacing 'var' with explicit type (when the type is not obvious).",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor ReplaceVarWithExplicitTypeInForEach = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceVarWithExplicitTypeInForEach,
            title: "Replace 'var' with explicit type (in foreach).",
            messageFormat: "Consider replacing 'var' with explicit type (in foreach).",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor ReplaceExplicitTypeWithVar = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceExplicitTypeWithVar,
            title: "Replace explicit type with 'var' (when the type is obvious).",
            messageFormat: "Consider replacing explicit type with 'var' (when the type is obvious).",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
        );

        public static readonly DiagnosticDescriptor ReplaceVarWithExplicitTypeEvenIfObvious = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceVarWithExplicitTypeEvenIfObvious,
            title: "Replace 'var' with explicit type (even if the type is obvious).",
            messageFormat: "Consider replacing 'var' with explicit type (even if the type is obvious).",
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
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor UseNameOfOperator = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.UseNameOfOperator,
            title: "Use nameof operator.",
            messageFormat: "Consider using nameof operator.",
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
            isEnabledByDefault: true
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

        public static readonly DiagnosticDescriptor AddAccessModifier = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.AddAccessModifier,
            title: "Add access modifier.",
            messageFormat: "Consider adding access modifier.",
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

        public static readonly DiagnosticDescriptor FormatEachEnumMemberOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEachEnumMemberOnSeparateLine,
            title: "Format each enum member on separate line.",
            messageFormat: "Consider formatting each enum member on separate line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor FormatEachStatementOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEachStatementOnSeparateLine,
            title: "Format each statement on separate line.",
            messageFormat: "Consider formatting each statement on separate line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatEmbeddedStatementOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatEmbeddedStatementOnSeparateLine,
            title: "Format embedded statement on separate line.",
            messageFormat: "Consider formatting embedded statement on separate line.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor FormatSwitchSectionStatementOnSeparateLine = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.FormatSwitchSectionStatementOnSeparateLine,
            title: "Format switch section statement on separate line.",
            messageFormat: "Consider formatting switch section statement on separate line.",
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
            defaultSeverity: DiagnosticSeverity.Warning,
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
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor NonAsynchronousMethodNameShouldNotEndWithAsync = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync,
            title: "Non-asynchronous method name should not end with 'Async'.",
            messageFormat: "Consider removing suffix 'Async' from non-asynchronous method name.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
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

        public static readonly DiagnosticDescriptor ReplaceForEachWithFor = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceForEachWithFor,
            title: "Replace foreach with for.",
            messageFormat: "Consider replacing foreach with for.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false
        );

        public static readonly DiagnosticDescriptor ReplaceForEachWithForFadeOut = ReplaceForEachWithFor.CreateFadeOut();

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

        public static readonly DiagnosticDescriptor MergeIfStatementWithNestedIfStatement = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.MergeIfStatementWithNestedIfStatement,
            title: "Merge if statement with nested if statement.",
            messageFormat: "Consider merging if statement with nested if statement.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor MergeIfStatementWithNestedIfStatementFadeOut = MergeIfStatementWithNestedIfStatement.CreateFadeOut();

        public static readonly DiagnosticDescriptor ReplaceInterpolatedStringWithStringLiteral = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceInterpolatedStringWithStringLiteral,
            title: "Replace interpolated string with string literal.",
            messageFormat: "Consider replacing interpolated string with string literal.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true
        );

        public static readonly DiagnosticDescriptor ReplaceInterpolatedStringWithStringLiteralFadeOut = ReplaceInterpolatedStringWithStringLiteral.CreateFadeOut();

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
            isEnabledByDefault: true
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
            isEnabledByDefault: true,
            customTags: WellKnownDiagnosticTags.Unnecessary
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

        public static readonly DiagnosticDescriptor ReplaceTabWithSpaces = new DiagnosticDescriptor(
            id: DiagnosticIdentifiers.ReplaceTabWithSpaces,
            title: "Replace tab with spaces.",
            messageFormat: "Consider replacing tab with spaces.",
            category: DiagnosticCategories.General,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: false
        );
#endif
    }
}
