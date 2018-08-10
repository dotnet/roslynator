### 1.0.0-rc4 (2018-08-10)

#### New API

* Add struct `Roslynator.CSharp.ExpressionChain`
* Add method `Roslynator.CSharp.SyntaxExtensions.AsChain`
* Add method `Roslynator.CSharp.Syntax.BinaryExpressionInfo.AsChain`
* Add method `Roslynator.CSharp.Syntax.StringConcatenationExpressionInfo.AsChain`
* Add method `Roslynator.CSharp.CSharpFacts.IsLiteralExpression`

#### Changed API

* Replace abstract class `Roslynator.Selection<T>` with interface `Roslynator.ISelection<T>`
* Make method `Roslynator.CSharp.Syntax.BinaryExpressionInfo.Expressions` obsolete
* Make method `Roslynator.CSharp.Syntax.StringConcatenationExpressionInfo.Expressions` obsolete

### 1.0.0-rc3 (2018-06-06)

#### Bug Fixes

* Struct Roslynator.ExtensionMethodSymbolInfo implements IEquatable<ExtensionMethodSymbolInfo>

#### New API

* Add method Roslynator.CSharp.CSharpFactory.DefaultLiteralExpression
* Add methods Roslynator.CSharp.CSharpFactory.LocalFunctionStatement
* Add extension method Roslynator.CSharp.SyntaxExtensions ToSyntaxTokenList(IEnumerable<SyntaxToken>)
* Add field 'This' to enum Roslynator.CSharp.ModifierKinds

### 1.0.0-rc2 (2018-05-17)

#### Bug Fixes

* Fix method Roslynator.CSharp.CSharpFactory.NameOfExpression

#### New API

* Add extension method Roslynator.CSharp.SyntaxExtensions.ReplaceRange(SyntaxList<TNode>, int, int, IEnumerable<TNode>)
* Add extension method Roslynator.CSharp.SyntaxExtensions.ReplaceRange(SeparatedSyntaxList<TNode>, int, int, IEnumerable<TNode>)
* Add extension method Roslynator.CSharp.SyntaxExtensions.ReplaceRange(SyntaxTokenList, int, int, IEnumerable<SyntaxToken>)
* Add extension method Roslynator.CSharp.SyntaxExtensions.ReplaceRange(SyntaxTriviaList, int, int, IEnumerable<SyntaxTrivia>)
* Add extension method Roslynator.CSharp.SyntaxExtensions.RemoveRange(SyntaxTriviaList, int, int)
* Add extension method Roslynator.CSharp.SyntaxExtensions.RemoveRange(SyntaxTokenList, int, int)

#### Changed API

* Add optional parameters to extension method Roslynator.SyntaxExtensions.TryGetContainingList(SyntaxTrivia, SyntaxTriviaList, bool, bool)

#### Improvements

* Add struct enumerator to types derived from Roslynator.Selection<T>
  * Roslynator.SyntaxListSelection<TNode>
  * Roslynator.SeparatedSyntaxListSelection<TNode>
  * Roslynator.Text.TextLineCollectionSelection

* Add DebuggerDisplay attribute

### 1.0.0-rc (2018-05-02)

#### Bug Fixes

* Fix extension method Roslynator.CSharp.CSharpExtensions.IsDefaultValue

#### New API

* Add extension method Roslynator.CSharp.SyntaxExtension.AddAttributeLists
* Add extension method Roslynator.CSharp.SyntaxExtensions.RemoveRange
* Add extension method Roslynator.SymbolExtensions.HasAttribute
* Add extension method Roslynator.SymbolExtensions.IsPubliclyVisible
* Add extension method Roslynator.SyntaxExtensions.DescendantTrivia

### 1.0.0-beta (2018-04-10)

* Initial release