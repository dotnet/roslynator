// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseImplicitOrExplicitObjectCreationAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UseImplicitOrExplicitObjectCreation);

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (((CSharpCompilation)startContext.Compilation).LanguageVersion >= LanguageVersion.CSharp9)
                {
                    startContext.RegisterSyntaxNodeAction(f => AnalyzeObjectCreationExpression(f), SyntaxKind.ObjectCreationExpression);
                }

                startContext.RegisterSyntaxNodeAction(f => AnalyzeImplicitObjectCreationExpression(f), SyntaxKind.ImplicitObjectCreationExpression);
            });
        }

        private static void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

            SyntaxNode parent = objectCreation.Parent;

            switch (parent.Kind())
            {
                case SyntaxKind.ThrowExpression:
                case SyntaxKind.ThrowStatement:
                    {
                        if (UseImplicitObjectCreation(context)
                            && context.SemanticModel.GetTypeSymbol(objectCreation, context.CancellationToken)?
                                .HasMetadataName(MetadataNames.System_Exception) == true)
                        {
                            ReportDiagnostic(context, objectCreation);
                        }

                        break;
                    }
                case SyntaxKind.EqualsValueClause:
                    {
                        if (!UseImplicitObjectCreation(context))
                            return;

                        parent = parent.Parent;

                        SyntaxDebug.Assert(parent.IsKind(SyntaxKind.VariableDeclarator, SyntaxKind.PropertyDeclaration), parent);

                        if (parent.IsKind(SyntaxKind.VariableDeclarator))
                        {
                            parent = parent.Parent;

                            if (parent is VariableDeclarationSyntax variableDeclaration)
                            {
                                SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement), parent.Parent);

                                if (parent.IsParentKind(SyntaxKind.FieldDeclaration))
                                {
                                    AnalyzeType(context, objectCreation, variableDeclaration.Type);
                                }
                                else if (parent.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement))
                                {
                                    if (context.UseVarInsteadOfImplicitObjectCreation() == false)
                                    {
                                        if (variableDeclaration.Type.IsVar)
                                        {
                                            ReportDiagnostic(context, objectCreation);
                                        }
                                        else
                                        {
                                            AnalyzeType(context, objectCreation, variableDeclaration.Type);
                                        }
                                    }
                                }
                            }
                        }
                        else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                        {
                            AnalyzeType(context, objectCreation, ((PropertyDeclarationSyntax)parent).Type);
                        }

                        break;
                    }
                case SyntaxKind.ArrowExpressionClause:
                    {
                        if (UseImplicitObjectCreation(context))
                        {
                            TypeSyntax type = DetermineReturnType(parent.Parent);

                            SyntaxDebug.Assert(type is not null, parent);

                            if (type is not null)
                                AnalyzeType(context, objectCreation, type);
                        }

                        break;
                    }
                case SyntaxKind.ArrayInitializerExpression:
                    {
                        SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ArrayCreationExpression, SyntaxKind.ImplicitArrayCreationExpression), parent.Parent);

                        if (UseImplicitObjectCreation(context)
                            && parent.IsParentKind(SyntaxKind.ArrayCreationExpression))
                        {
                            var arrayCreationExpression = (ArrayCreationExpressionSyntax)parent.Parent;

                            AnalyzeType(context, objectCreation, arrayCreationExpression.Type.ElementType);
                        }

                        break;
                    }
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                    {
                        if (!UseImplicitObjectCreationWhenTypeIsNotObvious(context))
                            return;

                        for (SyntaxNode node = parent.Parent; node is not null; node = node.Parent)
                        {
                            if (CSharpFacts.IsAnonymousFunctionExpression(node.Kind()))
                                return;

                            TypeSyntax type = DetermineReturnType(node);

                            if (type is not null)
                            {
                                if (parent.IsKind(SyntaxKind.YieldReturnStatement))
                                {
                                    ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(type, context.CancellationToken);

                                    if (typeSymbol?.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
                                    {
                                        var ienumerableOfT = (INamedTypeSymbol)typeSymbol;

                                        ITypeSymbol typeSymbol2 = ienumerableOfT.TypeArguments.Single();

                                        AnalyzeTypeSymbol(context, objectCreation, typeSymbol2);
                                    }
                                }
                                else
                                {
                                    AnalyzeType(context, objectCreation, type);
                                }

                                return;
                            }
                        }

                        break;
                    }
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.CoalesceAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                    {
                        if (UseImplicitObjectCreationWhenTypeIsNotObvious(context))
                        {
                            var assignment = (AssignmentExpressionSyntax)parent;
                            AnalyzeExpression(context, objectCreation, assignment.Left);
                        }

                        break;
                    }
                case SyntaxKind.CoalesceExpression:
                    {
                        if (UseImplicitObjectCreationWhenTypeIsNotObvious(context))
                        {
                            var coalesceExpression = (BinaryExpressionSyntax)parent;
                            AnalyzeExpression(context, objectCreation, coalesceExpression.Left);
                        }

                        break;
                    }
#if DEBUG
                case SyntaxKind.CollectionInitializerExpression:
                    {
                        SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression, SyntaxKind.SimpleAssignmentExpression), parent.Parent);

                        if (!UseImplicitObjectCreation(context))
                            return;

                        parent = parent.Parent;
                        if (parent.IsKind(SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression))
                        {
                            SyntaxNode parentObjectCreation = parent;

                            parent = parent.Parent;
                            if (parent.IsKind(SyntaxKind.EqualsValueClause))
                            {
                                parent = parent.Parent;
                                if (parent.IsKind(SyntaxKind.VariableDeclarator))
                                {
                                    parent = parent.Parent;
                                    if (parent is VariableDeclarationSyntax variableDeclaration
                                        && parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement))
                                    {
                                        if (parentObjectCreation is ExpressionSyntax parentObjectCreationExpression)
                                        {
                                            AnalyzeExpression(context, objectCreation, parentObjectCreationExpression, isGenericType: true);
                                        }
                                        else
                                        {
                                            AnalyzeType(context, objectCreation, variableDeclaration.Type, isGenericType: true);
                                        }
                                    }
                                }
                                else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                                {
                                    AnalyzeType(context, objectCreation, ((PropertyDeclarationSyntax)parent).Type);
                                }
                            }
                        }

                        break;
                    }
                case SyntaxKind.ComplexElementInitializerExpression:
                    {
                        break;
                    }
