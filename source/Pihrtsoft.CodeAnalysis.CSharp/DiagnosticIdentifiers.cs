// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class DiagnosticIdentifiers
    {
        public const string Prefix = "RCS";

        public const string AddBracesToStatement = Prefix + "1001";
        public const string RemoveBracesFromStatement = Prefix + "1002";
        public const string AddBracesToIfElseChain = Prefix + "1003";
        public const string RemoveBracesFromIfElseChain = Prefix + "1004";
        public const string SimplifyNestedUsingStatement = Prefix + "1005";
        public const string SimplifyElseClauseContainingIfStatement = Prefix + "1006";
        public const string AvoidEmbeddedStatement = Prefix + "1007";
        public const string DeclareExplicitType = Prefix + "1008";
        public const string DeclareExplicitTypeInForEach = Prefix + "1009";
        public const string DeclareImplicitType = Prefix + "1010";
        public const string DeclareImplicitTypeInForEach = Prefix + "1011";
        public const string DeclareExplicitTypeEvenIfObvious = Prefix + "1012";
        public const string UsePredefinedType = Prefix + "1013";
        public const string AvoidImplicitArrayCreation = Prefix + "1014";
        public const string UseNameOfOperator = Prefix + "1015";
        public const string UseExpressionBodiedMember = Prefix + "1016";
        public const string AvoidMultilineExpressionBody = Prefix + "1017";
        public const string AddAccessModifier = Prefix + "1018";
        public const string ReorderModifiers = Prefix + "1019";
        public const string SimplifyNullableOfT = Prefix + "1020";
        public const string SimplifyLambdaExpression = Prefix + "1021";
        public const string SimplifyLambdaExpressionParameterList = Prefix + "1022";
        public const string FormatBlock = Prefix + "1023";
        public const string FormatAccessorList = Prefix + "1024";
        public const string FormatEachEnumMemberOnSeparateLine = Prefix + "1025";
        public const string FormatEachStatementOnSeparateLine = Prefix + "1026";
        public const string FormatEmbeddedStatementOnSeparateLine = Prefix + "1027";
        public const string FormatCaseLabelStatementOnSeparateLine = Prefix + "1028";
        public const string FormatBinaryOperatorOnNextLine = Prefix + "1029";
        public const string AddEmptyLineAfterEmbeddedStatement = Prefix + "1030";
        public const string RemoveRedundantBraces = Prefix + "1031";
        public const string RemoveRedundantParentheses = Prefix + "1032";
        public const string RemoveRedundantBooleanLiteral = Prefix + "1033";
        public const string RemoveRedundantSealedModifier = Prefix + "1034";
        public const string RemoveRedundantCommaInInitializer = Prefix + "1035";
        public const string RemoveRedundantEmptyLine = Prefix + "1036";
        public const string RemoveTrailingWhitespace = Prefix + "1037";
        public const string RemoveEmptyStatement = Prefix + "1038";
        public const string RemoveEmptyAttributeArgumentList = Prefix + "1039";
        public const string RemoveEmptyElseClause = Prefix + "1040";
        public const string RemoveEmptyObjectInitializer = Prefix + "1041";
        public const string RemoveEnumDefaultUnderlyingType = Prefix + "1042";
        public const string RemovePartialModifierFromTypeWithSinglePart = Prefix + "1043";
        public const string RemoveOriginalExceptionFromThrowStatement = Prefix + "1044";
        public const string RenamePrivateFieldAccordingToCamelCaseWithUnderscore = Prefix + "1045";
        public const string AsyncMethodShouldHaveAsyncSuffix = Prefix + "1046";
        public const string NonAsyncMethodShouldNotHaveAsyncSuffix = Prefix + "1047";
        public const string UseLambdaExpressionInsteadOfAnonymousMethod = Prefix + "1048";
        public const string SimplifyBooleanComparison = Prefix + "1049";
        public const string AddConstructorArgumentList = Prefix + "1050";
        public const string AddParenthesesToConditionalExpressionCondition = Prefix + "1051";
        public const string DeclareEachAttributeSeparately = Prefix + "1052";
        public const string ConvertForEachToFor = Prefix + "1053";
        public const string MergeLocalDeclarationWithReturnStatement = Prefix + "1054";
        public const string AvoidSemicolonAtEndOfDeclaration = Prefix + "1055";
        public const string AvoidUsingAliasDirective = Prefix + "1056";
        public const string AddEmptyLineBetweenDeclarations = Prefix + "1057";
        public const string SimplifyAssignmentExpression = Prefix + "1058";
        public const string AvoidLockingOnPubliclyAccessibleInstance = Prefix + "1059";
        public const string DeclareEachTypeInSeparateFile = Prefix + "1060";
        public const string MergeIfStatementWithContainedIfStatement = Prefix + "1061";
        public const string UseStringLiteralInsteadOfInterpolatedString = Prefix + "1062";
        public const string AvoidUsageOfDoStatementToCreateInfiniteLoop = Prefix + "1063";
        public const string UseWhileStatementToCreateInfiniteLoop = Prefix + "1064";
        public const string UseForStatementToCreateInfiniteLoop = Prefix + "1065";
        public const string RemoveEmptyFinallyClause = Prefix + "1066";
        public const string RemoveEmptyArgumentList = Prefix + "1067";
        public const string SimplifyLogicalNotExpression = Prefix + "1068";
        public const string RemoveUnnecessaryCaseLabel = Prefix + "1069";
        public const string RemoveRedundantDefaultSwitchSection = Prefix + "1070";
        public const string RemoveRedundantBaseConstructorCall = Prefix + "1071";
        public const string RemoveEmptyNamespaceDeclaration = Prefix + "1072";
        public const string SimplifyIfStatementToReturnStatement = Prefix + "1073";
        public const string RemoveRedundantConstructor = Prefix + "1074";
        public const string AvoidEmptyCatchClauseThatCatchesSystemException = Prefix + "1075";
        public const string FormatDeclarationBraces = Prefix + "1076";
        public const string SimplifyLinqMethodChain = Prefix + "1077";
        public const string AvoidUsageOfStringEmpty = Prefix + "1078";
        public const string ThrowingOfNewNotImplementedException = Prefix + "1079";
        public const string UseCountOrLengthPropertyInsteadOfAnyMethod = Prefix + "1080";
        public const string SplitDeclarationIntoMultipleDeclarations = Prefix + "1081";
        public const string UseCountOrLengthPropertyInsteadOfCountMethod = Prefix + "1082";

#if DEBUG
        public const string UseLinefeedAsNewLine = Prefix + "X001";
        public const string UseCarriageReturnAndLinefeedAsNewLine = Prefix + "X002";
        public const string UseSpacesInsteadOfTab = Prefix + "X003";
#endif
    }
}
