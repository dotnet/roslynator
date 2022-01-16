# How to: Suppress Diagnostic

Suppression of diagnostics is useful to suppress rare cases that are not or cannot be covered by an analyzer.

This approach should not be used as a replacement for configuration of analyzers since analyzers that produce diagnostics still execute even if diagnostics are suppressed.

### Suppress Diagnostic for a Declaration

```csharp
using System.Diagnostics.CodeAnalysis;

class C
{
    [SuppressMessage("Readability", "RCS1008", Justification = "<Pending>")]
    void M()
    {
        var x = Foo(); // no RCS1008 here
    }
}
```

```csharp
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Readability", "RCS1008", Justification = "<Pending>", Scope = "member", Target = "~M:C.M")]

class C
{
    void M()
    {
        var x = Foo(); // no RCS1008 here
    }
}
```

### Suppress Diagnostic for Selected Lines

```csharp
using System.Diagnostics.CodeAnalysis;

class C
{
    void M()
    {
#pragma warning disable RCS1008
        var x = Foo(); // no RCS1008 here
#pragma warning restore RCS1008
    }
}
```

### Suppress Diagnostic for Namespace

```csharp
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Readability", "RCS1008", Justification = "<Pending>", Scope = "NamespaceAndDescendants", Target = "N1.N2")]

namespace N1.N2
{
    class C
    {
        void M()
        {
            var x = Foo(); // no RCS1008 here
        }
    }
}
```

### Suppress Diagnostic Globally

*Note: this option is applicable for Roslynator 2017*

Go to Visual Studio Tools > Options > Roslynator > Global Suppressions

![Global Suppressions](/images/GlobalSuppressionsOptions.png)

## See Also

* [Use code analyzers](https://docs.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers)
