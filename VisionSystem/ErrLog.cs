using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace VisionSystem
{
    static class ErrLog
    {
        public static void WriteLog(string errMsg)
        {
            string m_logPath = string.Format("{0}\\RunLog", Application.StartupPath);
            if (!Directory.Exists(m_logPath))
                Directory.CreateDirectory(m_logPath);
            string log_Path = string.Format("{0}\\{1}.log", m_logPath, DateTime.Now.ToString("yyyy-MM-dd"));
            using (StreamWriter streamWriter = new StreamWriter(log_Path, true, Encoding.Default))
            {
                streamWriter.WriteLine(string.Format("{0}->{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), errMsg));
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        public static void WriteLogEx(string errMsg)
        {
            string m_logPath = string.Format("{0}\\ErrorLog", Application.StartupPath);
            if (!Directory.Exists(m_logPath))
                Directory.CreateDirectory(m_logPath);
            string log_Path = string.Format("{0}\\{1}.log", m_logPath, DateTime.Now.ToString("yyyy-MM-dd"));
            using (StreamWriter streamWriter = new StreamWriter(log_Path, true, Encoding.Default))
            {
                streamWriter.WriteLine(string.Format("{0}->{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), errMsg));
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        public static void WriteData(string  filename,string errMsg)
        {
            string log_Path = string.Format("{0}{1}.csv", AppDomain.CurrentDomain.BaseDirectory+"\\DataFiles\\",filename);
            using (StreamWriter streamWriter = new StreamWriter(log_Path, true, Encoding.Default))
            {
                streamWriter.WriteLine(string.Format(errMsg));
                streamWriter.Flush();
                streamWriter.Close();
            }
        }
    }
}
