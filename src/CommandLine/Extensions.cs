// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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

        public static VisibilityFilter ToVisibilityFilter(this Visibility visibility)
        {
            switch (visibility)
            {
                case Visibility.NotApplicable:
                    return VisibilityFilter.None;
                case Visibility.Private:
                    return VisibilityFilter.Private;
                case Visibility.Internal:
                    return VisibilityFilter.Internal;
                case Visibility.Public:
                    return VisibilityFilter.Public;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
