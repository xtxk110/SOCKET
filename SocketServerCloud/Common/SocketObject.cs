using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace SocketServerCloud
{
  public  class SocketObject
    {
        public string UserCode { get; set; }
        /// <summary>
        /// 对阵赛事ID(移动端APP直播上传)
        /// </summary>
        public string LoopID { get; set; }
        public Socket ClientSocket { get; set; }
        public EndPoint RemoteEndpoint { get; set; }
        public int BufferSize { get; set; }
        public byte[] Buffer { get; set; }
        public string SendMessage { get; set; }
        public SocketMessage MessObj { get; set; }
        /// <summary>
        /// LoopId对应的裁判登录账号
        /// </summary>
        public string JudgeCode { get; set; }
    }
}
