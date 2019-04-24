// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct ParameterInfo
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, ParameterList ?? (SyntaxNode)Parameter); }
        }

        internal static ParameterInfo Create(ConstructorDeclarationSyntax constructorDeclaration, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = constructorDeclaration.ParameterList;

            if (!CheckParameterList(parameterList, allowMissing))
                return default;

            CSharpSyntaxNode body = constructorDeclaration.BodyOrExpressionBody();

            if (!Check(body, allowMissing))
                return default;

            return new ParameterInfo(parameterList, body);
        }

        internal static ParameterInfo Create(MethodDeclarationSyntax methodDeclaration, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = methodDeclaration.ParameterList;

            if (!Check(parameterList, allowMissing))
                return default;

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (!CheckParameters(parameters, allowMissing))
                return default;

            TypeParameterListSyntax typeParameterList = methodDeclaration.TypeParameterList;
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = typeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>);

            if (!CheckTypeParameters(typeParameters, allowMissing))
                return default;

            if (!parameters.Any()
                && !typeParameters.Any())
            {
                return default;
            }

            CSharpSyntaxNode body = methodDeclaration.BodyOrExpressionBody();

            if (!Check(body, allowMissing))
                return default;

            return new ParameterInfo(parameterList, typeParameterList, body);
        }

        internal static ParameterInfo Create(OperatorDeclarationSyntax operatorDeclaration, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = operatorDeclaration.ParameterList;

            if (!CheckParameterList(parameterList, allowMissing))
                return default;

            CSharpSyntaxNode body = operatorDeclaration.BodyOrExpressionBody();

            if (!Check(body, allowMissing))
                return default;

            return new ParameterInfo(parameterList, body);
        }

        internal static ParameterInfo Create(ConversionOperatorDeclarationSyntax conversionOperatorDeclaration, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = conversionOperatorDeclaration.ParameterList;

            if (!CheckParameterList(parameterList, allowMissing))
                return default;

            CSharpSyntaxNode body = conversionOperatorDeclaration.BodyOrExpressionBody();

            if (!Check(body, allowMissing))
                return default;

            return new ParameterInfo(parameterList, body);
        }

        internal static ParameterInfo Create(DelegateDeclarationSyntax delegateDeclaration, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = delegateDeclaration.ParameterList;

            if (!Check(parameterList, allowMissing))
                return default;

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (!CheckParameters(parameters, allowMissing))
                return default;

            TypeParameterListSyntax typeParameterList = delegateDeclaration.TypeParameterList;
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = typeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>);

            if (!CheckTypeParameters(typeParameters, allowMissing))
                return default;

            if (!parameters.Any()
                && !typeParameters.Any())
            {
                return default;
            }

            return new ParameterInfo(parameterList, typeParameterList, default(CSharpSyntaxNode));
        }

        internal static ParameterInfo Create(LocalFunctionStatementSyntax localFunction, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = localFunction.ParameterList;

            if (!Check(parameterList, allowMissing))
                return default;

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            if (!CheckParameters(parameters, allowMissing))
                return default;

            TypeParameterListSyntax typeParameterList = localFunction.TypeParameterList;
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = typeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>);

            if (!CheckTypeParameters(typeParameters, allowMissing))
                return default;

            if (!parameters.Any()
                && !typeParameters.Any())
            {
                return default;
            }

            CSharpSyntaxNode body = localFunction.BodyOrExpressionBody();

            if (!Check(body, allowMissing))
                return default;

            return new ParameterInfo(parameterList, typeParameterList, body);
        }

        internal static ParameterInfo Create(IndexerDeclarationSyntax indexerDeclaration, bool allowMissing = false)
        {
            BaseParameterListSyntax parameterList = indexerDeclaration.ParameterList;

            if (!CheckParameterList(parameterList, allowMissing))
                return default;

            CSharpSyntaxNode body = indexerDeclaration.AccessorList ?? (CSharpSyntaxNode)indexerDeclaration.ExpressionBody;

            if (!Check(body, allowMissing))
                return default;

            return new ParameterInfo(parameterList, body);
        }

        internal static ParameterInfo Create(SimpleLambdaExpressionSyntax simpleLambda, bool allowMissing = false)
        {
            ParameterSyntax parameter = simpleLambda.Parameter;

            if (!Check(parameter, allowMissing))
                return default;

            CSharpSyntaxNode body = simpleLambda.Body;

            if (!Check(body, allowMissing))
                return default;

            return new ParameterInfo(parameter, body);
        }

        internal static ParameterInfo Create(ParenthesizedLambdaExpressionSyntax parenthesizedLambda, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = parenthesizedLambda.ParameterList;

            if (!CheckParameterList(parameterList, allowMissing))
                return default;

            CSharpSyntaxNode body = parenthesizedLambda.Body;

            if (!Check(body, allowMissing))
                return default;

            return new ParameterInfo(parameterList, body);
        }

        internal static ParameterInfo Create(AnonymousMethodExpressionSyntax anonymousMethod, bool allowMissing = false)
        {
            ParameterListSyntax parameterList = anonymousMethod.ParameterList;

            if (!CheckParameterList(parameterList, allowMissing))
                return default;

            CSharpSyntaxNode body = anonymousMethod.Body;

            if (!Check(body, allowMissing))
                return default;

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
    }
}
