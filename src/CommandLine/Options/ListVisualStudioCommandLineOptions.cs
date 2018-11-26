// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using CommandLine;

namespace Roslynator.CommandLine
{
#if DEBUG
    [Verb("list-vs", HelpText = "Lists Visual Studio installations.")]
#endif
    public class ListVisualStudioCommandLineOptions : AbstractCommandLineOptions
    {
    }
}
