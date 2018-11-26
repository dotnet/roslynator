// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;

namespace Roslynator
{
    internal class TextWriterWithVerbosity : TextWriter
    {
        public TextWriterWithVerbosity(TextWriter writer)
        {
            Writer = writer;
        }

        public TextWriterWithVerbosity(TextWriter writer, IFormatProvider formatProvider) : base(formatProvider)
        {
            Writer = writer;
        }

        public Verbosity Verbosity { get; set; } = Verbosity.Diagnostic;

        public override Encoding Encoding => Writer.Encoding;

        protected TextWriter Writer { get; }

        public override void Write(bool value)
        {
            Writer.Write(value);
        }

        public override void Write(char value)
        {
            Writer.Write(value);
        }

        public override void Write(char[] buffer)
        {
            Writer.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Writer.Write(buffer, index, count);
        }

        public override void Write(decimal value)
        {
            Writer.Write(value);
        }

        public override void Write(double value)
        {
            Writer.Write(value);
        }

        public override void Write(int value)
        {
            Writer.Write(value);
        }

        public override void Write(long value)
        {
            Writer.Write(value);
        }

        public override void Write(object value)
        {
            Writer.Write(value);
        }

        public override void Write(float value)
        {
            Writer.Write(value);
        }

        public override void Write(string value)
        {
            Writer.Write(value);
        }

        public void Write(string value, Verbosity verbosity)
        {
            WriteIf(verbosity <= Verbosity, value);
        }

        public void WriteIf(bool condition, string value)
        {
            if (condition)
                Write(value);
        }

        public override void Write(string format, object arg0)
        {
            Writer.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            Writer.Write(format, arg0, arg1);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            Writer.Write(format, arg0, arg1, arg2);
        }

        public override void Write(string format, params object[] arg)
        {
            Writer.Write(format, arg);
        }

        public override void Write(uint value)
        {
            Writer.Write(value);
        }

        public override void Write(ulong value)
        {
            Writer.Write(value);
        }

        public override void WriteLine()
        {
            Writer.WriteLine();
        }

        public void WriteLine(Verbosity verbosity)
        {
            WriteLineIf(verbosity <= Verbosity);
        }

        public void WriteLineIf(bool condition)
        {
            if (condition)
                WriteLine();
        }

        public override void WriteLine(bool value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(char[] buffer)
        {
            Writer.WriteLine(buffer);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            Writer.WriteLine(buffer, index, count);
        }

        public override void WriteLine(decimal value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(double value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(int value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(object value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(string value)
        {
            Writer.WriteLine(value);
        }

        public void WriteLine(string value, Verbosity verbosity)
        {
            WriteLineIf(verbosity <= Verbosity, value);
        }

        public void WriteLineIf(bool condition, string value)
        {
            if (condition)
                WriteLine(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            Writer.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            Writer.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Writer.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            Writer.WriteLine(format, arg);
        }

        public override void WriteLine(uint value)
        {
            Writer.WriteLine(value);
        }

        public override void WriteLine(ulong value)
        {
            Writer.WriteLine(value);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Writer.Dispose();

            base.Dispose(disposing);
        }
    }
}
