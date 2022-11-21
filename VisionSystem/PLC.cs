using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace VisionSystem
{
    delegate void SerialDataReceivedEventHandler(byte[] buf, SerialDataReceivedEventArgs e);
    public class PLC
    {
        internal event SerialDataReceivedEventHandler PortDataReceived = null;
        private SerialPort m_SP = new SerialPort();
        private string m_strCOM = INIFileParam.PLCPortName;
        private int BaudRate = INIFileParam.PLCBaudRate;
        private Parity parParity = INIFileParam.PLCParity;
        private int DataBits = INIFileParam.PLCDataBits;
        private StopBits sbStopBits = INIFileParam.PLCStopBits;
        private DateTime timeM;
        public PLC()
        {
            try
            {
                if (!string.IsNullOrEmpty(m_strCOM))
                {
                    m_SP.PortName = m_strCOM;
                    m_SP.BaudRate = BaudRate;
                    m_SP.Parity = parParity;
                    m_SP.DataBits = DataBits;
                    m_SP.StopBits = sbStopBits;
                    if (!m_SP.IsOpen)
                    {
                        m_SP.Open();
                    }
                    m_SP.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(mPort_DataReceived);
                }
            }
            catch
            { }
        }

        private void mPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (m_SP.BytesToRead >= 4)
                {
                    byte[] buf = new byte[4];
                    m_SP.Read(buf, 0, 4);
                    if (PortDataReceived != null)
                    {
                        PortDataReceived(buf, e);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Close()
        {
            if (m_SP != null)
            {
                m_SP.DataReceived -= new System.IO.Ports.SerialDataReceivedEventHandler(mPort_DataReceived);
                m_SP.Close();
            }
        }

        //串口IO模块，第一路
        //open为开或者关
        public void Write(bool open)
        {
            try
            {
                TimeSpan time = DateTime.Now - timeM;
                while (time.TotalMilliseconds < 25)
                {
                    Thread.Sleep(1);
                    time = DateTime.Now - timeM;
                }
                if (m_SP.IsOpen)
                {
                    byte[] buf = new byte[8];
                    if (open)
                    {
                        buf[0] = 0x01;buf[1] = 0x05;buf[2] = 0x00;buf[3] = 0x00;
                        buf[4] = 0xFF;buf[5] = 0x00;buf[6] = 0x8C;buf[7] = 0x3A;
                    }
                    else
                    {
                        buf[0] = 0x01; buf[1] = 0x05; buf[2] = 0x00; buf[3] = 0x00;
                        buf[4] = 0x00; buf[5] = 0x00; buf[6] = 0xCD; buf[7] = 0xCA;
                    }
                    m_SP.Write(buf, 0, buf.Length);
                }
            }
            catch (Exception ex)
            { 
                
            }
        }
        //串口IO模块，第一路
        //open为开或者关
        public void Write1(bool open)
        {
            try
            {
                TimeSpan time = DateTime.Now - timeM;
                while (time.TotalMilliseconds < 25)
                {
                    Thread.Sleep(1);
                    time = DateTime.Now - timeM;
                }
                if (m_SP.IsOpen)
                {
                    byte[] buf = new byte[8];
                    if (open)
                    {
                        buf[0] = 0x01; buf[1] = 0x05; buf[2] = 0x00; buf[3] = 0x01;
                        buf[4] = 0xFF; buf[5] = 0x00; buf[6] = 0xDD; buf[7] = 0xFA;
                    }
                    else
                    {
                        buf[0] = 0x01; buf[1] = 0x05; buf[2] = 0x00; buf[3] = 0x01;
                        buf[4] = 0x00; buf[5] = 0x00; buf[6] = 0x9C; buf[7] = 0x0A;
                    }
                    m_SP.Write(buf, 0, buf.Length);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Write(byte byt, UInt16 num)
        {
            try
            {
                if (m_SP.IsOpen)
                {
                    byte[] buf = new byte[8];
                    buf[0] = 0xaa;
                    buf[7] = 0xbb;
                    buf[1] = byt;
                    buf[6] = byt;
                    Byte[] by = BitConverter.GetBytes(num);
                    buf[2] = by[1];
                    buf[3] = by[0];
                    m_SP.Write(buf, 0, buf.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string Read()
        {
            if (!m_SP.IsOpen) return "";
            char[] c = new char[m_SP.BytesToRead - 1];
            m_SP.Read(c, 0, c.Length);
            string s = c.ToString();
            return s;
        }

        public string StrCom
        {
            get
            {
                return m_strCOM;
            }
            set
            {
                if (m_strCOM != value)
                {
                    m_strCOM = value;
                    if (m_SP.IsOpen)
                    {
                        m_SP.Close();
                    }
                    m_SP.PortName = m_strCOM;
                    try
                    {
                        m_SP.Open();
                    }
                    catch (Exception exp)
                    {
                        throw new Exception("打开串口时发生错误！", exp);
                    }
                }
            }
        }
    }
}
