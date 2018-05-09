namespace Azdara.Metadata
{
    //https://docs.microsoft.com/ru-ru/dotnet/framework/data/adonet/sql-server-schema-collections
    //https://docs.microsoft.com/ru-ru/dotnet/framework/data/adonet/oracle-schema-collections
    public class AzdaraTable
    {
        /// <summary>
        /// En: C# namespace
        /// </summary>
        //public string CSharpNameSpace { get { return string.Format("{0}{1}_{2}", Azdara.prefixCSharp, this.SqlDb, this.SqlTableOwner); } }
        public string CSharpNameSpace {
            get {
                return string.Format("{0}{1}_{2}", ConfigExt.prefixCSharp, this.SqlDb, this.SqlTableOwner);
            }
        }

        public string CSharpOwner
        {
            get
            {
                return this.SqlTableOwner.Equals(ConfigExt.defaultSchema) ? string.Empty : this.SqlTableOwner + "_";
            }
        }
        /// <summary>
        /// En: C# table name
        /// Ru: Имя таблицы в c#
        /// </summary>
        public string CSharpTableName {
            get
            {
                return string.Format("{0}{1}{2}", ConfigExt.prefixCSharp, this.CSharpOwner, this.SqlTableName);
            }
        }
        //public string CSharpTableName { get { return this.SqlTableName; } }

        /// <summary>
        /// En: Sql database name or catalog of the table.
        /// Ru: База данных SQL или каталог таблицы.
        /// </summary>
        public string SqlDb { get; set; }

        /// <summary>
        /// En: Owner of the sql table
        /// Ru: Владелец таблицы sql
        /// </summary>
        public string SqlTableOwner { get; set; }

        /// <summary>
        /// En: Sql table name
        /// Ru: Имя таблицы в SQL
        /// </summary>
        public string SqlTableName { get; set; }

        /// <summary>
        /// En: Sql full table name
        /// Ru: Полное имя таблицы по правилам в SQL
        /// </summary>
        public string SqlFullTableName { get { return string.Format("{0}.{1}.{2}", this.SqlDb, this.SqlTableOwner, this.SqlTableName); } }
    }
}
