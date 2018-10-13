// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    public abstract class DocumentationResources
    {
        public static DocumentationResources Default { get; } = new DefaultDocumentationResources();

        public virtual char InheritanceChar { get; } = '\u2192';
        public virtual char InlineSeparatorChar { get; } = '\u2022';

        public virtual string CloseParenthesis { get; } = ")";
        public virtual string Colon { get; } = ":";
        public virtual string Comma { get; } = ",";
        public virtual string OpenParenthesis { get; } = "(";
        public virtual string EqualsSign { get; } = "=";

        public virtual string DllExtension { get; } = "dll";
        public virtual string Ellipsis { get; } = "...";
        public virtual string FalseValue { get; } = "false";
        public virtual string TrueValue { get; } = "true";

        public abstract string AssemblyTitle { get; }
        public abstract string AttributesTitle { get; }
        public abstract string ClassesTitle { get; }
        public abstract string ClassTitle { get; }
        public abstract string CombinationOfTitle { get; }
        public abstract string ConstructorsTitle { get; }
        public abstract string ConstructorTitle { get; }
        public abstract string ContainingNamespaceTitle { get; }
        public abstract string ContainingTypeTitle { get; }
        public abstract string DelegatesTitle { get; }
        public abstract string DelegateTitle { get; }
        public abstract string DeprecatedTitle { get; }
        public abstract string DerivedAllTitle { get; }
        public abstract string DerivedTitle { get; }
        public abstract string EnumsTitle { get; }
        public abstract string EnumTitle { get; }
        public abstract string EventsTitle { get; }
        public abstract string EventTitle { get; }
        public abstract string ExamplesTitle { get; }
        public abstract string ExceptionsTitle { get; }
        public abstract string ExplicitInterfaceImplementationsTitle { get; }
        public abstract string ExplicitInterfaceImplementationTitle { get; }
        public abstract string ExtensionMethodsTitle { get; }
        public abstract string ExtensionMethodTitle { get; }
        public abstract string ExtensionsOfExternalTypesTitle { get; }
        public abstract string ExtensionsTitle { get; }
        public abstract string FieldsTitle { get; }
        public abstract string FieldTitle { get; }
        public abstract string FieldValueTitle { get; }
        public abstract string HomeTitle { get; }
        public abstract string ImplementsTitle { get; }
        public abstract string IndexersTitle { get; }
        public abstract string IndexerTitle { get; }
        public abstract string InheritanceTitle { get; }
        public abstract string InheritedFrom { get; }
        public abstract string InterfacesTitle { get; }
        public abstract string InterfaceTitle { get; }
        public abstract string MemberTitle { get; }
        public abstract string MethodsTitle { get; }
        public abstract string MethodTitle { get; }
        public abstract string NamespacesTitle { get; }
        public abstract string NamespaceTitle { get; }
        public abstract string NameTitle { get; }
        public abstract string ObjectModelTitle { get; }
        public abstract string ObsoleteMessage { get; }
        public abstract string OperatorsTitle { get; }
        public abstract string OperatorTitle { get; }
        public abstract string OtherTitle { get; }
        public abstract string OverloadsTitle { get; }
        public abstract string OverridesTitle { get; }
        public abstract string ParametersTitle { get; }
        public abstract string PropertiesTitle { get; }
        public abstract string PropertyTitle { get; }
        public abstract string PropertyValueTitle { get; }
        public abstract string RemarksTitle { get; }
        public abstract string ReturnsTitle { get; }
        public abstract string ReturnValueTitle { get; }
        public abstract string SeeAllDerivedTypes { get; }
        public abstract string SeeAlsoTitle { get; }
        public abstract string StaticClassesTitle { get; }
        public abstract string StructsTitle { get; }
        public abstract string StructTitle { get; }
        public abstract string SummaryTitle { get; }
        public abstract string TypeParametersTitle { get; }
        public abstract string ValuesTitle { get; }
        public abstract string ValueTitle { get; }

        public string GetName(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Event:
                    return EventTitle;
                case SymbolKind.Field:
                    return FieldTitle;
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        switch (methodSymbol.MethodKind)
                        {
                            case MethodKind.Constructor:
                                return ConstructorTitle;
                            case MethodKind.Conversion:
                            case MethodKind.UserDefinedOperator:
                                return OperatorTitle;
                            case MethodKind.ExplicitInterfaceImplementation:
                            case MethodKind.Ordinary:
                                return MethodTitle;
                        }

                        throw new InvalidOperationException();
                    }
                case SymbolKind.Namespace:
                    return NamespaceTitle;
                case SymbolKind.Property:
                    {
                        return (((IPropertySymbol)symbol).IsIndexer) ? IndexerTitle : PropertyTitle;
                    }
                case SymbolKind.NamedType:
                    return GetName(((ITypeSymbol)symbol).TypeKind);
            }

            throw new InvalidOperationException();
        }

        internal string GetPluralName(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Event:
                    return EventsTitle;
                case SymbolKind.Field:
                    return FieldsTitle;
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        switch (methodSymbol.MethodKind)
                        {
                            case MethodKind.Constructor:
                                return ConstructorsTitle;
                            case MethodKind.Conversion:
                            case MethodKind.UserDefinedOperator:
                                return OperatorsTitle;
                            case MethodKind.ExplicitInterfaceImplementation:
                            case MethodKind.Ordinary:
                                return MethodsTitle;
                        }

                        throw new InvalidOperationException();
                    }
                case SymbolKind.Namespace:
                    return NamespacesTitle;
                case SymbolKind.Property:
                    return PropertiesTitle;
                case SymbolKind.NamedType:
                    return GetPluralName(((ITypeSymbol)symbol).TypeKind);
            }

            throw new InvalidOperationException();
        }

        public string GetName(TypeKind typeKind)
        {
            switch (typeKind)
            {
                case TypeKind.Class:
                    return ClassTitle;
                case TypeKind.Delegate:
                    return DelegateTitle;
                case TypeKind.Enum:
                    return EnumTitle;
                case TypeKind.Interface:
                    return InterfaceTitle;
                case TypeKind.Struct:
                    return StructTitle;
            }

            throw new InvalidOperationException();
        }

        public string GetPluralName(TypeKind typeKind)
        {
            switch (typeKind)
            {
                case TypeKind.Class:
                    return ClassesTitle;
                case TypeKind.Delegate:
                    return DelegatesTitle;
                case TypeKind.Enum:
                    return EnumsTitle;
                case TypeKind.Interface:
                    return InterfacesTitle;
                case TypeKind.Struct:
                    return StructsTitle;
            }

            throw new InvalidOperationException();
        }

        internal string GetHeading(NamespaceDocumentationParts part)
        {
            switch (part)
            {
                case NamespaceDocumentationParts.Examples:
                    return ExamplesTitle;
                case NamespaceDocumentationParts.Remarks:
                    return RemarksTitle;
                case NamespaceDocumentationParts.Classes:
                    return ClassesTitle;
                case NamespaceDocumentationParts.Structs:
                    return StructsTitle;
                case NamespaceDocumentationParts.Interfaces:
                    return InterfacesTitle;
                case NamespaceDocumentationParts.Enums:
                    return EnumsTitle;
                case NamespaceDocumentationParts.Delegates:
                    return DelegatesTitle;
                case NamespaceDocumentationParts.SeeAlso:
                    return SeeAlsoTitle;
                default:
                    throw new ArgumentException("", nameof(part));
            }
        }

        internal string GetHeading(TypeDocumentationParts part)
        {
            switch (part)
            {
                case TypeDocumentationParts.Examples:
                    return ExamplesTitle;
                case TypeDocumentationParts.Remarks:
                    return RemarksTitle;
                case TypeDocumentationParts.Constructors:
                    return ConstructorsTitle;
                case TypeDocumentationParts.Fields:
                    return FieldsTitle;
                case TypeDocumentationParts.Indexers:
                    return IndexersTitle;
                case TypeDocumentationParts.Properties:
                    return PropertiesTitle;
                case TypeDocumentationParts.Methods:
                    return MethodsTitle;
                case TypeDocumentationParts.Operators:
                    return OperatorsTitle;
                case TypeDocumentationParts.Events:
                    return EventsTitle;
                case TypeDocumentationParts.ExplicitInterfaceImplementations:
                    return ExplicitInterfaceImplementationsTitle;
                case TypeDocumentationParts.ExtensionMethods:
                    return ExtensionMethodsTitle;
                case TypeDocumentationParts.Classes:
                    return ClassesTitle;
                case TypeDocumentationParts.Structs:
                    return StructsTitle;
                case TypeDocumentationParts.Interfaces:
                    return InterfacesTitle;
                case TypeDocumentationParts.Enums:
                    return EnumsTitle;
                case TypeDocumentationParts.Delegates:
                    return DelegatesTitle;
                case TypeDocumentationParts.SeeAlso:
                    return SeeAlsoTitle;
                default:
                    throw new ArgumentException("", nameof(part));
            }
        }

        internal string GetHeading(RootDocumentationParts part)
        {
            switch (part)
            {
                case RootDocumentationParts.Namespaces:
                    return NamespacesTitle;
                case RootDocumentationParts.Classes:
                    return ClassesTitle;
                case RootDocumentationParts.StaticClasses:
                    return StaticClassesTitle;
                case RootDocumentationParts.Structs:
                    return StructsTitle;
                case RootDocumentationParts.Interfaces:
                    return InterfacesTitle;
                case RootDocumentationParts.Enums:
                    return EnumsTitle;
                case RootDocumentationParts.Delegates:
                    return DelegatesTitle;
                case RootDocumentationParts.Other:
                    return OtherTitle;
                default:
                    throw new ArgumentException("", nameof(part));
            }
        }

        private class DefaultDocumentationResources : DocumentationResources
        {
            public override string AssemblyTitle { get; } = "Assembly";
            public override string AttributesTitle { get; } = "Attributes";
            public override string ClassesTitle { get; } = "Classes";
            public override string ClassTitle { get; } = "Class";
            public override string CombinationOfTitle { get; } = "Combination of";
            public override string ConstructorsTitle { get; } = "Constructors";
            public override string ConstructorTitle { get; } = "Constructor";
            public override string ContainingNamespaceTitle { get; } = "Containing Namespace";
            public override string ContainingTypeTitle { get; } = "Containing Type";
            public override string DelegatesTitle { get; } = "Delegates";
            public override string DelegateTitle { get; } = "Delegate";
            public override string DeprecatedTitle { get; } = "deprecated";
            public override string DerivedAllTitle { get; } = "Derived (All)";
            public override string DerivedTitle { get; } = "Derived";
            public override string EnumsTitle { get; } = "Enums";
            public override string EnumTitle { get; } = "Enum";
            public override string EventsTitle { get; } = "Events";
            public override string EventTitle { get; } = "Event";
            public override string ExamplesTitle { get; } = "Examples";
            public override string ExceptionsTitle { get; } = "Exceptions";
            public override string ExplicitInterfaceImplementationsTitle { get; } = "Explicit Interface Implementations";
            public override string ExplicitInterfaceImplementationTitle { get; } = "Explicit Interface Implementation";
            public override string ExtensionMethodsTitle { get; } = "Extension Methods";
            public override string ExtensionMethodTitle { get; } = "Extension Method";
            public override string ExtensionsOfExternalTypesTitle { get; } = "Extensions of External Types";
            public override string ExtensionsTitle { get; } = "Extensions";
            public override string FieldsTitle { get; } = "Fields";
            public override string FieldTitle { get; } = "Field";
            public override string FieldValueTitle { get; } = "Field Value";
            public override string HomeTitle { get; } = "Home";
            public override string ImplementsTitle { get; } = "Implements";
            public override string IndexersTitle { get; } = "Indexers";
            public override string IndexerTitle { get; } = "Indexer";
            public override string InheritanceTitle { get; } = "Inheritance";
            public override string InheritedFrom { get; } = "Inherited from";
            public override string InterfacesTitle { get; } = "Interfaces";
            public override string InterfaceTitle { get; } = "Interface";
            public override string MemberTitle { get; } = "Member";
            public override string MethodsTitle { get; } = "Methods";
            public override string MethodTitle { get; } = "Method";
            public override string NamespacesTitle { get; } = "Namespaces";
            public override string NamespaceTitle { get; } = "Namespace";
            public override string NameTitle { get; } = "Name";
            public override string ObjectModelTitle { get; } = "Object Model";
            public override string ObsoleteMessage { get; } = "WARNING: This API is now obsolete.";
            public override string OperatorsTitle { get; } = "Operators";
            public override string OperatorTitle { get; } = "Operator";
            public override string OtherTitle { get; } = "Other";
            public override string OverloadsTitle { get; } = "Overloads";
            public override string OverridesTitle { get; } = "Overrides";
            public override string ParametersTitle { get; } = "Parameters";
            public override string PropertiesTitle { get; } = "Properties";
            public override string PropertyTitle { get; } = "Property";
            public override string PropertyValueTitle { get; } = "Property Value";
            public override string RemarksTitle { get; } = "Remarks";
            public override string ReturnsTitle { get; } = "Returns";
            public override string ReturnValueTitle { get; } = "Return Value";
            public override string SeeAllDerivedTypes { get; } = "See all derived types";
            public override string SeeAlsoTitle { get; } = "See Also";
            public override string StaticClassesTitle { get; } = "Static Classes";
            public override string StructsTitle { get; } = "Structs";
            public override string StructTitle { get; } = "Struct";
            public override string SummaryTitle { get; } = "Summary";
            public override string TypeParametersTitle { get; } = "Type Parameters";
            public override string ValuesTitle { get; } = "Values";
            public override string ValueTitle { get; } = "Value";
        }
    }
}
