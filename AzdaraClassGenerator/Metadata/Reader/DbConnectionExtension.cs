namespace Azdara.Metadata.Reader
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;

    public static class DbConnectionExtension
    {
        //https://docs.microsoft.com/ru-ru/dotnet/api/system.data.sqlclient.sqlconnection.getschema?view=netframework-4.5
        public static IEnumerable<AzdaraTable> GetAzdaraTables(this DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                yield return new AzdaraTable
                {
                    SqlDb = row["TABLE_CATALOG"] as string,
                    SqlTableName = row["TABLE_NAME"] as string,
                    SqlTableOwner = row["TABLE_SCHEMA"] as string
                };
            }
        }

        public static IEnumerable<AzdaraColumn> GetAzdaraColumns(this DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                bool IsNullable = row["IS_NULLABLE"].ToString() == "NO" ? false : true;

                yield return new AzdaraColumn
                {
                    SqlDb = row["TABLE_CATALOG"] as string,
                    SqlTableName = row["TABLE_NAME"] as string,
                    SqlTableOwner = row["TABLE_SCHEMA"] as string,

                    CSharpColumnDataType = MsSqlTypeToCSharpType(row["DATA_TYPE"].ToString(), IsNullable),
                    SqlColumnName = row["COLUMN_NAME"] as string,
                    SqlColumnOrdinal = (int)row["ORDINAL_POSITION"],
                    SqlColumnDataType = row["DATA_TYPE"] as string,
                    SqlColumnNullable = IsNullable,
                    SqlColumnStringLength = row["CHARACTER_MAXIMUM_LENGTH"] as int?,
                    SqlColumnNumericPrecision = row["NUMERIC_PRECISION"] as int?,
                    SqlColumnNumericScale = row["NUMERIC_SCALE"] as int?

                };

            }
        }

        //public static IEnumerable<AzdaraIndexes> GetAzdaraIndexes(this DataTable table)
        //{
        //    foreach (DataRow row in table.Rows)
        //    {
        //        yield return new AzdaraIndexes
        //        {
        //            SqlDb = row["TABLE_CATALOG"] as string,
        //            SqlTableName = row["TABLE_NAME"] as string,
        //            SqlTableOwner = row["TABLE_SCHEMA"] as string,

        //            SqlConstraintCatalog = row["constraint_catalog"] as string,
        //            SqlConstraintOwner = row["constraint_schema"] as string,
        //            SqlConstraintName = row["constraint_name"] as string,
        //            SqlIndexName = row["index_name"] as string
        //        };

        //    }
        //}

        //public static IEnumerable<AzdaraIndexColumns> GetAzdaraIndexColumns(this DataTable table)
        //{
        //    foreach (DataRow row in table.Rows)
        //    {
        //        yield return new AzdaraIndexColumns
        //        {
        //            SqlDb = row["TABLE_CATALOG"] as string,
        //            SqlTableName = row["TABLE_NAME"] as string,
        //            SqlTableOwner = row["TABLE_SCHEMA"] as string,

        //            SqlConstraintCatalog = row["constraint_catalog"] as string,
        //            SqlConstraintOwner = row["constraint_schema"] as string,
        //            SqlConstraintName = row["constraint_name"] as string,
        //            SqlIndexName = row["index_name"] as string,

        //            SqlColumnName = row["column_name"] as string,
        //            SqlIndexColumnOrdinal = (int)row["ordinal_position"],
        //            SqlKeyType = (byte)row["KeyType"]
        //        };

        //    }
        //}

        public static IEnumerable<AzdaraPrimaryKeys> GetMsSqlPrimaryKeys(this DbConnection connection)
        {
            string sqlCommand = @"
SELECT 
	  db_Name() as constraint_catalog
    , OBJECT_SCHEMA_NAME(i.object_id) AS constraint_schema
    , i.name as constraint_name 
	, db_name() as table_catalog 
    , OBJECT_SCHEMA_NAME(i.object_id) AS table_schema
    , OBJECT_NAME(i.object_id) AS table_name
    , c.name AS column_name 
    , i.name as index_name
    , convert(int,ic.key_ordinal - 1) AS ordinal_position
FROM sys.key_constraints AS kc
JOIN sys.indexes AS i ON i.object_id = kc.parent_object_id AND kc.name = i.name
JOIN sys.index_columns AS ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE kc.type_desc = N'PRIMARY_KEY_CONSTRAINT'
ORDER BY table_catalog, table_name, ordinal_position
                ";

            var command = connection.CreateCommand();
            command.CommandText = sqlCommand;
            command.CommandType = CommandType.Text;

            // Retrieve the data.
            DbDataReader row = command.ExecuteReader();
            while (row.Read())
            {
                yield return new AzdaraPrimaryKeys
                {
                    SqlDb = row["TABLE_CATALOG"] as string,
                    SqlTableName = row["TABLE_NAME"] as string,
                    SqlTableOwner = row["TABLE_SCHEMA"] as string,

                    SqlConstraintCatalog = row["constraint_catalog"] as string,
                    SqlConstraintOwner = row["constraint_schema"] as string,
                    SqlConstraintName = row["constraint_name"] as string,
                    SqlIndexName = row["index_name"] as string,

                    SqlPK_ColumnName = row["column_name"] as string,
                    SqlIndexColumnOrdinal = (int)row["ordinal_position"]
                };
            }
        }

        public static IEnumerable<AzdaraForeignKeys> GetMsSqlForeignKeys(this DbConnection connection)
        {
            string sqlCommand = @"
select
--        PK_TABLE_CATALOG    = db_name(r.rkeydbid),
        PK_TABLE_CATALOG    = db_name(),
        PK_TABLE_SCHEMA     = schema_name(o1.schema_id),
        PK_TABLE_NAME       = o1.name,
        PK_COLUMN_NAME      = c1.name,
        PK_COLUMN_GUID      = convert(uniqueidentifier,null),
        PK_COLUMN_PROPID    = convert(int,null),
--        FK_TABLE_CATALOG    = db_name(r.fkeydbid),
        FK_TABLE_CATALOG    = db_name(),
        FK_TABLE_SCHEMA     = schema_name(o2.schema_id),
        FK_TABLE_NAME       = o2.name,
        FK_COLUMN_NAME      = c2.name,
        FK_COLUMN_GUID      = convert(uniqueidentifier,null),
        FK_COLUMN_PROPID    = convert(int,null),
        ORDINAL             = convert(int,k.constraint_column_id),
--        UPDATE_RULE         = CASE ObjectProperty(r.constid, 'CnstIsUpdateCascade')
        UPDATE_RULE         = CASE ObjectProperty(r.object_id, 'CnstIsUpdateCascade')
                              WHEN 1 THEN N'CASCADE' 
                              ELSE        N'NO ACTION' 
                              END,
--        DELETE_RULE         = CASE ObjectProperty(r.constid, 'CnstIsDeleteCascade')
        DELETE_RULE         = CASE ObjectProperty(r.object_id, 'CnstIsDeleteCascade')
                              WHEN 1 THEN N'CASCADE' 
                              ELSE        N'NO ACTION' 
                              END,
        PK_NAME             = i.name,
--        FK_NAME             = object_name(r.constid),
        FK_NAME             = object_name(r.object_id),
        DEFERRABILITY       = convert(smallint, 3) -- DBPROPVAL_DF_NOT_DEFERRABLE
    from
        sys.all_objects o1, -- ISSUE - PERF - do inner joins here instead of old join in where clause !!!
        sys.all_objects o2, -- ISSUE - PERF - do inner joins here instead of old join in where clause !!!
        sys.all_columns c1,
        sys.all_columns c2,

--        sysreferences r,
        sys.foreign_keys r inner join
        sys.foreign_key_columns k on (k.constraint_object_id = r.object_id) inner join
        sys.indexes i on (r.referenced_object_id = i.object_id and r.key_index_id = i.index_id)
    where   
        o1.object_id = r.referenced_object_id and
        o1.object_id = c1.object_id and
        c1.column_id = k.referenced_column_id and
        o2.object_id = r.parent_object_id and
        o2.object_id = c2.object_id and
        c2.column_id = k.parent_column_id
    order by 8,9,2,3,13                ";

            var command = connection.CreateCommand();
            command.CommandText = sqlCommand;
            command.CommandType = CommandType.Text;

            // Retrieve the data.
            DbDataReader row = command.ExecuteReader();
            while (row.Read())
            {
                yield return new AzdaraForeignKeys
                {
                    SqlDb = row["PK_TABLE_CATALOG"] as string,
                    SqlTableName = row["PK_TABLE_NAME"] as string,
                    SqlTableOwner = row["PK_TABLE_SCHEMA"] as string,

                    //don't use it
                    //SqlConstraintCatalog = row["FK_TABLE_CATALOG"] as string,
                    //SqlConstraintOwner = row["FK_TABLE_SCHEMA"] as string,
                    SqlConstraintName = row["FK_NAME"] as string,
                    SqlIndexName = row["FK_NAME"] as string,

                    SqlPK_ColumnName = row["PK_COLUMN_NAME"] as string,

                    SqlFK_Db = row["FK_TABLE_CATALOG"] as string,
                    SqlFK_TableOwner = row["FK_TABLE_SCHEMA"] as string,
                    SqlFK_TableName = row["FK_TABLE_NAME"] as string,

                    SqlFK_ColumnName = row["FK_COLUMN_NAME"] as string,
                    SqlIndexColumnOrdinal = (int)row["ORDINAL"]
                };
            }
        }

        //public static 
        /// <summary>
        /// En: Convert sql datatype to C# datatype
        /// Ru: Строковое преобразование типа sql в тип c#
        /// </summary>
        public static string MsSqlTypeToCSharpType(string SqlColumnDataType, bool IsNullable)
        {
            string CSharpType = null;

            switch (SqlColumnDataType)
            {
                case "bit":
                    CSharpType = "bool{0}";
                    break;
                case "tinyint":
                    CSharpType = "byte{0}";
                    break;
                case "smallint":
                    CSharpType = "short{0}";
                    break;
                case "int":
                    CSharpType = "int{0}";
                    break;
                case "bigint":
                    CSharpType = "long{0}";
                    break;
                case "real":
                    CSharpType = "System.Single{0}";
                    break;
                case "float":
                    CSharpType = "System.Double{0}";
                    break;

                case "sql_variant":
                    CSharpType = "object";
                    break;

                case "uniqueidentifier":
                    CSharpType = "System.Guid{0}";
                    break;

                case "time":
                    CSharpType = "System.TimeSpan{0}";
                    break;

                case "datetimeoffset":
                    CSharpType = "System.DateTimeOffset{0}";
                    break;

                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    CSharpType = "System.DateTime{0}";
                    break;

                case "decimal":
                case "numeric":
                case "smallmoney":
                case "money":
                    CSharpType = "System.Decimal{0}";
                    break;

                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                case "text":
                case "ntext":
                case "xml":
                case "sysname":
                    CSharpType = "string";
                    break;

                case "binary":
                case "varbinary":
                case "timestamp":
                case "image":
                    CSharpType = "System.Byte{0}[]";
                    break;
            }

            if (CSharpType != null)
                return string.Format(CSharpType, IsNullable ? "?" : string.Empty);

            return null;
        }

    }
}