// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

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

            q = items.Select<string, bool>((f) =>
            {
                return;
            });
        }

        public async Task<bool> GetValue3()
        {
            var x = await GetValue4();

            return;
        }

        public Task<bool> GetValue4()
        {
            return Task.FromResult(false);
        }

        public string PropertyName
        {
            get { return; }
        }

        public string this[int value]
        {
            get { return; }
        }
    }
}
