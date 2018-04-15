// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1001, RCS1002, RCS1003, RCS1004, RCS1007, RCS1040, RCS1063, RCS1065, RCS1118, RCS1126, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class SimplifyCodeBranching
    {
        public static void Bar()
        {
            bool condition1 = false;
            bool condition2 = false;

            if (condition1)
            {
            }
            else
            {
                Bar();
            }

            if (condition1)
            {
            }
            else
                Bar();

            if (condition1)
            {
            }
            else if (condition2)
            {
                Bar();
            }

            if (condition1)
            {
            }
            else if (condition2)
                Bar();

            while (true)
            {
                if (condition1)
                {
                    Bar();
                }
                else
                {
                    break;
                }
            }

            while (true)
            {
                if (condition1)
                {
                    Bar();
                    Bar();
                }
                else
                {
                    break;
                }
            }

            while (true)
                if (condition1)
                {
                    Bar();
                }
                else
                {
                    break;
                }

            while (true)
                if (condition1)
                {
                    Bar();
                    Bar();
                }
                else
                {
                    break;
                }

            while (true)
            {
                if (condition1)
                    Bar();
                else
                    break;
            }

            do
            {
                Bar();

                if (condition1)
                {
                    break;
                }
            }
            while (true);

            do
            {
                Bar();

                if (condition1)
                    break;
            }
            while (true);

            while (true)
            {
                Bar();

                if (condition1)
                {
                    break;
                }
            }

            while (true)
            {
                Bar();

                if (condition1)
                    break;
            }

            while (true)
            {
                if (condition1)
                {
                    break;
                }

                Bar();
            }

            while (true)
            {
                if (condition1)
                    break;

                Bar();
            }

            do
            {
                if (condition1)
                {
                    break;
                }

                Bar();
            }
            while (true);

            do
            {
                if (condition1)
                    break;

                Bar();
            }
            while (true);

            //n

            if (condition1)
            {
            }
            else
            {
            }

            if (condition1)
            {
            }
            else if (condition2)
            {
            }

            if (condition1)
            {
                Bar();
            }
            else
            {
                Bar();
            }

            if (condition1)
            {
                Bar();
            }
            else if (condition2)
            {
                Bar();
            }

            if ()
            {
            }
            else if (condition2)
            {
                Bar();
            }

            if (condition1)
            {
            }
            else if ()
            {
                Bar();
            }

            while (true)
            {
                if (condition1)
                {
                    break;
                }
            }

            while (condition1)
            {
                if (condition2)
                {
                    break;
                }
            }

            while (true)
            {
                if (condition1)
                {
                    break;
                }
                else
                {
                }
            }

            while ()
            {
                if (condition1)
                {
                    break;
                }
            }

            while (true)
            {
                if ()
                {
                    break;
                }
            }

            while (condition1)
            {
                Bar();

                if (condition2)
                {
                    break;
                }
            }

            while (condition1)
            {
                Bar();

                if (condition2)
                {
                    return;
                }
            }

            while ()
            {
                Bar();

                if (condition1)
                {
                    break;
                }
            }

            while (condition1)
            {
                Bar();

                if ()
                {
                    break;
                }
            }

            do
            {
                Bar();

                if (condition2)
                {
                    break;
                }

            } while (condition1);

            do
            {
                Bar();

                if (condition2)
                {
                    return;
                }
            }
            while (condition1);

            do
            {
                Bar();

                if (condition1)
                {
                    break;
                }
            }
            while ();

            do
            {
                Bar();

                if ()
                {
                    break;
                }
            }
            while (condition1);

            do
            {
                Bar();

                if (condition2)
                {
                    break;
                }

            } while (condition1);

            do
            {
                Bar();

                if (condition2)
                {
                    return;
                }

            } while (condition1);

            do
            {
                Bar();

                if (condition1)
                {
                    break;
                }

            } while ();

            do
            {
                Bar();

                if ()
                {
                    break;
                }

            } while (condition1);

            while (condition1)
            {
                Bar();

                if (condition2)
                {
                    return;
                }
            }

            while ()
            {
                Bar();

                if (condition1)
                {
                    break;
                }
            }

            while (condition1)
            {
                Bar();

                if ()
                {
                    break;
                }
            }
        }
    }
}
