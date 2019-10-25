// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Tests;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class AddEmptyLineBeforeUsingDirectiveListTests : AbstractCSharpFixVerifier
    {
        private readonly CodeVerificationOptions _options;

        public AddEmptyLineBeforeUsingDirectiveListTests()
        {
            _options = base.Options.AddAllowedCompilerDiagnosticId("CS0430");
        }

        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddEmptyLineBeforeUsingDirectiveList;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddEmptyLineBeforeUsingDirectiveListAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AddEmptyLineBeforeAndAfterUsingDirectiveListCodeFixProvider();

        public override CodeVerificationOptions Options => _options;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBeforeUsingDirectiveList)]
        public async Task Test_Comment_Before()
        {
            await VerifyDiagnosticAndFixAsync(@"// x
[||]using System;
using System.Linq;

namespace N
{
}
", @"// x

using System;
using System.Linq;

namespace N
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBeforeUsingDirectiveList)]
        public async Task Test_CommentAndExternAlias_Before()
        {
            await VerifyDiagnosticAndFixAsync(@"
extern alias x;
[||]using System;
using System.Linq;

namespace N
{
}
", @"
extern alias x;

using System;
using System.Linq;

namespace N
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBeforeUsingDirectiveList)]
        public async Task Test_Comment_ExternAliasAndComment_Before()
        {
            await VerifyDiagnosticAndFixAsync(@"
extern alias x; // x
[||]using System;
using System.Linq;

namespace N
{
}
", @"
extern alias x; // x

using System;
using System.Linq;

namespace N
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBeforeUsingDirectiveList)]
        public async Task Test_Comment_ExternAliasAndRegionDirective_Before()
        {
            await VerifyDiagnosticAndFixAsync(@"
extern alias x;
[||]#region
using System;
using System.Linq;
#endregion

namespace N
{
}
", @"
extern alias x;

#region
using System;
using System.Linq;
#endregion

namespace N
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBeforeUsingDirectiveList)]
        public async Task Test_Comment_CommentAndRegionDirective_Before()
        {
            await VerifyDiagnosticAndFixAsync(@"// x
[||]#region
using System;
using System.Linq;
#endregion

namespace N
{
}
", @"// x

#region
using System;
using System.Linq;
#endregion

namespace N
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBeforeUsingDirectiveList)]
        public async Task Test_CommentAndPragmaDirective_Before()
        {
            await VerifyDiagnosticAndFixAsync(@"
#pragma warning disable x
[||]using System;
using System.Linq;

namespace N
{
}
", @"
#pragma warning disable x

using System;
using System.Linq;

namespace N
{
}
");
        }
    }
}
