// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis
{
    internal static class UseConstantInsteadOfFieldAnalysis
    {
        public static bool IsFixable(
            FieldDeclarationSyntax fieldDeclaration,
            SemanticModel semanticModel,
            bool onlyPrivate = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            bool isStatic = false;
            bool isReadOnly = false;

            foreach (SyntaxToken modifier in fieldDeclaration.Modifiers)
            {
                switch (modifier.Kind())
                {
                    case SyntaxKind.ConstKeyword:
                    case SyntaxKind.NewKeyword:
                        {
                            return false;
                        }
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.ProtectedKeyword:
                        {
                            if (onlyPrivate)
                                return false;

                            break;
                        }
                    case SyntaxKind.StaticKeyword:
                        {
                            isStatic = true;
                            break;
                        }
                    case SyntaxKind.ReadOnlyKeyword:
                        {
                            isReadOnly = true;
                            break;
                        }
                }
            }

            if (!isStatic)
                return false;

            if (!isReadOnly)
                return false;

            SeparatedSyntaxList<VariableDeclaratorSyntax> declarators = fieldDeclaration.Declaration.Variables;

            VariableDeclaratorSyntax firstDeclarator = declarators.First();

            var fieldSymbol = (IFieldSymbol)semanticModel.GetDeclaredSymbol(firstDeclarator, cancellationToken);

            if (fieldSymbol == null)
                return false;

            if (!fieldSymbol.Type.SupportsConstantValue())
                return false;

            foreach (VariableDeclaratorSyntax declarator in declarators)
            {
                ExpressionSyntax value = declarator.Initializer?.Value;

                if (value == null)
                    return false;

                if (!semanticModel.HasConstantValue(value, cancellationToken))
                    return false;
            }

            foreach (IMethodSymbol constructorSymbol in fieldSymbol.ContainingType.StaticConstructors)
            {
                foreach (SyntaxReference syntaxReference in constructorSymbol.DeclaringSyntaxReferences)
                {
                    if (syntaxReference.SyntaxTree != fieldDeclaration.SyntaxTree)
                        return false;

                    var constructorDeclaration = (ConstructorDeclarationSyntax)syntaxReference.GetSyntax(cancellationToken);

                    BlockSyntax body = constructorDeclaration.Body;

                    if (body != null)
                    {
                        UseConstantInsteadOfFieldWalker walker = UseConstantInsteadOfFieldWalker.GetInstance();

                        walker.FieldSymbol = fieldSymbol;
                        walker.SemanticModel = semanticModel;
                        walker.CancellationToken = cancellationToken;

                        walker.VisitBlock(body);

                        bool canBeConvertedToConstant = walker.CanBeConvertedToConstant;

                        UseConstantInsteadOfFieldWalker.Free(walker);

                        if (!canBeConvertedToConstant)
                            return false;
                    }
                }
            }

            return true;
        }

        private class UseConstantInsteadOfFieldWalker : AssignedExpressionWalker
        {
            public IFieldSymbol FieldSymbol { get; set; }

            public SemanticModel SemanticModel { get; set; }

            public CancellationToken CancellationToken { get; set; }

            public bool CanBeConvertedToConstant { get; private set; }

            protected override bool ShouldVisit
            {
                get { return CanBeConvertedToConstant; }
            }

            public override void VisitAssignedExpression(ExpressionSyntax expression)
            {
                if (IsFieldIdentifier(expression))
                    CanBeConvertedToConstant = false;
            }

            private bool IsFieldIdentifier(ExpressionSyntax expression)
            {
                CancellationToken.ThrowIfCancellationRequested();

                expression = expression?.WalkDownParentheses();

                if (expression.IsKind(SyntaxKind.IdentifierName))
                {
                    var identifierName = (IdentifierNameSyntax)expression;

                    if (string.Equals(identifierName.Identifier.ValueText, FieldSymbol.Name, StringComparison.Ordinal)
                        && SemanticModel.GetSymbol(identifierName, CancellationToken)?.Equals(FieldSymbol) == true)
                    {
                        return true;
                    }
                }

                return false;
            }

            public override void VisitArgument(ArgumentSyntax node)
            {
                switch (node.RefOrOutKeyword.Kind())
                {
                    case SyntaxKind.RefKeyword:
                        {
                            VisitAssignedExpression(node.Expression);
                            break;
                        }
                    case SyntaxKind.OutKeyword:
                        {
                            ExpressionSyntax expression = node.Expression;

                            if (expression?.IsKind(SyntaxKind.DeclarationExpression) == false)
                                VisitAssignedExpression(expression);

                            break;
                        }
                    case SyntaxKind.InKeyword:
                        {
                            if (IsFieldIdentifier(node.Expression))
                                CanBeConvertedToConstant = false;

                            break;
                        }
                }

                base.VisitArgument(node);
            }

            [ThreadStatic]
            private static UseConstantInsteadOfFieldWalker _cachedInstance;

            public static UseConstantInsteadOfFieldWalker GetInstance()
            {
                UseConstantInsteadOfFieldWalker walker = _cachedInstance;

                if (walker != null)
                {
                    _cachedInstance = null;

                    return walker;
                }
                else
                {
                    return new UseConstantInsteadOfFieldWalker();
                }
            }

            public static void Free(UseConstantInsteadOfFieldWalker walker)
            {
                walker.FieldSymbol = null;
                walker.SemanticModel = null;
                walker.CancellationToken = default;
                walker.CanBeConvertedToConstant = true;

                _cachedInstance = walker;
            }
        }
    }
}
