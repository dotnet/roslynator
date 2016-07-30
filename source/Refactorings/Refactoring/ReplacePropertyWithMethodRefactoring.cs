// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ReplacePropertyWithMethodRefactoring
    {
        public static bool CanRefactor(PropertyDeclarationSyntax propertyDeclaration)
        {
            return propertyDeclaration.AccessorList?.Accessors.Count > 0
                && propertyDeclaration.AccessorList.Accessors.All(f => f.Body != null);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax property,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            MethodDeclarationSyntax method = MethodDeclaration(
                property.AttributeLists,
                property.Modifiers,
                property.Type,
                property.ExplicitInterfaceSpecifier,
                property.Identifier.WithTrailingTrivia(),
                null,
                ParameterList(),
                List<TypeParameterConstraintClauseSyntax>(),
                body: null,
                expressionBody: null);

            var methods = new List<MethodDeclarationSyntax>();

            AccessorDeclarationSyntax getter = property.Getter();
            AccessorDeclarationSyntax setter = property.Setter();

            if (getter != null)
            {
                MethodDeclarationSyntax getMethod = method
                    .WithIdentifier(Identifier("Get" + method.Identifier))
                    .WithBody(Block(getter.Body.Statements))
                    .WithLeadingTrivia(property.GetLeadingTrivia())
                    .WithFormatterAnnotation();

                getMethod = SetAccessModifiers(getter, getMethod);

                if (setter == null)
                    getMethod = getMethod.WithTrailingTrivia(property.GetTrailingTrivia());

                methods.Add(getMethod);
            }

            if (setter != null)
            {
                ParameterSyntax parameter = Parameter(Identifier("value"))
                    .WithType(property.Type);

                MethodDeclarationSyntax setMethod = method
                    .WithReturnType(VoidType())
                    .WithIdentifier(Identifier("Set" + method.Identifier))
                    .WithParameterList(method.ParameterList.AddParameters(parameter))
                    .WithBody(Block(setter.Body.Statements))
                    .WithTrailingTrivia(property.GetTrailingTrivia())
                    .WithFormatterAnnotation();

                setMethod = SetAccessModifiers(setter, setMethod);

                if (getter == null)
                    setMethod = setMethod.WithLeadingTrivia(property.GetLeadingTrivia());

                if (getter != null)
                    setMethod = setMethod.WithLeadingTrivia(setMethod.GetLeadingTrivia().Insert(0, NewLine));

                methods.Add(setMethod);
            }

            root = root.ReplaceNode(property, methods);

            return document.WithSyntaxRoot(root);
        }

        private static MethodDeclarationSyntax SetAccessModifiers(AccessorDeclarationSyntax accessor, MethodDeclarationSyntax method)
        {
            if (accessor.Modifiers.Any())
            {
                SyntaxTokenList modifiers = method.Modifiers
                    .RemoveAccessModifiers()
                    .InsertRange(0, accessor.Modifiers);

                return method.WithModifiers(modifiers);
            }

            return method;
        }
    }
}
