// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    /// <summary>
    /// A set of extension methods for <see cref="FileLinePositionSpan"/>.
    /// </summary>
    public static class FileLinePositionSpanExtensions
    {
        /// <summary>
        /// Returns zero-based index of the start line of the specified span.
        /// </summary>
        /// <param name="fileLinePositionSpan"></param>
        /// <returns></returns>
        public static int StartLine(this FileLinePositionSpan fileLinePositionSpan)
        {
            return fileLinePositionSpan.StartLinePosition.Line;
        }

        /// <summary>
        /// Returns zero-based index of the end line of the specified span.
        /// </summary>
        /// <param name="fileLinePositionSpan"></param>
        /// <returns></returns>
        public static int EndLine(this FileLinePositionSpan fileLinePositionSpan)
        {
            return fileLinePositionSpan.EndLinePosition.Line;
        }

        /// <summary>
        /// Returns true if the specified <see cref="FileLinePositionSpan"/> spans over multiple lines.
        /// </summary>
        /// <param name="fileLinePositionSpan"></param>
        /// <returns></returns>
        public static bool IsMultiLine(this FileLinePositionSpan fileLinePositionSpan)
        {
            return fileLinePositionSpan.StartLine() != fileLinePositionSpan.EndLine();
        }

        /// <summary>
        /// Returns true if the specified <see cref="FileLinePositionSpan"/> does not span over multiple lines.
        /// </summary>
        /// <param name="fileLinePositionSpan"></param>
        /// <returns></returns>
        public static bool IsSingleLine(this FileLinePositionSpan fileLinePositionSpan)
        {
            return fileLinePositionSpan.StartLine() == fileLinePositionSpan.EndLine();
        }

        internal static int GetLineCount(this FileLinePositionSpan fileLinePositionSpan)
        {
            return fileLinePositionSpan.EndLine() - fileLinePositionSpan.StartLine() + 1;
        }
    }
}
