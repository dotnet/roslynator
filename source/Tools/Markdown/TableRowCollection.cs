// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Roslynator.Markdown
{
    public class TableRowCollection : Collection<TableRow>
    {
        internal TableRowCollection()
        {
        }

        internal TableRowCollection(IList<TableRow> list)
            : base(list)
        {
        }

        internal TableRowCollection(IEnumerable<TableRow> rows)
        {
            AddRange(rows);
        }

        public void AddRange(IEnumerable<TableRow> rows)
        {
            if (rows == null)
                throw new ArgumentNullException(nameof(rows));

            foreach (TableRow row in rows)
                Add(row);
        }
    }
}