
# How to Update Documentation

Documentation for [analyzers](analyzers) and [refactorings](refactorings) cannot be edited directly.
It is generated from following metadata files:
* [Analyzers.xml](../src/Analyzers/Analyzers.xml)
* [Refactorings.xml](../src/Refactorings/Refactorings.xml)

To update documentation it is necessary to edit [Analyzers.xml](../src/Analyzers/Analyzers.xml) and/or [Refactorings.xml](../src/Refactorings/Refactorings.xml) and then run script [MetadataGenerator.cmd](../tools/MetadataGenerator.cmd) to generate documentation.

## Structure of Analyzers.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<Analyzers>
  <Analyzer Identifier="Identifier">
    <Id>RCSXXXX</Id>
    <Title>Title</Title>
    <MessageFormat>MessageFormat</MessageFormat> <!-- Message if often same as title. -->
    <Category>General</Category> <!-- See https://github.com/JosefPihrt/Roslynator/blob/master/src/Analyzers/DiagnosticCategories.cs -->
    <DefaultSeverity>Info</DefaultSeverity> <!-- Hidden, Info, Warning or Error -->
    <IsEnabledByDefault>true</IsEnabledByDefault>
    <SupportsFadeOut>false</SupportsFadeOut> <!-- true if analyzer will fade some tokens -->
    <SupportsFadeOutAnalyzer>false</SupportsFadeOutAnalyzer> <!-- if true, RCSXXXXFadeOut analyzer will be generated -->
    <MinLanguageVersion>0.0</MinLanguageVersion> <!-- optional section that specified minimal language version -->
    <Summary>Summary</Summary> <!-- optional section that contains raw markdown -->
    <Samples> 
      <Sample>
        <Before><![CDATA[/* A code with diagnostic */ // [|Id|]]]></Before>
        <After><![CDATA[/* A code with fix */]]></After> <!-- omit this section if a diagnostic does not have a fix -->
      </Sample>
    </Samples>
    <Remarks>Remarks</Remarks> <!-- optional section that contains raw markdown -->
    <Links> <!-- optional links -->
      <Link>
        <Url>http://github.com/JosefPihrt/Roslynator</Url>
        <Text>A sample link</Text>
      </Link>
    </Links>
  </Analyzer>
</Analyzers>
```

Metadata sample above will produce [RCSXXXX.md](analyzers/RCSXXXX.md).

## Structure of Refactorings.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<Refactorings>
  <Refactoring Id="RRXXXX" Identifier="Identifier" Title="Title" IsEnabledByDefault="false"> <!-- IsEnabledByDefault="true" can be omitted -->
    <Syntaxes>
      <Syntax>supported syntax (for example 'if statement')</Syntax>
    </Syntaxes>
    <Span>A span inside which a refactoring is supported (for example 'if keyword')</Span>
    <Summary>Summary</Summary> <!-- optional section that contains raw markdown -->
    <Samples>
      <Sample>
        <Before><![CDATA[/* A code before the refactoring is applied */]]></Before>
        <After><![CDATA[/* A code after the refactoring is applied */]]></After>
      </Sample>
    </Samples>
    <Remarks>Remarks</Remarks> <!-- optional section that contains raw markdown -->
    <Links> <!-- optional links -->
      <Link>
        <Url>http://github.com/JosefPihrt/Roslynator</Url>
        <Text>A sample link</Text>
      </Link>
    </Links>
  </Refactoring>
</Refactorings>
```

Metadata sample above will produce [RRXXXX.md](refactorings/RRXXXX.md).
