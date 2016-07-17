## C# Refactorings

#### Add boolean comparison

* **Syntax**: boolean? expression in place where must be boolean expression

![Add boolean comparison](/images/refactorings/AddBooleanComparison.png)

#### Add cast expression

* **Syntax**: argument, assignment expression, return statement, variable declaration

![Add cast expression](/images/refactorings/AddCastExpressionToArgument.png)
![Add cast expression](/images/refactorings/AddCastExpressionToAssignmentExpression.png)
![Add cast expression](/images/refactorings/AddCastExpressionToReturnStatement.png)
![Add cast expression](/images/refactorings/AddCastExpressionToVariableDeclaration.png)

#### Add default value to parameter

* **Syntax**: parameter without default value
* **Scope**: identifier

![Add default value to parameter](/images/refactorings/AddDefaultValueToParameter.png)

#### Add interpolation

* **Syntax**: selected text inside interpolated string text

![Add interpolation](/images/refactorings/AddInterpolation.png)

#### Add parameter name to argument

* **Syntax**: argument list

![Add parameter name to argument](/images/refactorings/AddParameterNameToArgument.png)

#### Add parameter name to parameter

* **Syntax**: parameter
* **Scope**: missing identifier

![Add parameter name to parameter](/images/refactorings/AddParameterNameToParameter.png)

#### Change method return type to void

* **Syntax**: method

![Change method return type to void](/images/refactorings/ChangeMethodReturnTypeToVoid.png)

#### Change method/property/indexer type according to return expression

* **Syntax**: return statement in method/property/indexer

![Change method/property/indexer type according to return expression](/images/refactorings/ChangeMemberTypeAccordingToReturnExpression.png)

#### Change method/property/indexer type according to yield return expression

* **Syntax**: yield return statement in method/property/indexer

![Change method/property/indexer type according to yield return expression](/images/refactorings/ChangeMemberTypeAccordingToYieldReturnExpression.png)

#### Change type according to expression

* **Syntax**: variable declaration, foreach statement
* **Scope**: type

![Change type according to expression](/images/refactorings/ChangeTypeAccordingToExpression.png)
![Change type according to expression](/images/refactorings/ChangeForEachTypeAccordingToExpression.png)

#### Check parameter for null

* **Syntax**: parameter
* **Scope**: parameter identifier

![Check parameter for null](/images/refactorings/CheckParameterForNull.png)

#### Comment out member

* **Syntax**: method, constructor, property, indexer, operator, event, namespace, class, struct, interface
* **Scope**: opening or closing brace

![Comment out member](/images/refactorings/CommentOutMember.png)

#### Comment out statement

* **Syntax**: do statement, fixed statement, for statement, foreach statement, checked statement, if statement, lock statement, switch statement, try statement, unchecked statement, unsafe statement, using statement, while statement
* **Scope**: opening or closing brace

![Comment out statement](/images/refactorings/CommentOutStatement.png)

#### Duplicate argument

* **Syntax**: missing argument

![Duplicate argument](/images/refactorings/DuplicateArgument.png)

#### Duplicate member

* **Syntax**: method, constructor, property, indexer, operator, event, namespace, class, struct, interface
* **Scope**: opening or closing brace

![Duplicate member](/images/refactorings/DuplicateMember.png)

#### Duplicate parameter

* **Syntax**: missing parameter

![Duplicate parameter](/images/refactorings/DuplicateParameter.png)

#### Duplicate statement

* **Syntax**: do statement, fixed statement, for statement, foreach statement, checked statement, if statement, lock statement, switch statement, try statement, unchecked statement, unsafe statement, using statement, while statement
* **Scope**: opening or closing brace

![Duplicate statement](/images/refactorings/DuplicateStatement.png)

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

#### Expand initializer

* **Syntax**: initializer

![Expand initializer](/images/refactorings/ExpandInitializer.png)

#### Expand lambda expression body

* **Syntax**: lambda expression
* **Scope**: body

![Expand lambda expression body](/images/refactorings/ExpandLambdaExpressionBody.png)

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
* **Scope**: opening or closing parenthesis

![Extract expression from parentheses](/images/refactorings/ExtractExpressionFromParentheses.png)

#### Extract generic type

* **Syntax**: generic name (with single type argument)
* **Scope**: type argument

![Extract generic type](/images/refactorings/ExtractGenericType.png)

#### Extract statement(s)

* **Syntax**: else clause, fixed statement, for statement, foreach statement, checked statement, if statement, lock statement, try statement, unsafe statement, using statement, while statement

![Extract statement(s)](/images/refactorings/ExtractStatement.png)

#### Format accessor braces

