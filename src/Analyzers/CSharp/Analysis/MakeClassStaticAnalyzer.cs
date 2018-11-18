// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MakeClassStaticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.MakeClassStatic); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
        }

        public static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            if (classDeclaration.Modifiers.ContainsAny(
                SyntaxKind.StaticKeyword,
                SyntaxKind.AbstractKeyword,
                SyntaxKind.SealedKeyword,
                SyntaxKind.PartialKeyword))
            {
                return;
            }

            if (!classDeclaration.Members.Any())
                return;

            INamedTypeSymbol symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);

            if (symbol.BaseType?.IsObject() != true)
                return;

            if (!symbol.Interfaces.IsDefaultOrEmpty)
                return;

            ImmutableArray<ISymbol> members = symbol.GetMembers();

            if (!members.Any())
                return;

            if (!AnalyzeMembers(members))
                return;

            MakeClassStaticWalker walker = MakeClassStaticWalker.GetInstance();

            walker.CanBeMadeStatic = true;
            walker.Symbol = symbol;
            walker.SemanticModel = context.SemanticModel;
            walker.CancellationToken = context.CancellationToken;

            walker.Visit(classDeclaration);

            bool canBeMadeStatic = walker.CanBeMadeStatic;

            walker.Symbol = null;
            walker.SemanticModel = null;
            walker.CancellationToken = default;

            MakeClassStaticWalker.Free(walker);

            if (canBeMadeStatic)
                context.ReportDiagnostic(DiagnosticDescriptors.MakeClassStatic, classDeclaration.Identifier);
        }

        public static bool AnalyzeMembers(ImmutableArray<ISymbol> members)
        {
            bool areAllImplicitlyDeclared = true;

            foreach (ISymbol memberSymbol in members)
            {
                if (!memberSymbol.IsImplicitlyDeclared)
                    areAllImplicitlyDeclared = false;

                switch (memberSymbol.Kind)
                {
                    case SymbolKind.ErrorType:
                        {
                            return false;
                        }
                    case SymbolKind.NamedType:
                        {
                            var namedTypeSymbol = (INamedTypeSymbol)memberSymbol;

                            switch (namedTypeSymbol.TypeKind)
                            {
                                case TypeKind.Unknown:
                                case TypeKind.Error:
                                    {
                                        return false;
                                    }
                                case TypeKind.Class:
                                case TypeKind.Delegate:
                                case TypeKind.Enum:
                                case TypeKind.Interface:
                                case TypeKind.Struct:
                                    {
                                        if (memberSymbol.DeclaredAccessibility.ContainsProtected())
                                            return false;

                                        break;
                                    }
                                default:
                                    {
                                        Debug.Fail(namedTypeSymbol.TypeKind.ToString());
                                        break;
                                    }
                            }

                            break;
                        }
                    default:
                        {
                            Debug.Assert(memberSymbol.IsKind(SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.Property), memberSymbol.Kind.ToString());

                            if (memberSymbol.DeclaredAccessibility.ContainsProtected())
                                return false;

                            if (!memberSymbol.IsImplicitlyDeclared)
                            {
                                if (memberSymbol.IsStatic)
                                {
                                    if (memberSymbol.Kind == SymbolKind.Method
                                        && ((IMethodSymbol)memberSymbol).MethodKind.Is(MethodKind.UserDefinedOperator, MethodKind.Conversion))
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }

                            break;
                        }
                }
            }

            return !areAllImplicitlyDeclared;
        }

        private class MakeClassStaticWalker : CSharpSyntaxNodeWalker
        {
            [ThreadStatic]
            private static MakeClassStaticWalker _cachedInstance;

            public bool CanBeMadeStatic { get; set; }

            public INamedTypeSymbol Symbol { get; set; }

            public SemanticModel SemanticModel { get; set; }

            public CancellationToken CancellationToken { get; set; }

            protected override bool ShouldVisit => CanBeMadeStatic;

            protected override void VisitType(TypeSyntax node)
            {
                if (node.IsKind(SyntaxKind.IdentifierName))
                {
                    var identifierName = (IdentifierNameSyntax)node;

                    if (string.Equals(Symbol.Name, identifierName.Identifier.ValueText, StringComparison.Ordinal)
                        && SemanticModel
                            .GetSymbol(identifierName, CancellationToken)?
                            .OriginalDefinition
                            .Equals(Symbol) == true)
                    {
                        CanBeMadeStatic = false;
                    }
                }
                else
                {
                    base.VisitType(node);
                }
            }

            public static MakeClassStaticWalker GetInstance()
            {
                MakeClassStaticWalker walker = _cachedInstance;

                if (walker != null)
                {
                    _cachedInstance = null;
                    return walker;
                }

                return new MakeClassStaticWalker();
            }

            public static void Free(MakeClassStaticWalker walker)
            {
                _cachedInstance = walker;
            }
        }
    }
}
