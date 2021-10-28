// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CommandLine
{
    internal class FormatCommandResult : CommandResult
    {
        public FormatCommandResult(CommandStatus status, int count)
            : base(status)
        {
            Count = count;
        }

        public int Count { get; }
    }
}
