using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace iRemocon_Manager_01.Controllers
{
    public class RemoconDeleteController : ApiController
    {
        TestDB_01Entities db = new TestDB_01Entities();

        // GET api/remocondelete
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/remocondelete/5
        public string Get(string id)
        {
            id = id.Replace('_', '.');

            var target = db.iRemocons.Where(p => p.IPAddress.Equals(id)).Single();
            var targetcodes = target.RegistrationCodes;

            while (!(targetcodes.FirstOrDefault() == null)) {
                db.RegistrationCodes.Remove(targetcodes.FirstOrDefault());
                db.SaveChanges();
            }
            db.iRemocons.Remove(target);
            db.SaveChanges();

            return id;
        }

        // POST api/remocondelete
        public void Post([FromBody]string value)
        {
        }

        // PUT api/remocondelete/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/remocondelete/5
        public void Delete(int id)
        {
        }
    }
}
