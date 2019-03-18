// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;

namespace Roslynator.CodeFixes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class CodeFixDescriptor
    {
        public CodeFixDescriptor(
            string id,
            string title,
            bool isEnabledByDefault,
            params string[] fixableDiagnosticIds)
        {
            Id = id;
            Title = title;
            IsEnabledByDefault = isEnabledByDefault;
            FixableDiagnosticIds = fixableDiagnosticIds?.ToImmutableArray() ?? ImmutableArray<string>.Empty;
        }

        public string Id { get; }

        public string Title { get; }

        public bool IsEnabledByDefault { get; }

        public ImmutableArray<string> FixableDiagnosticIds { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Id} ({string.Join(", ", FixableDiagnosticIds)})";
    }
}
