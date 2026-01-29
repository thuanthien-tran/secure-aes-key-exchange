using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices.ComTypes;

namespace Alice
{


    public partial class Form1 : Form
    {

        private RSA rsaAlice;
        private TcpListener server;
        private TcpClient client;
        private NetworkStream stream;

        private bool isReceiving = true;

        public Form1()
        {
            InitializeComponent();
            InitializeKeys();
            GenerateAndDisplayFingerprint();
        }


        //*** FUNCTION ***//

        // Hàm khởi tạo khóa
        private void InitializeKeys()
        {
            rsaAlice = RSA.Create();
            string folderPath = @"C:\Alice\Keys";
            Directory.CreateDirectory(folderPath);

            string publicKeyPath = Path.Combine(folderPath, "Alice_PublicKey.xml");
            string privateKeyPath = Path.Combine(folderPath, "Alice_PrivateKey.xml");

            // Tạo khóa của Alice nếu chưa tồn tại
            if (!File.Exists(publicKeyPath))
            {
                File.WriteAllText(publicKeyPath, rsaAlice.ToXmlString(false));
            }

            if (!File.Exists(privateKeyPath))
            {
                File.WriteAllText(privateKeyPath, rsaAlice.ToXmlString(true));
            }
        }

        // Hàm nhận kết nối + khóa + dữ liệu từ Bob
        private void AcceptClient(IAsyncResult ar)
        {
            client = server.EndAcceptTcpClient(ar);
            stream = client.GetStream();
            this.Invoke(new Action(() => txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] Bob đã kết nối!\n" + Environment.NewLine)));
            
            ReceivePublicKey();
            ReceivePrivateKey(); 
            ReceiveEncryptedMessage();

            LogConnectionHistory("Bob đã kết nối.");
            LogConnectionHistory("Bob đã ngắt kết nối.\n");

        }

        // Hàm tạo Fingerprint SHA-256 - Xác thực 2 bước
        private void GenerateAndDisplayFingerprint()
        {
            try
            {
                string publicKeyPath = @"C:\Alice\Keys\Alice_PublicKey.xml";
                if (!File.Exists(publicKeyPath))
                {
                    MessageBox.Show("Không tìm thấy public key của Alice!");
                    return;
                }

                string publicKeyXml = File.ReadAllText(publicKeyPath);
                SHA256 sha256 = SHA256.Create();
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(publicKeyXml));
                sha256.Dispose();

                string fingerprint = BitConverter.ToString(hashBytes).Replace("-", ":");

                string fingerprintFilePath = @"C:\Alice\Keys\AliceFingerprint.txt";
                File.WriteAllText(fingerprintFilePath, fingerprint);

                txtOutput.AppendText("*********  Hãy gửi Fingerprint (riêng tư) cho Bob trước khi gửi khóa *********\n");
                txtOutput.AppendText(Environment.NewLine);
                txtOutput.AppendText("***********************" +
                                    "************************" +
                                    "*************************" +
                                    "*********\n\n");

                btnOpenFolder.Click += (sender, e) =>
                {
                    System.Diagnostics.Process.Start("explorer.exe", @"/select," + fingerprintFilePath);
                };

                this.Controls.Add(btnOpenFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo fingerprint: " + ex.Message);
            }
        }

        // Hàm gửi khóa công khai + riêng tư
        private void SendPrivateAndPublicKey()
        {
            try
            {
                if (client == null || stream == null || !stream.CanWrite)
                {
                    MessageBox.Show("Alice chưa kết nối với Bob!");
                    return;
                }

                // Public Key
                string publicKey = rsaAlice.ToXmlString(false);
                byte[] keyBytes_public = Encoding.UTF8.GetBytes(publicKey);
                stream.Write(keyBytes_public, 0, keyBytes_public.Length);
                stream.Flush();

                // Private Key
                string privateKeyPath = @"C:\Alice\Keys\Alice_PrivateKey.xml";
                if (!File.Exists(privateKeyPath))
                {
                    MessageBox.Show("Không tìm thấy private key của Alice!");
                    return;
                }

                string privateKeyXml = File.ReadAllText(privateKeyPath);
                byte[] keyBytes_private = Encoding.UTF8.GetBytes(privateKeyXml);
                stream.Write(keyBytes_private, 0, keyBytes_private.Length);
                stream.Flush();

                MessageBox.Show("Alice đã gửi khóa cho Bob!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi gửi khi gửi khóa: " + ex.Message);
            }
        }

        // Hàm nhận khóa công khai
        private async void ReceivePublicKey()
        {
            try
            {
                if (stream == null || !stream.CanRead)
                {
                    MessageBox.Show("Alice không thể đọc dữ liệu từ Bob!");
                    return;
                }

                byte[] buffer = new byte[1024]; // Kích thướt 1024 byte
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    string publicKeyXml = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    if (!publicKeyXml.StartsWith("<RSAKeyValue>"))
                    {
                        MessageBox.Show("Dữ liệu nhận được không phải public key hợp lệ từ Bob!");
                        return;
                    }

                    string savePath = @"C:\Alice\Keys\Bob_PublicKey_Received.xml";
                    File.WriteAllText(savePath, publicKeyXml);
                    this.Invoke(new Action(() => txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] Alice đã nhận được public key từ Bob!\n" + Environment.NewLine)));
                }
                else
                {
                    MessageBox.Show("Alice không nhận được khóa từ Bob!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nhận public key từ Bob: " + ex.Message);
            }
        }

        // Hàm nhận khóa riêng tư
        private async void ReceivePrivateKey()
        {
            try
            {
                if (stream == null || !stream.CanRead)
                {
                    MessageBox.Show("Alice không thể đọc dữ liệu!");
                    return;
                }

                byte[] buffer = new byte[1024]; // Kích thướt 1024 byte
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    string privateKeyXml = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    File.WriteAllText(@"C:\Alice\Keys\Bob_PrivateKey_Received.xml", privateKeyXml);
                    this.Invoke(new Action(() => txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] Alice đã nhận được private key từ Bob!\n" + Environment.NewLine)));
                }
                else
                {
                    MessageBox.Show("Alice không nhận được private key từ Bob!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nhận private key: " + ex.Message);
            }
        }
        private async void ReceiveEncryptedMessage()
        {
            try
            {
                string privateKeyPath = @"C:\Alice\Keys\Alice_PrivateKey.xml";
                if (!File.Exists(privateKeyPath))
                {
                    this.Invoke(new Action(() => txtOutput.AppendText("Không tìm thấy private key của Alice!\n")));
                    return;
                }

                RSA rsaAlicePrivate = RSA.Create();
                rsaAlicePrivate.FromXmlString(File.ReadAllText(privateKeyPath));

                // Bắt đầu: Lưu lịch sử chat
                string chatLogPath = @"C:\Alice\Logs\Alice_ChatHistory.txt";
                Directory.CreateDirectory(Path.GetDirectoryName(chatLogPath));
                LogNewChatSession();

                // Vòng lặp lắng nghe tin nhắn liên tục từ Bob
                while (isReceiving)
                {
                    if (stream == null || !stream.CanRead)
                    {
                        string disconnectMsg = $"[{DateTime.Now:HH:mm:ss}] Bob đã ngắt kết nối!\n";
                        this.Invoke(new Action(() => txtOutput.AppendText(disconnectMsg)));

                        // Kết thúc: Lưu lịch sử chat
                        LogEndChatSession();
                        return;
                    }

                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;

                    try
                    {
                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    }
                    catch (IOException)
                    {
                        this.Invoke(new Action(() => txtOutput.AppendText(
                            "[" + DateTime.Now.ToString("HH:mm:ss") + "] Kết nối đã bị đóng từ phía Bob.\n")));

                        // Kết thúc: Lưu lịch sử chat
                        LogEndChatSession();
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        this.Invoke(new Action(() => txtOutput.AppendText(
                            "[" + DateTime.Now.ToString("HH:mm:ss") + "] Stream đã bị đóng đột ngột.\n")));

                        // Kết thúc: Lưu lịch sử chat
                        LogEndChatSession();
                        break;
                    }

                    if (bytesRead == 0)
                    {
                        string disconnectMsg = $"[{DateTime.Now:HH:mm:ss}] Bob đã ngắt kết nối!\n";
                        this.Invoke(new Action(() => txtOutput.AppendText(disconnectMsg)));

                        // Kết thúc: Lưu lịch sử chat
                        LogEndChatSession();
                        break;
                    }
                    try
                    {
                        byte[] encryptedMessage = buffer.Take(bytesRead).ToArray();
                        byte[] decryptedMessage = rsaAlicePrivate.Decrypt(encryptedMessage, RSAEncryptionPadding.Pkcs1);
                        string decryptedMessageString = Encoding.UTF8.GetString(decryptedMessage);

                        string timestamp = "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
                        string fullMessage = $"{timestamp} Bob: {decryptedMessageString}";

                        // Hiển thị trên giao diện
                        this.Invoke(new Action(() => txtOutput.AppendText(fullMessage + Environment.NewLine)));

                        // Lưu vào file lịch sử chat
                        File.AppendAllText(chatLogPath, fullMessage + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() => txtOutput.AppendText("Lỗi giải mã hoặc mất kết nối: " + ex.Message + "\n")));

                        // Kết thúc: Lưu lịch sử chat
                        LogEndChatSession();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => txtOutput.AppendText("Lỗi nhận tin nhắn: " + ex.Message + "\n")));
            }
        }



        private void LogConnectionHistory(string status)
        {
            string logFilePath = @"C:\Alice\Logs\Alice_ConnectionHistory.txt";
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {status}";
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }

        private void LogNewChatSession()
        {
            string chatLogPath = @"C:\Alice\Logs\Alice_ChatHistory.txt";
            Directory.CreateDirectory(Path.GetDirectoryName(chatLogPath));
            string header = $"==== PHIÊN CHAT MỚI ====\n" +
                            $"Thời gian: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                            "----------------------------------------\n";
            File.AppendAllText(chatLogPath, header);
        }

        private void LogEndChatSession()
        {
            string chatLogPath = @"C:\Alice\Logs\Alice_ChatHistory.txt";
            File.AppendAllText(chatLogPath, "--- Kết thúc phiên ---\n\n");
        }



        // Xóa tất cả khóa sau khi thoát chương trình
        private void DeleteKeyFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                txtOutput.AppendText($"Đã xóa: {filePath}\n");
            }
        }

