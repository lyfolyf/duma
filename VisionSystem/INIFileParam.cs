using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.IO.Ports;

namespace VisionSystem
{
    static class INIFileParam
    {
        //相机参数
        public static double[] CamExposure = {0.22,0.22,0.22,0.22};
        public static double CamBrightness = 0.5;
        public static double CamContrast = 0.5;
        public static int CamLineRate = 3000;
        public static string Cam1SerialNum = "00C70338349";
        public static int CamDelay = 100000;
        public static uint CamWhiteBalanceR = 100;
        public static uint CamWhiteBalanceG = 100;
        public static uint CamWhiteBalanceB = 100;
        public static int LineDebouncingPeriod = 1000;
        public static int EncodeLineDebouncingPeriod = 1000;
        public static int CamROIWidth = 4096;
        public static int CamROIHeight = 3000;
        public static bool TriggerEnable = true;
        public static bool EncodeEnable = false;
        public static string CameraTiggerSource = "Line1";
        public static string EncodeTiggerSource = "Line2";
        public static string CameraTiggerActivation = "FallingEdge";
        public static string EncodeTiggerActivation = "FallingEdge";
        public static string CameraTiggerLineLevel = "Threshold12V";
        public static string EncodeTiggerLineLevel = "Threshold12V";
        public static string EncodeDivider = "7";
        public static string EncodeMultiplier = "8";


        //算法参数
        public static string ProductName = "";
        public static string BlockNameStr = null;
        public static string[] BlockName = null;
        public static int BlockNum = 18;
        public static int ProductType = 0;
        public static string ProductTypeStr = "";

        //图片保存
        public static bool PixtureSave = false;

        //数据库参数
        public static string IPaddr = "";
        public static string DBname = "";
        public static string DBuser = "";
        public static string DBpwd = "";





        //串口IO参数
        public static string PLCPortName = "COM1";
        public static int PLCBaudRate = 115200;
        public static Parity PLCParity = Parity.None;
        public static int PLCDataBits = 8;
        public static StopBits PLCStopBits = StopBits.One;
        private static string _Pwd = "a5eaa77414fd6e909cd85d281ac0ec50568e4fa9ee668741c987ad2c2f719917";
        public static INI clsINI = new INI(Application.StartupPath + "\\config.ini"); //配置文件
        public static INI Signal = new INI("D:\\2D与运动控制交互\\扫描交互文件.ini"); //配置文件
        public static INI ID_CODE = new INI("D:\\2D与运动控制交互\\产品ID列表.ini"); //配置文件

        public static void LoadINIInfo()
        {
            //Cam1SerialNum = clsINI.IniReadValue("Camera", "Cam1SerialNum");
            CamExposure[0] = Double.Parse(clsINI.IniReadValue("Camera", "CamExposure").Split('-')[0]);
            CamExposure[1] = Double.Parse(clsINI.IniReadValue("Camera", "CamExposure").Split('-')[1]);
            CamExposure[2] = Double.Parse(clsINI.IniReadValue("Camera", "CamExposure").Split('-')[2]);
            CamExposure[3] = Double.Parse(clsINI.IniReadValue("Camera", "CamExposure").Split('-')[3]);

            CamBrightness = Double.Parse(clsINI.IniReadValue("Camera", "CamBrightness"));
            CamContrast = Double.Parse(clsINI.IniReadValue("Camera", "CamContrast"));
            CamLineRate = Int32.Parse(clsINI.IniReadValue("Camera", "CamLineRate"));
            CamDelay = Int32.Parse(clsINI.IniReadValue("Camera", "CamDelay"));
            CamWhiteBalanceR = uint.Parse(clsINI.IniReadValue("Camera", "CamWhiteBalanceR"));
            CamWhiteBalanceG = uint.Parse(clsINI.IniReadValue("Camera", "CamWhiteBalanceG"));
            CamWhiteBalanceB = uint.Parse(clsINI.IniReadValue("Camera", "CamWhiteBalanceB"));
            LineDebouncingPeriod = Int32.Parse(clsINI.IniReadValue("Camera", "LineDebouncingPeriod"));
            EncodeLineDebouncingPeriod = Int32.Parse(clsINI.IniReadValue("Camera", "EncodeLineDebouncingPeriod"));
            CamROIWidth = Int32.Parse(clsINI.IniReadValue("Camera", "CamROIWidth"));
            CamROIHeight = Int32.Parse(clsINI.IniReadValue("Camera", "CamROIHeight"));

            CameraTiggerSource = clsINI.IniReadValue("Camera", "CameraTiggerSource");
            EncodeTiggerSource = clsINI.IniReadValue("Camera", "EncodeTiggerSource");
            CameraTiggerActivation = clsINI.IniReadValue("Camera", "CameraTiggerActivation");
            EncodeTiggerActivation = clsINI.IniReadValue("Camera", "EncodeTiggerActivation");
            CameraTiggerLineLevel = clsINI.IniReadValue("Camera", "CameraTiggerLineLevel");
            EncodeTiggerLineLevel = clsINI.IniReadValue("Camera", "EncodeTiggerLineLevel");
            TriggerEnable = bool.Parse(clsINI.IniReadValue("Camera", "TriggerEnable"));
            EncodeEnable = bool.Parse(clsINI.IniReadValue("Camera", "EncodeEnable"));

            EncodeDivider = clsINI.IniReadValue("Camera", "EncodeDivider");
            EncodeMultiplier = clsINI.IniReadValue("Camera", "EncodeMultiplier");

            ProductName = clsINI.IniReadValue("Product", "ProductName");
            //BlockName = clsINI.IniReadValue("Job", ProductName).Split('-');
            PixtureSave = bool.Parse(clsINI.IniReadValue("SysParam", "PixtureSave"));

            IPaddr = clsINI.IniReadValue("DataBase", "IPaddr");
            DBname = clsINI.IniReadValue("DataBase", "DBname");
            DBuser = clsINI.IniReadValue("DataBase", "DBuser");
            DBpwd = clsINI.IniReadValue("DataBase", "DBpwd");
        }

