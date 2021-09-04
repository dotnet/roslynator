// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Roslynator.CommandLine.Help
{
    internal static class HelpProvider
    {
        public const int SeparatorWidth = 2;

        public static ImmutableArray<CommandItem> GetCommandItems(IEnumerable<Command> commands, Filter filter = null)
        {
            if (!commands.Any())
                return ImmutableArray<CommandItem>.Empty;

            int width = commands.Max(f => f.Name.Length) + SeparatorWidth;

            ImmutableArray<CommandItem>.Builder builder = ImmutableArray.CreateBuilder<CommandItem>();

            foreach (Command command in commands)
            {
                StringBuilder sb = StringBuilderCache.GetInstance();

                sb.Append(command.Name);
                sb.AppendSpaces(width - command.Name.Length);

                string syntax = StringBuilderCache.GetStringAndFree(sb);
                string description = command.Description ?? "";

                var commandItem = new CommandItem(command, syntax, description);

                if (filter?.IsMatch(commandItem.Text) != false)
                    builder.Add(commandItem);
            }

            return builder.ToImmutableArray();
        }

        public static ImmutableArray<ArgumentItem> GetArgumentItems(
            IEnumerable<CommandArgument> arguments,
            Filter filter = null)
        {
            int width = CalculateArgumentsWidths(arguments);

            ImmutableArray<ArgumentItem>.Builder builder = ImmutableArray.CreateBuilder<ArgumentItem>();

            bool anyIsOptional = arguments.Any(f => !f.IsRequired);

            foreach (CommandArgument argument in arguments)
            {
                StringBuilder sb = StringBuilderCache.GetInstance();

                sb.Append(argument.Name);

                if (!string.IsNullOrEmpty(argument.Description))
                {
                    sb.AppendSpaces(width - argument.Name.Length - ((argument.IsRequired) ? 0 : 2));
                }

                string syntax = StringBuilderCache.GetStringAndFree(sb);
                string description = argument.Description ?? "";

                var argumentItem = new ArgumentItem(argument, syntax, description);

                if (filter?.IsMatch(argumentItem.Text) != false)
                {
                    builder.Add(argumentItem);
                }
            }

            return builder.ToImmutableArray();
        }

        public static ImmutableArray<OptionItem> GetOptionItems(IEnumerable<CommandOption> options, Filter filter = null)
        {
            int width = CalculateOptionsWidths(options);

            bool anyIsOptional = options.Any(f => !f.IsRequired);
            bool anyHasShortName = options.Any(f => !string.IsNullOrEmpty(f.ShortName));

            ImmutableArray<OptionItem>.Builder builder = ImmutableArray.CreateBuilder<OptionItem>();

            foreach (CommandOption option in options)
            {
                StringBuilder sb = StringBuilderCache.GetInstance();

                if (!string.IsNullOrEmpty(option.ShortName))
                {
                    sb.Append("-");
                    sb.Append(option.ShortName);

                    if (!string.IsNullOrEmpty(option.Name))
                        sb.Append(", ");
                }
                else if (anyHasShortName)
                {
                    sb.Append(' ', 4);
                }

                if (!string.IsNullOrEmpty(option.Name))
                {
                    sb.Append("--");
                    sb.Append(option.Name);
                }

                sb.AppendSpaces(1);

                if (!string.IsNullOrEmpty(option.MetaValue))
                    sb.Append(option.MetaValue);

                sb.AppendSpaces(width - sb.Length + SeparatorWidth);

                string syntax = StringBuilderCache.GetStringAndFree(sb);
                string description = option.Description ?? "";

                var optionItem = new OptionItem(option, syntax: syntax, description);

                if (filter?.IsMatch(optionItem.Text) != false)
                    builder.Add(optionItem);
            }

            return builder.ToImmutableArray();
        }

        public static ImmutableArray<OptionValueList> GetOptionValues(
            IEnumerable<CommandOption> options,
            IEnumerable<OptionValueProvider> providers,
            Filter filter = null)
        {
            providers = OptionValueProvider.GetProviders(options, providers).ToImmutableArray();

            IEnumerable<OptionValue> allValues = providers.SelectMany(p => p.Values.Where(v => !v.Hidden));

            ImmutableArray<OptionValueList>.Builder builder = ImmutableArray.CreateBuilder<OptionValueList>();

            int width = CalculateOptionValuesWidth(allValues);

            foreach (OptionValueProvider provider in providers)
            {
                IEnumerable<OptionValue> values = provider.Values.Where(f => !f.Hidden);
                ImmutableArray<OptionValueItem> valueItems = GetOptionValueItems(values, width);

                builder.Add(new OptionValueList(provider.Name, valueItems));
            }

            return (filter != null)
                ? FilterOptionValues(builder, filter)
                : builder.ToImmutableArray();
        }

        public static ImmutableArray<OptionValueItem> GetOptionValueItems(
            IEnumerable<OptionValue> optionValues,
            int maxWidth)
        {
            int maxShortNameWidth = optionValues.Max(f => GetShortValue(f).Length);

            ImmutableArray<OptionValueItem>.Builder builder = ImmutableArray.CreateBuilder<OptionValueItem>();

            foreach (OptionValue optionValue in optionValues)
            {
                StringBuilder sb = StringBuilderCache.GetInstance();

                string shortValue = GetShortValue(optionValue);

                if (!string.IsNullOrEmpty(shortValue))
                {
                    sb.Append(' ', maxShortNameWidth - shortValue.Length);
                    sb.Append(shortValue);
                    sb.Append(", ");
                }
                else
                {
                    sb.Append(' ', maxShortNameWidth + 2);
                }

                string value = GetValue(optionValue);

                sb.Append(value);
                sb.AppendSpaces(maxWidth - sb.Length);

                string description = optionValue.Description ?? "";

                builder.Add(new OptionValueItem(optionValue, StringBuilderCache.GetStringAndFree(sb), description));
            }

            return builder.ToImmutableArray();
        }

        private static ImmutableArray<OptionValueList> FilterOptionValues(
            IEnumerable<OptionValueList> values,
            Filter filter)
        {
            ImmutableArray<OptionValueList>.Builder builder = ImmutableArray.CreateBuilder<OptionValueList>();

            foreach (OptionValueList valueList in values)
            {
                if (filter.IsMatch(valueList.MetaValue))
                {
                    builder.Add(valueList);
                }
                else
                {
                    ImmutableArray<OptionValueItem> valueItems = valueList.Values
                        .Where(f => filter.IsMatch(f.Text))
                        .ToImmutableArray();

                    if (valueItems.Any())
                        builder.Add(new OptionValueList(valueList.MetaValue, valueItems));
                }
            }

            return builder.ToImmutableArray();
        }

        public static ImmutableArray<string> GetExpressionItems(IEnumerable<OptionValueList> values, bool includeDate = true)
        {
            return (values.SelectMany(f => f.Values).Any(f => f.Value.CanContainExpression))
                ? GetExpressionLines(includeDate: includeDate)
                : ImmutableArray<string>.Empty;
        }

        public static string GetExpressionsText(string variableName = null, string indent = null, bool includeDate = true)
        {
            ImmutableArray<string> expressions = GetExpressionLines(variableName: variableName, includeDate: includeDate);

            return indent + string.Join(Environment.NewLine + indent, expressions);
        }

        public static ImmutableArray<string> GetExpressionLines(string variableName = null, bool includeDate = true)
        {
            ImmutableArray<(string expression, string description)> items = GetExpressionItems(variableName, includeDate);

            int maxWidth = items.Max(f => f.expression.Length);

            return items
                .Select(f =>
                {
                    string s = f.expression;

                    if (!string.IsNullOrEmpty(f.description))
                    {
                        s = s.PadRight(maxWidth + SeparatorWidth);
                        s += f.description;
                    }

                    return s;
                })
                .ToImmutableArray();
        }

        public static ImmutableArray<(string expression, string description)> GetExpressionItems(
            string variableName = null,
            bool includeDate = true)
        {
            ImmutableArray<(string, string)>.Builder builder = ImmutableArray.CreateBuilder<(string, string)>();

            variableName ??= "x";

            builder.Add(($"{variableName}=n", ""));
            builder.Add(($"{variableName}<n", ""));
            builder.Add(($"{variableName}>n", ""));
            builder.Add(($"{variableName}<=n", ""));
            builder.Add(($"{variableName}>=n", ""));
            builder.Add(($"{variableName}=<min;max>", "Inclusive interval"));
            builder.Add(($"{variableName}=(min;max)", "Exclusive interval"));

            if (includeDate)
                builder.Add(($"{variableName}=-d|[d.]hh:mm[:ss]", $"{variableName} is greater than actual date - <VALUE>"));

            return builder.ToImmutableArray();
        }

        public static int CalculateArgumentsWidths(IEnumerable<CommandArgument> arguments)
        {
            return (arguments.Any())
                ? arguments.Max(f => f.Name.Length + ((f.IsRequired) ? 0 : 2)) + SeparatorWidth
                : default;
        }

        public static int CalculateOptionsWidths(IEnumerable<CommandOption> options)
        {
            if (!options.Any())
                return default;

            int width = 2; // --

            if (options.Any(f => !string.IsNullOrEmpty(f.ShortName)))
                width += 4; // -x, 

            width += options.Max(f =>
            {
                int length = f.Name.Length;

                if (f.MetaValue != null)
                {
                    length++; // separator between option name and meta value;
                    length += f.MetaValue.Length;
                }

                return length;
            });

            return width;
        }

        public static int CalculateOptionValuesWidth(IEnumerable<OptionValue> optionValues)
        {
            if (!optionValues.Any())
                return 0;

            int maxShortNameWidth = optionValues.Max(f => GetShortValue(f).Length);
            int maxNameWidth = optionValues.Max(f => GetValue(f).Length);

            return maxShortNameWidth + maxNameWidth + 4;
        }

        private static string GetValue(OptionValue value)
        {
            return value switch
            {
                SimpleOptionValue enumOptionValue => enumOptionValue.Value,
                KeyValuePairOptionValue keyOptionValue => $"{keyOptionValue.Key}={keyOptionValue.Value}",
                _ => throw new InvalidOperationException(),
            };
        }

        private static string GetShortValue(OptionValue value)
        {
            return value switch
            {
                SimpleOptionValue enumOptionValue => enumOptionValue.ShortValue,
                KeyValuePairOptionValue keyOptionValue => keyOptionValue.ShortKey,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