        //*** BUTTON ***//

        // Button: Mở Server
        private void btnOpenServer_Click(object sender, EventArgs e)
        {
            server = new TcpListener(IPAddress.Any, 12345);
            server.Start();
            btnOpenServer.Enabled = false;
            this.Invoke(new Action(() => txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] Server đã mở, chờ Bob kết nối...\n" + Environment.NewLine)));
            server.BeginAcceptTcpClient(AcceptClient, null);
        }

        // Button: Gửi khóa
        private void btnSendKeys_Click(object sender, EventArgs e)
        {
            SendPrivateAndPublicKey();
        }

        // Button: Gửi tin nhắn
        private async void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra Alice đã nhận và lưu được khóa của Bob chưa
                if (!File.Exists(@"C:\Alice\Keys\Bob_PublicKey_Received.xml"))
                {
                    MessageBox.Show("Alice chưa nhận được public key của Bob!");
                    return;
                }

                // Đọc khóa của Bob 
                RSA rsaBobPublic = RSA.Create();
                rsaBobPublic.FromXmlString(File.ReadAllText(@"C:\Alice\Keys\Bob_PublicKey_Received.xml"));

                // Mã hóa tin nhắn
                string message = txtInput.Text;
                byte[] encryptedMessage = rsaBobPublic.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.Pkcs1);

