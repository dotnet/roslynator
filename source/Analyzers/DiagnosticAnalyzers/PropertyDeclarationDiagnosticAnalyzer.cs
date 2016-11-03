// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyDeclarationDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.ReplacePropertyWithAutoImplementedProperty,
                    DiagnosticDescriptors.ReplacePropertyWithAutoImplementedPropertyFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzePropertyDeclaration(f), SyntaxKind.PropertyDeclaration);
        }

        private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var property = (PropertyDeclarationSyntax)context.Node;

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
                            DiagnosticDescriptors.ReplacePropertyWithAutoImplementedProperty,
                            property.GetLocation());

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
            ISymbol symbol = context
                .SemanticModel
                .GetSymbolInfo(identifier, context.CancellationToken)
                .Symbol;

            if (symbol?.IsField() == true
                && symbol.IsPrivate())
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
                    ISymbol symbol = context
                    .SemanticModel
                    .GetSymbolInfo(getterIdentifier, context.CancellationToken)
                    .Symbol;

                    if (symbol?.IsField() == true
                        && symbol.IsPrivate())
                    {
                        ISymbol symbol2 = context
                        .SemanticModel
                        .GetSymbolInfo(setterIdentifier, context.CancellationToken)
                        .Symbol;

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
            DiagnosticDescriptor descriptor = DiagnosticDescriptors.ReplacePropertyWithAutoImplementedPropertyFadeOut;

            if (property.ExpressionBody != null)
            {
                context.FadeOutNode(descriptor, property.ExpressionBody);
            }
            else
            {
                AccessorDeclarationSyntax getter = property.Getter();

                if (getter != null)
                    context.FadeOutNode(descriptor, getter.Body);

                AccessorDeclarationSyntax setter = property.Setter();

                if (setter != null)
                    context.FadeOutNode(descriptor, setter.Body);
            }
        }
    }
}
