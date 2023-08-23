// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Roslynator.Spelling;

internal class Spellchecker
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

    private static readonly Regex _splitIdentifierRegex = new(
        @"\P{L}+|" + _splitCasePattern,
        RegexOptions.IgnorePatternWhitespace);

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
        var context = new SpellingContext(value);

        int prevEnd = 0;

        Match match = _urlRegex.Match(value, prevEnd);

        while (match.Success)
        {
            AnalyzeText(value, prevEnd, match.Index - prevEnd, ref context);

            prevEnd = match.Index + match.Length;

            match = match.NextMatch();
        }

        AnalyzeText(value, prevEnd, value.Length - prevEnd, ref context);

        return context.Builder?.ToImmutableArray() ?? ImmutableArray<SpellingMatch>.Empty;
    }

    private void AnalyzeText(
        string value,
        int startIndex,
        int length,
        ref SpellingContext context)
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

            if (IsAllowedLength(match.Length))
            {
                AnalyzeSplit(_splitRegex, match.Value, match.Index, 0, ref context);
            }
        }
    }

    internal ImmutableArray<SpellingMatch> AnalyzeIdentifier(
        string value,
        int prefixLength = 0)
    {
        if (!IsAllowedLength(value.Length))
            return ImmutableArray<SpellingMatch>.Empty;

        if (prefixLength > 0
            && Data.Contains(value))
        {
            return ImmutableArray<SpellingMatch>.Empty;
        }

        var context = new SpellingContext(value);

        AnalyzeSplit(_splitIdentifierRegex, value, 0, prefixLength, ref context);

        return context.Builder?.ToImmutableArray() ?? ImmutableArray<SpellingMatch>.Empty;
    }

    private void AnalyzeSplit(
        Regex regex,
        string input,
        int offset,
        int prefixLength,
        ref SpellingContext context)
    {
        Match match = regex.Match(input, prefixLength);

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
                AnalyzeValue(input.Substring(prefixLength), offset + prefixLength, input, offset, ref context);
            }
            else
            {
                AnalyzeValue(input, offset, null, 0, ref context);
            }
        }
        else if (!Data.Contains(input))
        {
            int prevIndex = prefixLength;

            do
            {
                AnalyzeValue(input.Substring(prevIndex, match.Index - prevIndex), prevIndex + offset, input, offset, ref context);

                prevIndex = match.Index + match.Length;

                match = match.NextMatch();
            }
            while (match.Success);

            AnalyzeValue(input.Substring(prevIndex), prevIndex + offset, input, offset, ref context);
        }
    }

    private void AnalyzeValue(
        string value,
        int valueIndex,
        string containingValue,
        int containingValueIndex,
        ref SpellingContext context)
    {
        if (IsMatch(value)
            && !IsContainedInNonWord(value, valueIndex, Data.WordList, ref context)
            && !IsContainedInNonWord(value, valueIndex, Data.CaseSensitiveWordList, ref context))
        {
            var spellingMatch = new SpellingMatch(value, valueIndex, containingValue, containingValueIndex);

            (context.Builder ??= ImmutableArray.CreateBuilder<SpellingMatch>()).Add(spellingMatch);
        }
    }

    private static bool IsContainedInNonWord(string value, int index, WordList wordList, ref SpellingContext context)
    {
        foreach (string nonWord in wordList.NonWords)
        {
            int i = nonWord.IndexOf(value, wordList.Comparison);

            if (i >= 0
                && index >= i
                && index + nonWord.Length - i <= context.Value.Length
                && string.Compare(context.Value, index - i, nonWord, 0, nonWord.Length, wordList.Comparison) == 0)
            {
                return true;
            }
        }

        return false;
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

    private class SpellingContext
    {
        public SpellingContext(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public ImmutableArray<SpellingMatch>.Builder Builder { get; set; }
    }
}
