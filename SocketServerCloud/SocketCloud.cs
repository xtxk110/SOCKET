using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

namespace SocketServerCloud
{
    public partial class SocketCloud : Form
    {
        private static object obj = new object();//同步操作标志
        private Socket serverSocket = null;
        //private ConcurrentBag<SocketObject> socketObjList = new ConcurrentBag<SocketObject>();//存储客户端SOCKET对象
        private List<SocketObject> socketObjList = new List<SocketObject>();//存储客户端SOCKET对象
        private ConcurrentDictionary<string, string> loopJudgeDic = new ConcurrentDictionary<string, string>();//移动直播端所选对阵对应的裁判
        private bool flag = true;//循环检测
        private static int logCount = 0;//日志条数计数器
        private static int logMaxCount = 10000;//默认最大日志条数(假如配置文件读取错误)
        private static int ListenBacklog = 500;//监听队列最大数量
        private static bool IsShowLog = false;//是否在主界面显示日志,
        private Config socketConf =new Config();//socket配置对象
        public SocketCloud()
        {
            InitializeComponent();
        }
        #region  设置SOCKET下拉框数据
        private delegate void DelegateData();
        /// <summary>
        /// 设置下拉列表数据源
        /// </summary>
        private void SetDataSource()
        {
            if (cb_socket.InvokeRequired)
            {
                new Thread(() => { BeginInvoke(new DelegateData(SetDataSourceDele)); }).Start();
            }
            else
                SetDataSourceDele();

        }
        /// <summary>
        /// 设置下拉列表数据源回调
        /// </summary>
        private void SetDataSourceDele()
        {
            cb_socket.DataSource = socketObjList.ToList();
            cb_socket.DisplayMember = "RemoteEndpoint";
            lb_count.Text = socketObjList.Count.ToString();
        }
        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
            ShowSocketConf();//显示SOCKET配置
            try
            {
                int.TryParse(ConfigurationManager.AppSettings["LOG_MAX_COUNT"], out logMaxCount);
            }catch(Exception e1) { }
            
        }
        /// <summary>
        /// 开启Socket监听
        /// </summary>
        private void StartSocket()
        {
            //SocketListen(serverSocket);
            InitSocket();
            flag = true;
            //检测客户端SOCKET连接状态
            Task.Factory.StartNew(() =>
            {
                while (flag)
                {
                    System.Threading.Thread.Sleep(5000);
                    List<SocketObject> tempList = socketObjList.ToList();
                    foreach (var item in tempList)
                    {
                        if (!IsSocketConnected(item.ClientSocket))
                        {
                            //SocketObject obj = item;
                            //socketObjList.TryTake(out obj);
                            ListOperator(item, ListAction.Delete);
                            CloseSocket(item.ClientSocket);
                        }
                    }
                    SetDataSource();
                }
            });
        }
        /// <summary>
        /// 关闭所有SOCKET连接
        /// </summary>
        private void EndSocket()
        {
            flag = false;
            try
            {
                foreach (var item in socketObjList)
                {
                    CloseSocket(item.ClientSocket);
                    
                }
            }
            catch (Exception e)
            {

            }
            CloseSocket(serverSocket);
            try
            {
                serverSocket.Dispose();
                serverSocket = null;
            }catch(Exception e)
            {

            }
        }
        /// <summary>
        /// 初始化SOCKET服务
        /// </summary>
        private void InitSocket()
        {
            string endpointStr = string.Empty;
            try
            {
                //socketConf = DBHelper.GetSocketConfig();//获取SOCKET配置
                endpointStr = socketConf.SocketIpAndPort;//DBHelper.GetColudSocket();//获取云服务SOCKET监听的地址
            }catch(Exception e)
            {
                DoLog( e.Source + "->" + e.TargetSite + "->" + e.Message);
                return;
            }
            

            if (string.IsNullOrEmpty(endpointStr))
            {
                DoLog("获取的SOCKET地址为空");
                return;
            }
            else if (!endpointStr.Contains(":"))
            {
                DoLog("SOCKET地址不正确");
                return;
            }

            string[] temp = endpointStr.Split(':');
            string servIp = temp[0];
            int servPort = 0;
            int.TryParse(temp[1], out servPort);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(servIp), servPort);
                serverSocket.Bind(endpoint);
                SocketListen(serverSocket);
                StartHertCheck(serverSocket);//开启心跳检测
                DoLog(endpoint.ToString() + "监听中......");
                this.Text = "云SOCKET【" + serverSocket.LocalEndPoint.ToString() + "】";
                AcceptSocket(serverSocket);//接入客户端
            }
            catch (Exception e)
            {
                serverSocket = null;
                DoLog(e.Message);
                return;
            }

        }
        /// <summary>
        /// SOCKET监听
        /// </summary>
        /// <param name="server"></param>
        private void SocketListen(Socket server)
        {
            server.Listen(ListenBacklog);
        }
        /// <summary>
        /// 主动关闭SOCKET连接
        /// </summary>
        /// <param name="socket"></param>
        private void CloseSocket(Socket socket)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception e)
            {
                //DoLog("主动关闭->"+ e.Source + "->" + e.TargetSite + "->" + e.Message);
            }
        }
        /// <summary>
        /// 接入客户端SOCKET
        /// </summary>
        /// <param name="server"></param>
        private void AcceptSocket(Socket server)
        {
            try
            {
                IAsyncResult iar = serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), server);
            }
            catch (Exception e)
            {
                CloseSocket(serverSocket);
            }

        }
        /// <summary>
        /// 接入客户端回调
        /// </summary>
        /// <param name="iar"></param>
        private void AcceptCallback(IAsyncResult iar)
        {
            SocketObject obj = new SocketObject();
            Socket server = iar.AsyncState as Socket;
            Socket client = null;
            if (iar.IsCompleted)
            {
                try
                {
                    client = server.EndAccept(iar);
                    StartHertCheck(client);//开启心跳检测
                    obj.ClientSocket = client;
                    obj.BufferSize = 300000;
                    obj.Buffer = new byte[obj.BufferSize];
                    obj.RemoteEndpoint = client.RemoteEndPoint;

                    client.BeginReceive(obj.Buffer, 0, obj.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), obj);//接收消息
                }
                catch (Exception e)
                {
                    DoLog(e.Source + "->" + e.TargetSite + "->" + e.Message, true);
                    if (client != null)
                        CloseSocket(client);
                }

                AcceptSocket(server);
            }

        }
        /// <summary>
        /// 接收消息回调
        /// </summary>
        /// <param name="iar"></param>
        private void ReceiveCallback(IAsyncResult iar)
        {
            SocketObject obj = iar.AsyncState as SocketObject;
            Socket client = obj.ClientSocket;
            int dataLen = 0;
            if (iar.IsCompleted)
            {
                try
                {
                    dataLen = client.EndReceive(iar);
                }
                catch (Exception e)
                {
                    CloseSocket(obj.ClientSocket);
                    DoLog(e.Source + "->" + e.TargetSite + "->" + e.Message, true);
                }
                string dataStr = string.Empty;
                if (dataLen > 0)
                {
                    try
                    {
                        dataStr = Encoding.UTF8.GetString(obj.Buffer, 0, dataLen).Trim();
                        dataStr = Regex.Replace(dataStr,"\\s","");
                        SocketMessage messObj = JsonConvert.DeserializeObject<SocketMessage>(dataStr);
                        AnalyzeMessage(dataStr, messObj, obj);
                    }
                    catch (Exception e)
                    {
                        DoLog("数据接收处错误:"+e.Source + "->" + e.TargetSite + "->" + e.Message+"->"+dataStr, true);
                    }
                }
            }

            try
            {
                client.BeginReceive(obj.Buffer, 0, obj.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), obj);
            }
            catch (Exception e)
            {
                DoLog(e.Source + "->" + e.TargetSite + "->" + e.Message, true);
                CloseSocket(client);
            }

        }
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messObj"></param>
        /// <param name="obj"></param>
        private void AnalyzeMessage(string message, SocketMessage messObj, SocketObject obj)
        {
            switch (messObj.ServerAction)
            {
                case "Login"://移动端直播登录
                    LoginOperation(messObj, obj);
                    break;
                case "Talk"://直播比分数据传入
                    TalkOperation(message, messObj, obj);
                    break;
                case "HeartBeat"://心跳维持
                    break;
            }
        }
        /// <summary>
        /// 登录具体操作
        /// </summary>
        /// <param name="messObj"></param>
        /// <param name="obj"></param>
        private void LoginOperation(SocketMessage messObj, SocketObject obj)
        {
            //查询所选对阵赛事对应的裁判登录账号
            if (!loopJudgeDic.ContainsKey(messObj.LoopId))
            {
                string judgeCode = DBHelper.GetLoopJudge(messObj.LoopId);
                loopJudgeDic.TryAdd(messObj.LoopId, judgeCode);
            }

            SocketObject item = null;
            item = socketObjList.ToList().Where(e => e.RemoteEndpoint.ToString().Equals(obj.RemoteEndpoint.ToString())).FirstOrDefault();
            if (item == null)
            {
                obj.UserCode = messObj.UserCode;
                obj.LoopID = messObj.LoopId;
                obj.JudgeCode = loopJudgeDic[messObj.LoopId];
                socketObjList.Add(obj);
            }
            else
            {
                item.UserCode = messObj.UserCode;
                item.LoopID = messObj.LoopId;
                item.JudgeCode= loopJudgeDic[messObj.LoopId];
            }
            DoLog(obj.UserCode + "【" + obj.RemoteEndpoint + "】连接成功,赛事对阵ID:" + obj.LoopID);
            

            //登录成功返回ServerMessage为"1"
            //SocketMessage sm = new SocketMessage { ServerMessage = "1" };
            //string smJson = JsonConvert.SerializeObject(sm);
            //try
            //{
            //    obj.ClientSocket.Send(Encoding.UTF8.GetBytes(smJson));
            //}
            //catch(Exception e)
            //{
            //    DoLog(e.Message);
            //    CloseSocket(obj.ClientSocket);
            //    socketObjList.TryTake(out obj);
            //}
        }
        /// <summary>
        /// 比分数据具体操作
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messObj"></param>
        /// <param name="obj"></param>
        private void TalkOperation(string message, SocketMessage messObj, SocketObject obj)
        {
            if(IsShowLog)
                DoLog("当前客户数:" + socketObjList.Count + "->接收比分数据：" + message);
            string trasmitJsonStr = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<TransmitMessage>(message));
            socketObjList.ToList().AsParallel().ForAll((item) =>
            {
                string messLoopId = string.Empty;
                try
                {
                    messLoopId = messObj.Data.DetailList[0].LoopId;
                }
                catch(Exception e)
                {
                    DoLog("获取对阵ID->"+e.Source + "->" + e.TargetSite + "->" + e.Message + "->data:" + message, true);
                }
                if (item.LoopID == messLoopId || item.JudgeCode==messObj.UserCode)
                {
                    string message1 = trasmitJsonStr;
                    byte[] dataBytes = Encoding.UTF8.GetBytes(message1);
                    item.SendMessage = message1;
                    item.MessObj = messObj;
                    try
                    {
                        if (IsSocketConnected(item.ClientSocket))//发送前检测SOCKET通断
                        {
                            if (IsShowLog)
                                DoLog("开始向"+ item.ClientSocket.RemoteEndPoint + "【" + item.UserCode + "】发送数据");
                            item.ClientSocket.BeginSend(dataBytes, 0, dataBytes.Length, SocketFlags.None, new AsyncCallback(SendCallback), item);
                        }
                        else
                        {
                            //socketObjList.TryTake(out item);
                            ListOperator(item, ListAction.Delete);
                            CloseSocket(item.ClientSocket);
                        }
                    }
                    catch (Exception e)
                    {
                        DoLog(item.ClientSocket.RemoteEndPoint+"【"+item.UserCode+"】SOCKET链接未活动");
                    }
                }
            });
        }
        /// <summary>
        /// 发送回调函数
        /// </summary>
        /// <param name="iar"></param>
        private void SendCallback(IAsyncResult iar)
        {
            SocketObject obj = null;
            try
            {
                obj = iar.AsyncState as SocketObject;
                if (iar.IsCompleted)
                {
                    int sendLen = obj.ClientSocket.EndSend(iar);
                    string log = string.Format("已成功转发比分数据：字节数:{3}->{0}->{1}:{2}", obj.MessObj.UserCode, obj.UserCode, obj.SendMessage, sendLen);
                    if (IsShowLog)
                        DoLog(log);
                }
            }
            catch (Exception e)
            {
                //socketObjList.TryTake(out obj);
                ListOperator(obj, ListAction.Delete);
                if (obj != null)
                    CloseSocket(obj.ClientSocket);
                DoLog("发送失败->"+e.Source + "->" + e.TargetSite + "->" + e.Message, true);
            }
        }
        #region  日志
        private delegate void DelegateDoLog(string mess);
        private void WriteLog(string mess)
        {
            if (logCount > logMaxCount)
            {
                logCount = 0;
                this.txt_message.Clear();
            }

            logCount += 1;
            string log = string.Format("{0}：{1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), mess);
            this.txt_message.AppendText(log);
            this.txt_message.ScrollToCaret();
        }
        /// <summary>
        /// 日志写入主界面日志框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isFileLog"></param>
        private void DoLog(string message, bool isFileLog = false)
        {
            if (isFileLog)
            {
                FileLog.WriteLog(message);
                return;
            }

            if (txt_message.InvokeRequired)
            {

                //txt_message.Invoke(new DelegateDoLog(WriteLog), message);
                new Thread(() => { txt_message.BeginInvoke(new DelegateDoLog(WriteLog), message); }).Start();
            }
            else
            {
                WriteLog(message);
            }
        }
        #endregion
        #region SOCKET通断检测
        /// <summary>
        /// SOCKET通断检测,true为连接上
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private bool IsSocketConnected(Socket client)
        {
            bool result = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpCons = ipProperties.GetActiveTcpConnections();
            tcpCons.AsParallel().ForAll((item) => {
                try
                {
                    if (item.LocalEndPoint.Equals(client.LocalEndPoint) && item.RemoteEndPoint.Equals(client.RemoteEndPoint))
                    {
                        if (item.State == TcpState.Established)
                            result = true;
                    }
                }
                catch (Exception e) { }
            });
            return result;
        }
        /// <summary>
        /// 获取Keepalive心跳包数据
        /// </summary>
        /// <returns></returns>
        private byte[] GetKeepAliveData()
        {
            uint dummy = 0;
            byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)3000).CopyTo(inOptionValues, Marshal.SizeOf(dummy));//keep-alive间隔  
            BitConverter.GetBytes((uint)500).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);// 尝试间隔  
            return inOptionValues;
        }
        /// <summary>
        /// 开启心跳检测功能
        /// </summary>
        /// <param name="socket"></param>
        private void StartHertCheck(Socket socket)
        {
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            socket.IOControl(IOControlCode.KeepAliveValues, GetKeepAliveData(), null);
        }
        #endregion
        #region LIST集合同步操作
        /// <summary>
        /// SOCKET集合同步操作
        /// </summary>
        /// <param name="item"></param>
        /// <param name="action"></param>
        private void ListOperator(SocketObject item, ListAction action)
        {
            lock (obj)
            {
                switch (action)
                {
                    case ListAction.Add:
                        socketObjList.Add(item);
                        break;
                    case ListAction.Delete:
                        socketObjList.Remove(item);
                        break;
                }
            }
        }
        /// <summary>
        /// SOCKET未连接,更改IsOnline为false
        /// </summary>
        /// <param name="item"></param>
        #endregion
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr= MessageBox.Show("确定要关闭吗?", "关闭提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
                EndSocket();
            else
                e.Cancel = true;
        }

        private void btn_log_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (IsShowLog)
            {
                btn.Text = "显示日志";
                IsShowLog = false;
            }
            else
            {
                btn.Text = "关闭日志";
                IsShowLog = true;
            }
        }
        private void btn_listen_Click(object sender, EventArgs e)
        {
            StartSocket();
            if (serverSocket != null) {
                btn_listen.Enabled = false;
                btn_close.Enabled = true;
            }
                
        }
        private void btn_save_Click(object sender, EventArgs e)
        {
            if (socketConf.IntranetHttpIpAndPort == txt_inner_iis.Text.Trim() && socketConf.IntranetSocketIpAndPort == txt_inner_socket.Text.Trim() && socketConf.SocketIpAndPort == txt_cloud_socket.Text.Trim())
            {
                MessageBox.Show("内容未修改,操作取消", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                socketConf.IntranetHttpIpAndPort = txt_inner_iis.Text.Trim();
                socketConf.IntranetSocketIpAndPort = txt_inner_socket.Text.Trim();
                socketConf.SocketIpAndPort = txt_cloud_socket.Text.Trim();
            }
            DialogResult dr = MessageBox.Show("确定要保存配置吗?", "保存提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                try
                {
                    int result = DBHelper.SaveSocketConfig(txt_cloud_socket.Text.Trim(), txt_inner_socket.Text.Trim(), txt_inner_iis.Text.Trim());
                    if (result > 0)
                        MessageBox.Show("保存成功", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception e2)
                {
                    DoLog(e2.TargetSite + "->" + e2.Message, true);
                }
            }
        }
        /// <summary>
        /// 显示SOCKET配置地址
        /// </summary>
        private void ShowSocketConf()
        {
            try
            {
                socketConf = DBHelper.GetSocketConfig();
            }
            catch (Exception e)
            {
                DoLog(e.Source + "->" + e.TargetSite + "->" + e.Message);
                return;
            }
            this.txt_cloud_socket.Text = socketConf.SocketIpAndPort;
            this.txt_inner_socket.Text = socketConf.IntranetSocketIpAndPort;
            this.txt_inner_iis.Text = socketConf.IntranetHttpIpAndPort;
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            EndSocket();
            if (serverSocket == null)
            {
                btn_listen.Enabled = true;
                btn_close.Enabled = false;
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            this.txt_message.Clear();
        }
    }
}
