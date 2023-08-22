// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Roslynator.Spelling;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal class WordList
{
    public static StringComparison DefaultComparison { get; } = StringComparison.CurrentCultureIgnoreCase;

    public static StringComparer DefaultComparer { get; } = StringComparerUtility.FromComparison(DefaultComparison);

    public static WordList Default { get; } = new(null, DefaultComparison);

    public WordList(IEnumerable<string> values, StringComparison? comparison = null)
        : this(values, null, ImmutableArray<WordSequence>.Empty, comparison)
    {
    }

    public WordList(
        IEnumerable<string> words,
        IEnumerable<string> nonWords,
        IEnumerable<WordSequence> sequences,
        StringComparison? comparison = null)
    {
        Comparer = StringComparerUtility.FromComparison(comparison ?? DefaultComparison);
        Comparison = comparison ?? DefaultComparison;

        Words = words?.ToImmutableHashSet(Comparer) ?? ImmutableHashSet<string>.Empty;
        NonWords = nonWords?.ToImmutableHashSet(Comparer) ?? ImmutableHashSet<string>.Empty;

        Sequences = sequences?
            .GroupBy(f => f.First, Comparer)
            .ToImmutableDictionary(f => f.Key, f => f.ToImmutableArray(), Comparer)
            ?? ImmutableDictionary<string, ImmutableArray<WordSequence>>.Empty;
    }

    public ImmutableHashSet<string> Words { get; }

    public ImmutableHashSet<string> NonWords { get; }

    public ImmutableDictionary<string, ImmutableArray<WordSequence>> Sequences { get; }

    public StringComparison Comparison { get; }

    public StringComparer Comparer { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"Words = {Words.Count}  Sequences = {Sequences.Sum(f => f.Value.Length)}";

    public WordList Intersect(WordList wordList, params WordList[] additionalWordLists)
    {
        IEnumerable<string> intersect = Words.Intersect(wordList.Words, Comparer);

        if (additionalWordLists?.Length > 0)
        {
            intersect = intersect
                .Intersect(additionalWordLists.SelectMany(f => f.Words), Comparer);
        }

        return WithValues(intersect);
    }

    public WordList Except(WordList wordList, params WordList[] additionalWordLists)
    {
        IEnumerable<string> except = Words.Except(wordList.Words, Comparer);

        if (additionalWordLists?.Length > 0)
        {
            except = except
                .Except(additionalWordLists.SelectMany(f => f.Words), Comparer);
        }

        return WithValues(except);
    }

    public bool Contains(string value)
    {
        return Words.Contains(value);
    }

    public WordList AddValue(string value)
    {
        return new WordList(Words.Add(value), Comparison);
    }

    public WordList AddValues(IEnumerable<string> values)
    {
        values = Words.Concat(values).Distinct(Comparer);

        return new WordList(values, Comparison);
    }

    public WordList AddValues(WordList wordList, params WordList[] additionalWordLists)
    {
        IEnumerable<string> concat = Words.Concat(wordList.Words);

        if (additionalWordLists?.Length > 0)
            concat = concat.Concat(additionalWordLists.SelectMany(f => f.Words));

        return WithValues(concat.Distinct(Comparer));
    }

    public WordList WithValues(IEnumerable<string> values)
    {
        return new WordList(values, Comparison);
    }

    public static void Save(
        string path,
        WordList wordList)
    {
        Save(path, wordList.Words, wordList.Comparer);
    }

    public static void Save(
        string path,
        IEnumerable<string> values,
        StringComparer comparer,
        bool merge = false)
    {
        if (merge
            && File.Exists(path))
        {
            values = values.Concat(WordListLoader.LoadValues(path));
        }

        values = values
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .Select(f => f.Trim())
            .Distinct(comparer)
            .OrderBy(f => f, comparer);

        Debug.WriteLine($"Saving '{path}'");

        File.WriteAllText(path, string.Join(Environment.NewLine, values));
    }

    public void Save(string path)
    {
        Save(path ?? throw new ArgumentException("", nameof(path)), this);
    }
}
