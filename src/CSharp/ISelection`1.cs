// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator
{
    /// <summary>
    /// Represents consecutive sequence of selected items in a collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISelection<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Gets an index of the first selected item.
        /// </summary>
        int FirstIndex { get; }

        /// <summary>
        /// Gets an index of the last selected item.
        /// </summary>
        int LastIndex { get; }

        /// <summary>
        /// Gets the first selected item.
        /// </summary>
        /// <returns></returns>
        T First();

        /// <summary>
        /// Gets the last selected item.
        /// </summary>
        /// <returns></returns>
        T Last();
    }
}
