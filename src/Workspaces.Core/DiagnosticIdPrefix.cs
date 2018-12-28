// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class DiagnosticIdPrefix
    {
        private static readonly Dictionary<string, string> _prefixes = new Dictionary<string, string>(LetterPrefixStringEqualityComparer.Instance);

        public static IEnumerable<(string prefix, int count)> CountPrefixes(IEnumerable<string> values)
        {
            foreach (IGrouping<string, string> grouping in values
                .Select(id =>
                {
                    if (!_prefixes.TryGetValue(id, out string prefix))
                    {
                        int length = 0;

                        for (int i = 0; i < id.Length; i++)
                        {
                            if (char.IsLetter(id[i]))
                            {
                                length++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        prefix = id.Substring(0, length);

                        _prefixes[prefix] = prefix;
                    }

                    return prefix;
                })
                .GroupBy(f => f)
                .OrderBy(f => f.Key))
            {
                yield return (grouping.Key, grouping.Count());
            }
        }

        private class LetterPrefixStringEqualityComparer : EqualityComparer<string>
        {
            public static LetterPrefixStringEqualityComparer Instance { get; } = new LetterPrefixStringEqualityComparer();

            public override bool Equals(string x, string y)
            {
                int len = x.Length;

                if (x.Length > y.Length)
                {
                    len = y.Length;

                    if (char.IsLetter(x[len]))
                        return false;
                }
                else if (x.Length < y.Length)
                {
                    if (char.IsLetter(y[len]))
                        return false;
                }

                for (int i = 0; i < len; i++)
                {
                    if (char.IsLetter(x[i]))
                    {
                        if (char.IsLetter(y[i]))
                        {
                            if (x[i] != y[i])
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (char.IsLetter(y[i]))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                return true;
            }

            public override int GetHashCode(string obj)
            {
                return 0;
            }
        }
    }
}
