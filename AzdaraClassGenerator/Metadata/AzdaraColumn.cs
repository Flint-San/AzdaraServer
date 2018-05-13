namespace Azdara.Metadata
{
    using Azdara.CodeGenerator.Helpers;

    public class AzdaraColumn : AzdaraTable
    {
        /// <summary>
        /// En: C# column name
        /// Ru: Имя столбца в C#
        /// </summary>
        public string CSharpColumnName {
            get {
                string uniqueCname = SqlColumnName.CSharpName();
                if (uniqueCname.Equals(CSharpTableName)) //proporty name MUST not equal class name, because it's reserved for constructor name!
                {
                    uniqueCname = string.Concat("_",uniqueCname);
                }
                return uniqueCname;
            }
        }

        /// <summary>
        /// En: C# datatype of the column.
        /// Ru: Тип данных C# для столбца.
        /// </summary>
        public string CSharpColumnDataType { get; set; }

        /// <summary>
        /// En: Sql column name
        /// Ru: Имя столбца в sql
        /// </summary>
        public string SqlColumnName { get; set; }

        /// <summary>
        /// En: Sequence number of the sql column as created.
        /// Ru: Порядковый номер sql столбца в таблице.
        /// </summary>
        public int SqlColumnOrdinal { get; set; }

        /// <summary>
        /// En: Specifies whether a column allows NULLs
        /// Ru: Указывает, допускает ли столбец значения NULL.
        /// </summary>
        public bool SqlColumnNullable { get; set; }

        /// <summary>
        /// En: Sql datatype of the sql column.
        /// Ru: Тип данных столбца в SQL.
        /// </summary>
        public string SqlColumnDataType { get; set; }

        /// <summary>
        /// En: Maximum length, in characters, for binary data, character data, or text and image data. Otherwise, NULL is returned.
        /// Ru: Максимальная длина в символах для двоичных данных, символьных данных или текстовых данных и изображений. В противном случае возвращается значение NULL.
        /// </summary>
        public int? SqlColumnStringLength { get; set; }

        /// <summary>
        /// En: Precision of approximate numeric data, exact numeric data, integer data, or monetary data. Otherwise, NULL is returned.
        /// Ru: Точность приблизительных числовых данных, точных числовых данных, целочисленных данных или денежных данных. В противном случае возвращается значение NULL.
        /// </summary>
        public int? SqlColumnNumericPrecision { get; set; }

        /// <summary>
        /// En: Scale of approximate numeric data, exact numeric data, integer data, or monetary data. Otherwise, NULL is returned.
        /// Ru: Масштаб приблизительных числовых данных, точных числовых данных, целочисленных данных или денежных данных. В противном случае возвращается значение NULL.
        /// </summary>
        public int? SqlColumnNumericScale { get; set; }

    }
}