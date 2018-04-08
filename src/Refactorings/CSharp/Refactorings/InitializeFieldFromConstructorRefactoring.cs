// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InitializeFieldFromConstructorRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, FieldDeclarationSyntax fieldDeclaration)
        {
            if (!CanRefactor(fieldDeclaration))
                return;

            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = fieldDeclaration.Declaration.Variables;

            if (variables.Any(f => f.Initializer != null))
                return;

            context.RegisterRefactoring(
                GetTitle(variables.Count == 1),
                cancellationToken => RefactorAsync(context.Document, fieldDeclaration, cancellationToken));
        }

        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationListSelection selectedMembers)
        {
            int count = 0;

            foreach (MemberDeclarationSyntax selectedMember in selectedMembers)
            {
                if (selectedMember.Kind() != SyntaxKind.FieldDeclaration)
                    return;

                var fieldDeclaration = (FieldDeclarationSyntax)selectedMember;

                if (fieldDeclaration.Declaration.Variables.Any(f => f.Initializer != null))
                    return;

                if (!CanRefactor(fieldDeclaration))
                    return;

                count += fieldDeclaration.Declaration.Variables.Count;
            }

            context.RegisterRefactoring(
                GetTitle(count == 1),
                cancellationToken => RefactorAsync(context.Document, selectedMembers, cancellationToken));
        }

        private static string GetTitle(bool isSingle)
        {
            return (isSingle)
                ? "Initialize field from constructor"
                : "Initialize fields from constructor";
        }

        public static void ComputeRefactoring(RefactoringContext context, VariableDeclaratorSyntax variableDeclarator)
        {
            if (variableDeclarator.Initializer != null)
                return;

            if (!(variableDeclarator.Parent is VariableDeclarationSyntax variableDeclaration))
                return;

            if (!(variableDeclaration.Parent is FieldDeclarationSyntax fieldDeclaration))
                return;

            if (!CanRefactor(fieldDeclaration))
                return;

            FieldInfo fieldInfo = FieldInfo.Create(variableDeclaration.Type, variableDeclarator);

            context.RegisterRefactoring(
                "Initialize field from constructor",
                cancellationToken => RefactorAsync(context.Document, ImmutableArray.Create(fieldInfo), (TypeDeclarationSyntax)fieldDeclaration.Parent, cancellationToken));
        }

        private static bool CanRefactor(FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration.Modifiers.ContainsAny(SyntaxKind.StaticKeyword, SyntaxKind.ConstKeyword))
                return false;

            if (!(fieldDeclaration.Parent is TypeDeclarationSyntax typeDeclaration))
                return false;

            if (!typeDeclaration.IsKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
                return false;

            return typeDeclaration
                .Members
                .Any(f => f.Kind() == SyntaxKind.ConstructorDeclaration && !((ConstructorDeclarationSyntax)f).Modifiers.Contains(SyntaxKind.StaticKeyword));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            FieldDeclarationSyntax fieldDeclaration,
            CancellationToken cancellationToken)
        {
            VariableDeclarationSyntax declaration = fieldDeclaration.Declaration;

            ImmutableArray<FieldInfo> fieldInfo = declaration
                .Variables
                .Select(declarator => FieldInfo.Create(declaration.Type, declarator))
                .ToImmutableArray();

            return RefactorAsync(document, fieldInfo, (TypeDeclarationSyntax)fieldDeclaration.Parent, cancellationToken);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationListSelection selectedMembers,
            CancellationToken cancellationToken)
        {
            ImmutableArray<FieldInfo> fieldInfo = selectedMembers
                .Select(member => ((FieldDeclarationSyntax)member).Declaration)
                .SelectMany(declaration => declaration.Variables.Select(declarator => FieldInfo.Create(declaration.Type, declarator)))
                .ToImmutableArray();

            return RefactorAsync(document, fieldInfo, (TypeDeclarationSyntax)selectedMembers.Parent, cancellationToken);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ImmutableArray<FieldInfo> fieldInfos,
            TypeDeclarationSyntax typeDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxList<MemberDeclarationSyntax> members = typeDeclaration.Members;

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

                ConstructorInitializerSyntax initializer = null;
                ArgumentListSyntax argumentList = null;
                var arguments = default(SeparatedSyntaxList<ArgumentSyntax>);

                if (constructorDeclaration.Initializer?.Kind() == SyntaxKind.ThisConstructorInitializer)
                {
                    initializer = constructorDeclaration.Initializer;
                    argumentList = initializer.ArgumentList;
                    arguments = argumentList.Arguments;
                }

                SyntaxList<StatementSyntax> statements = body.Statements;

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    string parameterName = GetParameterName(fieldInfo.NameCamelCase, parameters, ref reservedNames);

                    parameters = parameters.Add(Parameter(fieldInfo.Type.WithoutTrivia(), parameterName));

                    if (initializer != null)
                        arguments = arguments.Add(Argument(IdentifierName(parameterName)));

                    statements = statements.Add(
                        SimpleAssignmentStatement(
                            SimpleMemberAccessExpression(ThisExpression(), IdentifierName(fieldInfo.Name)).WithSimplifierAnnotation(),
                            IdentifierName(parameterName)).WithFormatterAnnotation());
                }

                parameterList = parameterList.WithParameters(parameters).WithFormatterAnnotation();

                if (initializer != null)
                {
                    initializer = initializer
                        .WithArgumentList(argumentList.WithArguments(arguments))
                        .WithFormatterAnnotation();
                }

                body = body.WithStatements(statements);

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

        private readonly struct FieldInfo
        {
            private FieldInfo(TypeSyntax type, string name, string nameCamelCase)
            {
                Type = type;
                Name = name;
                NameCamelCase = nameCamelCase;
            }

            public static FieldInfo Create(TypeSyntax type, VariableDeclaratorSyntax variableDeclarator)
            {
                string name = variableDeclarator.Identifier.ValueText;
                string nameCamelCase = StringUtility.ToCamelCase(name);

                return new FieldInfo(type, name, nameCamelCase);
            }

            public TypeSyntax Type { get; }

            public string Name { get; }

            public string NameCamelCase { get; }
        }
    }
}
