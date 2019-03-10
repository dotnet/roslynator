// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.UnusedMember
{
    internal class UnusedMemberWalker : CSharpSyntaxNodeWalker
    {
        private bool _isEmpty;

        private IMethodSymbol _containingMethodSymbol;

        public Collection<NodeSymbolInfo> Nodes { get; } = new Collection<NodeSymbolInfo>();

        public SemanticModel SemanticModel { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public bool IsAnyNodeConst { get; private set; }

        public bool IsAnyNodeDelegate { get; private set; }

        protected override bool ShouldVisit
        {
            get { return !_isEmpty; }
        }

        public void Reset()
        {
            _isEmpty = false;
            _containingMethodSymbol = null;

            Nodes.Clear();
            SemanticModel = null;
            CancellationToken = default;
            IsAnyNodeConst = false;
            IsAnyNodeDelegate = false;
        }

        public void AddDelegate(string name, SyntaxNode node)
        {
            AddNode(name, node);

            IsAnyNodeDelegate = true;
        }

        public void AddNode(string name, SyntaxNode node)
        {
            Nodes.Add(new NodeSymbolInfo(name, node));
        }

        public void AddNodes(VariableDeclarationSyntax declaration, bool isConst = false)
        {
            foreach (VariableDeclaratorSyntax declarator in declaration.Variables)
                AddNode(declarator.Identifier.ValueText, declarator);

            if (isConst)
                IsAnyNodeConst = true;
        }

        private void RemoveNodeAt(int index)
        {
            Nodes.RemoveAt(index);

            if (Nodes.Count == 0)
                _isEmpty = true;
        }

        private void VisitSimpleName(SimpleNameSyntax node, string name)
        {
            for (int i = Nodes.Count - 1; i >= 0; i--)
            {
                NodeSymbolInfo info = Nodes[i];

                if (info.Name == name)
                {
                    if (info.Symbol == null)
                    {
                        ISymbol declaredSymbol = SemanticModel.GetDeclaredSymbol(info.Node, CancellationToken);

                        Debug.Assert(declaredSymbol != null, "");

                        if (declaredSymbol == null)
                        {
                            RemoveNodeAt(i);
                            continue;
                        }

                        info = new NodeSymbolInfo(info.Name, info.Node, declaredSymbol);

                        Nodes[i] = info;
                    }

                    SymbolInfo symbolInfo = SemanticModel.GetSymbolInfo(node, CancellationToken);

                    if (symbolInfo.Symbol != null)
                    {
                        ISymbol symbol = symbolInfo.Symbol;

                        if (symbol.Kind == SymbolKind.Method)
                        {
                            var methodSymbol = ((IMethodSymbol)symbol);

                            if (methodSymbol.MethodKind == MethodKind.ReducedExtension)
                                symbol = methodSymbol.ReducedFrom;
                        }

                        symbol = symbol.OriginalDefinition;

                        if (info.Symbol.Equals(symbol)
                            && _containingMethodSymbol?.Equals(symbol) != true)
                        {
                            RemoveNodeAt(i);
                        }
                    }
                    else if (symbolInfo.CandidateReason == CandidateReason.LateBound)
                    {
                        RemoveNodeAt(i);
                    }
                    else if (symbolInfo.CandidateReason == CandidateReason.MemberGroup)
                    {
                        ImmutableArray<ISymbol> candidateSymbols = symbolInfo.CandidateSymbols;

                        for (int j = 0; j < candidateSymbols.Length; j++)
                        {
                            ISymbol symbol = candidateSymbols[j].OriginalDefinition;

                            if (info.Symbol.Equals(symbol)
                                && _containingMethodSymbol?.Equals(symbol) != true)
                            {
                                RemoveNodeAt(i);
                            }
                        }
                    }
                }
            }
        }

        public override void VisitGenericName(GenericNameSyntax node)
        {
            VisitSimpleName(node, node.Identifier.ValueText);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            VisitSimpleName(node, node.Identifier.ValueText);
        }

        protected override void VisitType(TypeSyntax node)
        {
            if (IsAnyNodeDelegate)
                base.VisitType(node);
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

        public override void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
            Debug.Fail($"{nameof(UnusedMemberWalker)}.{nameof(VisitExplicitInterfaceSpecifier)}");
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
            Debug.Fail($"{nameof(UnusedMemberWalker)}.{nameof(VisitTypeParameterList)}");
        }

        public override void VisitBaseList(BaseListSyntax node)
        {
            Debug.Fail($"{nameof(UnusedMemberWalker)}.{nameof(VisitBaseList)}");
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            Debug.Fail($"{nameof(UnusedMemberWalker)}.{nameof(VisitTypeParameterConstraintClause)}");
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            if (node != null)
                base.VisitParameterList(node);
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            VisitMembers(node.Members);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            VisitMembers(node.Members);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            VisitMembers(node.Members);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            VisitMembers(node.Members);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
            VisitMembers(node.Members);
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);

            if (!ShouldVisit)
                return;

            TypeSyntax returnType = node.ReturnType;

            if (returnType != null)
                VisitType(returnType);

            if (!ShouldVisit)
                return;

            VisitParameterList(node.ParameterList);
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);

            if (IsAnyNodeConst)
            {
                foreach (EnumMemberDeclarationSyntax member in node.Members)
                {
                    if (!ShouldVisit)
                        return;

                    VisitEnumMemberDeclaration(member);
                }
            }
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);

            if (!ShouldVisit)
                return;

            TypeSyntax type = node.Type;

            if (type != null)
                VisitType(type);

            if (!ShouldVisit)
                return;

            EqualsValueClauseSyntax initializer = node.Initializer;

            if (initializer != null)
                VisitEqualsValueClause(initializer);

            if (!ShouldVisit)
                return;

            AccessorListSyntax accessorList = node.AccessorList;

            if (accessorList != null)
            {
                VisitAccessorList(accessorList);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = node.ExpressionBody;

                if (expressionBody != null)
                    VisitArrowExpressionClause(expressionBody);
            }
        }

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            VisitAttributeLists(node.AttributeLists);

            if (!ShouldVisit)
                return;

            TypeSyntax type = node.Type;

            if (type != null)
                VisitType(type);

            if (!ShouldVisit)
                return;

            BracketedParameterListSyntax parameterList = node.ParameterList;

            if (node != null)
                VisitBracketedParameterList(parameterList);

            if (!ShouldVisit)
                return;

            AccessorListSyntax accessorList = node.AccessorList;

            if (accessorList != null)
            {
                VisitAccessorList(accessorList);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = node.ExpressionBody;

                if (expressionBody != null)
                    VisitArrowExpressionClause(expressionBody);
            }
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            TypeSyntax returnType = node.ReturnType;

            if (returnType != null)
                VisitType(returnType);

            if (!ShouldVisit)
                return;

            VisitParameterList(node.ParameterList);

            if (!ShouldVisit)
                return;

            BlockSyntax body = node.Body;

            if (body != null)
            {
                VisitBlock(body);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = node.ExpressionBody;

                if (expressionBody != null)
                {
                    VisitArrowExpressionClause(expressionBody);
                }
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            Debug.Assert(_containingMethodSymbol == null);

            _containingMethodSymbol = SemanticModel.GetDeclaredSymbol(node, CancellationToken);

            VisitAttributeLists(node.AttributeLists);

            if (!ShouldVisit)
                return;

            TypeSyntax returnType = node.ReturnType;

            if (returnType != null)
                VisitType(returnType);

            if (!ShouldVisit)
                return;

            VisitParameterList(node.ParameterList);

            if (!ShouldVisit)
                return;

            BlockSyntax body = node.Body;

            if (body != null)
            {
                VisitBlock(body);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = node.ExpressionBody;

                if (expressionBody != null)
                {
                    VisitArrowExpressionClause(expressionBody);
                }
            }

            _containingMethodSymbol = null;
        }

        private void VisitMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            foreach (MemberDeclarationSyntax memberDeclaration in members)
            {
                if (!ShouldVisit)
                    return;

                VisitMemberDeclaration(memberDeclaration);
            }
        }

        private void VisitAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            foreach (AttributeListSyntax attributeList in attributeLists)
            {
                if (!ShouldVisit)
                    return;

                VisitAttributeList(attributeList);
            }
        }
    }
}
