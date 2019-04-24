// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a type parameter constraint.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct TypeParameterConstraintInfo
    {
        private TypeParameterConstraintInfo(
            TypeParameterConstraintSyntax constraint,
            TypeParameterConstraintClauseSyntax constraintClause)
        {
            Constraint = constraint;
            ConstraintClause = constraintClause;
        }

        /// <summary>
        /// The type parameter constraint.
        /// </summary>
        public TypeParameterConstraintSyntax Constraint { get; }

        /// <summary>
        /// The constraint clause that contains this constraint in <see cref="TypeParameterConstraintClauseSyntax.Constraints"/> collection.
        /// </summary>
        public TypeParameterConstraintClauseSyntax ConstraintClause { get; }

        /// <summary>
        /// A list of constraints that contains this constraint.
        /// </summary>
        public SeparatedSyntaxList<TypeParameterConstraintSyntax> Constraints
        {
            get { return ConstraintClause?.Constraints ?? default(SeparatedSyntaxList<TypeParameterConstraintSyntax>); }
        }

        /// <summary>
        /// The identifier name of this constraint.
        /// </summary>
        public IdentifierNameSyntax Name
        {
            get { return ConstraintClause?.Name; }
        }

        /// <summary>
        /// The name of this constraint.
        /// </summary>
        public string NameText
        {
            get { return Name?.Identifier.ValueText; }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return Constraint != null; }
        }

        internal bool IsDuplicateConstraint
        {
            get
            {
                if (Constraint == null)
                    return false;

                SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints = Constraints;

                int index = constraints.IndexOf(Constraint);

                SyntaxKind kind = Constraint.Kind();

                switch (kind)
                {
                    case SyntaxKind.ClassConstraint:
                    case SyntaxKind.StructConstraint:
                        {
                            for (int i = 0; i < index; i++)
                            {
                                if (constraints[i].Kind() == kind)
                                    return true;
                            }

                            break;
                        }
                }

                return false;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, Constraint); }
        }

        internal static TypeParameterConstraintInfo Create(
            TypeParameterConstraintSyntax constraint,
            bool allowMissing = false)
        {
            if (!(constraint?.Parent is TypeParameterConstraintClauseSyntax constraintClause))
                return default;

            IdentifierNameSyntax name = constraintClause.Name;

            if (!Check(name, allowMissing))
                return default;

            SyntaxNode parent = constraintClause.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)parent;

                        TypeParameterListSyntax typeParameterList = classDeclaration.TypeParameterList;

                        if (!Check(typeParameterList, allowMissing))
                            return default;

                        return new TypeParameterConstraintInfo(constraint, constraintClause);
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        var delegateDeclaration = (DelegateDeclarationSyntax)parent;

                        TypeParameterListSyntax typeParameterList = delegateDeclaration.TypeParameterList;

                        if (!Check(typeParameterList, allowMissing))
                            return default;

                        return new TypeParameterConstraintInfo(constraint, constraintClause);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)parent;

                        TypeParameterListSyntax typeParameterList = interfaceDeclaration.TypeParameterList;

                        if (!Check(typeParameterList, allowMissing))
                            return default;

                        return new TypeParameterConstraintInfo(constraint, constraintClause);
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunctionStatement = (LocalFunctionStatementSyntax)parent;

                        TypeParameterListSyntax typeParameterList = localFunctionStatement.TypeParameterList;

                        if (!Check(typeParameterList, allowMissing))
                            return default;

                        return new TypeParameterConstraintInfo(constraint, constraintClause);
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)parent;

                        TypeParameterListSyntax typeParameterList = methodDeclaration.TypeParameterList;

                        if (!Check(typeParameterList, allowMissing))
                            return default;

                        return new TypeParameterConstraintInfo(constraint, constraintClause);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)parent;

                        TypeParameterListSyntax typeParameterList = structDeclaration.TypeParameterList;

                        if (!Check(typeParameterList, allowMissing))
                            return default;

                        return new TypeParameterConstraintInfo(constraint, constraintClause);
                    }
            }

            return default;
        }
    }
}