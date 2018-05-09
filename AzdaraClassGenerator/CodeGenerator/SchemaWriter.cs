namespace Azdara.CodeGenerator
{
    using System.Text;
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using System.Security.AccessControl;
    using System.IO;
    using System.Security.Principal;
    using Interfaces;
    using Metadata;
    using Metadata.Reader;
    using Settings;

    public class SchemaWriter: ISchemaWriter
    {
        private readonly DbSchema _schema;
        private readonly SchemaWriterSettings _ss;
        StringBuilder codeGen;
        StringBuilder codeMessage;

        public SchemaWriter(DbSchema schema, SchemaWriterSettings schemaWriterSettings)
        {
            if (schema == null) throw new ArgumentNullException("schema");

            _schema = schema;
            _ss = schemaWriterSettings;
            codeGen = new StringBuilder();
            codeMessage = new StringBuilder();
        }

        public virtual bool HasPrimaryKey(string fullTableName)
        {
            return _schema.PrimaryKeys.Any(x => x.SqlFullTableName == fullTableName);
        }

        public virtual bool HasNotNullFields(string fullTableName)
        {
            return _schema.Columns.Any(x => x.SqlFullTableName == fullTableName && 
                                            x.SqlColumnNullable == false && //для альтернавтивного ключа поле должно быть not null
                                           !x.CSharpColumnDataType.Equals("string") //и это поле входящее в ключ не может быть строковым
                                      );
        }

        public virtual AzdaraPrimaryKeys CheckColumnOnPrimaryKey(AzdaraColumn column)
        {
            var pk = _schema.PrimaryKeys
                    .Where(x => x.SqlFullTableName == column.SqlFullTableName && x.SqlPK_ColumnName == column.SqlColumnName)
                    .SingleOrDefault();
            
            return pk;
        }
        public virtual void BuildAll(string dirCSharpCode)
        {
            int countTables = 0;
            int countIgnoreTables = 0;
            int countReadOnlyTables = 0;
            MessageLog("AzdaraCodeGen has been launch.");
            foreach (string Namespace in _schema.Catalogs)
            {
                WriteLibraries();
                WriteNamespaceBegin(Namespace);
                foreach (AzdaraTable table in _schema.Tables.Where(x => x.CSharpNameSpace == Namespace).OrderBy(o => o.CSharpTableName))
                {
                    bool hasPrimaryKey = HasPrimaryKey(table.SqlFullTableName);
                    bool hasNotNullFields = HasNotNullFields(table.SqlFullTableName);

                    if (hasPrimaryKey || hasNotNullFields)
                    {
                        if (!hasPrimaryKey)
                        { //Если у таблицы нет pk, то по правилам EF всем не строковым NOT NULL столбцам будет принудительно задан ключ. И такая таблица по дебильным ограничением EF будет доступна только для чтения!
                            MessageLog(string.Format("Entity Framework Error 6002: The table/view '{0}' does not have a primary key defined.The key has been inferred and the definition was created as a read - only table/view.", table.SqlFullTableName));
                            countReadOnlyTables++;
                        }

                        WriteAnnotationTable(table);
                        WriteAnnotationClass(table);
                        WriteClassBegin(table);
                        WriteClassBody(table, hasPrimaryKey);
                        WriteClassEnd();
                        countTables++;
                    }
                    else
                    { //Если у таблицы нет pk, и если нет возможности построить ключ по существующим не строковым столбцам с NOT NULL значениями, то в связи с дебильными ограничениями EntityFramework мы их игнорируем. :(
                        MessageLog(string.Format("Entity Framework Error 6013: The table/view '{0}' does not have a primary key defined and no valid primary key could be inferred.This table/view has been excluded.To use the entity, you will need to review your schema, add the correct keys, and uncomment it.", table.SqlFullTableName));
                        countIgnoreTables++;
                    }
                    //else
                    //{
                        
                    //    WriteAnnotationComplexType();
                    //    WriteClass(table);
                    //}
                }
                WriteNamespaceEnd(Namespace);

                SaveCode(dirCSharpCode, Namespace);
            }

            MessageLog("Completed results:");
            MessageLog(string.Format("{0}Processed {1} tables/views.", _ss.Tab, countTables));
            MessageLog(string.Format("{0}Ignored {1} tables/views.", _ss.Tab, countIgnoreTables));
            MessageLog(string.Format("{0}Read-only {1} tables/views.", _ss.Tab, countReadOnlyTables));

            SaveLog(dirCSharpCode);
        }

        public virtual void WriteLibraries()
        {
            codeGen.AppendFormat("using System;{0}", _ss.NewLine);
            codeGen.AppendFormat("using System.ComponentModel.DataAnnotations;{0}", _ss.NewLine);
            codeGen.AppendFormat("using System.ComponentModel.DataAnnotations.Schema;{0}", _ss.NewLine);
            codeGen.AppendFormat("using System.Collections.Generic;{0}", _ss.NewLine);
        }

        public virtual void WriteNamespaceBegin(string Namespace)
        {
            codeGen.AppendFormat("{0}namespace {1}{0} {{", _ss.NewLine, Namespace);
        }
        public virtual void WriteNamespaceEnd(string Namespace)
        {
            codeGen.AppendFormat("{0} }}", _ss.NewLine); //end namespace
        }
        //Can be applied to an entity class to configure the corresponding table name and schema in the database.
        public virtual void WriteAnnotationTable(AzdaraTable table)
        {
            codeGen.AppendFormat("{0}[Table(\"{1}\", Schema=\"{2}\")]", _ss.NewLineAndDoubleTab, table.SqlTableName, table.SqlTableOwner);
        }
        public virtual void WriteAnnotationClass(AzdaraTable table) { }
        //Marks the class as complex type in EF 6. EF Core 2.0 does not support this attribute.
        public virtual void WriteAnnotationComplexType()
        {
            codeGen.AppendFormat("{0}[ComplexType()]", _ss.NewLineAndDoubleTab);
        }
        public virtual void WriteRegionBegin(string nameRegion)
        {
            if (_ss.IsRegionProperties)
                codeGen.AppendFormat("{0}#region \"{1}\"", _ss.NewLineAndTripleTab, nameRegion);
        }
        public virtual void WriteRegionEnd()
        {
            if (_ss.IsRegionProperties) codeGen.AppendFormat("{0}#endregion", _ss.NewLineAndTripleTab);
        }

        //public virtual void WriteFakeKey()
        //{
        //    //codeGen.AppendFormat("{0}[NotMapped]", _ss.NewLineAndTripleTab);
        //    codeGen.AppendFormat("{0}[Key]", _ss.NewLineAndTripleTab);
        //    //если вы хотите иметь таблицу с неавтоматическим индексом инкремента в структуре сущности; вам нужно добавить аннотацию [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //    codeGen.AppendFormat("{0}[DatabaseGenerated(DatabaseGeneratedOption.None)]", _ss.NewLineAndTripleTab); //https://msdn.microsoft.com/ru-ru/library/system.componentmodel.dataannotations.schema.databasegeneratedoption(v=vs.110).aspx
        //    codeGen.AppendFormat("{0}public virtual Guid {1}{2} {{ get {{ return Guid.NewGuid(); }} set {{ ; }} }}", _ss.NewLineAndTripleTab, Azdara.prefixCSharp, "FakeGuidKey");
        //}
        public virtual void WriteClassBegin(AzdaraTable table)
        {
            codeGen.AppendFormat("{0}public partial class {1} {{", _ss.NewLineAndDoubleTab, table.CSharpTableName);
        }
        public virtual void WriteClassEnd()
        {
            //end class
            codeGen.AppendFormat("{0} }}", _ss.NewLineAndDoubleTab);
        }
        public virtual void WriteClassBody(AzdaraTable table, bool hasPrimaryKey)
        {
            WriteClassConstructor(table);

            WriteRegionBegin("columns " + table.CSharpTableName);

            //if (!hasPrimaryKey) WriteFakeKey();
            
            foreach (AzdaraColumn column in _schema.Columns.Where(x => x.SqlFullTableName == table.SqlFullTableName ).OrderBy(o => o.SqlColumnOrdinal))
            {
                codeGen.AppendLine();

                if (hasPrimaryKey)
                {
                    var pk = CheckColumnOnPrimaryKey(column);
                    if (pk != null)
                        WriteAnnotationPrimaryKey(pk);
                    else
                        WriteAnnotationColumn(column);
                } else
                {
                    if (!column.SqlColumnNullable 
                        && !column.CSharpColumnDataType.Equals("string") 
                        )
                        WriteAnnotationFakePrimaryKey(column);
                    else
                        WriteAnnotationColumn(column);
                }

                WriteAnnotationColumnRequired(column);
                WriteAnnotationColumnStringLength(column);
                WriteColumn(column);
            }
            WriteRegionEnd();
            
            if (_ss.IsForeignKeys) WriteForeignKeys(table);
            
            if (_ss.IsInversionProperties) WriteInverseProperties(table);
        }
        //primary KeyAttribute
        public virtual void WriteAnnotationPrimaryKey(AzdaraPrimaryKeys pk)
        {
            codeGen.AppendFormat("{0}[Key, Column(name:\"{1}\", Order = {2}) ]", _ss.NewLineAndTripleTab, pk.SqlPK_ColumnName, pk.SqlIndexColumnOrdinal);
            //codeGen.AppendFormat("{0}[Key]", _ss.NewLineAndTripleTab);
        }

        public virtual void WriteAnnotationFakePrimaryKey(AzdaraColumn column)
        {
            codeGen.AppendFormat("{0}[Key, Column(name:\"{1}\", Order = {2}) ]", _ss.NewLineAndTripleTab, column.SqlColumnName, column.SqlColumnOrdinal);
                //codeGen.AppendFormat("{0}[Key]", _ss.NewLineAndTripleTab);
        }
        //Column name attribute. Can be applied to a property to configure the corresponding column name, order and data type in the database.
        public virtual void WriteAnnotationColumn(AzdaraColumn column)
        {
            codeGen.AppendFormat("{0}[Column(name:\"{1}\")]", _ss.NewLineAndTripleTab, column.SqlColumnName);

            //codeGen.AppendFormat("{0}[Column(name:\"{1}\", Order = {2}, TypeName = \"{3}\")]",
            //    _ss.NewLineAndTripleTab,
            //    column.SqlColumnName,
            //    column.SqlColumnOrdinal,
            //    column.SqlColumnDataType
            //    );
        }
        //DisplayNameAttribute
        public virtual void WriteAnnotationColumnDisplayName(AzdaraColumn column)
        {
            codeGen.AppendFormat("{0}[Display(Name=\"{1}\")]", _ss.NewLineAndTripleTab, column.SqlColumnName);
        }
        //RequiredAttribute
        public virtual void WriteAnnotationColumnRequired(AzdaraColumn column)
        {
            if (!column.SqlColumnNullable) {
                codeGen.AppendFormat("{0}[Required]", _ss.NewLineAndTripleTab);
            }
        }
        public virtual void WriteAnnotationColumnStringLength(AzdaraColumn column)
        {
            if (column.SqlColumnStringLength != null && column.SqlColumnStringLength > 0)
            {
                codeGen.AppendFormat("{0}[StringLength({1})]", _ss.NewLineAndTripleTab, column.SqlColumnStringLength);
            }
        }
        //Append column as property of class. 
        public virtual void WriteColumn(AzdaraColumn column)
        {
            codeGen.AppendFormat("{0}public {1} {2}{3} {{ get; set; }}", _ss.NewLineAndTripleTab, column.CSharpColumnDataType, ConfigExt.prefixCSharp, column.SqlColumnName);
        }
        
        //ForeignKeyAttribute - Can be applied to a property to mark it as a foreign key property.
        public virtual void WriteAnnotationForeignKey(string keys)
        {
            codeGen.AppendFormat("{0}[ForeignKey(\"{1}\")]", _ss.NewLineAndTripleTab, keys);
        }
        public virtual void WriteForeignKey(dynamic fkType)
        {
            codeGen.AppendFormat("{0}public {1} {2} {{ get; set; }}", _ss.NewLineAndTripleTab, fkType.CSharpTableName, fkType.SqlIndexName);
        }
        public virtual void WriteForeignKeys(AzdaraTable table)
        {
            var uniqueFKIndexName = _schema.ForeignKeys
                                            .Where(x => x.SqlFK_FullTableName == table.SqlFullTableName)
                                            //.Where(x => x.SqlFK_TableName == table.SqlTableName && x.SqlTableOwner == table.SqlTableOwner)
                                            .Select(s => new { s.CSharpTableName, s.SqlFK_TableOwner, s.SqlTableName, s.SqlIndexName })
                                            .Distinct();
            if (uniqueFKIndexName.Count() > 0)
            {
                codeGen.AppendLine();
                WriteRegionBegin("foreign keys");

                foreach (var fkType in uniqueFKIndexName)
                {
                    var fkeysPrepare = _schema.ForeignKeys
                                           .OrderBy(o => o.SqlIndexColumnOrdinal)
                                           .Where(x => x.SqlIndexName == fkType.SqlIndexName);
                    
                    var fkeysAttribute = fkeysPrepare
                       .Select(s => string.Format("{0}{1}", ConfigExt.prefixCSharp, s.SqlFK_ColumnName.ToString())) //Вытащим все ключи(поля) входящие в Foreign key, полей в fk может быть несколько.
                       .ToArray();

                    WriteAnnotationForeignKey(string.Join(",", fkeysAttribute));
                    WriteForeignKey(fkType);
                }

                WriteRegionEnd();
            }

        }
        public virtual dynamic GetInverseProperties(AzdaraTable table)
        {
            return
                            _schema.ForeignKeys
                                            .Where(x => x.SqlFullTableName == table.SqlFullTableName)
                                            .Select(s => new { s.CSharpFKTableName, s.SqlTableName, s.SqlIndexName, s.SqlFK_TableName, s.SqlFK_FullTableName })
                                            .Distinct();
        }

        public virtual dynamic GetFKColumns(string SqlIndexName)
        {
            return _schema.ForeignKeys
                                       .OrderBy(o => o.SqlIndexColumnOrdinal)
                                       .Where(x => x.SqlIndexName == SqlIndexName)
                                       .Select(s => s.SqlFK_ColumnName.ToString()) //Вытащим все ключи(поля) входящие в Foreign key, полей в fk может быть более 1!
                                       .ToArray();
        }
        //InversePropertyAttribute - Can be applied to a property to specify the inverse of a navigation property that represents the other end of the same relationship.
        public virtual void WriteInverseProperties(AzdaraTable table)
        {
            var inversionProperies = GetInverseProperties(table);

            if ( ((IEnumerable<object>)inversionProperies).Count() > 0)
            {
                codeGen.AppendLine();
                WriteRegionBegin("inversion");

                foreach (var fkType in inversionProperies)
                {
                    if (HasPrimaryKey(fkType.SqlFK_FullTableName))
                    {
                        var fieldsForReverse = GetFKColumns(fkType.SqlIndexName);

                        //codeGen.AppendFormat("{0}// reference to the name of another navigation property in class {1}", _ss.NewLineAndTripleTab, table.CSharpTableName);
                        codeGen.AppendFormat("{0}[InverseProperty(\"{1}\")]", _ss.NewLineAndTripleTab, fkType.SqlIndexName);
                        codeGen.AppendFormat("{0}public virtual ICollection<{1}> List_{1}_{2} {{ get; set; }}",
                            _ss.NewLineAndTripleTab,
                            fkType.CSharpFKTableName,
                            string.Join("_", fieldsForReverse));
                    }
                }

                WriteRegionEnd();
            }
        }

        public virtual void WriteClassConstructor(AzdaraTable table)
        {
            var inversionProperies = GetInverseProperties(table);

            if (((IEnumerable<object>)inversionProperies).Count() > 0)
            {
                codeGen.AppendLine();
                WriteRegionBegin("constructor");

                codeGen.AppendFormat("{0}public {1}() {{", _ss.NewLineAndTripleTab, table.CSharpTableName);

                foreach (var fkType in inversionProperies)
                {
                    if (HasPrimaryKey(fkType.SqlFK_FullTableName))
                    {
                        var fieldsForReverse = GetFKColumns(fkType.SqlIndexName);

                        codeGen.AppendFormat("{0}List_{1}_{2} = new HashSet<{1}>();",
                            _ss.NewLineAndFourthTab,
                            fkType.CSharpFKTableName,
                            string.Join("_", fieldsForReverse)
                            );
                    }
                }
                codeGen.AppendFormat("{0}}}", _ss.NewLineAndTripleTab); //end class constructor
                WriteRegionEnd();
            }
        }
        
        public virtual void MessageLog(string message)
        {
            codeMessage.AppendFormat("{0} {1}{2}", DateTime.Now.ToString("HH:mm:ss.fffff"), message, _ss.NewLine);
        }

        public virtual void MakePath(string dirCSharpCode)
        {
            if (!Directory.Exists(dirCSharpCode))
            {
                DirectorySecurity securityRules = new DirectorySecurity(); //https://github.com/aspnet/EntityFramework6/issues/275
                securityRules.AddAccessRule(new FileSystemAccessRule(
                                                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                                                FileSystemRights.FullControl, InheritanceFlags.ObjectInherit |
                                                InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                Directory.CreateDirectory(dirCSharpCode, securityRules);
            }
        }

        public virtual void SaveCode(string dirCSharpCode, string nm_CSharpNameSpace)
        {
            MakePath(dirCSharpCode);
            string fullPath = System.IO.Path.Combine(dirCSharpCode, nm_CSharpNameSpace + ".cs");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path: fullPath, append: false))
            {
                file.WriteLine(codeGen.ToString());
                file.Close();
            }
            codeGen.Clear();
        }

        public virtual void SaveLog(string dirCSharpCode)
        {
            string dirLogs = string.Format("{0}\\Logs", dirCSharpCode);
            MakePath(dirLogs);

            string fullPath = System.IO.Path.Combine(dirLogs, string.Format("POCO_messages-{0}.log", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_fffff")));
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path: fullPath, append: false))
            {
                file.WriteLine(codeMessage.ToString());
                file.Close();
            }
            codeMessage.Clear();
        }
    }
}

//public virtual void WriteEnforceColumnKey(AzdaraTable table, AzdaraColumn column)
//{
//    codeGen.AppendFormat("{0}[Key]", _ss.NewLineAndTripleTab);
//    codeGen.AppendFormat("{0}public Guid {2}CodeGenKey {{ get; set; }}", _ss.NewLineAndTripleTab, Azdara.prefixCSharp);
//}