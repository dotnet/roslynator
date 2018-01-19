using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Roslynator.Workspaces")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Pihrtsoft")]
[assembly: AssemblyProduct("Roslynator.Workspaces")]
[assembly: AssemblyCopyright("Copyright (c) 2016-2018 Josef Pihrt")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: InternalsVisibleTo("Roslynator.CSharp.Analyzers, PublicKey=002400000480000094000000060200000024000052534131000400000100010047f923980de8a2"
    + "f3f091a587e29c4969e7c116fbe59d5a884e7d95369213f850faaa52fe90215d04a56874049848"
    + "bd5ccc5ed968f6a68b7a05690a78f911246d43d89ac9c81f8f2528e49e53a2dda2d98aa513bc65"
    + "4f3672a05ef281e53a5e9e3cc7b7843ec0051aafe633598310990c1aee26e73f4fa8e9cd75351d"
    + "1aeefdce")]

[assembly: InternalsVisibleTo("Roslynator.CSharp.Refactorings, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c74bcaaf3436c4"
    + "c7252b55d3f388f0f86e4f1984eedc4371f7dbdcf84639158af8f6d55726437176afce9fdefe53"
    + "d8d04ad5bca57e1e50e30c8b541f7c2d071c032367310f6a6f483a60b4545da1ecdda6dcf6a214"
    + "8183943c6eb96fce9d8773c4acfaa27abdf712580c89f284de3cb440abc59a7152501d5c4dabf4"
    + "83721eba")]

[assembly: InternalsVisibleTo("Roslynator.CSharp.CodeFixes, PublicKey=0024000004800000940000000602000000240000525341310004000001000100fdda624b34aff4"
    + "a1cf16c648096fd7b1a6e96504643548e9a7dcbfd68a931547ce5b93cc0d226933ed6916430996"
    + "538977ee05972c1c4ccde1e7199eecde8ad67ab397c09d4626a7b58003746be42e6c02560128c6"
    + "53981af331e2c3ce8932209c17c27ef91f0bcc9594885730f62ae3d6584bf4b4849ddd850ec20d"
    + "8d1be8c7")]

[assembly: InternalsVisibleTo("Roslynator.Common, PublicKey=002400000480000094000000060200000024000052534131000400000100010059888ca8b30a39"
    + "80efbfa6b84fd8eb45869abe86e11f8ac8ca9baef2660d9f5cd413b0aa8f87078841f2486ae7cb"
    + "ef7f32b596f0545e04255e54988f43bc3f55393f1c55efd8ac9b184ae84c74acc00a758ba6035d"
    + "268ed22efeffbc7ec36eea5064fdd5820e016f81dadea7ab7493fae89bbd37a26a9cd0f86bf34a"
    + "563d4da8")]

[assembly: InternalsVisibleTo("Roslynator.CodeGeneration, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d9b95872376a46"
    + "264630691e97240ee45e09d36b33cc7bff5fbb10d69aad9c91843d5be94966fb704812d2da170d"
    + "da03f47dd89b3f815e8b80c7321c86bbf13282168ed126fb00849988e561528acbe96d09143604"
    + "d34d38fd9d8e0315e89f4f32bf120192b1668ea0848393becd3cd8a4aef7ca60fbfc8851acd746"
    + "c7eda1c9")]

[assembly: InternalsVisibleTo("Roslynator.VisualStudio, PublicKey=002400000480000094000000060200000024000052534131000400000100010049b46f501cfd62"
    + "0281084cad58c70bfeff5ae2b2a341774f94c56f3184ba9d52a47f3de211de98ce5e7d6986b81e"
    + "2a373bbe622ad3f3f1d175bf6d582191fccb43b361701c35ec5fa917ce151f271be3027cd76d9c"
    + "94c08e9c73f7f0887eff31e0c9370ed0689edb906fa494c2a43ec22d330f473f6e33018ed821d1"
    + "ab5333b9")]

[assembly: InternalsVisibleTo("Roslynator.VisualStudio.Core, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b9334d9f979634"
    + "48aa80cc9e862c7f3635cca0ff373a856c090df36f5c540bbfab571110f4f57f59fe951f9ec176"
    + "27e24d3d331db917aa303ba63715c0a711769d4a297874a1a25a971653d7e25ca727e6cfb0f8cd"
    + "368a7ceca10e72dbfba2ce5f98e6bf989055c262672bb613785ca9ca735349e445b8e010e38f70"
    + "4e3ef0b1")]

[assembly: InternalsVisibleTo("Roslynator.VisualStudio.Refactorings, PublicKey=00240000048000009400000006020000002400005253413100040000010001009fd9b32c91985e"
    + "6a72b452d0e396f63c6dfbe1a2d9d2f9f97ee2a9ec85386ba835ced804004d15950125a423dede"
    + "ecae5676dc22d56963d8176e558d1055e4761daaa2a7b8bf0703069ee212516277db9346629dd8"
    + "7f56da3aef0360740c19ec1274e8d86e745d55146d251aa2e43c42eea156ced86b47a1c043cf1c"
    + "607050ef")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.6.30")]
//[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]