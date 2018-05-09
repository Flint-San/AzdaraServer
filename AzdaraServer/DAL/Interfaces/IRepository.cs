namespace AzdaraServer.DAL.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IRepository<T>: IDisposable
        where T : class
    {
        
        T Find(IEnumerable<KeyValuePair<string, object>> primaryKeys);

        IQueryable<T> GetAll();
        IQueryable<T> GetByKeys(IEnumerable<KeyValuePair<string, object>> primaryKeys);

        void AddEntity(object newEntity);
        void RemoveEntity(object originalEntity);

        void PutEntity(IEnumerable<KeyValuePair<string, object>> primaryKeys, object newEntity);
        void PatchEntity(IEnumerable<KeyValuePair<string, object>> primaryKeys, object originalEntity, object deltaEntity);

        Task SaveAsync();
    }
}
