// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Roslynator.Spelling;

internal partial class Spellchecker
{
    private static readonly Regex _splitIdentifierRegex = new(
        @"\P{L}+|" + _splitCasePattern,
        RegexOptions.IgnorePatternWhitespace);

    internal ImmutableArray<SpellingMatch> AnalyzeIdentifier(string value, int prefixLength = 0)
    {
        if (!IsAllowedLength(value.Length))
            return ImmutableArray<SpellingMatch>.Empty;

        if (prefixLength > 0
            && Data.Contains(value))
        {
            return ImmutableArray<SpellingMatch>.Empty;
        }

        ImmutableArray<SpellingMatch>.Builder builder = null;

        Match match = _splitIdentifierRegex.Match(value, prefixLength);

        if (match.Success
            && prefixLength > 0
            && match.Index == prefixLength)
        {
            match = match.NextMatch();
        }

        if (!match.Success)
        {
            if (prefixLength > 0)
            {
                AnalyzeIdentifierValue(value.Substring(prefixLength), prefixLength, value, ref builder);
            }
            else
            {
                AnalyzeIdentifierValue(value, 0, null, ref builder);
            }
        }
        else if (prefixLength > 0
            || !Data.Contains(value))
        {
            int prevEnd = prefixLength;

            do
            {
                AnalyzeIdentifierValue(value.Substring(prevEnd, match.Index - prevEnd), prevEnd, value, ref builder);

                prevEnd = match.Index + match.Length;

                match = match.NextMatch();
            }
            while (match.Success);

            AnalyzeIdentifierValue(value.Substring(prevEnd), prevEnd, value, ref builder);
        }

        return builder?.ToImmutableArray() ?? ImmutableArray<SpellingMatch>.Empty;
    }

    private void AnalyzeIdentifierValue(
        string value,
        int valueIndex,
        string parentValue,
        ref ImmutableArray<SpellingMatch>.Builder builder)
    {
        if (IsMatch(value))
        {
            var spellingMatch = new SpellingMatch(value, valueIndex, parentValue);
            (builder ??= ImmutableArray.CreateBuilder<SpellingMatch>()).Add(spellingMatch);
        }
    }
}
