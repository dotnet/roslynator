// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseGenericEventHandlerAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseGenericEventHandler); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSymbolAction(AnalyzeEvent, SymbolKind.Event);
        }

        private static void AnalyzeEvent(SymbolAnalysisContext context)
        {
            var eventSymbol = (IEventSymbol)context.Symbol;

            if (eventSymbol.IsImplicitlyDeclared)
                return;

            if (eventSymbol.IsOverride)
                return;

            if (!eventSymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty)
                return;

            var namedType = eventSymbol.Type as INamedTypeSymbol;

            if (namedType?.Arity != 0)
                return;

            if (namedType.HasMetadataName(MetadataNames.System_EventHandler))
                return;

            IMethodSymbol delegateInvokeMethod = namedType.DelegateInvokeMethod;

            if (delegateInvokeMethod == null)
                return;

            ImmutableArray<IParameterSymbol> parameters = delegateInvokeMethod.Parameters;

            if (parameters.Length != 2)
                return;

            if (!parameters[0].Type.IsObject())
                return;

            if (eventSymbol.ImplementsInterfaceMember<IEventSymbol>(allInterfaces: true))
                return;

            SyntaxNode node = eventSymbol.GetSyntax(context.CancellationToken);

            TypeSyntax type = GetTypeSyntax(node);

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.UseGenericEventHandler, type);
        }

        private static TypeSyntax GetTypeSyntax(SyntaxNode node)
        {
            switch (node)
            {
                case EventDeclarationSyntax eventDeclaration:
                    {
                        return eventDeclaration.Type;
                    }
                case VariableDeclaratorSyntax declarator:
                    {
                        if (declarator.Parent is VariableDeclarationSyntax declaration)
                            return declaration.Type;

                        break;
                    }
            }

            throw new InvalidOperationException();
        }
    }
}
