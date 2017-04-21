// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class RemoveRedundantBaseInterface
    {
        private class Foo : List<string>
        {
        }

        private class Foo2 : List<string>, IEnumerable<string>
        {
        }

        private class Foo3 : IList<string>, IEnumerable<string>
        {
        }

        private interface Foo4 : IEnumerable<string>, IList<string>
        {
        }

        private struct Foo5 : IEnumerable<string>, IList<string>
        {
        }

        private class Foo6 : List<string>, ICollection<string>, IEnumerable<string>
        {
        }

        private class Foo7 : ICollection<string>, List<string>, IEnumerable<string>
        {
        }

        private class Foo7 : ICollection<string>, IList<string>, IEnumerable<string>
        {
        }
    }
}
