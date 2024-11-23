// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;
using static Roslynator.DiagnosticHelpers;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SingleLineDocumentationCommentTriviaAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(
                    ref _supportedDiagnostics,
                    DiagnosticRules.AddSummaryToDocumentationComment,
                    DiagnosticRules.AddSummaryElementToDocumentationComment,
                    DiagnosticRules.AddParamElementToDocumentationComment,
                    DiagnosticRules.AddTypeParamElementToDocumentationComment,
                    DiagnosticRules.UnusedElementInDocumentationComment,
                    DiagnosticRules.OrderElementsInDocumentationComment,
                    DiagnosticRules.FixDocumentationCommentTag,
                    DiagnosticRules.InvalidReferenceInDocumentationComment);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (IsAnyEffective(c, _supportedDiagnostics))
                    AnalyzeSingleLineDocumentationCommentTrivia(c);
            },
            SyntaxKind.SingleLineDocumentationCommentTrivia);
    }

    private static void AnalyzeSingleLineDocumentationCommentTrivia(SyntaxNodeAnalysisContext context)
    {
        var documentationComment = (DocumentationCommentTriviaSyntax)context.Node;

        if (!documentationComment.IsPartOfMemberDeclaration())
            return;

        bool? fixDocumentationCommentTagEnabled = null;
        var containsInheritDoc = false;
        var containsIncludeOrExclude = false;
        var containsSummaryElement = false;
        var containsContentElement = false;
        var isFirst = true;

        CancellationToken cancellationToken = context.CancellationToken;

        SyntaxList<XmlNodeSyntax> content = documentationComment.Content;

        for (int i = 0; i < content.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            XmlElementInfo info = SyntaxInfo.XmlElementInfo(content[i]);

            if (info.Success)
            {
                XmlTag tag = info.GetTag();
                switch (tag)
                {
                    case XmlTag.Include:
                    case XmlTag.Exclude:
                    {
                        if (isFirst)
                            containsIncludeOrExclude = true;

                        break;
                    }
                    case XmlTag.InheritDoc:
                    {
                        containsInheritDoc = true;
                        break;
                    }
                    case XmlTag.Content:
                    {
                        containsContentElement = true;
                        break;
                    }
                    case XmlTag.Summary:
                    {
                        if (info.IsContentEmptyOrWhitespace)
                            ReportDiagnosticIfEffective(context, DiagnosticRules.AddSummaryToDocumentationComment, info.Element);

                        containsSummaryElement = true;

                        if (fixDocumentationCommentTagEnabled ??= DiagnosticRules.FixDocumentationCommentTag.IsEffective(context))
                            FixDocumentationCommentTagAnalysis.Analyze(context, info);

                        break;
                    }
                    case XmlTag.Example:
                    case XmlTag.Remarks:
                    case XmlTag.Returns:
                    case XmlTag.Value:
                    {
                        if (fixDocumentationCommentTagEnabled ??= DiagnosticRules.FixDocumentationCommentTag.IsEffective(context))
                            FixDocumentationCommentTagAnalysis.Analyze(context, info);

                        AnalyzeUnusedElement(context, info, tag);
                        break;
                    }
                    case XmlTag.List:
                    {
                        if (fixDocumentationCommentTagEnabled ??= DiagnosticRules.FixDocumentationCommentTag.IsEffective(context))
                            FixDocumentationCommentTagAnalysis.Analyze(context, info);

                        break;
                    }
                    case XmlTag.Exception:
                    case XmlTag.Permission:
                    {
                        if (fixDocumentationCommentTagEnabled ??= DiagnosticRules.FixDocumentationCommentTag.IsEffective(context))
                            FixDocumentationCommentTagAnalysis.Analyze(context, info);

                        AnalyzeUnusedElement(context, info, tag, checkAttributes: true);
                        break;
                    }
                    case XmlTag.Param:
                    case XmlTag.TypeParam:
                    {
                        if (fixDocumentationCommentTagEnabled ??= DiagnosticRules.FixDocumentationCommentTag.IsEffective(context))
                            FixDocumentationCommentTagAnalysis.Analyze(context, info);

                        AnalyzeUnusedElement(context, info, tag);
                        break;
                    }
                    case XmlTag.SeeAlso:
                    {
                        AnalyzeUnusedElement(context, info, tag, checkAttributes: true);
                        break;
                    }
                    case XmlTag.C:
                    case XmlTag.Code:
                    case XmlTag.Para:
                    case XmlTag.ParamRef:
                    case XmlTag.See:
                    case XmlTag.TypeParamRef:
                    {
                        break;
                    }
                    default:
                    {
                        SyntaxDebug.Fail(content[i]);
                        break;
                    }
                }

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    containsIncludeOrExclude = false;
                }
            }
        }

        if (containsInheritDoc
            || containsIncludeOrExclude)
        {
            return;
        }

        if (!containsSummaryElement
            && !containsContentElement)
        {
            ReportDiagnosticIfEffective(context, DiagnosticRules.AddSummaryElementToDocumentationComment, documentationComment);
        }

        SyntaxNode parent = documentationComment.ParentTrivia.Token.Parent;

        bool invalidReference = DiagnosticRules.InvalidReferenceInDocumentationComment.IsEffective(context);
        bool orderParams = DiagnosticRules.OrderElementsInDocumentationComment.IsEffective(context);
        bool addParam = DiagnosticRules.AddParamElementToDocumentationComment.IsEffective(context);
        bool addTypeParam = DiagnosticRules.AddTypeParamElementToDocumentationComment.IsEffective(context);

        if (addParam
            || orderParams
            || invalidReference)
        {
            SeparatedSyntaxList<ParameterSyntax> parameters = CSharpUtility.GetParameters(
                (parent is MemberDeclarationSyntax) ? parent : parent.Parent);

            if (addParam
                && parameters.Any())
            {
                foreach (ParameterSyntax parameter in parameters)
                {
                    if (IsMissing(documentationComment, parameter))
                    {
                        ReportDiagnostic(context, DiagnosticRules.AddParamElementToDocumentationComment, documentationComment);
                        break;
                    }
                }
            }

            if (orderParams || invalidReference)
            {
                Analyze(context, documentationComment.Content, parameters, XmlTag.Param, (nodes, name) => nodes.IndexOf(name));
            }
        }

        if (addTypeParam
            || orderParams
            || invalidReference)
        {
            SeparatedSyntaxList<TypeParameterSyntax> typeParameters = CSharpUtility.GetTypeParameters(
                (parent is MemberDeclarationSyntax) ? parent : parent.Parent);

            if (addTypeParam
                && typeParameters.Any())
            {
                foreach (TypeParameterSyntax typeParameter in typeParameters)
                {
                    if (IsMissing(documentationComment, typeParameter))
                    {
                        ReportDiagnostic(context, DiagnosticRules.AddTypeParamElementToDocumentationComment, documentationComment);
                        break;
                    }
                }
            }

            if (orderParams || invalidReference)
            {
                Analyze(context, documentationComment.Content, typeParameters, XmlTag.TypeParam, (nodes, name) => nodes.IndexOf(name));
            }
        }
    }

    private static void AnalyzeUnusedElement(SyntaxNodeAnalysisContext context, XmlElementInfo info, XmlTag tag, bool checkAttributes = false)
    {
        if (DiagnosticRules.UnusedElementInDocumentationComment.IsEffective(context)
            && info.IsContentEmptyOrWhitespace
            && (!checkAttributes || !info.HasAttributes))
        {
            ReportDiagnostic(context, DiagnosticRules.UnusedElementInDocumentationComment, info.Element, XmlTagMapper.GetName(tag));
        }
    }

    private static bool IsMissing(DocumentationCommentTriviaSyntax documentationComment, ParameterSyntax parameter)
    {
        foreach (XmlNodeSyntax xmlNode in documentationComment.Content)
        {
            XmlElementInfo elementInfo = SyntaxInfo.XmlElementInfo(xmlNode);

            if (elementInfo.Success
                && !elementInfo.IsEmptyElement
                && elementInfo.HasTag(XmlTag.Param))
            {
                var element = (XmlElementSyntax)elementInfo.Element;

                string value = element.GetAttributeValueText("name");

                if (value is not null
                    && string.Equals(parameter.Identifier.ValueText, value, StringComparison.Ordinal))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool IsMissing(DocumentationCommentTriviaSyntax documentationComment, TypeParameterSyntax typeParameter)
    {
        foreach (XmlNodeSyntax xmlNode in documentationComment.Content)
        {
            XmlElementInfo elementInfo = SyntaxInfo.XmlElementInfo(xmlNode);

            if (elementInfo.Success
                && !elementInfo.IsEmptyElement
                && elementInfo.HasTag(XmlTag.TypeParam))
            {
                var element = (XmlElementSyntax)elementInfo.Element;

                string value = element.GetAttributeValueText("name");

                if (value is not null
                    && string.Equals(typeParameter.Identifier.ValueText, value, StringComparison.Ordinal))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static void Analyze<TNode>(
        SyntaxNodeAnalysisContext context,
        SyntaxList<XmlNodeSyntax> xmlNodes,
        SeparatedSyntaxList<TNode> nodes,
        XmlTag tag,
        Func<SeparatedSyntaxList<TNode>, string, int> indexOf) where TNode : SyntaxNode
    {
        XmlNodeSyntax firstElement = null;

        int firstIndex = -1;

        for (int i = 0; i < xmlNodes.Count; i++)
        {
            XmlElementInfo elementInfo = SyntaxInfo.XmlElementInfo(xmlNodes[i]);

            if (!elementInfo.Success)
                continue;

            if (!elementInfo.HasTag(tag))
            {
                firstIndex = -1;
                continue;
            }

            XmlNodeSyntax element = elementInfo.Element;

            IdentifierNameSyntax identifierName = (element.IsKind(SyntaxKind.XmlElement))
                ? ((XmlElementSyntax)element).GetAttributeValue("name")
                : ((XmlEmptyElementSyntax)element).GetAttributeValue("name");

            if (identifierName is null)
            {
                firstIndex = -1;
                continue;
            }

            int index = indexOf(nodes, identifierName.Identifier.ValueText);

            if (index == -1)
            {
                ReportDiagnosticIfEffective(context, DiagnosticRules.InvalidReferenceInDocumentationComment, identifierName, GetElementName(tag), identifierName.Identifier.ValueText);
            }
            else if (index < firstIndex)
            {
                ReportDiagnosticIfEffective(context, DiagnosticRules.OrderElementsInDocumentationComment, firstElement);
                return;
            }
            else
            {
                firstElement = element;
            }

            firstIndex = index;
        }

        static string GetElementName(XmlTag tag)
        {
            return tag switch
            {
                XmlTag.Param => "Parameter",
                XmlTag.TypeParam => "Type parameter",
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
