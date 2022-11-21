using System;
using System.Collections.Generic;
using System.Text;
using Cognex.VisionPro;
using Cognex.VisionPro.Exceptions;
using System.Threading;
using System.Windows.Forms;

namespace VisionSystem
{


    public delegate void ErrMsgHandler(string msg);
    public delegate void ImageReadyHandler(ICogImage image);


    public class AcqFIFOManager
    {
        public event ErrMsgHandler OnErrMsg = null;

        public event ImageReadyHandler OnImageReady1 = null;

        public event ImageReadyHandler OnImageReady2 = null;


        public ICogAcqFifo AcqFifo1
        {
            get { return mAcqFifo1; }
        }
        private ICogAcqFifo mAcqFifo1 = null;
        ICogGigEAccess gigEAccess1 = null;
        private uint mNumAcqs1 = 0;
        private bool mConnection1 = false;
        public bool Connection1
        {
            get { return mConnection1; }
        }


        public ICogAcqFifo AcqFifo2
        {
            get { return mAcqFifo2; }
        }
        private ICogAcqFifo mAcqFifo2 = null;
        ICogGigEAccess gigEAccess2 = null;
        private uint mNumAcqs2 = 0;
        private bool mConnection2 = false;
        public bool Connection2
        {
            get { return mConnection2; }
        }


        /// <summary>
        /// 初始化必要数据
        /// </summary>
        public void InitAcqFIFOManager()
        {
            CogFrameGrabbers mFrameGrabbers = new CogFrameGrabbers();
            foreach (ICogFrameGrabber mFrameGrabber in mFrameGrabbers)
            {

                switch (mFrameGrabber.SerialNumber)
                {
                    case "00D89953926":
                        mAcqFifo1 = mFrameGrabber.CreateAcqFifo(mFrameGrabber.AvailableVideoFormats[0], CogAcqFifoPixelFormatConstants.Format8Grey, 0, true);
                        gigEAccess1 = mFrameGrabber.OwnedGigEAccess;
                        break;

                    case "00D89951113926":
                        mAcqFifo2 = mFrameGrabber.CreateAcqFifo(mFrameGrabber.AvailableVideoFormats[0], CogAcqFifoPixelFormatConstants.Format8Grey, 0, true);
                        gigEAccess2 = mFrameGrabber.OwnedGigEAccess;
                        break;

                    default:
                        break;
                }

            }

            if (mAcqFifo1 != null)
            {
                mAcqFifo1.Complete += new CogCompleteEventHandler(mAcqFifo1_Complete);
                if (mAcqFifo1.OwnedExposureParams != null)
                    mAcqFifo1.OwnedExposureParams.Exposure = 0.2; //mSecs
                if (mAcqFifo1.OwnedBrightnessParams != null)
                {
                    mAcqFifo1.OwnedBrightnessParams.Brightness = 0;
                    mAcqFifo1.OwnedContrastParams.Contrast = 0;
                }

                gigEAccess1.SetFeature("LineSelector", "Line1");
                //gigEAccess1.SetDoubleFeature("LineDebouncerTimeAbs", 100);

                gigEAccess1.SetFeature("TriggerActivation", "RisingEdge");


                // Alternatively you can the trigger activaton to FallingEdge.
                // gigEAccess.SetFeature("TriggerActivation", "FallingEdge");
                mConnection1 = true;
                mAcqFifo1.OwnedTriggerParams.TriggerEnabled = false;
            }
            else
            {
                ErrMsg("相机丢失......");
            }





        }



        #region Complete event


        void mAcqFifo1_Complete(object sender, CogCompleteEventArgs e)
        {
            int numReadyVal, numPendingVal, myTicket, currentTrigNum;
            bool busyVal;
            try
            {
                //This is the complete event handler for acquisition.  When an image is acquired,
                //it fires a complete event.  This handler verifies the state of the acquisition
                //fifo, and then calls Complete(), which gets the image from the fifo.

                //Note that it is necessary to call the .NET garbarge collector on a regular
                //basis so large images that are no longer used will be released back to the
                //heap.  In this sample, it is called every 5th acqusition. 

                mAcqFifo1.GetFifoState(out numPendingVal, out numReadyVal, out busyVal);
              
                if (numReadyVal > 0)
                {
                    CogImage8Grey image = mAcqFifo1.CompleteAcquire(-1, out myTicket, out currentTrigNum) as CogImage8Grey;
                    ///OnImageReady1(image);
                }
             
              
                //mConnection = true;
                mNumAcqs1++;
                // We need to run the garbage collector on occasion to cleanup
                // images that are no longer being used.
                if (mNumAcqs1 > 5)
                {
                    GC.Collect();
                    mNumAcqs1 = 0;

                }

            }
            catch (CogException ce)
            {

                OnImageReady1(null);
            }
        }


