// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CommandLine
{
    internal static class Extensions
    {
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
