// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    internal sealed class ConsoleWriter : TextWriterWithVerbosity
    {
        public static ConsoleWriter Instance { get; } = new();

        private ConsoleWriter() : base(Console.Out, Console.Out.FormatProvider)
        {
        }

        public ConsoleColors Colors
        {
            get { return new ConsoleColors(Console.ForegroundColor, Console.BackgroundColor); }
            set
            {
                if (value.Foreground != null)
                    Console.ForegroundColor = value.Foreground.Value;

                if (value.Background != null)
                    Console.BackgroundColor = value.Background.Value;
            }
        }

        public void Write(string value, ConsoleColors colors)
        {
            if (!colors.IsDefault)
            {
                ConsoleColors tmp = Colors;
                Colors = colors;
                Write(value);
                Colors = tmp;
            }
            else
            {
                Write(value);
            }
        }

        public void Write(string value, ConsoleColors colors, Verbosity verbosity)
        {
            WriteIf(ShouldWrite(verbosity), value, colors);
        }

        public void WriteIf(bool condition, string value, ConsoleColors colors)
        {
            if (condition)
                Write(value, colors);
        }

        public void WriteLine(string value, ConsoleColors colors)
        {
            if (!colors.IsDefault)
            {
                ConsoleColors tmp = Colors;
                Colors = colors;
                Write(value);
                Colors = tmp;
                WriteLine();
            }
            else
            {
                WriteLine(value);
            }
        }

        public void WriteLine(string value, ConsoleColors colors, Verbosity verbosity)
        {
            WriteLineIf(ShouldWrite(verbosity), value, colors);
        }

        public override void WriteLine(LogMessage message)
        {
            if (message.Colors != null)
            {
                WriteLineIf(ShouldWrite(message.Verbosity), message.Text, message.Colors.Value);
            }
            else
            {
                base.WriteLine(message);
            }
        }

        public void WriteLineIf(bool condition, string value, ConsoleColors colors)
        {
            if (condition)
                WriteLine(value, colors);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
