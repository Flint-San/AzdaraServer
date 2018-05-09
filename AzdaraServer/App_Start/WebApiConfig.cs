namespace AzdaraServer
{
    using Azdara;
    using Azdara.CodeGenerator.Settings;
    using Azdara.Metadata.Reader;
    using AzdaraServer.Http;
    using Microsoft.AspNet.OData;
    using Microsoft.AspNet.OData.Builder;
    using Microsoft.AspNet.OData.Extensions;
    using Microsoft.AspNet.OData.Formatter;
    using Microsoft.OData.Edm;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Http.Dispatcher;
    using System.Web.Configuration;
    using DAL.DbContext;

    public static class WebApiConfig
    {
        private static AzdaraPOCO poco;

        private static void AzdaraInit()
        {
            string connectionStringName = WebConfigurationManager.AppSettings["defaultConnectionString"];
            string prefix = WebConfigurationManager.AppSettings["prefixCSharp"];
            string defaultDbSchema = WebConfigurationManager.AppSettings["defaultSchema"];

            //1. read structure of the database
            poco = new AzdaraPOCO(new CodeConfig() { prefixCSharp = prefix, defaultSchema = defaultDbSchema });
            poco.GetDbStructure(new DBSchemaSettings() { connectionStringName = connectionStringName, providerName = "System.Data.SqlClient" });
            //2. generate classes *.cs 
            poco.GenerateAndBuildCSharpCode(new SchemaWriterSettings() { IsRegionProperties = true });
            //3. build .dll
            poco.BuildAssembly();
        }

        public static void Register(HttpConfiguration config)
        {
            int maxServerPageSize = Convert.ToInt32(WebConfigurationManager.AppSettings["maxServerPageSize"]);

            AzdaraInit();

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // create a special dynamic controller selector
            TableControllerSelector selector = new TableControllerSelector(config);
            config.Services.Replace(typeof(IHttpControllerSelector), selector);

            //Use global configuration
            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null); // enable query options for all properties
            config.AddODataQueryFilter(new EnableQueryAttribute() {
                PageSize = maxServerPageSize,
                AllowedQueryOptions = Microsoft.AspNet.OData.Query.AllowedQueryOptions.All,
                AllowedFunctions = Microsoft.AspNet.OData.Query.AllowedFunctions.All,
                AllowedArithmeticOperators = Microsoft.AspNet.OData.Query.AllowedArithmeticOperators.All,
                AllowedLogicalOperators = Microsoft.AspNet.OData.Query.AllowedLogicalOperators.All
            });

            //строка для включения null значений в открытых типах, пример свойство в сущности public IDictionary<string, object> DynamicProperties { get; set; }
            config.Properties.AddOrUpdate("System.Web.OData.NullDynamicPropertyKey", val => true, (oldVal, newVal) => true);

            // Конфигурация и службы веб-API
            IEdmModel EDMmodel = Metadata();

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: "odata",
                model: EDMmodel
                );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // below code allows endpoints to respond with either XML or JSON, depending on accept header preferences sent from client 
            // (default in absence of accept header is JSON)
            //https://docs.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/json-and-xml-serialization
            var odataFormatters = ODataMediaTypeFormatters.Create();
            config.Formatters.InsertRange(0, odataFormatters);

        }
        
        public static IEdmModel Metadata()
        {
            //Создает модель EDM (модель EDM). OData Endpoint
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.Namespace = poco.schema.Settings.connectionStringName;
            builder.ContainerName = "Default";

            Assembly assembly = poco.assembly;

            var exportedTypes = assembly.GetExportedTypes();
            foreach (Type typeClass in exportedTypes)
            {
                //Take from poco class a [TableAttribute] 
                System.ComponentModel.DataAnnotations.Schema.TableAttribute tableAttribute =
                    typeClass.GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute)) as System.ComponentModel.DataAnnotations.Schema.TableAttribute;
                
                //Ограничение EF https://stackoverflow.com/questions/18638741/the-type-company-model-user-and-the-type-company-core-model-user-both-have-t
                builder.AddClrObject(new AssemblyTableInfo
                {
                    SqlTableOwner = tableAttribute.Schema,
                    SqlTableName = tableAttribute.Name,
                    AssemblyName = typeClass.Name,
                    AssemblyFullName = typeClass.FullName,
                    ClrTypeClass = typeClass,
                    EntitySetName = typeClass.Name
                });

            }

            EdmModel model = (EdmModel)builder.GetEdmModel();

            return model;

        }

    }
}
