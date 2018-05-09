namespace AzdaraServer.DAL.DbContext
{
    using Microsoft.AspNet.OData.Builder;
    using System.Collections.Generic;

    public static class DynamicEntitySet
    {
        public static List<AssemblyTableInfo> listTablesInfo = new List<AssemblyTableInfo>();
        
        public static EntitySetConfiguration AddClrObject(this ODataModelBuilder builder, AssemblyTableInfo assemblyTableInfo)
        {
            System.Reflection.MethodInfo methodEntitySet = builder.GetType().GetMethod("EntitySet");

            var genericEntityType = methodEntitySet.MakeGenericMethod(assemblyTableInfo.ClrTypeClass);
            //Registers an EntitySet<genericEntityType>(EntitySetName);
            var entitySetConfiguration = genericEntityType.Invoke(builder, new object[] { assemblyTableInfo.EntitySetName });
            
            listTablesInfo.Add(assemblyTableInfo);

            return entitySetConfiguration as EntitySetConfiguration;
        }
    }
}