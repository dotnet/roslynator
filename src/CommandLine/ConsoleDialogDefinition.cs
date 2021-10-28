// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;

namespace Roslynator.CommandLine
{
    internal abstract class ConsoleDialogDefinition
    {
        private static ConsoleDialogDefinition _default;

        protected ConsoleDialogDefinition(string shortHelp, string longHelp)
        {
            ShortHelp = shortHelp;
            LongHelp = longHelp;
        }

        public static ConsoleDialogDefinition Default
        {
            get
            {
                if (_default == null)
                    Interlocked.CompareExchange(ref _default, CreateDefaultDefinition(), null);

                return _default;
            }
        }

        public string ShortHelp { get; }

        public string LongHelp { get; }

        public abstract bool TryGetValue(string key, out DialogResult result);

        private static ConsoleDialogDefinition CreateDefaultDefinition()
        {
            ImmutableDictionary<string, DialogResult>.Builder builder
                = ImmutableDictionary.CreateBuilder<string, DialogResult>();

            builder.Add("y", DialogResult.Yes);
            builder.Add("yes", DialogResult.Yes);
            builder.Add("ya", DialogResult.YesToAll);
            builder.Add("yes to all", DialogResult.YesToAll);
            builder.Add("n", DialogResult.No);
            builder.Add("no", DialogResult.No);
            builder.Add("na", DialogResult.NoToAll);
            builder.Add("no to all", DialogResult.NoToAll);
            builder.Add("c", DialogResult.Cancel);
            builder.Add("cancel", DialogResult.Cancel);

            ImmutableDictionary<string, DialogResult> map = builder.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);

            return new DefaultDialogDefinition(
                shortHelp: "Y[A]/N[A]/C",
                longHelp: "Y (Yes), YA (Yes to All), N (No), NA (No to All), C (Cancel)",
                map: map);
        }

        private class DefaultDialogDefinition : ConsoleDialogDefinition
        {
            public DefaultDialogDefinition(
                string shortHelp,
                string longHelp,
                ImmutableDictionary<string, DialogResult> map)
                : base(shortHelp, longHelp)
            {
                Map = map;
            }

            public ImmutableDictionary<string, DialogResult> Map { get; }

            public override bool TryGetValue(string key, out DialogResult result)
            {
                return Map.TryGetValue(key, out result);
            }
        }
    }
}
