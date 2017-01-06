// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemoveRedundantParentheses
    {
        private static readonly object _lockObject = new object();

        [Obsolete((""))]
        public static bool MethodName(string value)
        {
            bool f = /*1*/(/*2*/
                           /*3*/false/*4*/
                                     /*5*/)/*6*/;

            while ((true))
            {
            }

            do
            {
            } while ((true));

            using (((IDisposable)null))
            {
            }

            lock ((_lockObject))
            {
            }

            if ((true))
            {
            }

            switch ((true))
            {
            }

            return (true);

            (MethodName(""));

            MethodName((""));

            var arr = new string[] { (null) };

            var dic = new Dictionary<object, object>() { ([0] = null) };

            dic = new Dictionary<object, object>() { ({ 0, null }) };

            var items = new List<string>() { (null) };

            foreach (string item in (items))
            {
            }

            string s = $"{("")}";

            int i = 0;

            (f) = (false);
            (i) += (0);
            (i) -= (0);
            (i) *= (0);
            (i) /= (0);
            (i) %= (0);
            (f) &= (false);
            (f) ^= (false);
            (f) |= (false);
            (i) <<= (0);
            (i) >>= (0);
        }

        public static string MethodName2() => (null);

        public static IEnumerable<object> MethodName3()
        {
            yield return (null);
        }
    }
}
