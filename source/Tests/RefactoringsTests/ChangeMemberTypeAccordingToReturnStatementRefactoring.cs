// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ChangeMemberTypeTypeAccordingToReturnStatementRefactoring
    {
        public void GetValue()
        {
            return 0;
        }

        public void SomeMethod2() => 0;

        private async void MethodAsync()
        {
            return await Task.FromResult(false);
            return Task.FromResult(false);
        }

        private async Task Method2Async()
        {
            return await Task.FromResult(false);
            return Task.FromResult(false);
        }

        private async Task<bool> Method3Async()
        {
            return await Task.FromResult(new object());
            return Task.FromResult(new object());
            return Task.Run(null);

            return false;
        }

        public void GetValues()
        {
            return Enumerable.Range(0, 0).OrderByDescending(f => f);
        }

        public IEnumerable<int> GetValues2()
        {
            return Enumerable.Range(0, 0).OrderByDescending(f => f);
        }
    }
}
