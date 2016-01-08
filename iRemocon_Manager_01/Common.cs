using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRemocon_Manager_01 {
    public class Common {

        public string ConnectRemocon(string ip, string send){
            string res = ip;

            string ipOrHost = ip;
            string sendMsg = send;
            int port = 51013;

            System.Net.Sockets.TcpClient tcp =
                new System.Net.Sockets.TcpClient(ipOrHost, port);
            System.Net.Sockets.NetworkStream ns = tcp.GetStream();
            ns.ReadTimeout = 10000;
            ns.WriteTimeout = 10000;
            System.Text.Encoding enc = System.Text.Encoding.UTF8;

            byte[] sendBytes = enc.GetBytes(sendMsg+"\r\n");
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

            resMsg1 = resMsg1.TrimEnd('\n');
            resMsg1 = resMsg1.TrimEnd('\r');

            ns.Close();
            tcp.Close();

            return resMsg1;
        }

    }
}