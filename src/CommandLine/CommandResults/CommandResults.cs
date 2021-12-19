// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CommandLine
{
    internal static class CommandResults
    {
        public static CommandResult Fail { get; } = new(CommandStatus.Fail);

        public static CommandResult Success { get; } = new(CommandStatus.Success);

        public static CommandResult NotSuccess { get; } = new(CommandStatus.NotSuccess);
    }
}
