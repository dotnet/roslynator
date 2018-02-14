// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1016, RCS1058, RCS1118, RCS1163, RCS1176

using System;
using System.Text.RegularExpressions;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class RemoveRedundantToStringCall
    {
        private class Foo
        {
            public void Bar()
            {
                string s = null;

                object o = null;

                int i = 0;

                var options = RegexOptions.None;

                Entity e1 = null;
                EntityWithNew en = null;
                EntityWithOperator eo = null;

                s = $"{this.ToString()}x";

                s = s.ToString();

                s = $"{o.ToString()}{s.ToString()}{e1.ToString()}{eo.ToString()}x";

                s = s + e1.ToString();
                s = s + (e1.ToString());
                s = "" + e1.ToString();
                s = e1.ToString() + s;
                s = (e1.ToString()) + s;
                s = e1.ToString() + "";

                //n

                s = s + i.ToString();

                s = options.ToString() + "";

                s = $"{en.ToString()}x";

                s = s + en.ToString();
                s = s + eo.ToString();

                s = en.ToString() + s;
                s = eo.ToString() + s;

                s = $"{base.ToString()}x";
            }
        }

        private abstract class Entity
        {
            public override string ToString()
            {
                return null;
            }
        }

        private abstract class EntityWithNew
        {
            new public string ToString()
            {
                return "new";
            }
        }

        private abstract class EntityWithOperator
        {
            [Obsolete("message", true)]
            public static string operator +(EntityWithOperator left, string right)
            {
                return "operator";
            }

            [Obsolete("message", true)]
            public static string operator +(string left, EntityWithOperator right)
            {
                return "operator";
            }
        }
    }
}
