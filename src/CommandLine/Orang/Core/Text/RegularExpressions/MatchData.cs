// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace Roslynator.Text.RegularExpressions
{
    internal sealed class MatchData
    {
        private MatchData(
            Regex regex,
            string input,
            int maxCount,
            MatchItemCollection items,
            GroupDefinitionCollection groupDefinitions)
        {
            Regex = regex;
            Input = input;
            MaxCount = maxCount;
            Items = items;
            GroupDefinitions = groupDefinitions;
        }

        public static MatchData Create(
            string input,
            Regex regex,
            int maxCount = -1,
            CancellationToken cancellationToken = default)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            Match match = regex.Match(input);

            return Create(input, regex, match, maxCount, cancellationToken);
        }

        internal static MatchData Create(
            string input,
            Regex regex,
            Match match,
            int maxCount = -1,
            CancellationToken cancellationToken = default)
        {
            var groupDefinitions = new GroupDefinitionCollection(regex);

            var items = new List<MatchItem>();

            int max = (maxCount < 0) ? int.MaxValue : maxCount;

            while (match.Success
                && items.Count < max)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var item = new MatchItem(match, groupDefinitions);
                items.Add(item);
                match = match.NextMatch();
            }

            return new MatchData(regex, input, maxCount, new MatchItemCollection(items), groupDefinitions);
        }

        public Regex Regex { get; }

        public string Input { get; }

        public int MaxCount { get; }

        public MatchItemCollection Items { get; }

        public GroupDefinitionCollection GroupDefinitions { get; }

        public int Count => Items.Count;
    }
}
