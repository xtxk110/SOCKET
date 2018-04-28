using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServerCloud
{
    public class SocketMessage
    {
        public string ServerAction { get; set; }
        public string UserCode { get; set; }
        public string ServerMessage { get; set; }
        /// <summary>
        /// 对阵赛事ID(移动端APP直播上传)
        /// </summary>
        public string LoopId { get; set; }
        /// <summary>
        /// 是否启用直播开关，true:启用
        /// </summary>
        public bool IsEnableLiveScore { get; set; }
        /// <summary>
        /// 对阵数据
        /// </summary>
        public DataObj Data { get; set; }

        public  class DataObj
        {
            public List<DataDetail> DetailList { get; set; }
           public class DataDetail
            {
                /// <summary>
                /// 裁判打分时数据里的对阵赛事ID
                /// </summary>
                public string LoopId { get; set; }
            }
        }
    }

}
