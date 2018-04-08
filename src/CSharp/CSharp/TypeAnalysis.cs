// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    [DebuggerDisplay("{Flags}")]
    internal readonly struct TypeAnalysis : IEquatable<TypeAnalysis>
    {
        internal TypeAnalysis(TypeAnalysisFlags flags)
        {
            Flags = flags;
        }

        public bool IsImplicit => Any(TypeAnalysisFlags.Implicit);

        public bool IsExplicit => Any(TypeAnalysisFlags.Explicit);

        public bool SupportsImplicit => Any(TypeAnalysisFlags.SupportsImplicit);

        public bool SupportsExplicit => Any(TypeAnalysisFlags.SupportsExplicit);

        public bool IsTypeObvious => Any(TypeAnalysisFlags.TypeObvious);

        internal TypeAnalysisFlags Flags { get; }

        public static TypeAnalysis AnalyzeType(
            VariableDeclarationSyntax variableDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TypeSyntax type = variableDeclaration.Type;

            Debug.Assert(type != null);

            if (type == null)
                return TypeAnalysisFlags.None;

            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;

            Debug.Assert(variables.Any());

            if (!variables.Any())
                return TypeAnalysisFlags.None;

            if (variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.EventFieldDeclaration))
                return TypeAnalysisFlags.None;

            ExpressionSyntax expression = variables[0].Initializer?.Value?.WalkDownParentheses();

            if (expression == null)
                return TypeAnalysisFlags.None;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

            if (typeSymbol == null)
                return TypeAnalysisFlags.None;

            SymbolKind kind = typeSymbol.Kind;

            if (kind == SymbolKind.ErrorType)
                return TypeAnalysisFlags.None;

            if (kind == SymbolKind.DynamicType)
                return TypeAnalysisFlags.Dynamic;

            var flags = TypeAnalysisFlags.None;

            if (type.IsVar)
            {
                flags |= TypeAnalysisFlags.Implicit;

                if (typeSymbol.SupportsExplicitDeclaration())
                    flags |= TypeAnalysisFlags.SupportsExplicit;
            }
            else
            {
                flags |= TypeAnalysisFlags.Explicit;

                if (variables.Count == 1
                    && (variableDeclaration.Parent as LocalDeclarationStatementSyntax)?.IsConst != true
                    && !expression.IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultLiteralExpression))
                {
                    flags |= TypeAnalysisFlags.SupportsImplicit;
                }
            }

            switch (expression.Kind())
            {
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.ArrayCreationExpression:
                case SyntaxKind.CastExpression:
                case SyntaxKind.AsExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.DefaultExpression:
                    {
                        flags |= TypeAnalysisFlags.TypeObvious;
                        break;
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

                        if (symbol?.Kind == SymbolKind.Field
                            && symbol.ContainingType?.TypeKind == TypeKind.Enum)
                        {
                            flags |= TypeAnalysisFlags.TypeObvious;
                        }

                        break;
                    }
            }

            return flags;
        }

        public static bool IsImplicitThatCanBeExplicit(
            VariableDeclarationSyntax variableDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return IsImplicitThatCanBeExplicit(variableDeclaration, semanticModel, TypeAppearance.None, cancellationToken);
        }

        public static bool IsImplicitThatCanBeExplicit(
            VariableDeclarationSyntax variableDeclaration,
            SemanticModel semanticModel,
            TypeAppearance typeAppearance,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TypeSyntax type = variableDeclaration.Type;

            Debug.Assert(type != null);

            if (type == null)
                return false;

            if (!type.IsVar)
                return false;

            if (variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.EventFieldDeclaration))
                return false;

            Debug.Assert(variableDeclaration.Variables.Any());

            ExpressionSyntax expression = variableDeclaration
                .Variables
                .FirstOrDefault()?
                .Initializer?
                .Value?
                .WalkDownParentheses();

            if (expression == null)
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

            if (typeSymbol == null)
                return false;

            if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
                return false;

            if (!typeSymbol.SupportsExplicitDeclaration())
                return false;

            switch (typeAppearance)
            {
                case TypeAppearance.Obvious:
                    return IsObvious(expression, semanticModel, cancellationToken);
                case TypeAppearance.NotObvious:
                    return !IsObvious(expression, semanticModel, cancellationToken);
            }

            Debug.Assert(typeAppearance == TypeAppearance.None, typeAppearance.ToString());

            return true;
        }

        public static bool IsExplicitThatCanBeImplicit(
            VariableDeclarationSyntax variableDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return IsExplicitThatCanBeImplicit(variableDeclaration, semanticModel, TypeAppearance.None, cancellationToken);
        }

        public static bool IsExplicitThatCanBeImplicit(
            VariableDeclarationSyntax variableDeclaration,
            SemanticModel semanticModel,
            TypeAppearance typeAppearance,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TypeSyntax type = variableDeclaration.Type;

            Debug.Assert(type != null);

            if (type == null)
                return false;

            if (type.IsVar)
                return false;

            switch (variableDeclaration.Parent.Kind())
            {
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    {
                        return false;
                    }
                case SyntaxKind.LocalDeclarationStatement:
                    {
                        if (((LocalDeclarationStatementSyntax)variableDeclaration.Parent).IsConst)
                            return false;

                        break;
                    }
            }

            Debug.Assert(variableDeclaration.Variables.Any());

            ExpressionSyntax expression = variableDeclaration
                .Variables
                .SingleOrDefault(shouldThrow: false)?
                .Initializer?
                .Value?
                .WalkDownParentheses();

            if (expression == null)
                return false;

            if (expression.IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultLiteralExpression))
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

            if (typeSymbol == null)
                return false;

            if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
                return false;

            if (!typeSymbol.Equals(semanticModel.GetTypeSymbol(expression, cancellationToken)))
                return false;

            switch (typeAppearance)
            {
                case TypeAppearance.Obvious:
                    return IsObvious(expression, semanticModel, cancellationToken);
                case TypeAppearance.NotObvious:
                    return !IsObvious(expression, semanticModel, cancellationToken);
            }

            Debug.Assert(typeAppearance == TypeAppearance.None, typeAppearance.ToString());

            return true;
        }

        private static bool IsObvious(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.ArrayCreationExpression:
                case SyntaxKind.CastExpression:
                case SyntaxKind.AsExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.DefaultExpression:
                    {
                        return true;
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

                        return symbol?.Kind == SymbolKind.Field
                            && symbol.ContainingType?.TypeKind == TypeKind.Enum;
                    }
            }

            return false;
        }

        public static TypeAnalysis AnalyzeType(
            DeclarationExpressionSyntax declarationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TypeSyntax type = declarationExpression.Type;

            if (type == null)
                return TypeAnalysisFlags.None;

            if (!(declarationExpression.Designation is SingleVariableDesignationSyntax singleVariableDesignation))
                return TypeAnalysisFlags.None;

            if (!(semanticModel.GetDeclaredSymbol(singleVariableDesignation, cancellationToken) is ILocalSymbol localSymbol))
                return TypeAnalysisFlags.None;

            ITypeSymbol typeSymbol = localSymbol.Type;

            Debug.Assert(typeSymbol != null);

            if (typeSymbol == null)
                return TypeAnalysisFlags.None;

            SymbolKind kind = typeSymbol.Kind;

            if (kind == SymbolKind.ErrorType)
                return TypeAnalysisFlags.None;

            if (kind == SymbolKind.DynamicType)
                return TypeAnalysisFlags.Dynamic;

            var flags = TypeAnalysisFlags.None;

            if (type.IsVar)
            {
                flags |= TypeAnalysisFlags.Implicit;

                if (typeSymbol.SupportsExplicitDeclaration())
                    flags |= TypeAnalysisFlags.SupportsExplicit;
            }
            else
            {
                flags |= TypeAnalysisFlags.Explicit;
                flags |= TypeAnalysisFlags.SupportsImplicit;
            }

            return flags;
        }

        public static bool IsImplicitThatCanBeExplicit(
            DeclarationExpressionSyntax declarationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TypeSyntax type = declarationExpression.Type;

            Debug.Assert(type != null);

            if (type == null)
                return false;

            if (!type.IsVar)
                return false;

            if (!(declarationExpression.Designation is SingleVariableDesignationSyntax singleVariableDesignation))
                return false;

            if (!(semanticModel.GetDeclaredSymbol(singleVariableDesignation, cancellationToken) is ILocalSymbol localSymbol))
                return false;

            ITypeSymbol typeSymbol = localSymbol.Type;

            Debug.Assert(typeSymbol != null);

            if (typeSymbol == null)
                return false;

            if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
                return false;

            return typeSymbol.SupportsExplicitDeclaration();
        }

        public static bool IsExplicitThatCanBeImplicit(
            DeclarationExpressionSyntax declarationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            TypeSyntax type = declarationExpression.Type;

            if (type == null)
                return false;

            if (type.IsVar)
                return false;

            if (!(declarationExpression.Designation is SingleVariableDesignationSyntax singleVariableDesignation))
                return false;

            if (!(semanticModel.GetDeclaredSymbol(singleVariableDesignation, cancellationToken) is ILocalSymbol localSymbol))
                return false;

            ITypeSymbol typeSymbol = localSymbol.Type;

            Debug.Assert(typeSymbol != null);

            if (typeSymbol == null)
                return false;

            if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
                return false;

            return true;
        }

        public static TypeAnalysis AnalyzeType(ForEachStatementSyntax forEachStatement, SemanticModel semanticModel)
        {
            TypeSyntax type = forEachStatement.Type;

            Debug.Assert(type != null);

            if (type == null)
                return TypeAnalysisFlags.None;

            ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

            ITypeSymbol typeSymbol = info.ElementType;

            if (typeSymbol == null)
                return TypeAnalysisFlags.None;

            SymbolKind kind = typeSymbol.Kind;

            if (kind == SymbolKind.ErrorType)
                return TypeAnalysisFlags.None;

            if (kind == SymbolKind.DynamicType)
                return TypeAnalysisFlags.Dynamic;

            var flags = TypeAnalysisFlags.None;

            if (type.IsVar)
            {
                flags |= TypeAnalysisFlags.Implicit;

                if (typeSymbol.SupportsExplicitDeclaration())
                    flags |= TypeAnalysisFlags.SupportsExplicit;
            }
            else
            {
                flags |= TypeAnalysisFlags.Explicit;

                if (info.ElementConversion.IsIdentity)
                    flags |= TypeAnalysisFlags.SupportsImplicit;
            }

            return flags;
        }

        public static bool IsImplicitThatCanBeExplicit(ForEachStatementSyntax forEachStatement, SemanticModel semanticModel)
        {
            TypeSyntax type = forEachStatement.Type;

            Debug.Assert(type != null);

            if (type == null)
                return false;

            if (!type.IsVar)
                return false;

            ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

            ITypeSymbol typeSymbol = info.ElementType;

            if (typeSymbol == null)
                return false;

            if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
                return false;

            return typeSymbol.SupportsExplicitDeclaration();
        }

        public static bool IsExplicitThatCanBeImplicit(ForEachStatementSyntax forEachStatement, SemanticModel semanticModel)
        {
            TypeSyntax type = forEachStatement.Type;

            Debug.Assert(type != null);

            if (type == null)
                return false;

            if (type.IsVar)
                return false;

            ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

            ITypeSymbol typeSymbol = info.ElementType;

            if (typeSymbol == null)
                return false;

            if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
                return false;

            return info.ElementConversion.IsIdentity;
        }

        public bool Any(TypeAnalysisFlags flags)
        {
            return (Flags & flags) != 0;
        }

        public bool All(TypeAnalysisFlags flags)
        {
            return (Flags & flags) != flags;
        }

        public override bool Equals(object obj)
        {
            return obj is TypeAnalysis other && Equals(other);
        }

        public bool Equals(TypeAnalysis other)
        {
            return Flags == other.Flags;
        }

        public override int GetHashCode()
        {
            return Flags.GetHashCode();
        }

        public static implicit operator TypeAnalysis(TypeAnalysisFlags value)
        {
            return new TypeAnalysis(value);
        }

        public static implicit operator TypeAnalysisFlags(TypeAnalysis value)
        {
            return value.Flags;
        }

        public static bool operator ==(TypeAnalysis analysis1, TypeAnalysis analysis2)
        {
            return analysis1.Equals(analysis2);
        }

        public static bool operator !=(TypeAnalysis analysis1, TypeAnalysis analysis2)
        {
            return !(analysis1 == analysis2);
        }
    }
}
