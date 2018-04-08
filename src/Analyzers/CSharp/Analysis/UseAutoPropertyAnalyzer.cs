// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseAutoPropertyAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UseAutoProperty,
                    DiagnosticDescriptors.UseAutoPropertyFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
        }

        //XPERF:
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

            if (BackingFieldHasNonSerializedAttribute(fieldSymbol, context.Compilation))
                return;

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

        private static bool BackingFieldHasNonSerializedAttribute(IFieldSymbol fieldSymbol, Compilation compilation)
        {
            ImmutableArray<AttributeData> attributes = fieldSymbol.GetAttributes();

            for (int i = 0; i < attributes.Length; i++)
            {
                INamedTypeSymbol attributeSymbol = attributes[i].AttributeClass;

                if (attributeSymbol.MetadataName == "NonSerializedAttribute"
                    && attributeSymbol.Equals(compilation.GetTypeByMetadataName(MetadataNames.System_NonSerializedAttribute)))
                {
                    return true;
                }
            }

            return false;
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

            if (symbol?.DeclaredAccessibility == Accessibility.Private
                && symbol.Kind == SymbolKind.Field)
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

                    if (symbol?.DeclaredAccessibility == Accessibility.Private
                        && symbol.Kind == SymbolKind.Field)
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
            if (expression?.Kind() == SyntaxKind.SimpleAssignmentExpression)
            {
                var assignment = (AssignmentExpressionSyntax)expression;
                ExpressionSyntax right = assignment.Right;

                if (right?.Kind() == SyntaxKind.IdentifierName
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

                        if (memberAccess.Expression?.Kind() == SyntaxKind.ThisExpression)
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
    }
}
