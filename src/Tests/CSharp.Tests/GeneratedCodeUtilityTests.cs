// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;
using static Roslynator.GeneratedCodeUtility;

namespace Roslynator.CSharp.Tests
{
    public static class GeneratedCodeUtilityTests
    {
        [Fact]
        public static void TestIsGeneratedCodeFile()
        {
            Assert.True(IsGeneratedCodeFile("TemporaryGeneratedFile_"));
            Assert.True(IsGeneratedCodeFile("TemporaryGeneratedFile_Foo"));
            Assert.True(IsGeneratedCodeFile("TemporaryGeneratedFile_.cs"));
            Assert.True(IsGeneratedCodeFile("TemporaryGeneratedFile_Foo.cs"));

            Assert.True(IsGeneratedCodeFile(@"c:\TemporaryGeneratedFile_.cs"));
            Assert.True(IsGeneratedCodeFile(@"c:\TemporaryGeneratedFile_Foo.cs"));
            Assert.True(IsGeneratedCodeFile("c:TemporaryGeneratedFile_.cs"));
            Assert.True(IsGeneratedCodeFile("c:TemporaryGeneratedFile_Foo.cs"));

            Assert.True(IsGeneratedCodeFile(".designer.cs"));
            Assert.True(IsGeneratedCodeFile(".generated.cs"));
            Assert.True(IsGeneratedCodeFile(".g.cs"));
            Assert.True(IsGeneratedCodeFile(".g.i.cs"));
            Assert.True(IsGeneratedCodeFile(".AssemblyAttributes.cs"));

            Assert.True(IsGeneratedCodeFile("Foo.designer.cs"));
            Assert.True(IsGeneratedCodeFile("Foo.generated.cs"));
            Assert.True(IsGeneratedCodeFile("Foo.g.cs"));
            Assert.True(IsGeneratedCodeFile("Foo.g.i.cs"));
            Assert.True(IsGeneratedCodeFile("Foo.AssemblyAttributes.cs"));

            Assert.True(IsGeneratedCodeFile(@"c:\.designer.cs"));
            Assert.True(IsGeneratedCodeFile(@"c:\.generated.cs"));
            Assert.True(IsGeneratedCodeFile(@"c:\.g.cs"));
            Assert.True(IsGeneratedCodeFile(@"c:\.g.i.cs"));
            Assert.True(IsGeneratedCodeFile(@"c:\.AssemblyAttributes.cs"));

            Assert.True(IsGeneratedCodeFile("c:.designer.cs"));
            Assert.True(IsGeneratedCodeFile("c:.generated.cs"));
            Assert.True(IsGeneratedCodeFile("c:.g.cs"));
            Assert.True(IsGeneratedCodeFile("c:.g.i.cs"));
            Assert.True(IsGeneratedCodeFile("c:.AssemblyAttributes.cs"));

            Assert.True(IsGeneratedCodeFile(@"c:\Foo.designer.cs"));
            Assert.True(IsGeneratedCodeFile(@"c:\Foo.generated.cs"));
            Assert.True(IsGeneratedCodeFile(@"c:\Foo.g.cs"));
            Assert.True(IsGeneratedCodeFile(@"c:\Foo.g.i.cs"));
            Assert.True(IsGeneratedCodeFile(@"c:\Foo.AssemblyAttributes.cs"));

            Assert.True(IsGeneratedCodeFile("c:Foo.designer.cs"));
            Assert.True(IsGeneratedCodeFile("c:Foo.generated.cs"));
            Assert.True(IsGeneratedCodeFile("c:Foo.g.cs"));
            Assert.True(IsGeneratedCodeFile("c:Foo.g.i.cs"));
            Assert.True(IsGeneratedCodeFile("c:Foo.AssemblyAttributes.cs"));
        }

        [Fact]
        public static void TestIsNotGeneratedCodeFile()
        {
            Assert.False(IsGeneratedCodeFile(null));
            Assert.False(IsGeneratedCodeFile(""));
            Assert.False(IsGeneratedCodeFile(" "));
            Assert.False(IsGeneratedCodeFile("."));
            Assert.False(IsGeneratedCodeFile(@"\"));
            Assert.False(IsGeneratedCodeFile("foo"));
            Assert.False(IsGeneratedCodeFile("foo."));
            Assert.False(IsGeneratedCodeFile(@"foo\"));
            Assert.False(IsGeneratedCodeFile(@"foo\."));
            Assert.False(IsGeneratedCodeFile(@"c:\foo"));
            Assert.False(IsGeneratedCodeFile(@"c:\foo\"));
            Assert.False(IsGeneratedCodeFile("c:foo"));
            Assert.False(IsGeneratedCodeFile(@"c:foo\"));

            Assert.False(IsGeneratedCodeFile("Foo.designer"));
            Assert.False(IsGeneratedCodeFile("Foo.generated"));
            Assert.False(IsGeneratedCodeFile("Foo.g"));
            Assert.False(IsGeneratedCodeFile("Foo.AssemblyAttributes"));
        }
    }
}
