using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro;
using System.IO;
using System.Threading;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ImageFile;

using Cognex.VisionPro.ID;

namespace VisionSystem
{
    public partial class Form1 : Form
    {
        //软件运行状态
        private bool RunStatus = false;

        //相机对象列表
        private AcqFIFOManager mAcqFIFOManager = null;

        private CogToolBlock mCogToolBlock1 = null;


        private CogIDTool mCogIDTool1 = null;
        private CogIDTool mCogIDTool2 = null;

        private Thread ResultHandle;
        private bool ResultThreadRun = false;
        private DateTime RunStart, RunStop;
        private bool LiveVideo = false;

        private string A_ID1_Code = string.Empty, A_ID2_Code = string.Empty;
        private int A_ID1 = -1, A_ID2 = -1;
        bool A_read = true;


        double exposeTime = 0;
        double gain = 0;
        INI ini = null;
        public Form1()
        {
            InitializeComponent();
            RunStatus = false;
            button5.BackColor = Color.Red;



            ini = new INI(Application.StartupPath + "\\Config.ini"); //配置文件
            exposeTime = double.Parse(ini.IniReadValue("", "Expose"));

            gain = double.Parse(ini.IniReadValue("", "Gain"));

            numExpose.Value = Convert.ToDecimal(exposeTime);
            numGain.Value = Convert.ToDecimal(gain);

            mAcqFIFOManager = new AcqFIFOManager();
            mAcqFIFOManager.InitAcqFIFOManager();

            mAcqFIFOManager.SetExposure(1, exposeTime);//初始化曝光时间
            mAcqFIFOManager.SetContrast(1, gain);
            mAcqFIFOManager.SetTriggerModel(1, CogAcqTriggerModelConstants.Auto);
            mAcqFIFOManager.OnImageReady1 += MAcqFIFOManager_OnImageReady1;
            mAcqFIFOManager.StartAcquire(1);

            mAcqFIFOManager.OnErrMsg += new ErrMsgHandler(mAcqFIFOManager_OnErrMsg);


            InitToolBlock();
            //ResultHandle = new System.Threading.Thread(ResultHandleThread);
            //ResultThreadRun = true;
            //ResultHandle.Start();
        }

