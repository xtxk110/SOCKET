using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

    /// <summary>
    /// SOCKET接收包分解处理(解决粘包\分包问题)
    /// </summary>
    public class SocketPacketAnalyze
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">返回完整数据包</param>
        /// <param name="socketObj">包含SOCKET的对象</param>
        public delegate void AnalyzeCallback<T>(T data,SocketObject socketObj);//一条完整数据包委托
        /// <summary>
        /// 返回完整数据包字符串处理事件
        /// </summary>
        public event AnalyzeCallback<string> AnalyzeStrHandler;//事件
        /// <summary>
        /// 返回完整数据包字节数组处理事件
        /// </summary>
        public event AnalyzeCallback<byte[]> AnalyzeByteHandler;//事件

        /// <summary>
        /// 带分割符数据包解析,并触发相应事件
        /// </summary>
        /// <param name="socketObj">包含SOCKET的对象</param>
        /// <param name="packet">收到的数据包</param>
        /// <param name="partByte">数据包不完整时存储的对象</param>
        /// <param name="separator">统一定义的分割符,假如为空默认为*,最好是单个字符串</param>
        /// <param name="error">错误输出</param>
        /// <param name="encode">字符编码,默认为UTF8</param>
        /// <param name="isByte">事件返回的完整数据包默认为字符串;反之TRUE,字节数组</param>
        public void SeparatorAnalyze(SocketObject socketObj, byte[] packet, List<byte> partByte, string separator, out string error, Encoding encode = null, bool isByte = false)
        {
            try
            {
                if (encode == null)
                    encode = Encoding.UTF8;
                if (string.IsNullOrEmpty(separator))
                    separator = "*#";
                int rnFixLength = separator.Length;
                if (partByte.Count > 0)//假如剩余包有数据,则加入接收包之前
                {
                    partByte.AddRange(packet);
                    packet = partByte.ToArray();
                    partByte.Clear();
                }
                string rawMsg = encode.GetString(packet);//得到数据包字符串 

                int sepIndex = -1;

                for(int i = 0; i < rawMsg.Length;)
                {
                    sepIndex = rawMsg.Substring(i).IndexOf(separator);
                    if (sepIndex > -1)
                    {
                        string full = rawMsg.Substring(i, sepIndex);
                        //找到了消息结束符，触发完整数据包事件
                        if (isByte)
                        {
                            if (this.AnalyzeByteHandler != null)
                                this.AnalyzeByteHandler(encode.GetBytes(full), socketObj);
                        }
                        else
                        {
                            if (this.AnalyzeStrHandler != null)
                                this.AnalyzeStrHandler(full, socketObj);
                        }
                        i = i+sepIndex + rnFixLength;
                    }
                    else
                    {
                        //partSb.Append(rawMsg.Substring(i, rawMsg.Length - i));
                        partByte.AddRange(encode.GetBytes(rawMsg.Substring(i, rawMsg.Length - i)));
                        break;
                    }
                }
                
                error = string.Empty;
            }
            catch (Exception e)
            {
                error = e.TargetSite + "->" + e.Message;
            }
        }

        /// <summary>
        /// 数据包前固定几字节表示实际数据长度的数据包解析.并触发相应事件
        /// </summary>
        /// <param name="socketObj">包含SOCKET的对象</param>
        /// <param name="packet">收到的数据包</param>
        /// <param name="partByte">数据包不完整时存储的对象</param>
        /// <param name="size">包头表示实际数据长度所需字节长度(固定值)</param>
        /// <param name="error">解析包时可能的错误</param>
        /// <param name="encode">字符编码,默认为UTF8</param>
        /// <param name="isByte">事件返回的完整数据包默认为字符串;反之TRUE,字节数组</param>
        public void HeadLenAnalyze1(SocketObject socketObj, byte[] packet, List<byte> partByte, int size, out string error, Encoding encode = null, bool isByte = false)
        {
            if (size <= 0 || size != 2 || size != 4)//包头固定字节数小于0,直接返回
            {
                error = "表示实际数据长度占用的字节数不能小于1,一般为2字节,最多4字节,根据实际协议制定";
                return;
            }
            if (encode == null)
                encode = Encoding.UTF8;
            try
            {
                if (partByte.Count > 0)//剩余数据拼接在接收数据包前面
                {
                    partByte.AddRange(packet);
                    packet = partByte.ToArray();
                    partByte.Clear();
                }

                int offset = 0;//接收数据包里的偏移量
                int headDataLen = 0;//实际数据的长度

                while (offset < packet.Length)
                {
                    if (packet.Length-offset < size)//剩余数据大小小于包头字节数
                    {
                        byte[] temp = new byte[packet.Length-offset];
                        Buffer.BlockCopy(packet, offset, temp, 0, packet.Length-offset);
                        partByte.AddRange(temp);
                        break;
                    }

                    switch (size)//获取包头里实际数据的长度
                    {
                        case 2:
                            headDataLen = BitConverter.ToInt16(packet.Skip(offset).Take(2).ToArray(), 0);

                            break;
                        case 4:
                            headDataLen = BitConverter.ToInt32(packet.Skip(offset).Take(4).ToArray(), 0);
                            break;
                    }
                    offset = offset + size;
                    if (packet.Length- offset >= headDataLen)//数据包部长大于实际数据长度
                    {
                        byte[] temp = new byte[headDataLen];
                        Buffer.BlockCopy(packet, offset, temp, 0, headDataLen);
                        
                        offset = offset + headDataLen;//偏移量移到下一个数据段首位
                        //拼成完整数据包时触发事件
                        if (isByte)
                        {
                            if (this.AnalyzeByteHandler != null)
                                this.AnalyzeByteHandler(temp, socketObj);
                        }
                        else
                        {
                            if (this.AnalyzeStrHandler != null)
                                this.AnalyzeStrHandler(encode.GetString(temp), socketObj);
                        }

                    }
                    else
                    {
                        byte[] temp = new byte[packet.Length - offset+size];
                        Buffer.BlockCopy(packet, offset-size, temp, 0, packet.Length - offset+size);
                        partByte.AddRange(temp);
                        break;
                    }

                }
                error = string.Empty;
            }
            catch (Exception e) { error = e.TargetSite + "->" + e.Message; }
        }

        public  List<string> SeparatorAnalyze( byte[] packet, List<byte> partByte, string separator, Encoding encode = null)
        {
            List<string> list = new List<string>();
            try
            {
                if (encode == null)
                    encode = Encoding.UTF8;
                if (string.IsNullOrEmpty(separator))
                    separator = "*";
                int rnFixLength = separator.Length;
                if (partByte.Count > 0)//假如剩余包有数据,则加入接收包之前
                {
                    partByte.AddRange(packet);
                    packet = partByte.ToArray();
                    partByte.Clear();
                }
                string rawMsg = encode.GetString(packet);//得到数据包字符串 

                int sepIndex = -1;

                for (int i = 0; i < rawMsg.Length;)
                {
                    sepIndex = rawMsg.Substring(i).IndexOf(separator);
                    if (sepIndex > -1)
                    {
                        list.Add(rawMsg.Substring(i, sepIndex));//完整数据包
                        i = i+sepIndex + rnFixLength;
                    }
                    else
                    {
                        //partSb.Append(rawMsg.Substring(i, rawMsg.Length - i));
                        partByte.AddRange(encode.GetBytes(rawMsg.Substring(i, rawMsg.Length - i)));
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                
            }
            return list;
        }

    }

