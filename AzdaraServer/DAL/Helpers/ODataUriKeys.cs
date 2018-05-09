namespace AzdaraServer.DAL.Helpers
{
    using Microsoft.OData.Edm;
    using System.Collections.Generic;
    using Microsoft.AspNet.OData.Routing;
    
    public static class ODataUriKeys
    {
        //get all keys FromODataUri
        public static IEnumerable<KeyValuePair<string, object>> GetKeysFromODataUri(this ODataPath path)
        {
            foreach (var segment in path.Segments)
            {
                EdmType edmType = (EdmType)segment.EdmType;
                Microsoft.OData.UriParser.KeySegment keySegment = segment as Microsoft.OData.UriParser.KeySegment;
                if (keySegment != null)
                {
                    return keySegment.Keys;
                }
            }
            return null;
        }

    }
}