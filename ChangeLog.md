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
