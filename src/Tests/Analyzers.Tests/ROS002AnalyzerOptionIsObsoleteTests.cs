// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class ROS0002AnalyzerOptionIsObsoleteTests : AbstractCSharpDiagnosticVerifier<AnalyzerOptionIsObsoleteAnalyzer, DummyCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = CommonDiagnosticRules.AnalyzerOptionIsObsolete;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveBracesFromIfElse)]
        public async Task Test()
        {
            await VerifyDiagnosticAsync(@"
class C
{
    void M()
    {
    }
}
", options: Options.AddConfigOption("dotnet_diagnostic.RCS1207.severity", "suggestion")
                .AddConfigOption(LegacyConfigOptions.ConvertMethodGroupToAnonymousFunction.Key, "true"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveBracesFromIfElse)]
        public async Task Test2()
        {
            await VerifyDiagnosticAsync(@"
class C
{
    void M()
    {
    }
}
", options: Options.EnableDiagnostic(DiagnosticRules.UseAnonymousFunctionOrMethodGroup)
                .AddConfigOption(LegacyConfigOptions.ConvertMethodGroupToAnonymousFunction.Key, "true"));
        }
    }
}
