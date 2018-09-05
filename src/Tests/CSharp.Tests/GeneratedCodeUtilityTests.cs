// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

namespace Roslynator.CSharp.Tests
{
    public static class GeneratedCodeUtilityTests
    {
        [Fact]
        public static void TestIsGeneratedCodeFile()
        {
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile("TemporaryGeneratedFile_"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile("TemporaryGeneratedFile_Foo"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile("TemporaryGeneratedFile_.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile("TemporaryGeneratedFile_Foo.cs"));

            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\TemporaryGeneratedFile_.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\TemporaryGeneratedFile_Foo.cs"));

            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(".designer.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(".generated.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(".g.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(".g.i.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(".AssemblyAttributes.cs"));

            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile("Foo.designer.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile("Foo.generated.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile("Foo.g.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile("Foo.g.i.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile("Foo.AssemblyAttributes.cs"));

            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\.designer.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\.generated.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\.g.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\.g.i.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\.AssemblyAttributes.cs"));

            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\Foo.designer.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\Foo.generated.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\Foo.g.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\Foo.g.i.cs"));
            Assert.True(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\Foo.AssemblyAttributes.cs"));
        }

        [Fact]
        public static void TestIsNotGeneratedCodeFile()
        {
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile(null));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile(""));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile(" "));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile("."));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile(@"\"));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile("foo"));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile("foo."));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile(@"foo\"));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile(@"foo\."));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\foo"));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile(@"c:\foo\"));

            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile("Foo.designer"));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile("Foo.generated"));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile("Foo.g"));
            Assert.False(GeneratedCodeUtility.IsGeneratedCodeFile("Foo.AssemblyAttributes"));
        }
    }
}
