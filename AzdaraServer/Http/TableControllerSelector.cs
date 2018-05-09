namespace AzdaraServer.Http
{
    using AzdaraServer.DAL.DbContext;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;

    public class TableControllerSelector : DefaultHttpControllerSelector
    {
        public HttpConfiguration Configuration { get; private set; }

        public TableControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            Configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            string name = GetControllerName(request);
            if (name != null && name != "Metadata") // is it a known table name?
            {
                AssemblyTableInfo assemblyTableInfo = DynamicEntitySet.listTablesInfo
                                                        .Where(w => w.EntitySetName == name)
                                                        .Single();

                Type typeClass = assemblyTableInfo.ClrTypeClass;

                //get the generyc type of your controller
                var genericControllerType = typeof(Controllers.TableController<>);
                //пример от "andyroschy" подтолкнул на мысль задания типа для контроллера https://stackoverflow.com/questions/46916688/web-api-controller-not-found-when-class-is-generic?rq=1
                //задаем тип класса из Assembly
                var controllerType = genericControllerType.MakeGenericType(typeClass); //на выходе получаем выхов контроллера типа TableController<ClrTypeClass>

                HttpControllerDescriptor desc =
                    new HttpControllerDescriptor(Configuration, name, controllerType);

                //HttpControllerDescriptor desc = 
                //    new HttpControllerDescriptor(Configuration, name, typeof(Controllers.TableController<T>));

                return desc;
            }

            return base.SelectController(request);
        }

    }
}