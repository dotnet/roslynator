// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#pragma warning disable RCS1079

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemoveRedundantCast
    {
        public static class Foo
        {
            public static void Bar(SyntaxNode node, Dictionary<int, string> dic)
            {
                SyntaxNode parent = ((BlockSyntax)node).Parent;

                parent = ((BlockSyntax)node)?.Parent;

                string x = ((IDictionary<int, string>)dic)[0];

                x = ((IDictionary<int, string>)dic)?[0];

                IEnumerable<string> q = Enumerable.Empty<string>()
                    .AsEnumerable()
                    .Cast<string>();

                ((IDisposable)default(FooDisposable)).Dispose();

                //n

                SyntaxToken openBrace = ((BlockSyntax)node).OpenBraceToken;
                Location location = ((BlockSyntax)node).GetLocation();

                var q2 = ((IEnumerable<string>)new EnumerableOfString()).GetEnumerator();
                var q3 = ((IEnumerable<string>)new EnumerableOfString2()).GetEnumerator();

                IEnumerableOfString i = null;
                var q4 = ((IEnumerable<string>)i).GetEnumerator();

                ((IDisposable)default(FooExplicitDisposable)).Dispose();
            }
        }

        private interface IEnumerableOfString : IEnumerable<string>
        {
        }

        private class EnumerableOfString : IEnumerable<string>
        {
            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private class EnumerableOfString2 : EnumerableOfString
        {
        }

        private class FooDisposable : IDisposable
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        private class FooExplicitDisposable : IDisposable
        {
            void IDisposable.Dispose()
            {
                throw new NotImplementedException();
            }
        }
    }
}
