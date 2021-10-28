// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    internal readonly struct LogMessage
    {
        public LogMessage(string text)
            : this(text, colors: default)
        {
        }

        public LogMessage(string text, ConsoleColors? colors)
            : this(text, colors: colors, verbosity: default)
        {
        }

        public LogMessage(string text, Verbosity verbosity)
            : this(text, colors: default, verbosity: verbosity)
        {
        }

        public LogMessage(string text, ConsoleColors? colors, Verbosity verbosity)
        {
            Text = text;
            Colors = colors;
            Verbosity = verbosity;
        }

        public string Text { get; }

        public ConsoleColors? Colors { get; }

        public Verbosity Verbosity { get; }
    }
}
