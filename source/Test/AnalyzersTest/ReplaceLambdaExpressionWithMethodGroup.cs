// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS0219, RCS1016, RCS1021, RCS1048, RCS1163

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class ReplaceLambdaExpressionWithMethodGroup
    {
        private class Foo
        {
            private void Bar()
            {
                IEnumerable<object> x = null;
                Func<string, object> project = null;
                Func<string, int, object> projectWithIndex = null;

                var items = new List<string>();

                x = items.Select(f => Project(f));

                x = items.Select(f =>
                {
                    return Project(f);
                });

                x = items.Select(delegate (string f)
                {
                    return Project(f);
                });

                x = items.Select((f, i) => ProjectWithIndex(f, i));

                x = items.Select((f, i) =>
                {
                    return ProjectWithIndex(f, i);
                });

                x = items.Select(delegate (string f, int i)
                {
                    return ProjectWithIndex(f, i);
                });

                project = f => Project(f);

                project = f =>
                {
                    return Project(f);
                };

                project = delegate (string f)
                {
                    return Project(f);
                };

                projectWithIndex = (f, i) => ProjectWithIndex(f, i);

                projectWithIndex = (f, i) =>
                {
                    return ProjectWithIndex(f, i);
                };

                projectWithIndex = delegate (string f, int i)
                {
                    return ProjectWithIndex(f, i);
                };

                items.ForEach(f => Do(f));

                items.ForEach(f =>
                {
                    Do(f);
                });

                items.ForEach((f) => Do(f));

                items.ForEach((f) =>
                {
                    Do(f);
                });

                items.ForEach(delegate (string f)
                {
                    Do(f);
                });
            }

            private static string Project(string value)
            {
                return value;
            }

            private static string ProjectWithIndex(string value, int index)
            {
                return value;
            }

            private static void Do(string value)
            {
            }
        }
    }
}
