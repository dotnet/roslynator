// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine;

[Verb("help", HelpText = "Displays help.")]
internal sealed class HelpCommandLineOptions : AbstractCommandLineOptions
{
    [Value(
        index: 0,
        HelpText = "Command name.",
        MetaName = "<COMMAND>")]
    public string Command { get; set; } = null!;

    [Option(
        shortName: OptionShortNames.Manual,
        longName: "manual",
        HelpText = "Display full manual.")]
    public bool Manual { get; set; }
}
