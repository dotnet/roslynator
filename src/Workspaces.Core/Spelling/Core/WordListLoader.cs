// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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

        foreach (string word in state.Words)
            fixes.Remove(word);

        List<string> caseSensitiveWords = state.CaseSensitiveWords;

        if (caseSensitiveWords is not null)
        {
            foreach (string word in caseSensitiveWords)
                fixes.Remove(word);
        }

        foreach (HashSet<string> values in fixes.Values.ToList())
        {
            foreach (string value in values)
                fixes.Remove(value);
        }

        return new WordListLoaderResult(
            new WordList(state.Words, state.NonWords, state.Sequences, StringComparison.CurrentCultureIgnoreCase),
            new WordList(
                state.CaseSensitiveWords,
                state.CaseSensitiveNonWords,
                state.CaseSensitiveSequences,
                StringComparison.CurrentCulture),
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
        List<string> nonWords = state.NonWords;
        List<string> caseSensitiveWords = state.CaseSensitiveWords;
        List<string> caseSensitiveNonWords = state.CaseSensitiveNonWords;
        List<WordSequence> sequences = state.Sequences;
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
                if (line[i] == ':')
                {
                    if (i < line.Length - 1
                        && line[i + 1] == ':')
                    {
                        i++;
                    }
                    else
                    {
                        separatorIndex = i;
                        break;
                    }
                }
                else if (char.IsWhiteSpace(line[i]))
                {
                    whitespaceIndex = i;
                    break;
                }

                i++;
            }

            i = endIndex - 1;

            while (i >= startIndex
                && char.IsWhiteSpace(line[i]))
            {
                endIndex--;
                i--;
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
                string value = line
                    .Substring(startIndex, endIndex - startIndex)
                    .Replace("::", ":");

                if (value.Length == 0)
                    continue;

                if (whitespaceIndex >= 0
                    && whitespaceIndex < endIndex)
                {
                    string[] values = _splitRegex.Split(value);

                    if (caseSensitiveSequences is not null
                        && !IsLower(value))
                    {
                        caseSensitiveSequences.Add(new WordSequence(values));
                    }
                    else
                    {
                        sequences.Add(new WordSequence(values));
                    }
                }
                else if (caseSensitiveWords is not null
                    && !IsLower(value))
                {
                    if (caseSensitiveNonWords is not null
                        && !IsWord(value))
                    {
                        caseSensitiveNonWords.Add(value);
                    }
                    else if (value.Length >= minWordLength
                        && value.Length <= maxWordLength)
                    {
                        caseSensitiveWords.Add(value);
                    }
                }
                else if (nonWords is not null
                    && !IsWord(value))
                {
                    nonWords.Add(value);
                }
                else if (value.Length >= minWordLength
                    && value.Length <= maxWordLength)
                {
                    words.Add(value);
                }
            }
        }
    }

    private static bool IsWord(string value)
    {
        Match match = Spellchecker.WordRegex.Match(value);

        return match.Success
            && match.Index == 0
            && match.Length == value.Length;
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
            List<string> nonWords,
            List<string> caseSensitiveWords,
            List<string> caseSensitiveNonWords,
            List<WordSequence> sequences,
            List<WordSequence> caseSensitiveSequences,
            Dictionary<string, HashSet<string>> fixes)
        {
            Words = words;
            NonWords = nonWords;
            CaseSensitiveWords = caseSensitiveWords;
            CaseSensitiveNonWords = caseSensitiveNonWords;
            Sequences = sequences;
            CaseSensitiveSequences = caseSensitiveSequences;
            Fixes = fixes;
        }

        public static LoadState Create(WordListLoadOptions options)
        {
            var words = new List<string>();
            var sequences = new List<WordSequence>();
            var fixes = new Dictionary<string, HashSet<string>>(WordList.DefaultComparer);

            List<string> nonWords = null;
            List<string> caseSensitiveWords = null;
            List<string> caseSensitiveNonWords = null;
            List<WordSequence> caseSensitiveSequences = null;

            if ((options & WordListLoadOptions.DetectNonWords) != 0)
                nonWords = new List<string>();

            if ((options & WordListLoadOptions.IgnoreCase) == 0)
            {
                caseSensitiveWords = new List<string>();
                caseSensitiveSequences = new List<WordSequence>();

                if ((options & WordListLoadOptions.DetectNonWords) != 0)
                    caseSensitiveNonWords = new List<string>();
            }

            return new LoadState(words, nonWords, caseSensitiveWords, caseSensitiveNonWords, sequences, caseSensitiveSequences, fixes);
        }

        public List<string> Words { get; }

        public List<string> NonWords { get; }

        public List<string> CaseSensitiveWords { get; }

        public List<string> CaseSensitiveNonWords { get; }

        public List<WordSequence> Sequences { get; }

        public List<WordSequence> CaseSensitiveSequences { get; }

        public Dictionary<string, HashSet<string>> Fixes { get; }
    }
}
