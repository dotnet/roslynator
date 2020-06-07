// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    internal readonly struct LogMessage
    {
        public LogMessage(string text)
            : this(text, foregroundColor: default)
        {
        }

        public LogMessage(string text, ConsoleColor? foregroundColor)
            : this(text, foregroundColor: foregroundColor, verbosity: default)
        {
        }

        public LogMessage(string text, Verbosity verbosity)
            : this(text, foregroundColor: default, verbosity: verbosity)
        {
        }

        public LogMessage(string text, ConsoleColor? foregroundColor, Verbosity verbosity)
        {
            Text = text;
            ForegroundColor = foregroundColor;
            Verbosity = verbosity;
        }

        public string Text { get; }

        public ConsoleColor? ForegroundColor { get; }

        public Verbosity Verbosity { get; }
    }
}
