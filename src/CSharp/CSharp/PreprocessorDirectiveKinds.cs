// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp
{
    /// <summary>
    /// Specifies C# preprocessor directives.
    /// </summary>
    [Flags]
    public enum PreprocessorDirectiveKinds
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
        Elif = 2,

        /// <summary>
        /// #else preprocessor directive.
        /// </summary>
        Else = 4,

        /// <summary>
        /// #endif preprocessor directive.
        /// </summary>
        EndIf = 8,

        /// <summary>
        /// #region preprocessor directive.
        /// </summary>
        Region = 16,

        /// <summary>
        /// #endregion preprocessor directive.
        /// </summary>
        EndRegion = 32,

        /// <summary>
        /// #define preprocessor directive.
        /// </summary>
        Define = 64,

        /// <summary>
        /// #undef preprocessor directive.
        /// </summary>
        Undef = 128,

        /// <summary>
        /// #error preprocessor directive.
        /// </summary>
        Error = 256,

        /// <summary>
        /// #warning preprocessor directive.
        /// </summary>
        Warning = 512,

        /// <summary>
        /// #line preprocessor directive.
        /// </summary>
        Line = 1024,

        /// <summary>
        /// #pragma warning preprocessor directive.
        /// </summary>
        PragmaWarning = 2048,

        /// <summary>
        /// #pragma checksum preprocessor directive.
        /// </summary>
        PragmaChecksum = 4096,

        /// <summary>
        /// #pragma preprocessor directive.
        /// </summary>
        Pragma = PragmaWarning | PragmaChecksum,

        /// <summary>
        /// #r preprocessor directive.
        /// </summary>
        Reference = 8192,

        /// <summary>
        /// #load preprocessor directive.
        /// </summary>
        Load = 16384,

        /// <summary>
        /// Bad preprocessor directive.
        /// </summary>
        Bad = 32768,

        /// <summary>
        /// Shebang preprocessor directive.
        /// </summary>
        Shebang = 65536,

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
            | Shebang,
    }
}
