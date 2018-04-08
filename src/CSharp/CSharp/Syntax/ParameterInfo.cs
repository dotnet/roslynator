// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    internal readonly struct ParameterInfo : IEquatable<ParameterInfo>
    {
        public ParameterInfo(ParameterSyntax parameter, CSharpSyntaxNode body)
        {
            Parameter = parameter;
            ParameterList = default(BaseParameterListSyntax);
            Body = body;
            TypeParameterList = default(TypeParameterListSyntax);
        }

        public ParameterInfo(BaseParameterListSyntax parameterList, CSharpSyntaxNode body)
            : this(parameterList, default(TypeParameterListSyntax), body)
        {
        }

        public ParameterInfo(BaseParameterListSyntax parameterList, TypeParameterListSyntax typeParameterList, CSharpSyntaxNode body)
        {
            ParameterList = parameterList;
            Parameter = default(ParameterSyntax);
            Body = body;
            TypeParameterList = typeParameterList;
        }

        private static ParameterInfo Default { get; } = new ParameterInfo();

        public SyntaxNode Node
        {
            get { return ParameterList?.Parent ?? Parameter?.Parent; }
        }

        public TypeParameterListSyntax TypeParameterList { get; }

        public SeparatedSyntaxList<TypeParameterSyntax> TypeParameters
        {
            get { return TypeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>); }
        }

        public ParameterSyntax Parameter { get; }

        public BaseParameterListSyntax ParameterList { get; }

        public SeparatedSyntaxList<ParameterSyntax> Parameters
        {
            get { return ParameterList?.Parameters ?? default(SeparatedSyntaxList<ParameterSyntax>); }
        }

        public CSharpSyntaxNode Body { get; }

        public bool Success
        {
            get { return ParameterList != null || Parameter != null; }
        }

        internal static ParameterInfo Create(ConstructorDeclarationSyntax constructorDeclaration, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = constructorDeclaration.ParameterList;

            if (!CheckParameterList(parameterList, allowMissing))
                return Default;

            CSharpSyntaxNode body = constructorDeclaration.BodyOrExpressionBody();

            if (!Check(body, allowMissing))
                return Default;

            return new ParameterInfo(parameterList, body);
        }

        internal static ParameterInfo Create(MethodDeclarationSyntax methodDeclaration, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = methodDeclaration.ParameterList;

            if (!Check(parameterList, allowMissing))
                return Default;

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (!CheckParameters(parameters, allowMissing))
                return Default;

            TypeParameterListSyntax typeParameterList = methodDeclaration.TypeParameterList;
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = typeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>);

            if (!CheckTypeParameters(typeParameters, allowMissing))
                return Default;

            if (!parameters.Any()
                && !typeParameters.Any())
            {
                return Default;
            }

            CSharpSyntaxNode body = methodDeclaration.BodyOrExpressionBody();

            if (!Check(body, allowMissing))
                return Default;

            return new ParameterInfo(parameterList, typeParameterList, body);
        }

        internal static ParameterInfo Create(OperatorDeclarationSyntax operatorDeclaration, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = operatorDeclaration.ParameterList;

            if (!CheckParameterList(parameterList, allowMissing))
                return Default;

            CSharpSyntaxNode body = operatorDeclaration.BodyOrExpressionBody();

            if (!Check(body, allowMissing))
                return Default;

            return new ParameterInfo(parameterList, body);
        }

        internal static ParameterInfo Create(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = conversionOperatorDeclaration.ParameterList;

            if (!CheckParameterList(parameterList, allowMissing))
                return Default;

            CSharpSyntaxNode body = conversionOperatorDeclaration.BodyOrExpressionBody();

            if (!Check(body, allowMissing))
                return Default;

            return new ParameterInfo(parameterList, body);
        }

        internal static ParameterInfo Create(DelegateDeclarationSyntax delegateDeclaration, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = delegateDeclaration.ParameterList;

            if (!Check(parameterList, allowMissing))
                return Default;

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (!CheckParameters(parameters, allowMissing))
                return Default;

            TypeParameterListSyntax typeParameterList = delegateDeclaration.TypeParameterList;
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = typeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>);

            if (!CheckTypeParameters(typeParameters, allowMissing))
                return Default;

            if (!parameters.Any()
                && !typeParameters.Any())
            {
                return Default;
            }

            return new ParameterInfo(parameterList, typeParameterList, default(CSharpSyntaxNode));
        }

        internal static ParameterInfo Create(LocalFunctionStatementSyntax localFunction, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = localFunction.ParameterList;

            if (!Check(parameterList, allowMissing))
                return Default;

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (!CheckParameters(parameters, allowMissing))
                return Default;

            TypeParameterListSyntax typeParameterList = localFunction.TypeParameterList;
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = typeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>);

            if (!CheckTypeParameters(typeParameters, allowMissing))
                return Default;

            if (!parameters.Any()
                && !typeParameters.Any())
            {
                return Default;
            }

            CSharpSyntaxNode body = localFunction.BodyOrExpressionBody();

            if (!Check(body, allowMissing))
                return Default;

            return new ParameterInfo(parameterList, typeParameterList, body);
        }

        internal static ParameterInfo Create(IndexerDeclarationSyntax indexerDeclaration, bool allowMissing = false)
        {
            BaseParameterListSyntax parameterList = indexerDeclaration.ParameterList;

            if (!CheckParameterList(parameterList, allowMissing))
                return Default;

            CSharpSyntaxNode body = indexerDeclaration.AccessorList ?? (CSharpSyntaxNode)indexerDeclaration.ExpressionBody;

            if (!Check(body, allowMissing))
                return Default;

            return new ParameterInfo(parameterList, body);
        }

        internal static ParameterInfo Create(SimpleLambdaExpressionSyntax simpleLambda, bool allowMissing = false)
        {
            ParameterSyntax parameter = simpleLambda.Parameter;

            if (!Check(parameter, allowMissing))
                return Default;

            CSharpSyntaxNode body = simpleLambda.Body;

            if (!Check(body, allowMissing))
                return Default;

            return new ParameterInfo(parameter, body);
        }

        internal static ParameterInfo Create(ParenthesizedLambdaExpressionSyntax parenthesizedLambda, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = parenthesizedLambda.ParameterList;

            if (!CheckParameterList(parameterList, allowMissing))
                return Default;

            CSharpSyntaxNode body = parenthesizedLambda.Body;

            if (!Check(body, allowMissing))
                return Default;

            return new ParameterInfo(parameterList, body);
        }

        internal static ParameterInfo Create(AnonymousMethodExpressionSyntax anonymousMethod, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = anonymousMethod.ParameterList;

            if (!CheckParameterList(parameterList, allowMissing))
                return Default;

            CSharpSyntaxNode body = anonymousMethod.Body;

            if (!Check(body, allowMissing))
                return Default;

            return new ParameterInfo(parameterList, body);
        }

        private static bool CheckParameterList(BaseParameterListSyntax parameterList, bool allowMissing)
        {
            if (!Check(parameterList, allowMissing))
                return false;

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (!parameters.Any())
                return false;

            foreach (ParameterSyntax parameter in parameters)
            {
                if (!Check(parameter, allowMissing))
                    return false;
            }

            return true;
        }

        private static bool CheckTypeParameters(SeparatedSyntaxList<TypeParameterSyntax> typeParameters, bool allowMissing)
        {
            foreach (TypeParameterSyntax typeParameter in typeParameters)
            {
                if (!Check(typeParameter, allowMissing))
                    return false;
            }

            return true;
        }

        private static bool CheckParameters(SeparatedSyntaxList<ParameterSyntax> parameters, bool allowMissing)
        {
            foreach (ParameterSyntax parameter in parameters)
            {
                if (!Check(parameter, allowMissing))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return Node?.ToString() ?? "";
        }

        public override bool Equals(object obj)
        {
            return obj is ParameterInfo other && Equals(other);
        }

        public bool Equals(ParameterInfo other)
        {
            return EqualityComparer<SyntaxNode>.Default.Equals(Node, other.Node);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<SyntaxNode>.Default.GetHashCode(Node);
        }

        public static bool operator ==(ParameterInfo info1, ParameterInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(ParameterInfo info1, ParameterInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