                // Gửi thông điệp đã mã hóa tới Bob
                if (stream != null && stream.CanWrite)
                {
                    await stream.WriteAsync(encryptedMessage, 0, encryptedMessage.Length);
                    this.Invoke(new Action(() => txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] Alice: " + message + Environment.NewLine)));
                }
                else
                {
                    MessageBox.Show("Không thể gửi thông điệp tới Bob!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi gửi thông điệp: " + ex.Message);
            }
        }


        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            frmHistory historyForm = new frmHistory();
            historyForm.ShowDialog(); // hoặc Show() nếu bạn muốn mở nhiều lần
        }

        private void btnViewChatHistory_Click(object sender, EventArgs e)
        {
            frmChatHistory chatHistoryForm = new frmChatHistory();
            chatHistoryForm.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                isReceiving = false; // Ngắt vòng lặp nhận tin nhắn

                // Gửi thông báo ngắt kết nối nếu stream còn hoạt động
                if (stream != null && stream.CanWrite)
                {
                    try
                    {
                        string disconnectMessage = "DISCONNECT";
                        byte[] messageBytes = Encoding.UTF8.GetBytes(disconnectMessage);
                        stream.Write(messageBytes, 0, messageBytes.Length);
                        stream.Flush();
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() => txtOutput.AppendText(
                            "[" + DateTime.Now.ToString("HH:mm:ss") + "] Không thể gửi DISCONNECT (có thể Bob đã ngắt trước): " + ex.Message + Environment.NewLine)));

                    }
                }

                // Đóng kết nối an toàn
                try { stream?.Close(); } catch { }
                stream = null;

                try { client?.Close(); } catch { }
                client = null;

                // Xóa tất cả khóa
                DeleteKeyFile(@"C:\Alice\Keys\Alice_PrivateKey.xml");
                DeleteKeyFile(@"C:\Alice\Keys\Alice_PublicKey.xml");
                DeleteKeyFile(@"C:\Alice\Keys\Bob_PrivateKey_Received.xml");
                DeleteKeyFile(@"C:\Alice\Keys\Bob_PublicKey_Received.xml");
                DeleteKeyFile(@"C:\Alice\Keys\AliceFingerprint.txt");

                // Thoát chương trình
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thoát chương trình: " + ex.Message);
            }
        }
    }
}
