// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Utilities;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InitializeFieldFromConstructorRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, VariableDeclaratorSyntax variableDeclarator)
        {
            if (variableDeclarator.Initializer != null)
                return;

            if (!variableDeclarator.IsParentKind(SyntaxKind.VariableDeclaration))
                return;

            if (!(variableDeclarator.Parent.Parent is FieldDeclarationSyntax fieldDeclaration))
                return;

            if (fieldDeclaration.Modifiers.ContainsAny(SyntaxKind.StaticKeyword, SyntaxKind.ConstKeyword))
                return;

            if (!(fieldDeclaration.Parent is TypeDeclarationSyntax typeDeclaration))
                return;

            if (!typeDeclaration.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
                return;

            if (!typeDeclaration
                .Members
                .Any(f => f.Kind() == SyntaxKind.ConstructorDeclaration && !((ConstructorDeclarationSyntax)f).Modifiers.Contains(SyntaxKind.StaticKeyword)))
            {
                return;
            }

            context.RegisterRefactoring(
                "Initialize field from constructor",
                cancellationToken => RefactorAsync(context.Document, variableDeclarator, typeDeclaration, cancellationToken));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            VariableDeclaratorSyntax variableDeclarator,
            TypeDeclarationSyntax typeDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxList<MemberDeclarationSyntax> members = typeDeclaration.Members;

            string name = variableDeclarator.Identifier.ValueText;
            string camelCaseName = StringUtility.ToCamelCase(name);

            HashSet<string> reservedNames = null;

            for (int i = 0; i < members.Count; i++)
            {
                if (!(members[i] is ConstructorDeclarationSyntax constructorDeclaration))
                    continue;

                if (constructorDeclaration.Modifiers.Contains(SyntaxKind.StaticKeyword))
                    continue;

                ParameterListSyntax parameterList = constructorDeclaration.ParameterList;

                if (parameterList == null)
                    continue;

                BlockSyntax body = constructorDeclaration.Body;

                if (body == null)
                    continue;

                SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

                reservedNames?.Clear();

                string parameterName = GetParameterName(camelCaseName, parameters, ref reservedNames);

                ParameterSyntax parameter = Parameter(((VariableDeclarationSyntax)variableDeclarator.Parent).Type.WithoutTrivia(), parameterName);

                parameterList = parameterList.WithParameters(parameters.Add(parameter)).WithFormatterAnnotation();

                ConstructorInitializerSyntax initializer = constructorDeclaration.Initializer;

                if (initializer?.Kind() == SyntaxKind.ThisConstructorInitializer)
                {
                    ArgumentListSyntax argumentList = initializer.ArgumentList;

                    if (argumentList != null)
                    {
                        SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

                        initializer = initializer.WithArgumentList(
                            argumentList.WithArguments(
                                arguments.Add(Argument(IdentifierName(parameterName))))).WithFormatterAnnotation();
                    }
                }

                body = body.WithStatements(
                    body.Statements.Add(
                        SimpleAssignmentStatement(
                            SimpleMemberAccessExpression(ThisExpression(), IdentifierName(name)).WithSimplifierAnnotation(),
                            IdentifierName(parameterName)).WithFormatterAnnotation()));

                constructorDeclaration = constructorDeclaration.Update(
                    constructorDeclaration.AttributeLists,
                    constructorDeclaration.Modifiers,
                    constructorDeclaration.Identifier,
                    parameterList,
                    initializer,
                    body,
                    constructorDeclaration.SemicolonToken);

                members = members.ReplaceAt(i, constructorDeclaration);
            }

            TypeDeclarationSyntax newNode = typeDeclaration.WithMembers(members);

            return document.ReplaceNodeAsync(typeDeclaration, newNode, cancellationToken);
        }

        private static string GetParameterName(
            string name,
            SeparatedSyntaxList<ParameterSyntax> parameters,
            ref HashSet<string> reservedNames)
        {
            bool isConflict = false;

            foreach (ParameterSyntax parameter in parameters)
            {
                if (string.Equals(name, parameter.Identifier.ValueText, StringComparison.Ordinal))
                {
                    isConflict = true;
                    break;
                }
            }

            if (!isConflict)
                return name;

            if (reservedNames == null)
                reservedNames = new HashSet<string>();

            foreach (ParameterSyntax parameter in parameters)
                reservedNames.Add(parameter.Identifier.ValueText);

            return NameGenerator.Default.EnsureUniqueName(name, reservedNames);
        }
    }
}
