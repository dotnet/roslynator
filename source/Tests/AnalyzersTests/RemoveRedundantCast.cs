// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#pragma warning disable RCS1008, RCS1016, RCS1019, RCS1079, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemoveRedundantCast
    {
        private class Foo : BaseFoo
        {
            public static void Bar(SyntaxNode node, Dictionary<int, string> dic)
            {
                SyntaxNode parent = ((BlockSyntax)node).Parent;

                parent = ((BlockSyntax)node)?.Parent;

                string s = ((IDictionary<int, string>)dic)[0];

                s = ((IDictionary<int, string>)dic)?[0];

                IEnumerable<string> items = Enumerable.Empty<string>()
                    .AsEnumerable()
                    .Cast<string>();

                ((IDisposable)new FooDisposable()).Dispose();

                ((Foo)new BaseFoo()).ProtectedInternal();
            }
        }

        private class BaseFoo
        {
            private void Bar(BaseFoo baseFoo)
            {
                ((DerivedFoo)baseFoo).Protected();

                ((DerivedFoo)baseFoo).PrivateProtected();

                ((DerivedFoo)baseFoo).ProtectedInternal();
            }

            private class DerivedFoo : BaseFoo
            {
                private void Bar2(BaseFoo baseFoo)
                {
                    ((DerivedFoo)baseFoo).Protected();

                    ((DerivedFoo)baseFoo).PrivateProtected();

                    ((DerivedFoo)baseFoo).ProtectedInternal();
                }
            }

            protected void Protected() { }

            private protected void PrivateProtected() { }

            protected internal void ProtectedInternal() { }
        }

        //n

        private class Foo2 : BaseFoo
        {
            public static void Bar(SyntaxNode node)
            {
                SyntaxToken openBrace = ((BlockSyntax)node).OpenBraceToken;
                Location location = ((BlockSyntax)node).GetLocation();

                var q2 = ((IEnumerable<string>)new EnumerableOfString()).GetEnumerator();
                var q3 = ((IEnumerable<string>)new DerivedEnumerableOfString()).GetEnumerator();

                IEnumerableOfString i = null;
                var q4 = ((IEnumerable<string>)i).GetEnumerator();

                ((IDisposable)default(FooExplicitDisposable)).Dispose();
            }
        }

        private class Foo3 : BaseFoo
        {
            private void Bar(BaseFoo baseFoo)
            {
                ((Foo3)baseFoo).Protected();

                ((Foo3)baseFoo).PrivateProtected();
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

        private class DerivedEnumerableOfString : EnumerableOfString
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
