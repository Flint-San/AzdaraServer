using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AzdaraServer.Chinook.Tests.Helpers
{
    public class ChecklistHelper
    {
        private static string _path = "Checklist";

        public static string GetString(string fileName)
        {
            using (TextReader reader = new StreamReader(GetProjectNotBinPath(fileName)))
            {
                return reader.ReadToEnd();
            }
        }
        public static JObject GetJson(string fileName)
        {
            return JObject.Parse(GetString(fileName));
        }
        public static XElement GetXml(string fileName)
        {
            return XElement.Parse(GetString(fileName));
        }
        
        //I don't like to include test files into resourse or
        //activated them properties "Copy to otput Directory" as "Copy Allways" for each file!
        //Instead it i prefer to read source project directory.
        private static string GetProjectNotBinPath(string fileName)
        {
            var appDomain = AppDomain.CurrentDomain;
            string basePath = appDomain.BaseDirectory;
            string projectPath = Directory.GetParent(Directory.GetParent(basePath).FullName).FullName;
            string filePath = string.Format("{0}\\{1}",_path,fileName);
            string path = Path.Combine(projectPath, filePath);
            if (!File.Exists(path))
            {
                string message = string.Format("The file '{0}' at directory {1} was not found.", fileName, path);
                throw new FileNotFoundException(message, path);
            }
            return path;
        }
    }
}
