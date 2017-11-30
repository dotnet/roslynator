// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Metadata
{
    public class SampleDescriptor
    {
        public SampleDescriptor(string before, string after)
        {
            Before = before;
            After = after;
        }

        public string Before { get; }
        public string After { get; }
    }
}
