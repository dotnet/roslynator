// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

#pragma warning disable CA1813

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UseAttributeUsageAttribute
    {
        private class FooAttribute : Attribute
        {
        }

        //n

        [AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = false)]
        private class Foo2Attribute : Attribute
        {
        }

        private class Foo3Attribute : Foo2Attribute
        {
        }

        private class Foo4Attribute
        {
        }

        private class Foo : Attribute
        {
        }
    }
}
