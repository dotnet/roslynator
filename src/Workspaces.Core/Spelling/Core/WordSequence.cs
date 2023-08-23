// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Roslynator.Spelling;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly struct WordSequence
{
    public WordSequence(IEnumerable<string> words)
    {
        if (words is null)
            throw new ArgumentNullException(nameof(words));

        Words = words.ToImmutableArray();
    }

    public string First => Words[0];

    public int Count => Words.Length;

    public ImmutableArray<string> Words { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => (Words.IsDefault) ? "Uninitialized" : $"Count = {Count}  {ToString()}";

    public override string ToString()
    {
        return (Words.IsDefault) ? "" : string.Join(" ", Words);
    }
}
