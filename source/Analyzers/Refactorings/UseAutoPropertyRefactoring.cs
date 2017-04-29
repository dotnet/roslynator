// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseAutoPropertyRefactoring
    {
        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;

            if (!property.ContainsDiagnostics)
            {
                IFieldSymbol fieldSymbol = GetBackingFieldSymbol(context, property);

                if (fieldSymbol != null)
                {
                    IPropertySymbol propertySymbol = context.SemanticModel.GetDeclaredSymbol(property, context.CancellationToken);

                    if (propertySymbol != null
                        && propertySymbol.IsStatic == fieldSymbol.IsStatic
                        && propertySymbol.Type.Equals(fieldSymbol.Type)
                        && propertySymbol.ContainingType?.Equals(fieldSymbol.ContainingType) == true
                        && CheckPreprocessorDirectives(property, (VariableDeclaratorSyntax)fieldSymbol.GetSyntax(context.CancellationToken)))
                    {
                        context.ReportDiagnostic(DiagnosticDescriptors.UseAutoProperty, property.Identifier);

                        FadeOut(context, property);
                    }
                }
            }
        }

        private static IFieldSymbol GetBackingFieldSymbol(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property)
        {
            if (property.ExpressionBody != null)
            {
                NameSyntax identifier = GetIdentifierFromExpression(property.ExpressionBody.Expression);

                if (identifier != null)
                    return GetBackingFieldSymbol(context, identifier);
            }
            else
            {
                AccessorDeclarationSyntax getter = property.Getter();

                if (getter != null)
                {
                    AccessorDeclarationSyntax setter = property.Setter();

                    if (setter != null)
                    {
                        return GetBackingFieldSymbol(context, getter, setter);
                    }
                    else
                    {
                        NameSyntax identifier = GetIdentifierFromGetter(getter);

                        if (identifier != null)
                            return GetBackingFieldSymbol(context, identifier);
                    }
                }
            }

            return null;
        }

        private static IFieldSymbol GetBackingFieldSymbol(
            SyntaxNodeAnalysisContext context,
            NameSyntax identifier)
        {
            ISymbol symbol = context.SemanticModel.GetSymbol(identifier, context.CancellationToken);

            if (symbol.IsPrivate()
                && symbol.IsField())
            {
                var fieldSymbol = (IFieldSymbol)symbol;

                if (fieldSymbol.IsReadOnly)
                    return fieldSymbol;
            }

            return null;
        }

        private static IFieldSymbol GetBackingFieldSymbol(
            SyntaxNodeAnalysisContext context,
            AccessorDeclarationSyntax getter,
            AccessorDeclarationSyntax setter)
        {
            NameSyntax getterIdentifier = GetIdentifierFromGetter(getter);

            if (getterIdentifier != null)
            {
                NameSyntax setterIdentifier = GetIdentifierFromSetter(setter);

                if (setterIdentifier != null)
                {
                    ISymbol symbol = context.SemanticModel.GetSymbol(getterIdentifier, context.CancellationToken);

                    if (symbol?.IsField() == true
                        && symbol.IsPrivate())
                    {
                        ISymbol symbol2 = context.SemanticModel.GetSymbol(setterIdentifier, context.CancellationToken);

                        if (symbol.Equals(symbol2))
                            return (IFieldSymbol)symbol;
                    }
                }
            }

            return null;
        }

        private static NameSyntax GetIdentifierFromGetter(AccessorDeclarationSyntax getter)
        {
            if (getter != null)
            {
                BlockSyntax body = getter.Body;

                if (body != null)
                {
                    SyntaxList<StatementSyntax> statements = body.Statements;

                    if (statements.Count == 1
                        && statements[0].IsKind(SyntaxKind.ReturnStatement))
                    {
                        var returnStatement = (ReturnStatementSyntax)statements[0];

                        return GetIdentifierFromExpression(returnStatement.Expression);
                    }
                }
            }

            return null;
        }

        private static NameSyntax GetIdentifierFromSetter(AccessorDeclarationSyntax setter)
        {
            if (setter != null)
            {
                BlockSyntax body = setter.Body;

                if (body != null)
                {
                    SyntaxList<StatementSyntax> statements = body.Statements;

                    if (statements.Count == 1
                        && statements[0].IsKind(SyntaxKind.ExpressionStatement))
                    {
                        var statement = (ExpressionStatementSyntax)statements[0];

                        ExpressionSyntax expression = statement.Expression;

                        if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                        {
                            var assignment = (AssignmentExpressionSyntax)expression;

                            ExpressionSyntax right = assignment.Right;

                            if (right?.IsKind(SyntaxKind.IdentifierName) == true
                                && ((IdentifierNameSyntax)right).Identifier.ValueText == "value")
                            {
                                return GetIdentifierFromExpression(assignment.Left);
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static SimpleNameSyntax GetIdentifierFromExpression(ExpressionSyntax expression)
        {
            switch (expression?.Kind())
            {
                case SyntaxKind.IdentifierName:
                    {
                        return (IdentifierNameSyntax)expression;
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)expression;

                        if (memberAccess.Expression?.IsKind(SyntaxKind.ThisExpression) == true)
                            return memberAccess.Name;

                        break;
                    }
            }

            return null;
        }

        private static bool CheckPreprocessorDirectives(
            PropertyDeclarationSyntax property,
            VariableDeclaratorSyntax declarator)
        {
            if (property.ExpressionBody != null)
            {
                if (property.ExpressionBody.SpanContainsDirectives())
                    return false;
            }
            else if (property.AccessorList.Accessors.Any(f => f.SpanContainsDirectives()))
            {
                return false;
            }

            var variableDeclaration = (VariableDeclarationSyntax)declarator.Parent;

            if (variableDeclaration.Variables.Count == 1)
            {
                if (variableDeclaration.Parent.SpanContainsDirectives())
                    return false;
            }
            else if (declarator.SpanContainsDirectives())
            {
                return false;
            }

            return true;
        }

        private static void FadeOut(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property)
        {
            if (property.ExpressionBody != null)
            {
                context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, property.ExpressionBody);
            }
            else
            {
                AccessorDeclarationSyntax getter = property.Getter();

                if (getter != null)
                    context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, getter.Body);

                AccessorDeclarationSyntax setter = property.Setter();

                if (setter != null)
                    context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, setter.Body);
            }
        }

        public static async Task<Solution> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            Solution solution = document.Solution();

            IdentifierNameSyntax newIdentifier = IdentifierName(propertyDeclaration.Identifier);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

            ISymbol fieldSymbol = GetFieldSymbol(propertyDeclaration, semanticModel, cancellationToken);

            var variableDeclarator = (VariableDeclaratorSyntax)await fieldSymbol.DeclaringSyntaxReferences[0].GetSyntaxAsync(cancellationToken).ConfigureAwait(false);

            var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

            var fieldDeclaration = (FieldDeclarationSyntax)variableDeclaration.Parent;

            var newDocuments = new List<Document>();

            foreach (SyntaxReference syntaxReference in propertySymbol.ContainingType.DeclaringSyntaxReferences)
            {
                var containingMember = (MemberDeclarationSyntax)await syntaxReference.GetSyntaxAsync(cancellationToken).ConfigureAwait(false);

                SyntaxList<MemberDeclarationSyntax> members = containingMember.GetMembers();

                SyntaxTree syntaxTree = containingMember.SyntaxTree;

                document = solution.GetDocument(syntaxTree);
                semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                int fieldIndex = -1;

                if (variableDeclarator.SyntaxTree == syntaxTree)
                    fieldIndex = members.IndexOf(fieldDeclaration);

                int propertyIndex = -1;

                if (propertyDeclaration.SyntaxTree == syntaxTree)
                    propertyIndex = members.IndexOf(propertyDeclaration);

                ImmutableArray<SyntaxNode> oldNodes = await document.FindNodesAsync(fieldSymbol, cancellationToken: cancellationToken).ConfigureAwait(false);

                MemberDeclarationSyntax newContainingMember = containingMember.ReplaceNodes(oldNodes, (f, g) => newIdentifier.WithTriviaFrom(f));

                members = newContainingMember.GetMembers();

                if (fieldIndex != -1)
                {
                    if (variableDeclaration.Variables.Count == 1)
                    {
                        newContainingMember = newContainingMember.RemoveNode(
                            newContainingMember.GetMemberAt(fieldIndex),
                            SyntaxRemoveOptions.KeepUnbalancedDirectives);

                        if (propertyIndex != -1
                            && propertyIndex > fieldIndex)
                        {
                            propertyIndex--;
                        }
                    }
                    else
                    {
                        var field = (FieldDeclarationSyntax)members[fieldIndex];

                        FieldDeclarationSyntax newField = field.RemoveNode(
                            field.Declaration.Variables[variableDeclaration.Variables.IndexOf(variableDeclarator)],
                            SyntaxRemoveOptions.KeepUnbalancedDirectives);

                        members = members.Replace(field, newField.WithFormatterAnnotation());

                        newContainingMember = newContainingMember.WithMembers(members);
                    }
                }

                members = newContainingMember.GetMembers();

                if (propertyIndex != -1)
                {
                    var property = (PropertyDeclarationSyntax)members[propertyIndex];

                    PropertyDeclarationSyntax newProperty = CreateAutoProperty(property, variableDeclarator.Initializer);

                    members = members.Replace(property, newProperty);

                    newContainingMember = newContainingMember.WithMembers(members);
                }

                newDocuments.Add(await document.ReplaceNodeAsync(containingMember, newContainingMember).ConfigureAwait(false));
            }

            foreach (Document newDocument in newDocuments)
                solution = solution.WithDocumentSyntaxRoot(newDocument.Id, await newDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false));

            return solution;
        }

        private static ISymbol GetFieldSymbol(PropertyDeclarationSyntax property, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (property.ExpressionBody != null)
            {
                return semanticModel.GetSymbol(property.ExpressionBody.Expression, cancellationToken);
            }
            else
            {
                var returnStatement = (ReturnStatementSyntax)property.Getter().Body.Statements[0];

                return semanticModel.GetSymbol(returnStatement.Expression, cancellationToken);
            }
        }

        private static PropertyDeclarationSyntax CreateAutoProperty(PropertyDeclarationSyntax property, EqualsValueClauseSyntax initializer)
        {
            AccessorListSyntax accessorList = CreateAccessorList(property);

            if (accessorList
                .DescendantTrivia()
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                accessorList = accessorList.RemoveWhitespaceOrEndOfLineTrivia();
            }

            PropertyDeclarationSyntax newProperty = property
                .WithIdentifier(property.Identifier.WithTrailingTrivia(Space))
                .WithExpressionBody(null)
                .WithAccessorList(accessorList);

            if (initializer != null)
            {
                newProperty = newProperty
                    .WithInitializer(initializer)
                    .WithSemicolonToken(SemicolonToken());
            }
            else
            {
                newProperty = newProperty.WithoutSemicolonToken();
            }

            return newProperty
                .WithTriviaFrom(property)
                .WithFormatterAnnotation();
        }

        private static AccessorListSyntax CreateAccessorList(PropertyDeclarationSyntax property)
        {
            if (property.ExpressionBody != null)
            {
                return AccessorList(AutoGetAccessorDeclaration())
                    .WithTriviaFrom(property.ExpressionBody);
            }
            else
            {
                AccessorListSyntax accessorList = property.AccessorList;

                IEnumerable<AccessorDeclarationSyntax> newAccessors = accessorList
                    .Accessors
                    .Select(accessor =>
                    {
                        return accessor
                            .WithBody(null)
                            .WithSemicolonToken(SemicolonToken())
                            .WithTriviaFrom(accessor);
                    });

                return accessorList.WithAccessors(List(newAccessors));
            }
        }
    }
}