#endif
            }
        }

        private void AnalyzeImplicitObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var implicitObjectCreation = (ImplicitObjectCreationExpressionSyntax)context.Node;

            SyntaxNode parent = implicitObjectCreation.Parent;

            switch (parent.Kind())
            {
                case SyntaxKind.ThrowExpression:
                case SyntaxKind.ThrowStatement:
                    {
                        if (UseExplicitObjectCreation(context)
                            && context.SemanticModel.GetTypeSymbol(implicitObjectCreation, context.CancellationToken)?
                                .HasMetadataName(MetadataNames.System_Exception) == true)
                        {
                            ReportDiagnostic(context, implicitObjectCreation);
                        }

                        break;
                    }
                case SyntaxKind.EqualsValueClause:
                    {
                        parent = parent.Parent;

                        SyntaxDebug.Assert(parent.IsKind(SyntaxKind.VariableDeclarator, SyntaxKind.PropertyDeclaration), parent);

                        if (parent.IsKind(SyntaxKind.VariableDeclarator))
                        {
                            parent = parent.Parent;

                            if (parent is VariableDeclarationSyntax variableDeclaration)
                            {
                                SyntaxDebug.Assert(!variableDeclaration.Type.IsVar, variableDeclaration);

                                SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement), parent.Parent);

                                if (UseExplicitObjectCreation(context))
                                {
                                    ReportDiagnostic(context, implicitObjectCreation);
                                }
                                else if (parent.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement)
                                    && variableDeclaration.Variables.Count == 1
                                    && !variableDeclaration.Type.IsVar
                                    && context.UseVarInsteadOfImplicitObjectCreation() == true)
                                {
                                    DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseImplicitOrExplicitObjectCreation, variableDeclaration, "explicit");
                                }
                            }
                        }
                        else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                        {
                            if (UseExplicitObjectCreation(context))
                                ReportDiagnostic(context, implicitObjectCreation);
                        }

                        break;
                    }
                case SyntaxKind.ArrowExpressionClause:
                    {
                        if (UseExplicitObjectCreation(context))
                        {
                            TypeSyntax type = DetermineReturnType(parent.Parent);

                            SyntaxDebug.Assert(type is not null, parent);

                            if (type is not null)
                                ReportDiagnostic(context, implicitObjectCreation);
                        }

                        return;
                    }
                case SyntaxKind.ArrayInitializerExpression:
                    {
                        SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ArrayCreationExpression, SyntaxKind.ImplicitArrayCreationExpression), parent.Parent);

                        if (UseExplicitObjectCreation(context)
                            && parent.IsParentKind(SyntaxKind.ArrayCreationExpression))
                        {
                            var arrayCreationExpression = (ArrayCreationExpressionSyntax)parent.Parent;

                            ReportDiagnostic(context, implicitObjectCreation);
                        }

                        break;
                    }
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                    {
                        if (!UseExplicitObjectCreationWhenTypeIsNotObvious(context))
                            return;

                        for (SyntaxNode node = parent.Parent; node is not null; node = node.Parent)
                        {
                            if (CSharpFacts.IsAnonymousFunctionExpression(node.Kind()))
                                return;

                            TypeSyntax type = DetermineReturnType(node);

                            if (type is not null)
                            {
                                if (parent.IsKind(SyntaxKind.YieldReturnStatement))
                                {
                                    ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(type, context.CancellationToken);

                                    if (typeSymbol?.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
                                    {
                                        var ienumerableOfT = (INamedTypeSymbol)typeSymbol;

                                        ITypeSymbol typeSymbol2 = ienumerableOfT.TypeArguments.Single();

                                        ReportDiagnostic(context, implicitObjectCreation);
                                    }
                                }
                                else
                                {
                                    ReportDiagnostic(context, implicitObjectCreation);
                                }
                            }
                        }

                        break;
                    }
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.CoalesceAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                case SyntaxKind.CoalesceExpression:
                    {
                        if (UseExplicitObjectCreationWhenTypeIsNotObvious(context))
                            ReportDiagnostic(context, implicitObjectCreation);

                        break;
                    }
#if DEBUG
                case SyntaxKind.CollectionInitializerExpression:
                    {
                        SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression, SyntaxKind.SimpleAssignmentExpression), parent.Parent);

                        if (!UseExplicitObjectCreation(context))
                            return;

                        parent = parent.Parent;
                        if (parent.IsKind(SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression))
                        {
                            parent = parent.Parent;
                            if (parent.IsKind(SyntaxKind.EqualsValueClause))
                            {
                                parent = parent.Parent;
                                if (parent.IsKind(SyntaxKind.VariableDeclarator))
                                {
                                    parent = parent.Parent;
                                    if (parent is VariableDeclarationSyntax variableDeclaration)
                                    {
                                        SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement), parent.Parent);

                                        if (UseExplicitObjectCreation(context))
                                            ReportDiagnostic(context, implicitObjectCreation);
                                    }
                                }
                                else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                                {
                                    if (UseExplicitObjectCreation(context))
                                        ReportDiagnostic(context, implicitObjectCreation);
                                }
                            }
                        }

                        break;
                    }
                case SyntaxKind.ComplexElementInitializerExpression:
                    {
                        break;
                    }
