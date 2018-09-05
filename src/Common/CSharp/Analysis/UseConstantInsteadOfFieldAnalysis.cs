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

                        bool isUsedInRefOrOutArgument = walker.IsUsedInRefOrOutArgument;

                        UseConstantInsteadOfFieldWalker.Free(walker);

                        if (isUsedInRefOrOutArgument)
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

            public bool IsUsedInRefOrOutArgument { get; private set; }

            protected override bool ShouldVisit
            {
                get { return !IsUsedInRefOrOutArgument; }
            }

            public override void VisitAssignedExpression(ExpressionSyntax expression)
            {
                CancellationToken.ThrowIfCancellationRequested();

                expression = expression.WalkDownParentheses();

                if (expression.IsKind(SyntaxKind.IdentifierName))
                {
                    var identifierName = (IdentifierNameSyntax)expression;

                    if (string.Equals(identifierName.Identifier.ValueText, FieldSymbol.Name, StringComparison.Ordinal)
                        && SemanticModel.GetSymbol(identifierName, CancellationToken)?.Equals(FieldSymbol) == true)
                    {
                        IsUsedInRefOrOutArgument = true;
                    }
                }
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
                walker.IsUsedInRefOrOutArgument = false;

                _cachedInstance = walker;
            }
        }
    }
}
