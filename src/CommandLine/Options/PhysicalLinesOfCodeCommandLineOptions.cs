// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
    [Verb("loc", HelpText = "Counts physical lines of code in the specified project or solution.")]
    public class PhysicalLinesOfCodeCommandLineOptions : AbstractLinesOfCodeCommandLineOptions
    {
        [Option(longName: "ignore-block-boundary",
            HelpText = "Indicates whether a line that contains only block boundary should not be counted.")]
        public bool IgnoreBlockBoundary { get; set; }

        [Option(longName: "include-comments",
            HelpText = "Indicates whether a line that contains only comment should be counted.")]
        public bool IncludeComments { get; set; }

        [Option(longName: "include-preprocessor-directives",
            HelpText = "Indicates whether preprocessor directive line should be counted.")]
        public bool IncludePreprocessorDirectives { get; set; }

        [Option(longName: "include-whitespace",
            HelpText = "Indicates whether white-space line should be counted.")]
        public bool IncludeWhitespace { get; set; }
    }
}
