// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CommandLine
{
    internal static class CommandLineHelpers
    {
        public static void WaitForKeyPress(string message = null)
        {
            if (Console.IsInputRedirected)
                return;

            Console.Write(message ?? "Press any key to continue...");
            Console.ReadKey();
            Console.WriteLine();
        }
    }
}
