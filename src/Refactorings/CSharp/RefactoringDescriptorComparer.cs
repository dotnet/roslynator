// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings
{
    internal abstract class RefactoringDescriptorComparer : IComparer<RefactoringDescriptor>, IEqualityComparer<RefactoringDescriptor>, IComparer, IEqualityComparer
    {
        public static RefactoringDescriptorComparer Id { get; } = new RefactoringDescriptorIdComparer();

        public abstract int Compare(RefactoringDescriptor x, RefactoringDescriptor y);

        public abstract bool Equals(RefactoringDescriptor x, RefactoringDescriptor y);

        public abstract int GetHashCode(RefactoringDescriptor obj);

        public int Compare(object x, object y)
        {
            if (x == y)
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (x is RefactoringDescriptor a
                && y is RefactoringDescriptor b)
            {
                return Compare(a, b);
            }

            throw new ArgumentException("", nameof(x));
        }

        new public bool Equals(object x, object y)
        {
            if (x == y)
                return true;

            if (x == null)
                return false;

            if (y == null)
                return false;

            if (x is RefactoringDescriptor a
                && y is RefactoringDescriptor b)
            {
                return Equals(a, b);
            }

            throw new ArgumentException("", nameof(x));
        }

        public int GetHashCode(object obj)
        {
            if (obj == null)
                return 0;

            if (obj is RefactoringDescriptor descriptor)
                return GetHashCode(descriptor);

            throw new ArgumentException("", nameof(obj));
        }

        private class RefactoringDescriptorIdComparer : RefactoringDescriptorComparer
        {
            public override int Compare(RefactoringDescriptor x, RefactoringDescriptor y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x.Id == null)
                    return -1;

                if (y.Id == null)
                    return 1;

                return string.CompareOrdinal(x.Id, y.Id);
            }

            public override bool Equals(RefactoringDescriptor x, RefactoringDescriptor y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x.Id == null)
                    return false;

                if (y.Id == null)
                    return false;

                return string.Equals(x.Id, y.Id, StringComparison.Ordinal);
            }

            public override int GetHashCode(RefactoringDescriptor obj)
            {
                if (obj.Id == null)
                    return 0;

                return StringComparer.Ordinal.GetHashCode(obj.Id);
            }
        }
    }
}