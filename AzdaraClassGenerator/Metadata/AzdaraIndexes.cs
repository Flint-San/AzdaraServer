namespace Azdara.Metadata
{
    public class AzdaraIndexes: AzdaraTable
    {
        public string SqlConstraintOwner { get; set; }

        public string SqlConstraintCatalog { get; set; }

        public string SqlConstraintName { get; set; }

        public string SqlIndexName { get; set; }
    }
}