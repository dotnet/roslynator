// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CommandLine
{
    internal class ProjectOrSolutionLoadException : Exception
    {
        public ProjectOrSolutionLoadException()
        {
        }

        public ProjectOrSolutionLoadException(string message) : base(message)
        {
        }

        public ProjectOrSolutionLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