* **Syntax**: get accessor, set accessor, add accessor, remove accessor
* **Scope**: block

![Format accessor braces](/images/refactorings/FormatAccessorBracesOnMultipleLines.png)
![Format accessor braces](/images/refactorings/FormatAccessorBracesOnSingleLine.png)

#### Format argument list

* **Syntax**: argument list

![Format argument list](/images/refactorings/FormatEachArgumentOnSeparateLine.png)
![Format argument list](/images/refactorings/FormatAllArgumentsOnSingleLine.png)

#### Format binary expression

* **Syntax**: logical and/or expression, bitwise and/or expression

![Format binary expression](/images/refactorings/FormatBinaryExpressionOnMultipleLines.png)

#### Format conditional expression

* **Syntax**: conditional expression

![Format conditional expression](/images/refactorings/FormatConditionalExpressionOnMultipleLines.png)
![Format conditional expression](/images/refactorings/FormatConditionalExpressionOnSingleLine.png)

#### Format expression chain

* **Syntax**: expression chain

![Format expression chain](/images/refactorings/FormatExpressionChainOnMultipleLines.png)
![Format expression chain](/images/refactorings/FormatExpressionChainOnSingleLine.png)

#### Format initializer

* **Syntax**: initializer

![Format initializer](/images/refactorings/FormatInitializerOnMultipleLines.png)
![Format initializer](/images/refactorings/FormatInitializerOnSingleLine.png)

#### Format parameter list

* **Syntax**: parameter list

![Format parameter list](/images/refactorings/FormatEachParameterOnSeparateLine.png)
![Format parameter list](/images/refactorings/FormatAllParametersOnSingleLine.png)

#### Generate switch sections

* **Syntax**: switch statement (that is empty or contains only default section)

![Generate switch sections](/images/refactorings/GenerateSwitchSections.png)

#### Initialize local with default value

* **Syntax**: local declaration without initializer
* **Scope**: identifier

![Initialize local with default value](/images/refactorings/InitializeLocalWithDefaultValue.png)

#### Introduce constructor from selected member(s)

* **Syntax**: field, property

![Introduce constructor from selected member(s)](/images/refactorings/IntroduceConstructor.png)

#### Introduce using static directive

* **Syntax**: member access expression (public or internal static class)
* **Scope**: selected class name

![Introduce using static directive](/images/refactorings/IntroduceUsingStaticDirective.png)

#### Make member abstract

* **Syntax**: non-abstract indexer/method/property in abstract class
* **Scope**: indexer/method/property header

![Make member abstract](/images/refactorings/MakeMemberAbstract.png)

#### Mark all members as static

* **Syntax**: non-static field/method/property/event in static class

![Mark all members as static](/images/refactorings/MarkAllMembersAsStatic.png)

#### Mark member as static

* **Syntax**: non-static field/method/property/event in static class

![Mark member as static](/images/refactorings/MarkMemberAsStatic.png)

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

#### Remove all xml comments

* **Syntax**: singleline/multiline xml documentation comment

![Remove all xml comments](/images/refactorings/RemoveAllXmlComments.png)

#### Remove comment

* **Syntax**: singleline/multiline comment, singleline/multiline xml documentation comment

![Remove comment](/images/refactorings/RemoveComment.png)

#### Remove condition from last else-if

* **Syntax**: else clause
* **Scope**: else keyword

![Remove condition from last else-if](/images/refactorings/RemoveConditionFromLastElseIf.png)

#### Remove member

* **Syntax**: method, constructor, property, indexer, operator, event, namespace, class, struct, interface
* **Scope**: opening or closing brace

![Remove member](/images/refactorings/RemoveMember.png)

#### Remove parameter name from argument

* **Syntax**: selected argument(s)

![Remove parameter name from argument](/images/refactorings/RemoveParameterNameFromArgument.png)

#### Remove property initializer

* **Syntax**: property initializer

![Remove property initializer](/images/refactorings/RemovePropertyInitializer.png)

#### Remove statement

* **Syntax**: do statement, fixed statement, for statement, foreach statement, checked statement, if statement, lock statement, switch statement, try statement, unchecked statement, unsafe statement, using statement, while statement
* **Scope**: open/close brace

![Remove statement](/images/refactorings/RemoveStatement.png)

#### Rename backing field according to property name

* **Syntax**: backing field
* **Scope**: property declaration

![Rename backing field according to property name](/images/refactorings/RenameBackingFieldAccordingToPropertyName.png)

#### Rename identifier according to type name

* **Syntax**: foreach statement, local/field/constant declaration
* **Scope**: identifier

