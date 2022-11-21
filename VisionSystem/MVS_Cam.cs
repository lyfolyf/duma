using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using MvCamCtrl;
using MvCamCtrl.NET;
using System.Windows.Forms;

namespace VisionSystem
{
    class MVS_Cam
    {
        public MyCamera.cbOutputExdelegate ImageCallback;
        public MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        public MyCamera MVS_Camera1;
        public void Init_MVS_Cam()
        {

            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE, ref m_pDeviceList);

            // ch:在窗体列表中显示设备名 | en:Display device name in the form list
            for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                if (gigeInfo.chSerialNumber == INIFileParam.Cam1SerialNum)
                {
                    MVS_Camera1 = new MyCamera();
                    nRet = MVS_Camera1.MV_CC_CreateDevice_NET(ref device);
                    if (MyCamera.MV_OK != nRet)
                    {
                        MessageBox.Show("创建相机对象失败！");
                        return;
                    }
                    nRet = MVS_Camera1.MV_CC_OpenDevice_NET();
                    if (MyCamera.MV_OK != nRet)
                    {
                        MVS_Camera1.MV_CC_DestroyDevice_NET();
                        MessageBox.Show(string.Format("相机打开失败，SN：{0}！", INIFileParam.Cam1SerialNum));
                        return;
                    }
                }

            }
            
        }

        public bool StartGrab()
        {
            if (MVS_Camera1 != null)
            {
                // 开启抓图 
                int nRet = MVS_Camera1.MV_CC_StartGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("开启抓图失败");
                    return false;
                }
                return true;
            }
            else
                return false;
        }
        public bool StopGrab()
        {
            if (MVS_Camera1 != null)
            {
                // 停止抓图 
                int nRet = MVS_Camera1.MV_CC_StopGrabbing_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("停止抓图失败");
                    return false;
                }
                return true;
            }
            else
                return false;
        }

        public bool SetAOI_HeightWidth(uint Height, uint Width)
        {
            if (MVS_Camera1 != null)
            {
                // 停止抓图 
                int nRet = MVS_Camera1.MV_CC_SetHeight_NET(Height);
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("设置Height失败");
                    return false;
                }
                nRet = MVS_Camera1.MV_CC_SetHeight_NET(Width);
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("设置Width失败");
                    return false;
                }
                return true;
            }
            else
                return false;

        }

        public bool SetExposure(float Exposure)
        {
            
            if (MVS_Camera1 != null)
            {
                // 停止抓图 
                int nRet = MVS_Camera1.MV_CC_SetExposureTime_NET(Exposure);
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("设置Exposure失败");
                    return false;
                }
                return true;
            }
            else
                return false;
        }

        public bool SetGain(float Gain)
        {

            if (MVS_Camera1 != null)
            {
                // 停止抓图 
                int nRet = MVS_Camera1.MV_CC_SetGain_NET(Gain);
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("设置Gain失败");
                    return false;
                }
                return true;
            }
            else
                return false;
        }

        public bool SetBalanceRatio_RGB(uint Red, uint Green, uint Blue)
        {

            if (MVS_Camera1 != null)
            {
                // 设置RGB白平衡数值 
                int nRet = MVS_Camera1.MV_CC_SetBalanceRatioRed_NET(Red);
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("设置SetBalanceRatio_Red失败");
                    return false;
                }
                // 设置RGB白平衡数值 
                nRet = MVS_Camera1.MV_CC_SetBalanceRatioGreen_NET(Green);
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("设置SetBalanceRatio_Green失败");
                    return false;
                }
                // 设置RGB白平衡数值 
                nRet = MVS_Camera1.MV_CC_SetBalanceRatioBlue_NET(Blue);
                if (MyCamera.MV_OK != nRet)
                {
                    MessageBox.Show("设置SetBalanceRatio_Blue失败");
                    return false;
                }
                return true;
            }
            else
                return false;
        }

        public bool SetTriggerParam()
        {
            if (MVS_Camera1 != null)
            {

                return true;
            }
            else
                return false;
        }

        public void FreeMyCamera()
        {
                // ch:关闭设备 | en:Close device
                int nRet = MVS_Camera1.MV_CC_CloseDevice_NET();

                // ch:销毁设备 | en:Destroy device
                nRet = MVS_Camera1.MV_CC_DestroyDevice_NET();
        }
    }
    
}
