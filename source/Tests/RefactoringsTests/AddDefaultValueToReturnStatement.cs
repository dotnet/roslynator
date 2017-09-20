// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class AddDefaultValueToReturnStatement
    {
        public DateTime GetDateTime()
        {
            return;
        }

        public string GetValue()
        {
            return;
        }

        public string GetValue2()
        {
            var items = new List<string>();

            var q = items.Select<string, bool>(f =>
            {
                return;
            });

            q = items.Select<string, bool>(delegate
            {
                return;
            });
        }

        public async Task<bool> GetValue3()
        {
            var x = await GetValue3();

            return;
        }

        public string PropertyName
        {
            get { return; }
        }

        public DateTime this[int value]
        {
            get { return; }
        }

        public static explicit operator DateTime(AddDefaultValueToReturnStatement x)
        {
            return;
        }
    }
}
