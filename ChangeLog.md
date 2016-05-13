### 0.9.2 (2016-05-09)

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
