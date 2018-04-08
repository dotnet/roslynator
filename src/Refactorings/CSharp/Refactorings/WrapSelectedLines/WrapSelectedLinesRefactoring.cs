// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings.WrapSelectedLines
{
    internal abstract class WrapSelectedLinesRefactoring : SelectedLinesRefactoring
    {
        public virtual bool Indent
        {
            get { return false; }
        }

        public abstract string GetFirstLineText();

        public abstract string GetLastLineText();

        public override ImmutableArray<TextChange> GetTextChanges(IEnumerable<TextLine> selectedLines)
        {
            var textChanges = new List<TextChange>();

            using (IEnumerator<TextLine> en = selectedLines.GetEnumerator())
            {
                en.MoveNext();

                TextLine firstLine = en.Current;

                string text = firstLine.ToString();

                string indent = (Indent)
                    ? StringUtility.GetLeadingWhitespaceExceptNewLine(text)
                    : "";

                string newText = indent + GetFirstLineText() + Environment.NewLine + text + Environment.NewLine;

                if (en.MoveNext())
                {
                    textChanges.Add(new TextChange(firstLine.SpanIncludingLineBreak, newText));

                    TextLine lastLine = en.Current;

                    while (en.MoveNext())
                        lastLine = en.Current;

                    textChanges.Add(new TextChange(lastLine.SpanIncludingLineBreak, lastLine.ToString() + Environment.NewLine + indent + GetLastLineText() + Environment.NewLine));
                }
                else
                {
                    newText += indent + GetLastLineText() + Environment.NewLine;

                    textChanges.Add(new TextChange(firstLine.SpanIncludingLineBreak, newText));
                }
            }

            return textChanges.ToImmutableArray();
        }
    }
}
