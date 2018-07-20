// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxWalkers
{
    internal abstract class LocalOrParameterReferenceWalker : CSharpSyntaxNodeWalker
    {
        protected LocalOrParameterReferenceWalker()
        {
        }

        protected override void VisitType(TypeSyntax node)
        {
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
        }

        public override void VisitCrefParameter(CrefParameterSyntax node)
        {
        }

        public override void VisitDefaultExpression(DefaultExpressionSyntax node)
        {
        }

        public override void VisitGenericName(GenericNameSyntax node)
        {
        }

        public override void VisitNullableType(NullableTypeSyntax node)
        {
        }

        public override void VisitPointerType(PointerTypeSyntax node)
        {
        }

        public override void VisitRefType(RefTypeSyntax node)
        {
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
        }

        public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
        }

        public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
        }

        public override void VisitTupleElement(TupleElementSyntax node)
        {
        }

        public override void VisitTypeConstraint(TypeConstraintSyntax node)
        {
        }

        public override void VisitTypeCref(TypeCrefSyntax node)
        {
        }

        public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
        }
    }
}
