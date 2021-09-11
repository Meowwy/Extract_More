using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnzipMore
{
    public partial class Form1 : Form
    {
        public string rootPath { get; set; }
        public bool running { get; set; }
        public bool deleteSource;
        public Form1()
        {
            InitializeComponent();
            lbl_info.Text = "Select a directory, click on EXTRACT and wait until DONE appears.";
            lbl_status.Text = "";
            pictureBox1.Image = Properties.Resources.Extract_More_logo_v2;
            btn_cancel.Enabled = false;
            label4.Visible = false;
        }
        private static Form1 instanceForm1 = null;

        public static Form1 InstanceForm1
        {
            get
            {
                if (instanceForm1 == null)
                {
                    instanceForm1 = new Form1();
                }
                return instanceForm1;
            }
        }

        private void btn_extract_Click(object sender, EventArgs e)
        {
            running = true;
            
            if (string.IsNullOrEmpty(rootPath))
            {
                MessageBox.Show("ROOTPATH must be selected", "Input error");
                return;
            }

            ExtractService exSer = new ExtractService();
            lbl_status.Text = "Searching files...";
            btn_cancel.Enabled = true;
            Task.Run(() => exSer.Unzip(rootPath));
            exSer.Untar(rootPath);
            btn_cancel.Enabled = false;
            lbl_status.Text = "DONE";
        }

        private void btn_root_Click(object sender, EventArgs e)
        {
            lbl_status.Text = "";
            label4.Visible = false;
            lbl_root.Text = "Nothing in here yet";
            rootPath = "";
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                lbl_root.Text = folderBrowser.SelectedPath;
                label4.Visible = true;
                rootPath = folderBrowser.SelectedPath;
            }
        }

        public void Message(string text)
        {
            MessageBox.Show(text);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo psInfo = new ProcessStartInfo
            {
                FileName = "https://github.com/sssvt-koleckar-pavel/Extract_More",
                UseShellExecute = true
            };
            Process.Start(psInfo);
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            if(running == true)
            {
                ExtractService exSer = new ExtractService();
                exSer.StopIt();
            }
            
        }       
    }
}
