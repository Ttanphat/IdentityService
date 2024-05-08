# Cài đặt
## CLI Tools
dotnet tool install -g dotnet-aspnet-codegenerator

dotnet tool install --global dotnet-ef
dotnet tool update dotnet-ef

# SQL SERVER Trên Docker
Vào thư mục sql-server-docker
Chuỗi kết nối đến SQL Server trong appsettings.json
"ConnectionStrings": {
    "AppMvcConnectionString" : "Data Source=localhost,1433; Initial Catalog=appmvc; User ID=SA;Password=Password123;TrustServerCertificate=True"
  },
Chạy lệnh docker-compose up -d

# Migration database
Cập nhật migration lên SQL Server
dotnet ef database update

# Đăng ký tài khoản
Sau khi đăng ký tài khoản, mail confirm sẽ nằm trong thư mục mailssave
Có thể sử dụng các ứng dụng mail như Outlook để mở và lấy link confirm

# ADMIN ACCOUNT:
username: admin
email: admin@example.com
password: admin123