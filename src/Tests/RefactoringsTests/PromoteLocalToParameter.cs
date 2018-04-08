// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class PromoteLocalToParameter
    {
        public static object GetValue()
        {
            var value = new object();

            return value;
        }

        public static void MethodName(object parameter)
        {
            var value = "";
            string value2 = "", value22 = "";
        }

        public static void MethodName2(object parameter)
        {
            string value;
            string value2, value22;
        }

        public static void MethodName3(object parameter)
        {
            var q = Enumerable.Range(1, 1).Select(f => new { Value = f });
            var value = q;
        }
    }
}
