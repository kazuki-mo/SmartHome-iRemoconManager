using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace iRemocon_Manager_01.Controllers {
    public class StartController : ApiController {
        // GET api/start
        public IEnumerable<string> Get() {
            return new string[] { "value1", "value2" };
        }

        // GET api/start/5
        public string Get(int id) {

            //サーバーのIPアドレス（または、ホスト名）とポート番号
            string ipOrHost = "";

            if (id == 1) {
                ipOrHost = "192.168.10.80";
            } else if (id == 2) {
                ipOrHost = "192.168.10.81";
            } else if (id == 3) {
                ipOrHost = "192.168.10.69";
            } else {
                return "You can use [api/start/1-3]";
            }

            string sendMsg = "*is;777\r\n";
            int port = 51013;

            System.Net.Sockets.TcpClient tcp =
                new System.Net.Sockets.TcpClient(ipOrHost, port);
            System.Net.Sockets.NetworkStream ns = tcp.GetStream();
            ns.ReadTimeout = 10000;
            ns.WriteTimeout = 10000;
            System.Text.Encoding enc = System.Text.Encoding.UTF8;

            byte[] sendBytes = enc.GetBytes(sendMsg);
            ns.Write(sendBytes, 0, sendBytes.Length);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] resBytes = new byte[256];
            int resSize = 0;
            do {
                resSize = ns.Read(resBytes, 0, resBytes.Length);
                if (resSize == 0) {
                    break;
                }
                ms.Write(resBytes, 0, resSize);
            } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');

            string resMsg1 = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            ms.Close();

            System.Threading.Thread.Sleep(5000);

            sendMsg = "*is;666\r\n";
            sendBytes = enc.GetBytes(sendMsg);
            ns.Write(sendBytes, 0, sendBytes.Length);

            ms = new System.IO.MemoryStream();
            resBytes = new byte[256];
            resSize = 0;
            do {
                resSize = ns.Read(resBytes, 0, resBytes.Length);
                if (resSize == 0) {
                    break;
                }
                ms.Write(resBytes, 0, resSize);
            } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');

            string resMsg2 = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            ms.Close();

            resMsg1 = resMsg1.TrimEnd('\n');
            resMsg1 = resMsg1.TrimEnd('\r');
            resMsg2 = resMsg2.TrimEnd('\n');
            resMsg2 = resMsg2.TrimEnd('\r');

            ns.Close();
            tcp.Close();

            return resMsg1+" , "+resMsg2;
        }

        // POST api/start
        public void Post([FromBody]string value) {
        }

        // PUT api/start/5
        public void Put(int id, [FromBody]string value) {
        }

        // DELETE api/start/5
        public void Delete(int id) {
        }
    }
}
