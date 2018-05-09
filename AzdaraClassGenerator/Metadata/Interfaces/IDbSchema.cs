namespace Azdara.Metadata.Interfaces
{
    using System.Collections.Generic;

    public interface IDbSchema
    {
        ICollection<string> Catalogs { get; set; }

        /// <summary>
        /// En: List all tables.
        /// Ru: Список всех таблиц.
        /// </summary>
        ICollection<AzdaraTable> Tables { get; set; }

        /// <summary>
        /// En: List all collumns.
        /// Ru: Список всех столбцов таблиц.
        /// </summary>
        ICollection<AzdaraColumn> Columns { get; set; }

        /// <summary>
        /// En: List all primary keys.
        /// Ru: Список всех первичных ключей.
        /// </summary>
        ICollection<AzdaraPrimaryKeys> PrimaryKeys { get; set; }

        /// <summary>
        /// En: List all foreign keys.
        /// Ru: Список всех ссылочных ключей.
        /// </summary>
        ICollection<AzdaraForeignKeys> ForeignKeys { get; set; }
    }
    
    

}