        #endregion
        /// <summary>
        /// 启动获取图像
        /// </summary>
        public void StartAcquire(uint camNo)
        {
            switch (camNo)
            {


                case 1:
                    if (mAcqFifo1 != null)
                    {
                        if (!mAcqFifo1.OwnedTriggerParams.TriggerEnabled)
                        {
                            mAcqFifo1.Flush();
                            mAcqFifo1.OwnedTriggerParams.TriggerEnabled = true;
                        }
                    }
                    break;




                default:
                    break;
            }
        }
        /// <summary>
        /// 停止获取图像
        /// </summary>
        public void StopAcquire(uint camNo)
        {
            switch (camNo)
            {
                case 1:
                    if (mAcqFifo1 != null)
                    {
                        if (mAcqFifo1.OwnedTriggerParams.TriggerEnabled)
                        {
                            mAcqFifo1.OwnedTriggerParams.TriggerEnabled = false;
                            mAcqFifo1.Flush();
                        }
                    }
                    break;



                default: break;


            }
        }
        /// <summary>
        /// 设置白平衡
        /// </summary>
        /// <param name="gigEAccess"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void CfgSetWB(ICogGigEAccess gigEAccess, uint r, uint g, uint b)
        {
            if (gigEAccess != null)
            {
                gigEAccess.SetFeature("BalanceRatioSelector", "Red");
                gigEAccess.SetIntegerFeature("BalanceRatioRaw", r);
                gigEAccess.SetFeature("BalanceRatioSelector", "Green");
                gigEAccess.SetIntegerFeature("BalanceRatioRaw", g);
                gigEAccess.SetFeature("BalanceRatioSelector", "Blue");
                gigEAccess.SetIntegerFeature("BalanceRatioRaw", b);
            }

        }
        /// <summary>
        /// 自动白平衡
        /// </summary>
        /// <param name="gigEAccess"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void CfgAutoWB(uint camNo, out uint r, out uint g, out uint b)
        {
            r = g = b = 128;
            switch (camNo)
            {

                case 1:
                    if (gigEAccess1 != null)
                    {

                        if (mAcqFifo1.OwnedTriggerParams.TriggerEnabled)
                        {
                            gigEAccess1.SetFeature("BalanceWhiteAuto", "Once");
                            while (gigEAccess1.GetFeature("BalanceWhiteAuto") == "Once")
                            { Thread.Sleep(500); }
                        }
                        if (gigEAccess1.GetFeature("BalanceWhiteAuto") != "Once")
                        {
                            gigEAccess1.SetFeature("BalanceRatioSelector", "Red");
                            r = gigEAccess1.GetIntegerFeature("BalanceRatioRaw");
                            gigEAccess1.SetFeature("BalanceRatioSelector", "Green");
                            g = gigEAccess1.GetIntegerFeature("BalanceRatioRaw");
                            gigEAccess1.SetFeature("BalanceRatioSelector", "Blue");
                            b = gigEAccess1.GetIntegerFeature("BalanceRatioRaw");
                        }
                    }
                    break;

                default: break;
            }
        }
        /// <summary>
        /// 设置出发模式
        /// </summary>
        /// <param name="triggerModel">触发模式</param>
        public void SetTriggerModel(uint canNo, CogAcqTriggerModelConstants triggerModel)
        {
            switch (canNo)
            {

                case 1:
                    if (mAcqFifo1 != null)
                        mAcqFifo1.OwnedTriggerParams.TriggerModel = triggerModel;
                    break;



                default: break;
            }
        }
        /// <summary>
        /// 设置亮度
        /// </summary>
        /// <param name="brightness">亮度值</param>
        public void SetBrightness(uint camNo, double brightness)
        {
            switch (camNo)
            {
                case 1:
                    if (mAcqFifo1 != null)
                        mAcqFifo1.OwnedBrightnessParams.Brightness = brightness;
                    break;



                default: break;
            }
        }
        /// <summary>
        /// 设置曝光时间mSecs
        /// </summary>
        /// <param name="exposure">曝光时间mSecs</param>
        public void SetExposure(uint camNo, double exposure)
        {
            switch (camNo)
            {
                case 1:
                    if (mAcqFifo1 != null)
                        mAcqFifo1.OwnedExposureParams.Exposure = exposure; //mSecs
                    break;



                default: break;
            }
        }



        /// <summary>
        /// 设置相机对比度
        /// </summary>
        /// <param name="contrast">对比度值</param>
        public void SetContrast(uint camNo, double contrast)
        {
            switch (camNo)
            {
                case 1:
                    if (mAcqFifo1 != null)
                        mAcqFifo1.OwnedContrastParams.Contrast = contrast;
                    break;

                default: break;
            }
        }

        private void ErrMsg(string msg)
        {
            if (OnErrMsg != null)
                OnErrMsg(msg);
        }



        public void AcqFIFOManagerFree()
        {
            CogFrameGrabbers frameGrabbers = new CogFrameGrabbers();
            foreach (ICogFrameGrabber fg in frameGrabbers)
            {
                fg.Disconnect(false);
            }

        }
    }

}
