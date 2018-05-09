namespace Azdara.CodeBuilder
{
    using System;
    using System.CodeDom.Compiler;
    using System.Linq;
    using System.Reflection;

    public static class BuildCSharpCode
    {
        public static CodeDomProvider CSharpProvider { get; private set; }
        public static CompilerResults compilerResults { get; private set; }

        //https://nodogmablog.bryanhogan.net/tag/dynamic-compilation/
        private static string[] GetAssemblyLocation(string[] assemblyNames)
        {
            string[] locations = new string[assemblyNames.Length];

            for (int loop = 0; loop <= assemblyNames.Length - 1; loop++)
            {
                locations[loop] = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && a.ManifestModule.Name == assemblyNames[loop]).Select(a => a.Location).FirstOrDefault();
            }
            return locations;
        }

        public static Assembly MakeAssembly(string dllFileName, string dirCSharpCode, string[] fileNames)
        {
            try
            {
                string errorMessage = String.Empty;

                CSharpProvider = CodeDomProvider.CreateProvider("CSharp");

                CompilerParameters compilerParameters = new CompilerParameters()
                {
                    GenerateExecutable = false,
                    GenerateInMemory = false,
                    //https://docs.microsoft.com/ru-ru/dotnet/csharp/language-reference/compiler-options/listed-alphabetically
                    //CompilerOptions = string.Format("-nowarn:CS2005 -recurse:{0}", dirCSharpCode),
                    //TreatWarningsAsErrors = false,
                    //IncludeDebugInformation = false,
                    OutputAssembly = string.Format("{0}\\{1}.dll", dirCSharpCode, dllFileName)
                };

                //указываем системные сборки для компилятора

                //Assembly.GetEntryAssembly();
                //compilerParameters.ReferencedAssemblies.Add("EntityFramework.dll");
                //Функция GetAssemblyLocation вызывает ошибку CS2005 при повторном запуске программы!
                //string[] dllFiles = GetAssemblyLocation(new string[] {
                //        "System.dll",
                //        "System.ComponentModel.DataAnnotations.dll",
                //        "System.Data.dll",
                //        "System.Core.dll",
                //        "EntityFramework.dll"
                //});
                //string[] dllFiles = GetAssemblyLocation(new string[] {
                //        "System.dll",
                //        "System.ComponentModel.DataAnnotations.dll",
                //        "System.Data.dll",
                //});
                string[] dllFiles = new string[] {
                        "System.dll",
                        "System.ComponentModel.DataAnnotations.dll",
                        "System.Data.dll",
                        "System.Core.dll"
                };
                compilerParameters.ReferencedAssemblies.AddRange(dllFiles);

                compilerResults = CSharpProvider.CompileAssemblyFromFile(compilerParameters, fileNames);
            
                if (compilerResults.Errors.HasErrors)
                {
                    errorMessage = compilerResults.Errors.Count.ToString() + " Errors C# building:";
                    foreach (CompilerError ce in compilerResults.Errors)
                    {
                        errorMessage += Environment.NewLine + "Line: " + ce.ToString();
                    }
                    throw new ApplicationException(errorMessage);
                }
                return compilerResults.CompiledAssembly;
                //return compilerResults.PathToAssembly;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while trying to build generated code.", ex);
            }
        }
    }
}