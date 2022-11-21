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
    public partial class LoadingForm : Form
    {
        //用来显示软件初始化进度
        public int LoadProccess = 0;
        public string ShowText = "初始化相机中";
        public LoadingForm()
        {
            InitializeComponent();
        }

        public void ProcessShow()
        {
            progressBar1.Value = LoadProccess;
            label2.Text = ShowText; 
        }
    }
}
