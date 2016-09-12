// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class ReplacePropertyWithMethodRefactoring
    {
        private static readonly string[] _prefixes = new string[]
        {
            "Is",
            "Has",
            "Are",
            "Can",
            "Allow",
            "Supports",
            "Should",
            "Get",
            "Set"
        };

        public static bool CanRefactor(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            AccessorListSyntax accessorList = propertyDeclaration.AccessorList;

            if (accessorList != null)
            {
                SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

                if (accessors.Any())
                {
                    if (accessors.All(f => f.Body != null))
                    {
                        return true;
                    }
                    else if (context.SupportsCSharp6
                        && accessors.Count == 1
                        && accessors.First().IsAutoGetter()
                        && propertyDeclaration.Initializer?.Value != null)
                    {
                        return true;
                    }
                }
            }

            return false;
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

            string methodName = method.Identifier.ValueText;

            bool addPrefix = !_prefixes.Any(prefix => TextUtility.HasPrefix(methodName, prefix));

            if (getter != null)
            {
                string name = methodName;

                if (addPrefix)
                    name = "Get" + methodName;

                BlockSyntax body = getter.Body;

                BlockSyntax methodBody = (body != null)
                    ? Block(body.Statements)
                    : Block(ReturnStatement(property.Initializer.Value));

                MethodDeclarationSyntax getMethod = method
                    .WithIdentifier(Identifier(name))
                    .WithBody(methodBody)
                    .WithLeadingTrivia(property.GetLeadingTrivia())
                    .WithFormatterAnnotation();

                if (body != null)
                    getMethod = SetAccessModifiers(getter, getMethod);

                if (setter == null)
                    getMethod = getMethod.WithTrailingTrivia(property.GetTrailingTrivia());

                methods.Add(getMethod);
            }

            if (setter != null)
            {
                string name = methodName;

                if (addPrefix)
                    name = "Set" + methodName;

                ParameterSyntax parameter = Parameter(Identifier("value"))
                    .WithType(property.Type);

                MethodDeclarationSyntax setMethod = method
                    .WithReturnType(VoidType())
                    .WithIdentifier(Identifier(name))
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
            SyntaxTokenList accessorModifiers = accessor.Modifiers;

            if (accessorModifiers.Any())
            {
                SyntaxTokenList newModifiers = accessorModifiers
                    .Concat(method.Modifiers.Where(f => !ModifierUtility.IsAccessModifier(f)))
                    .ToSyntaxTokenList();

                return method.WithModifiers(newModifiers);
            }

            return method;
        }
    }
}
