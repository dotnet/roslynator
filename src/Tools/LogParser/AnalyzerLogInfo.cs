// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Diagnostics
{
    public readonly struct AnalyzerLogInfo
    {
        public AnalyzerLogInfo(string fullName, int elapsed, int percent)
        {
            Elapsed = elapsed;
            Percent = percent;
            FullName = fullName;
        }

        public string FullName { get; }

        public int Elapsed { get; }

        public int Percent { get; }
    }
}
