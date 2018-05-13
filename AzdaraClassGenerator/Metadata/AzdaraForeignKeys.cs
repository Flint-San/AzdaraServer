namespace Azdara.Metadata
{
    using Azdara.CodeGenerator.Helpers;

    public class AzdaraForeignKeys : AzdaraPrimaryKeys
    {
        public string CSharpFKOwner
        {
            get
            {
                return this.SqlFK_TableOwner.Equals(ConfigExt.defaultSchema) ? string.Empty : this.SqlFK_TableOwner + "_";
            }
        }
        public string CSharpFKTableName {
            get {
                return string.Format("{0}{1}{2}", ConfigExt.prefixCSharp, this.CSharpFKOwner, this.SqlFK_TableName);
            }
        }
        public string CSharpFKColumnName
        {
            get
            {
                string uniqueCname = SqlFK_ColumnName.CSharpName();
                if (uniqueCname.Equals(SqlFK_TableName)) //proporty name MUST not equal class name, because it's reserved for constructor name!
                {
                    uniqueCname = string.Concat("_", uniqueCname);
                }
                return uniqueCname;
            }
        }
        public string SqlFK_Db { get; set; }
        public string SqlFK_TableName { get; set; }
        public string SqlFK_TableOwner { get; set; }
        public string SqlFK_FullTableName { get { return string.Format("{0}.{1}.{2}", this.SqlFK_Db, this.SqlFK_TableOwner, this.SqlFK_TableName); } }

        public string SqlFK_ColumnName { get; set; }

    }
}