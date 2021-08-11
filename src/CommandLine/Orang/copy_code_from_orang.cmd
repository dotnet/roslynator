@echo off

set _parameters=-n AssemblyInfo.cs l e ne --conflict o -i "bin,obj" l li e ne -y su

orang copy "../../../../orang/src/core" -e cs --target "core" %_parameters%
orang copy "../../../../orang/src/commandline.core" -e cs --target "commandline.core" %_parameters%

orang replace -e cs -c "namespace Orang" w -r "namespace Roslynator"

orang replace -e cs ^
 -c "// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information." l ^
 -r "// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information."

pause