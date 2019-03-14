// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CommandLine
{
    internal static class Extensions
    {
        public static bool ContainsProject(this Solution solution, string name)
        {
            foreach (Project project in solution.Projects)
            {
                if (project.Name == name)
                    return true;
            }

            return false;
        }
    }
}
