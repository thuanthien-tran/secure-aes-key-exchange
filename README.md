============ Secure AES Key Exchange Defense System ============ 
Hệ thống mô phỏng trao đổi khóa an toàn giữa hai thực thể (Alice – Bob) sử dụng RSA và Diffie–Hellman, kết hợp cơ chế fingerprint verification để phát hiện và ngăn chặn Man-in-the-Middle (MITM).
Dự án được xây dựng nhằm minh họa rủi ro trong trao đổi khóa và đánh giá hiệu quả của các biện pháp phòng thủ.

Hệ thống gồm 3 thành phần chính:
- Alice (Server): Khởi tạo kết nối, tạo khóa RSA, xác thực Bob, mã hóa và gửi dữ liệu
- Bob (Client): Kết nối tới Alice, xác minh fingerprint, giải mã và trao đổi dữ liệu
- TWatcher (MITM Simulator): Mô phỏng tấn công MITM để kiểm chứng cơ chế phòng thủ

MỤC TIÊU DỰ ÁN:
- Phân tích quy trình trao đổi khóa
- Minh họa kịch bản MITM
- Đánh giá vai trò của fingerprint verification trong việc phát hiện giả mạo khóa

KIẾN TRÚC HỆ THỐNG:
 Bob  <———>  Alice
   \        /
    \      /
     TWatcher (MITM)

THÀNH PHẦN CHI TIẾT
** Alice:
  - Chạy TCP server (port 12345)
  - Tạo cặp khóa RSA
  - Xuất public key + fingerprint SHA-256
  - Thực hiện: Gửi/nhận khóa, chat mã hóa RSA, ghi log lịch sử kết nối & trao đổi
  - Lưu trữ khóa
** Bob:
  - Kết nối TCP tới Alice
  - Tạo cặp khóa RSA riêng
  - Nhận public key của Alice và xác minh fingerprint
  - Từ chối kết nối nếu fingerprint không khớp
  - Thực hiện: Giải mã dữ liệu RSA, ghi log lịch sử chat & kết nối
  - Lưu trữ khóa
** T-Watcher (MITM):
  - Đóng vai trò Man-in-the-Middle
  - Hoạt động: Kết nối tới Alice, mở cổng giả cho Bob
  - Tạo RSA key giả mạo cho cả hai phía
  - Chặn và lưu: Public/private key thật, dữ liệu mã hóa
  - Khả năng: Giải mã dữ liệu nếu fingerprint không được kiểm tra, hiển thị lịch sử chat MITM
  - Lưu trữ khóa thật
    (TWatcher chỉ phục vụ mục đích nghiên cứu và mô phỏng tấn công, không dùng trong môi trường thực tế)

CƠ CHẾ PHÒNG THỦ:
** Fingerprint Verification:
  - Public key được băm bằng SHA-256
  - Fingerprint được trao đổi qua kênh tin cậy ngoài băng
  - Bob/Alice so sánh fingerprint trước khi chấp nhận khóa
  => Nếu MITM thay thế khóa: Fingerprint không khớp → Kết nối bị từ chối

QUY TRÌNH HOẠT ĐỘNG:
1. Alice tạo RSA keypair → sinh fingerprint
2. Bob nhận public key + fingerprint
3. Bob xác minh fingerprint
4. Nếu hợp lệ → bắt đầu trao đổi dữ liệu mã hóa
5. Nếu có MITM:
   - Không kiểm tra fingerprint → MITM thành công
   - Có kiểm tra fingerprint → MITM bị phát hiện
  
============ CÁCH CÀI ĐẬT ============
1. Chạy Alice
  - Build & chạy project Alice
  - Server lắng nghe tại port 12345
2. Chạy Bob
  - Nhập IP Alice
  - Nhập fingerprint public key của Alice
  - Thực hiện kết nối và chat mã hóa
3. Mô phỏng MITM (T-Watcher)
  - Chạy TWatcher
  - Bob kết nối tới TWatcher thay vì Alice
  - Quan sát:
      - Trường hợp không kiểm tra fingerprint → MITM đọc được dữ liệu
      - Trường hợp có kiểm tra fingerprint → kết nối bị chặn

  OUTPUT & LOGGING:
  - Lịch sử kết nối
  - Lịch sử chat (mã hóa / giải mã)
  - Log khóa và fingerprint
  - Giao diện WinForms hiển thị trạng thái từng bước

HẠN CHẾ HIỆN TẠI/ HƯỚNG PHÁT TRIỂN:
- Chưa triển khai chứng thực CA/PKI
- Chưa có xác thực hai chiều nâng cao
- Chưa tích hợp sandbox mạng
- Có thể mở rộng: TLS-like handshake hoặc AI-based anomaly detection cho key exchange













  
