// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
#if DEBUG
    [Verb("generate-source-references")]
#endif
    public class GenerateSourceReferencesCommandLineOptions : MSBuildCommandLineOptions
    {
        [Value(
            index: 0,
            HelpText = "The project or solution file.",
            MetaName = "<PROJECT|SOLUTION>")]
        public string Path { get; set; }

        [Option(
            longName: "output",
            Required = true,
            HelpText = "Defines a path for the output file.",
            MetaValue = "<OUTPUT_FILE>")]
        public string Output { get; set; }

        [Option(
            longName: "repository-url",
            Required = true)]
        public string RepositoryUrl { get; set; }

        [Option(
            longName: "root-path",
            Required = true)]
        public string RootPath { get; set; }

        [Option(
            longName: "version",
            Required = true)]
        public string Version { get; set; }

        [Option(
            longName: "branch",
            Default = "master")]
        public string Branch { get; set; }

        [Option(longName: "commit")]
        public string Commit { get; set; }

        [Option(
            longName: OptionNames.Depth,
            HelpText = "Defines a depth of a documentation. Allowed values are member (default), type or namespace.",
            MetaValue = "<DEPTH>")]
        public string Depth { get; set; }

        [Option(
            longName: "repository-type",
            Default = "git")]
        public string RepositoryType { get; set; }

        [Option(
            longName: OptionNames.Visibility,
            Default = nameof(Roslynator.Visibility.Public),
            HelpText = "Defines a visibility of a type or a member. Allowed values are public (default), internal or private.",
            MetaValue = "<VISIBILITY>")]
        public string Visibility { get; set; }
    }
}
