// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator.CodeFixes
{
    internal static class ConsoleHelpers
    {
        public static void Write(string message = null)
        {
            WriteIf(true, message);
        }

        public static void WriteIf(bool condition, string message = null)
        {
            if (condition)
            {
                Console.Write(message);
            }
        }

        public static void Write(string message, ConsoleColor color)
        {
            WriteIf(true, message, color);
        }

        public static void WriteIf(bool condition, string message, ConsoleColor color)
        {
            if (condition)
            {
                Console.ForegroundColor = color;
                Console.Write(message);
                Console.ResetColor();
            }
        }

        public static void WriteLine(string message = null)
        {
            WriteLineIf(true, message);
        }

        public static void WriteLineIf(bool condition, string message = null)
        {
            if (condition)
            {
                Console.WriteLine(message);
            }
        }

        public static void WriteLine(string message, ConsoleColor color)
        {
            WriteLineIf(true, message, color);
        }

        public static void WriteLineIf(bool condition, string message, ConsoleColor color)
        {
            if (condition)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        public static void Write(IEnumerable<Diagnostic> diagnostics, int max, ConsoleColor color)
        {
            using (IEnumerator<Diagnostic> en = diagnostics.GetEnumerator())
            {
                int count = 0;

                while (en.MoveNext())
                {
                    count++;

                    if (count <= max)
                    {
                        WriteLine(en.Current.ToString(), color);
                    }
                    else
                    {
                        count = 0;

                        while (en.MoveNext())
                            count++;

                        WriteLine($"and {count} more diagnostics", color);
                    }
                }
            }
        }
    }
}
