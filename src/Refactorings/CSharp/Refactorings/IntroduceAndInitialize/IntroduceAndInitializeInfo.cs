// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.IntroduceAndInitialize
{
    internal abstract class IntroduceAndInitializeInfo
    {
        protected IntroduceAndInitializeInfo(ParameterSyntax parameter)
        {
            Parameter = parameter;
            ParameterName = parameter.Identifier.ValueText;
        }

        public ParameterSyntax Parameter { get; }

        public string ParameterName { get; }

        public abstract string Name { get; }

        public abstract SyntaxKind Kind { get; }

        public TypeSyntax Type
        {
            get { return Parameter.Type?.WithoutTrivia(); }
        }

        public abstract MemberDeclarationSyntax CreateDeclaration();

        public virtual ExpressionStatementSyntax CreateAssignment()
        {
            return SimpleAssignmentStatement(
                IdentifierName(Name).QualifyWithThis(),
                IdentifierName(Parameter.Identifier.WithoutTrivia()));
        }
    }
}
