// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Roslynator
{
    public abstract class OptionValue
    {
        private protected static readonly Regex _lowerLetterUpperLetterRegex = new(@"\p{Ll}\p{Lu}");

        protected OptionValue(
            string name,
            string helpValue,
            string description = null,
            bool hidden = false,
            bool canContainExpression = false)
        {
            Name = name;
            HelpValue = helpValue;
            Description = description;
            Hidden = hidden;
            CanContainExpression = canContainExpression;
        }

        public abstract OptionValueKind Kind { get; }

        public string Name { get; }

        public string HelpValue { get; }

        public string Description { get; }

        public bool Hidden { get; }

        public bool CanContainExpression { get; }

        public static string GetDefaultHelpText<TEnum>(bool multiline = false) where TEnum : struct
        {
            IEnumerable<string> values = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(f => _lowerLetterUpperLetterRegex
                    .Replace(f.ToString(), e => e.Value.Insert(1, "-"))
                    .ToLowerInvariant())
                .OrderBy(f => f);

            if (multiline)
            {
                return string.Join(Environment.NewLine + "  ", values);
            }
            else
            {
                return TextHelpers.Join(", ", " and ", values);
            }
        }
    }
}
