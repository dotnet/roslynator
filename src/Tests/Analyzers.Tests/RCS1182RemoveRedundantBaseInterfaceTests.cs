// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Tests;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1182RemoveRedundantBaseInterfaceTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveRedundantBaseInterface;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveRedundantBaseInterfaceAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new BaseTypeCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantBaseInterface)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

class C : List<object>, ICollection<object>
{
    IEnumerator<object> IEnumerable<object>.GetEnumerator()
    {
        return null;
    }
}
");
        }
    }
}
