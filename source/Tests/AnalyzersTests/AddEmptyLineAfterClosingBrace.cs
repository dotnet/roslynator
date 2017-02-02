// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class AddEmptyLineAfterClosingBrace
    {
        public static void Foo()
        {
            var items = new List<string>();

            {
            }
            while (true)
            {
            }
            for (int i = 0; i < items.Count; i++)
            {
            }
            foreach (var item in items)
            {
            }
            using (resource)
            {
            }
            fixed ()
            {
            }
            checked
            {
            }
            unchecked
            {
            }
            unsafe
            {
            }
            lock (this)
            {
            }
            if (true)
            {
            }
            switch (switch_on)
            {
            }
            try
            {
            }
            catch (System.Exception)
            {
            }
            try
            {
            }
            finally
            {
            }
            Foo();
        }
    }
}
