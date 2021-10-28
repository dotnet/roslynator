// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    internal static class Logger
    {
        public static ConsoleWriter ConsoleOut { get; } = ConsoleWriter.Instance;

        public static TextWriterWithVerbosity Out { get; set; }

        public static void Write(char value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(char value, int repeatCount)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                ConsoleOut.Write(value);
                Out?.Write(value);
            }
        }

        public static void Write(char value, int repeatCount, Verbosity verbosity)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                ConsoleOut.Write(value, verbosity);
                Out?.Write(value, verbosity);
            }
        }

        public static void Write(char[] buffer)
        {
            ConsoleOut.Write(buffer);
            Out?.Write(buffer);
        }

        public static void Write(char[] buffer, int index, int count)
        {
            ConsoleOut.Write(buffer, index, count);
            Out?.Write(buffer, index, count);
        }

        public static void Write(bool value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(int value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(uint value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(long value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(ulong value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(float value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(double value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(decimal value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(string value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void Write(string value, Verbosity verbosity)
        {
            ConsoleOut.Write(value, verbosity: verbosity);
            Out?.Write(value, verbosity: verbosity);
        }

        public static void Write(string value, ConsoleColors colors)
        {
            ConsoleOut.Write(value, colors);
            Out?.Write(value);
        }

        public static void Write(string value, ConsoleColors colors, Verbosity verbosity)
        {
            ConsoleOut.Write(value, colors, verbosity: verbosity);
            Out?.Write(value, verbosity: verbosity);
        }

        public static void WriteIf(bool condition, string value)
        {
            ConsoleOut.WriteIf(condition, value);
            Out?.WriteIf(condition, value);
        }

        public static void WriteIf(bool condition, string value, ConsoleColors colors)
        {
            ConsoleOut.WriteIf(condition, value, colors);
            Out?.WriteIf(condition, value);
        }

        public static void Write(object value)
        {
            ConsoleOut.Write(value);
            Out?.Write(value);
        }

        public static void WriteLine()
        {
            ConsoleOut.WriteLine();
            Out?.WriteLine();
        }

        public static void WriteLine(Verbosity verbosity)
        {
            ConsoleOut.WriteLine(verbosity);
            Out?.WriteLine(verbosity);
        }

        public static void WriteLineIf(bool condition)
        {
            ConsoleOut.WriteLineIf(condition);
            Out?.WriteLineIf(condition);
        }

        public static void WriteLine(char value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(char[] buffer)
        {
            ConsoleOut.WriteLine(buffer);
            Out?.WriteLine(buffer);
        }

        public static void WriteLine(char[] buffer, int index, int count)
        {
            ConsoleOut.WriteLine(buffer, index, count);
            Out?.WriteLine(buffer, index, count);
        }

        public static void WriteLine(bool value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(int value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(uint value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(long value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(ulong value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(float value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(double value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(decimal value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(string value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteLine(string value, Verbosity verbosity)
        {
            ConsoleOut.WriteLine(value, verbosity: verbosity);
            Out?.WriteLine(value, verbosity: verbosity);
        }

        public static void WriteLine(string value, ConsoleColors colors)
        {
            ConsoleOut.WriteLine(value, colors);
            Out?.WriteLine(value);
        }

        public static void WriteLine(string value, ConsoleColors colors, Verbosity verbosity)
        {
            ConsoleOut.WriteLine(value, colors, verbosity: verbosity);
            Out?.WriteLine(value, verbosity: verbosity);
        }

        public static void WriteLine(LogMessage message)
        {
            ConsoleOut.WriteLine(message);
            Out?.WriteLine(message);
        }

        public static void WriteLineIf(bool condition, string value)
        {
            ConsoleOut.WriteLineIf(condition, value);
            Out?.WriteLineIf(condition, value);
        }

        public static void WriteLineIf(bool condition, string value, ConsoleColors colors)
        {
            ConsoleOut.WriteLineIf(condition, value, colors);
            Out?.WriteLineIf(condition, value);
        }

        public static void WriteLine(object value)
        {
            ConsoleOut.WriteLine(value);
            Out?.WriteLine(value);
        }

        public static void WriteError(
            Exception exception,
            ConsoleColor color = ConsoleColor.Red,
            Verbosity verbosity = Verbosity.Quiet)
        {
            WriteError(exception, exception.Message, color, verbosity);
        }

        public static void WriteError(
            Exception exception,
            string message,
            ConsoleColor color = ConsoleColor.Red,
            Verbosity verbosity = Verbosity.Quiet)
        {
            var colors = new ConsoleColors(color);

            WriteLine(exception.Message, colors, verbosity);

            if (exception is AggregateException aggregateException)
                WriteInnerExceptions(aggregateException, "");
#if DEBUG
            WriteLine(exception.ToString());
#endif
            void WriteInnerExceptions(AggregateException aggregateException, string indent)
            {
                indent += "  ";

                foreach (Exception innerException in aggregateException.InnerExceptions)
                {
                    WriteLine(indent + "Inner exception: " + innerException.Message, colors, verbosity);

                    if (innerException is AggregateException aggregateException2)
                        WriteInnerExceptions(aggregateException2, indent);
                }

                indent = indent.Substring(2);
            }
        }

        public static bool ShouldWrite(Verbosity verbosity)
        {
            return verbosity <= ConsoleOut.Verbosity
                || (Out != null && verbosity <= Out.Verbosity);
        }
    }
}
