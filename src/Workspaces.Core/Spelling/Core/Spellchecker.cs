// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Roslynator.Spelling;

#pragma warning disable RCS1043

internal partial class Spellchecker
{
    private const string _splitCasePattern = @"
    (?<=
        \p{Lu}
    )
    (?=
        \p{Lu}\p{Ll}
    )
|
    (?<=
        \p{Ll}
    )
    (?=
        \p{Lu}
    )
";

    private static readonly Regex _urlRegex = new(
        @"\bhttps?://[^\s]+(?=\s|\z)", RegexOptions.IgnoreCase);

    private readonly Regex _splitRegex = new("-|" + _splitCasePattern, RegexOptions.IgnorePatternWhitespace);

    public SpellingData Data { get; }

    public SpellcheckerOptions Options { get; }

    internal static Regex WordRegex { get; } = new(
        @"
\b
\p{L}{2,}
(-\p{L}{2,})*
\p{L}*
(
    (?='s\b)
|
    ('(s|d|ll|m|re|t|ve)\b)
|
    ('(?!\p{L})\b)
|
    \b
)",
        RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);

    public Spellchecker(
        SpellingData data,
        SpellcheckerOptions options = null)
    {
        Data = data;
        Options = options ?? SpellcheckerOptions.Default;
    }

    public ImmutableArray<SpellingMatch> AnalyzeText(string value)
    {
        ImmutableArray<SpellingMatch>.Builder builder = null;

        int prevEnd = 0;

        Match match = _urlRegex.Match(value, prevEnd);

        while (match.Success)
        {
            AnalyzeTextSegment(value, prevEnd, match.Index - prevEnd, ref builder);

            prevEnd = match.Index + match.Length;

            match = match.NextMatch();
        }

        AnalyzeTextSegment(value, prevEnd, value.Length - prevEnd, ref builder);

        return builder?.ToImmutableArray() ?? ImmutableArray<SpellingMatch>.Empty;
    }

    private void AnalyzeTextSegment(
        string value,
        int startIndex,
        int length,
        ref ImmutableArray<SpellingMatch>.Builder builder)
    {
        int sequenceEndIndex = -1;

        for (
            Match match = WordRegex.Match(value, startIndex, length);
            match.Success;
            match = match.NextMatch())
        {
            if (sequenceEndIndex >= 0)
            {
                if (match.Index <= sequenceEndIndex)
                {
                    continue;
                }
                else
                {
                    sequenceEndIndex = -1;
                }
            }

            WordSequenceMatch sequenceMatch = Data.GetSequenceMatch(value, startIndex, length, match);

            if (!sequenceMatch.Sequence.Words.IsDefault)
            {
                sequenceEndIndex = sequenceMatch.EndIndex;
                continue;
            }

            if (!IsAllowedLength(match.Length))
                continue;

            AnalyzeWord(value, match.Value, match.Index, ref builder);
        }
    }

    private void AnalyzeWord(
        string input,
        string value,
        int index,
        ref ImmutableArray<SpellingMatch>.Builder builder)
    {
        Match match = _splitRegex.Match(value);

        if (!match.Success)
        {
            AnalyzeWordPart(input, value, index, null, 0, ref builder);
        }
        else if (!Data.Contains(value))
        {
            int lastEnd = 0;

            do
            {
                AnalyzeWordPart(input, value.Substring(lastEnd, match.Index - lastEnd), lastEnd + index, value, index, ref builder);

                lastEnd = match.Index + match.Length;
                match = match.NextMatch();
            }
            while (match.Success);

            AnalyzeWordPart(input, value.Substring(lastEnd), lastEnd + index, value, index, ref builder);
        }
    }

    private void AnalyzeWordPart(
        string input,
        string value,
        int index,
        string parentValue,
        int parentIndex,
        ref ImmutableArray<SpellingMatch>.Builder builder)
    {
        if (IsMatch(value)
            && !IsContainedInNonWord(input, value, index, Data.WordList)
            && !IsContainedInNonWord(input, value, index, Data.CaseSensitiveWordList))
        {
            var spellingMatch = new SpellingMatch(value, index, parentValue, parentIndex);

            (builder ??= ImmutableArray.CreateBuilder<SpellingMatch>()).Add(spellingMatch);
        }

        static bool IsContainedInNonWord(string input, string value, int index, WordList wordList)
        {
            foreach (string nonWord in wordList.NonWords)
            {
                int i = nonWord.IndexOf(value, wordList.Comparison);

                if (i >= 0
                    && index >= i
                    && index + nonWord.Length - i <= input.Length
                    && string.Compare(input, index - i, nonWord, 0, nonWord.Length, wordList.Comparison) == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private bool IsMatch(string value)
    {
        if (!IsAllowedLength(value.Length))
            return false;

        if (IsAllowedNonsensicalWord(value))
            return false;

        if (Data.Contains(value))
            return false;

        return true;
    }

    private bool IsAllowedLength(int value)
    {
        return value >= Options.MinWordLength
            && value <= Options.MaxWordLength;
    }

    private static bool IsAllowedNonsensicalWord(string value)
    {
        if (value.Length < 3)
            return false;

        switch (value)
        {
            case "xyz":
            case "Xyz":
            case "XYZ":
            case "asdfgh":
            case "Asdfgh":
            case "ASDFGH":
            case "qwerty":
            case "Qwerty":
            case "QWERTY":
            case "qwertz":
            case "Qwertz":
            case "QWERTZ":
                return true;
        }

        if (IsAbcSequence())
            return true;

        if (IsAaaSequence())
            return true;

        if (IsAaaBbbCccSequence())
            return true;

        return false;

        bool IsAbcSequence()
        {
            int num = 0;

            if (value[0] == 'a')
            {
                if (value[1] == 'b')
                {
                    num = 'c';
                }
                else
                {
                    return false;
                }
            }
            else if (value[0] == 'A')
            {
                if (value[1] == 'B')
                {
                    num = 'C';
                }
                else if (value[1] == 'b')
                {
                    num = 'c';
                }
                else
                {
                    return false;
                }
            }

            for (int i = 2; i < value.Length; i++)
            {
                if (value[i] != num)
                    return false;

                num++;
            }

            return true;
        }

        bool IsAaaSequence()
        {
            char ch = value[0];
            int i = 1;

            if (ch >= 65
                && ch <= 90
                && value[1] == ch + 32)
            {
                ch = (char)(ch + 32);
                i++;
            }

            while (i < value.Length)
            {
                if (value[i] != ch)
                    return false;

                i++;
            }

            return true;
        }

        // aabbcc
        bool IsAaaBbbCccSequence()
        {
            char ch = value[0];
            int i = 1;

            while (i < value.Length
                && value[i] == ch)
            {
                i++;
            }

            if (i > 1
                && (ch == 'a' || ch == 'A')
                && value.Length >= 6
                && value.Length % i == 0)
            {
                int length = i;
                int count = value.Length / i;

                for (int j = 0; j < count - 1; j++)
                {
                    var ch2 = (char)(ch + j + 1);

                    int start = i + (j * length);
                    int end = start + length;

                    for (int k = i + (j * length); k < end; k++)
                    {
                        if (ch2 != value[k])
                            return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
