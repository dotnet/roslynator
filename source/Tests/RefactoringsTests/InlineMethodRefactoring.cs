// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    public partial class InlineMethodRefactoring
    {
        public void Method()
        {
            Entity x = null;
            Entity y = null;
            Entity z = null;

            Entity.VoidMethod(x, y);

            x = Entity.Method(x, y);

            x = Entity.MethodWithExpressionBody(x, y);

            Entity.VoidMethod2(x, y);

            x = Entity.Method2(x, y);

            x = Method3(x, y);

            x = Entity.Method(Entity.Method(x, y), y);

            x = Entity.Method2(Entity.Method2(x, y), y);

            x = z.ExtensionMethod(x, y).ExtensionMethod(x, y);

            x = z.ExtensionMethod2(x, y).ExtensionMethod2(x, y);

            z.InstanceMethod(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Entity Method3(Entity p1, Entity p2)
        {
#if DEBUG
            return p1 + p2 + p2;
        }
#endif

        public partial class Entity
        {
            private readonly Entity _value;

            public Entity()
            {
                InstanceMethod(null, null);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public static Entity Method(Entity p1, Entity p2)
            {
#if DEBUG
                return p1 + p2 + p2;
            }
#endif
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public static void VoidMethod(Entity p1, Entity p2)
            {
#if DEBUG
                var a = p1 + p2 + p2;
                var b = p1 + p2 + p2;
            }
#endif
            public static Entity MethodWithExpressionBody(Entity p1, Entity p2) => p1 + p2 + p2;

            public Entity InstanceMethod(Entity p1, Entity p2)
            {

                return _value + p1 + p2;
            }

            public static Entity operator +(Entity left, Entity right)
            {
                return null;
            }
        }
    }

    public static partial class InlineMethodRefactoringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static InlineMethodRefactoring.Entity ExtensionMethod(
            this InlineMethodRefactoring.Entity entity,
            InlineMethodRefactoring.Entity ep1,
            InlineMethodRefactoring.Entity ep2)
        {
#if DEBUG
            return entity + ep1 + ep2;
        }
#endif
    }
}
