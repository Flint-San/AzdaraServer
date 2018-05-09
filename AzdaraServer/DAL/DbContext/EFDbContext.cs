namespace AzdaraServer.DAL.DbContext
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Web.Configuration;

    public partial class EFDbContext: DbContext
    {
        public IObjectContextAdapter contextAdapter { get; private set; }
        public ObjectContext objectContext { get; private set; }
        
        public EFDbContext():
            base(string.Format("name={0}", WebConfigurationManager.AppSettings["defaultConnectionString"]))
        {
            //Disable initializer
            Database.SetInitializer<EFDbContext>(null);

            Configuration.ProxyCreationEnabled = false; //в EF для генерации ответа XML(Accept: application/xml) прокси мешает серилизации типов ICollection<ClrTypeClass> так как их еще нет в памяти https://stackoverflow.com/questions/6622806/datacontractresolver-knowntype-issue-when-custom-class-contains-another-custom
            contextAdapter = (System.Data.Entity.Infrastructure.IObjectContextAdapter)this;
            objectContext = contextAdapter.ObjectContext;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);

            var entityTypeConfiguration = modelBuilder.GetType().GetMethod("Entity");

            foreach (AssemblyTableInfo table in DynamicEntitySet.listTablesInfo)
            {
                var genericEntityTypeConfiguration = entityTypeConfiguration.MakeGenericMethod(table.ClrTypeClass);
                dynamic obj = genericEntityTypeConfiguration.Invoke(modelBuilder, new object[] { });
                
            }
        }
    }
}