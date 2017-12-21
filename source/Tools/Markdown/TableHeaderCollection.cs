// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Roslynator.Markdown
{
    public class TableHeaderCollection : Collection<TableHeader>
    {
        internal TableHeaderCollection()
        {
        }

        internal TableHeaderCollection(IList<TableHeader> list)
            : base(list)
        {
        }

        internal TableHeaderCollection(IEnumerable<TableHeader> headers)
        {
            AddRange(headers);
        }

        public void AddRange(IEnumerable<TableHeader> headers)
        {
            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            foreach (TableHeader header in headers)
                Add(header);
        }
    }
}