#endif
            }
        }

        private static bool UseExplicitObjectCreation(SyntaxNodeAnalysisContext context)
        {
            return context.GetObjectCreationTypeStyle() == ObjectCreationTypeStyle.Explicit;
        }

        private static bool UseImplicitObjectCreation(SyntaxNodeAnalysisContext context)
        {
            ObjectCreationTypeStyle style = context.GetObjectCreationTypeStyle();

            return style == ObjectCreationTypeStyle.Implicit
                || style == ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious;
        }

        private static bool UseImplicitObjectCreationWhenTypeIsNotObvious(SyntaxNodeAnalysisContext context)
        {
            return context.GetObjectCreationTypeStyle() == ObjectCreationTypeStyle.Implicit;
        }

        private static bool UseExplicitObjectCreationWhenTypeIsNotObvious(SyntaxNodeAnalysisContext context)
        {
            ObjectCreationTypeStyle style = context.GetObjectCreationTypeStyle();

            return style == ObjectCreationTypeStyle.Explicit
                || style == ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious;
        }

        private static void AnalyzeType(
            SyntaxNodeAnalysisContext context,
            ObjectCreationExpressionSyntax objectCreation,
            TypeSyntax type,
            bool isGenericType = false)
        {
            if (!type.IsVar)
            {
                AnalyzeExpression(context, objectCreation, type, isGenericType: isGenericType);
            }
        }

        private static void AnalyzeExpression(
            SyntaxNodeAnalysisContext context,
            ObjectCreationExpressionSyntax objectCreation,
            ExpressionSyntax expression,
            bool isGenericType = false)
        {
            ITypeSymbol typeSymbol1 = context.SemanticModel.GetTypeSymbol(expression);

            if (isGenericType)
            {
                typeSymbol1 = (typeSymbol1 as INamedTypeSymbol)?.TypeArguments.SingleOrDefault(shouldThrow: false);
            }

            AnalyzeTypeSymbol(context, objectCreation, typeSymbol1);
        }

        private static void AnalyzeTypeSymbol(
            SyntaxNodeAnalysisContext context,
            ObjectCreationExpressionSyntax objectCreation,
            ITypeSymbol typeSymbol1)
        {
            if (typeSymbol1?.IsErrorType() == false)
            {
                ITypeSymbol typeSymbol2 = context.SemanticModel.GetTypeSymbol(objectCreation);

                if (SymbolEqualityComparer.Default.Equals(typeSymbol1, typeSymbol2))
                    ReportDiagnostic(context, objectCreation);
            }
        }

        private static TypeSyntax DetermineReturnType(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.LocalFunctionStatement:
                    return ((LocalFunctionStatementSyntax)node).ReturnType;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).ReturnType;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).ReturnType;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).Type;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).Type;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).Type;
            }

            if (node is AccessorDeclarationSyntax)
            {
                SyntaxDebug.Assert(node.IsParentKind(SyntaxKind.AccessorList), node.Parent);

                if (node.IsParentKind(SyntaxKind.AccessorList))
                    return DetermineReturnType(node.Parent.Parent);
            }

            return null;
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ObjectCreationExpressionSyntax objectCreation)
        {
            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseImplicitOrExplicitObjectCreation, objectCreation.Type, "implicit");
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ImplicitObjectCreationExpressionSyntax implicitObjectCreation)
        {
            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UseImplicitOrExplicitObjectCreation, implicitObjectCreation, "explicit");
        }
    }
}
