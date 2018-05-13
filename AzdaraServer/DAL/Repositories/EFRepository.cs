namespace AzdaraServer.DAL.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Entity.Core.Objects;
    using AzdaraServer.DAL.DbContext;
    using AzdaraServer.DAL.Interfaces;
    using System.Threading.Tasks;
    using System.Data.Entity;
    using System.Reflection;

    public class EFRepository<T>: IRepository<T>
        where T : class
    {
        private EFDbContext db;

        public EFRepository()
        {
            this.db = new EFDbContext();
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            //base.Dispose<T>(disposing);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region "internal tools"
        private ObjectQuery<T> CreateOQuery()
        {
            string entitySetName = db.objectContext.MetadataWorkspace
                        .GetEntityContainer(db.objectContext.DefaultContainerName, System.Data.Entity.Core.Metadata.Edm.DataSpace.CSpace)
                        .BaseEntitySets.Where(bes => bes.ElementType.Name == typeof(T).Name).First().Name;
            string entityName = string.Format("{0}.{1}", db.objectContext.DefaultContainerName, entitySetName);
            ObjectQuery<T> query = db.objectContext.CreateQuery<T>(entityName);
            return query;
        }

        private ObjectQuery<T> FindAsObjectQuery(IEnumerable<KeyValuePair<string, object>> keysFromODataUri)
        {
            List<ObjectParameter> parameters = new List<ObjectParameter>();

            string eSQL = string.Empty;
            string chainCondition = string.Empty;
            foreach (var kvp in keysFromODataUri)
            {
                ObjectParameter param = new ObjectParameter(kvp.Key, kvp.Value);
                parameters.Add(param);

                //Build the ESQL 
                eSQL += string.Format("{0}it.{1} = @{1}", chainCondition, kvp.Key); //Используйте ключевое слово it для ссылки на текущую инструкцию запроса.
                chainCondition = " AND ";
            }

            var query = CreateOQuery();

            var fullQuery = query.Where(eSQL, parameters.ToArray());
            return fullQuery;
        }
        
        #endregion

        /// <summary>
        /// Search a record by key fields.
        /// </summary>
        /// <param name="primaryKeys">The PRIMARY KEY constraint uniquely identifies each row in an entity. 
        /// Primary keys must contain UNIQUE values, and cannot contain NULL values.
        /// An entity can have only one primary key, which may consist of single or multiple proporties.</param>
        /// <returns>Returns found entity or empty structure.</returns>
        public T Find(IEnumerable<KeyValuePair<string, object>> keysFromODataUri)
        {
            var fullQuery = FindAsObjectQuery(keysFromODataUri);
            return fullQuery.SingleOrDefault(); //https://stackoverflow.com/questions/11839123/why-singleordefault-result-top2-in-sql
        }

        #region "READ"
        /// <summary>
        /// Query entity set. Get all rows.
        /// </summary>
        public IQueryable<T> GetAll()
        {
            var query = CreateOQuery();
            return query; //get all records
        }
        /// <summary>
        /// Query single entity. 
        /// </summary>
        /// <param name="primaryKeys">The PRIMARY KEY constraint uniquely identifies each row in an entity. 
        /// Primary keys must contain UNIQUE values, and cannot contain NULL values.
        /// An entity can have only one primary key, which may consist of single or multiple proporties.</param>
        /// <returns>Returns record by keys. 
        /// Note: Returns more recors if keys are not unique in a database table!</returns>
        public IQueryable<T> GetByKeys(IEnumerable<KeyValuePair<string, object>> primaryKeys)
        {
            var fullQuery = FindAsObjectQuery(primaryKeys);
            return fullQuery; //get by keys one or more records
        }
        #endregion

        #region "CREATE(Insert record)"
        /// <summary>
        /// Create an entity. Insert new record in a database table.
        /// </summary>
        /// <param name="newEntity">An entity consisting of the fields and values for new record.</param>
        public void AddEntity(object newEntity)
        {
            db.Entry(newEntity).State = EntityState.Added;
        }
        #endregion

        #region "DELETE(single record)"
        /// <summary>
        /// Delete an entity. Delete record from a database table.
        /// </summary>
        /// <param name="originalEntity">Entity to remove.</param>
        public void RemoveEntity(object originalEntity)
        {
            db.Entry(originalEntity).State = EntityState.Deleted;
        }
        #endregion

        #region "UPDATE(single record. Warning! EF can't update primary keys!)"
        /// <summary>
        /// Fully update an entity. Update ALL COLUMNS in a single record in database table. Warning! Entityframework can't update primary keys!
        /// </summary>
        /// <param name="primaryKeys">The PRIMARY KEY constraint uniquely identifies each row in an entity. 
        /// Primary keys must contain UNIQUE values, and cannot contain NULL values.
        /// An entity can have only one primary key, which may consist of single or multiple proporties.</param>
        /// <param name="newEntity">An entity consisting of the all fields and values for updating record. New entity MUST not contain primary keys.</param>
        public void PutEntity(IEnumerable<KeyValuePair<string, object>> primaryKeys, object newEntity)
        {
            //Injection into newEntity of the old values as primary keys.
            foreach (KeyValuePair<string, object> kvp in primaryKeys)
            {
                PropertyInfo prop = newEntity.GetType().GetProperties().Where(x => x.Name.Equals(kvp.Key)).Single();
                prop.SetValue(newEntity, kvp.Value);
            }

            db.Entry(newEntity).State = EntityState.Modified;
        }
        /// <summary>
        /// Partially update an entity. Update a(the) CUSTOM column(s) for in a database table. Warning! Entityframework can't update primary keys!
        /// </summary>
        /// <param name="primaryKeys">The PRIMARY KEY constraint uniquely identifies each row in an entity. 
        /// Primary keys must contain UNIQUE values, and cannot contain NULL values.
        /// An entity can have only one primary key, which may consist of single or multiple proporties.</param>
        /// <param name="originalEntity">Found an existing record in the table.</param>
        /// <param name="deltaEntity">Entity containing selected fields and values for updating record.</param>
        public void PatchEntity(IEnumerable<KeyValuePair<string, object>> primaryKeys, object originalEntity, object deltaEntity)
        {   
            foreach (PropertyInfo propertyInfo in originalEntity.GetType().GetProperties())
            {
                if (!primaryKeys.Any(x => x.Key == propertyInfo.Name))
                {
                    var deltaValue = deltaEntity.GetType().GetProperty(propertyInfo.Name).GetValue(deltaEntity);
                    if (deltaValue != null)
                    {
                        propertyInfo.SetValue(originalEntity, deltaValue, null);
                    }
                }
            }
        }
        #endregion
        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous save operation. The task result contains
        //  the number of state entries written to the underlying database. This can include
        //  state entries for entities and/or relationships. Relationship state entries are
        //  created for many-to-many relationships and relationships where there is no foreign
        //  key property included in the entity class (often referred to as independent associations).
        /// </returns>
        public async Task SaveAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}