        private void MAcqFIFOManager_OnImageReady1(ICogImage image)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ImageReadyHandler(MAcqFIFOManager_OnImageReady1), image);
                return;
            }
            try
            {
                if (LiveVideo)
                {
                    cogRecordDisplay1.Image = image;

                }
                if (LiveVideo == false && RunStatus == true)
                {
                    //A_ID1 = Int32.Parse(INIFileParam.Signal.IniReadValue("扫码交互", "A侧ID1"));
                    //A_ID2 = Int32.Parse(INIFileParam.Signal.IniReadValue("扫码交互", "A侧ID2"));
                    cogRecordDisplay1.Image = image;
                    cogRecordDisplay1.Fit(true);
                    mCogToolBlock1.Inputs["Input"].Value = image;
                    mCogToolBlock1.Run();
                }


            }
            catch (Exception ex)
            {

              
            }
        }

  
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (mAcqFIFOManager != null)
                {
                    toolStripStatusLabel2.BackColor = mAcqFIFOManager.Connection1 ? Color.Lime : Color.Red;

                }
            }
            catch (Exception ex)
            {

              
            }
        }

        private void ResultHandleThread()
        {

            try
            {

                    while (ResultThreadRun)
                    {
                   
                        if (RunStatus == false)
                        {
                            Thread.Sleep(20);
                            continue;
                        }
                        Thread.Sleep(20);
                        int signal_A = Int32.Parse(INIFileParam.Signal.IniReadValue("扫码交互", "A侧请求扫码"));
                        if (signal_A == 0)
                        {
                            INIFileParam.Signal.IniWriteValue("扫码交互", "视觉取消A侧请求扫码", "0");
                            A_read = true;
                        }
                            
                        if (signal_A == 1 && A_read)
                        {
                            A_read = false;
       
                            INIFileParam.Signal.IniWriteValue("扫码交互", "视觉取消A侧请求扫码", "1");
                            //mAcqFIFOManager.SetExposure(1,exposeTime);
                            //mAcqFIFOManager.AcqFifo1.StartAcquire();
                            Thread.Sleep(20);
                        }

                        //Thread.Sleep(10);
                        //int signal_A_back = Int32.Parse(INIFileParam.Signal.IniReadValue("扫码交互", "PLC取消A侧扫码反馈"));
                        //if (signal_A_back == 1)
                        //{
                        //    INIFileParam.Signal.IniWriteValue("扫码交互", "A侧扫码反馈", "0");
                        //    A_read = true;
                        //}

                    }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void InitToolBlock()
        {
            //string JobPath = AppDomain.CurrentDomain.BaseDirectory + "BlobFixture";
            string JobPath = AppDomain.CurrentDomain.BaseDirectory + "Code.vpp";
            mCogToolBlock1 = CogSerializer.LoadObjectFromFile(JobPath) as CogToolBlock;
            mCogIDTool1 = mCogToolBlock1.Tools["CogIDTool1"] as CogIDTool;
            mCogToolBlock1.Ran += new EventHandler(mCogToolBlock1_Ran);

            mCogIDTool2 = mCogToolBlock1.Tools["CogIDTool2"] as CogIDTool;



            cogToolBlockEditV21.Subject = mCogToolBlock1;
        }

        void mAcqFIFOManager_OnErrMsg(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ErrMsgHandler(mAcqFIFOManager_OnErrMsg), msg);
                return;
            }
            //MessageBox.Show(msg);
            //ErrLog.WriteLog(msg);
        }
        //void mAcqFIFOManager_OnImageReady(uint camNo, Cognex.VisionPro.ICogImage image)
        //{
        //    if (InvokeRequired)
        //    {
        //        BeginInvoke(new ImageReadyHandler(mAcqFIFOManager_OnImageReady), camNo, image);
        //        return;
        //    }
        //    switch (camNo)
        //    {
        //        case 1:
                   
        //            if (LiveVideo == false && RunStatus == true && A_index == 1)
        //            {
        //                cogRecordDisplay1.Image = image;
        //                mCogToolBlock1.Inputs["Input"].Value = image;
        //                mCogToolBlock1.Run();
        //            }

             
        //            if (LiveVideo == false && RunStatus == true && A_index == 2)
        //            {
        //                cogRecordDisplay2.Image = image;
        //                mCogToolBlock2.Inputs["Input"].Value = image;
        //                mCogToolBlock2.Run();
        //            }

        //            break;
        //        case 2:

                   
        //            if (LiveVideo == false && RunStatus == true && B_index == 1)
        //            {
        //                cogRecordDisplay3.Image = image;
        //                mCogToolBlock3.Inputs["Input"].Value = image;
        //                mCogToolBlock3.Run();
        //            }

                
        //            if (LiveVideo == false && RunStatus == true && B_index == 2)
        //            {
        //                cogRecordDisplay4.Image = image;
        //                mCogToolBlock4.Inputs["Input"].Value = image;
        //                mCogToolBlock4.Run();
        //            }


        //            break;


        //        default:
        //            break;
        //    }

        //}

        //读码
        private void mCogToolBlock1_Ran(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler(mCogToolBlock1_Ran), sender, e);
                return;
            }

            try
            {

                //if (mCogIDTool1 != null &&  mCogIDTool1.RunStatus.Result == CogToolResultConstants.Accept &&  mCogIDTool1.Results.Count > 0)
                //{
                //    A_ID1_Code  = mCogIDTool1.Results[0].DecodedData.DecodedString;
                //    textBox1.BackColor = Color.Lime;

                //}
                //else
                //{
                //    A_ID1_Code = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + A_ID1.ToString() + "-" + Guid.NewGuid().ToString().Substring(0, 8);
                //    textBox1.BackColor = Color.Yellow;

                //}

                 A_ID1_Code  = mCogIDTool1.Results[0].DecodedData.DecodedString;
                //INIFileParam.ID_CODE.IniWriteValue("ID", A_ID1.ToString(), A_ID1_Code);
                //this.Invoke(new MethodInvoker(delegate {
                textBox1.Text = A_ID1.ToString() + "---" + A_ID1_Code;
                //            richTextBox1.AppendText(A_ID1.ToString() + "---" + A_ID1_Code+Environment.NewLine);
                // }));


                //Thread.Sleep(10);
                //if (mCogIDTool2 != null && mCogIDTool2.RunStatus.Result == CogToolResultConstants.Accept && mCogIDTool2.Results.Count > 0)
                //{
                //    A_ID2_Code  = mCogIDTool2.Results[0].DecodedData.DecodedString;
                //    textBox2.BackColor = Color.Lime;
           
                //}
                //else
                //{
                //    A_ID2_Code = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + A_ID1.ToString() + "-" + Guid.NewGuid().ToString().Substring(0, 8);
                //    textBox2.BackColor = Color.Yellow;
            
                //}
  
                // INIFileParam.ID_CODE.IniWriteValue("ID", A_ID2.ToString(), A_ID2_Code);
                // this.Invoke(new MethodInvoker(delegate {
                //   textBox2.Text = A_ID2.ToString() + "---" + A_ID2_Code;
                //     richTextBox1.AppendText(A_ID2.ToString() + "---" + A_ID2_Code + Environment.NewLine);
                //     if (richTextBox1.TextLength > 10000)
                //     {
                //         richTextBox1.Clear();
                //     }
                // }));

         
          
                cogRecordDisplay1.Record = mCogToolBlock1.CreateLastRunRecord().SubRecords[0];
            }
            catch (Exception ex)
            {

           
            }

        }
        //读码

        private void numExpose_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (mAcqFIFOManager != null)
                {
         
                    exposeTime = Convert.ToDouble(numExpose.Value);
                    mAcqFIFOManager.SetExposure(1, Convert.ToDouble(numExpose.Value));
                    ini.IniWriteValue("", "Expose", numExpose.Value.ToString());
                }
            }
            catch (Exception ex)
            {

         
            }
        }

        private void numGain_ValueChanged(object sender, EventArgs e)
        {
            try
            {

                exposeTime = Convert.ToDouble(numGain.Value);
                mAcqFIFOManager.SetContrast(1, Convert.ToDouble(numGain.Value));
                ini.IniWriteValue("", "Gain", numGain.Value.ToString());
            }
            catch (Exception ex)
            {


            }
        }

        //读码

        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Text == "开始检测")
            {
                tableLayoutPanel1.Enabled = false;
                button19.Enabled = false;
                button5.Text = "停止检测";
                RunStatus = true;
                button5.BackColor = Color.Lime;
            }
            else
            {
                button5.Text = "开始检测";
                //mAcqFIFOManager.SetTriggerModel(1, CogAcqTriggerModelConstants.Manual);
                // mAcqFIFOManager.SetTriggerModel(2, CogAcqTriggerModelConstants.Manual);
                tableLayoutPanel1.Enabled = true;
                button19.Enabled = true;
                RunStatus = false;
                button5.BackColor = Color.Red;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenImage = new OpenFileDialog();
            OpenImage.Filter = "图像文件|*.bmp";
            if (OpenImage.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RunStart = DateTime.Now;
                CogImageFile ImageFile = new CogImageFile();
                ImageFile.Open(OpenImage.FileName,CogImageFileModeConstants.Read);
                if (ImageFile.Count > 0)
                {
                    mCogToolBlock1.Inputs["Input"].Value = ImageFile[0];
                    cogRecordDisplay1.Image = ImageFile[0];
                    cogRecordDisplay1.Fit();
                    mCogToolBlock1.Run();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string JobPath = AppDomain.CurrentDomain.BaseDirectory + "Code.vpp";
                CogSerializer.SaveObjectToFile(mCogToolBlock1, JobPath);
            }
            catch (Exception ex)
            {

               
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //mAcqFIFOManager.StopAcquire(1);
                mAcqFIFOManager.SetExposure(1, exposeTime);//初始化曝光时间
                mAcqFIFOManager.SetContrast(1, gain);
                mAcqFIFOManager.SetTriggerModel(1, CogAcqTriggerModelConstants.Manual);
                mAcqFIFOManager.AcqFifo1.StartAcquire();
            }
            catch (Exception ex)
            {

                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                mCogToolBlock1.Inputs["Input"].Value = cogRecordDisplay1.Image;
            }
            catch (Exception ex)
            {

              
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ResultThreadRun = false;
            //ResultHandle.Abort();
            mAcqFIFOManager.AcqFIFOManagerFree();

        }


        private void button19_Click(object sender, EventArgs e)
        {
            if (RunStatus == true)
            {
                return;
            }
            if (button19.Text == "实况视频")
            {
                LiveVideo = true;
                button19.Text = "实况中";
                if (mAcqFIFOManager != null)
                {
                    mAcqFIFOManager.StopAcquire(1);
                    mAcqFIFOManager.SetExposure(1, exposeTime);//初始化曝光时间
                    mAcqFIFOManager.SetContrast(1, gain);
                    mAcqFIFOManager.SetTriggerModel(1,CogAcqTriggerModelConstants.FreeRun);
                    mAcqFIFOManager.StartAcquire(1);
                }

            }
            else
            {
                LiveVideo = false;
                button19.Text = "实况视频";
                if (mAcqFIFOManager != null)
                {
                    mAcqFIFOManager.StopAcquire(1);
                    mAcqFIFOManager.SetExposure(1, exposeTime);//初始化曝光时间
                    mAcqFIFOManager.SetContrast(1, gain);
                    mAcqFIFOManager.SetTriggerModel(1, CogAcqTriggerModelConstants.Auto);
                    mAcqFIFOManager.StartAcquire(1);

                }
            }
        }

    }
}
