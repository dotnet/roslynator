// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Roslynator.Spelling;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal static class WordListLoader
{
    private static readonly Regex _splitRegex = new(" +");

    public static WordListLoaderResult Load(
        IEnumerable<string> paths,
        int minWordLength = -1,
        int maxWordLength = int.MaxValue,
        WordListLoadOptions options = WordListLoadOptions.None,
        CancellationToken cancellationToken = default)
    {
        LoadState state = LoadState.Create(options);

        foreach (string filePath in GetFiles(paths))
        {
            cancellationToken.ThrowIfCancellationRequested();

            LoadFile(filePath, minWordLength, maxWordLength, state);
        }

        Dictionary<string, HashSet<string>> fixes = state.Fixes;
        List<string> words = state.Words;
        List<string> nonWords = null;

        if ((options & WordListLoadOptions.DetectNonWords) != 0)
        {
            nonWords = new List<string>();

            for (int i = words.Count - 1; i >= 0; i--)
            {
                string word = words[i];
                Match match = Spellchecker.WordRegex.Match(word);

                if (!match.Success
                    || match.Index > 0
                    || match.Length < word.Length)
                {
                    nonWords.Add(word);
                    words.RemoveAt(i);
                }
            }
        }

        for (int i = words.Count - 1; i >= 0; i--)
        {
            fixes.Remove(words[i]);

            if (words[i].Length < minWordLength
                || words[i].Length > maxWordLength)
            {
                words.RemoveAt(i);
            }
        }

        List<string> caseSensitiveWords = state.CaseSensitiveWords;
        List<string> caseSensitiveNonWords = null;

        if (caseSensitiveWords is not null)
        {
            if ((options & WordListLoadOptions.DetectNonWords) != 0)
            {
                caseSensitiveNonWords = new List<string>();

                for (int i = caseSensitiveWords.Count - 1; i >= 0; i--)
                {
                    string word = caseSensitiveWords[i];
                    Match match = Spellchecker.WordRegex.Match(word);

                    if (!match.Success
                        || match.Index > 0
                        || match.Length < word.Length)
                    {
                        caseSensitiveNonWords.Add(word);
                        caseSensitiveWords.RemoveAt(i);
                    }
                }
            }

            for (int i = caseSensitiveWords.Count - 1; i >= 0; i--)
            {
                fixes.Remove(caseSensitiveWords[i]);

                if (caseSensitiveWords[i].Length < minWordLength
                    || caseSensitiveWords[i].Length > maxWordLength)
                {
                    caseSensitiveWords.RemoveAt(i);
                }
            }
        }

        foreach (HashSet<string> values in fixes.Values.ToList())
        {
            foreach (string value in values)
                fixes.Remove(value);
        }

        return new WordListLoaderResult(
            new WordList(state.Words, nonWords, state.Sequences, StringComparison.CurrentCultureIgnoreCase),
            new WordList(state.CaseSensitiveWords, caseSensitiveNonWords, state.CaseSensitiveSequences, StringComparison.CurrentCulture),
            FixList.Create(fixes));
    }

    private static IEnumerable<string> GetFiles(IEnumerable<string> paths)
    {
        foreach (string path in paths)
        {
            if (File.Exists(path))
            {
                yield return path;
            }
            else if (Directory.Exists(path))
            {
                foreach (string filePath in Directory.EnumerateFiles(
                    path,
                    "*.*",
                    SearchOption.AllDirectories))
                {
                    yield return filePath;
                }
            }
            else
            {
                throw new InvalidOperationException($"File or directory not found: {path}.");
            }
        }
    }

    public static List<string> LoadValues(
        string path,
        int minWordLength = -1,
        int maxWordLength = int.MaxValue)
    {
        LoadState state = LoadState.Create(WordListLoadOptions.IgnoreCase);

        LoadFile(path, minWordLength, maxWordLength, state);

        return state.Words;
    }

    private static void LoadFile(
        string path,
        int minWordLength,
        int maxWordLength,
        LoadState state)
    {
        List<string> words = state.Words;
        List<WordSequence> sequences = state.Sequences;
        List<string> caseSensitiveWords = state.CaseSensitiveWords;
        List<WordSequence> caseSensitiveSequences = state.CaseSensitiveSequences;
        Dictionary<string, HashSet<string>> fixes = state.Fixes;

        foreach (string line in File.ReadLines(path))
        {
            int i = 0;
            int startIndex = 0;
            int endIndex = line.Length;
            int separatorIndex = -1;
            int whitespaceIndex = -1;

            while (i < line.Length
                && char.IsWhiteSpace(line[i]))
            {
                startIndex++;
                i++;
            }

            while (i < line.Length)
            {
                char ch = line[i];

                if (ch == '#')
                {
                    endIndex = i;
                    break;
                }
                else if (separatorIndex == -1)
                {
                    if (ch == ':')
                    {
                        separatorIndex = i;
                    }
                    else if (whitespaceIndex == -1
                        && char.IsWhiteSpace(ch))
                    {
                        whitespaceIndex = i;
                    }
                }

                i++;
            }

            int j = endIndex - 1;

            while (j >= startIndex
                && char.IsWhiteSpace(line[j]))
            {
                endIndex--;
                j--;
            }

            if (separatorIndex >= 0)
            {
                string key = line.Substring(startIndex, separatorIndex - startIndex);

                if (key.Length >= minWordLength
                    && key.Length <= maxWordLength)
                {
                    startIndex = separatorIndex + 1;

                    while (startIndex < endIndex
                        && char.IsWhiteSpace(line[startIndex]))
                    {
                        startIndex++;
                    }

                    string value = line.Substring(startIndex, endIndex - startIndex);

                    Debug.Assert(value.Length > 0);

                    if (fixes.TryGetValue(key, out HashSet<string> fixes2))
                    {
                        fixes2.Add(value);
                    }
                    else
                    {
                        fixes[key] = new HashSet<string>(WordList.DefaultComparer) { value };
                    }
                }
            }
            else
            {
                string value = line.Substring(startIndex, endIndex - startIndex);

                if (whitespaceIndex >= 0
                    && whitespaceIndex < endIndex)
                {
                    string[] s = _splitRegex.Split(value);

                    Debug.Assert(s.Length > 1, s.Length.ToString());

                    if (s.Length > 0)
                    {
                        if (caseSensitiveSequences is not null
                            && !IsLower(value))
                        {
                            caseSensitiveSequences.Add(new WordSequence(s.ToImmutableArray()));
                        }
                        else
                        {
                            sequences.Add(new WordSequence(s.ToImmutableArray()));
                        }
                    }
                }
                else if (caseSensitiveWords is not null
                    && !IsLower(value))
                {
                    caseSensitiveWords.Add(value);
                }
                else
                {
                    words.Add(value);
                }
            }
        }
    }

    private static bool IsLower(string value)
    {
        foreach (char ch in value)
        {
            if (!char.IsLower(ch)
                && !char.IsWhiteSpace(ch))
            {
                return false;
            }
        }

        return true;
    }

    private class LoadState
    {
        private LoadState(
            List<string> words,
            List<string> caseSensitiveWords,
            List<WordSequence> sequences,
            List<WordSequence> caseSensitiveSequences,
            Dictionary<string, HashSet<string>> fixes)
        {
            Words = words;
            CaseSensitiveWords = caseSensitiveWords;
            Sequences = sequences;
            CaseSensitiveSequences = caseSensitiveSequences;
            Fixes = fixes;
        }

        public static LoadState Create(WordListLoadOptions options)
        {
            var words = new List<string>();
            var sequences = new List<WordSequence>();
            var fixes = new Dictionary<string, HashSet<string>>(WordList.DefaultComparer);

            List<string> caseSensitiveWords = null;
            List<WordSequence> caseSensitiveSequences = null;

            if ((options & WordListLoadOptions.IgnoreCase) == 0)
            {
                caseSensitiveWords = new List<string>();
                caseSensitiveSequences = new List<WordSequence>();
            }

            return new LoadState(words, caseSensitiveWords, sequences, caseSensitiveSequences, fixes);
        }

        public List<string> Words { get; }

        public List<string> CaseSensitiveWords { get; }

        public List<WordSequence> Sequences { get; }

        public List<WordSequence> CaseSensitiveSequences { get; }

        public Dictionary<string, HashSet<string>> Fixes { get; }
    }
}
