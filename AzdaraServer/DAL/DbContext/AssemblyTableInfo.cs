namespace AzdaraServer.DAL.DbContext
{
    using System;

    public class AssemblyTableInfo
    {
        public string SqlTableOwner { get; set; }
        public string SqlTableName { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyFullName { get; set; }
        public Type ClrTypeClass { get; set; }
        public string EntitySetName { get; set; }
    }
}