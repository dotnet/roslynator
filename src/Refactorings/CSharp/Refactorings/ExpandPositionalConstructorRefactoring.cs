// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    internal static class ExpandPositionalConstructorRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, RecordDeclarationSyntax recordDeclaration)
        {
            SeparatedSyntaxList<ParameterSyntax> parameters = recordDeclaration.ParameterList.Parameters;

            if (recordDeclaration.ParameterList?.Parameters.Count > 0)
            {
                context.RegisterRefactoring(
                    "Use explicit constructor",
                    ct => RefactorAsync(context.Document, recordDeclaration, ct),
                    RefactoringDescriptors.ExpandPositionalConstructor);
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            RecordDeclarationSyntax recordDeclaration,
            CancellationToken cancellationToken = default)
        {
            ParameterListSyntax parameterList = recordDeclaration.ParameterList;
            BaseListSyntax baseList = recordDeclaration.BaseList;
            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            var identifiersMap = new Dictionary<string, SyntaxToken>();
            var statements = new List<StatementSyntax>();
            var properties = new List<PropertyDeclarationSyntax>();
            var allAttributeLists = new List<AttributeListSyntax>();

            var baseType = baseList?
                .Types
                .FirstOrDefault(f => f.IsKind(SyntaxKind.PrimaryConstructorBaseType)) as PrimaryConstructorBaseTypeSyntax;

            ImmutableHashSet<string> basePropertyNames = ImmutableHashSet<string>.Empty;

            if (baseType != null)
            {
                basePropertyNames = baseType
                    .ArgumentList?
                    .Arguments
                    .Select(f => f.ToString())
                    .ToImmutableHashSet();
            }

            bool isWritable = !recordDeclaration.Modifiers.Contains(SyntaxKind.ReadOnlyKeyword)
                && recordDeclaration.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword);

            foreach (ParameterSyntax parameter in parameters)
            {
                SyntaxToken identifier = parameter.Identifier;
                string identifierText = identifier.ValueText;
                SyntaxToken parameterIdentifier = Identifier(StringUtility.FirstCharToLower(identifierText));
                SyntaxToken propertyIdentifier = Identifier(StringUtility.FirstCharToUpper(identifierText));
                identifiersMap.Add(identifier.ValueText, parameterIdentifier);

                IEnumerable<AttributeListSyntax> attributeLists = parameter.AttributeLists.Where(f => f.Target?.Identifier.IsKind(SyntaxKind.PropertyKeyword) == true);

                allAttributeLists.AddRange(attributeLists);

                statements.Add(SimpleAssignmentStatement(
                    IdentifierName(propertyIdentifier).QualifyWithThis(),
                    IdentifierName(parameterIdentifier.WithTriviaFrom(identifier))));

                if (!basePropertyNames.Contains(identifierText))
                {
                    properties.Add(PropertyDeclaration(
                        attributeLists.Select(f => f.WithTarget(null)).ToSyntaxList(),
                        Modifiers.Public(),
                        parameter.Type,
                        default(ExplicitInterfaceSpecifierSyntax),
                        propertyIdentifier,
                        AccessorList(
                            AutoGetAccessorDeclaration(),
                            (isWritable) ? AutoSetAccessorDeclaration() : AutoInitAccessorDeclaration()
                    )));
                }
            }

            ParameterListSyntax newParameterList = parameterList.RemoveNodes(
                allAttributeLists,
                SyntaxRemoveOptions.KeepLeadingTrivia | SyntaxRemoveOptions.KeepUnbalancedDirectives);

            newParameterList = newParameterList.ReplaceTokens(
                newParameterList.Parameters.Select(parameter => parameter.Identifier),
                (identifier, _) => identifiersMap[identifier.ValueText]);

            ParameterSyntax firstParameter = parameterList.Parameters.First();
            if (firstParameter.GetLeadingTrivia().SingleOrDefault(shouldThrow: false).IsKind(SyntaxKind.WhitespaceTrivia))
            {
                SyntaxTriviaList newIndentation = SyntaxTriviaAnalysis.GetIncreasedIndentationTriviaList(firstParameter, cancellationToken);

                newParameterList = newParameterList.ReplaceNodes(
                    newParameterList.Parameters,
                    (parameter, _) =>
                    {
                        return (parameter.GetLeadingTrivia().SingleOrDefault().IsKind(SyntaxKind.WhitespaceTrivia))
                            ? parameter.WithLeadingTrivia(newIndentation)
                            : parameter;
                    });
            }

            ConstructorDeclarationSyntax constructor = ConstructorDeclaration(
                Modifiers.Public(),
                recordDeclaration.Identifier.WithoutTrivia(),
                newParameterList,
                Block(statements));

            RecordDeclarationSyntax newRecord = recordDeclaration
                .WithIdentifier(recordDeclaration.Identifier.WithTrailingTrivia(recordDeclaration.ParameterList.GetTrailingTrivia()))
                .WithParameterList(null)
                .WithSemicolonToken(default);

            if (baseType != null)
            {
                SimpleBaseTypeSyntax newBaseType = SimpleBaseType(baseType.Type).WithTrailingTrivia(baseType.GetTrailingTrivia());

                BaseListSyntax newBaseList = baseList.ReplaceNode(baseType, newBaseType);

                ArgumentListSyntax newArgumentList = baseType.ArgumentList.ReplaceNodes(
                    baseType.ArgumentList.Arguments.Select(argument => argument.Expression),
                    (node, _) => IdentifierName(identifiersMap[node.ToString()]).WithTriviaFrom(node));

                ConstructorInitializerSyntax initializer = BaseConstructorInitializer(newArgumentList);

                constructor = constructor.WithInitializer(initializer);

                newRecord = newRecord.WithBaseList(newBaseList);
            }

            if (newRecord.OpenBraceToken.IsKind(SyntaxKind.None))
                newRecord = newRecord.WithOpenBraceToken(OpenBraceToken());

            if (newRecord.CloseBraceToken.IsKind(SyntaxKind.None))
                newRecord = newRecord.WithCloseBraceToken(CloseBraceToken());

            newRecord = MemberDeclarationInserter.Default.Insert(newRecord, constructor);

            int insertIndex = MemberDeclarationInserter.Default.GetInsertIndex(newRecord.Members, properties[0]);

            newRecord = newRecord.WithMembers(newRecord.Members.InsertRange(insertIndex, properties));

            return document.ReplaceNodeAsync(recordDeclaration, newRecord, cancellationToken);
        }
    }
}
