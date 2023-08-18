// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

namespace Roslynator.Testing.CSharp;

public static class FileSystemHelpersTests
{
    [Fact]
    public static void Test_DetermineRelativePath()
    {
        Assert.Equal("c", FileSystemHelpers.DetermineRelativePath("b/c", "b/x.md"));
        Assert.Equal("c/d", FileSystemHelpers.DetermineRelativePath("b/c/d", "b/x.md"));
        Assert.Equal("../../b", FileSystemHelpers.DetermineRelativePath("b", "b/c/x.md"));
        Assert.Equal("../../../b", FileSystemHelpers.DetermineRelativePath("b", "b/c/d/x.md"));
        Assert.Equal("", FileSystemHelpers.DetermineRelativePath("b/c", "b/c/x.md"));
        Assert.Equal("", FileSystemHelpers.DetermineRelativePath(".", "x.md"));
    }
}
