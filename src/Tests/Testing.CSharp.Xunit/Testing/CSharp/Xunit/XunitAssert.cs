// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Testing.CSharp.Xunit
{
    internal class XunitAssert : IAssert
    {
        public static XunitAssert Instance { get; } = new();

        public void Equal(string expected, string actual)
        {
            global::Xunit.Assert.Equal(expected, actual);
        }

        public void True(bool condition, string userMessage)
        {
            global::Xunit.Assert.True(condition, userMessage);
        }
    }
}
