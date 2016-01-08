using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace iRemocon_Manager_01.Controllers
{
    public class TestController : ApiController
    {
        TestDB_01Entities db = new TestDB_01Entities();

        // GET api/test
        public string Get()
        {
            return "test";
        }

        // GET api/test/5
        public string Get(string id)
        {
            iRemocon insert = new iRemocon();
            insert.IPAddress = "999.999.999.999";
            insert.Detail = "Test";

            RegistrationCode insert1 = new RegistrationCode();
            insert1.RegistrationCode1 = int.Parse(id);
            insert1.Detail = "Test";

            insert.RegistrationCodes.Add(insert1);
            db.iRemocons.Add(insert);

            db.SaveChanges();


            return "Success!";
        }

        // POST api/test
        public string Post([FromBody]string value)
        {
            return "Test";
        }

        // PUT api/test/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/test/5
        public void Delete(int id)
        {
        }
    }
}
