using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServerCloud
{
   public class Config
    {
        /// <summary>
        /// 云SOCKET地址
        /// </summary>
        public string SocketIpAndPort { get; set; }
        /// <summary>
        /// 局域网IIS地址
        /// </summary>
        public string IntranetHttpIpAndPort { get; set; }
        /// <summary>
        /// 局域网SOCKET地址
        /// </summary>
        public string IntranetSocketIpAndPort { get; set; }
    }
}
