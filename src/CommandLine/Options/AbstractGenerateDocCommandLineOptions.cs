// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine
{
    public abstract class AbstractGenerateDocCommandLineOptions : MSBuildCommandLineOptions
    {
        [Value(
            index: 0,
            HelpText = "The project or solution file.",
            MetaName = "<PROJECT|SOLUTION>")]
        public string Path { get; set; }

        [Option(
            longName: "heading",
            Required = true,
            HelpText = "Defines a heading of the root documentation file.",
            MetaValue = "<HEADING>")]
        public string Heading { get; set; }

        [Option(
            shortName: OptionShortNames.Output,
            longName: "output",
            Required = true,
            HelpText = "Defines a path for the output directory.",
            MetaValue = "<DIRECTORY_PATH>")]
        public string Output { get; set; }

        [Option(
            longName: OptionNames.Depth,
            HelpText = "Defines a depth of a documentation. Allowed values are member (default), type or namespace.",
            MetaValue = "<DEPTH>")]
        public string Depth { get; set; }

        [Option(
            longName: "ignored-names",
            HelpText = "Defines a list of metadata names that should be excluded from a documentation. Namespace of type names can be specified.",
            MetaValue = "<FULLY_QUALIFIED_METADATA_NAME>")]
        public IEnumerable<string> IgnoredNames { get; set; }

        [Option(
            longName: "no-mark-obsolete",
            HelpText = "Indicates whether obsolete types and members should not be marked as '[deprecated]'.")]
        public bool NoMarkObsolete { get; set; }

        [Option(
            longName: "no-precedence-for-system",
            HelpText = "Indicates whether symbols contained in 'System' namespace should be ordered as any other symbols and not before other symbols.")]
        public bool NoPrecedenceForSystem { get; set; }

        [Option(
            longName: "scroll-to-content",
            HelpText = "Indicates whether a link should lead to the top of the documentation content.")]
        public bool ScrollToContent { get; set; }

        [Option(
            longName: OptionNames.Visibility,
            Default = nameof(Roslynator.Visibility.Public),
            HelpText = "Defines a visibility of a type or a member. Allowed values are public (default), internal or private.",
            MetaValue = "<VISIBILITY>")]
        public string Visibility { get; set; }
    }
}