![Rename identifier according to type name](/images/refactorings/RenameForEachIdentifierAccordingToTypeName.png)
![Rename identifier according to type name](/images/refactorings/RenameFieldIdentifierAccordingToTypeName.png)

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

#### Replace "" with string.Empty

* **Syntax**: empty string literal

![Replace "" with string.Empty](/images/refactorings/ReplaceEmptyStringLiteralWithStringEmpty.png)

#### Replace anonymous method with lambda expression

* **Syntax**: anonymous method
* **Scope**: delegate keyword

![Replace anonymous method with lambda expression](/images/refactorings/ReplaceAnonymousMethodWithLambdaExpression.png)

#### Replace 'Any/All' with 'All/Any'

* **Syntax**: Any(Func<T, bool> or All(Func<T, bool> from System.Linq.Enumerable namespace
* **Scope**: method name

![Replace 'Any/All' with 'All/Any'](/images/refactorings/ReplaceAnyWithAllOrAllWithAny.png)

#### Replace block with embedded statement

* **Syntax**: do statement, else clause, fixed statement, for statement, foreach statement, if statement, lock statement, using statement, while statement
* **Scope**: block with a single statement

![Replace block with embedded statement](/images/refactorings/ReplaceBlockWithEmbeddedStatement.png)

#### Replace block with embedded statement (in if-else)

* **Syntax**: if-else chain
* **Scope**: topmost if keyword

![Replace block with embedded statement (in if-else)](/images/refactorings/ReplaceBlockWithEmbeddedStatementInIfElse.png)

#### Replace block with statements (in each section)

* **Syntax**: switch statement
* **Scope**: switch keyword

![Replace block with statements (in each section)](/images/refactorings/ReplaceBlockWithStatementsInEachSection.png)

#### Replace conditional expression with if-else

* **Syntax**: conditional expression

![Replace conditional expression with if-else](/images/refactorings/ReplaceConditionalExpressionWithIfElse.png)

#### Replace constant with field

* **Syntax**: constant declaration

![Replace constant with field](/images/refactorings/ReplaceConstantWithField.png)

#### Replace 'Count/Length' with 'Length/Count'

* **Syntax**: member access expression
* **Scope**: name

![Replace 'Count/Length' with 'Length/Count'](/images/refactorings/ReplaceCountWithLengthOrLengthWithCount.png)

#### Replace do statement with while statement

* **Syntax**: do statement
* **Scope**: do keyword

![Replace do statement with while statement](/images/refactorings/ReplaceDoStatementWithWhileStatement.png)

#### Replace embedded statement with block

* **Syntax**: do statement, else clause, fixed statement, for statement, foreach statement, if statement, lock statement, using statement, while statement
* **Scope**: embedded statement

![Replace embedded statement with block](/images/refactorings/ReplaceEmbeddedStatementWithBlock.png)

#### Replace embedded statement with block (in if-else)

* **Syntax**: if-else chain
* **Scope**: topmost if keyword

![Replace embedded statement with block (in if-else)](/images/refactorings/ReplaceEmbeddedStatementWithBlockInIfElse.png)

#### Replace Enum.HasFlag method with bitwise operation

* **Syntax**: Enum.HasFlag method invocation

![Replace Enum.HasFlag method with bitwise operation](/images/refactorings/ReplaceEnumHasFlagWithBitwiseOperation.png)

#### Replace explicit type with 'var'

* **Syntax**: variable declaration, foreach statement
* **Scope**: type

![Replace explicit type with 'var'](/images/refactorings/ReplaceExplicitTypeWithVar.png)

#### Replace field with constant

* **Syntax**: read-only field

![Replace field with constant](/images/refactorings/ReplaceFieldWithConstant.png)

#### Replace for with foreach

* **Syntax**: for statement

![Replace for with foreach](/images/refactorings/ReplaceForWithForEach.png)

#### Replace foreach with for

* **Syntax**: foreach statement

![Replace foreach with for](/images/refactorings/ReplaceForEachWithFor.png)

#### Replace increment operator with decrement operator

* **Syntax**: prefix/postfix unary expression

![Replace increment operator with decrement operator](/images/refactorings/ReplaceIncrementOperatorWithDecrementOperator.png)

#### Replace interpolated string with string literal

* **Syntax**: Interpolated string without any interpolation

![Replace interpolated string with string literal](/images/refactorings/ReplaceInterpolatedStringWithStringLiteral.png)

#### Replace method invocation with '[]'

* **Syntax**: First/Last/ElementAt method invocation
* **Scope**: method name

![Replace method invocation with '[]'](/images/refactorings/ReplaceMethodInvocationWithElementAccess.png)

#### Replace method with property

* **Syntax**: method
* **Scope**: method header

![Replace method with property](/images/refactorings/ReplaceMethodWithProperty.png)

#### Replace prefix operator to postfix operator

* **Syntax**: prefix/postfix unary expression

![Replace prefix operator to postfix operator](/images/refactorings/ReplacePrefixOperatorWithPostfixOperator.png)

#### Replace property with method

* **Syntax**: read-only property
* **Scope**: property header

![Replace property with method](/images/refactorings/ReplacePropertyWithMethod.png)

#### Replace regular string literal with verbatim string literal

* **Syntax**: regular string literal

![Replace regular string literal with verbatim string literal](/images/refactorings/ReplaceRegularStringLiteralWithVerbatimStringLiteral.png)

#### Replace statements with block (in each section)

* **Syntax**: switch statement
* **Scope**: switch keyword

![Replace statements with block (in each section)](/images/refactorings/ReplaceStatementsWithBlockInEachSection.png)

#### Replace string literal with character literal

* **Syntax**: string literal

![Replace string literal with character literal](/images/refactorings/ReplaceStringLiteralWithCharacterLiteral.png)

#### Replace string literal with interpolated string

* **Syntax**: string literal

![Replace string literal with interpolated string](/images/refactorings/ReplaceStringLiteralWithInterpolatedString.png)

#### Replace string.Empty with ""

* **Syntax**: string.Empty

![Replace string.Empty with ""](/images/refactorings/ReplaceStringEmptyWithEmptyStringLiteral.png)

#### Replace string.Format with interpolated string

* **Syntax**: string.Format method

![Replace string.Format with interpolated string](/images/refactorings/ReplaceStringFormatWithInterpolatedString.png)

#### Replace switch section block with statements

* **Syntax**: switch section
* **Scope**: block

![Replace switch section block with statements](/images/refactorings/ReplaceSwitchSectionBlockWithStatements.png)

#### Replace switch section statements with block

* **Syntax**: switch section
* **Scope**: statements

![Replace switch section statements with block](/images/refactorings/ReplaceSwitchSectionStatementsWithBlock.png)

#### Replace switch with if-else

* **Syntax**: switch statement
* **Scope**: switch keyword

![Replace switch with if-else](/images/refactorings/ReplaceSwitchWithIfElse.png)

#### Replace 'var' with explicit type

* **Syntax**: variable declaration, foreach statetement
* **Scope**: type

![Replace 'var' with explicit type](/images/refactorings/ReplaceVarWithExplicitType.png)

#### Replace verbatim string literal with regular string literal

* **Syntax**: verbatim string literal

![Replace verbatim string literal with regular string literal](/images/refactorings/ReplaceVerbatimStringLiteralWithRegularStringLiteral.png)

#### Replace verbatim string literal with regular string literals

* **Syntax**: multiline verbatim string literal

![Replace verbatim string literal with regular string literals](/images/refactorings/ReplaceVerbatimStringLiteralWithRegularStringLiterals.png)

#### Replace while statement with do statement

* **Syntax**: while statement
* **Scope**: while keyword

![Replace while statement with do statement](/images/refactorings/ReplaceWhileStatementWithDoStatement.png)

#### Reverse for loop

* **Syntax**: for statement
* **Scope**: for keyword

![Reverse for loop](/images/refactorings/ReverseForLoop.png)

#### Simplify lambda expression

* **Syntax**: lambda expression with block with single single-line statement
* **Scope**: body

![Simplify lambda expression](/images/refactorings/SimplifyLambdaExpression.png)

#### Split attributes

* **Syntax**: selected attribute list

![Split attributes](/images/refactorings/SplitAttributes.png)

#### Split variable declaration 

* **Syntax**: local declaration, field declaration, event field declaration

![Split variable declaration ](/images/refactorings/SplitVariableDeclaration.png)

#### Swap expressions in binary expression

* **Syntax**: logical and/or expression
* **Scope**: binary operator

![Swap expressions in binary expression](/images/refactorings/SwapExpressionsInBinaryExpression.png)

#### Swap expressions in conditional expression

* **Syntax**: conditional expression
* **Scope**: condition

![Swap expressions in conditional expression](/images/refactorings/SwapExpressionsInConditionalExpression.png)

#### Swap members

* **Syntax**: empty line between member declarations

![Swap members](/images/refactorings/SwapMembers.png)

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

#### Wrap declaration in using statement

* **Syntax**: local declaration of type that implements IDisposable

![Wrap declaration in using statement](/images/refactorings/WrapDeclarationInUsingStatement.png)

#### Wrap expression in parentheses

* **Syntax**: selected expression

![Wrap expression in parentheses](/images/refactorings/WrapExpressionInParentheses.png)
