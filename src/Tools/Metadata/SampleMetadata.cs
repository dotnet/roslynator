// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace Roslynator.Metadata
{
    public readonly struct SampleMetadata
    {
        public SampleMetadata(string before, string after)
        {
            Before = before;
            After = after;
        }

        public string Before { get; }

        public string After { get; }

        public SampleMetadata WithBefore(string before)
        {
            return new SampleMetadata(
                before: before,
                after: After);
        }
    }
}
