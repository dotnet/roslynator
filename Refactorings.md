## C# Refactorings

#### [Access element using '[]' instead of 'First/Last/ElementAt' method](#UseElementAccessInsteadOfMethod) 

* **Syntax**: First/Last/ElementAt method invocation
* **Scope**: method name

![Access element using '[]' instead of 'First/Last/ElementAt' method](/images/refactorings/UseElementAccessInsteadOfMethod.png)

#### Add boolean comparison

* **Syntax**: boolean? expression in place where must be boolean expression

![Add boolean comparison](/images/refactorings/AddBooleanComparison.png)

#### Add braces to embedded statement

* **Syntax**: do statement, else clause, fixed statement, for statement, foreach statement, if statement, lock statement, using statement, while statement

![Add braces to embedded statement](/images/refactorings/AddBracesToEmbeddedStatement.png)

#### Add braces to if-else chain

* **Syntax**: if-else chain
* **Scope**: topmost if keyword

![Add braces to if-else chain](/images/refactorings/AddBracesToIfElseChain.png)

#### Add braces to switch section

* **Syntax**: switch section

![Add braces to switch section](/images/refactorings/AddBracesToSwitchSection.png)

#### Add braces to switch sections

* **Syntax**: switch statement
* **Scope**: switch keyword

![Add braces to switch sections](/images/refactorings/AddBracesToSwitchSections.png)

#### Add cast according to parameter type

* **Syntax**: argument

![Add cast according to parameter type](/images/refactorings/AddCastAccordingToParameterType.png)

#### Add cast to assignment expression

* **Syntax**: assignment expression
* **Scope**: right expression

![Add cast to assignment expression](/images/refactorings/AddCastToAssignmentExpression.png)

#### Add cast to return statement's expression

* **Syntax**: return statement
* **Scope**: expression

