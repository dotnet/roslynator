// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseCastMethodInsteadOfSelectMethod
    {
        private static void Foo()
        {
            var items = new List<string>();

            IEnumerable<object> q = items.Select(f => (object)f);

            q = items.Select((f) => (object)f);

            q = items.Select(f =>
            {
                return (object)f;
            });
        }
    }
}
