namespace Azdara.Metadata.Reader
{
    using Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    public class DBSchemaSettings
    {
        public string providerName { get; set; }
        public string connectionStringName { get; set; }

        public DBSchemaSettings()
        {
            providerName = "System.Data.SqlClient";
        }
    }

    public class DbSchema: IDbSchema, IDbSchemaConnection
    {
        public ICollection<string> Catalogs { get; set; }

        public ICollection<AzdaraTable> Tables { get; set; }
        public ICollection<AzdaraColumn> Columns { get; set; }
        public ICollection<AzdaraPrimaryKeys> PrimaryKeys { get; set; }
        public ICollection<AzdaraForeignKeys> ForeignKeys { get; set; }

        public ICollection<string> Namespaces { get; private set; }

        public DbConnection Connection { get; set; }
        public DbProviderFactory DbFactory { get; private set; }

        public DBSchemaSettings Settings { get; set; }
        public DbSchema(DBSchemaSettings settings)
        {
            if (settings.connectionStringName == null) throw new ArgumentNullException("connectionStringName");

            this.Settings = settings;
            
            this.DbFactory = DbProviderFactories.GetFactory(settings.providerName);
            this.Connection = DbFactory.CreateConnection();
            this.Connection.ConnectionString = GetConnectionString();
        }

        public virtual string GetConnectionString()
        {
            //string defaultConnectionString = System.Web.Configuration.WebConfigurationManager.AppSettings[appSettingsKey];
            return ConfigurationManager.ConnectionStrings[Settings.connectionStringName].ConnectionString;
        }

        public virtual void OpenConnection()
        {
            if (Connection.State != ConnectionState.Open)
            {
                // Connect to the database then retrieve the schema information.  
                Connection.Open();
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }
                Connection = null;
            }
        }
        
        public virtual void ReadTables()
        {
            OpenConnection();
            Tables = Connection.GetSchema("Tables").GetAzdaraTables().ToList();

            if (Tables.Count() > 0)
            {
                Catalogs = Tables.OrderBy(o => o.CSharpNameSpace).Select(s => s.CSharpNameSpace).Distinct().ToList();
            }
        }

        public virtual void ReadColumns()
        {
            OpenConnection();
            Columns = Connection.GetSchema("Columns").GetAzdaraColumns().ToList();
        }

        public virtual void ReadPrimaryKeys()
        {
            OpenConnection();
            PrimaryKeys = Connection.GetMsSqlPrimaryKeys().ToList();
        }
        public virtual void ReadForeignKeys()
        {
            OpenConnection();
            ForeignKeys = Connection.GetMsSqlForeignKeys().ToList();
        }

        public virtual void ReadAll()
        {
            ReadTables();
            ReadColumns();
            ReadPrimaryKeys();
            ReadForeignKeys();
        }
    }

}