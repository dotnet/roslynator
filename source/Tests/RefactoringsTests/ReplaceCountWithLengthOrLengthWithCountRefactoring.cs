// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ReplaceCountWithLengthOrLengthWithCountRefactoring
    {
        public void Do()
        {
            var list = new List<object>();

            var count = list.Length;

            count = Items.Length;
            count = this.GetList().Length;

            var array = new string[0];
            var length = array.Count;
        }

        private List<object> GetList()
        {
            return null;
        }

        public Collection<object> Items { get; } = new Collection<object>();
    }
}
