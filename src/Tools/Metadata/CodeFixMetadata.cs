// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Roslynator.Metadata
{
    public class CodeFixMetadata
    {
        public CodeFixMetadata(
            string id,
            string identifier,
            string title,
            bool isEnabledByDefault,
            bool isObsolete,
            IEnumerable<string> fixableDiagnosticIds)
        {
            Id = id;
            Identifier = identifier;
            Title = title;
            IsEnabledByDefault = isEnabledByDefault;
            IsObsolete = isObsolete;
            FixableDiagnosticIds = new ReadOnlyCollection<string>(fixableDiagnosticIds?.ToArray() ?? Array.Empty<string>());
        }

        public string Id { get; }

        public string Identifier { get; }

        public string Title { get; }

        public bool IsEnabledByDefault { get; }

        public bool IsObsolete { get; }

        public IReadOnlyList<string> FixableDiagnosticIds { get; }
    }
}
