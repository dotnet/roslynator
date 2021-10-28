// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CommandLine
{
    internal abstract class AbstractLinesOfCodeCommand<TResult> : MSBuildWorkspaceCommand<LinesOfCodeCommandResult>
    {
        protected AbstractLinesOfCodeCommand(in ProjectFilter projectFilter) : base(projectFilter)
        {
        }
    }
}
