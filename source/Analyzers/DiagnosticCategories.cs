// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp
{
    public static class DiagnosticCategories
    {
        public const string General = "General";
        public const string FadeOut = "FadeOut";

#if DEBUG
        public const string Design = "Design";
        public const string Formatting = "Formatting";
        public const string Maintainability = "Maintainability";
        public const string Naming = "Naming";
        public const string Performance = "Performance";
        public const string Style = "Style";
        public const string Usage = "Usage";
#endif
    }
}
