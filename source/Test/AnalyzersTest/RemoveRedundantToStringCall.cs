// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1016, RCS1058

using System;
using System.Text.RegularExpressions;

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class RemoveRedundantToStringCall
    {
        public static void Bar()
        {
            string s = null;

            object o = null;

            int i = 0;

            var options = RegexOptions.None;

            Entity e1 = null;
            EntityWithNew en = null;
            EntityWithOperator eo = null;

            s = s.ToString();

            s = $"{o.ToString()}{s.ToString()}{e1.ToString()}{eo.ToString()}";

            s = s + e1.ToString();
            s = s + (e1.ToString());
            s = "" + e1.ToString();
            s = e1.ToString() + s;
            s = (e1.ToString()) + s;
            s = e1.ToString() + "";

            //n

            s = s + i.ToString();

            s = options.ToString() + "";

            s = $"{en.ToString()}";

            s = s + en.ToString();
            s = s + eo.ToString();

            s = en.ToString() + s;
            s = eo.ToString() + s;
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
