// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace Roslynator
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct ConsoleColors : IEquatable<ConsoleColors>
    {
        public static ConsoleColors Cyan { get; } = new(ConsoleColor.Cyan);

        public static ConsoleColors DarkGray { get; } = new(ConsoleColor.DarkGray);

        public static ConsoleColors Gray { get; } = new(ConsoleColor.Gray);

        public static ConsoleColors Green { get; } = new(ConsoleColor.Green);

        public static ConsoleColors Magenta { get; } = new(ConsoleColor.Magenta);

        public static ConsoleColors Yellow { get; } = new(ConsoleColor.Yellow);

        public ConsoleColors(ConsoleColor? foreground)
            : this(foreground, null)
        {
        }

        public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
        {
            Foreground = foreground;
            Background = background;
        }

        public ConsoleColor? Foreground { get; }

        public ConsoleColor? Background { get; }

        public bool IsDefault => Foreground == null && Background == null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return $"{((Foreground != null) ? Foreground.ToString() : "None")}  "
                    + $"{((Background != null) ? Background.ToString() : "None")}";
            }
        }

        public override bool Equals(object obj)
        {
            return obj is ConsoleColors colors
                && Equals(colors);
        }

        public bool Equals(ConsoleColors other)
        {
            return Foreground == other.Foreground
                && Background == other.Background;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Foreground?.GetHashCode() ?? -1, Background?.GetHashCode() ?? -1);
        }

        public static bool operator ==(ConsoleColors left, ConsoleColors right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ConsoleColors left, ConsoleColors right)
        {
            return !(left == right);
        }
    }
}
