// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

#pragma warning disable CS0162, CS0168, CS0219, CS8321, RCS1002, RCS1004, RCS1016, RCS1021, RCS1048, RCS1054, RCS1061, RCS1090, RCS1118, RCS1124, RCS1136, RCS1163, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemoveRedundantAsyncAwait
    {
        private static class Foo
        {
            public static Task<object> GetAsync() => Task.FromResult(default(object));

            public static async Task<object> MethodWitBodyAsync()
            {
                return await GetAsync().ConfigureAwait(false);

                async Task<object> LocalWithBodyAsync()
                {
                    return await GetAsync().ConfigureAwait(false);
                }

                async Task<object> LocalWithExpressionBodyAsync() => await GetAsync().ConfigureAwait(false);
            }

            public static async Task<object> MethodWithExpressionBodyAsync() => await GetAsync().ConfigureAwait(false);

            public static async Task<object> Get2Async()
            {
                return await GetAsync();

                async Task<object> GetAsync()
                {
                    return await GetAsync();
                }
            }

            public static void Bar()
            {
                Func<object, Task<object>> func = async f =>
                {
                    return await GetAsync().ConfigureAwait(false);
                };

                Func<object, Task<object>> func2 = async f => await GetAsync().ConfigureAwait(false);

                Func<object, Task<object>> func3 = async (f) =>
                {
                    return await GetAsync().ConfigureAwait(false);
                };

                Func<object, Task<object>> func4 = async (f) => await GetAsync().ConfigureAwait(false);

                Func<object, Task<object>> func5 = async delegate (object f)
                {
                    return await GetAsync().ConfigureAwait(false);
                };
            }

            public static async Task<object> IfAndReturnAsync()
            {
                bool f = false;

                if (f)
                {
                    return await GetAsync().ConfigureAwait(false);
                }
                else if (f)
                {
                    return await GetAsync().ConfigureAwait(false);
                }

                return await GetAsync().ConfigureAwait(false);
            }

            public static async Task<object> IfElseAsync()
            {
                bool f = false;

                if (f)
                {
                    return await GetAsync().ConfigureAwait(false);
                }
                else
                {
                    return await GetAsync().ConfigureAwait(false);
                }
            }

            public static async Task<object> SwitchAndReturnAsync()
            {
                bool f = false;

                switch (f)
                {
                    case true:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                    case false:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                }

                return await GetAsync().ConfigureAwait(false);
            }

            public static async Task<object> SwitchWithDefaultAsync()
            {
                bool f = false;

                switch (f)
                {
                    case true:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                    case false:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                    default:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                }
            }

            //n

            public static async Task<object> IfElse2Async()
            {
                bool f = false;
                if (f)
                {
                    return default(object);
                    return await GetAsync().ConfigureAwait(false);
                }
                else
                {
                    return await GetAsync().ConfigureAwait(false);
                }
            }

            public static async Task<object> IfElse3Async()
            {
                bool f = false;
                if (f)
                {
                    await GetAsync().ConfigureAwait(false);
                    return await GetAsync().ConfigureAwait(false);
                }
                else
                {
                    return await GetAsync().ConfigureAwait(false);
                }
            }

            public static async Task<object> SwitchAndReturn2Async()
            {
                bool f = false;
                switch (f)
                {
                    case true:
                        {
                            return default(object);
                            return await GetAsync().ConfigureAwait(false);
                        }
                    case false:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                }

                return await GetAsync().ConfigureAwait(false);
            }

            public static async Task<object> SwitchAndReturn3Async()
            {
                bool f = false;
                switch (f)
                {
                    case true:
                        {
                            await GetAsync().ConfigureAwait(false);
                            return await GetAsync().ConfigureAwait(false);
                        }
                    case false:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                }

                return await GetAsync().ConfigureAwait(false);
            }

            public static async Task<object> SwitchWithDefault2Async()
            {
                bool f = false;
                switch (f)
                {
                    case true:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                    default:
                        {
                            return default(object);
                            return await GetAsync().ConfigureAwait(false);
                        }
                }
            }

            public static async Task<object> SwitchWithDefault3Async()
            {
                bool f = false;
                switch (f)
                {
                    case true:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                    default:
                        {
                            await GetAsync().ConfigureAwait(false);
                            return await GetAsync().ConfigureAwait(false);
                        }
                }
            }

            public static async Task<object> MethodWitBody2Async()
            {
                bool f = false;

                if (f)
                {
                    if (f)
                    {
                        return default(object);
                    }
                }

                return await GetAsync().ConfigureAwait(false);
            }

            public static async Task<object> MethodWitBody3Async()
            {
                bool f = false;

                if (f)
                {
                    if (f)
                    {
                        await GetAsync().ConfigureAwait(false);
                    }
                }

                return await GetAsync().ConfigureAwait(false);
            }

            public static async Task DoAsync()
            {
                return await DoAsync().ConfigureAwait(false);
            }

            public static async Task<object> GetValueAsync()
            {
                object value = await GetAsync().ConfigureAwait(false);
                return value;
            }
        }

        private static class ReturnTypeAndAwaitTypeEquals
        {
            public static Task<string> GetAsync() => Task.FromResult(default(string));

            public static async Task<object> MethodWitBodyAsync()
            {
                return await GetAsync().ConfigureAwait(false);

                async Task<object> LocalWithBodyAsync()
                {
                    return await GetAsync().ConfigureAwait(false);
                }

                async Task<object> LocalWithExpressionBodyAsync() => await GetAsync().ConfigureAwait(false);
            }

            public static async Task<object> MethodWithExpressionBodyAsync() => await GetAsync().ConfigureAwait(false);

            public static async Task<object> Get2Async()
            {
                return await GetAsync();

                async Task<object> LocalAsync()
                {
                    return await GetAsync();
                }
            }

            public static void Foo()
            {
                Func<object, Task<object>> func = async f =>
                {
                    return await GetAsync().ConfigureAwait(false);
                };

                Func<object, Task<object>> func2 = async f => await GetAsync().ConfigureAwait(false);

                Func<object, Task<object>> func3 = async (f) =>
                {
                    return await GetAsync().ConfigureAwait(false);
                };

                Func<object, Task<object>> func4 = async (f) => await GetAsync().ConfigureAwait(false);

                Func<object, Task<object>> func5 = async delegate (object f)
                {
                    return await GetAsync().ConfigureAwait(false);
                };
            }

            public static async Task<object> IfAndReturnAsync()
            {
                bool f = false;

                if (f)
                {
                    return await GetAsync().ConfigureAwait(false);
                }
                else if (f)
                {
                    return await GetAsync().ConfigureAwait(false);
                }

                return await GetAsync().ConfigureAwait(false);
            }

            public static async Task<object> IfElseAsync()
            {
                bool f = false;

                if (f)
                {
                    return await GetAsync().ConfigureAwait(false);
                }
                else
                {
                    return await GetAsync().ConfigureAwait(false);
                }
            }

            public static async Task<object> SwitchAndReturnAsync()
            {
                bool f = false;

                switch (f)
                {
                    case true:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                    case false:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                }

                return await GetAsync().ConfigureAwait(false);
            }

            public static async Task<object> SwitchWithDefaultAsync()
            {
                bool f = false;

                switch (f)
                {
                    case true:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                    case false:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                    default:
                        {
                            return await GetAsync().ConfigureAwait(false);
                        }
                }
            }

            public static async Task DoAsync()
            {
                return await DoAsync().ConfigureAwait(false);
            }

            public static async Task<string> GetValueAsync()
            {
                string value = await GetAsync().ConfigureAwait(false);
                return value;
            }
        }

        private static class AwaitContainsAwait
        {
            public static Task<object> GetAsync() => Task.FromResult(default(object));

            public static Task<object> GetAsync(object parameter) => Task.FromResult(parameter);

            public static async Task<object> MethodWitBodyAsync()
            {
                return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);

                async Task<object> LocalWithBodyAsync()
                {
                    return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                }

                async Task<object> LocalWithExpressionBodyAsync() => await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
            }

            public static async Task<object> MethodWithExpressionBodyAsync() => await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);

            public static async Task<object> Get2Async()
            {
                return await GetAsync(await GetAsync().ConfigureAwait(false));

                async Task<object> LocalAsync(object parameter)
                {
                    return await GetAsync(await GetAsync().ConfigureAwait(false));
                }
            }

            public static void Foo()
            {
                Func<object, Task<object>> func = async f =>
                {
                    return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                };

                Func<object, Task<object>> func2 = async f => await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);

                Func<object, Task<object>> func3 = async (f) =>
                {
                    return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                };

                Func<object, Task<object>> func4 = async (f) => await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);

                Func<object, Task<object>> func5 = async delegate (object f)
                {
                    return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                };
            }

            public static async Task<object> IfAndReturnAsync()
            {
                bool f = false;

                if (f)
                {
                    return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                }
                else if (f)
                {
                    return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                }

                return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
            }

            public static async Task<object> IfElseAsync()
            {
                bool f = false;

                if (f)
                {
                    return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                }
                else
                {
                    return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                }
            }

            public static async Task<object> SwitchAndReturnAsync()
            {
                bool f = false;

                switch (f)
                {
                    case true:
                        {
                            return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                        }
                    case false:
                        {
                            return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                        }
                }

                return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
            }

            public static async Task<object> SwitchWithDefaultAsync()
            {
                bool f = false;

                switch (f)
                {
                    case true:
                        {
                            return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                        }
                    case false:
                        {
                            return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                        }
                    default:
                        {
                            return await GetAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
                        }
                }
            }

            public static async Task DoAsync(object parameter)
            {
                return await DoAsync(await GetAsync().ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        private static class AwaitableNonTaskType
        {
            public static async Task<bool> GetAsync()
            {
                return await Observable.Range(0, 1).Any(_ => false);
            }
        }
    }
}
