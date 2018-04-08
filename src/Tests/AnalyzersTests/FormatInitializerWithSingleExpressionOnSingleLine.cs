// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

#pragma warning disable RCS1016, RCS1067, RCS1095, RCS1163

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class FormatInitializerWithSingleExpressionOnSingleLine
    {
        private class Entity
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        public static void Foo()
        {
            var entity = new Entity()
            {
                Name = "Name"
            };

            var dic = new Dictionary<int, string>()
            {
                { 0, "0" }
            };

            var dic2 = new Dictionary<int, string>()
            {
                [0] = "0"
            };

            var entities = new Entity[]
            {
                new Entity()
                {
                    Name = new string('a', 1)
                }
            };

            var items = new List<string>()
            {
                { null }
            };

            // n

            var entity2 = new Entity()
            {
                Name = "Name",
                Value = 0
            };

            var entity3 = new Entity()
            {
                Name =
                    "Name"
            };

            var entity4 = new Entity()
            {
                // abc
                Name = "Name"
            };
        }
    }
}
