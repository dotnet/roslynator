// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CodeFixes
{
    public readonly struct CodeFixIdentifier : IEquatable<CodeFixIdentifier>, IComparable<CodeFixIdentifier>, IComparable
    {
        public const string CodeFixIdPrefix = "RCF";

        public CodeFixIdentifier(string compilerDiagnosticId, string codeFixId)
        {
            if (compilerDiagnosticId?.StartsWith("CS", StringComparison.Ordinal) == false)
                throw new ArgumentException("", nameof(compilerDiagnosticId));

            if (codeFixId?.StartsWith(CodeFixIdPrefix, StringComparison.Ordinal) == false)
                throw new ArgumentException("", nameof(codeFixId));

            CompilerDiagnosticId = compilerDiagnosticId;
            CodeFixId = codeFixId;
        }

        public string CompilerDiagnosticId { get; }

        public string CodeFixId { get; }

        public bool IsDefault
        {
            get { return CompilerDiagnosticId == null && CodeFixId == null; }
        }

        public static CodeFixIdentifier Parse(string text)
        {
            return Parse(text, shouldThrow: true);
        }

        public static bool TryParse(string text, out CodeFixIdentifier codeFixIdentifier)
        {
            codeFixIdentifier = Parse(text, shouldThrow: false);

            return !codeFixIdentifier.IsDefault;
        }

        private static CodeFixIdentifier Parse(string text, bool shouldThrow)
        {
            if (text == null)
            {
                if (shouldThrow)
                    throw new ArgumentNullException(nameof(text));

                return default;
            }

            int index = text.IndexOf(".");

            if (index == -1
                || index == 0
                || index == text.Length - 1)
            {
                if (shouldThrow)
                    throw new ArgumentException("", nameof(text));

                return default;
            }

            if (index == -1)
            {
                if (text.StartsWith(CodeFixIdPrefix, StringComparison.Ordinal))
                    return new CodeFixIdentifier(null, text);

                if (text.StartsWith("CS", StringComparison.Ordinal))
                    return new CodeFixIdentifier(text, null);

                if (shouldThrow)
                    throw new ArgumentException("", nameof(text));

                return default;
            }

            return new CodeFixIdentifier(text.Substring(0, index), text.Substring(index + 1));
        }

        public override bool Equals(object obj)
        {
            return obj is CodeFixIdentifier other
                && Equals(other);
        }

        public bool Equals(CodeFixIdentifier other)
        {
            return CompilerDiagnosticId == other.CompilerDiagnosticId
                && CodeFixId == other.CodeFixId;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(CompilerDiagnosticId, Hash.Create(CodeFixId));
        }

        public override string ToString()
        {
            if (CompilerDiagnosticId != null)
            {
                if (CodeFixId != null)
                {
                    return CompilerDiagnosticId + "." + CodeFixId;
                }
                else
                {
                    return CompilerDiagnosticId;
                }
            }
            else if (CodeFixId != null)
            {
                return CodeFixId;
            }

            return "";
        }

        public int CompareTo(CodeFixIdentifier other)
        {
            int diff = StringComparer.Ordinal.Compare(CompilerDiagnosticId, other.CompilerDiagnosticId);

            if (diff != 0)
                return diff;

            return StringComparer.Ordinal.Compare(CodeFixId, other.CodeFixId);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (obj is CodeFixIdentifier other)
                return CompareTo(other);

            throw new ArgumentException("", nameof(obj));
        }

        public static bool operator ==(CodeFixIdentifier left, CodeFixIdentifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CodeFixIdentifier left, CodeFixIdentifier right)
        {
            return !(left == right);
        }
    }
}
