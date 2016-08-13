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
