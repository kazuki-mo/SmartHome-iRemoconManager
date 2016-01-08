using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace iRemocon_Manager_01.Controllers
{
    public class RunController : ApiController
    {
        // GET api/run
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/run/{IPアドレス("."を"_"に置き換えたもの)}/{コマンド("*"と"\r\n"を抜いたもの)}
        public string Get(string id, string comand)
        {
            string ip = id.Replace('_','.');
            comand = "*"+ comand;
            string res;

            Common common = new Common();
            try { 
                res = common.ConnectRemocon(ip, comand); 
            } catch {
                return "iRemocon(" + ip + ") に接続できませんでした。";
            }

            return res;
        }

        // POST api/run
        public void Post([FromBody]string value)
        {
        }

        // PUT api/run/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/run/5
        public void Delete(int id)
        {
        }
    }
}
