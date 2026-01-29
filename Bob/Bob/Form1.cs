using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Bob
{
    public partial class Form1: Form
    {

        private RSA rsaBob;
        private TcpClient client;
        private NetworkStream stream;

        private bool isReceiving = true;

        public Form1()
        {
            InitializeComponent();
            InitializeKeys();
        }

        //*** FUNCTION ***//

        // Hàm khởi tạo khóa
        private void InitializeKeys()
        {
            rsaBob = RSA.Create();
            string folderPath = @"C:\Bob\Keys\";
            Directory.CreateDirectory(folderPath);

            string publicKeyPath = Path.Combine(folderPath, "Bob_PublicKey.xml");
            string privateKeyPath = Path.Combine(folderPath, "Bob_PrivateKey.xml");

            // Tạo khóa của Bob nếu chưa tồn tại
            if (!File.Exists(publicKeyPath))
            {
                File.WriteAllText(publicKeyPath, rsaBob.ToXmlString(false));
            }

            if (!File.Exists(privateKeyPath))
            {
                File.WriteAllText(privateKeyPath, rsaBob.ToXmlString(true));
            }
        }

        // Hàm gửi khóa công khai + riêng tư
        private async Task SendPrivateAndPublicKey()
        {
            try
            {
                if (client == null || stream == null || !stream.CanWrite)
                {
                    MessageBox.Show("Bob chưa kết nối với Alice!");
                    return;
                }

                // Send Public Key
                string publicKey = rsaBob.ToXmlString(false);
                byte[] keyBytes_public = Encoding.UTF8.GetBytes(publicKey);

                // Dùng await để gọi phương thức bất đồng bộ.
                await stream.WriteAsync(keyBytes_public, 0, keyBytes_public.Length);

                // Sử dụng FlushAsync thay vì Flush
                await stream.FlushAsync();

                // Send Private Key
                string privateKeyPath = @"C:\Bob\Keys\Bob_PrivateKey.xml";
                if (!File.Exists(privateKeyPath))
                {
                    MessageBox.Show("Không tìm thấy private key của Bob!");
                    return;
                }

                string privateKeyXml = File.ReadAllText(privateKeyPath);
                byte[] keyBytes_private = Encoding.UTF8.GetBytes(privateKeyXml);

                await stream.WriteAsync(keyBytes_private, 0, keyBytes_private.Length);
                await stream.FlushAsync(); 

                MessageBox.Show("Bob đã gửi khóa cho Alice!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi gửi khi gửi khóa: " + ex.Message);
            }
        }
                                                
        // Hàm nhận khóa công khai
        private async Task ReceivePublicKey()
        {
            try
            {
                if (stream == null || !stream.CanRead)
                {
                    MessageBox.Show("Bob không thể đọc dữ liệu từ Alice!");
                    return;
                }

                byte[] buffer = new byte[1024];  // Kích thước 1024 byte
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    string publicKeyXml = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    if (!publicKeyXml.StartsWith("<RSAKeyValue>"))
                    {
                        MessageBox.Show("Dữ liệu nhận được không phải public key hợp lệ từ Alice!");
                        return;
                    }

                    // Kiểm tra fingerprint trước khi tiếp tục
                    if (!VerifyAlicePublicKeyFingerprint(publicKeyXml))
                    {
                        MessageBox.Show("Mã fingerprint của Alice không khớp! Có thể bị tấn công MITM!",
                            "Cảnh báo bảo mật", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Ngắt kết nối nếu Fingerprint không khớp.
                        stream?.Close();
                        stream = null;
                        client?.Close();
                        client = null;

                        this.Invoke(new Action(() => txtOutput.AppendText(
                            "[" + DateTime.Now.ToString("HH:mm:ss") + "] Kết nối bị hủy do fingerprint không hợp lệ.\n")));

                        return;
                    }

                    // Xác thực Fingerprint thành công: Nhận khóa công khai từ Alice
                    string savePath = @"C:\Bob\Keys\Alice_PublicKey_Received.xml";
                    File.WriteAllText(savePath, publicKeyXml);
                    this.Invoke(new Action(() => txtOutput.AppendText(
                        "[" + DateTime.Now.ToString("HH:mm:ss") + "] Bob đã nhận được public key từ Alice!\n" +
                        Environment.NewLine)));
                }
                else
                {
                    MessageBox.Show("Bob không nhận được khóa từ Alice!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nhận public key từ Alice: " + ex.Message);
            }
        }

        private async Task ReceivePrivateKey()
        {
            try
            {
                if (stream == null || !stream.CanRead)
                {
                    MessageBox.Show("Bob không thể đọc dữ liệu!");
                    return;
                }

                byte[] buffer = new byte[2048];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    string privateKeyXml = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    File.WriteAllText(@"C:\Bob\Keys\Alice_PrivateKey_Received.xml", privateKeyXml);
                    this.Invoke(new Action(() => txtOutput.AppendText(
                        "[" + DateTime.Now.ToString("HH:mm:ss") + "]" +
                        " Bob đã nhận được private key từ Alice!\n" + 
                        Environment.NewLine)));
                }
                else
                {
                    MessageBox.Show("Bob không nhận được private key từ Alice!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nhận private key: " + ex.Message);
            }
        }
        private async Task ReceiveEncryptedMessage()
        {
            try
            {
                string privateKeyPath = @"C:\Bob\Keys\Bob_PrivateKey.xml";
                if (!File.Exists(privateKeyPath))
                {
                    this.Invoke(new Action(() => txtOutput.AppendText("Không tìm thấy private key của Bob!\n")));
                    return;
                }

                RSA rsaBobPrivate = RSA.Create();
                rsaBobPrivate.FromXmlString(File.ReadAllText(privateKeyPath));

                // Bắt đầu: Lưu lịch sử chat
                string chatLogPath = @"C:\Bob\Logs\Bob_ChatHistory.txt";
                Directory.CreateDirectory(Path.GetDirectoryName(chatLogPath));
                LogNewChatSession();

                while (true) // Vòng lặp lắng nghe liên tục
                {
                    if (stream == null || !stream.CanRead)
                    {
                        string disconnectMsg = $"[{DateTime.Now:HH:mm:ss}] Alice đã ngắt kết nối!\n";
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
                            "[" + DateTime.Now.ToString("HH:mm:ss") + "] Kết nối đã bị đóng từ phía Alice.\n")));
                        LogEndChatSession();
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        this.Invoke(new Action(() => txtOutput.AppendText(
                            "[" + DateTime.Now.ToString("HH:mm:ss") + "] Stream đã bị đóng đột ngột.\n")));
                        LogEndChatSession();
                        break;
                    }

                    if (bytesRead == 0)
                    {
                        this.Invoke(new Action(() => txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] Alice đã ngắt kết nối!\n")));
                        LogEndChatSession();
                        return;
                    }

                    try
                    {
                        byte[] encryptedMessage = buffer.Take(bytesRead).ToArray();
                        byte[] decryptedMessage = rsaBobPrivate.Decrypt(encryptedMessage, RSAEncryptionPadding.Pkcs1);
                        string decryptedMessageString = Encoding.UTF8.GetString(decryptedMessage);

                        string timestamp = "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
                        string fullMessage = $"{timestamp} Alice: {decryptedMessageString}";

                        // Hiển thị trên giao diện
                        this.Invoke(new Action(() => txtOutput.AppendText(fullMessage + Environment.NewLine)));

                        // Ghi vào file lịch sử chat
                        File.AppendAllText(chatLogPath, fullMessage + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() => txtOutput.AppendText("Lỗi giải mã hoặc mất kết nối: " + ex.Message + "\n")));
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


        private void LogNewChatSession()
        {
            string chatLogPath = @"C:\Bob\Logs\Bob_ChatHistory.txt";
            Directory.CreateDirectory(Path.GetDirectoryName(chatLogPath));
            string header = $"==== PHIÊN CHAT MỚI ====\n" +
                            $"Thời gian: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                            "----------------------------------------\n";
            File.AppendAllText(chatLogPath, header);
        }

        private void LogEndChatSession()
        {
            string chatLogPath = @"C:\Bob\Logs\Bob_ChatHistory.txt";
            File.AppendAllText(chatLogPath, "--- Kết thúc phiên ---\n\n");
        }

        private void DeleteKeyFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                txtOutput.AppendText($"Đã xóa: {filePath}\n");
            }
        }

        private bool VerifyAlicePublicKeyFingerprint(string receivedPublicKeyXml)
        {
            try
            {
                // Giải mã SHA-256 fingerprint của khóa vừa nhận
                SHA256 sha256 = SHA256.Create();
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(receivedPublicKeyXml));
                sha256.Dispose();

                string calculatedFingerprint = BitConverter.ToString(hashBytes).Replace("-", ":");

                // Hiển thị fingerprint đã tính (Nếu muốn)
                this.Invoke(new Action(() => txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] Fingerprint tính từ khóa nhận được: \n" + calculatedFingerprint + "\r\n")));

                // Yêu câu nhập Fingerprint (Fingerprint của Alice)
                string expectedFingerprint = Microsoft.VisualBasic.Interaction.InputBox(
                    "Nhập fingerprint mà Alice gửi bạn qua kênh an toàn:",
                    "Xác minh fingerprint Alice"
                );

                // So sánh Fingerprint Alice và hiện tại
                if (calculatedFingerprint == expectedFingerprint)
                {
                    this.Invoke(new Action(() => txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] Fingerprint khớp! Khóa từ Alice là hợp lệ.\r\n")));
                    return true; // Trả về true nếu fingerprint khớp
                }
                else
                {
                    this.Invoke(new Action(() => txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] CẢNH BÁO: Fingerprint KHÔNG khớp! Có thể bị MITM!\r\n")));
                    MessageBox.Show("Cảnh báo: Khóa nhận từ Alice không hợp lệ. Có thể đang bị MITM!", "MITM Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false; // Trả về false nếu fingerprint không khớp
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xác minh fingerprint: " + ex.Message);
                return false; // Trả về false khi có lỗi
            }
        }

        private void LogConnectionHistory(string status)
        {
            string logFilePath = @"C:\Bob\Logs\Bob_ConnectionHistory.txt";
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {status}";
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }

        // BUTTON

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            var endpoints = new List<(string ip, int port)>
            {
                ("192.168.1.230", 54321),  // T-Watcher
                ("192.168.1.230", 12345),   //  Alice-Lan
                ("192.168.1.229", 54321),  // T-Watcher
                ("192.168.1.229", 12345),   //  Alice-Lan
                ("192.168.76.136", 12345) // Alice-VW
            };

            bool connected = false;

            //txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đang kết nối tới Alice...\n");
            this.Invoke(new Action(() => txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] Đang kết nối tới Alice...\n" + Environment.NewLine)));

            foreach (var endpoint in endpoints)
            {
                try
                {
                    TcpClient testClient = new TcpClient();
                    var connectTask = testClient.ConnectAsync(endpoint.ip, endpoint.port);
                    var timeoutTask = Task.Delay(3000);

                    var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                    if (completedTask == timeoutTask || !testClient.Connected)
                        continue;

                    client = testClient;
                    stream = client.GetStream();
                    connected = true;

                    //txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Kết nối thành công đến Alice!\n\n");
                    this.Invoke(new Action(() => txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] Kết nối thành công đến Alice!\n" + Environment.NewLine)));
                    btnConnect.Enabled = false;

                    await ReceivePublicKey();
                    await ReceivePrivateKey();
                    await ReceiveEncryptedMessage();

                    LogConnectionHistory("Alice đã kết nối.");
                    LogConnectionHistory("Alice đã ngắt kết nối.\n");

                    break;
                }
                catch
                {
                    continue;
                }
            }

            if (!connected)
            {
                txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] ❌ Không thể kết nối đến Alice!\n");
                MessageBox.Show("Không tìm thấy cổng Alice nào đang mở!", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem khóa công khai của Alice đã được nhận chưa
                if (!File.Exists(@"C:\Bob\Keys\Alice_PublicKey_Received.xml"))
                {
                    MessageBox.Show("Bob chưa nhận được public key của Alice!");
                    return;
                }

                // Đọc khóa công khai của Alice
                RSA rsaAlicePublic = RSA.Create();
                rsaAlicePublic.FromXmlString(File.ReadAllText(@"C:\Bob\Keys\Alice_PublicKey_Received.xml"));

                // Mã hóa thông điệp cần gửi
                string message = txtInput.Text;  // Thông điệp cần mã hóa
                byte[] encryptedMessage = rsaAlicePublic.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.Pkcs1);

                // Gửi thông điệp đã mã hóa tới Alice qua stream
                if (stream != null && stream.CanWrite)
                {
                    await stream.WriteAsync(encryptedMessage, 0, encryptedMessage.Length);
                    //MessageBox.Show("Thông điệp đã được mã hóa và gửi tới Alice.");
                    this.Invoke(new Action(() => txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] Bob: " + message + Environment.NewLine)));
                }
                else
                {
                    MessageBox.Show("Không thể gửi thông điệp tới Alice!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi gửi thông điệp: " + ex.Message);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                // Gửi thông báo ngắt kết nối cho Alice
                if (stream != null && stream.CanWrite)
                {
                    string disconnectMessage = "DISCONNECT";
                    byte[] messageBytes = Encoding.UTF8.GetBytes(disconnectMessage);
                    stream.Write(messageBytes, 0, messageBytes.Length);
                    stream.Flush();
                }

                // Đóng kết nối
                stream?.Close();
                stream = null;
                client?.Close();
                client = null;

                // Xóa tất cả khóa
                DeleteKeyFile(@"C:\Bob\Keys\Bob_PrivateKey.xml");
                DeleteKeyFile(@"C:\Bob\Keys\Bob_PublicKey.xml");
                DeleteKeyFile(@"C:\Bob\Keys\Alice_PrivateKey_Received.xml");
                DeleteKeyFile(@"C:\Bob\Keys\Alice_PublicKey_Received.xml");

                // Thoát chương trình
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thoát chương trình: " + ex.Message);
            }
        }
        private async void btnSendKeys_Click(object sender, EventArgs e)
        {
            await SendPrivateAndPublicKey();
        }

        private void btnViewChatHistory_Click(object sender, EventArgs e)
        {
            frmChatHistory chatHistoryForm = new frmChatHistory();
            chatHistoryForm.ShowDialog();
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            frmHistory historyForm = new frmHistory();
            historyForm.ShowDialog(); // hoặc Show() nếu bạn muốn mở nhiều lần
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            string folderPath = @"C:\Bob\Keys";
            if (Directory.Exists(folderPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", folderPath);
            }
            else
            {
                MessageBox.Show("Thư mục không tồn tại: " + folderPath);
            }
        }

    }
}
