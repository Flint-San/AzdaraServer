namespace Azdara.CodeGenerator.Settings
{
    using System;

    public class SchemaWriterSettings
    {
        public string NewLine { get; set; }
        public string Tab { get; set; }
        public string DoubleTab { get { return Tab + Tab; } }
        public string TripleTab { get { return Tab + DoubleTab; } }
        public string FourthTab { get { return Tab + TripleTab; } }
        public string NewLineAndTab { get { return NewLine + Tab; } }
        public string NewLineAndDoubleTab { get { return NewLine + DoubleTab; } }
        public string NewLineAndTripleTab { get { return NewLine + TripleTab; } }
        public string NewLineAndFourthTab { get { return NewLine + FourthTab; } }

        public bool IsRegionProperties { get; set; }
        public bool IsForeignKeys { get; set; }
        public bool IsInversionProperties { get; set; }

        public SchemaWriterSettings()
        {
            NewLine = Environment.NewLine;
            Tab = "\t";
            IsRegionProperties = true;
            IsForeignKeys = true;
            IsInversionProperties = true;
        }

    }
}
