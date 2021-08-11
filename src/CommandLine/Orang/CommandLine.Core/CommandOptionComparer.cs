// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Roslynator
{
    public abstract class CommandOptionComparer : IComparer<CommandOption>
    {
        public static CommandOptionComparer Name { get; } = new CommandOptionNameComparer();

        public abstract int Compare(CommandOption x, CommandOption y);

        private class CommandOptionNameComparer : CommandOptionComparer
        {
            public override int Compare(CommandOption x, CommandOption y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                string name1 = x.Name;
                string name2 = y.Name;

                if (string.IsNullOrEmpty(name1))
                {
                    if (string.IsNullOrEmpty(name2))
                    {
                        string shortName1 = x.ShortName;
                        string shortName2 = y.ShortName;

                        if (string.IsNullOrEmpty(shortName1))
                        {
                            if (string.IsNullOrEmpty(shortName2))
                            {
                                Debug.Fail("");
                                return 0;
                            }

                            return 1;
                        }

                        if (string.IsNullOrEmpty(shortName2))
                            return -1;

                        return StringComparer.InvariantCulture.Compare(shortName1, shortName2);
                    }

                    return 1;
                }

                if (string.IsNullOrEmpty(name2))
                    return -1;

                return StringComparer.InvariantCulture.Compare(name1, name2);
            }
        }
    }
}
