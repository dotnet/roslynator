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
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseAutoImplementedPropertyRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax property)
        {
            IFieldSymbol fieldSymbol = GetBackingFieldSymbol(context, property);

            if (fieldSymbol != null)
            {
                var declarator = (VariableDeclaratorSyntax)fieldSymbol.DeclaringSyntaxReferences[0].GetSyntax(context.CancellationToken);

                if (declarator.SyntaxTree.Equals(property.SyntaxTree))
                {
                    IPropertySymbol propertySymbol = context
                        .SemanticModel
                        .GetDeclaredSymbol(property, context.CancellationToken);

                    if (propertySymbol != null
                        && propertySymbol.IsStatic == fieldSymbol.IsStatic
                        && propertySymbol.Type == fieldSymbol.Type
                        && propertySymbol.ContainingType?.Equals(fieldSymbol.ContainingType) == true
                        && CheckPreprocessorDirectives(property, declarator))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.UseAutoImplementedProperty,
                            property);

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

            if (symbol.IsPrivateField())
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

                    if (symbol.IsPrivateField())
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
            if (getter != null
                && getter.Body != null
                && getter.Body.Statements.Count == 1
                && getter.Body.Statements[0].IsKind(SyntaxKind.ReturnStatement))
            {
                var returnStatement = (ReturnStatementSyntax)getter.Body.Statements[0];

                return GetIdentifierFromExpression(returnStatement.Expression);
            }

            return null;
        }

        private static NameSyntax GetIdentifierFromSetter(AccessorDeclarationSyntax setter)
        {
            if (setter != null
                && setter.Body != null
                && setter.Body.Statements.Count == 1
                && setter.Body.Statements[0].IsKind(SyntaxKind.ExpressionStatement))
            {
                var statement = (ExpressionStatementSyntax)setter.Body.Statements[0];

                if (statement.Expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                {
                    var assignment = (AssignmentExpressionSyntax)statement.Expression;

                    if (assignment.Right?.IsKind(SyntaxKind.IdentifierName) == true
                        && ((IdentifierNameSyntax)assignment.Right).Identifier.ValueText == "value")
                    {
                        return GetIdentifierFromExpression(assignment.Left);
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
            DiagnosticDescriptor descriptor = DiagnosticDescriptors.UseAutoImplementedPropertyFadeOut;

            if (property.ExpressionBody != null)
            {
                context.ReportNode(descriptor, property.ExpressionBody);
            }
            else
            {
                AccessorDeclarationSyntax getter = property.Getter();

                if (getter != null)
                    context.ReportNode(descriptor, getter.Body);

                AccessorDeclarationSyntax setter = property.Setter();

                if (setter != null)
                    context.ReportNode(descriptor, setter.Body);
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax property,
            CancellationToken cancellationToken)
        {
            var parentMember = (MemberDeclarationSyntax)property.Parent;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ISymbol fieldSymbol = GetFieldSymbol(property, semanticModel, cancellationToken);

            var declarator = (VariableDeclaratorSyntax)await fieldSymbol.DeclaringSyntaxReferences[0].GetSyntaxAsync(cancellationToken).ConfigureAwait(false);

            var variableDeclaration = (VariableDeclarationSyntax)declarator.Parent;

            SyntaxList<MemberDeclarationSyntax> members = parentMember.GetMembers();

            int propertyIndex = members.IndexOf(property);

            int fieldIndex = members.IndexOf((FieldDeclarationSyntax)variableDeclaration.Parent);

            ImmutableArray<SyntaxNode> oldNodes = await document.FindSymbolNodesAsync(fieldSymbol, cancellationToken).ConfigureAwait(false);

            IdentifierNameSyntax newNode = IdentifierName(property.Identifier);

            MemberDeclarationSyntax newParentMember = parentMember.ReplaceNodes(oldNodes, (f, g) => newNode.WithTriviaFrom(f));

            members = newParentMember.GetMembers();

            if (variableDeclaration.Variables.Count == 1)
            {
                newParentMember = newParentMember.RemoveNode(
                    newParentMember.GetMemberAt(fieldIndex),
                    SyntaxRemoveOptions.KeepUnbalancedDirectives);

                if (propertyIndex > fieldIndex)
                    propertyIndex--;
            }
            else
            {
                var field = (FieldDeclarationSyntax)members[fieldIndex];

                FieldDeclarationSyntax newField = field.RemoveNode(
                    field.Declaration.Variables[variableDeclaration.Variables.IndexOf(declarator)],
                    SyntaxRemoveOptions.KeepUnbalancedDirectives);

                members = members.Replace(field, newField.WithFormatterAnnotation());

                newParentMember = newParentMember.SetMembers(members);
            }

            members = newParentMember.GetMembers();

            property = (PropertyDeclarationSyntax)members[propertyIndex];

            PropertyDeclarationSyntax newProperty = CreateAutoProperty(property, declarator.Initializer);

            members = members.Replace(property, newProperty);

            newParentMember = newParentMember.SetMembers(members);

            return await document.ReplaceNodeAsync(parentMember, newParentMember, cancellationToken).ConfigureAwait(false);
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
                accessorList = Remover.RemoveWhitespaceOrEndOfLine(accessorList);
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
                return AccessorList(AutoImplementedGetter())
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
