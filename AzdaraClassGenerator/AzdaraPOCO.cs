namespace Azdara
{
    using Azdara.CodeGenerator;
    using Azdara.Metadata.Reader;
    using CodeBuilder;
    using CodeGenerator.Settings;
    using Metadata;
    using System;
    using System.IO;
    using System.Reflection;

    public class AzdaraPOCO
    {
        public DbSchema schema { get; private set; }
        public SchemaWriter writer { get; private set; }
        public Assembly assembly { get; private set; }
         
        public CodeConfig codeConfig { get; private set; }

        public AzdaraPOCO(CodeConfig codeConfig)
        {
            this.codeConfig = codeConfig;

            if (this.codeConfig.folderName == null) throw new ArgumentNullException("folderName");

            ConfigExt.prefixCSharp = this.codeConfig.prefixCSharp;
            ConfigExt.defaultSchema = this.codeConfig.defaultSchema;
        }

        /// <summary>
        /// Чтение структуры БД(таблицы, поля, ключи, ссылочные ключи и т.п)
        /// </summary>
        /// <param name="schemaSettings">настройки подключения к БД</param>
        public void GetDbStructure(DBSchemaSettings schemaSettings)
        {
            schema = new DbSchema(schemaSettings);
            schema.ReadAll();
        }

        /// <summary>
        /// Генерация кода c# и запись в файлы *.cs
        /// </summary>
        /// <param name="writerSettings">настройки генератора c# кода</param>
        public void GenerateCSharpCode(SchemaWriterSettings writerSettings)
        {
            writer = new SchemaWriter(schema: schema, schemaWriterSettings: writerSettings);
            writer.WriteAll(FileLocationEdm());
        }

        /// <summary>
        /// Определение местонахождения генерируемых файлов *.cs
        /// </summary>
        /// <returns></returns>
        public virtual string FileLocationEdm()
        {
            var appDomain = AppDomain.CurrentDomain;
            string basePath = appDomain.BaseDirectory;
            //string filePath = Path.Combine(basePath, string.Format("App_Data\\Edm\\{0}", _nameDB));
            string filePath = 
                Path.Combine(basePath, 
                        string.Format("Edm\\{0}{1}", codeConfig.prefixCSharp, codeConfig.folderName)
                );
            return filePath;
        }
        /// <summary>
        /// Компиляция кода с# в dll файл
        /// </summary>
        public virtual void BuildAssembly()
        {
            string dirCSharpCode = FileLocationEdm();
            string dllFileName = codeConfig.folderName;
            string[] files = Directory.GetFiles(dirCSharpCode, "*.cs", SearchOption.AllDirectories);
            assembly = BuildCSharpCode.MakeAssembly(dllFileName, dirCSharpCode, files);
        }
    }
}
