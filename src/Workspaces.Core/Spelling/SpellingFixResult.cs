// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Spelling
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal sealed class SpellingFixResult
    {
        private int _lineStartIndex = -1;
        private int _lineEndIndex = -1;

        private SpellingFixResult(
            string input,
            SpellingCapture capture,
            TextSpan span,
            SpellingFix fix,
            FileLinePositionSpan lineSpan)
        {
            Input = input;
            Capture = capture;
            Span = span;
            Fix = fix;
            LineSpan = lineSpan;
        }

        public static SpellingFixResult Create(string input, SpellingDiagnostic diagnostic)
        {
            return Create(input, diagnostic, default);
        }

        public static SpellingFixResult Create(string input, SpellingDiagnostic diagnostic, SpellingFix fix)
        {
            return new SpellingFixResult(
                input,
                new SpellingCapture(diagnostic.Value, diagnostic.Index, diagnostic.Parent, diagnostic.ParentIndex),
                diagnostic.Span,
                fix,
                diagnostic.Location.GetMappedLineSpan());
        }

        public string Input { get; }

        public SpellingCapture Capture { get; }

        public string Value => Capture.Value;

        public int Index => Capture.Index;

        public int Length => Capture.Length;

        public string ContainingValue => Capture.ContainingValue;

        public int ContainingValueIndex => Capture.ContainingValueIndex;

        public TextSpan Span { get; }

        public SpellingFix Fix { get; }

        public string Replacement => Fix.Value;

        public SpellingFixKind Kind => Fix.Kind;

        public bool HasFix => !Fix.IsDefault;

        public FileLinePositionSpan LineSpan { get; }

        public string FilePath => LineSpan.Path;

        public int LineNumber => LineSpan.StartLine() + 1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Value}  {LineSpan.Path}";

        public int LineStartIndex
        {
            get
            {
                if (_lineStartIndex == -1)
                    _lineStartIndex = Span.Start - LineSpan.StartLinePosition.Character;

                return _lineStartIndex;
            }
        }

        public int LineEndIndex
        {
            get
            {
                if (_lineEndIndex == -1)
                    _lineEndIndex = TextUtility.GetLineEndIndex(Input, Index + Length);

                return _lineEndIndex;
            }
        }
    }
}
