# How to: Configure Refactorings

> Note: There is a difference between analyzers and refactorings (Please see [Analyzers vs. Refactorings](AnalyzersVsRefactorings.md)). If you want to configure analyzers please see [How to Configure Analyzers](HowToConfigureAnalyzers.md).

## Introduction

Visual Studio does not provide any configuration mechanism for refactorings. Since it is desirable to enable/disable a given refactoring, Roslynator provides two ways for configuring refactorings:

* **Visual Studio Options Page**
* **Config File**

## Visual Studio Options

* Roslynator provides standard options page that enables to disable a given refactoring.

![RefactoringsOptions](/images/RefactoringsOptions.png)

## Config File

Config file is a XML file that has name **roslynator.config**. It has to be placed in solution root directory.

By default, any setting in config file **overrides** setting from IDE options. This behavior can be disabled by unchecking 'Use config file' in the IDE options.

### Structure

```xml
<?xml version="1.0" encoding="utf-8"?>
<Roslynator>
  <Settings>
    <General>
      <PrefixFieldIdentifierWithUnderscore IsEnabled="true" />
    </General>
    <Refactorings>
      <Refactoring Id="RR0001" IsEnabled="false" />
    </Refactorings>
  </Settings>
</Roslynator>
```

### Benefits

* Config file is not bound to IDE installation.
* One configuration file can be used by multiple users.

### Default Config File

Refactorings are distinguished by their identifiers which is not very descriptive. To make config file more descriptive, you can use [Default Config File](../src/DefaultConfigFile.xml) which contains comment for each refactoring.
