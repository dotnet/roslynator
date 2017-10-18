// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp
{
    public static class DiagnosticCategories
    {
        public const string Design = "Design";
        public const string FadeOut = "FadeOut";
        public const string Formatting = "Formatting";
        public const string General = "General";
        public const string Maintainability = "Maintainability";
        public const string Naming = "Naming";
        public const string Performance = "Performance";
        public const string Readability = "Readability";
        public const string Redundancy = "Redundancy";
        public const string Reliability = "Reliability";
        public const string Simplification = "Simplification";
        public const string Style = "Style";
        public const string Usage = "Usage";

        [Obsolete]
        public const string ErrorFix = "ErrorFix";

        //public const string Compatibility = "Compatibility";
        //public const string Correctness = "Correctness";
        //public const string Globalization = "Globalization";
        //public const string Security = "Security";
    }
}
