namespace Azdara.CodeGenerator.Interfaces
{
    using Azdara.Metadata;

    interface ISchemaWriter
    {
        void BuildAll(string dirCSharpCode);

        void WriteLibraries();

        void WriteNamespaceBegin(string Namespace); 
        void WriteNamespaceEnd(string Namespace); 

        void WriteAnnotationTable(AzdaraTable table);
        void WriteAnnotationComplexType();
        void WriteAnnotationClass(AzdaraTable table);
        void WriteClassBegin(AzdaraTable table);
        void WriteClassBody(AzdaraTable table, bool hasTablePrimaryKey);
        void WriteClassEnd();

        void WriteClassConstructor(AzdaraTable table);

        bool HasPrimaryKey(string fullTableName);
        bool HasNotNullFields(string fullTableName);

        AzdaraPrimaryKeys CheckColumnOnPrimaryKey(AzdaraColumn column);

        /// <summary>
        /// Write annotation KeyAttribute and ColumnAttribute for property of class.
        /// </summary>
        /// <param name="pk"></param>
        /// <returns></returns>
        void WriteAnnotationPrimaryKey(AzdaraPrimaryKeys pk);
        void WriteAnnotationFakePrimaryKey(AzdaraColumn column);
        /// <summary>
        /// Write annotation ColumnAttribute for property of class.
        /// </summary>
        /// <param name="column"></param>
        void WriteAnnotationColumn(AzdaraColumn column);
        /// <summary>
        /// Write annotation DisplayNameAttribute for property of class.
        /// </summary>
        /// <param name="column"></param>
        void WriteAnnotationColumnDisplayName(AzdaraColumn column);
        /// <summary>
        /// Write annotation RequiredAttribute for property of class.
        /// </summary>
        /// <param name="column"></param>
        void WriteAnnotationColumnRequired(AzdaraColumn column);
        /// <summary>
        /// Write annotation StringLengthAttribute for property of class.
        /// </summary>
        /// <param name="column"></param>
        void WriteAnnotationColumnStringLength(AzdaraColumn column);

        void WriteColumn(AzdaraColumn column);

        /// <summary>
        /// Write foreign keys for all properties of class.
        /// </summary>
        /// <param name="table"></param>
        void WriteForeignKeys(AzdaraTable table);

        /// <summary>
        /// Write annotation ForeignKeyAttribute for property of class.
        /// </summary>
        /// <param name="keys"></param>
        void WriteAnnotationForeignKey(string keys);
        void WriteForeignKey(dynamic fkType);

        /// <summary>
        /// Write annotation InversePropertyAttribute for property of class.
        /// </summary>
        /// <param name="table"></param>
        void WriteInverseProperties(AzdaraTable table);
        /// <summary>
        /// Write #region "nameRegion"
        /// </summary>
        /// <param name="nameRegion"></param>
        void WriteRegionBegin(string nameRegion);
        /// <summary>
        /// Write #endregion
        /// </summary>
        void WriteRegionEnd();

    }
}
