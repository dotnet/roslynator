// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

            return true;
        }
    }
}
