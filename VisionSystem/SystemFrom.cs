using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VisionSystem
{
    public partial class SystemFrom : Form
    {
        public SystemFrom()
        {
            InitializeComponent();
        }

        private void SystemFrom_Load(object sender, EventArgs e)
        {
            if (INIFileParam.EncodeEnable)
            {
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                comboBox6.Enabled = true;
            }
            else
            {
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;
                comboBox6.Enabled = false;
            }
            if (INIFileParam.TriggerEnable)
            {
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
            }
            else
            {
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
            }
            if (INIFileParam.PixtureSave)
            {
                radioButton2.Checked = true;
                radioButton1.Checked = false;
            }
            else
            {
                radioButton2.Checked = false;
                radioButton1.Checked = true;
            }
            if (INIFileParam.EncodeEnable)
            {
                EncodeEnable.Checked = true;
                //编码器参数加载
                switch (INIFileParam.EncodeTiggerSource)
                {
                    case "Line1":
                        comboBox6.SelectedIndex = 0;
                        break;
                    case "Line2":
                        comboBox6.SelectedIndex = 1;
                        break;
                }
                switch (INIFileParam.EncodeTiggerActivation)
                {
                    case "FallingEdge":
                        comboBox5.SelectedIndex = 0;
                        break;
                    case "RisingEdge":
                        comboBox5.SelectedIndex = 1;
                        break;
                }
                switch (INIFileParam.EncodeTiggerLineLevel)
                {
                    case "Threshold5V":
                        comboBox4.SelectedIndex = 0;
                        break;
                    case "Threshold12V":
                        comboBox4.SelectedIndex = 1;
                        break;
                    case "Threshold24V":
                        comboBox4.SelectedIndex = 2;
                        break;
                }

            }
            else
            {
                EncodeEnable.Checked = false;
            }
            if (INIFileParam.TriggerEnable)
            {
                Trigger.Checked = true;
                //相机触发参数加载
                switch (INIFileParam.CameraTiggerSource)
                {
                    case "Line1":
                        comboBox1.SelectedIndex = 0;
                        break;
                    case "Line2":
                        comboBox1.SelectedIndex = 1;
                        break;
                }
                switch (INIFileParam.CameraTiggerActivation)
                {
                    case "FallingEdge":
                        comboBox2.SelectedIndex = 1;
                        break;
                    case "RisingEdge":
                        comboBox2.SelectedIndex = 2;
                        break;
                }
                switch (INIFileParam.CameraTiggerLineLevel)
                {
                    case "Threshold5V":
                        comboBox3.SelectedIndex = 0;
                        break;
                    case "Threshold12V":
                        comboBox3.SelectedIndex = 1;
                        break;
                    case "Threshold24V":
                        comboBox3.SelectedIndex = 2;
                        break;
                }
            }
            else
            {
                Trigger.Checked = false;
            }
            CameraSNTextBox.Text = INIFileParam.Cam1SerialNum;
            comboBox7.Items.Add(INIFileParam.CamExposure[0].ToString("0.00"));
            comboBox7.Items.Add(INIFileParam.CamExposure[1].ToString("0.00"));
            comboBox7.Items.Add(INIFileParam.CamExposure[2].ToString("0.00"));
            CameraLineRateTextBox.Text = INIFileParam.CamLineRate.ToString();
            ROIWidthTextBox.Text = INIFileParam.CamROIWidth.ToString();
            ROIHeightTextBox.Text = INIFileParam.CamROIHeight.ToString();
            CameraDebounTextBox.Text = INIFileParam.LineDebouncingPeriod.ToString();
            CameraTriggerDelayTextBox.Text = INIFileParam.CamDelay.ToString();
            WhiteBalanceRTextBox.Text = INIFileParam.CamWhiteBalanceR.ToString();
            WhiteBalanceGTextBox.Text = INIFileParam.CamWhiteBalanceG.ToString();
            WhiteBalanceBTextBox.Text = INIFileParam.CamWhiteBalanceB.ToString();
            DividerTxt.Text = INIFileParam.EncodeDivider;
            MultiplierTxt.Text = INIFileParam.EncodeMultiplier;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (CameraSNTextBox.Text != string.Empty)
                    INIFileParam.Cam1SerialNum = CameraSNTextBox.Text;
                //if (CameraExposureTextBox.Text != string.Empty)
                //    INIFileParam.CamExposure = double.Parse(CameraExposureTextBox.Text);
                if (CameraLineRateTextBox.Text != string.Empty)
                    INIFileParam.CamLineRate = Int32.Parse(CameraLineRateTextBox.Text);
                if (ROIWidthTextBox.Text != string.Empty)
                    INIFileParam.CamROIWidth = Int32.Parse(ROIWidthTextBox.Text);
                if (ROIHeightTextBox.Text != string.Empty)
                    INIFileParam.CamROIHeight = Int32.Parse(ROIHeightTextBox.Text);
                if (CameraDebounTextBox.Text != string.Empty)
                    INIFileParam.LineDebouncingPeriod = Int32.Parse(CameraDebounTextBox.Text);
                if (CameraTriggerDelayTextBox.Text != string.Empty)
                    INIFileParam.CamDelay = Int32.Parse(CameraTriggerDelayTextBox.Text);
                if (WhiteBalanceRTextBox.Text != string.Empty)
                    INIFileParam.CamWhiteBalanceR = uint.Parse(WhiteBalanceRTextBox.Text);
                if (WhiteBalanceGTextBox.Text != string.Empty)
                    INIFileParam.CamWhiteBalanceG = uint.Parse(WhiteBalanceGTextBox.Text);
                if (WhiteBalanceBTextBox.Text != string.Empty)
                    INIFileParam.CamWhiteBalanceB = uint.Parse(WhiteBalanceBTextBox.Text);
                INIFileParam.TriggerEnable = Trigger.Checked;
                if (Trigger.Checked)
                {                    
                    //相机触发参数保存
                    if (comboBox1.SelectedItem != null)
                    {
                        switch (comboBox1.SelectedItem.ToString())
                        {
                            case "Line1":
                                INIFileParam.CameraTiggerSource = "Line1";
                                break;
                            case "Line2":
                                INIFileParam.CameraTiggerSource = "Line2";
                                break;
                        }
                    }
                    if (comboBox2.SelectedItem != null)
                    {
                        switch (comboBox2.SelectedItem.ToString())
                        {
                            case "上升沿触发":
                                INIFileParam.CameraTiggerActivation = "RisingEdge";
                                break;
                            case "下降沿触发":
                                INIFileParam.CameraTiggerActivation = "FallingEdge";
                                break;
                        }
                    }
                    if (comboBox3.SelectedItem != null)
                    {
                        switch (INIFileParam.CameraTiggerLineLevel)
                        {
                            case "5V电平（TTL）":
                                INIFileParam.CameraTiggerLineLevel = "Threshold5V";
                                break;
                            case "12V电平":
                                INIFileParam.CameraTiggerLineLevel = "Threshold12V";
                                break;
                            case "24V电平":
                                INIFileParam.CameraTiggerLineLevel = "Threshold24V";
                                break;
                        }
                    }
                }
                INIFileParam.EncodeEnable = EncodeEnable.Checked;
                if (EncodeEnable.Checked != INIFileParam.EncodeEnable)
                {
                    //编码器触发参数保存
                    if (comboBox6.SelectedItem != null)
                    {
                        switch (comboBox6.SelectedItem.ToString())
                        {
                            case "Line1":
                                INIFileParam.CameraTiggerSource = "Line1";
                                break;
                            case "Line2":
                                INIFileParam.CameraTiggerSource = "Line2";
                                break;
                        }
                    }
                    if (comboBox5.SelectedItem != null)
                    {
                        switch (comboBox5.SelectedItem.ToString())
                        {
                            case "上升沿触发":
                                INIFileParam.CameraTiggerActivation = "RisingEdge";
                                break;
                            case "下降沿触发":
                                INIFileParam.CameraTiggerActivation = "FallingEdge";
                                break;
                        }
                    }
                    if (comboBox4.SelectedItem != null)
                    {
                        switch (comboBox4.SelectedItem.ToString())
                        {
                            case "5V电平（TTL）":
                                INIFileParam.CameraTiggerLineLevel = "Threshold5V";
                                break;
                            case "12V电平":
                                INIFileParam.CameraTiggerLineLevel = "Threshold12V";
                                break;
                            case "24V电平":
                                INIFileParam.CameraTiggerLineLevel = "Threshold24V";
                                break;
                        }
                    }
                    INIFileParam.EncodeDivider = DividerTxt.Text;
                    INIFileParam.EncodeMultiplier = MultiplierTxt.Text;
                }
                INIFileParam.SaveCameraInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入格式错误，请确认");
            }
        }

        private void Trigger_CheckedChanged(object sender, EventArgs e)
        {
            if (Trigger.Checked)
            {
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
            }
            else
            {
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
            }
        }

        private void EncodeEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (EncodeEnable.Checked)
            {
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                comboBox6.Enabled = true;
            }
            else 
            {
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;
                comboBox6.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //if (radioButton1.Checked == false)
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
