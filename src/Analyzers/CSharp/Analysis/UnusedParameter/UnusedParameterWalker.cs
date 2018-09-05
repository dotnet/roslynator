// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.UnusedParameter
{
    internal class UnusedParameterWalker : CSharpSyntaxNodeWalker
    {
        private static readonly StringComparer _ordinalComparer = StringComparer.Ordinal;

        private bool _isEmpty;

        public Dictionary<string, NodeSymbolInfo> Nodes { get; } = new Dictionary<string, NodeSymbolInfo>(_ordinalComparer);

        public SemanticModel SemanticModel { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public bool IsIndexer { get; set; }

        public bool IsAnyTypeParameter { get; set; }

        protected override bool ShouldVisit
        {
            get { return !_isEmpty; }
        }

        public void SetValues(SemanticModel semanticModel, CancellationToken cancellationToken, bool isIndexer = false)
        {
            _isEmpty = false;

            Nodes.Clear();
            SemanticModel = semanticModel;
            CancellationToken = cancellationToken;
            IsIndexer = isIndexer;
            IsAnyTypeParameter = false;
        }

        public void Clear()
        {
            SetValues(default(SemanticModel), default(CancellationToken));
        }

        public void AddParameter(ParameterSyntax parameter)
        {
            AddNode(parameter.Identifier.ValueText, parameter);
        }

        public void AddTypeParameter(TypeParameterSyntax typeParameter)
        {
            AddNode(typeParameter.Identifier.ValueText, typeParameter);
        }

        private void AddNode(string name, SyntaxNode node)
        {
            Nodes[name] = new NodeSymbolInfo(name, node);
        }

        private void RemoveNode(string name)
        {
            Nodes.Remove(name);

            if (Nodes.Count == 0)
                _isEmpty = true;
        }

        protected override void VisitType(TypeSyntax node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ArrayType:
                    {
                        VisitArrayType((ArrayTypeSyntax)node);
                        break;
                    }
                case SyntaxKind.AliasQualifiedName:
                case SyntaxKind.GenericName:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.NullableType:
                case SyntaxKind.OmittedTypeArgument:
                case SyntaxKind.PointerType:
                case SyntaxKind.PredefinedType:
                case SyntaxKind.QualifiedName:
                case SyntaxKind.RefType:
                case SyntaxKind.TupleType:
                    {
                        if (IsAnyTypeParameter)
                            base.VisitType(node);

                        break;
                    }
                default:
                    {
                        base.VisitType(node);
                        break;
                    }
            }
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            string name = node.Identifier.ValueText;

            if (Nodes.TryGetValue(name, out NodeSymbolInfo info))
            {
                if (info.Symbol == null)
                {
                    ISymbol declaredSymbol = SemanticModel.GetDeclaredSymbol(info.Node, CancellationToken);

                    if (declaredSymbol == null)
                    {
                        RemoveNode(name);
                        return;
                    }

                    info = new NodeSymbolInfo(info.Name, info.Node, declaredSymbol);

                    Nodes[name] = info;
                }

                ISymbol symbol = SemanticModel.GetSymbol(node, CancellationToken);

                if (IsIndexer)
                    symbol = GetIndexerParameterSymbol(node, symbol);

                if (info.Symbol.Equals(symbol))
                    RemoveNode(name);
            }
        }

        private static ISymbol GetIndexerParameterSymbol(IdentifierNameSyntax identifierName, ISymbol symbol)
        {
            if (!(symbol?.ContainingSymbol is IMethodSymbol methodSymbol))
                return null;

            if (!(methodSymbol.AssociatedSymbol is IPropertySymbol propertySymbol))
                return null;

            if (!propertySymbol.IsIndexer)
                return null;

            string name = identifierName.Identifier.ValueText;

            foreach (IParameterSymbol parameterSymbol in propertySymbol.Parameters)
            {
                if (string.Equals(name, parameterSymbol.Name, StringComparison.Ordinal))
                    return parameterSymbol;
            }

            return null;
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
        }

        public override void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
        }

        public override void VisitNameColon(NameColonSyntax node)
        {
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
        }

        public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            if (node.IsParentKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement))
                base.VisitParameterList(node);
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            if (IsAnyTypeParameter
                && node.IsParentKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement))
            {
                base.VisitTypeParameterConstraintClause(node);
            }
        }
    }
}

