using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Roslynator.Core")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Pihrtsoft")]
[assembly: AssemblyProduct("Roslynator.Core")]
[assembly: AssemblyCopyright("Copyright (c) 2016-2017 Josef Pihrt")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: InternalsVisibleTo("Roslynator.CSharp.Analyzers, PublicKey=002400000480000094000000060200000024000052534131000400000100010007679a9fd3cadf"
    + "961890caf6f99bf30031375796d41ace9a0b14251f358250bd2f4cded3290020b13a480a9a3cfe"
    + "758f2fe0a9038e059b36f025c2253431e2b7e8339eb06303e557037d8dfaf50b56d7270e3bc2e1"
    + "5cc32e692a4578bef13e364eca8fd1f8787181de0503942b7d44ce6547c8d564e2e50ac0959f51"
    + "492710c4")]

[assembly: InternalsVisibleTo("Roslynator.CSharp.Refactorings, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c74aa44ee50417"
    + "e7cdd08d74b8d6fae9815db5ed25155929328e14c1ba81c027cd9c3107cb973dfb9c32c18519bd"
    + "4334974668cebc884ff15933e12532c96e341ac4a694039f3cc614aa4d0478ee9b955ec86b1261"
    + "b773382da6c90534a03417e81859426290e7c6633f35cd39df70b142978b73b608856c85f336fc"
    + "d56207b4")]

[assembly: InternalsVisibleTo("Roslynator.CSharp.CodeFixes, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b9211cc0161f42"
    + "ca93cffe3f389f413abc30d25a26bb88aeef10bd11170104f9ce069c048e117f8ee151b054f847"
    + "a9dbf042c5ccf11b927edde3b64fdbbdaff8bcc85cde47abc04eefabcc34ac163e3bc2a7dcd30a"
    + "edbc509a8f32877a53a966b67920477111808929ee6aa9940afba37c7dccff70ac97923ad17008"
    + "8b62acb8")]

[assembly: InternalsVisibleTo("Roslynator.Common, PublicKey=00240000048000009400000006020000002400005253413100040000010001009522c764838a39"
    + "6b5830615df0ec37cb920e65c593e029f6264955d4415ccfb53b6f48f05552e84ccedb6a4f062e"
    + "071a1b93ce8d841fec418119364eda47a9df4ef0ddd4f6eb11aa9218b6ab51fdf3dc8733ed5f11"
    + "ed17524d53341c64b52bba0fbe327f67847a2c727ef24b67c5ae4354037425c122b42028aca4f5"
    + "29309ea9")]

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
[assembly: AssemblyVersion("1.4.55")]
//[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]