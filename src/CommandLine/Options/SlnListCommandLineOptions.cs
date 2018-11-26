// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
#if DEBUG
    [Verb("sln-list", HelpText = "Gets an information about specified solution and its projects.")]
#endif
    public class SlnListCommandLineOptions : MSBuildCommandLineOptions
    {
    }
}
