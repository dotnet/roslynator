// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Roslynator.CommandLine
{
    internal readonly struct ProjectName
    {
        private ProjectName(string name) : this(name, name, null)
        {
        }

        private ProjectName(string name, string nameWithoutMoniker, string moniker)
        {
            Name = name;
            NameWithoutMoniker = nameWithoutMoniker;
            Moniker = moniker;
        }

        public string Name { get; }

        public string NameWithoutMoniker { get; }

        public string Moniker { get; }

        public static ProjectName Create(string name)
        {
            if (name.EndsWith(")"))
            {
                int openParenToken = name.LastIndexOf('(');

                if (openParenToken > 0)
                {
                    string moniker = name.Substring(openParenToken + 1, name.Length - 2 - openParenToken);

                    Debug.Assert(IsTargetFrameworkMoniker(moniker), moniker);

                    if (IsTargetFrameworkMoniker(moniker))
                    {
                        string nameWithoutMoniker = name.Remove(name.Length - 2 - moniker.Length);

                        return new ProjectName(name, nameWithoutMoniker, moniker);
                    }
                }
            }

            return new ProjectName(name);
        }

        private static bool IsTargetFrameworkMoniker(string value)
        {
            return WellKnownTargetFrameworkMonikers.Contains(value)
                || _targetFrameworkMonikerRegex.IsMatch(value);
        }

        private static readonly Regex _targetFrameworkMonikerRegex = new(@"\Anet[0-9]+\.[0-9](-[a-zA-Z0-9.]+)?\z");

        private static readonly ImmutableHashSet<string> WellKnownTargetFrameworkMonikers = ImmutableHashSet.CreateRange(new[] {
            "aspnet50",
            "aspnetcore50",
            "dnx",
            "dnx45",
            "dnx451",
            "dnx452",
            "dnxcore50",
            "dotnet",
            "dotnet50",
            "dotnet51",
            "dotnet52",
            "dotnet53",
            "dotnet54",
            "dotnet55",
            "dotnet56",
            "net11",
            "net20",
            "net35",
            "net40",
            "net403",
            "net45",
            "net451",
            "net452",
            "net46",
            "net461",
            "net462",
            "net47",
            "net471",
            "net472",
            "net48",
            "net50",
            "net5.0",
            "net6.0",
            "net7.0",
            "netcore",
            "netcore45",
            "netcore451",
            "netcore50",
            "netcoreapp1.0",
            "netcoreapp1.1",
            "netcoreapp2.0",
            "netcoreapp2.1",
            "netcoreapp2.2",
            "netcoreapp3.0",
            "netcoreapp3.1",
            "netmf",
            "netstandard1.0",
            "netstandard1.1",
            "netstandard1.2",
            "netstandard1.3",
            "netstandard1.4",
            "netstandard1.5",
            "netstandard1.6",
            "netstandard2.0",
            "netstandard2.1",
            "sl4",
            "sl5",
            "tizen3",
            "tizen4",
            "uap",
            "uap10.0",
            "win",
            "win10",
            "win8",
            "win81",
            "winrt",
            "wp",
            "wp7",
            "wp75",
            "wp8",
            "wp81",
            "wpa81"
        });
    }
}
