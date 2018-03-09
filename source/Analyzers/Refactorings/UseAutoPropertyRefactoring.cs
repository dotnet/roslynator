// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
        private static readonly SyntaxAnnotation _removeAnnotation = new SyntaxAnnotation();

        private static readonly SymbolDisplayFormat _symbolDisplayFormat = new SymbolDisplayFormat(
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;

            if (property.ContainsDiagnostics)
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            IFieldSymbol fieldSymbol = null;

            AccessorDeclarationSyntax getter = null;
            AccessorDeclarationSyntax setter = null;

            ArrowExpressionClauseSyntax expressionBody = property.ExpressionBody;

            if (expressionBody != null)
            {
                fieldSymbol = GetBackingFieldSymbol(expressionBody, semanticModel, cancellationToken);
            }
            else
            {
                getter = property.Getter();

                if (getter != null)
                {
                    setter = property.Setter();

                    if (setter != null)
                    {
                        fieldSymbol = GetBackingFieldSymbol(getter, setter, semanticModel, cancellationToken);
                    }
                    else
                    {
                        fieldSymbol = GetBackingFieldSymbol(getter, semanticModel, cancellationToken);
                    }
                }
            }

            if (fieldSymbol == null)
                return;

            var variableDeclarator = (VariableDeclaratorSyntax)fieldSymbol.GetSyntax(cancellationToken);

            if (variableDeclarator.SyntaxTree != property.SyntaxTree)
                return;

            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(property, cancellationToken);

            if (propertySymbol == null)
                return;

            if (!propertySymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty)
                return;

            if (propertySymbol.IsStatic != fieldSymbol.IsStatic)
                return;

            if (!propertySymbol.Type.Equals(fieldSymbol.Type))
                return;

            if (propertySymbol.ContainingType?.Equals(fieldSymbol.ContainingType) != true)
                return;

            if (setter == null
                && propertySymbol.IsOverride
                && propertySymbol.OverriddenProperty?.SetMethod != null)
            {
                return;
            }

            if (HasStructLayoutAttributeWithExplicitKind(propertySymbol.ContainingType, context.Compilation))
                return;

            if (IsBackingFieldUsedInRefOrOutArgument(context, fieldSymbol, property))
                return;

            if (!CheckPreprocessorDirectives(property, variableDeclarator))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UseAutoProperty, property.Identifier);

            if (property.ExpressionBody != null)
            {
                context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, property.ExpressionBody);
            }
            else
            {
                if (getter != null)
                    FadeOutBodyOrExpressionBody(context, getter);

                if (setter != null)
                    FadeOutBodyOrExpressionBody(context, setter);
            }
        }

        private static bool IsBackingFieldUsedInRefOrOutArgument(
            SyntaxNodeAnalysisContext context,
            IFieldSymbol fieldSymbol,
            PropertyDeclarationSyntax propertyDeclaration)
        {
            ImmutableArray<SyntaxReference> syntaxReferences = fieldSymbol.ContainingType.DeclaringSyntaxReferences;

            if (syntaxReferences.Length == 1)
            {
                return IsBackingFieldUsedInRefOrOutArgument(fieldSymbol, propertyDeclaration.Parent, context.SemanticModel, context.CancellationToken);
            }
            else
            {
                foreach (SyntaxReference syntaxReference in syntaxReferences)
                {
                    SyntaxNode declaration = syntaxReference.GetSyntax(context.CancellationToken);

                    SemanticModel semanticModel = (declaration.SyntaxTree == context.SemanticModel.SyntaxTree)
                        ? context.SemanticModel
                        : context.Compilation.GetSemanticModel(declaration.SyntaxTree);

                    if (IsBackingFieldUsedInRefOrOutArgument(fieldSymbol, declaration, semanticModel, context.CancellationToken))
                        return true;
                }

                return false;
            }
        }

        private static bool IsBackingFieldUsedInRefOrOutArgument(
            IFieldSymbol fieldSymbol,
            SyntaxNode declaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (SyntaxNode node in declaration.DescendantNodes())
            {
                if (node.IsKind(SyntaxKind.IdentifierName))
                {
                    var identifierName = (IdentifierNameSyntax)node;

                    if (string.Equals(identifierName.Identifier.ValueText, fieldSymbol.Name, StringComparison.Ordinal)
                        && fieldSymbol.Equals(semanticModel.GetSymbol(identifierName, cancellationToken)))
                    {
                        for (SyntaxNode current = node.Parent; current != null; current = current.Parent)
                        {
                            if (current.IsKind(SyntaxKind.Argument)
                                && ((ArgumentSyntax)current).RefOrOutKeyword.IsKind(SyntaxKind.RefKeyword, SyntaxKind.OutKeyword))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static bool HasStructLayoutAttributeWithExplicitKind(INamedTypeSymbol typeSymbol, Compilation compilation)
        {
            AttributeData attribute = typeSymbol.GetAttributeByMetadataName(MetadataNames.System_Runtime_InteropServices_StructLayoutAttribute, compilation);

            if (attribute != null)
            {
                ImmutableArray<TypedConstant> constructorArguments = attribute.ConstructorArguments;

                if (constructorArguments.Length == 1)
                {
                    TypedConstant typedConstant = constructorArguments[0];

                    return typedConstant.Type?.Equals(compilation.GetTypeByMetadataName(MetadataNames.System_Runtime_InteropServices_LayoutKind)) == true
                        && (((LayoutKind)typedConstant.Value) == LayoutKind.Explicit);
                }
            }

            return false;
        }

        private static void FadeOutBodyOrExpressionBody(SyntaxNodeAnalysisContext context, AccessorDeclarationSyntax accessor)
        {
            BlockSyntax body = accessor.Body;

            if (body != null)
            {
                switch (body.Statements.First())
                {
                    case ReturnStatementSyntax returnStatement:
                        {
                            context.ReportToken(DiagnosticDescriptors.UseAutoPropertyFadeOut, returnStatement.ReturnKeyword);
                            context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, returnStatement.Expression);
                            break;
                        }
                    case ExpressionStatementSyntax expressionStatement:
                        {
                            context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, expressionStatement.Expression);
                            break;
                        }
                }

                context.ReportBraces(DiagnosticDescriptors.UseAutoPropertyFadeOut, body);
            }
            else
            {
                context.ReportNode(DiagnosticDescriptors.UseAutoPropertyFadeOut, accessor.ExpressionBody);
            }
        }

        private static IFieldSymbol GetBackingFieldSymbol(
            AccessorDeclarationSyntax getter,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            NameSyntax identifier = GetIdentifierFromGetter(getter);

            if (identifier != null)
                return GetBackingFieldSymbol(identifier, semanticModel, cancellationToken);

            return null;
        }

        private static IFieldSymbol GetBackingFieldSymbol(
            ArrowExpressionClauseSyntax expressionBody,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            NameSyntax identifier = GetIdentifierFromExpression(expressionBody.Expression);

            if (identifier != null)
                return GetBackingFieldSymbol(identifier, semanticModel, cancellationToken);

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

                if (fieldSymbol.IsReadOnly
                    && !fieldSymbol.IsVolatile)
                {
                    return fieldSymbol;
                }
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

                    if (symbol?.IsPrivate() == true
                        && symbol.IsField())
                    {
                        var fieldSymbol = (IFieldSymbol)symbol;

                        if (!fieldSymbol.IsVolatile)
                        {
                            ISymbol symbol2 = semanticModel.GetSymbol(setterIdentifier, cancellationToken);

                            if (fieldSymbol.Equals(symbol2))
                                return fieldSymbol;
                        }
                    }
                }
            }

            return null;
        }

        private static SimpleNameSyntax GetIdentifierFromGetter(AccessorDeclarationSyntax getter)
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

        private static SimpleNameSyntax GetIdentifierFromSetter(AccessorDeclarationSyntax setter)
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

        private static SimpleNameSyntax GetIdentifierFromSetterExpression(ExpressionSyntax expression)
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

        public static async Task<Solution> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxToken propertyIdentifier = propertyDeclaration.Identifier.WithoutTrivia();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

            ISymbol fieldSymbol = GetFieldSymbol(propertyDeclaration, semanticModel, cancellationToken);

            var variableDeclarator = (VariableDeclaratorSyntax)await fieldSymbol.DeclaringSyntaxReferences[0].GetSyntaxAsync(cancellationToken).ConfigureAwait(false);

            var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

            var fieldDeclaration = (FieldDeclarationSyntax)variableDeclaration.Parent;

            bool isSingleDeclarator = variableDeclaration.Variables.Count == 1;

            Solution solution = document.Solution();

            foreach (DocumentReferenceInfo info in await SyntaxFinder.FindReferencesByDocumentAsync(fieldSymbol, solution, allowCandidate: false, cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                ImmutableArray<SyntaxNode> nodes = info.References;

                if (propertyDeclaration.SyntaxTree == info.SyntaxTree)
                {
                    nodes = nodes.Add(propertyDeclaration);

                    if (isSingleDeclarator)
                    {
                        nodes = nodes.Add(fieldDeclaration);
                    }
                    else
                    {
                        nodes = nodes.Add(variableDeclarator);
                    }
                }

                SyntaxNode newRoot = info.Root.ReplaceNodes(nodes, (node, _) =>
                {
                    switch (node.Kind())
                    {
                        case SyntaxKind.IdentifierName:
                            {
                                return CreateNewExpression(node, propertyIdentifier, propertySymbol)
                                    .WithTriviaFrom(node)
                                    .WithFormatterAnnotation();
                            }
                        case SyntaxKind.PropertyDeclaration:
                            {
                                return CreateAutoProperty(propertyDeclaration, variableDeclarator.Initializer);
                            }
                        case SyntaxKind.VariableDeclarator:
                        case SyntaxKind.FieldDeclaration:
                            {
                                return node.WithAdditionalAnnotations(_removeAnnotation);
                            }
                        default:
                            {
                                Debug.Fail(node.ToString());
                                return node;
                            }
                    }
                });

                SyntaxNode nodeToRemove = newRoot.GetAnnotatedNodes(_removeAnnotation).FirstOrDefault();

                if (nodeToRemove != null)
                    newRoot = newRoot.RemoveNode(nodeToRemove, SyntaxRemoveOptions.KeepUnbalancedDirectives);

                solution = solution.WithDocumentSyntaxRoot(info.Document.Id, newRoot);
            }

            return solution;
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

        public static ExpressionSyntax CreateNewExpression(SyntaxNode node, SyntaxToken identifier, IPropertySymbol propertySymbol)
        {
            if (node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression)
                && ((MemberAccessExpressionSyntax)node.Parent).Name == node)
            {
                return IdentifierName(identifier);
            }
            else if (propertySymbol.IsStatic)
            {
                return ParseName($"{propertySymbol.ContainingType.ToTypeSyntax()}.{propertySymbol.ToDisplayString(_symbolDisplayFormat)}")
                    .WithSimplifierAnnotation();
            }
            else
            {
                return IdentifierName(identifier).QualifyWithThis();
            }
        }

        public static PropertyDeclarationSyntax CreateAutoProperty(PropertyDeclarationSyntax propertyDeclaration, EqualsValueClauseSyntax initializer)
        {
            AccessorListSyntax accessorList = CreateAccessorList(propertyDeclaration);

            if (accessorList
                .DescendantTrivia()
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                accessorList = accessorList.RemoveWhitespaceOrEndOfLineTrivia();
            }

            PropertyDeclarationSyntax newProperty = propertyDeclaration
                .WithIdentifier(propertyDeclaration.Identifier.WithTrailingTrivia(Space))
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
                .WithTriviaFrom(propertyDeclaration)
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
