// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Roslynator.Spelling;

internal class SpellingData
{
    private WordCharMap _charIndexMap;
    private WordCharMap _reversedCharIndexMap;
    private ImmutableDictionary<string, ImmutableHashSet<string>> _charMap;

    public SpellingData(
        WordList words,
        WordList caseSensitiveWords,
        FixList fixes)
        : this(words, caseSensitiveWords, fixes, ImmutableHashSet.Create<string>(StringComparer.InvariantCulture))
    {
    }

    private SpellingData(
        WordList wordList,
        WordList caseSensitiveWordList,
        FixList fixes,
        ImmutableHashSet<string> ignoredValues)
    {
        WordList = wordList;
        CaseSensitiveWordList = caseSensitiveWordList;
        Fixes = fixes ?? FixList.Empty;
        IgnoredValues = ignoredValues.ToImmutableHashSet(StringComparer.InvariantCulture);
    }

    public WordList WordList { get; }

    public WordList CaseSensitiveWordList { get; }

    public ImmutableHashSet<string> IgnoredValues { get; }

    public FixList Fixes { get; }

    public WordCharMap CharIndexMap
    {
        get
        {
            if (_charIndexMap is null)
                Interlocked.CompareExchange(ref _charIndexMap, WordCharMap.CreateCharIndexMap(WordList), null);

            return _charIndexMap;
        }
    }

    public WordCharMap ReversedCharIndexMap
    {
        get
        {
            if (_reversedCharIndexMap is null)
                Interlocked.CompareExchange(ref _reversedCharIndexMap, WordCharMap.CreateCharIndexMap(WordList, reverse: true), null);

            return _reversedCharIndexMap;
        }
    }

    public ImmutableDictionary<string, ImmutableHashSet<string>> CharMap
    {
        get
        {
            if (_charMap is null)
                Interlocked.CompareExchange(ref _charMap, Create(), null);

            return _charMap;

            ImmutableDictionary<string, ImmutableHashSet<string>> Create()
            {
                return WordList.Words
                    .Select(s =>
                    {
                        char[] arr = s.ToCharArray();

                        Array.Sort(arr, (x, y) => x.CompareTo(y));

                        return (value: s, value2: new string(arr));
                    })
                    .GroupBy(f => f.value, WordList.Comparer)
                    .ToImmutableDictionary(f => f.Key, f => f.Select(f => f.value2).ToImmutableHashSet(WordList.Comparer));
            }
        }
    }

    public bool Contains(string value)
    {
        return IgnoredValues.Contains(value)
            || CaseSensitiveWordList.Contains(value)
            || WordList.Contains(value);
    }

    public WordSequenceMatch GetSequenceMatch(string value, int startIndex, int length, Match match)
    {
        if (CaseSensitiveWordList.Sequences.TryGetValue(match.Value, out ImmutableArray<WordSequence> sequences))
        {
            WordSequenceMatch sequenceMatch = GetSequenceMatch(value, startIndex, length, match, sequences, WordList.Comparison);

            if (!sequenceMatch.Sequence.Words.IsDefault)
                return sequenceMatch;
        }

        if (WordList.Sequences.TryGetValue(match.Value, out sequences))
            return GetSequenceMatch(value, startIndex, length, match, sequences, WordList.Comparison);

        return default;
    }

    private static WordSequenceMatch GetSequenceMatch(
        string value,
        int startIndex,
        int length,
        Match match,
        ImmutableArray<WordSequence> sequences,
        StringComparison comparison)
    {
        List<WordSequence> sequenceList = sequences.ToList();
        int endIndex = startIndex + length;
        int i = match.Index + match.Length;

        WordSequenceMatch sequenceMatch = default;

        int whitespaceCount = 0;

        while (i < endIndex
            && char.IsWhiteSpace(value[i]))
        {
            i++;
            whitespaceCount++;
        }

        if (whitespaceCount == 0)
            return default;

        int secondWordIndex = i;

        for (int j = sequenceList.Count - 1; j >= 0; j--)
        {
            i = secondWordIndex;

            WordSequence sequence = sequenceList[j];

            for (int k = 1; k < sequence.Count; k++)
            {
                string word = sequence.Words[k];

                if (string.Compare(value, i, word, 0, word.Length, comparison) == 0)
                {
                    if (sequence.Words.Length - 1 == k)
                    {
                        i += word.Length;

                        if (i == endIndex
                            || !char.IsLetter(value[i]))
                        {
                            if (sequenceMatch.EndIndex < i)
                                sequenceMatch = new WordSequenceMatch(sequence, match.Index, i - match.Index);
                        }

                        break;
                    }
                    else
                    {
                        i += word.Length;

                        whitespaceCount = 0;

                        while (i < endIndex
                            && char.IsWhiteSpace(value[i]))
                        {
                            i++;
                            whitespaceCount++;
                        }

                        if (whitespaceCount == 0)
                            break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        return sequenceMatch;
    }

    public SpellingData AddWords(IEnumerable<string> values)
    {
        WordList newList = WordList.AddValues(values);

        return new SpellingData(newList, CaseSensitiveWordList, Fixes, IgnoredValues);
    }

    public SpellingData AddWord(string value)
    {
        return new SpellingData(WordList.AddValue(value), CaseSensitiveWordList, Fixes, IgnoredValues);
    }

    public SpellingData AddFix(string error, SpellingFix fix)
    {
        FixList fixList = Fixes.Add(error, fix);

        return new SpellingData(WordList, CaseSensitiveWordList, fixList, IgnoredValues);
    }

    public SpellingData AddIgnoredValue(string value)
    {
        return new SpellingData(WordList, CaseSensitiveWordList, Fixes, IgnoredValues.Add(value));
    }

    public SpellingData AddIgnoredValues(IEnumerable<string> values)
    {
        return new SpellingData(WordList, CaseSensitiveWordList, Fixes, IgnoredValues.Union(values));
    }
}
