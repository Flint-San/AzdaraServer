namespace Azdara.Metadata
{
    public class AzdaraIndexColumns : AzdaraIndexes
    {
        public string SqlColumnName { get; set; }

        public int SqlIndexColumnOrdinal { get; set; }

        public byte SqlKeyType { get; set; }
    }
}