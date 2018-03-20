### 1.8.0 (2018-03-20)

#### Analyzers

##### Changes of "IsEnabledByDefault"

* [RCS1008](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1008.md): disabled by default
* [RCS1009](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1009.md): disabled by default
* [RCS1010](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1010.md): disabled by default
* [RCS1035](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1035.md): disabled by default
* [RCS1040](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1040.md): enabled by default
* [RCS1073](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1073.md): enabled by default

##### Changes of "DefaultSeverity"

* [RCS1017](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1017.md): from Warning to Info
* [RCS1026](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1026.md): from Warning to Info
* [RCS1027](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1027.md): from Warning to Info
* [RCS1028](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1028.md): from Warning to Info
* [RCS1030](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1030.md): from Warning to Info
* [RCS1044](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1044.md): from Info to Warning
* [RCS1045](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1045.md): from Warning to Info
* [RCS1055](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1055.md): from Info to Hidden
* [RCS1056](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1056.md): from Warning to Info
* [RCS1073](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1073.md): from Hidden to Info
* [RCS1076](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1076.md): from Info to Hidden
* [RCS1081](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1081.md): from Warning to Info
* [RCS1086](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1086.md): from Warning to Info
* [RCS1087](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1087.md): from Warning to Info
* [RCS1088](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1088.md): from Warning to Info
* [RCS1094](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1094.md): from Warning to Info
* [RCS1110](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1110.md): from Warning to Info
* [RCS1182](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1182.md): from Info to Hidden

### 1.7.2 (2018-03-06)

#### Analyzers

* Add analyzer [ReplaceInterpolatedStringWithStringConcatenation](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1217.md) (RCS1217).

#### Refactorings

