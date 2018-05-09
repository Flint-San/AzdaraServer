namespace Azdara
{
    public class CodeConfig
    {
        public bool useCSharpPrefix { get; set; }
        public string prefixCSharp { get; set; }
        public string defaultSchema { get; set; }

        public CodeConfig()
        {
            useCSharpPrefix = false;
            if (useCSharpPrefix) prefixCSharp = "Azdara_";
            defaultSchema = "dbo";
        }
    }
}
