namespace Azdara
{
    using System;

    public class CodeConfig
    {
        public bool useCSharpPrefix { get; set; }
        public string prefixCSharp { get; set; }
        public string defaultSchema { get; set; }
        public string folderName { get; set; }

        public CodeConfig()
        {
            useCSharpPrefix = false;
            if (useCSharpPrefix) prefixCSharp = "Azdara_";
            defaultSchema = "dbo";
        }
    }
}
