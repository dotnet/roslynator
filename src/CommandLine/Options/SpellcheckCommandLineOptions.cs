// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("spellcheck", HelpText = "Searches the specified project or solution for possible misspellings or typos.")]
    public sealed class SpellcheckCommandLineOptions : MSBuildCommandLineOptions
    {
        [Value(
            index: 0,
            HelpText = "Path to one or more project/solution files.",
            MetaName = "<PROJECT|SOLUTION>")]
        public IEnumerable<string> Paths { get; set; }

        [Option(
            longName: "case-sensitive",
            HelpText = "Specifies case-sensitive matching.")]
        public bool CaseSensitive { get; set; }

        [Option(
            longName: "culture",
            HelpText = "Defines culture that should be used to display diagnostic message.",
            MetaValue = "<CULTURE_ID>")]
        public string Culture { get; set; }

        [Option(
            shortName: OptionShortNames.DryRun,
            longName: "dry-run",
            HelpText = "Display misspellings and typos but do not save changes to a disk.")]
        public bool DryRun { get; set; }

        [Option(
            longName: OptionNames.IgnoredScope,
            HelpText = "Defines syntax that should not be analyzed. Allowed values are comment, type, member, local, parameter, literal, non-symbol and symbol.",
            MetaValue = "<SCOPE>")]
        public IEnumerable<string> IgnoredScope { get; set; }

        [Option(
            longName: "include-generated-code",
            HelpText = "Indicates whether generated code should be spellchecked.")]
        public bool IncludeGeneratedCode { get; set; }

        [Option(
            longName: "interactive",
            HelpText = "Enable editing of a replacement.")]
        public bool Interactive { get; set; }

        [Option(
            longName: "max-word-length",
            Default = int.MaxValue,
            HelpText = "Specifies maximal word length to be checked.",
            MetaValue = "<NUM>")]
        public int MaxWordLength { get; set; }

        [Option(
            longName: "min-word-length",
            Default = 3,
            HelpText = "Specifies minimal word length to be checked. Default value is 3.",
            MetaValue = "<NUM>")]
        public int MinWordLength { get; set; }
#if DEBUG
        [Option(
            longName: OptionNames.NoAutofix,
            HelpText = "Disable applying predefined fixes.")]
        public bool NoAutofix { get; set; }
#endif
        [Option(
            longName: OptionNames.Scope,
            HelpText = "Defines syntax that should be analyzed. Allowed values are comment, type, member, local, parameter, literal, non-symbol, symbol and all. Literals are not analyzed by default.",
            MetaValue = "<SCOPE>")]
        public IEnumerable<string> Scope { get; set; }

        [Option(
            longName: OptionNames.Visibility,
            Default = nameof(Roslynator.Visibility.Public),
            HelpText = "Defines a  maximal visibility of a symbol to be fixable. Allowed values are public (default), internal or private.",
            MetaValue = "<VISIBILITY>")]
        public string Visibility { get; set; }

        [Option(
            longName: "words",
            Required = true,
            HelpText = "Specified path to file and/or directory that contains list of allowed words.",
            MetaValue = "<PATH>")]
        public IEnumerable<string> Words { get; set; } = null!;
    }
}
