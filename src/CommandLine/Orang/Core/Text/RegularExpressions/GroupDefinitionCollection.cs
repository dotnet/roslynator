// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Roslynator.Text.RegularExpressions
{
    internal class GroupDefinitionCollection : ReadOnlyCollection<GroupDefinition>
    {
        private readonly Dictionary<string, GroupDefinition> _names;
        private readonly Dictionary<int, GroupDefinition> _numbers;

        public GroupDefinitionCollection(Regex regex)
            : base(CreateGroupDefinitions(regex))
        {
            _names = Items.ToDictionary(f => f.Name, f => f);
            _numbers = Items.ToDictionary(f => f.Number, f => f);
        }

        private static GroupDefinition[] CreateGroupDefinitions(Regex regex)
        {
            if (regex == null)
                throw new ArgumentNullException(nameof(regex));

            string[] names = regex.GetGroupNames();

            var groups = new GroupDefinition[names.Length];

            for (int i = 0; i < names.Length; i++)
                groups[i] = new GroupDefinition(i, names[i]);

            return groups;
        }

        public bool Contains(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return _names.ContainsKey(name);
        }

        public bool Contains(int number)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number));

            return _numbers.ContainsKey(number);
        }

        public GroupDefinition this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                try
                {
                    return _names[name];
                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentOutOfRangeException(nameof(name));
                }
            }
        }
    }
}
