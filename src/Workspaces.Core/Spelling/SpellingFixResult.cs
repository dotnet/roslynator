// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Spelling
{
    internal sealed class SpellingFixResult
    {
        public SpellingFixResult(
            string oldValue,
            string newValue,
            FileLinePositionSpan lineSpan,
            SpellingFixKind kind) : this(oldValue, newValue, lineSpan, kind, null, null, -1)
        {
        }

        public SpellingFixResult(
            string oldValue,
            string newValue,
            FileLinePositionSpan lineSpan,
            SpellingFixKind kind,
            string oldIdentifier,
            string newIdentifier,
            int valueIndex)
        {
            OldValue = oldValue;
            NewValue = newValue;
            LineSpan = lineSpan;
            Kind = kind;
            OldIdentifier = oldIdentifier;
            NewIdentifier = newIdentifier;
            ValueIndex = valueIndex;
        }

        public string OldValue { get; }

        public string NewValue { get; }

        public FileLinePositionSpan LineSpan { get; }

        public SpellingFixKind Kind { get; }

        public string OldIdentifier { get; }

        public string NewIdentifier { get; }

        public int ValueIndex { get; }

        public bool Success => NewValue != null;

        public bool IsSymbol => OldIdentifier != null;
    }
}
