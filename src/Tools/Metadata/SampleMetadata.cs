// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace Roslynator.Metadata
{
    [SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "<Pending>")]
    public readonly struct SampleMetadata
    {
        public SampleMetadata(string before, string after)
        {
            Before = before;

            After = after;
        }

        public string Before { get; }

        public string After { get; }
    }
}
