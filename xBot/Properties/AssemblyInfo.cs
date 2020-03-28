using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// La información general de un ensamblado se controla mediante el siguiente 
// conjunto de atributos. Cambie estos valores de atributo para modificar la información
// asociada con un ensamblado.
[assembly: AssemblyTitle("xBot")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("xBot")]
[assembly: AssemblyCopyright("Copyright © Engels Quintero 2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Si establece ComVisible en false, los tipos de este ensamblado no estarán visibles 
// para los componentes COM.  Si es necesario obtener acceso a un tipo en este ensamblado desde 
// COM, establezca el atributo ComVisible en true en este tipo.
[assembly: ComVisible(false)]

//Para comenzar a compilar aplicaciones que se puedan traducir, establezca
//<UICulture>CultureYouAreCodingWith</UICulture> en el archivo .csproj
//dentro de <PropertyGroup>.  Por ejemplo, si utiliza inglés de EE.UU.
//en los archivos de código fuente, establezca <UICulture> en en-US.  A continuación, quite la marca de comentario
//del atributo NeutralResourceLanguage.  Actualice "en-US" en
//la siguiente línea para que coincida con el valor UICulture del archivo de proyecto.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //donde se encuentran los diccionarios de recursos específicos del tema
                                     //(se utiliza si no se encuentra ningún recurso en la página,
                                     // ni diccionarios de recursos de la aplicación)
    ResourceDictionaryLocation.SourceAssembly //donde se encuentra el diccionario de recursos genérico
                                              //(se utiliza si no se encuentra ningún recurso en la página,
                                              // aplicación o diccionarios de recursos específicos del tema)
)]

// 1. Mayor version
// - Incompatibility between the most recent build
// 2. Minor version 
// - If a lot of changes has been made, has to include new features and can include direct patches to older ones
// 3. Revision :
// - Any change done, bugfix or not
[assembly: AssemblyVersion("0.0.1")]
[assembly: AssemblyFileVersion("0.0.1")]
