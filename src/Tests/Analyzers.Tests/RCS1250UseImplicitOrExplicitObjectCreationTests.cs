﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1250UseImplicitOrExplicitObjectCreationTests : AbstractCSharpDiagnosticVerifier<UseImplicitOrExplicitObjectCreationAnalyzer, UseImplicitOrExplicitObjectCreationCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseImplicitOrExplicitObjectCreation;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_ThrowStatement()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        throw new [|System.Exception|]();
    }
}
", @"
class C
{
    void M()
    {
        throw new();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_ThrowExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M() => throw new [|System.Exception|]();
}
", @"
class C
{
    string M() => throw new();
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_Property()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string P { get; } = new [|string|](' ', 1);
}
", @"
class C
{
    string P { get; } = new(' ', 1);
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_Field()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string F = new [|string|](' ', 1);
}
", @"
class C
{
    string F = new(' ', 1);
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_LocalDeclaration()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_DoNotPreferVar_LocalDeclaration()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var s = new [|string|](' ', 1);
    }
}
", @"
class C
{
    void M()
    {
        string s = new(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_DoNotPreferVar_LocalDeclaration2()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = new [|string|](' ', 1);
    }
}
", @"
class C
{
    void M()
    {
        string s = new(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_UsingStatement()
    {
        await VerifyNoDiagnosticAsync("""
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new StringReader(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_DoNotPreferVar_UsingStatement()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.IO;

class C
{
    void M()
    {
        using (var s = new [|StringReader|](""))
        {
        }
    }
}
""", """
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_DoNotPreferVar_UsingStatement2()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new [|StringReader|](""))
        {
        }
    }
}
""", """
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_ArrowExpressionClause()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M() => new [|string|](' ', 1);
}
", @"
class C
{
    string M() => new(' ', 1);
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_Array()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var arr = new string[] { new [|string|](' ', 1) };
    }
}
", @"
class C
{
    void M()
    {
        var arr = new string[] { new(' ', 1) };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_ReturnStatement()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        return new [|string|](' ', 1);
    }
}
", @"
class C
{
    string M()
    {
        return new(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_YieldReturnStatement()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {
        yield return new [|string|](' ', 1);
    }
}
", @"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {
        yield return new(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_Assignment()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;
        s = new [|string|](' ', 1);
    }
}
", @"
class C
{
    void M()
    {
        string s = null;
        s = new(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_CoalesceExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;
        string s2 = null;
        s = s2 ?? new [|string|](' ', 1);
    }
}
", @"
class C
{
    void M()
    {
        string s = null;
        string s2 = null;
        s = s2 ?? new(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_CollectionInitializer_Field()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    private List<string> _items = new()
    {
        new [|string|](' ', 1)
    };

    void M()
    {
    }
}
", @"
using System.Collections.Generic;

class C
{
    private List<string> _items = new()
    {
        new(' ', 1)
    };

    void M()
    {
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicit_CollectionInitializer_Local()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>()
        {
            new [|string|](' ', 1)
        };
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>()
        {
            new(' ', 1)
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_ThrowStatement()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        throw new [|System.Exception|]();
    }
}
", @"
class C
{
    void M()
    {
        throw new();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_ThrowExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M() => throw new [|System.Exception|]();
}
", @"
class C
{
    string M() => throw new();
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_Property()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string P { get; } = new [|string|](' ', 1);
}
", @"
class C
{
    string P { get; } = new(' ', 1);
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_Field()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string F = new [|string|](' ', 1);
}
", @"
class C
{
    string F = new(' ', 1);
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_LocalDeclaration()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_DoNotPreferVar_LocalDeclaration()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var s = new [|string|](' ', 1);
    }
}
", @"
class C
{
    void M()
    {
        string s = new(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_DoNotPreferVar_LocalDeclaration2()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = new [|string|](' ', 1);
    }
}
", @"
class C
{
    void M()
    {
        string s = new(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_UsingStatement()
    {
        await VerifyNoDiagnosticAsync("""
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new StringReader(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_DoNotPreferVar_UsingStatement()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.IO;

class C
{
    void M()
    {
        using (var s = new [|StringReader|](""))
        {
        }
    }
}
""", """
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_DoNotPreferVar_UsingStatement2()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new [|StringReader|](""))
        {
        }
    }
}
""", """
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_ArrowExpressionClause()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M() => new [|string|](' ', 1);
}
", @"
class C
{
    string M() => new(' ', 1);
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_Array()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var arr = new string[] { new [|string|](' ', 1) };
    }
}
", @"
class C
{
    void M()
    {
        var arr = new string[] { new(' ', 1) };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_ReturnStatement()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    string M()
    {
        return new(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_YieldReturnStatement()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {
        yield return new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_Assignment()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = null;
        s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_CoalesceExpression()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = null;
        string s2 = null;
        s = s2 ?? new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_CollectionInitializer_Field()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    private List<string> _items = new()
    {
        new [|string|](' ', 1)
    };

    void M()
    {
    }
}
", @"
using System.Collections.Generic;

class C
{
    private List<string> _items = new()
    {
        new(' ', 1)
    };

    void M()
    {
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_CollectionInitializer_Local()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>()
        {
            new [|string|](' ', 1)
        };
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>()
        {
            new(' ', 1)
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_ArrayInitializerInFieldInitializer()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    public string[] f = { new [|string|](' ', 0) };
}
", @"
class C
{
    public string[] f = { new(' ', 0) };
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_ArrayInitializerInPropertyInitializer()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    public string[] P { get; } = { new [|string|](' ', 0) };
}
", @"
class C
{
    public string[] P { get; } = { new(' ', 0) };
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_ArrayInitializerInLocalVariableInitializer()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string[] x = { new [|string|](' ', 0) };
    }
}
", @"
class C
{
    void M()
    {
        string[] x = { new(' ', 0) };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_PropertyLazyInitialization()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

public record R(C? P = null)
{
    public C P { get; init; } = P ?? new [|C|]();
}

public class C
{
}
", @"
#nullable enable

public record R(C? P = null)
{
    public C P { get; init; } = P ?? new();
}

public class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferImplicitWhenTypeIsObvious_FieldLazyInitialization()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

public record R(C? P = null)
{
    public C F = P ?? new [|C|]();
}

public class C
{
}
", @"
#nullable enable

public record R(C? P = null)
{
    public C F = P ?? new();
}

public class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_ThrowStatement()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        throw [|new()|];
    }
}
", @"
class C
{
    void M()
    {
        throw new System.Exception();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_ThrowExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M() => throw [|new()|];
}
", @"
class C
{
    string M() => throw new System.Exception();
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_Property()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string P { get; } = [|new(' ', 1)|];
}
", @"
class C
{
    string P { get; } = new string(' ', 1);
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_Field()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string F = [|new(' ', 1)|];
}
", @"
class C
{
    string F = new string(' ', 1);
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_LocalDeclaration()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_DoNotPreferVar_LocalDeclaration()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = [|new(' ', 1)|];
    }
}
", @"
class C
{
    void M()
    {
        string s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_DoNotPreferVar_LocalDeclaration2()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = [|new(' ', 1)|];
    }
}
", @"
class C
{
    void M()
    {
        string s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_UsingStatement()
    {
        await VerifyNoDiagnosticAsync("""
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new StringReader(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_DoNotPreferVar_UsingStatement()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = [|new("")|])
        {
        }
    }
}
""", """
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new StringReader(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_DoNotPreferVar_UsingStatement2()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = [|new("")|])
        {
        }
    }
}
""", """
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new StringReader(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_ArrowExpressionClause()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M() => [|new(' ', 1)|];
}
", @"
class C
{
    string M() => new string(' ', 1);
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_Array()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var arr = new string[] { [|new(' ', 1)|] };
    }
}
", @"
class C
{
    void M()
    {
        var arr = new string[] { new string(' ', 1) };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_ReturnStatement()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    string M()
    {
        return new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_YieldReturnStatement()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {
        yield return new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_Assignment()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = null;
        s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ConvertImplicitToExplicit_CoalesceExpression()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = null;
        string s2 = null;
        s = s2 ?? new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_ThrowStatement()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        throw new System.Exception();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_ThrowExpression()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    string M() => throw new System.Exception();
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_Property()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    string P { get; } = new string(' ', 1);
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_Field()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    string F = new string(' ', 1);
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_LocalDeclaration()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_DoNotPreferVar_LocalDeclaration()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        var s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_DoNotPreferVar_LocalDeclaration2()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_UsingStatement()
    {
        await VerifyNoDiagnosticAsync("""
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new StringReader(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_DoNotPreferVar_UsingStatement()
    {
        await VerifyNoDiagnosticAsync("""
using System.IO;

class C
{
    void M()
    {
        using (var s = new StringReader(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_DoNotPreferVar_UsingStatement2()
    {
        await VerifyNoDiagnosticAsync("""
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new StringReader(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit)
            .AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_ArrowExpressionClause()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    string M() => new string(' ', 1);
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_Array()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        var arr = new string[] { new string(' ', 1) };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_ReturnStatement()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    string M()
    {
        return new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_YieldReturnStatement()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {
        yield return new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_Assignment()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = null;
        s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_CoalesceExpression()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = null;
        string s2 = null;
        s = s2 ?? new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_CollectionInitializer_Field()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    private List<string> _items = new List<string>()
    {
        [|new(' ', 1)|]
    };

    void M()
    {
    }
}
", @"
using System.Collections.Generic;

class C
{
    private List<string> _items = new List<string>()
    {
        new string(' ', 1)
    };

    void M()
    {
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferExplicit_CollectionInitializer_Local()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>()
        {
            [|new(' ', 1)|]
        };
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>()
        {
            new string(' ', 1)
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferVarInsteadOfImplicitObjectCreation_DoNotPreferVar_LocalDeclaration()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = [|new(' ', 1)|];
    }
}
", @"
class C
{
    void M()
    {
        var s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferVarInsteadOfImplicitObjectCreation_DoNotPreferVar_LocalDeclaration2()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = new(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferVarInsteadOfImplicitObjectCreation_DoNotPreferVar_UsingStatement()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = [|new("")|])
        {
        }
    }
}
""", """
using System.IO;

class C
{
    void M()
    {
        using (var s = new StringReader(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_PreferVarInsteadOfImplicitObjectCreation_DoNotPreferVar_UsingStatement2()
    {
        await VerifyNoDiagnosticAsync("""
using System.IO;

class C
{
    void M()
    {
        using (StringReader s = new(""))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.UseVarInsteadOfImplicitObjectCreation, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task TestNoDiagnostic_ForEachExpression2()
    {
        await VerifyNoDiagnosticAsync("""
class C
{
    string[] M()
    {
        return ["", ""];
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ExplicitToCollectionExpression_ImplicitStyle()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;
class C
{
    void M(List<string> items)
    {
        items = new [|List<string>|]() { "" };
    }
}
""", """
using System.Collections.Generic;
class C
{
    void M(List<string> items)
    {
        items = [""];
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ExplicitToCollectionExpression_ImplicitWhenObviousStyle()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;
class C
{
    List<string> P { get; } = new [|List<string>|]() { "" };
}
""", """
using System.Collections.Generic;
class C
{
    List<string> P { get; } = [""];
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ImplicitToCollectionExpression_ImplicitStyle()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;
class C
{
    void M(List<string> items)
    {
        items = [|new() { "" }|];
    }
}
""", """
using System.Collections.Generic;
class C
{
    void M(List<string> items)
    {
        items = [""];
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ImplicitToCollectionExpression_ImplicitWhenObviousStyle()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;
class C
{
    List<string> P { get; } = [|new() { "" }|];
}
""", """
using System.Collections.Generic;
class C
{
    List<string> P { get; } = [""];
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_CollectionExpressionToExplicit_ImplicitStyle()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;
class C
{
    void M(List<string> items)
    {
        items = [|[""]|];
    }
}
""", """
using System.Collections.Generic;
class C
{
    void M(List<string> items)
    {
        items = new List<string>() { "" };
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_CollectionExpressionToExplicit_ImplicitWhenObviousStyle()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;
class C
{
    private string _f = "";

    void M(List<string> items)
    {
        items = [|[_f]|];
    }
}
""", """
using System.Collections.Generic;
class C
{
    private string _f = "";

    void M(List<string> items)
    {
        items = new List<string>() { _f };
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_CollectionExpressionToImplicit_ImplicitStyle()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;
class C
{
    void M(List<string> items)
    {
        items = [|[""]|];
    }
}
""", """
using System.Collections.Generic;
class C
{
    void M(List<string> items)
    {
        items = new() { "" };
    }
}
""", options: Options
            .AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_CollectionExpressionToImplicit_ImplicitWhenObviousStyle()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;
class C
{
    List<string> P { get; } = [|[""]|];
}
""", """
using System.Collections.Generic;
class C
{
    List<string> P { get; } = new() { "" };
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_ExplicitWithParameters()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;
class C : List<string> 
{
    C() { }
    C(string p) { }

    C M()
    {
        return new [|C|]("s");
    }   
}
""", """
using System.Collections.Generic;
class C : List<string> 
{
    C() { }
    C(string p) { }

    C M()
    {
        return new("s");
    }   
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_ImplicitWhenTypeIsObvious)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task Test_CollectionExpressionToExplicit()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;
class C
{
    void M(List<string> x)
    {
        x = [|[]|];
    }   
}
", @"
using System.Collections.Generic;
class C
{
    void M(List<string> x)
    {
        x = new List<string>();
    }   
}
", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task TestNoDiagnostic_ObjectInitializerWithPropertySet()
    {
        await VerifyNoDiagnosticAsync("""
using System;
using System.Collections;
using System.Collections.Generic;

class C : IEnumerable<int>
{
    public string P { get; set; }

    public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    static C M()
    {
        C c = new() { P = "" };

        return c;
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task TestNoDiagnostic_Array()
    {
        await VerifyNoDiagnosticAsync("""
using System;

class C
{
    void M1()
    {
        string[] values = [];
    }

    void M2()
    {
        string[] values = ["a", "b", "c"];
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseImplicitOrExplicitObjectCreation)]
    public async Task TestNoDiagnostic_ComplexElementInitializer()
    {
        await VerifyNoDiagnosticAsync("""
using System;
using System.Collections;
using System.Collections.Generic;
class C : IEnumerable<KeyValuePair<int, int>>
{
    public IEnumerator<KeyValuePair<int, int>> GetEnumerator() => throw new System.NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => throw new System.NotImplementedException();

    public void Add(int arg1, int arg2)
    {
    }

    static C M()
    {
        C c = new() { { 1, 2 } };
        return c;
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.ObjectCreationTypeStyle, ConfigOptionValues.ObjectCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }
}
