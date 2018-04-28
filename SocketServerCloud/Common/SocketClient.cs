using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServerCloud
{
    /// <summary>
    /// 连接远程SOCKET服务器
    /// </summary>
    public class SocketClient
    {
        //private static byte[] result = new byte[1024];
        private static Socket clientSocket=null;
        /// <summary>
        /// 连接远程SOCKET服务器
        /// </summary>
        private static void ConnectRemote()
        {
            //获取服务器IP地址
            string ipStr = ConfigurationManager.AppSettings["RemoteSocketIp"];
            string port = ConfigurationManager.AppSettings["RemoteSocketPort"];
            int port_int = 0;
            int.TryParse(port, out port_int);
            if (ipStr.Equals("") || port.Equals("") || port_int == 0)
            {
                System.Windows.Forms.MessageBox.Show("配置文件中远程SOCKET服务地址不正确！", "错误提示");
            }
                
            IPAddress ip = IPAddress.Parse(ipStr);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, port_int)); //配置服务器IP与端口
                //Console.WriteLine("连接服务器成功");
            }
            catch
            {

                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return;
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="message"></param>
        public static void SendMessage(string message)
        {
            if (clientSocket == null)
                ConnectRemote();

            //通过 clientSocket 发送数据
            byte[] sendBytes = Encoding.UTF8.GetBytes(message);
            try
            { 
                clientSocket.Send(sendBytes);
            }
            catch
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                ConnectRemote();
                clientSocket.Send(sendBytes);
            }
        }
        class SocketMessage
        {
            public string ServerAction { get; set; }
            public string UserCode { get; set; }
        }

    }
}
