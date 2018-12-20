// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    internal sealed class ConsoleWriter : TextWriterWithVerbosity
    {
        public static ConsoleWriter Instance { get; } = new ConsoleWriter();

        private ConsoleWriter() : base(Console.Out, Console.Out.FormatProvider)
        {
        }

        public void Write(string value, ConsoleColor color)
        {
            ConsoleColor tmp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Write(value);
            Console.ForegroundColor = tmp;
        }

        public void Write(string value, ConsoleColor color, Verbosity verbosity)
        {
            WriteIf(verbosity <= Verbosity, value, color);
        }

        public void WriteIf(bool condition, string value, ConsoleColor color)
        {
            if (condition)
                Write(value, color);
        }

        public void WriteLine(string value, ConsoleColor color)
        {
            ConsoleColor tmp = Console.ForegroundColor;
            Console.ForegroundColor = color;
            WriteLine(value);
            Console.ForegroundColor = tmp;
        }

        public void WriteLine(string value, ConsoleColor color, Verbosity verbosity)
        {
            WriteLineIf(verbosity <= Verbosity, value, color);
        }

        public void WriteLineIf(bool condition, string value, ConsoleColor color)
        {
            if (condition)
                WriteLine(value, color);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
