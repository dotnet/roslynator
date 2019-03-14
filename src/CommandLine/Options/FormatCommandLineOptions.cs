// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using CommandLine;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp;

namespace Roslynator.CommandLine
{
    [Verb("format", HelpText = "Formats documents in the specified project or solution.")]
    public class FormatCommandLineOptions : MSBuildCommandLineOptions
    {
        [Option(longName: "culture",
            HelpText = "Defines culture that should be used to display diagnostic message.",
            MetaValue = "<CULTURE_ID>")]
        public string Culture { get; set; }

        [Option(longName: "empty-line-after-closing-brace",
            HelpText = "Indicates whether a closing brace should be followed with empty line.")]
        public bool EmptyLineAfterClosingBrace { get; set; }

        [Option(longName: "empty-line-after-embedded-statement",
            HelpText = "Indicates whether an embedded statement should be followed with empty line.")]
        public bool EmptyLineAfterEmbeddedStatement { get; set; }

        [Option(longName: "empty-line-before-while-in-do-statement",
            HelpText = "Indicates whether while keyword in 'do' statement should be preceded with empty line.")]
        public bool EmptyLineBeforeWhileInDoStatement { get; set; }

        [Option(longName: "empty-line-between-declarations",
            HelpText = "Indicates whether member declarations should be separated with empty line.")]
        public bool EmptyLineBetweenDeclarations { get; set; }

        [Option(longName: "end-of-line",
            HelpText = "Defines end of line character(s). Allowed values are lf or crlf.",
            MetaValue = "<END_OF_LINE>")]
        public string EndOfLine { get; set; }

        [Option(longName: "format-accessor-list",
            HelpText = "Indicates whether access list should be formatted.")]
        public bool FormatAccessorList { get; set; }

        [Option(longName: "include-generated-code",
            HelpText = "Indicates whether generated code should be formatted.")]
        public bool IncludeGeneratedCode { get; set; }

        [Option(longName: "new-line-after-switch-label",
            HelpText = "Indicates whether switch label should be followed with new line.")]
        public bool NewLineAfterSwitchLabel { get; set; }

        [Option(longName: "new-line-before-binary-operator",
            HelpText = "Indicates whether a binary operator should be preceded with new line.")]
        public bool NewLineBeforeBinaryOperator { get; set; }

        [Option(longName: "new-line-before-closing-brace-in-block",
            HelpText = "Indicates whether closing brace in a block should be preceded with new line.")]
        public bool NewLineBeforeClosingBraceInBlock { get; set; }

        [Option(longName: "new-line-before-closing-brace-in-empty-block",
            HelpText = "Indicates whether closing brace in an empty block should be preceded with new line.")]
        public bool NewLineBeforeClosingBraceInEmptyBlock { get; set; }

        [Option(longName: "new-line-before-closing-brace-in-empty-declaration",
            HelpText = "Indicates whether closing brace in a type declaration should be preceded with new line.")]
        public bool NewLineBeforeClosingBraceInEmptyDeclaration { get; set; }

        [Option(longName: "new-line-before-conditional-expression-operator",
            HelpText = "Indicates whether operator in a conditional expression be preceded with new line.")]
        public bool NewLineBeforeConditionalExpressionOperator { get; set; }

        [Option(longName: "new-line-before-embedded-statement",
            HelpText = "Indicates whether an embedded statement should be preceded with new line.")]
        public bool NewLineBeforeEmbeddedStatement { get; set; }

        [Option(longName: "new-line-before-enum-member",
            HelpText = "Indicates whether an enum member declaration should be preceded with new line.")]
        public bool NewLineBeforeEnumMember { get; set; }

        [Option(longName: "new-line-before-statement",
            HelpText = "Indicates whether a statement should be preceded with new line.")]
        public bool NewLineBeforeStatement { get; set; }

        [Option(longName: "remove-redundant-empty-line",
            HelpText = "Indicates whether redundant empty lines should be removed.")]
        public bool RemoveRedundantEmptyLine { get; set; }

        internal IEnumerable<DiagnosticDescriptor> GetSupportedDiagnostics()
        {
            if (EmptyLineAfterClosingBrace)
                yield return DiagnosticDescriptors.AddEmptyLineAfterClosingBrace;

            if (EmptyLineAfterEmbeddedStatement)
                yield return DiagnosticDescriptors.AddEmptyLineAfterEmbeddedStatement;

            if (EmptyLineBeforeWhileInDoStatement)
                yield return DiagnosticDescriptors.AddEmptyLineBeforeWhileInDoStatement;

            if (EmptyLineBetweenDeclarations)
                yield return DiagnosticDescriptors.AddEmptyLineBetweenDeclarations;

            if (FormatAccessorList)
                yield return DiagnosticDescriptors.FormatAccessorList;

            if (NewLineAfterSwitchLabel)
                yield return DiagnosticDescriptors.AddNewLineAfterSwitchLabel;

            if (NewLineBeforeBinaryOperator)
                yield return DiagnosticDescriptors.FormatBinaryOperatorOnNextLine;

            if (NewLineBeforeClosingBraceInBlock)
                yield return DiagnosticDescriptors.FormatSingleLineBlock;

            if (NewLineBeforeClosingBraceInEmptyBlock)
                yield return DiagnosticDescriptors.FormatEmptyBlock;

            if (NewLineBeforeClosingBraceInEmptyDeclaration)
                yield return DiagnosticDescriptors.FormatDeclarationBraces;

            if (NewLineBeforeConditionalExpressionOperator)
                yield return DiagnosticDescriptors.FormatConditionalExpression;

            if (NewLineBeforeEmbeddedStatement)
                yield return DiagnosticDescriptors.AddNewLineBeforeEmbeddedStatement;

            if (NewLineBeforeEnumMember)
                yield return DiagnosticDescriptors.AddNewLineBeforeEnumMember;

            if (NewLineBeforeStatement)
                yield return DiagnosticDescriptors.AddNewLineBeforeStatement;

            if (RemoveRedundantEmptyLine)
                yield return DiagnosticDescriptors.RemoveRedundantEmptyLine;

            if (EndOfLine == "lf")
            {
                yield return DiagnosticDescriptors.UseLinefeedAsNewLine;
            }
            else if (EndOfLine == "crlf")
            {
                yield return DiagnosticDescriptors.UseCarriageReturnAndLinefeedAsNewLine;
            }
        }
    }
}
