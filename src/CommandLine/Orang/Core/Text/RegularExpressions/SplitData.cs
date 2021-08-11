// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace Roslynator.Text.RegularExpressions
{
    internal sealed class SplitData
    {
        private SplitData(
            Regex regex,
            string input,
            int maxCount,
            SplitItemCollection items,
            bool omitGroups,
            GroupDefinitionCollection groupDefinitions)
        {
            Regex = regex;
            Input = input;
            MaxCount = maxCount;
            GroupDefinitions = groupDefinitions;
            OmitGroups = omitGroups;
            Items = items;
        }

        public static SplitData Create(
            Regex regex,
            string input,
            int count = 0,
            bool omitGroups = false,
            CancellationToken cancellationToken = default)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var groupDefinitions = new GroupDefinitionCollection(regex);

            List<SplitItem> items = GetItems(regex, input, count, groupDefinitions, omitGroups, cancellationToken);
#if DEBUG
            List<SplitItem> items2 = GetItems(regex, input, count, groupDefinitions, omitGroups: false, cancellationToken);

            string[] splits = regex.Split(input, count);

            Debug.Assert(splits.Length == items2.Count, items2.Count.ToString() + " " + splits.Length.ToString());

            for (int i = 0; i < items2.Count; i++)
            {
                if (items2[i].Value != splits[i])
                {
                    Debug.WriteLine(i);
                    Debug.WriteLine(splits[i]);
                    Debug.WriteLine(items2[i].Value);
                    Debug.Fail("");
                }
            }
#endif

            return new SplitData(regex, input, count, new SplitItemCollection(items), omitGroups, groupDefinitions);
        }

        private static List<SplitItem> GetItems(
            Regex regex,
            string input,
            int maxCount,
            GroupDefinitionCollection groupDefinitions,
            bool omitGroups,
            CancellationToken cancellationToken)
        {
            var splits = new List<SplitItem>();

            if (maxCount == 1)
            {
                splits.Add(SplitItem.Create(input));
                return splits;
            }

            Match firstMatch = regex.Match(input);

            if (!firstMatch.Success)
            {
                splits.Add(SplitItem.Create(input));
                return splits;
            }

            int prevIndex = 0;
            int count = 0;
            int splitNumber = 1;

            foreach (Match match in EnumerateMatches(regex, firstMatch, maxCount, cancellationToken))
            {
                splits.Add(SplitItem.Create(input.Substring(prevIndex, match.Index - prevIndex), prevIndex, splitNumber));
                count++;
                splitNumber++;
                prevIndex = match.Index + match.Length;

                if (!omitGroups)
                {
                    if (regex.RightToLeft)
                    {
                        for (int i = (groupDefinitions.Count - 1); i >= 0; i--)
                        {
                            if (groupDefinitions[i].Number != 0)
                            {
                                Group group = match.Groups[groupDefinitions[i].Number];
                                if (group.Success)
                                {
                                    splits.Add(SplitItem.Create(group, groupDefinitions[i]));
                                    count++;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (GroupDefinition groupDefinition in groupDefinitions)
                        {
                            if (groupDefinition.Number != 0)
                            {
                                Group group = match.Groups[groupDefinition.Number];
                                if (group.Success)
                                {
                                    splits.Add(SplitItem.Create(group, groupDefinition));
                                    count++;
                                }
                            }
                        }
                    }
                }
            }

            splits.Add(SplitItem.Create(input.Substring(prevIndex, input.Length - prevIndex), prevIndex, splitNumber));
            return splits;
        }

        private static IEnumerable<Match> EnumerateMatches(
            Regex regex,
            Match match,
            int count,
            CancellationToken cancellationToken)
        {
            count--;

            if (regex.RightToLeft)
            {
                var matches = new List<Match>();

                while (match.Success)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    matches.Add(match);

                    count--;

                    if (count == 0)
                        break;

                    match = match.NextMatch();
                }

                for (int i = (matches.Count - 1); i >= 0; i--)
                    yield return matches[i];
            }
            else
            {
                while (match.Success)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return match;

                    count--;

                    if (count == 0)
                        break;

                    match = match.NextMatch();
                }
            }
        }

        public Regex Regex { get; }

        public string Input { get; }

        public int MaxCount { get; }

        public GroupDefinitionCollection GroupDefinitions { get; }

        public bool OmitGroups { get; }

        public SplitItemCollection Items { get; }

        public int Count => Items.Count;
    }
}
