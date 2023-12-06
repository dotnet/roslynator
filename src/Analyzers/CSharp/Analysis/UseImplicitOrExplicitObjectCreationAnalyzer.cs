// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseImplicitOrExplicitObjectCreationAnalyzer : BaseDiagnosticAnalyzer
{
    private static readonly ImmutableDictionary<string, string> _implicitToCollectionExpression = ImmutableDictionary.CreateRange(new[]
        {
            new KeyValuePair<string, string>(DiagnosticPropertyKeys.ImplicitToCollectionExpression, null)
        });

    private static readonly ImmutableDictionary<string, string> _collectionExpressionToImplicit = ImmutableDictionary.CreateRange(new[]
        {
            new KeyValuePair<string, string>(DiagnosticPropertyKeys.CollectionExpressionToImplicit, null)
        });

    private static readonly ImmutableDictionary<string, string> _explicitToCollectionExpression = ImmutableDictionary.CreateRange(new[]
        {
            new KeyValuePair<string, string>(DiagnosticPropertyKeys.ExplicitToCollectionExpression, null)
        });

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
                startContext.RegisterSyntaxNodeAction(f => AnalyzeExplicit(f), SyntaxKind.ObjectCreationExpression);
            }

            startContext.RegisterSyntaxNodeAction(c => AnalyzeImplicit(c), SyntaxKind.ImplicitObjectCreationExpression);
            startContext.RegisterSyntaxNodeAction(c => AnalyzeImplicit(c), SyntaxKind.CollectionExpression);
        });
    }

    private static void AnalyzeExplicit(SyntaxNodeAnalysisContext context)
    {
        var objectCreation = (ObjectCreationExpressionSyntax)context.Node;

        SyntaxNode parent = objectCreation.Parent;

        switch (parent.Kind())
        {
            case SyntaxKind.ThrowExpression:
            case SyntaxKind.ThrowStatement:
                {
                    if (UseImplicitOrImplicitWhenObvious(context)
                        && context.SemanticModel.GetTypeSymbol(objectCreation, context.CancellationToken)?
                            .HasMetadataName(MetadataNames.System_Exception) == true)
                    {
                        ReportExplicitToImplicit(context, objectCreation);
                    }

                    break;
                }
            case SyntaxKind.EqualsValueClause:
                {
                    if (!UseImplicitOrImplicitWhenObvious(context))
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
                                        ReportExplicitToImplicit(context, objectCreation, canUseCollectionExpression: false);
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
                    if (UseImplicitOrImplicitWhenObvious(context))
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
                    SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ArrayCreationExpression, SyntaxKind.ImplicitArrayCreationExpression, SyntaxKind.EqualsValueClause, SyntaxKind.ImplicitStackAllocArrayCreationExpression), parent.Parent);

                    if (UseImplicitOrImplicitWhenObvious(context))
                    {
                        if (parent.IsParentKind(SyntaxKind.ArrayCreationExpression))
                        {
                            var arrayCreationExpression = (ArrayCreationExpressionSyntax)parent.Parent;

                            AnalyzeType(context, objectCreation, arrayCreationExpression.Type.ElementType);
                        }
                        else if (parent.IsParentKind(SyntaxKind.EqualsValueClause))
                        {
                            parent = parent.Parent.Parent;

                            if (parent.IsKind(SyntaxKind.VariableDeclarator))
                            {
                                parent = parent.Parent;

                                if (parent is VariableDeclarationSyntax variableDeclaration)
                                {
                                    if (parent.IsParentKind(SyntaxKind.FieldDeclaration))
                                    {
                                        AnalyzeArrayType(context, objectCreation, variableDeclaration.Type);
                                    }
                                    else if (parent.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement))
                                    {
                                        if (!variableDeclaration.Type.IsVar)
                                            AnalyzeArrayType(context, objectCreation, variableDeclaration.Type);
                                    }
                                }
                            }
                            else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                            {
                                AnalyzeArrayType(context, objectCreation, ((PropertyDeclarationSyntax)parent).Type);
                            }
                        }
                    }

                    break;
                }
            case SyntaxKind.ReturnStatement:
            case SyntaxKind.YieldReturnStatement:
                {
                    bool isAnalyzable = context.GetObjectCreationTypeStyle() switch
                    {
                        ObjectCreationTypeStyle.Explicit => false,
                        ObjectCreationTypeStyle.Implicit => true,
                        ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious => IsSingleReturnStatement(parent),
                        _ => false,
                    };

                    if (!isAnalyzable)
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
                    if (UseImplicit(context))
                    {
                        var assignment = (AssignmentExpressionSyntax)parent;
                        AnalyzeExpression(context, objectCreation, assignment.Left);
                    }

                    break;
                }
            case SyntaxKind.CoalesceExpression:
                {
                    ObjectCreationTypeStyle style = context.GetObjectCreationTypeStyle();

                    if (style == ObjectCreationTypeStyle.Implicit)
                    {
                        var coalesceExpression = (BinaryExpressionSyntax)parent;
                        AnalyzeExpression(context, objectCreation, coalesceExpression.Left);
                    }
                    else if (style == ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious
                        && parent.IsParentKind(SyntaxKind.EqualsValueClause))
                    {
                        if (parent.Parent.Parent is VariableDeclaratorSyntax variableDeclarator)
                        {
                            if (variableDeclarator.Parent is VariableDeclarationSyntax variableDeclaration
                                && variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration))
                            {
                                AnalyzeType(context, objectCreation, variableDeclaration.Type);
                            }
                        }
                        else if (parent.Parent.Parent is PropertyDeclarationSyntax propertyDeclaration)
                        {
                            AnalyzeType(context, objectCreation, propertyDeclaration.Type);
                        }
                    }

                    break;
                }
            case SyntaxKind.CollectionInitializerExpression:
                {
                    SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression, SyntaxKind.SimpleAssignmentExpression), parent.Parent);

                    if (!UseImplicitOrImplicitWhenObvious(context))
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
        }
    }

    private static void AnalyzeImplicit(SyntaxNodeAnalysisContext context)
    {
        SyntaxNode node = context.Node;
        SyntaxNode parent = node.Parent;

        switch (parent.Kind())
        {
            case SyntaxKind.ThrowExpression:
            case SyntaxKind.ThrowStatement:
                {
                    if (UseExplicit(context)
                        && context.SemanticModel.GetTypeSymbol(node, context.CancellationToken)?
                            .HasMetadataName(MetadataNames.System_Exception) == true)
                    {
                        ReportImplicitToExplicit(context);
                    }

                    break;
                }
            case SyntaxKind.EqualsValueClause:
                {
                    parent = parent.Parent;

                    SyntaxDebug.Assert(parent.IsKind(SyntaxKind.VariableDeclarator, SyntaxKind.PropertyDeclaration, SyntaxKind.Parameter), parent);

                    if (parent.IsKind(SyntaxKind.VariableDeclarator))
                    {
                        parent = parent.Parent;

                        if (parent is VariableDeclarationSyntax variableDeclaration)
                        {
                            SyntaxDebug.Assert(!variableDeclaration.Type.IsVar, variableDeclaration);
                            SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement), parent.Parent);

                            if (!ReportImplicitObvious(context)
                                && parent.IsParentKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement)
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
                        ReportImplicitObvious(context);
                    }

                    break;
                }
            case SyntaxKind.ArrowExpressionClause:
                {
                    if (UseExplicit(context))
                    {
                        TypeSyntax type = DetermineReturnType(parent.Parent);

                        SyntaxDebug.Assert(type is not null, parent);

                        if (type is not null)
                            ReportImplicitObvious(context);
                    }

                    break;
                }
            case SyntaxKind.ArrayInitializerExpression:
                {
                    SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ArrayCreationExpression, SyntaxKind.ImplicitArrayCreationExpression, SyntaxKind.EqualsValueClause), parent.Parent);

                    if (parent.IsParentKind(SyntaxKind.ArrayCreationExpression))
                        ReportImplicitObvious(context);

                    break;
                }
            case SyntaxKind.ReturnStatement:
            case SyntaxKind.YieldReturnStatement:
                {
                    bool isAnalyzable = context.GetObjectCreationTypeStyle() switch
                    {
                        ObjectCreationTypeStyle.Explicit => true,
                        ObjectCreationTypeStyle.Implicit => false,
                        ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious => !IsSingleReturnStatement(parent),
                        _ => false,
                    };

                    if (!isAnalyzable)
                        return;

                    for (SyntaxNode node2 = parent.Parent; node2 is not null; node2 = node2.Parent)
                    {
                        if (CSharpFacts.IsAnonymousFunctionExpression(node2.Kind()))
                            return;

                        TypeSyntax type = DetermineReturnType(node2);

                        if (type is not null)
                        {
                            if (parent.IsKind(SyntaxKind.YieldReturnStatement))
                            {
                                ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(type, context.CancellationToken);

                                if (typeSymbol?.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T)
                                    ReportImplicitNotObvious(context);
                            }
                            else
                            {
                                ReportImplicit(context, isObvious: IsSingleReturnStatement(parent));
                            }
                        }
                    }

                    break;
                }
            case SyntaxKind.SimpleAssignmentExpression:
            case SyntaxKind.CoalesceAssignmentExpression:
            case SyntaxKind.AddAssignmentExpression:
            case SyntaxKind.SubtractAssignmentExpression:
                {
                    ReportImplicitNotObvious(context);
                    break;
                }
            case SyntaxKind.CoalesceExpression:
                {
                    if (parent.IsParentKind(SyntaxKind.EqualsValueClause))
                    {
                        switch (parent.Parent.Parent)
                        {
                            case VariableDeclaratorSyntax variableDeclarator:
                                {
                                    if (variableDeclarator.Parent is VariableDeclarationSyntax)
                                        ReportImplicitObvious(context);

                                    return;
                                }
                            case PropertyDeclarationSyntax:
                                {
                                    ReportImplicitObvious(context);
                                    return;
                                }
                        }
                    }

                    ReportImplicitNotObvious(context);
                    break;
                }
            case SyntaxKind.CollectionInitializerExpression:
                {
                    SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression, SyntaxKind.SimpleAssignmentExpression), parent.Parent);

                    //TODO: ?
                    if (!UseExplicit(context))
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
                                if (parent is VariableDeclarationSyntax)
                                {
                                    SyntaxDebug.Assert(parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement), parent.Parent);

                                    ReportImplicitObvious(context);
                                }
                            }
                            else if (parent.IsKind(SyntaxKind.PropertyDeclaration))
                            {
                                ReportImplicitObvious(context);
                            }
                        }
                    }

                    break;
                }
            case SyntaxKind.ComplexElementInitializerExpression:
                {
                    break;
                }
        }
    }

    private static bool UseExplicit(SyntaxNodeAnalysisContext context)
    {
        return context.GetObjectCreationTypeStyle() == ObjectCreationTypeStyle.Explicit;
    }

    private static bool UseImplicit(SyntaxNodeAnalysisContext context)
    {
        return context.GetObjectCreationTypeStyle() == ObjectCreationTypeStyle.Implicit;
    }

    private static bool UseImplicitOrImplicitWhenObvious(SyntaxNodeAnalysisContext context)
    {
        ObjectCreationTypeStyle style = context.GetObjectCreationTypeStyle();

        return style == ObjectCreationTypeStyle.Implicit
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

    private static void AnalyzeArrayType(
        SyntaxNodeAnalysisContext context,
        ObjectCreationExpressionSyntax objectCreation,
        TypeSyntax type,
        bool isGenericType = false)
    {
        if (type is ArrayTypeSyntax arrayType)
        {
            type = arrayType.ElementType;

            if (!type.IsVar)
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
                ReportExplicitToImplicit(context, objectCreation);
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

    private static bool ReportImplicitObvious(SyntaxNodeAnalysisContext context)
    {
        return ReportImplicit(context, isObvious: true);
    }

    private static bool ReportImplicitNotObvious(SyntaxNodeAnalysisContext context)
    {
        return ReportImplicit(context, isObvious: false);
    }

    private static bool ReportImplicit(SyntaxNodeAnalysisContext context, bool isObvious)
    {
        ObjectCreationTypeStyle style = context.GetObjectCreationTypeStyle();

        if (style == ObjectCreationTypeStyle.Explicit
            || (!isObvious && style == ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious))
        {
            ReportImplicitToExplicit(context);
            return true;
        }
        else if (style == ObjectCreationTypeStyle.Implicit
            || (isObvious && style == ObjectCreationTypeStyle.ImplicitWhenTypeIsObvious))
        {
            if (context.Node.IsKind(SyntaxKind.ImplicitObjectCreationExpression))
            {
                if (UseCollectionExpression(context))
                {
                    ReportImplicitToCollectionExpression(context);
                    return true;
                }
            }
            else if (context.UseCollectionExpression() == false)
            {
                ReportCollectionExpressionToImplicit(context);
            }
        }

        return false;
    }

    private static void ReportExplicitToImplicit(SyntaxNodeAnalysisContext context, ObjectCreationExpressionSyntax objectCreation, bool canUseCollectionExpression = true)
    {
        bool useCollectionExpression = canUseCollectionExpression && UseCollectionExpression(context);

        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            objectCreation.Type.GetLocation(),
            properties: (useCollectionExpression) ? _explicitToCollectionExpression : ImmutableDictionary<string, string>.Empty,
            (useCollectionExpression) ? "collection expression" : "implicit object creation");
    }

    private static void ReportImplicitToExplicit(SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node,
            "explicit object creation");
    }

    private static void ReportImplicitToCollectionExpression(SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node.GetLocation(),
            properties: _implicitToCollectionExpression,
            "collection expression");
    }

    private static void ReportCollectionExpressionToImplicit(SyntaxNodeAnalysisContext context)
    {
        DiagnosticHelpers.ReportDiagnostic(
            context,
            DiagnosticRules.UseImplicitOrExplicitObjectCreation,
            context.Node.GetLocation(),
            properties: _collectionExpressionToImplicit,
            "implicit object creation");
    }

    private static bool IsSingleReturnStatement(SyntaxNode parent)
    {
        return parent.IsKind(SyntaxKind.ReturnStatement)
            && parent.Parent is BlockSyntax block
            && block.Statements.Count == 1
            && parent.Parent.Parent is MemberDeclarationSyntax;
    }

    private static bool UseCollectionExpression(SyntaxNodeAnalysisContext context)
    {
        Debug.Assert(!context.Node.IsKind(SyntaxKind.CollectionExpression), context.Node.Kind().ToString());

        return context.UseCollectionExpression() == true
            && ((CSharpCompilation)context.Compilation).SupportsCollectionExpression()
            && SyntaxUtility.CanConvertToCollectionExpression(context.Node, context.SemanticModel, context.CancellationToken);
    }
}
