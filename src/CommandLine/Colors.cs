// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CommandLine
{
    internal static class Colors
    {
        public static ConsoleColors Message_OK { get; } = ConsoleColors.Green;
        public static ConsoleColors Message_DryRun { get; } = ConsoleColors.DarkGray;
        public static ConsoleColors Message_Warning { get; } = ConsoleColors.Yellow;
    }
}
