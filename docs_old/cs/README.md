## Compiler Diagnostics Fixable with Roslynator

| Id  | Title |
| --- | ----- |
| [CS0019](CS0019.md) | Operator 'operator' cannot be applied to operands of type 'type' and 'type'\. |
| [CS0021](CS0021.md) | Cannot apply indexing with \[\] to an expression of type 'type'\. |
| [CS0023](CS0023.md) | Operator 'operator' cannot be applied to operand of type 'type'\. |
| [CS0029](CS0029.md) | Cannot implicitly convert type 'type' to 'type'\. |
| [CS0030](CS0030.md) | Cannot convert type 'type' to 'type'\. |
| [CS0037](CS0037.md) | Cannot convert null to 'type' because it is a non\-nullable value type\. |
| [CS0069](CS0069.md) | An event in an interface cannot have add or remove accessors\. |
| [CS0077](CS0077.md) | The as operator must be used with a reference type or nullable type \('type' is a non\-nullable value type\)\. |
| [CS0080](CS0080.md) | Constraints are not allowed on non\-generic declarations\. |
| [CS0101](CS0101.md) | The namespace 'namespace' already contains a definition for 'type'\. |
| [CS0102](CS0102.md) | The type 'type name' already contains a definition for 'identifier'\. |
| [CS0103](CS0103.md) | The name 'identifier' does not exist in the current context\. |
| [CS0107](CS0107.md) | More than one protection modifier\. |
| [CS0109](CS0109.md) | The member 'member' does not hide an inherited member\. The new keyword is not required\. |
| [CS0112](CS0112.md) | A static member 'function' cannot be marked as override, virtual or abstract\. |
| [CS0114](CS0114.md) | 'function1' hides inherited member 'function2'\. To make the current method override that implementation, add the override keyword\. Otherwise add the new keyword\. |
| [CS0115](CS0115.md) | 'function': no suitable method found to override\. |
| [CS0119](CS0119.md) | 'identifier' is a 'construct', which is not valid in the given context\. |
| [CS0120](CS0120.md) | An object reference is required for the nonstatic field, method, or property 'member'\. |
| [CS0123](CS0123.md) | No overload for 'method' matches delegate 'delegate'\. |
| [CS0126](CS0126.md) | An object of a type convertible to 'type' is required\. |
| [CS0127](CS0127.md) | Since 'function' returns void, a return keyword must not be followed by an object expression\. |
| [CS0128](CS0128.md) | A local variable named 'variable' is already defined in this scope\. |
| [CS0131](CS0131.md) | The left\-hand side of an assignment must be a variable, property or indexer\. |
| [CS0132](CS0132.md) | 'constructor': a static constructor must be parameterless\. |
| [CS0133](CS0133.md) | The expression being assigned to 'variable' must be constant\. |
| [CS0136](CS0136.md) | A local variable named 'var' cannot be declared in this scope because it would give a different meaning to 'var', which is already used in a 'parent or current/child' scope to denote something else\. |
| [CS0139](CS0139.md) | No enclosing loop out of which to break or continue\. |
| [CS0152](CS0152.md) | The switch statement contains multiple cases with same label value\. |
| [CS0161](CS0161.md) | 'method': not all code paths return a value\. |
| [CS0162](CS0162.md) | Unreachable code detected\. |
| [CS0163](CS0163.md) | Control cannot fall through from one case label \('label'\) to another\. |
| [CS0164](CS0164.md) | This label has not been referenced\. |
| [CS0165](CS0165.md) | Use of unassigned local variable 'name'\. |
| [CS0168](CS0168.md) | The variable 'var' is declared but never used\. |
| [CS0173](CS0173.md) | Type of conditional expression cannot be determined because there is no implicit conversion between 'class1' and 'class2'\. |
| [CS0177](CS0177.md) | The out parameter 'parameter' must be assigned to before control leaves the current method\. |
| [CS0191](CS0191.md) | A readonly field cannot be assigned to \(except in a constructor or a variable initializer\)\. |
| [CS0192](CS0192.md) | Readonly field 'name' cannot be used as ref or out value \(except in a constructor\)\. |
| [CS0201](CS0201.md) | Only assignment, call, increment, decrement, and new object expressions can be used as a statement\. |
| [CS0214](CS0214.md) | Pointers and fixed size buffers may only be used in an unsafe context\. |
| [CS0216](CS0216.md) | The operator 'operator' requires a matching operator 'missing\_operator' to also be defined\. |
| [CS0219](CS0219.md) | The variable 'variable' is assigned but its value is never used\. |
| [CS0221](CS0221.md) | Constant value 'value' cannot be converted to a 'type' \(use 'unchecked' syntax to override\)\. |
| [CS0225](CS0225.md) | The params parameter must be a single dimensional array\. |
| [CS0238](CS0238.md) | 'identifier' cannot be sealed because it is not an override\. |
| [CS0246](CS0246.md) | The type or namespace name 'type/namespace' could not be found \(are you missing a using directive or an assembly reference?\)\. |
| [CS0260](CS0260.md) | Missing partial modifier on declaration of type 'type'; another partial declaration of this type exists\. |
| [CS0262](CS0262.md) | Partial declarations of 'type' have conflicting accessibility modifiers\. |
| [CS0266](CS0266.md) | Cannot implicitly convert type 'type1' to 'type2'\. An explicit conversion exists \(are you missing a cast?\)\. |
| [CS0267](CS0267.md) | The partial modifier can only appear immediately before 'class', 'struct', or 'interface'\. |
| [CS0275](CS0275.md) | 'accessor': accessibility modifiers may not be used on accessors in an interface\. |
| [CS0305](CS0305.md) | Using the generic type 'generic type' requires 'number' type arguments\. |
| [CS0401](CS0401.md) | The new\(\) constraint must be the last constraint specified\. |
| [CS0403](CS0403.md) | Cannot convert null to type parameter 'name' because it could be a non\-nullable value type\. Consider using default\('T'\) instead\. |
| [CS0405](CS0405.md) | Duplicate constraint 'constraint' for type parameter 'type parameter'\. |
| [CS0407](CS0407.md) | 'return\-type method' has the wrong return type\. |
| [CS0409](CS0409.md) | A constraint clause has already been specified for type parameter 'type parameter'\. All of the constraints for a type parameter must be specified in a single where clause\. |
| [CS0428](CS0428.md) | Cannot convert method group 'identifier' to non\-delegate type 'type'\. Did you intend to invoke the method? |
| [CS0441](CS0441.md) | 'class': a class cannot be both static and sealed\. |
| [CS0442](CS0442.md) | 'property': abstract properties cannot have private accessors\. |
| [CS0449](CS0449.md) | The 'class' or 'struct' constraint must come before any other constraints\. |
| [CS0450](CS0450.md) | 'type parameter name': cannot specify both a constraint class and the 'class' or 'struct' constraint\. |
| [CS0451](CS0451.md) | The 'new\(\)' constraint cannot be used with the 'struct' constraint\. |
| [CS0472](CS0472.md) | The result of the expression is always 'value1' since a value of type 'value2' is never equal to 'null' of type 'value3'\. |
| [CS0500](CS0500.md) | 'class member' cannot declare a body because it is marked abstract\. |
| [CS0501](CS0501.md) | 'member function' must declare a body because it is not marked abstract, extern, or partial\. |
| [CS0507](CS0507.md) | 'function1': cannot change access modifiers when overriding 'access' inherited member 'function2'\. |
| [CS0508](CS0508.md) | 'type1': return type must be 'type2' to match overridden member 'member name'\. |
| [CS0513](CS0513.md) | 'function' is abstract but it is contained in nonabstract class 'class'\. |
| [CS0515](CS0515.md) | 'function': access modifiers are not allowed on static constructors\. |
| [CS0525](CS0525.md) | Interfaces cannot contain fields\. |
| [CS0527](CS0527.md) | Type 'type' in interface list is not an interface\. |
| [CS0531](CS0531.md) | 'member': interface members cannot have a definition\. |
| [CS0539](CS0539.md) | 'member' in explicit interface declaration is not a member of interface |
| [CS0541](CS0541.md) | 'declaration': explicit interface declaration can only be declared in a class or struct\. |
| [CS0549](CS0549.md) | New virtual member in sealed class\. |
| [CS0558](CS0558.md) | User\-defined operator 'operator' must be declared static and public\. |
| [CS0567](CS0567.md) | Interfaces cannot contain operators\. |
| [CS0568](CS0568.md) | Structs cannot contain explicit parameterless constructors\. |
| [CS0573](CS0573.md) | 'field declaration': cannot have instance field initializers in structs\. |
| [CS0574](CS0574.md) | Name of destructor must match name of class\. |
| [CS0575](CS0575.md) | Only class types can contain destructors\. |
| [CS0579](CS0579.md) | Duplicate 'attribute' attribute\. |
| [CS0592](CS0592.md) | Attribute 'attribute' is not valid on this declaration type\. It is valid on 'type' declarations only\. |
| [CS0621](CS0621.md) | 'member': virtual or abstract members cannot be private\. |
| [CS0628](CS0628.md) | member' : new protected member declared in sealed class\. |
| [CS0659](CS0659.md) | 'class' overrides Object\.Equals\(object o\) but does not override Object\.GetHashCode\(\)\. |
| [CS0660](CS0660.md) | 'class' defines operator == or operator \!= but does not override Object\.Equals\(object o\)\. |
| [CS0661](CS0661.md) | 'class' defines operator == or operator \!= but does not override Object\.GetHashCode\(\)\. |
| [CS0678](CS0678.md) | 'variable': a field can not be both volatile and readonly\. |
| [CS0693](CS0693.md) | Type parameter 'type parameter' has the same name as the type parameter from outer type 'type'\. |
| [CS0708](CS0708.md) | 'field': cannot declare instance members in a static class\. |
| [CS0710](CS0710.md) | Static classes cannot have instance constructors |
| [CS0713](CS0713.md) | Static class 'static type' cannot derive from type 'type'\. Static classes must derive from object\. |
| [CS0714](CS0714.md) | Static class cannot implement interfaces\. |
| [CS0718](CS0718.md) | 'type': static types cannot be used as type arguments\. |
| [CS0750](CS0750.md) | A partial method cannot have access modifiers or the virtual, abstract, override, new, sealed, or extern modifiers\. |
| [CS0751](CS0751.md) | A partial method must be declared in a partial class or partial struct\. |
| [CS0753](CS0753.md) | Only methods, classes, structs, or interfaces may be partial\. |
| [CS0756](CS0756.md) | A partial method may not have multiple defining declarations\. |
| [CS0759](CS0759.md) | No defining declaration found for implementing declaration of partial method 'method'\. |
| [CS0766](CS0766.md) | Partial methods must have a void return type\. |
| [CS0815](CS0815.md) | Cannot assign 'expression' to an implicitly typed local\. |
| [CS0819](CS0819.md) | Implicitly typed locals cannot have multiple declarators\. |
| [CS0822](CS0822.md) | Implicitly typed locals cannot be const\. |
| [CS1003](CS1003.md) | Syntax error, 'char' expected\. |
| [CS1004](CS1004.md) | Duplicate 'modifier' modifier\. |
| [CS1012](CS1012.md) | Too many characters in character literal\. |
| [CS1023](CS1023.md) | Embedded statement cannot be a declaration or labeled statement\. |
| [CS1031](CS1031.md) | Type expected\. |
| [CS1057](CS1057.md) | 'member': static classes cannot contain protected members\. |
| [CS1061](CS1061.md) | 'type' does not contain a definition for 'member' and no extension method 'name' accepting a first argument of type 'type' could be found \(are you missing a using directive or an assembly reference?\)\. |
| [CS1100](CS1100.md) | Method 'name' has a parameter modifier 'this' which is not on the first parameter\. |
| [CS1105](CS1105.md) | Extension methods must be static\. |
| [CS1106](CS1106.md) | Extension methods must be defined in a non\-generic static class\. |
| [CS1503](CS1503.md) | Argument 'number' cannot convert from 'type1' to 'type2'\. |
| [CS1522](CS1522.md) | Empty switch block\. |
| [CS1526](CS1526.md) | A new expression requires \(\), \[\], or \{\} after type\. |
| [CS1527](CS1527.md) | Elements defined in a namespace cannot be explicitly declared as private, protected, protected internal or private protected\. |
| [CS1591](CS1591.md) | Missing XML comment for publicly visible type or member 'name'\. |
| [CS1597](CS1597.md) | Semicolon after method or accessor block is not valid\. |
| [CS1609](CS1609.md) | Modifiers cannot be placed on event accessor declarations\. |
| [CS1615](CS1615.md) | Argument 'number' should not be passed with the 'keyword' keyword\. |
| [CS1620](CS1620.md) | Argument 'number' must be passed with the 'keyword' keyword\. |
| [CS1621](CS1621.md) | The yield statement cannot be used inside an anonymous method or lambda expression\. |
| [CS1622](CS1622.md) | Cannot return a value from an iterator\. Use the yield return statement to return a value, or yield break to end the iteration\. |
| [CS1623](CS1623.md) | Iterators cannot have ref or out parameters\. |
| [CS1624](CS1624.md) | The body of 'identifier' cannot be an iterator block because 'type' is not an iterator interface type\. |
| [CS1643](CS1643.md) | Not all code paths return a value in method of type 'type'\. |
| [CS1674](CS1674.md) | 'T': type used in a using statement must be implicitly convertible to 'System\.IDisposable'\. |
| [CS1689](CS1689.md) | Attribute 'attribute' is only valid on methods or attribute classes\. |
| [CS1715](CS1715.md) | 'type1': type must be 'type2' to match overridden member 'member'\. |
| [CS1717](CS1717.md) | Assignment made to same variable; did you mean to assign something else? |
| [CS1722](CS1722.md) | Base class 'class' must come before any interfaces\. |
| [CS1737](CS1737.md) | Optional parameters must appear after all required parameters\. |
| [CS1741](CS1741.md) | A ref or out parameter cannot have a default value\. |
| [CS1743](CS1743.md) | Cannot specify a default value for the 'this' parameter\. |
| [CS1750](CS1750.md) | A value of type 'type1' cannot be used as a default parameter because there are no standard conversions to type 'type2\. |
| [CS1751](CS1751.md) | Cannot specify a default value for a parameter array\. |
| [CS1955](CS1955.md) | Non\-invocable member 'name' cannot be used like a method\. |
| [CS1983](CS1983.md) | The return type of an async method must be void, Task or Task\<T>\. |
| [CS1988](CS1988.md) | Async methods cannot have ref or out parameters\. |
| [CS1994](CS1994.md) | The 'async' modifier can only be used in methods that have a body\. |
| [CS1997](CS1997.md) | Since 'RemoveReturnKeywordOrReturnExpression\.FooAsync\(\)' is an async method that returns 'Task', a return keyword must not be followed by an object expression\. Did you intend to return 'Task\<T>>'? |
| [CS3000](CS3000.md) | Methods with variable arguments are not CLS\-compliant\. |
| [CS3001](CS3001.md) | Argument type 'type' is not CLS\-compliant\. |
| [CS3002](CS3002.md) | Return type of 'method' is not CLS\-compliant\. |
| [CS3003](CS3003.md) | Type of 'variable' is not CLS\-compliant\. |
| [CS3005](CS3005.md) | Identifier 'identifier' differing only in case is not CLS\-compliant\. |
| [CS3006](CS3006.md) | Overloaded method 'method' differing only in ref or out, or in array rank, is not CLS\-compliant\. |
| [CS3007](CS3007.md) | Overloaded method 'method' differing only by unnamed array types is not CLS\-compliant\. |
| [CS3008](CS3008.md) | Identifier 'identifier' is not CLS\-compliant\. |
| [CS3009](CS3009.md) | Base type 'type' is not CLS\-compliant\. |
| [CS3016](CS3016.md) | Arrays as attribute arguments is not CLS\-compliant\. |
| [CS3024](CS3024.md) | Constraint type 'type' is not CLS\-compliant\. |
| [CS3027](CS3027.md) | 'type\_1' is not CLS\-compliant because base interface 'type\_2' is not CLS\-compliant\. |
| [CS7036](CS7036.md) | There is no argument given that corresponds to the required formal parameter 'parameter' of 'member'\. |
| [CS8050](CS8050.md) | Only auto\-implemented properties can have initializers\. |
| [CS8070](CS8070.md) | Control cannot fall out of switch from final case label \('default'\)\. |
| [CS8112](CS8112.md) | 'function' is a local function and must therefore always have a body\. |
| [CS8139](CS8139.md) | Cannot change tuple element names when overriding inherited member\. |
| [CS8340](CS8340.md) | Instance fields of read\-only structs must be read\-only\. |
| [CS8403](CS8403.md) | Method with an iterator block must be 'async' to return 'IAsyncEnumerable\<T\<'\. |
| [CS8602](CS8602.md) | Dereference of a possibly null reference\. |
| [CS8604](CS8604.md) | Possible null reference argument for parameter\. |
| [CS8618](CS8618.md) | Non\-nullable member is uninitialized\. Consider declaring the member as nullable\. |
| [CS8625](CS8625.md) | Cannot convert null literal to non\-nullable reference type\. |
| [CS8632](CS8632.md) | The annotation for nullable reference types should only be used in code within a '\#nullable' annotations context\. |


*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*