* Add refactoring [ReplaceInterpolatedStringWithStringFormat](http://github.com/JosefPihrt/Roslynator/blob/master/docs/refactorings/RR0201.md) (RR0201).

### 1.7.1 (2018-02-14)

#### Analyzers

* Add analyzer [UnneccesaryUnsafeContext](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1216.md) (RCS1216).
* Improve analyzer ReplaceCommentWithDocumentationComment (RCS1181) - support trailing comment.

### 1.7.0 (2018-02-02)

#### Analyzers

* Rename analyzer AddBraces to AddBracesWhenExpressionSpansOverMultipleLines (RCS1001).
* Rename analyzer AddBracesToIfElse to AddBracesToIfElseWhenExpressionSpansOverMultipleLines (RCS1003).
* Rename analyzer AvoidEmbeddedStatement to AddBraces (RCS1007).
* Rename analyzer AvoidEmbeddedStatementInIfElse to AddBracesToIfElse (RCS1126).

#### Refactorings

* Add refactoring [UncommentMultilineComment](http://github.com/JosefPihrt/Roslynator/blob/master/docs/refactorings/RR0200.md) (RR0200).

### 1.6.30 (2018-01-19)

* Add support for 'private protected' accessibility.

#### Analyzers

* Do not report unused parameter (RCS1163) when parameter name consists of underscore(s).

#### Refactorings

* Add refactoring [InlineProperty](http://github.com/JosefPihrt/Roslynator/blob/master/docs/refactorings/RR0198.md) (RR0198).
* Add refactoring [RemoveEnumMemberValue](http://github.com/JosefPihrt/Roslynator/blob/master/docs/refactorings/RR0199.md) (RR0199).
* Remove, duplicate or comment out local function.
* Change accessibility for selected members.

#### Code Fixes

* Add code fixes for CS0029, CS0133, CS0201, CS0501, CS0527.

### 1.6.20 (2018-01-03)

#### Analyzers

* Add analyzer [AvoidInterpolatedStringWithNoInterpolatedText](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1214.md) (RCS1214).
* Add analyzer [ExpressionIsAlwaysEqualToTrueOrFalse](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1215.md) (RCS1215).

#### Refactorings

* Add refactoring [InitializeFieldFromConstructor](http://github.com/JosefPihrt/Roslynator/blob/master/docs/refactorings/RR0197.md) (RR0197).

#### Code Fixes

* Add code fixes for CS1503, CS1751.

### 1.6.10 (2017-12-21)

#### Analyzers

* Add analyzer [UnusedMemberDeclaration](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1213.md) (RCS1213).
* Improve analyzer [UnusedParameter](http://github.com/JosefPihrt/Roslynator/blob/master/docs/analyzers/RCS1163.md)
  * Report unused parameters of lambda expressions and anonymous methods.

#### Code Fixes

* Add code fixes for CS0030, CS1597.

### 1.6.0 (2017-12-13)

#### Refactorings

* Add refactoring [AddMemberToInterface](http://github.com/JosefPihrt/Roslynator/blob/master/docs/refactorings/RR0195.md) (RR0195).
* Add refactoring [MergeIfWithParentIf](http://github.com/JosefPihrt/Roslynator/blob/master/docs/refactorings/RR0196.md) (RR0196).

#### Code Fixes

Add code fix for CS1031 and CS8112. 

### 1.5.14 (2017-11-29)

#### Refactorings

* Add refactoring ReplaceInterpolatedStringWithConcatenation (RR0193).
* Add refactoring SplitDeclarationAndInitialization (RR0194).

#### Code Fixes

* Add code fixes for CS0246.

### 1.5.13 (2017-11-09)

#### Analyzers

* Add analyzer RemoveRedundantAssignment (RCS1212).

#### Refactorings

* Add refactoring ReplaceCommentWithDocumentationComment (RR0192).

#### Code Fixes

* Add code fixes for CS0216, CS0659, CS0660, CS0661 and CS1526.

### 1.5.12 (2017-10-19)

#### Analyzers

* Add analyzer ReturnTaskInsteadOfNull (RCS1210).
* Add analyzer RemoveUnnecessaryElseClause (RCS1211).
* Remove analyzer SimplifyLambdaExpressionParameterList (RCS1022).

#### Refactorings

* Replace refactoring ChangeMemberTypeAccordingToReturnExpression (RR0019) with code fix.
* Replace refactoring ChangeMemberTypeAccordingToYieldReturnExpression (RR0020) with code fix.
* Replace refactoring AddDefaultValueToReturnStatement (RR0008) with code fix.

#### Code Fixes

  * Add code fix for CS0126, CS0139, CS0713 and CS1750.

### 1.5.10 (2017-10-04)

#### Code Fixes

  * Add code fixes for CS0103, CS0192, CS0403 and CS0541.

### 1.5.0 (2017-09-22)

 * Bug fixes.

### 1.4.58 (2017-09-16)

#### Analyzers

  * Remove analyzer UseCSharp6DictionaryInitializer (RCS1095)

#### Refactorings

##### New Refactorings

  * UseCSharp6DictionaryInitializer (RR0191)

### 1.4.57 (2017-09-06)

#### Refactorings

##### New Refactorings

  * ReplaceIfElseWithIfReturn (RR0190)

#### Code Fixes

  * Add code fix for CS0021.

### 1.4.56 (2017-08-28)

#### Analyzers

##### New Analyzers

  * ReorderTypeParameterConstraints (RCS1209)

### 1.4.55 (2017-08-16)

#### Code Fixes

  * Add code fixes for CS0077, CS0201, CS0472, CS1623.

#### Analyzers

##### New Analyzers

  * ReduceIfNesting (RCS1208)

#### Refactorings

##### New Refactorings

  * ReduceIfNesting (RR0189)

### 1.4.54 (2017-08-08)

#### Code Fixes

  * Improve code fixes for CS0162, CS1061.

#### Analyzers

* Add code fix for analyzer ParameterNameDiffersFromBase (RCS1168)

##### New Analyzers

* UseAttributeUsageAttribute (RCS1203)
* UseEventArgsEmpty (RCS1204)
* ReorderNamedArguments (RCS1205)
* UseConditionalAccessInsteadOfConditionalExpression (RCS1206)
* UseMethodGroupInsteadOfAnonymousFunction (RCS1207)

### 1.4.53 (2017-08-02)

#### Code Fixes

  * New code fixes for CS0139, CS0266, CS0592, CS1689.

#### Analyzers

##### New Analyzers

* SimplifyBooleanExpression (RCS1199)
* CallThenByInsteadOfOrderBy (RCS1200)
* UseMethodChaining (RCS1201)
* UseConditionalAccessToAvoidNullReferenceException (RCS1202)

### 1.4.52 (2017-07-24)

#### Code Fixes

  * New code fixes for CS0115, CS1106, CS1621, CS1988.

### 1.4.51 (2017-07-19)

#### Refactorings

  * MarkContainingClassAsAbstract (RR0073) has been replaced with code fix.

##### New Refactorings

  * FormatWhereConstraint (RR0187)
  * ReplaceForEachWithForAndReverseLoop (RR0188)

#### Code Fixes

##### New Code Fixes

Code fixes has been added for the following compiler diagnostics:

  * NamespaceAlreadyContainsDefinition (CS0101)
  * TypeAlreadyContainsDefinition (CS0102)
  * TypeOfConditionalExpressionCannotBeDetermined (CS0173)
  * OutParameterMustBeAssignedToBeforeControlLeavesCurrentMethod (CS0177)
  * NewConstraintMustBeLastConstraintSpecified (CS0401)
  * DuplicateConstraintForTypeParameter (CS0405)
  * ConstraintClauseHasAlreadyBeenSpecified (CS0409)
  * ClassOrStructConstraintMustComeBeforeAnyOtherConstraints (CS0449)
  * CannotSpecifyBothConstraintClassAndClassOrStructConstraint (CS0450)
  * NewConstraintCannotBeUsedWithStructConstraint (CS0451)
  * TypeParameterHasSameNameAsTypeParameterFromOuterType (CS0693)
  * StaticTypesCannotBeUsedAsTypeArguments (CS0718)
  * PartialMethodCannotHaveAccessModifiersOrVirtualAbstractOverrideNewSealedOrExternModifiers (CS0750)
  * NoDefiningDeclarationFoundForImplementingDeclarationOfPartialMethod (CS0759)
  * PartialMethodsMustHaveVoidReturnType (CS0766)
  * MethodHasParameterModifierThisWhichIsNotOnFirstParameter (CS1100)
  * ExtensionMethodMustBeStatic (CS1105)
  * ElementsDefinedInNamespaceCannotBeExplicitlyDeclaredAsPrivateProtectedOrProtectedInternal (CS1527)
  * AsyncModifierCanOnlyBeUsedInMethodsThatHaveBody (CS1994)

### 1.4.50 (2017-07-04)

* Add code fixes that fix 80+ compiler diagnostics (like 'CS0001')

#### Analyzers

* Following analyzers have been replaced with code fixes:

  * ReplaceReturnStatementWithExpressionStatement (RCS1115)
  * AddBreakStatementToSwitchSection (RCS1116)
  * AddReturnStatementThatReturnsDefaultValue (RCS1117)
  * AddMissingSemicolon (RCS1122)
  * MarkMemberAsStatic (RCS1125)
  * ReplaceReturnWithYieldReturn (RCS1131)
  * AddDocumentationComment (RCS1137)
  * MarkContainingClassAsAbstract (RCS1144)
  * RemoveInapplicableModifier (RCS1147)
  * RemoveUnreachableCode (RCS1148)
  * RemoveImplementationFromAbstractMember (RCS1149)
  * MemberTypeMustMatchOverriddenMemberType (RCS1152)
  * OverridingMemberCannotChangeAccessModifiers (RCS1176)

#### Refactorings

* Following refactorings have been replaced with code fixes:

  * AddBooleanComparison (RR0001)
  * ExtractDeclarationFromUsingStatement (RR0042)
  * MarkMemberAsStatic (RR0072)
  * ReplaceCountWithLengthOrLengthWitCount (RR0122)
  * ReplaceStringLiteralWithCharacterLiteral (RR0146)

##### New Refactorings

  * ChangeAccessibility (RR0186)

### 1.4.13 (2017-06-21)

#### Analyzers

##### New Analyzers

* OptimizeStringBuilderAppendCall (RCS1197)
* AvoidBoxingOfValueType (RCS1198)

### 1.4.12 (2017-06-11)

#### Analyzers

##### New Analyzers

* UseRegularStringLiteralInsteadOfVerbatimStringLiteral (RCS1192)
* OverridingMemberCannotChangeParamsModifier (RCS1193)
* ImplementExceptionConstructors (RCS1194)
* UseExclusiveOrOperator (RCS1195)
* CallExtensionMethodAsInstanceMethod (RCS1196)

#### Refactorings

##### New Refactorings

* UseListInsteadOfYield (RR0183)
* SplitIfStatement (RR0184)
* ReplaceObjectCreationWithDefaultValue (RR0185)

### 1.4.1 (2017-06-05)

#### Analyzers

##### New Analyzers

* DeclareEnumValueAsCombinationOfNames (RCS1191)
* MergeStringExpressions (RCS1190)
* AddOrRemoveRegionName (RCS1189)
* RemoveRedundantAutoPropertyInitialization (RCS1188)
* MarkFieldAsConst (RCS1187)
* UseRegexInstanceInsteadOfStaticMethod (RCS1186)

#### Refactorings

##### New Refactorings

* UseStringBuilderInsteadOfConcatenation (RR0182)
* InlineConstant (RR0181)

### 1.4.0 (2017-05-29)

#### Analyzers

* Delete analyzer MergeLocalDeclarationWithReturnStatement (RCS1054) - Its functionality is incorporated into analyzer InlineLocalVariable (RCS1124).
* Disable analyzer FormatAccessorList (RCS1024) by default.
* Disable analyzer FormatEmptyBlock (RCS1023) by default.
* Modify analyzer RemoveEmptyRegion (RCS1091) - Change default severity from Info to Hidden.
* Modify analyzer CompositeEnumValueContainsUndefinedFlag (RCS1157) - Change default severity from Warning to Info.
* Modify analyzer RemoveRedundantParentheses (RCS1032) - Exclude following syntaxes from analyzer:
  * AssignmentExpression.Right
  * ForEachExpression.Expression
  * EqualsValueClause.Value

#### Refactorings

* Modify refactoring CheckExpressionForNull (RR0024) - Do not add empty line.

### 1.3.11 (2017-05-18)

* A lot of bug fixes and improvements.

### 1.3.10 (2017-04-24)

#### Analyzers

* Improve analyzer RemoveInapplicableModifier (RCS1147) - Analyze local function.
* Improve analyzer SimplifyMethodChain (RCS1077) - Merge combination of Where and Any.
* Improve analyzer StaticMemberInGenericTypeShouldUseTypeParameter (RCS1158) - Member must be public, internal or protected internal.

##### New Analyzers

* CallDebugFailInsteadOfDebugAssert (RCS1178)
* UseReturnInsteadOfAssignment (RCS1179)
* InlineLazyInitialization (RCS1180)
* ReplaceCommentWithDocumentationComment (RCS1181)
* RemoveRedundantBaseInterface (RCS1182)
* FormatInitializerWithSingleExpressionOnSingleLine (RCS1183)
* FormatConditionalExpression (RCS1184)
* AvoidSingleLineBlock (RCS1185)

### 1.3.0 (2017-04-02)

* Add support for configuration file.

#### Analyzers

* Disable UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious (RCS1176) by default.
* Disable UseVarInsteadOfExplicitTypeInForEach (RCS1177) by default.

### 1.2.53 (2017-03-27)

* Filter list of refactorings in options.
* Bug fixes.

#### Analyzers

* Change default severity of AddExceptionToDocumentationComment (RCS1140) from Warning to Hidden.
* Change default severity of EnumMemberShouldDeclareExplicitValue (RCS1161) from Warning to Hidden.

### 1.2.52 (2017-03-22)

#### Analyzers

##### New Analyzers

* UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious (RCS1176)
* UseVarInsteadOfExplicitTypeInForEach (RCS1177)

#### Refactorings

##### New Refactorings

* InlineUsingStatic (RR0180)

### 1.2.51 (2017-03-14)

* Bug fixes.

### 1.2.50 (2017-03-13)

* Improved options page with a list of refactorings.
  * Refactorings are displayed in WPF control instead of property grid.
  * Each refactoring has an identifier 'RR....' to avoid confusion with analyzers.

#### Analyzers

##### New Analyzers

* UnusedThisParameter

#### Refactorings

##### New Refactorings

* ImplementIEquatableOfT
* AddTypeParameter

### 1.2.16 (2017-03-02)

#### Analyzers

##### New Analyzers

* SimplifyLazilyInitializedProperty
* UseIsOperatorInsteadOfAsOperator
* UseCoalesceOperatorInsteadOfIf
* RemoveRedundantAsyncAwait

#### Refactorings

##### New Refactorings

* ReplaceHexadecimalLiteralWithDecimalLiteral
* WrapInElseClause

### 1.2.15 (2017-02-23)

#### Analyzers

##### Changes

* Analyzer RemoveRedundantBraces was deleted.

##### New Analyzers

* MarkFieldAsReadOnly
* UseReadOnlyAutoProperty

### 1.2.14 (2017-02-19)

#### Analyzers

##### New Analyzers

* ParameterNameDiffersFromBase
* OverridingMemberCannotChnageAccessModifiers
* ValueTypeCheckedForNull
* UnconstrainedTypeParameterCheckedForNull
* UnusedTypeParameter
* UnusedParameter

### 1.2.13 (2017-02-11)

#### Analyzers

##### New Analyzers

* SortEnumMembers
* UseStringComparison
* UseStringLengthInsteadOfComparisonWithEmptyString
* CompositeEnumValueContainsUndefinedFlag
* AvoidStaticMembersInGenericTypes
* UseGenericEventHandler
* AbstractTypeShouldNotHavePublicConstructors
* EnumMemberShouldDeclareExplicitValue
* AvoidChainOfAssginments

#### Refactorings

##### New Refactorings

* ReplaceExpressionWithConstantValue

### 1.2.12 (2017-02-02)

#### Analyzers

##### New Analyzers

* SimplifyCoalesceExpression
* MarkContainingClassAsAbstract
* RemoveRedundantAsOperator
* UseConditionalAccess
* RemoveInapplicableModifier
* RemoveUnreachableCode
* RemoveImplementationFromAbstractMember
* CallStringConcatInsteadOfStringJoin
* RemoveRedundantCast
* MemberTypeMustMatchOverriddenMemberType
* AddEmptyLineAfterClosingBrace

#### Refactorings

##### New Refactorings

* SortMemberDeclarations
* ReplaceWhileWithFor
* GenerateEnumValues
* GenerateEnumMember
* GenerateCombinedEnumMember

### 1.2.11 (2017-01-27)

#### Analyzers

##### New Analyzers

* BitwiseOperatorOnEnumWithoutFlagsAttribute
* ReplaceReturnWithYieldReturn
* RemoveRedundantOverridenMember
* RemoveRedundantDisposeOrCloseCall
* RemoveRedundantContinueStatement
* DeclareEnumMemberWithZeroValue
* MergeSwitchSectionsWithEquivalentContent
* AddDocumentationComment
* AddSummaryToDocumentationComment
* AddSummaryElementToDocumentationComment
* AddExceptionToDocumentationComment
* AddParameterToDocumentationComment
* AddTypeParameterToDocumentationComment

### 1.2.10 (2017-01-22)

#### Analyzers

##### New Analyzers

* ReplaceReturnStatementWithExpressionStatement
* AddBreakStatementToSwitchSection
* AddReturnStatementThatReturnsDefaultValue
* MarkLocalVariableAsConst
* CallFindMethodInsteadOfFirstOrDefaultMethod
* UseElementAccessInsteadOfElementAt
* UseElementAccessInsteadOfFirst
* AddMissingSemicolon
* AddParenthesesAccordingToOperatorPrecedence
* InlineLocalVariable
* MarkMemberAsStatic
* AvoidEmbeddedStatementInIfElse
* MergeLocalDeclarationWithInitialization
* UseCoalesceExpression
* RemoveRedundantFieldInitialization

### 1.2.0 (2017-01-18)

* Release of package Roslynator.Analyzers 1.2.0
* Release of package CSharpAnalyzers 1.2.0

### 1.1.95 (2017-01-04)

* Initial release of Roslynator 2017 and Roslynator Refactorings 2017

### 1.1.90 (2016-12-16)

#### Refactorings

##### New Refactorings

* MergeStringExpressions
* ReplaceForWithWhile
* MarkContainingClassAsAbstract
* MakeMemberVirtual

### 1.1.8 (2016-12-07)

#### Refactorings

##### New Refactorings

* ReplaceStatementWithIfStatement
* NegateIsExpression
* ReplaceCastWithAs
* SplitSwitchLabels
* CheckExpressionForNull

### 1.1.7 (2016-11-29)

#### Refactorings

##### New Refactorings

* CallExtensionMethodAsInstanceMethod
* ReplaceMethodGroupWithLambda
* ReplaceIfStatementWithReturnStatement
* IntroduceLocalFromExpressionStatementThatReturnsValue

### 1.1.6 (2016-11-24)

#### Analyzers

##### New Analyzers

* CombineEnumerableWhereMethodChain
* UseStringIsNullOrEmptyMethod
* RemoveRedundantDelegateCreation

#### Refactorings

##### New Refactorings

* AddExceptionToDocumentationComment
* ReplaceNullLiteralExpressionWithDefaultExpression

### 1.1.5 (2016-11-19)

#### Analyzers

##### New Analyzers

* RemoveEmptyDestructor
* RemoveRedundantStringToCharArrayCall
* AddStaticModifierToAllPartialClassDeclarations
* UseCastMethodInsteadOfSelectMethod
* DeclareTypeInsideNamespace
* AddBracesToSwitchSectionWithMultipleStatements

#### Refactorings

##### New Refactorings

* ReplaceEqualsExpressionWithStringIsNullOrEmpty
* ReplaceEqualsExpressionWithStringIsNullOrWhiteSpace

### 1.1.4 (2016-11-15)

#### Analyzers

##### New Analyzers

* FormatDocumentationSummaryOnSingleLine
* FormatDocumentationSummaryOnMultipleLines
* MarkClassAsStatic
* SimplifyIfElseStatement
* SimplifyConditionalExpression
* MergeInterpolationIntoInterpolatedString

#### Refactorings

##### New Refactorings

* MergeInterpolationIntoInterpolatedString

### 1.1.3 (2016-11-12)

#### Analyzers

##### New Analyzers

* RemoveRedundantToStringCall
* AvoidNullLiteralExpressionOnLeftSideOfBinaryExpression
* DefaultLabelShouldBeLastLabelInSwitchSection

#### Refactorings

##### New Refactorings

* IntroduceFieldToLockOn

### 1.1.2 (2016-11-10)

#### Analyzers

##### New Analyzers

* UseCSharp6DictionaryInitializer
* UseBitwiseOperationInsteadOfHasFlagMethod

#### Refactorings

##### New Refactorings

* CopyDocumentationCommentFromBaseMember

### 1.1.1 (2016-11-06)

#### Analyzers

##### New Analyzers

* RemoveFileWithNoCode
* DeclareUsingDirectiveOnTopLevel

#### Refactorings

##### New Refactorings

* RemoveRegion

### 1.1.0 (2016-11-04)

#### Refactorings

##### New Refactorings

* ReplaceAsWithCast
* ReplaceEqualsExpressionWithStringEquals

### 1.0.9 (2016-11-02)

#### Refactorings

##### New Refactorings

* RemoveUsingAliasDirective
* ReplaceInterpolatedStringWithInterpolationExpression

### 1.0.8 (2016-10-31)

#### Analyzers

##### New Analyzers

* AddEmptyLineAfterLastStatementInDoStatement

#### Refactorings

##### New Refactorings

* ReplaceIfElseWithSwitch

### 1.0.7 (2016-10-29)

#### Refactorings

##### New Refactorings

* RemoveAllPreprocessorDirectives
* AddToMethodInvocation

### 1.0.6 (2016-10-26)

#### Analyzers

##### New Analyzers

* RemoveEmptyRegion

#### Refactorings

##### New Refactorings

* GenerateOnEventMethod

### 1.0.5 (2016-10-24)

#### Refactorings

##### Improvements

* InlineMethod - void method with multiple statements can be inlined.
* CheckParameterForNull - refactoring can be applied to multiple parameters at once.
* AddBraces - braces can be added to if statement in last else-if.

##### New Refactorings

* GenerateBaseConstructors

### 1.0.4 (2016-10-20)

#### Refactorings

##### New Refactorings

* PromoteLocalToParameter
* RemoveInterpolation

### 1.0.3 (2016-10-15)

#### Analyzers

##### New Analyzers

* UsePostfixUnaryOperatorInsteadOfAssignment
* AddConfigureAwait

### 1.0.2 (2016-10-12)

#### Analyzers

##### New Analyzers

* UseLinefeedAsNewline
* UseCarriageReturnAndLinefeedAsNewline
* AvoidUsageOfTab

### 1.0.1 (2016-10-08)

#### Refactorings

##### Changes

* ReplaceMethodWithProperty and ReplacePropertyWithMethod refactorings significantly improved.

##### New Refactorings

* ExtractTypeDeclarationToNewFile
* MergeLocalDeclarations

### 1.0.0 (2016-10-03)

* Entire project was renamed to **Roslynator**

* Visual Studio extension **C# Analyzers and Refactorings** was renamed to **Roslynator**
* Visual Studio extension **C# Refactorings** was renamed to **Roslynator Refactorings**

* Some assemblies were renamed. As a result **ruleset** files must be updated in a following way: 
  * replace &lt;Rules AnalyzerId="Pihrtsoft.CodeAnalysis.CSharp" RuleNamespace="Pihrtsoft.CodeAnalysis.CSharp">
  * with &lt;Rules AnalyzerId="Roslynator.CSharp.Analyzers" RuleNamespace="Roslynator.CSharp.Analyzers">

### 0.99.5 (2016-09-12)

#### Analyzers

##### Changes

* "DeclareEachTypeInSeparateFile" has code fix.

##### Bug Fixes

* "ReplacePropertyWithAutoProperty" - property and field must be of equal type.

#### Refactorings

##### Bug Fixes

* "InsertInterpolation" - '{' and '}' are escaped by doubling when creating interpolated string from string literal.

### 0.99.0 (2016-08-28)

#### Analyzers

##### Changes

* "UseExplicitTypeInsteadOfVar" and "UseVarInsteadOfExplicitType" allow 'var' for enum member expression.
* "AddDefaultAccessModifier" works with partial classes.
* "AvoidUsageOfUsingAliasDirective" has code fix.

#### Refactorings

##### New Refactorings

* ReplaceIfElseWithConditionalExpression
* ReplaceConditionalExpressionWithExpression

### 0.98.0 (2016-08-14)

#### Analyzers

##### Changes

* "RemoveRedundantEmptyLine" analyzer - empty line is allowed when it is last line in 'do' statement's body (when 'while' token is on the same line as closing brace)
* "UseExplicitTypeInsteadOfVar" and "UseVarInsteadOfExplicitType" analyzers - 'var' is allowed for 'default(T)' expression

#### Refactorings

##### New Refactorings

* MergeAssignmentExpressionWithReturnStatement
* CollapseToInitializer
* IntroduceAndInitializeField
* IntroduceAndInitializeProperty

### 0.97.0 (2016-08-08)

#### Refactorings

##### New Refactorings

* AddRegion
* AddIfDirective
* RemoveAllStatements
* RemoveAllMembers
* AddUsingDirective

### 0.96.0 (2016-08-05)

#### Refactorings

##### New Refactorings

* MergeIfStatements
* AddDefaultValueToReturnStatement
* InlineMethod

### 0.95.0 (2016-07-30)

#### Refactorings

##### New Refactorings

* AddExpressionFromIfStatement
* RemoveAllSwitchSections
* RemoveStatementsFromSwitchSections
* AddConfigureAwait
* RemovePreprocessorDirectiveAndRelatedDirectives

### 0.94.0 (2016-07-26)

#### Refactorings

##### New Refactorings

* ReplaceReturnStatementWithIfStatement
* WrapStatementsInTryCatch
* WrapStatementsInIfStatement
* RemoveMemberDeclarations

### 0.93.0 (2016-07-21)

#### Refactorings

##### New Refactorings

* AddIdentifierToVariableDeclaration
* RemoveEmptyLines

### 0.92.0 (2016-07-18)

#### Refactorings

##### New Refactorings

* CommentOutMember
* CommentOutStatement
* IntializerLocalWithDefaultValue
* AddDefaultValueToParameter

##### Improvements

* refactoring "ChangeTypeAccordingToExpression" works for field declaration
* refactoring "AddCastExpression" works for case label expression
* refactoring "FormatExpressionChain" does not format namespace
* refactoring "ReplacePropertyWithMethod" works for property with setter
* refactoring "ReverseForLoop" works for reversed for loop

### 0.91.0 (2016-07-11)

#### Refactorings

##### New Refactorings

* RemoveConditionFromLastElseIf
* RemoveAllXmlComments
* RemoveStatement
* DuplicateStatement
* ReplaceAnonymousMethodWithLambdaExpression
* SplitVariableDeclaration
* ReplaceCountWithLengthOrLengthWithCount

##### Changes

* ChangeMethodReturnTypeToVoid
  * refactoring is available only when method body contains at least one statement
  * refactoring is not available for async method that returns Task
* IntroduceUsingStaticDirective
  * refactoring is available only when class name is selected

### 0.9.90 (2016-07-08)

#### Refactorings

##### New Refactorings

* ReplaceDoStatementWithWhileStatement
* ReplaceWhileStatementWithDoStatement
* IntroduceUsingStaticDirective
* ChangeMethodReturnTypeToVoid
* ReplaceEnumHasFlagWithBitwiseOperation

### 0.9.81 (2016-07-06)

#### Refactorings

##### Changes

* refactoring "FormatBinaryExpression" is available for bitwise and/or expressions.
* refactorings for argument and argument list are also available for attribute argument and attribute argument list.

##### Bug Fixes

* refactorings "RemoveComment" and "RemoveAllComments" are available at comment inside trivia.
* refactoring "AddCastExpressionToArgument" handles properly params parameter.
* refactoring "ExpandPropertyAndAddBackingField" handles properly read-only auto-property.

### 0.9.80 (2016-07-05)

#### Analyzers

##### Changes

* many analyzers renamed
* **developmentDependency** element added to CSharpAnalyzers.nuspec

#### Refactorings

##### New Refactorings

* AddInterpolation
* SimplifyLambdaExpression

##### Changes

* refactorings can be enabled/disabled in Visual Studio UI (Tools - Options)
* some refactorings are available only when C# 6.0 is available.
* many refactorings renamed
* refactoring "ChangeMemberTypeAccordingToReturnExpression" improved for async method
* refactoring "AddCastToReturnExpression" improved for async method
* refactoring "CheckParameterForNull" is not available for lambda and anonymous method

##### Bug Fixes

* refactoring "MarkMemberAsStatic" should not be available for a constant.

### 0.9.70 (2016-06-23)

#### Analyzers

##### Changes

* analyzer "MergeIfStatementWithContainedIfStatement" renamed to "MergeIfStatementWithNestedIfStatement"

#### Refactorings

##### New Refactorings

* MarkMemberAsStatic
* MarkAllMembersAsStatic
* FormatAccessorBracesOnSingleLine
* GenerateSwitchSections
* ConvertStringLiteralToCharacterLiteral

##### Changes

* refactoring "ReverseForLoop" is available within 'for' keyword.
* refactoring "SwapExpressionsInBinaryExpression" is available only for logical and/or expression.
* refactoring "AddCastAccordingToParameterType" can offer more than one cast.
* refactorings "SwapParameters" and "SwapArguments" removed (these are covered by "Change signature..." dialog)
* refactorings "RemoveMember" and "DuplicateMember" are available only at opening/closing brace

##### Bug Fixes

* refactoring "RemoveAllRegions" is available inside #endregion directive.
* refactoring "RenameMethodAccordingToTypeName" handles properly async method.

### 0.9.60 (2016-06-14)

#### Analyzers

##### Changes

* UseNameOfOperator analyzer:
  * only quote marks (and at sign) are faded out.
  * analyzer detects property name in property setter.
* SimplifyLambdaExpressionParameterList analyzer - parenthesized lambda with parameter list with a single parameter without type can be simplified to simple lambda

##### Bug Fixes

* UseExpressionBodiedMember analyzer

#### Refactorings

##### New Refactorings

* Duplicate argument
* Add cast to return statement's expression
* Add cast to variable declaration
* Merge string literals
* Merge string literals into multiline string literal
* Convert regular string literal to verbatim string literal
* Convert verbatim string literal to regular string literal
* Convert verbatim string literal to regular string literals
* Use expression-bodied member

##### Changes

* "Extract expression from parentheses" refactoring is available when cursor is on opening/closing parenthesis.

##### Bug Fixes

* "Check parameter for null" refactoring is available for lambda expression and anonymous method.
* "Remove comment" and "Remove all comments" refactorings is available when cursor is inside xml documentation comment.
* "Convert foreach to for" refactoring is available for string expression.

### 0.9.50 (2016-06-02)

#### Analyzers

##### New Analyzers

* SplitDeclarationIntoMultipleDeclarations
* UseCountOrLengthPropertyInsteadOfCountMethod
* UseAnyMethodInsteadOfCountMethod
* UseCoalesceExpressionInsteadOfConditionalExpression
* UseAutoImplementedProperty

##### Changes

* DeclareExplicitType and DeclareImplicitType analyzers - 'var' is allowed for ThisExpression.

##### Bug Fixes

* "RemoveRedundantEmptyLine" analyzer - empty line can be between using directive (or extern alias) inside namespace declaration and first member declaration.

#### Refactorings

##### New Refactorings

* Expand coalesce expression
* Expand event
* Swap members
* Split attributes
* Merge attributes
* Change method/property/indexer type according to yield return statement
* Notify property changed
* Add cast to assignment expression
* Format accessor braces on multiple lines

##### Changes

* "Remove/duplicate member" refactoring: 
  * triggers inside header or on closing brace (if any)
  * is available for method/constructor/property/indexer/operator/event/namespace/class/struct/interface.
* "Add/remove parameter name" refactoring - argument(s) must be selected.
* "Rename variable/method/property/parameter according to type name" refactorings - predefined types are excluded.
* "Convert method to read-only property" refactoring - triggers only inside method header.
* "Convert property to method" refactoring - triggers only inside property header
* "Make method/property/indexer method" refactoring - triggers only inside method/property/indexer header

##### Bug Fixes

* "Convert constant to read-only field" refactoring - static keyword is added if the constant is declared in static class.
* "Convert switch to if-else chain" refactoring - there must be at least one non-default section.
* "Rename parameter according to type name" refactoring - now works for lambda's argument list.
* "Add parentheses" refactoring

### 0.9.40 (2016-05-24)

#### Analyzers

* **NEW** - **"RemoveEmptyFinallyClause"** analyzer and code fix added
* **NEW** - **"RemoveEmptyArgumentList"** analyzer and code fix added
* **NEW** - **"SimplifyLogicalNotExpression"** analyzer and code fix added
* **NEW** - **"RemoveUnnecessaryCaseLabel"** analyzer and code fix added
* **NEW** - **"RemoveRedundantDefaultSwitchSection"** analyzer and code fix added
* **NEW** - **"RemoveRedundantBaseConstructorCall"** analyzer and code fix added
* **NEW** - **"RemoveEmptyNamespaceDeclaration"** analyzer and code fix added
* **NEW** - **"SimplifyIfStatementToReturnStatement"** analyzer and code fix added
* **NEW** - **"RemoveRedundantConstructor"** analyzer and code fix added
* **NEW** - **"AvoidEmptyCatchClauseThatCatchesSystemException"** analyzer and code fix added
* **NEW** - **"FormatDeclarationBraces"** analyzer and code fix added
* **NEW** - **"SimplifyLinqMethodChain"** analyzer and code fix added
* **NEW** - **"AvoidUsageOfStringEmpty"** analyzer and code fix added
* **NEW** - **"ThrowingOfNewNotImplementedException"** analyzer added
* **NEW** - **"UseCountOrLengthPropertyInsteadOfAnyMethod"** analyzer and code fix added

#### Refactorings

* **NEW** - **"Swap arguments"** refactoring added
* **NEW** - **"Swap expressions"** refactoring added
* **NEW** - **"Swap parameters"** refactoring added
* **NEW** - **"Duplicate parameter"** refactoring added
* **NEW** - **"Access element using '[]' instead of 'First/Last/ElementAt' method"** refactoring added
* **NEW** - **"Introduce constructor from selected member(s)"** refactoring added
* **NEW** - **"Change method/property/indexer type according to return statement"** refactoring added

* **"Remove member"** refactoring removes xml comment that belongs to a member
* **"Add boolean comparison"** refactoring works for return statement in method/property/indexer
* **"Convert string literal to interpolated string"** refactoring adds empty interpolation
* Bug fixed in **"Rename field according to property name"** refactoring
* Bug fixed in **"Convert foreach to for"** refactoring

### 0.9.30 (2016-05-16)

#### Analyzers

* **NEW** - **"UseForStatementToCreateInfiniteLoop"** analyzer and code fix added
* **NEW** - **"UseWhileStatementToCreateInfiniteLoop"** analyzer and code fix added
* **NEW** - **"AvoidUsageOfDoStatementToCreateInfiniteLoop"** analyzer and code fix added
* **NEW** - **UseStringLiteralInsteadOfInterpolatedString** analyzer and code fix added

* **"RemoveRedundantEmptyLine"** analyzer enhanced
* **"FormatAccessorList"** analyzer now works for auto-property accessor list
* **"MergeLocalDeclarationWithReturnStatement"** code fix now works when cursor is in return statement
* **"MergeIfStatementWithContainedIfStatement"** code fix improved (unnecessary parentheses are not added) 
* bug fixed in **"SimplifyAssignmentExpression"** analyzer

#### Refactorings

* **"Extract statement(s) from if statement"** refactoring now works for topmost if statement that has else clause
* **"Format binary expression on multiple lines"** refactoring now works for a single binary expression
* **"Negate binary expression"** refactoring now works properly for a chain of logical and/or expressions
* **"Remove parameter name from each argument"** refactoring now works when any argument has parameter name 
* **"Expand property and add backing field"** improved (accessor is on a single line)

### 0.9.20 (2016-05-09)

#### Analyzers

* **NEW** - **MergeIfStatementWithContainedIfStatement** analyzer and code fix added
* **NEW** - **DeclareEachTypeInSeparateFile** analyzer added
* **NEW** - **AvoidLockingOnPubliclyAccessibleInstance** analyzer and code fix added (without batch fixer)
* **NEW** - **SimplifyAssignmentExpression** analyzer and code fix added
* **NEW** - **AddEmptyLinesBetweenDeclarations** analyzer and code fix added
* **NEW** - **AvoidUsingAliasDirective** analyzer added
* **NEW** - **AvoidSemicolonAtEndOfDeclaration** analyzer and code fix added
* 
* **UseLogicalNotOperator** analyzer renamed to **SimplifyBooleanComparison** and improved
* **RemoveRedundantBooleanLiteral** analyzer now works for `&& true` and `|| false`

#### Refactorings

* **NEW** - **"Add boolean comparison"** refactoring added
* **NEW** - **"Convert interpolated string to string literal"** refactoring added
* **NEW** - **"Convert string literal to interpolated string"** refactoring added
* **NEW** - **"Change 'Any/All' to 'All/Any'"** refactoring added
* 
* **"Format all parameters on a single line"** refactoring now works for parameter list with a single parameter
* **"Convert to constant"** refactoring now works only for predefined types (except object)
* **"Remove comment/comments"** refactorings now work for comments that are inside trivia
* **"Make member abstract"** refactoring now work only for non-abstract indexer/method/property that are in abstract class
* **"Add/remove parameter name (to/from each argument)"** refactorings now work when cursor is right behind the parameter
* Bug fixed in **"Uncomment"** refactoring

### 0.9.11 (2016-04-30)
 
* Bug fixes and minor improvements
    
### 0.9.1 (2016-04-27)
    
* Bug fixes
    
### 0.9.0 (2016-04-26)
    
* Initial release
