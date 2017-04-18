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

            IFieldSymbol fieldSymbol = GetBackingFieldSymbol(property, context.SemanticModel, context.CancellationToken);

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
                            DiagnosticDescriptors.UseAutoProperty,
                            property);

                        if (property.ExpressionBody != null)
                        {
                            context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, property.ExpressionBody);
                        }
                        else
                        {
                            AccessorDeclarationSyntax getter = property.Getter();

                            if (getter != null)
                                FadeOutBodyOrExpressionBody(context, getter);

                            AccessorDeclarationSyntax setter = property.Setter();

                            if (setter != null)
                                FadeOutBodyOrExpressionBody(context, setter);
                        }
                    }
                }
            }
        }

        private static void FadeOutBodyOrExpressionBody(SyntaxNodeAnalysisContext context, AccessorDeclarationSyntax getter)
        {
            BlockSyntax body = getter.Body;

            if (body != null)
            {
                StatementSyntax statement = body.Statements.First();

                switch (statement.Kind())
                {
                    case SyntaxKind.ReturnStatement:
                        {
                            context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, ((ReturnStatementSyntax)statement).Expression);
                            break;
                        }
                    case SyntaxKind.ExpressionStatement:
                        {
                            context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, ((ExpressionStatementSyntax)statement).Expression);
                            break;
                        }
                }

                context.ReportBraces(DiagnosticDescriptors.UseAutoPropertyFadeOut, body);
            }
            else
            {
                context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, getter.ExpressionBody);
            }
        }

        private static IFieldSymbol GetBackingFieldSymbol(
            PropertyDeclarationSyntax property,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ArrowExpressionClauseSyntax expressionBody = property.ExpressionBody;

            if (expressionBody != null)
            {
                NameSyntax identifier = GetIdentifierFromExpression(expressionBody.Expression);

                if (identifier != null)
                    return GetBackingFieldSymbol(identifier, semanticModel, cancellationToken);
            }
            else
            {
                AccessorDeclarationSyntax getter = property.Getter();

                if (getter != null)
                {
                    AccessorDeclarationSyntax setter = property.Setter();

                    if (setter != null)
                    {
                        return GetBackingFieldSymbol(getter, setter, semanticModel, cancellationToken);
                    }
                    else
                    {
                        NameSyntax identifier = GetIdentifierFromGetter(getter);

                        if (identifier != null)
                            return GetBackingFieldSymbol(identifier, semanticModel, cancellationToken);
                    }
                }
            }

            return null;
        }

        private static IFieldSymbol GetBackingFieldSymbol(
            NameSyntax identifier,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = semanticModel.GetSymbol(identifier, cancellationToken);

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
            AccessorDeclarationSyntax getter,
            AccessorDeclarationSyntax setter,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            NameSyntax getterIdentifier = GetIdentifierFromGetter(getter);

            if (getterIdentifier != null)
            {
                NameSyntax setterIdentifier = GetIdentifierFromSetter(setter);

                if (setterIdentifier != null)
                {
                    ISymbol symbol = semanticModel.GetSymbol(getterIdentifier, cancellationToken);

                    if (symbol?.IsField() == true
                        && symbol.IsPrivate())
                    {
                        ISymbol symbol2 = semanticModel.GetSymbol(setterIdentifier, cancellationToken);

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
                else
                {
                    return GetIdentifierFromExpression(getter.ExpressionBody?.Expression);
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

                        return GetIdentifierFromSetterExpression(expression);
                    }
                }
                else
                {
                    return GetIdentifierFromSetterExpression(setter.ExpressionBody.Expression);
                }
            }

            return null;
        }

        private static NameSyntax GetIdentifierFromSetterExpression(ExpressionSyntax expression)
        {
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
            ArrowExpressionClauseSyntax expressionBody = property.ExpressionBody;

            if (expressionBody != null)
            {
                if (expressionBody.SpanContainsDirectives())
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

            ImmutableArray<SyntaxNode> oldNodes = await document.FindNodesAsync(fieldSymbol, cancellationToken: cancellationToken).ConfigureAwait(false);

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

                newParentMember = newParentMember.WithMembers(members);
            }

            members = newParentMember.GetMembers();

            property = (PropertyDeclarationSyntax)members[propertyIndex];

            PropertyDeclarationSyntax newProperty = CreateAutoProperty(property, declarator.Initializer);

            members = members.Replace(property, newProperty);

            newParentMember = newParentMember.WithMembers(members);

            return await document.ReplaceNodeAsync(parentMember, newParentMember, cancellationToken).ConfigureAwait(false);
        }

        private static ISymbol GetFieldSymbol(PropertyDeclarationSyntax property, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ArrowExpressionClauseSyntax expressionBody = property.ExpressionBody;

            if (expressionBody != null)
            {
                return semanticModel.GetSymbol(expressionBody.Expression, cancellationToken);
            }
            else
            {
                AccessorDeclarationSyntax getter = property.Getter();

                BlockSyntax body = getter.Body;

                if (body != null)
                {
                    var returnStatement = (ReturnStatementSyntax)body.Statements[0];

                    return semanticModel.GetSymbol(returnStatement.Expression, cancellationToken);
                }
                else
                {
                    return semanticModel.GetSymbol(getter.ExpressionBody.Expression, cancellationToken);
                }
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
                            .WithExpressionBody(null)
                            .WithSemicolonToken(SemicolonToken())
                            .WithTriviaFrom(accessor);
                    });

                return accessorList.WithAccessors(List(newAccessors));
            }
        }
    }
}
