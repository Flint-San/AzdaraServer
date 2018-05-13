using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using Microsoft.OData.Edm;

namespace AzdaraServer.Http
{
    //https://docs.microsoft.com/ru-ru/aspnet/web-api/overview/odata-support-in-aspnet-web-api/odata-routing-conventions
    //https://stackoverflow.com/questions/15702843/webapi-odata-entityset-key-navigation-key-support
    //https://stackoverflow.com/questions/34819648/handle-odata-entityset-key-navigation
    //https://code.msdn.microsoft.com/Support-Composite-Key-in-d1d53161#content
    //https://github.com/rbeauchamp/Swashbuckle.OData/blob/master/Swashbuckle.OData.Sample/App_Start/CustomNavigationPropertyRoutingConvention.cs
    //https://github.com/jaystack/northwind-webapi-odatav4/blob/master/Northwind.WebApiOData4/ContainmentRoutingConvention.cs
    public class NavigationIndexRoutingConvention : EntitySetRoutingConvention
    {
        public override string SelectAction(ODataPath odataPath, HttpControllerContext context,
            ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (context.Request.Method == HttpMethod.Get &&
                odataPath.PathTemplate == "~/entityset/key/navigation/key")
            {
                Microsoft.OData.UriParser.NavigationPropertySegment navigationSegment = odataPath.Segments[2] as Microsoft.OData.UriParser.NavigationPropertySegment;
                IEdmNavigationProperty navigationProperty = navigationSegment.NavigationProperty.Partner;
                IEdmEntityType declaringType = navigationProperty.DeclaringType as IEdmEntityType;

                string actionName = "Get" + declaringType.Name;
                if (actionMap.Contains(actionName))
                {
                    // Add keys to route data, so they will bind to action parameters.
                    var keyValueSegment = odataPath.Segments[1] as Microsoft.OData.UriParser.ValueSegment;
                    context.RouteData.Values[ODataRouteConstants.Key] = keyValueSegment;

                    var relatedKeySegment = odataPath.Segments[3];
                    context.RouteData.Values[ODataRouteConstants.RelatedKey] = relatedKeySegment;

                    return actionName;
                }
            }
            // Not a match.
            return null;
        }
    }
}