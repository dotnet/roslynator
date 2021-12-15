// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("rename-symbol", HelpText = "Rename symbols in the specified project or solution.")]
    public class RenameSymbolCommandLineOptions : MSBuildCommandLineOptions
    {
        [Value(
            index: 0,
            HelpText = "Path to one or more project/solution files.",
            MetaName = "<PROJECT|SOLUTION>")]
        public IEnumerable<string> Paths { get; set; }

        [Option(
            longName: OptionNames.Ask,
            HelpText = "Ask whether to rename a symbol.")]
        public bool Ask { get; set; }
#if DEBUG
        [Option(
            longName: OptionNames.CodeContext,
            HelpText = "Number of lines to display before and after a line with symbol definition.",
            MetaValue = "<NUM>",
            Default = -1)]
        public int CodeContext { get; set; }
#endif
        [Option(
            shortName: OptionShortNames.DryRun,
            longName: "dry-run",
            HelpText = "List symbols to be renamed but do not save changes to a disk.")]
        public bool DryRun { get; set; }
#if DEBUG
        [Option(
            longName: OptionNames.IgnoredCompilerDiagnostics,
            HelpText = "A list of compiler diagnostics that should be ignored.")]
        public IEnumerable<string> IgnoredCompilerDiagnostics { get; set; }
#endif
        [Option(
            shortName: OptionShortNames.IncludeGeneratedCode,
            longName: "include-generated-code",
            HelpText = "Include symbols that are part of generated code.")]
        public bool IncludeGeneratedCode { get; set; }

        [Option(
            longName: OptionNames.Interactive,
            HelpText = "Enable editing of a new name.")]
        public bool Interactive { get; set; }

        [Option(
            longName: OptionNames.Match,
            HelpText = "C# expression that can be used as a expression body of a method 'bool M(ISymbol symbol)'.",
            MetaValue = "<EXPRESSION>")]
        public string Match { get; set; }

        [Option(
            longName: OptionNames.MatchFrom,
            HelpText = "Path to a C# code file that contains publicly accessible method with signature 'bool M(ISymbol symbol)'.",
            MetaValue = "<FILE_PATH>")]
        public string MatchFrom { get; set; }

        [Option(
            longName: OptionNames.OnError,
            HelpText = "Action to choose when renaming of a symbol causes compilation error. Allowed values are abort, ask, fix, list and skip.",
            MetaValue = "<ERROR_RESOLUTION>")]
        public string OnError { get; set; }

        [Option(
            longName: OptionNames.NewName,
            HelpText = "C# expression that can be used as a expression body of a method 'string M(ISymbol symbol)'",
            MetaValue = "<EXPRESSION>")]
        public string NewName { get; set; }

        [Option(
            longName: OptionNames.NewNameFrom,
            HelpText = "Path to a C# code file that contains publicly accessible method with signature 'string M(ISymbol symbol)'",
            MetaValue = "<FILE_PATH>")]
        public string NewNameFrom { get; set; }

        [Option(
            longName: OptionNames.Scope,
            HelpText = "Symbol groups to be included. Allowed values are type, member and local.",
            MetaValue = "<SCOPE>")]
        public IEnumerable<string> Scope { get; set; }
#if DEBUG
        [Option(
            longName: OptionNames.Visibility,
            Default = nameof(Roslynator.Visibility.Public),
            HelpText = "Defines a  maximal visibility of a symbol to be renamed. Allowed values are public (default), internal or private.",
            MetaValue = "<VISIBILITY>")]
        public string Visibility { get; set; }
#endif
    }
}
