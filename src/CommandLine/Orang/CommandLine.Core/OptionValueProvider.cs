// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Roslynator
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class OptionValueProvider
    {
        public static readonly Regex MetaValueRegex = new(@"\<\p{Lu}+(_\p{Lu}+)*\>");

        public OptionValueProvider(string name, params OptionValue[] values)
            : this(name, null, values)
        {
        }

        public OptionValueProvider(string name, IEnumerable<OptionValue> values)
            : this(name, null, values)
        {
        }

        public OptionValueProvider(string name, OptionValueProvider parent, params OptionValue[] values)
        {
            Name = name;
            Parent = parent;
            Values = values.ToImmutableArray();
        }

        public OptionValueProvider(string name, OptionValueProvider parent, IEnumerable<OptionValue> values)
        {
            Name = name;
            Parent = parent;
            Values = values.ToImmutableArray();
        }

        public string Name { get; }

        public OptionValueProvider Parent { get; }

        public ImmutableArray<OptionValue> Values { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => Name;

        public OptionValueProvider WithValues(params OptionValue[] values)
        {
            return WithValues(Name, values);
        }

        public OptionValueProvider WithValues(string name, params OptionValue[] values)
        {
            return new OptionValueProvider(name, this, Values.AddRange(values));
        }

        public OptionValueProvider WithoutValues(params OptionValue[] values)
        {
            return WithoutValues(Name, values);
        }

        public OptionValueProvider WithoutValues(string name, params OptionValue[] values)
        {
            ImmutableArray<OptionValue>.Builder builder = ImmutableArray.CreateBuilder<OptionValue>();

            foreach (OptionValue value in Values)
            {
                if (Array.IndexOf(values, value) == -1)
                    builder.Add(value);
            }

            return new OptionValueProvider(name, this, builder);
        }

        public bool ContainsKeyOrShortKey(string value)
        {
            foreach (OptionValue optionValue in Values)
            {
                if (optionValue.Kind == OptionValueKind.KeyValuePair)
                {
                    var keyValue = (KeyValuePairOptionValue)optionValue;

                    if (keyValue.IsKeyOrShortKey(value))
                        return true;
                }
            }

            return false;
        }

        public bool TryParseEnum<TEnum>(string value, out TEnum result) where TEnum : struct
        {
            foreach (OptionValue optionValue in Values)
            {
                if (optionValue is SimpleOptionValue enumOptionValue)
                {
                    if (enumOptionValue.Value == value
                        || enumOptionValue.ShortValue == value)
                    {
                        return Enum.TryParse(optionValue.Name, out result);
                    }
                }
            }

            result = default;
            return false;
        }

        public OptionValue GetValue(string name)
        {
            foreach (OptionValue value in Values)
            {
                if (string.Equals(value.Name, name, StringComparison.Ordinal))
                    return value;
            }

            throw new ArgumentException("", nameof(name));
        }

        public static IEnumerable<OptionValueProvider> GetProviders(
            IEnumerable<CommandOption> options,
            IEnumerable<OptionValueProvider> providers)
        {
            IEnumerable<string> metaValues = options
                .SelectMany(f => MetaValueRegex.Matches(f.MetaValue).Cast<Match>().Select(m => m.Value))
                .Concat(options.Select(f => f.MetaValue))
                .Distinct();

            ImmutableArray<OptionValueProvider> providers2 = metaValues
                .Join(providers, f => f, f => f.Name, (_, f) => f)
                .ToImmutableArray();

            return providers2
                .SelectMany(f => f.Values)
                .OfType<KeyValuePairOptionValue>()
                .Select(f => f.Value)
                .Distinct()
                .Join(providers, f => f, f => f.Name, (_, f) => f)
                .Concat(providers2)
                .Distinct()
                .OrderBy(f => f.Name);
        }
    }
}