![Add cast to return statement's expression](/images/refactorings/AddCastToReturnStatement.png)

#### Add cast to variable declaration

* **Syntax**: variable declaration
* **Scope**: initializer value

![Add cast to variable declaration](/images/refactorings/AddCastToVariableDeclaration.png)

#### Add parameter according to its type name

* **Syntax**: parameter
* **Scope**: missing identifier

![Add parameter according to its type name](/images/refactorings/AddParameterNameAccordingToTypeName.png)

#### Add parameter name to argument

* **Syntax**: argument list

![Add parameter name to argument](/images/refactorings/AddParameterNameToArgument.png)

#### Add parentheses

* **Syntax**: expression
* **Scope**: selected expression

![Add parentheses](/images/refactorings/AddParentheses.png)

#### Add using statement

* **Syntax**: local declaration of type that implements IDisposable

![Add using statement](/images/refactorings/AddUsingStatement.png)

#### Change 'Any/All' to 'All/Any'

* **Syntax**: Any(Func<T, bool> or All(Func<T, bool> from System.Linq.Enumerable namespace
* **Scope**: Any/All name

![Change 'Any/All' to 'All/Any'](/images/refactorings/ChangeAnyToAllOrAllToAny.png)

#### Change foreach variable's declared type according to expression

* **Syntax**: foreach statement
* **Scope**: element type

![Change foreach variable's declared type according to expression](/images/refactorings/ChangeForeachTypeAccordingToExpression.png)

#### Change foreach variable's declared type to 'var'

* **Syntax**: foreach statement
* **Scope**: element type

![Change foreach variable's declared type to 'var'](/images/refactorings/ChangeForeachTypeToVar.png)

#### Change method/property/indexer type according to return statement

* **Syntax**: method, property, indexer
* **Scope**: return statement's expression

![Change method/property/indexer type according to return statement](/images/refactorings/ChangeMemberTypeAccordingToReturnStatement.png)

#### Change method/property/indexer type according to yield return statement

* **Syntax**: method, property, indexer
* **Scope**: yield return statement's expression

![Change method/property/indexer type according to yield return statement](/images/refactorings/ChangeMemberTypeAccordingToYieldReturnStatement.png)

#### Change type according to expression

* **Syntax**: variable declaration
* **Scope**: type

![Change type according to expression](/images/refactorings/ChangeTypeAccordingToExpression.png)

#### Change variable declaration type

* **Syntax**: variable declaration
* **Scope**: type

![Change variable declaration type](/images/refactorings/ChangeVariableDeclarationType.png)

#### Check parameter for null

* **Syntax**: parameter
* **Scope**: parameter identifier

![Check parameter for null](/images/refactorings/CheckParameterForNull.png)

#### Convert "" to string.Empty

* **Syntax**: empty string literal

![Convert "" to string.Empty](/images/refactorings/ConvertEmptyStringLiteralToStringEmpty.png)

#### Convert conditional expression to if-else

* **Syntax**: conditional expression

![Convert conditional expression to if-else](/images/refactorings/ConvertConditionalExpressionToIfElse.png)

#### Convert constant to read-only field

* **Syntax**: constant declaration

![Convert constant to read-only field](/images/refactorings/ConvertConstantToReadOnlyField.png)

#### Convert for to foreach

* **Syntax**: for statement

![Convert for to foreach](/images/refactorings/ConvertForToForeach.png)

#### Convert foreach to for

* **Syntax**: foreach statement

![Convert foreach to for](/images/refactorings/ConvertForeachToFor.png)

#### Convert interpolated string to string literal

* **Syntax**: Interpolated string without any expression

![Convert interpolated string to string literal](/images/refactorings/ConvertInterpolatedStringToStringLiteral.png)

#### Convert method to read-only property

* **Syntax**: method
* **Scope**: method header

![Convert method to read-only property](/images/refactorings/ConvertMethodToReadOnlyProperty.png)

#### Convert read-only field to constant

* **Syntax**: read-only field

![Convert read-only field to constant](/images/refactorings/ConvertReadOnlyFieldToConstant.png)

#### Convert read-only property to method

* **Syntax**: read-only property
* **Scope**: property header

![Convert read-only property to method](/images/refactorings/ConvertReadOnlyPropertyToMethod.png)

#### Convert regular string literal to verbatim string literal

* **Syntax**: regular string literal

![Convert regular string literal to verbatim string literal](/images/refactorings/ConvertRegularStringLiteralToVerbatimStringLiteral.png)

#### Convert string literal to interpolated string

* **Syntax**: String literal

![Convert string literal to interpolated string](/images/refactorings/ConvertStringLiteralToInterpolatedString.png)

#### Convert string.Empty to ""

* **Syntax**: string.Empty

![Convert string.Empty to ""](/images/refactorings/ConvertStringEmptyToEmptyStringLiteral.png)

#### Convert switch to if-else chain

* **Syntax**: switch statement
* **Scope**: switch keyword

![Convert switch to if-else chain](/images/refactorings/ConvertSwitchToIfElseChain.png)

#### Convert to increment/decrement operator

* **Syntax**: prefix/postfix unary expression

![Convert to increment/decrement operator](/images/refactorings/ConvertIncrementOperatorToDecrementOperator.png)

#### Convert to interpolated string

* **Syntax**: string.Format method

![Convert to interpolated string](/images/refactorings/ConvertStringFormatToInterpolatedString.png)

#### Convert to prefix/postfix operator

* **Syntax**: prefix/postfix unary expression

![Convert to prefix/postfix operator](/images/refactorings/ConvertPrefixOperatorToPostfixOperator.png)

#### Convert verbatim string literal to regular string literal

* **Syntax**: verbatim string literal

![Convert verbatim string literal to regular string literal](/images/refactorings/ConvertVerbatimStringLiteralToRegularStringLiteral.png)

#### Convert verbatim string literal to regular string literals

* **Syntax**: multiline verbatim string literal

![Convert verbatim string literal to regular string literals](/images/refactorings/ConvertVerbatimStringLiteralToRegularStringLiterals.png)

#### Duplicate argument

* **Syntax**: missing argument

![Duplicate argument](/images/refactorings/DuplicateArgument.png)

#### Duplicate member

* **Syntax**: method, constructor, property, indexer, operator, event, namespace, class, struct, interface
* **Scope**: header or closing brace

![Duplicate member](/images/refactorings/DuplicateMember.png)

#### Duplicate parameter

* **Syntax**: missing parameter

![Duplicate parameter](/images/refactorings/DuplicateParameter.png)

#### Expand assignment expression

* **Syntax**: assignment expression
* **Scope**: operator

![Expand assignment expression](/images/refactorings/ExpandAssignmentExpression.png)

#### Expand coalesce expression

* **Syntax**: coalesce expression
* **Scope**: ?? operator

![Expand coalesce expression](/images/refactorings/ExpandCoalesceExpression.png)

#### Expand event

* **Syntax**: event field declaration

![Expand event](/images/refactorings/ExpandEvent.png)

#### Expand expression-bodied member

* **Syntax**: expression body

![Expand expression-bodied member](/images/refactorings/ExpandExpressionBodiedMember.png)

#### Expand lambda expression's body

* **Syntax**: lambda expression
* **Scope**: body

![Expand lambda expression's body](/images/refactorings/ExpandLambdaExpressionBody.png)

#### Expand object initializer

* **Syntax**: object initializer

![Expand object initializer](/images/refactorings/ExpandObjectInitializer.png)

#### Expand property

* **Syntax**: auto-property

![Expand property](/images/refactorings/ExpandProperty.png)

#### Expand property and add backing field

* **Syntax**: auto-property

![Expand property and add backing field](/images/refactorings/ExpandPropertyAndAddBackingField.png)

#### Extract declaration from using statement

* **Syntax**: using statement
* **Scope**: declaration

![Extract declaration from using statement](/images/refactorings/ExtractDeclarationFromUsingStatement.png)

#### Extract expression from parentheses

* **Syntax**: parenthesized expression
* **Scope**: opening/closing parenthesis

![Extract expression from parentheses](/images/refactorings/ExtractExpressionFromParentheses.png)

#### Extract generic type

* **Syntax**: generic name (with single type argument)
* **Scope**: type argument

![Extract generic type](/images/refactorings/ExtractGenericType.png)

#### Extract statement(s)

* **Syntax**: else clause, fixed statement, for statement, foreach statement, checked statement, if statement, lock statement, try statement, unsafe statement, using statement, while statement

![Extract statement(s)](/images/refactorings/ExtractStatement.png)

#### Format accessor braces on multiple lines

* **Syntax**: get accessor, set accessor, add accessor, remove accessor
* **Scope**: block

![Format accessor braces on multiple lines](/images/refactorings/FormatAccessorBracesOnMultipleLines.png)

#### Format all arguments on a single line

* **Syntax**: argument list

![Format all arguments on a single line](/images/refactorings/FormatAllArgumentsOnSingleLine.png)

#### Format all parameters on a single line

* **Syntax**: parameter list

![Format all parameters on a single line](/images/refactorings/FormatAllParametersOnSingleLine.png)

#### Format binary expressions on multiple lines

* **Syntax**: do statement, if statement, while statement
* **Scope**: condition

![Format binary expressions on multiple lines](/images/refactorings/FormatBinaryExpressionsOnMultipleLines.png)

#### Format conditional expression on multiple lines

* **Syntax**: conditional expression

![Format conditional expression on multiple lines](/images/refactorings/FormatConditionalExpressionOnMultipleLines.png)

#### Format conditional expression to a single line

* **Syntax**: conditional expression

![Format conditional expression to a single line](/images/refactorings/FormatConditionalExpressionToSingleLine.png)

#### Format each argument on separate line

* **Syntax**: argument list

![Format each argument on separate line](/images/refactorings/FormatEachArgumentOnSeparateLine.png)

#### Format each parameter on separate line

* **Syntax**: parameter list

![Format each parameter on separate line](/images/refactorings/FormatEachParameterOnSeparateLine.png)

#### Format expression chain on a single line

* **Syntax**: expression chain

![Format expression chain on a single line](/images/refactorings/FormatExpressionChainOnSingleLine.png)

#### Format expression chain on multiple lines

* **Syntax**: expression chain

![Format expression chain on multiple lines](/images/refactorings/FormatExpressionChainOnMultipleLines.png)

#### Format initializer on a single line

* **Syntax**: initializer

![Format initializer on a single line](/images/refactorings/FormatInitializerOnSingleLine.png)

#### Format initializer on multiple lines

* **Syntax**: initializer

![Format initializer on multiple lines](/images/refactorings/FormatInitializerOnMultipleLines.png)

#### Introduce constructor from selected member(s)

* **Syntax**: selected fields/properties

![Introduce constructor from selected member(s)](/images/refactorings/IntroduceConstructorFromSelectedMembers.png)

#### Make member abstract

* **Syntax**: non-abstract indexer/method/property in abstract class
* **Scope**: indexer/method/property header

![Make member abstract](/images/refactorings/MakeMemberAbstract.png)

#### Merge attributes

* **Syntax**: selected attribute lists

![Merge attributes](/images/refactorings/MergeAttributes.png)

#### Merge string literals

* **Syntax**: concatenated string literals

![Merge string literals](/images/refactorings/MergeStringLiterals.png)

#### Merge string literals into multiline string literal

* **Syntax**: concatenated string literals

![Merge string literals into multiline string literal](/images/refactorings/MergeStringLiteralsIntoMultilineStringLiteral.png)

#### Negate binary expression

* **Syntax**: logical and/or expression

![Negate binary expression](/images/refactorings/NegateBinaryExpression.png)

#### Negate boolean literal

* **Syntax**: boolean literal

![Negate boolean literal](/images/refactorings/NegateBooleanLiteral.png)

#### Negate operator

* **Syntax**: !=, &&, ||, <, <=, ==, >, >=

![Negate operator](/images/refactorings/NegateOperator.png)

#### Notify property changed

* **Syntax**: property in class/struct that implements INotifyPropertyChanged
* **Scope**: setter

![Notify property changed](/images/refactorings/NotifyPropertyChanged.png)

#### Remove all comments

* **Syntax**: singleline/multiline comment, singleline/multiline xml documentation comment

![Remove all comments](/images/refactorings/RemoveAllComments.png)

#### Remove all comments (except xml comments)

* **Syntax**: singleline/multiline comment

![Remove all comments (except xml comments)](/images/refactorings/RemoveAllCommentsExceptXmlComments.png)

#### Remove all regions

* **Syntax**: region directive

![Remove all regions](/images/refactorings/RemoveAllRegions.png)

#### Remove braces from if-else chain

* **Syntax**: if-else chain
* **Scope**: topmost if keyword

![Remove braces from if-else chain](/images/refactorings/RemoveBracesFromIfElseChain.png)

#### Remove braces from switch section

* **Syntax**: switch section

![Remove braces from switch section](/images/refactorings/RemoveBracesFromSwitchSection.png)

#### Remove braces from switch sections

* **Syntax**: switch statement
* **Scope**: switch keyword

![Remove braces from switch sections](/images/refactorings/RemoveBracesFromSwitchSections.png)

#### Remove comment

* **Syntax**: singleline/multiline comment, singleline/multiline xml documentation comment

![Remove comment](/images/refactorings/RemoveComment.png)

#### Remove member

* **Syntax**: method, constructor, property, indexer, operator, event, namespace, class, struct, interface
* **Scope**: header or closing brace

![Remove member](/images/refactorings/RemoveMember.png)

#### Remove parameter name from argument

* **Syntax**: selected argument(s)

![Remove parameter name from argument](/images/refactorings/RemoveParameterNameFromArgument.png)

#### Remove property initializer

* **Syntax**: property initializer

![Remove property initializer](/images/refactorings/RemovePropertyInitializer.png)

#### Rename backing field according to property name

* **Syntax**: backing field
* **Scope**: property declaration

![Rename backing field according to property name](/images/refactorings/RenameBackingFieldAccordingToPropertyName.png)

#### Rename foreach variable according to its type name

* **Syntax**: foreach statement
* **Scope**: foreach statement identifier

![Rename foreach variable according to its type name](/images/refactorings/RenameForeachVariableAccordingToTypeName.png)

#### Rename local/field/const according to type name

* **Syntax**: local/field/constant declaration

![Rename local/field/const according to type name](/images/refactorings/RenameLocalOrFieldOrConstantAccordingToTypeName.png)

#### Rename method according to type name

* **Syntax**: method

![Rename method according to type name](/images/refactorings/RenameMethodAccordingToTypeName.png)

#### Rename parameter according to its type name

* **Syntax**: parameter
* **Scope**: parameter identifier

![Rename parameter according to its type name](/images/refactorings/RenameParameterAccordingToTypeName.png)

#### Rename property according to type name

* **Syntax**: property identifier

![Rename property according to type name](/images/refactorings/RenamePropertyAccordingToTypeName.png)

#### Reverse for loop

* **Syntax**: for statement

![Reverse for loop](/images/refactorings/ReverseForLoop.png)

#### Split attributes

* **Syntax**: selected attribute list

![Split attributes](/images/refactorings/SplitAttributes.png)

#### Swap arguments

* **Syntax**: arguments
* **Scope**: comma between parameters

![Swap arguments](/images/refactorings/SwapArguments.png)

#### Swap expressions in binary expression

* **Syntax**: binary expression
* **Scope**: binary operator

![Swap expressions in binary expression](/images/refactorings/SwapExpressionsInBinaryExpression.png)

#### Swap expressions in conditional expression

* **Syntax**: conditional expression
* **Scope**: condition

![Swap expressions in conditional expression](/images/refactorings/SwapExpressionsInConditionalExpression.png)

#### Swap members

* **Syntax**: empty line between member declarations

![Swap members](/images/refactorings/SwapMembers.png)

#### Swap parameters

* **Syntax**: parameters
* **Scope**: comma between parameters

![Swap parameters](/images/refactorings/SwapParameters.png)

#### Swap statements in if-else

* **Syntax**: if statement
* **Scope**: if keyword

![Swap statements in if-else](/images/refactorings/SwapStatementsInIfElse.png)

#### Uncomment

* **Syntax**: single-line comment(s)

![Uncomment](/images/refactorings/Uncomment.png)

#### Use expression-bodied member

* **Syntax**: method, property, indexer, operator
* **Scope**: body or accessor list

![Use expression-bodied member](/images/refactorings/UseExpressionBodiedMember.png)
