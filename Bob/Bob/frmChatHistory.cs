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

namespace Bob
{
    public partial class frmChatHistory : Form
    {
        public frmChatHistory()
        {
            InitializeComponent();
            LoadChatHistory();
        }

        private void LoadChatHistory()
        {
            string chatLogPath = @"C:\Bob\Logs\Bob_ChatHistory.txt";

            if (File.Exists(chatLogPath))
            {
                rtxtMessageHistory.Text = File.ReadAllText(chatLogPath);
            }
            else
            {
                rtxtMessageHistory.Text = "Chưa có lịch sử trò chuyện nào.";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
