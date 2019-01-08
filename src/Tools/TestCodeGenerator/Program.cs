// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;

namespace Roslynator.CodeGeneration
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string dirPath = args[0];

            var generator = new TestCodeGenerator(dirPath, StringComparer.InvariantCulture);

            generator.Generate(args.Skip(1));
        }
    }
}
