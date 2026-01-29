using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TWatcher
{
    public partial class frmHistory : Form
    {
        public frmHistory()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            string logFilePath = @"C:\Alice\Logs\AliceBob_ConnectionHistory.txt";

            if (File.Exists(logFilePath))
            {
                rtxtHistory.Text = File.ReadAllText(logFilePath);
            }
            else
            {
                rtxtHistory.Text = "Chưa có lịch sử kết nối nào được lưu.";
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
