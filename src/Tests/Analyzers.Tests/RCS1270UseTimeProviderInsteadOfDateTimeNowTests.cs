// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CSharp.Analysis;
using Roslynator.CSharp.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1270UseTimeProviderInsteadOfDateTimeNowTests : AbstractCSharpDiagnosticVerifier<UseTimeProviderInsteadOfDateTimeNowAnalyzer, UseTimeProviderInsteadOfDateTimeNowCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseTimeProviderInsteadOfDateTimeNow;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_DateTimeUtcNow_PrimaryConstructorParameter()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C(TimeProvider timeProvider)
{
    long Timestamp() => timeProvider.GetTimestamp();

    DateTime M() => [|DateTime.UtcNow|];
}
", @"
using System;

class C(TimeProvider timeProvider)
{
    long Timestamp() => timeProvider.GetTimestamp();

    DateTime M() => timeProvider.GetUtcNow().UtcDateTime;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_DateTimeNow_Field()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    private readonly TimeProvider _timeProvider;

    C(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    string M()
    {
        return [|DateTime.Now|].ToString();
    }
}
", @"
using System;

class C
{
    private readonly TimeProvider _timeProvider;

    C(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    string M()
    {
        return _timeProvider.GetLocalNow().DateTime.ToString();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_DateTimeToday_Parameter()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    DateTime M(TimeProvider clock)
    {
        return [|System.DateTime.Today|];
    }
}
", @"
using System;

class C
{
    DateTime M(TimeProvider clock)
    {
        return clock.GetLocalNow().Date;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_DateTimeOffset_Local()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        TimeProvider timeProvider = TimeProvider.System;

        DateTimeOffset local = [|DateTimeOffset.Now|];
        DateTimeOffset utc = [|DateTimeOffset.UtcNow|];
    }
}
", @"
using System;

class C
{
    void M()
    {
        TimeProvider timeProvider = TimeProvider.System;

        DateTimeOffset local = timeProvider.GetLocalNow();
        DateTimeOffset utc = timeProvider.GetUtcNow();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_ParameterIsPreferredOverField()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    private readonly TimeProvider _timeProvider;
    private readonly DateTime _created;

    C(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        _created = [|DateTime.UtcNow|];
    }
}
", @"
using System;

class C
{
    private readonly TimeProvider _timeProvider;
    private readonly DateTime _created;

    C(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        _created = timeProvider.GetUtcNow().UtcDateTime;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_DerivedTimeProviderProperty()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class FakeTimeProvider : TimeProvider
{
}

class C
{
    public FakeTimeProvider Clock { get; } = new FakeTimeProvider();

    DateTime M() => [|DateTime.UtcNow|];
}
", @"
using System;

class FakeTimeProvider : TimeProvider
{
}

class C
{
    public FakeTimeProvider Clock { get; } = new FakeTimeProvider();

    DateTime M() => Clock.GetUtcNow().UtcDateTime;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_NoFix_NoTimeProviderInScope()
    {
        await VerifyDiagnosticAndNoFixAsync(@"
using System;

class C
{
    DateTime M() => [|DateTime.UtcNow|];
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_NoFix_InstanceFieldInStaticMethod()
    {
        await VerifyDiagnosticAndNoFixAsync(@"
using System;

class C
{
    private readonly TimeProvider _timeProvider = TimeProvider.System;

    static DateTime M() => [|DateTime.UtcNow|];
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_NoFix_InstanceFieldInFieldInitializer()
    {
        await VerifyDiagnosticAndNoFixAsync(@"
using System;

class C
{
    private readonly TimeProvider _timeProvider = TimeProvider.System;
    private readonly DateTime _created = [|DateTime.UtcNow|];
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_PrimaryConstructorParameterInFieldInitializer()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C(TimeProvider timeProvider)
{
    private readonly DateTime _created = [|DateTime.UtcNow|];

    long Timestamp() => timeProvider.GetTimestamp();
}
", @"
using System;

class C(TimeProvider timeProvider)
{
    private readonly DateTime _created = timeProvider.GetUtcNow().UtcDateTime;

    long Timestamp() => timeProvider.GetTimestamp();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_NoFix_StaticLambdaCannotCaptureParameter()
    {
        await VerifyDiagnosticAndNoFixAsync(@"
using System;

class C
{
    void M(TimeProvider timeProvider)
    {
        Func<DateTime> f = static () => [|DateTime.UtcNow|];
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_NoFix_AmbiguousTimeProviders()
    {
        await VerifyDiagnosticAndNoFixAsync(@"
using System;

class C
{
    DateTime M(TimeProvider first, TimeProvider second) => [|DateTime.UtcNow|];
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task Test_NoFix_LocalDeclaredLater()
    {
        await VerifyDiagnosticAndNoFixAsync(@"
using System;

class C
{
    void M()
    {
        DateTime now = [|DateTime.UtcNow|];
        TimeProvider timeProvider = TimeProvider.System;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task TestNoDiagnostic_InsideTimeProvider()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class FixedTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;

    public override DateTimeOffset GetUtcNow() => _now;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task TestNoDiagnostic_NameOf()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    string M() => nameof(DateTime.Now);
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task TestNoDiagnostic_ExpressionTree()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.Linq.Expressions;

class C
{
    Expression<Func<DateTime, bool>> M() => d => d < DateTime.UtcNow;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseTimeProviderInsteadOfDateTimeNow)]
    public async Task TestNoDiagnostic_OtherMembers()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M(DateTime value)
    {
        DateTime max = DateTime.MaxValue;
        DateTime date = value.Date;
        DateTime now = Clock.Now;
        DateTimeOffset offset = DateTimeOffset.MinValue;
    }
}

static class Clock
{
    public static DateTime Now { get; }
}
");
    }
}
