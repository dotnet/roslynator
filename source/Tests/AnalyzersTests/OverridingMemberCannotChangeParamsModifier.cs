// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class OverridingMemberCannotChangeParamsModifier
    {
        private class Derived : Base
        {
            public override void FooWithParams(object[] values)
            {
            }

            public override void FooWithoutParams(params object[] values)
            {
            }

            public override string this[bool withParams, string[] values] => base[withParams, values];

            public override string this[int withoutParams, params string[] values] => base[withoutParams, values];
        }

        private class Base
        {
            public virtual void FooWithParams(params object[] values)
            {

            }

            public virtual void FooWithoutParams(object[] values)
            {

            }

            public virtual string this[bool withParams, params string[] values]
            {
                get { return ""; }
            }

            public virtual string this[int withoutParams, string[] values]
            {
                get { return ""; }
            }
        }
    }
}
