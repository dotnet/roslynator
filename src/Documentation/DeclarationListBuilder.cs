// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal class DeclarationListBuilder
    {
        private static readonly SymbolDisplayFormat _namespaceFormat = SymbolDisplayFormats.NamespaceDeclaration;

        private static readonly SymbolDisplayFormat _namespaceHierarchyFormat = SymbolDisplayFormats.NamespaceDeclaration.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly);

        private static readonly SymbolDisplayFormat _typeFormat = SymbolDisplayFormats.FullDeclaration.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly);

        private static readonly SymbolDisplayFormat _memberFormat = SymbolDisplayFormats.FullDeclaration.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        private static readonly SymbolDisplayFormat _enumFieldFormat = SymbolDisplayFormats.FullDeclaration;

        private bool _pendingIndentation;
        private int _indentationLevel;
        private INamespaceSymbol _currentNamespace;

        public DeclarationListBuilder(
            StringBuilder stringBuilder = null,
            DeclarationListOptions options = null,
            IComparer<INamespaceSymbol> namespaceComparer = null,
            IComparer<INamedTypeSymbol> typeComparer = null,
            IComparer<ISymbol> memberComparer = null)
        {
            StringBuilder = stringBuilder ?? new StringBuilder();
            Options = options ?? DeclarationListOptions.Default;
            NamespaceComparer = namespaceComparer ?? NamespaceSymbolComparer.SystemFirst;
            TypeComparer = typeComparer ?? TypeDeclarationComparer.Instance;
            MemberComparer = memberComparer ?? MemberDeclarationComparer.Instance;
        }

        public StringBuilder StringBuilder { get; }

        public DeclarationListOptions Options { get; }

        public int Length => StringBuilder.Length;

        internal HashSet<INamespaceSymbol> Namespaces { get; } = new HashSet<INamespaceSymbol>(MetadataNameEqualityComparer<INamespaceSymbol>.Instance);

        public IComparer<INamespaceSymbol> NamespaceComparer { get; }

        public IComparer<INamedTypeSymbol> TypeComparer { get; }

        public IComparer<ISymbol> MemberComparer { get; }

        public virtual bool IsVisibleType(ISymbol symbol)
        {
            return symbol.IsPubliclyVisible();
        }

        public virtual bool IsVisibleMember(ISymbol symbol)
        {
            if (!symbol.IsPubliclyVisible())
                return false;

            switch (symbol.Kind)
            {
                case SymbolKind.Event:
                case SymbolKind.Field:
                case SymbolKind.Property:
                    {
                        return true;
                    }
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        switch (methodSymbol.MethodKind)
                        {
                            case MethodKind.Constructor:
                                {
                                    return methodSymbol.ContainingType.TypeKind != TypeKind.Struct
                                        || methodSymbol.Parameters.Any();
                                }
                            case MethodKind.Conversion:
                            case MethodKind.UserDefinedOperator:
                            case MethodKind.Ordinary:
                                return true;
                            case MethodKind.ExplicitInterfaceImplementation:
                            case MethodKind.StaticConstructor:
                            case MethodKind.Destructor:
                            case MethodKind.EventAdd:
                            case MethodKind.EventRaise:
                            case MethodKind.EventRemove:
                            case MethodKind.PropertyGet:
                            case MethodKind.PropertySet:
                                return false;
                            default:
                                {
                                    Debug.Fail(methodSymbol.MethodKind.ToString());
                                    break;
                                }
                        }

                        return true;
                    }
                case SymbolKind.NamedType:
                    {
                        return false;
                    }
                default:
                    {
                        Debug.Fail(symbol.Kind.ToString());
                        return false;
                    }
            }
        }

        public virtual bool IsVisibleAttribute(INamedTypeSymbol attributeType)
        {
            return DocumentationUtility.IsVisibleAttribute(attributeType);
        }

        public void Append(DocumentationModel documentationModel)
        {
            if ((Options.IgnoredParts & DeclarationListParts.AssemblyAttributes) == 0)
            {
                foreach (IAssemblySymbol assembly in documentationModel.Assemblies
                    .OrderBy(f => f.Name)
                    .ThenBy(f => f.Identity.Version))
                {
                    AppendAssemblyAttributes(assembly);
                }
            }

            if (Options.NestNamespaces)
            {
                AppendWithNamespaceHierarchy(documentationModel);
            }
            else
            {
                IEnumerable<INamedTypeSymbol> types = documentationModel.Types.Where(f => f.ContainingType == null && !Options.ShouldBeIgnored(f));

                foreach (INamespaceSymbol namespaceSymbol in types
                    .Select(f => f.ContainingNamespace)
                    .Distinct(MetadataNameEqualityComparer<INamespaceSymbol>.Instance)
                    .OrderBy(f => f, NamespaceComparer))
                {
                    if (!namespaceSymbol.IsGlobalNamespace)
                    {
                        Append(namespaceSymbol, _namespaceFormat);
                        BeginTypeContent();
                    }

                    _currentNamespace = namespaceSymbol;

                    if (Options.Depth <= DocumentationDepth.Type)
                        AppendTypes(types.Where(f => MetadataNameEqualityComparer<INamespaceSymbol>.Instance.Equals(f.ContainingNamespace, namespaceSymbol)));

                    _currentNamespace = null;

                    if (!namespaceSymbol.IsGlobalNamespace)
                    {
                        EndTypeContent();
                        AppendLine();
                    }
                }
            }
        }

        private void AppendWithNamespaceHierarchy(DocumentationModel documentationModel)
        {
            IEnumerable<INamedTypeSymbol> types = documentationModel.Types.Where(f => f.ContainingType == null && !Options.ShouldBeIgnored(f));

            var rootNamespaces = new HashSet<INamespaceSymbol>(MetadataNameEqualityComparer<INamespaceSymbol>.Instance);

            var nestedNamespaces = new HashSet<INamespaceSymbol>(MetadataNameEqualityComparer<INamespaceSymbol>.Instance);

            foreach (INamespaceSymbol namespaceSymbol in types.Select(f => f.ContainingNamespace))
            {
                if (namespaceSymbol.IsGlobalNamespace)
                {
                    rootNamespaces.Add(namespaceSymbol);
                }
                else
                {
                    INamespaceSymbol n = namespaceSymbol;

                    while (true)
                    {
                        INamespaceSymbol containingNamespace = n.ContainingNamespace;

                        if (containingNamespace.IsGlobalNamespace)
                        {
                            rootNamespaces.Add(n);
                            break;
                        }

                        nestedNamespaces.Add(n);

                        n = containingNamespace;
                    }
                }
            }

            foreach (INamespaceSymbol namespaceSymbol in rootNamespaces
                .OrderBy(f => f, NamespaceComparer))
            {
                AppendNamespace(namespaceSymbol);
                AppendLine();
            }

            void AppendNamespace(INamespaceSymbol namespaceSymbol, bool isNested = false, bool startsWithNewLine = false)
            {
                if (!namespaceSymbol.IsGlobalNamespace)
                {
                    if (isNested)
                    {
                        if (startsWithNewLine)
                            AppendLine();

                        Append("// ");
                        Append(namespaceSymbol, SymbolDisplayFormats.TypeNameAndContainingTypesAndNamespaces);
                        AppendLine();
                    }

                    Append(namespaceSymbol, _namespaceHierarchyFormat);
                    BeginTypeContent();
                }

                _currentNamespace = namespaceSymbol;

                if (Options.Depth <= DocumentationDepth.Type)
                    AppendTypes(types.Where(f => MetadataNameEqualityComparer<INamespaceSymbol>.Instance.Equals(f.ContainingNamespace, namespaceSymbol)));

                startsWithNewLine = false;

                foreach (INamespaceSymbol namespaceSymbol2 in nestedNamespaces
                    .Where(f => MetadataNameEqualityComparer<INamespaceSymbol>.Instance.Equals(f.ContainingNamespace, namespaceSymbol))
                    .Distinct(MetadataNameEqualityComparer<INamespaceSymbol>.Instance)
                    .OrderBy(f => f, NamespaceComparer)
                    .ToArray())
                {
                    nestedNamespaces.Remove(namespaceSymbol2);

                    AppendNamespace(namespaceSymbol2, isNested: true, startsWithNewLine: startsWithNewLine);

                    startsWithNewLine = true;
                }

                _currentNamespace = null;

                if (!namespaceSymbol.IsGlobalNamespace)
                    EndTypeContent();
            }
        }

        private void AppendTypes(IEnumerable<INamedTypeSymbol> types, bool insertNewLineBeforeFirstType = false)
        {
            using (IEnumerator<INamedTypeSymbol> en = types.OrderBy(f => f, TypeComparer).GetEnumerator())
            {
                if (en.MoveNext())
                {
                    if (insertNewLineBeforeFirstType)
                        AppendLine();

                    while (true)
                    {
                        TypeKind typeKind = en.Current.TypeKind;

                        Append(SymbolDeclarationBuilder.GetDisplayParts(
                            en.Current,
                            _typeFormat,
                            SymbolDisplayTypeDeclarationOptions.IncludeAccessibility | SymbolDisplayTypeDeclarationOptions.IncludeModifiers,
                            isVisibleAttribute: IsVisibleAttribute,
                            formatBaseList: Options.FormatBaseList,
                            formatConstraints: Options.FormatConstraints,
                            formatParameters: Options.FormatParameters,
                            splitAttributes: Options.SplitAttributes,
                            includeAttributeArguments: Options.IncludeAttributeArguments,
                            omitIEnumerable: Options.OmitIEnumerable));

                        switch (typeKind)
                        {
                            case TypeKind.Class:
                                {
                                    BeginTypeContent();

                                    if (Options.Depth == DocumentationDepth.Member)
                                        AppendMembers(en.Current);

                                    EndTypeContent();
                                    break;
                                }
                            case TypeKind.Delegate:
                                {
                                    AppendLine(";");
                                    break;
                                }
                            case TypeKind.Enum:
                                {
                                    BeginTypeContent();

                                    foreach (ISymbol member in en.Current.GetMembers())
                                    {
                                        if (member.Kind == SymbolKind.Field
                                            && member.DeclaredAccessibility == Accessibility.Public)
                                        {
                                            Append(member, _enumFieldFormat);
                                            Append(",");
                                            AppendLine();
                                        }
                                    }

                                    EndTypeContent();
                                    break;
                                }
                            case TypeKind.Interface:
                                {
                                    BeginTypeContent();

                                    if (Options.Depth == DocumentationDepth.Member)
                                        AppendMembers(en.Current);

                                    EndTypeContent();
                                    break;
                                }
                            case TypeKind.Struct:
                                {
                                    BeginTypeContent();

                                    if (Options.Depth == DocumentationDepth.Member)
                                        AppendMembers(en.Current);

                                    EndTypeContent();
                                    break;
                                }
                        }

                        if (en.MoveNext())
                        {
                            AppendLine();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        private void BeginTypeContent()
        {
            if (Options.NewLineBeforeOpenBrace)
            {
                AppendLine();
                AppendLine("{");
            }
            else
            {
                AppendLine(" {");
            }

            _indentationLevel++;
        }

        private void EndTypeContent()
        {
            Debug.Assert(_indentationLevel > 0, "Cannot decrease indentation level.");

            _indentationLevel--;

            AppendLine("}");
        }

        private void AppendMembers(INamedTypeSymbol typeModel)
        {
            bool isAny = false;

            using (IEnumerator<ISymbol> en = typeModel.GetMembers().Where(f => IsVisibleMember(f))
                .OrderBy(f => f, MemberComparer)
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    MemberDeclarationKind kind = MemberDeclarationComparer.GetKind(en.Current);

                    while (true)
                    {
                        ImmutableArray<SymbolDisplayPart> attributeParts = SymbolDeclarationBuilder.GetAttributesParts(
                            en.Current.GetAttributes(),
                            predicate: IsVisibleAttribute,
                            splitAttributes: Options.SplitAttributes,
                            includeAttributeArguments: Options.IncludeAttributeArguments);

                        Append(attributeParts);

                        ImmutableArray<SymbolDisplayPart> parts = en.Current.ToDisplayParts(_memberFormat);

                        //XTODO: attribute on event accessor
                        if (en.Current.Kind == SymbolKind.Property)
                        {
                            var propertySymbol = (IPropertySymbol)en.Current;

                            IMethodSymbol getMethod = propertySymbol.GetMethod;

                            if (getMethod != null)
                                parts = AppendAccessorAttributes(parts, getMethod, "get");

                            IMethodSymbol setMethod = propertySymbol.SetMethod;

                            if (setMethod != null)
                                parts = AppendAccessorAttributes(parts, setMethod, "set");
                        }

                        ImmutableArray<IParameterSymbol> parameters = en.Current.GetParameters();

                        if (parameters.Any())
                        {
                            parts = AppendParameterAttributes(parts, en.Current, parameters);

                            if (Options.FormatParameters
                                && parameters.Length > 1)
                            {
                                ImmutableArray<SymbolDisplayPart>.Builder builder = parts.ToBuilder();
                                SymbolDeclarationBuilder.FormatParameters(en.Current, builder, Options.IndentChars);

                                parts = builder.ToImmutableArray();
                            }
                        }

                        Append(parts);

                        if (en.Current.Kind != SymbolKind.Property)
                            Append(";");

                        AppendLine();

                        isAny = true;

                        if (en.MoveNext())
                        {
                            MemberDeclarationKind kind2 = MemberDeclarationComparer.GetKind(en.Current);

                            if (kind != kind2
                                || Options.EmptyLineBetweenMembers)
                            {
                                AppendLine();
                            }

                            kind = kind2;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            AppendTypes(typeModel.GetTypeMembers().Where(f => IsVisibleType(f)), insertNewLineBeforeFirstType: isAny);
        }

        private ImmutableArray<SymbolDisplayPart> AppendParameterAttributes(
            ImmutableArray<SymbolDisplayPart> parts,
            ISymbol symbol,
            ImmutableArray<IParameterSymbol> parameters)
        {
            int i = SymbolDeclarationBuilder.FindParameterListStart(symbol, parts);

            if (i == -1)
                return parts;

            int parameterIndex = 0;

            IParameterSymbol parameter = parameters[parameterIndex];

            ImmutableArray<SymbolDisplayPart> attributeParts = SymbolDeclarationBuilder.GetAttributesParts(
                parameter.GetAttributes(),
                predicate: IsVisibleAttribute,
                splitAttributes: Options.SplitAttributes,
                includeAttributeArguments: Options.IncludeAttributeArguments,
                addNewLine: false);

            if (attributeParts.Any())
            {
                parts = parts.Insert(i + 1, SymbolDisplayPartFactory.Space());
                parts = parts.InsertRange(i + 1, attributeParts);
            }

            int parenthesesDepth = 0;
            int bracesDepth = 0;
            int bracketsDepth = 0;
            int angleBracketsDepth = 0;

            ImmutableArray<SymbolDisplayPart>.Builder builder = null;

            int prevIndex = 0;

            AddParameterAttributes();

            if (builder != null)
            {
                while (prevIndex < parts.Length)
                {
                    builder.Add(parts[prevIndex]);
                    prevIndex++;
                }

                return builder.ToImmutableArray();
            }

            return parts;

            void AddParameterAttributes()
            {
                while (i < parts.Length)
                {
                    SymbolDisplayPart part = parts[i];

                    if (part.Kind == SymbolDisplayPartKind.Punctuation)
                    {
                        switch (part.ToString())
                        {
                            case ",":
                                {
                                    if (((angleBracketsDepth == 0 && parenthesesDepth == 1 && bracesDepth == 0 && bracketsDepth == 0)
                                            || (angleBracketsDepth == 0 && parenthesesDepth == 0 && bracesDepth == 0 && bracketsDepth == 1))
                                        && i < parts.Length - 1)
                                    {
                                        SymbolDisplayPart nextPart = parts[i + 1];

                                        if (nextPart.Kind == SymbolDisplayPartKind.Space)
                                        {
                                            parameterIndex++;

                                            attributeParts = SymbolDeclarationBuilder.GetAttributesParts(
                                                parameters[parameterIndex].GetAttributes(),
                                                predicate: IsVisibleAttribute,
                                                splitAttributes: Options.SplitAttributes,
                                                includeAttributeArguments: Options.IncludeAttributeArguments,
                                                addNewLine: false);

                                            if (attributeParts.Any())
                                            {
                                                if (builder == null)
                                                {
                                                    builder = ImmutableArray.CreateBuilder<SymbolDisplayPart>();

                                                    builder.AddRange(parts, i + 1);
                                                }
                                                else
                                                {
                                                    for (int j = prevIndex; j <= i; j++)
                                                        builder.Add(parts[j]);
                                                }

                                                builder.Add(SymbolDisplayPartFactory.Space());
                                                builder.AddRange(attributeParts);

                                                prevIndex = i + 1;
                                            }
                                        }
                                    }

                                    break;
                                }
                            case "(":
                                {
                                    parenthesesDepth++;
                                    break;
                                }
                            case ")":
                                {
                                    Debug.Assert(parenthesesDepth >= 0);
                                    parenthesesDepth--;

                                    if (parenthesesDepth == 0
                                        && symbol.IsKind(SymbolKind.Method, SymbolKind.NamedType))
                                    {
                                        return;
                                    }

                                    break;
                                }
                            case "[":
                                {
                                    bracketsDepth++;
                                    break;
                                }
                            case "]":
                                {
                                    Debug.Assert(bracketsDepth >= 0);
                                    bracketsDepth--;

                                    if (bracketsDepth == 0
                                        && symbol.Kind == SymbolKind.Property)
                                    {
                                        return;
                                    }

                                    break;
                                }
                            case "{":
                                {
                                    bracesDepth++;
                                    break;
                                }
                            case "}":
                                {
                                    Debug.Assert(bracesDepth >= 0);
                                    bracesDepth--;
                                    break;
                                }
                            case "<":
                                {
                                    angleBracketsDepth++;
                                    break;
                                }
                            case ">":
                                {
                                    Debug.Assert(angleBracketsDepth >= 0);
                                    angleBracketsDepth--;
                                    break;
                                }
                        }
                    }

                    i++;
                }
            }
        }

        private ImmutableArray<SymbolDisplayPart> AppendAccessorAttributes(
            ImmutableArray<SymbolDisplayPart> parts,
            IMethodSymbol method,
            string keyword)
        {
            ImmutableArray<SymbolDisplayPart> attributeParts = SymbolDeclarationBuilder.GetAttributesParts(
                method.GetAttributes(),
                predicate: IsVisibleAttribute,
                splitAttributes: Options.SplitAttributes,
                includeAttributeArguments: Options.IncludeAttributeArguments,
                addNewLine: false);

            if (attributeParts.Any())
            {
                SymbolDisplayPart part = parts.FirstOrDefault(f => f.IsKeyword(keyword));

                Debug.Assert(part.Kind == SymbolDisplayPartKind.Keyword);

                if (part.Kind == SymbolDisplayPartKind.Keyword)
                {
                    int index = parts.IndexOf(part);

                    parts = parts.Insert(index, SymbolDisplayPartFactory.Space());
                    parts = parts.InsertRange(index, attributeParts);
                }
            }

            return parts;
        }

        private void AppendAssemblyAttributes(IAssemblySymbol assemblySymbol)
        {
            ImmutableArray<AttributeData> attributes = assemblySymbol.GetAttributes();

            ImmutableArray<SymbolDisplayPart> attributeParts = SymbolDeclarationBuilder.GetAttributesParts(
                attributes,
                IsVisibleAttribute,
                splitAttributes: Options.SplitAttributes,
                includeAttributeArguments: Options.IncludeAttributeArguments,
                isAssemblyAttribute: true);

            if (attributeParts.Any())
            {
                Append("// ");
                AppendLine(assemblySymbol.Identity.Name);
                Append(attributeParts);
                AppendLine();
            }
        }

        public void Append(ISymbol symbol, SymbolDisplayFormat format)
        {
            Append(symbol.ToDisplayParts(format));
        }

        public void Append(INamedTypeSymbol symbol, SymbolDisplayFormat format, SymbolDisplayTypeDeclarationOptions typeDeclarationOptions = SymbolDisplayTypeDeclarationOptions.None)
        {
            Append(symbol.ToDisplayParts(format, typeDeclarationOptions));
        }

        private void Append(ImmutableArray<SymbolDisplayPart> parts)
        {
            foreach (SymbolDisplayPart part in parts)
            {
                CheckPendingIndentation();

                if (part.IsTypeName())
                {
                    ISymbol symbol = part.Symbol;

                    if (symbol != null)
                    {
                        INamespaceSymbol containingNamespace = symbol.ContainingNamespace;

                        if (!containingNamespace.IsGlobalNamespace
                            && ShouldAddNamespace(containingNamespace))
                        {
                            Namespaces.Add(containingNamespace);
                        }
                    }
                }

                StringBuilder.Append(part);

                if (part.Kind == SymbolDisplayPartKind.LineBreak
                    && Options.Indent)
                {
                    _pendingIndentation = true;
                }
            }

            bool ShouldAddNamespace(INamespaceSymbol containingNamespace)
            {
                if (_currentNamespace != null)
                {
                    INamespaceSymbol n = _currentNamespace;

                    do
                    {
                        if (MetadataNameEqualityComparer<INamespaceSymbol>.Instance.Equals(containingNamespace, n))
                            return false;

                        n = n.ContainingNamespace;
                    }
                    while (n?.IsGlobalNamespace == false);
                }

                return true;
            }
        }

        public void Append(string value)
        {
            CheckPendingIndentation();
            StringBuilder.Append(value);
        }

        public void Append(char value)
        {
            CheckPendingIndentation();
            StringBuilder.Append(value);
        }

        public void Append(object value)
        {
            CheckPendingIndentation();
            StringBuilder.Append(value);
        }

        public void Append(char value, int repeatCount)
        {
            CheckPendingIndentation();
            StringBuilder.Append(value, repeatCount);
        }

        public void AppendLine()
        {
            StringBuilder.AppendLine();

            if (Options.Indent)
                _pendingIndentation = true;
        }

        public void AppendLine(string value)
        {
            Append(value);
            AppendLine();
        }

        public void AppendIndentation()
        {
            for (int i = 0; i < _indentationLevel; i++)
            {
                Append(Options.IndentChars);
            }
        }

        private void CheckPendingIndentation()
        {
            if (_pendingIndentation)
            {
                _pendingIndentation = false;
                AppendIndentation();
            }
        }

        public override string ToString()
        {
            return StringBuilder.ToString();
        }
    }
}
