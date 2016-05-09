### 0.9.2 (2016-05-09)

#### Analyzers

* "MergeIfStatementWithContainedIfStatement" analyzer and code fix added
* "DeclareEachTypeInSeparateFile" analyzer added
* "AvoidLockingOnPubliclyAccessibleInstance" analyzer and code fix added (without batch fixer)
* "SimplifyAssignmentExpression" analyzer and code fix added
* "AddEmptyLinesBetweenDeclarations" analyzer and code fix added
* "AvoidUsingAliasDirective" analyzer added
* "AvoidSemicolonAtEndOfDeclaration" analyzer and code fix added
* "UseLogicalNotOperator" analyzer renamed to "SimplifyBooleanComparison" and improved
* "RemoveRedundantBooleanLiteral" analyzer now works for "&& true" and "|| false"

#### Refactorings

* "Add boolean comparison" refactoring added
* "Convert interpolated string to string literal" refactoring added
* "Convert string literal to interpolated string" refactoring added
* "Change 'Any/All' to 'All/Any'" refactoring added
* "Format all parameters on a single line" refactoring now works for parameter list with a single parameter
* "Convert to constant" refactoring now works only for predefined types (except object)
* "Remove comment/comments" refactorings now work for comments that are inside trivia
* "Make member abstract" refactoring now work only for non-abstract indexer/method/property that are in abstract class
* "Add/remove parameter name (to/from each argument)" refactorings now work when cursor is right behind the parameter
* Bug fixed in "Uncomment" refactoring

### 0.9.11 (2016-04-30)
 
* Bug fixes and minor improvements
    
### 0.9.1 (2016-04-27)
    
* Bug fixes
    
### 0.9.0 (2016-04-26)
    
* Initial release
