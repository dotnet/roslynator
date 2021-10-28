// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;

namespace Roslynator.CommandLine
{
#if DEBUG
    [Verb("analyze-assembly", HelpText = "Searches file or directory for analyzer assemblies.")]
#endif
    public class AnalyzeAssemblyCommandLineOptions : AbstractCommandLineOptions
    {
        [Value(
            index: 0,
            Required = true,
            HelpText = "The path to file or directory to analyze.",
            MetaName = "<PATH>")]
        public string Path { get; set; }

        [Option(longName: "additional-paths")]
        public IEnumerable<string> AdditionalPaths { get; set; }

        [Option(longName: "culture")]
        public string Culture { get; set; }

        [Option(longName: "file-name-pattern")]
        public string FileNamePattern { get; set; }

        [Option(longName: "language")]
        public string Language { get; set; }

        [Option(longName: "no-analyzers")]
        public bool NoAnalyzers { get; set; }

        [Option(longName: "no-fixers")]
        public bool NoFixers { get; set; }

        [Option(longName: "output")]
        public IEnumerable<string> Output { get; set; }

        internal IEnumerable<string> GetPaths()
        {
            yield return Path;

            foreach (string path in AdditionalPaths)
                yield return path;
        }
    }
}
