
namespace AzdaraServer.Controllers
{
    using Microsoft.AspNet.OData;
    using AzdaraServer.DAL.Interfaces;
    using System.Linq;
    using Microsoft.AspNet.OData.Extensions;
    using DAL.Repositories;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Net;
    using System.Collections.Generic;
    using DAL.Helpers;
    
    public class TableController<T> : ODataController
        where T : class
    {
        public IRepository<T> db;

        public TableController()
        {
            db = new EFRepository<T>();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        #region "REST"
        //Examples:
        //1) Get all records: http://host/Azdara/odata/Customers
        //2) Get a single record by surrogate id: http://host/Azdara/odata/Customers(1)
        //3) Get a single record by composite primary keys: http://host/Azdara/Passport(firstName = "Aleksander", lastName = "Stolpovskikh")
        //4) Get a single record by other composite primary keys: http://host/Azdara/DriverLicense(firstName = "Alexander", lastName = "Stolpovskikh", middleName = "Nickolaevich")
        // etc you can have more composite primary keys with any datatypes in your entity
        //5) any datatypes http://host/Azdara/Telephones(firstDigit = "+7", areaDigit = 351, phone = 9000000) 
        //thus TableController has not methods as like GetById(datatype key) or GetByKeys(key1,key2,key3 ... etc how more keys in an entity can you have?), 
        //because you can have any datatype for your key(s)!
        public IQueryable<T> Get()
        {
            IEnumerable<KeyValuePair<string, object>> primaryKeys =
                Request.ODataProperties().Path.GetKeysFromODataUri();
            if (primaryKeys != null) //User has request a single record by key(s).
            {
                var fullQuery = db.GetByKeys(primaryKeys);
                //if you need result as Status: 404 "Not found" uncomment this lines
                //var entry = fullQuery.SingleOrDefault();
                //if (entry == null) throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
                return fullQuery;
            }
            return db.GetAll(); //User has request entity without key(s). Returns all records as result.
        }

        public async Task<IHttpActionResult> Post(T newEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.AddEntity(newEntity);
            await db.SaveAsync();
            return Created(newEntity);
        }

        public async Task<IHttpActionResult> Delete()
        {
            IEnumerable<KeyValuePair<string, object>> primaryKeys = 
                Request.ODataProperties().Path.GetKeysFromODataUri();

            object originalEntity = db.Find(primaryKeys);
            if (originalEntity == null)
            {
                return NotFound();
            }

            db.RemoveEntity(originalEntity);
            await db.SaveAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        public async Task<IHttpActionResult> Put(T newEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable<KeyValuePair<string, object>> primaryKeys =
                Request.ODataProperties().Path.GetKeysFromODataUri();

            db.PutEntity(primaryKeys, newEntity);
            await db.SaveAsync();
            return Updated(newEntity);
        }

        public async Task<IHttpActionResult> Patch(T deltaEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            IEnumerable<KeyValuePair<string, object>> primaryKeys =
                Request.ODataProperties().Path.GetKeysFromODataUri();

            object entity = db.Find(primaryKeys);
            db.PatchEntity(primaryKeys, entity, deltaEntity);
            
            await db.SaveAsync();
            return Updated(entity);
        }
        #endregion
    }
}
