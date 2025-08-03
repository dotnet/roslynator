// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1246UseElementAccessTests : AbstractCSharpDiagnosticVerifier<InvocationExpressionAnalyzer, OptimizeLinqMethodCallCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseElementAccess;

    [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    [InlineData("((List<object>)x).[|First()|]", "((List<object>)x)[0]")]
    [InlineData("((IList<object>)x).[|First()|]", "((IList<object>)x)[0]")]
    [InlineData("((IReadOnlyList<object>)x).[|First()|]", "((IReadOnlyList<object>)x)[0]")]
    [InlineData("((Collection<object>)x).[|First()|]", "((Collection<object>)x)[0]")]
    [InlineData("((ImmutableArray<object>)x).[|First()|]", "((ImmutableArray<object>)x)[0]")]
    [InlineData("((object[])x).[|First()|]", "((object[])x)[0]")]
    [InlineData("((string)x).[|First()|]", "((string)x)[0]")]
    public async Task Test_UseElementAccessInsteadOfFirst(string source, string expected)
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

class C
{
    void M()
    {
        object x = null;
        var y = [||];
    }
}
", source, expected);
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    public async Task Test_UseElementAccessInsteadOfFirst_DerivedFromList()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Linq;
using System.Collections.Generic;

class C : List<string>
{
    void M()
    {
        var list = new C();
        var x = list.[|First()|];
    }
}
", @"
using System.Linq;
using System.Collections.Generic;

class C : List<string>
{
    void M()
    {
        var list = new C();
        var x = list[0];
    }
}
");
    }

    [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    [InlineData("((List<object>)x).[|ElementAt(1)|]", "((List<object>)x)[1]")]
    [InlineData("((IList<object>)x).[|ElementAt(1)|]", "((IList<object>)x)[1]")]
    [InlineData("((IReadOnlyList<object>)x).[|ElementAt(1)|]", "((IReadOnlyList<object>)x)[1]")]
    [InlineData("((Collection<object>)x).[|ElementAt(1)|]", "((Collection<object>)x)[1]")]
    [InlineData("((ImmutableArray<object>)x).[|ElementAt(1)|]", "((ImmutableArray<object>)x)[1]")]
    [InlineData("((object[])x).[|ElementAt(1)|]", "((object[])x)[1]")]
    [InlineData("((string)x).[|ElementAt(1)|]", "((string)x)[1]")]
    public async Task Test_UseElementAccessInsteadOfElementAt(string source, string expected)
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

class C
{
    void M()
    {
        object x = null;
        var y = [||];
    }
}
", source, expected);
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    public async Task Test_UseElementAccessInsteadOfElementAt_DerivedFromList()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Linq;
using System.Collections.Generic;

class C : List<string>
{
    void M()
    {
        var list = new C();
        var x = list.[|ElementAt(1)|];
    }
}
", @"
using System.Linq;
using System.Collections.Generic;

class C : List<string>
{
    void M()
    {
        var list = new C();
        var x = list[1];
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    public async Task TestNoDiagnostic_UseElementAccessInsteadOfElementAt()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

class C
{
    void M()
    {
        object x = null;

        x = ((ICollection<object>)x).ElementAt(1);
        x = ((IReadOnlyCollection<object>)x).ElementAt(1);
        x = ((IEnumerable<object>)x).ElementAt(1);

        x = ((Dictionary<object, object>)x).ElementAt(1);
    }
}
");
    }

    [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    [InlineData("((List<object>)x).[|Last()|]", "((List<object>)x)[^1]")]
    [InlineData("((IList<object>)x).[|Last()|]", "((IList<object>)x)[^1]")]
    [InlineData("((IReadOnlyList<object>)x).[|Last()|]", "((IReadOnlyList<object>)x)[^1]")]
    [InlineData("((Collection<object>)x).[|Last()|]", "((Collection<object>)x)[^1]")]
    [InlineData("((ImmutableArray<object>)x).[|Last()|]", "((ImmutableArray<object>)x)[^1]")]
    [InlineData("((object[])x).[|Last()|]", "((object[])x)[^1]")]
    [InlineData("((string)x).[|Last()|]", "((string)x)[^1]")]
    public async Task Test_UseElementAccessInsteadOfLast(string source, string expected)
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

class C
{
    void M()
    {
        object x = null;
        var y = [||];
    }
}
", source, expected);
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    public async Task Test_UseElementAccessInsteadOfLast_CSharp7()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Linq;
using System.Collections.Generic;

class C
{
    void M()
    {
        List<string> x = null;
        var y = x.Last();
    }
}
", options: WellKnownCSharpTestOptions.Default_CSharp7);
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    public async Task TestNoDiagnostic_UseElementAccessInsteadOfLast()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

class C
{
    void M()
    {
        object x = null;

        x = ((ICollection<object>)x).Last();
        x = ((IReadOnlyCollection<object>)x).Last();
        x = ((IEnumerable<object>)x).Last();

        x = ((Dictionary<object, object>)x).Last();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    public async Task TestNoDiagnostic_UseElementAccessInsteadOfFirst()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

class C
{
    void M()
    {
        object x = null;

        x = ((ICollection<object>)x).First();
        x = ((IReadOnlyCollection<object>)x).First();
        x = ((IEnumerable<object>)x).First();

        x = ((Dictionary<object, object>)x).First();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    public async Task TestNoDiagnostic_UseElementAccessOnElementAccess()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Linq;
using System.Collections.Generic;

class C
{
    List<object> this[int index] => null;

    void M()
    {
        C x = default;

        var first = x[0].First();
        var second = x[0].ElementAt(1);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    public async Task TestNoDiagnostic_UseElementAccess_ExpressionStatement()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Linq;
using System.Collections.Generic;

class C
{
    void M()
    {
        object x = null;

        ((List<object>)x).First();
        ((List<object>)x).ElementAt(1);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    public async Task TestNoDiagnostic_UseElementAccessInsteadOfElementAt_InfiniteRecursion()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

class C : IReadOnlyList<int>
{
    public int this[int index] => this.ElementAt(index);

    public int Count => throw new NotImplementedException();

    public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseElementAccess)]
    public async Task TestNoDiagnostic_UseElementAccessInsteadOf_OrderedDictionary()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        OrderedDictionary<string, string> dic = new OrderedDictionary<string, string>();

        KeyValuePair<string, string> first = dic.First();
        KeyValuePair<string, string> last = dic.Last();
        KeyValuePair<string, string> elementAt = dic.ElementAt(1);
     }
}
");
    }}
