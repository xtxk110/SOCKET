using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;


namespace SocketServerCloud
{
    class FileLog
    {
        public static void WriteLog(string message)
        {
            string dirPath = Application.StartupPath.TrimEnd(@"\".ToCharArray()) + @"\log";
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            string logPath = dirPath+@"\log_"+DateTime.Now.ToString("yyyyMMdd")+@".txt";
            try
            {
                using (FileStream fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：" + message);
                    }
                }
            }catch(Exception e) {  }
            
        }
    }
}
