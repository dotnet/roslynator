// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roslynator.Metadata
{
    public class SourceFile
    {
        public SourceFile(
            string id,
            IEnumerable<string> paths)
        {
            Id = id;
            Paths = new ReadOnlyCollection<string>(paths?.ToArray() ?? Array.Empty<string>());
        }

        public string Id { get; }

        public IReadOnlyList<string> Paths { get; }
    }
}
