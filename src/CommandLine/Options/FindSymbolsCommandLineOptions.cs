// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine;

[Verb("find-symbols", HelpText = "Finds symbols in the specified project or solution.")]
public class FindSymbolsCommandLineOptions : MSBuildCommandLineOptions
{
    [Value(
        index: 0,
        HelpText = "Path to one or more project/solution files.",
        MetaName = "<PROJECT|SOLUTION>")]
    public IEnumerable<string> Paths { get; set; }

    [Option(
        longName: "ignored-symbols",
        HelpText = "Defines a list of symbols that should be ignored. Namespace of types can be specified.",
        MetaValue = "<FULLY_QUALIFIED_METADATA_NAME>")]
    public IEnumerable<string> IgnoredSymbols { get; set; }

    [Option(longName: "ignore-generated-code")]
    public bool IgnoreGeneratedCode { get; set; }

    [Option(longName: OptionNames.SymbolKind)]
    public IEnumerable<string> SymbolKind { get; set; }

    [Option(longName: "unused")]
    public bool Unused { get; set; }

    [Option(longName: OptionNames.Visibility)]
    public IEnumerable<string> Visibility { get; set; }

    [Option(longName: "with-attributes")]
    public IEnumerable<string> WithAttributes { get; set; }

    [Option(longName: "without-attributes")]
    public IEnumerable<string> WithoutAttributes { get; set; }

    [Option(longName: "with-modifiers")]
    public IEnumerable<string> WithModifiers { get; set; }

    [Option(longName: "without-modifiers")]
    public IEnumerable<string> WithoutModifiers { get; set; }
}
