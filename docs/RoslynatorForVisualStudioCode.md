# How to Enable Roslynator for Visual Studio Code

## Introduction

* Refactorings and code fixes are supported by VS Code at the moment.

* Analyzers should be supported in future version of VS Code.

## Step by Step Tutorial

1. [Download](http://marketplace.visualstudio.com/items?itemName=josefpihrt.Roslynator2017) latest extension from Visual Studio Marketplace

2. Change extension from **vsix** to **zip**.

3. Extract zip file.

4. Copy selected libraries to a directory of your choice (for example **C://lib/roslynator**)

   * **Roslynator.Common.dll** (required)
   * **Roslynator.Common.Workspaces.dll** (required)
   * **Roslynator.CSharp.dll** (required)
   * **Roslynator.CSharp.Workspaces.dll** (required)
   * **Roslynator.CSharp.CodeFixes.dll** (contains code fixes for compiler diagnostics)
   * **Roslynator.CSharp.Refactorings.dll** (contains refactorings)

5. It may be necessary to [unblock dll files](https://blogs.msdn.microsoft.com/delay/p/unblockingdownloadedfile/).

6. Create file at **%USERPROFILE%/.omnisharp/omnisharp.json** with following content:

   ```json
   {
     "RoslynExtensionsOptions": {
       "LocationPaths": [
         "C:/lib/roslynator"
       ]
     }
   }
   ```

   See [OmniSharp Wiki](http://github.com/OmniSharp/omnisharp-roslyn/wiki/Configuration-Options) for detail information about configuration options.

7. Restart VS Code.
