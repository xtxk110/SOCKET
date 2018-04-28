using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServerCloud
{
    class TransmitMessage
    {

        public DataObject Data { get; set; }
        public string UserCode { get; set; }
        public string Type { get; set; }
        public bool IsEnableLiveScore { get; set; }
        public string ServerAction { get; set; }

        public class DataObject
        {
            public bool IsTeam { get; set; }
            public int RowVersion { get; set; }
            public bool isSelected { get; set; }
            public Detailmodellist[] DetailModelList { get; set; }
            public string ScoreOriginType { get; set; }
            public int CurrentIndex { get; set; }
            public int TableNo { get; set; }
            public bool Exchange { get; set; }
            public int RowState { get; set; }
        }

        public class Detailmodellist
        {
            public string UserHeadUrl1 { get; set; }
            public string TeamId { get; set; }
            public int TeamScore { get; set; }
            public string TeamName { get; set; }
            public string UserName1 { get; set; }
            public int BigScore { get; set; }
            public int SmallScore { get; set; }
            public string NationalFlagUrl { get; set; }
            public string UserName2 { get; set; }
        }

    }
}