        public static void SaveProductInfo()
        {
            clsINI.IniWriteValue("Product", "ProductName", ProductName);
        }
        public static void SaveCameraInfo()
        {
            clsINI.IniWriteValue("Camera", "CamExposure", CamExposure[0].ToString("0.00") + "-" + CamExposure[1].ToString("0.00") + "-" + CamExposure[2].ToString("0.00") + "-" + CamExposure[3].ToString("0.00"));
            clsINI.IniWriteValue("Camera", "CamLineRate", CamLineRate.ToString());
            clsINI.IniWriteValue("Camera", "CamDelay", CamDelay.ToString());
            clsINI.IniWriteValue("Camera", "CamWhiteBalanceR", CamWhiteBalanceR.ToString());
            clsINI.IniWriteValue("Camera", "CamWhiteBalanceG", CamWhiteBalanceG.ToString());
            clsINI.IniWriteValue("Camera", "CamWhiteBalanceB", CamWhiteBalanceB.ToString());
            clsINI.IniWriteValue("Camera", "LineDebouncingPeriod", LineDebouncingPeriod.ToString());
            clsINI.IniWriteValue("Camera", "EncodeLineDebouncingPeriod", EncodeLineDebouncingPeriod.ToString());
            clsINI.IniWriteValue("Camera", "CamROIWidth", CamROIWidth.ToString());
            clsINI.IniWriteValue("Camera", "CamROIHeight", CamROIHeight.ToString());

            clsINI.IniWriteValue("Camera", "TriggerEnable", TriggerEnable.ToString());
            clsINI.IniWriteValue("Camera", "EncodeEnable", EncodeEnable.ToString());
            clsINI.IniWriteValue("Camera", "CameraTiggerSource", CameraTiggerSource);
            clsINI.IniWriteValue("Camera", "EncodeTiggerSource", EncodeTiggerSource);
            clsINI.IniWriteValue("Camera", "CameraTiggerActivation", CameraTiggerActivation);
            clsINI.IniWriteValue("Camera", "EncodeTiggerActivation", EncodeTiggerActivation);
            clsINI.IniWriteValue("Camera", "CameraTiggerLineLevel", CameraTiggerLineLevel);
            clsINI.IniWriteValue("Camera", "EncodeTiggerLineLevel", EncodeTiggerLineLevel);
            clsINI.IniWriteValue("Camera", "EncodeDivider", EncodeDivider);
            clsINI.IniWriteValue("Camera", "EncodeMultiplier", EncodeMultiplier);
        }

        public static string GetCode(string code)
        {
            //5a744de0c8d8a0c06ef9e67197dcf197
            string byte2String = string.Empty;
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(System.Text.Encoding.UTF32.GetBytes(code));
            int length = result.Length;
            for (int j = 0; j < length; j++)
            {
                byte2String += result[j].ToString("x");
            }
            if (byte2String.Length > 32)
                byte2String = byte2String.Substring(0, 32);
            if (byte2String.Length < 32)
                byte2String = byte2String.PadRight(32, '0');
            return byte2String;
        }

        public static bool CheckCode(string checkCode)
        {
            bool result = false;
            if (checkCode == null)
                return result;
            if (_Pwd == null)
                return result;

            result = true;
            if (checkCode != _Pwd)
            {
                result = false;
            }
            return result;

        }

    }
}
