using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BackgroundWorkerPause
{
    public partial class MainForm : Form
    {
        ManualResetEvent manualResetEvent = new ManualResetEvent(true);
        BackgroundWorker backgroundWorker = null;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                if(backgroundWorker.CancellationPending)//用户申请取消
                {
                    for (int j = i; j >= 0; j--)
                    {
                        Thread.Sleep(10);
                        backgroundWorker.ReportProgress(j);
                    }
                    e.Cancel = true;
                    return;
                }

                manualResetEvent.WaitOne();

                backgroundWorker.ReportProgress(i + 1);
                Thread.Sleep(500);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Cancelled)
            {
                MessageBox.Show("用户取消了操作");
            }
            else
            {
                MessageBox.Show("正常完成了操作");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            backgroundWorker.RunWorkerAsync();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (btnPause.Text == "暂停")
            {
                manualResetEvent.Reset();
                btnPause.Text = "继续";
            }
            else
            {
                manualResetEvent.Set();
                btnPause.Text = "暂停";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker.CancelAsync();
        }
    }
}
