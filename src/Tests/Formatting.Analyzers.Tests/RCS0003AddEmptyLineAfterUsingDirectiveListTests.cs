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
    public class AddEmptyLineAfterUsingDirectiveListTests : AbstractCSharpFixVerifier
    {
        private readonly CodeVerificationOptions _options;

        public AddEmptyLineAfterUsingDirectiveListTests()
        {
            _options = base.Options.AddAllowedCompilerDiagnosticId("CS0430");
        }

        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddEmptyLineAfterUsingDirectiveList;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddEmptyLineAfterUsingDirectiveListAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AddEmptyLineBeforeAndAfterUsingDirectiveListCodeFixProvider();

        public override CodeVerificationOptions Options => _options;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterUsingDirectiveList)]
        public async Task Test_CompilationUnit_Comment_After()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Linq;[||]
// x

namespace N
{
}
", @"
using System;
using System.Linq;

// x

namespace N
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterUsingDirectiveList)]
        public async Task Test_CompilationUnit_DocumentationComment_After()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Linq;[||]
/// <summary></summary>
namespace N
{
}
", @"
using System;
using System.Linq;

/// <summary></summary>
namespace N
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterUsingDirectiveList)]
        public async Task Test_CompilationUnit_AssemblyAttribute_After()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Linq;[||]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(null, null)]

namespace N
{
}
", @"
using System;
using System.Linq;

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(null, null)]

namespace N
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterUsingDirectiveList)]
        public async Task Test_CompilationUnit_CommentAndAssemblyAttribute_After()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Linq; // x[||]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(null, null)]

namespace N
{
}
", @"
using System;
using System.Linq; // x

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(null, null)]

namespace N
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterUsingDirectiveList)]
        public async Task Test_CompilationUnit_CommentAndEndRegionDirective_After()
        {
            await VerifyDiagnosticAndFixAsync(@"
#region
using System;
using System.Linq;
#endregion[||]
namespace N
{
}
", @"
#region
using System;
using System.Linq;
#endregion

namespace N
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterUsingDirectiveList)]
        public async Task Test_CompilationUnit_CommentAndPragmaDirective_After()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Linq;[||]
#pragma warning disable x

namespace N
{
}
", @"
using System;
using System.Linq;

#pragma warning disable x

namespace N
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterUsingDirectiveList)]
        public async Task Test_CompilationUnit_NamespaceDeclaration_After()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Linq;[||]
namespace N
{
}
", @"
using System;
using System.Linq;

namespace N
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterUsingDirectiveList)]
        public async Task Test_NamespaceDeclaration_Comment_After()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    using System;
    using System.Linq;[||]
    // x

    class C
    {
    }
}
", @"
namespace N
{
    using System;
    using System.Linq;

    // x

    class C
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterUsingDirectiveList)]
        public async Task Test_NamespaceDeclaration_DocumentationComment_After()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    using System;
    using System.Linq;[||]
    /// <summary></summary>
    class C
    {
    }
}
", @"
namespace N
{
    using System;
    using System.Linq;

    /// <summary></summary>
    class C
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterUsingDirectiveList)]
        public async Task Test_NamespaceDeclaration_CommentAndEndRegionDirective_After()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    #region
    using System;
    using System.Linq;
    #endregion[||]
    class C
    {
    }
}
", @"
namespace N
{
    #region
    using System;
    using System.Linq;
    #endregion

    class C
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterUsingDirectiveList)]
        public async Task Test_NamespaceDeclaration_CommentAndPragmaDirective_After()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    using System;
    using System.Linq;[||]
    #pragma warning disable x

    class C
    {
    }
}
", @"
namespace N
{
    using System;
    using System.Linq;

    #pragma warning disable x

    class C
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterUsingDirectiveList)]
        public async Task Test_NamespaceDeclaration_ClassDeclaration_After()
        {
            await VerifyDiagnosticAndFixAsync(@"
namespace N
{
    using System;
    using System.Linq;[||]
    class C
    {
    }
}
", @"
namespace N
{
    using System;
    using System.Linq;

    class C
    {
    }
}
");
        }
    }
}
