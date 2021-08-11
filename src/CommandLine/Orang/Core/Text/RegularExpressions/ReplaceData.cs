// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace Roslynator.Text.RegularExpressions
{
    internal sealed class ReplaceData
    {
        private ReplaceData(
            Regex regex,
            string input,
            int maxCount,
            ReplaceItemCollection items,
            string output)
        {
            Regex = regex;
            Input = input;
            MaxCount = maxCount;
            Items = items;
            Output = output;
        }

        public static ReplaceData Create(
            Regex regex,
            string input,
            string replacement = null,
            int count = -1,
            CancellationToken cancellationToken = default)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (replacement == null)
                replacement = "";

            return Create(regex, input, f => Evaluate(f), count, cancellationToken);

            string Evaluate(Match match)
            {
                return match.Result(replacement);
            }
        }

        public static ReplaceData Create(
            Regex regex,
            string input,
            MatchEvaluator evaluator,
            int count = -1,
            CancellationToken cancellationToken = default)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (evaluator == null)
                throw new ArgumentNullException(nameof(evaluator));

            var items = new List<ReplaceItem>();
            int offset = 0;

            string output = regex.Replace(
                input,
                match =>
                {
                    string result = evaluator(match);

                    var item = new ReplaceItem(match, result, match.Index + offset);
                    items.Add(item);

                    if (!regex.RightToLeft)
                        offset += item.Value.Length - match.Length;

                    cancellationToken.ThrowIfCancellationRequested();

                    return item.Value;
                },
                count);

            if (regex.RightToLeft)
                items.Reverse();

            return new ReplaceData(regex, input, count, new ReplaceItemCollection(items), output);
        }

        public Regex Regex { get; }

        public string Input { get; }

        public int MaxCount { get; }

        public ReplaceItemCollection Items { get; }

        public string Output { get; }

        public int Count => Items.Count;
    }
}
