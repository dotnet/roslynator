// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp
{
    /// <summary>
    /// Specifies C# preprocessor directives.
    /// </summary>
    [Flags]
    public enum PreprocessorDirectiveFilter
    {
        /// <summary>
        /// No preprocessor directive.
        /// </summary>
        None = 0,

        /// <summary>
        /// #if preprocessor directive.
        /// </summary>
        If = 1,

        /// <summary>
        /// #elif preprocessor directive.
        /// </summary>
        Elif = 1 << 1,

        /// <summary>
        /// #else preprocessor directive.
        /// </summary>
        Else = 1 << 2,

        /// <summary>
        /// #endif preprocessor directive.
        /// </summary>
        EndIf = 1 << 3,

        /// <summary>
        /// #region preprocessor directive.
        /// </summary>
        Region = 1 << 4,

        /// <summary>
        /// #endregion preprocessor directive.
        /// </summary>
        EndRegion = 1 << 5,

        /// <summary>
        /// #define preprocessor directive.
        /// </summary>
        Define = 1 << 6,

        /// <summary>
        /// #undef preprocessor directive.
        /// </summary>
        Undef = 1 << 7,

        /// <summary>
        /// #error preprocessor directive.
        /// </summary>
        Error = 1 << 8,

        /// <summary>
        /// #warning preprocessor directive.
        /// </summary>
        Warning = 1 << 9,

        /// <summary>
        /// #line preprocessor directive.
        /// </summary>
        Line = 1 << 10,

        /// <summary>
        /// #pragma warning preprocessor directive.
        /// </summary>
        PragmaWarning = 1 << 11,

        /// <summary>
        /// #pragma checksum preprocessor directive.
        /// </summary>
        PragmaChecksum = 1 << 12,

        /// <summary>
        /// #pragma preprocessor directive.
        /// </summary>
        Pragma = PragmaWarning | PragmaChecksum,

        /// <summary>
        /// #r preprocessor directive.
        /// </summary>
        Reference = 1 << 13,

        /// <summary>
        /// #load preprocessor directive.
        /// </summary>
        Load = 1 << 14,

        /// <summary>
        /// Bad preprocessor directive.
        /// </summary>
        Bad = 1 << 15,

        /// <summary>
        /// Shebang preprocessor directive.
        /// </summary>
        Shebang = 1 << 16,

        /// <summary>
        /// Nullable preprocessor directive.
        /// </summary>
        Nullable = 1 << 17,

        /// <summary>
        /// All preprocessor directives.
        /// </summary>
        All = If
            | Elif
            | Else
            | EndIf
            | Region
            | EndRegion
            | Define
            | Undef
            | Error
            | Warning
            | Line
            | PragmaWarning
            | PragmaChecksum
            | Reference
            | Load
            | Bad
            | Shebang
            | Nullable,
    }
}
