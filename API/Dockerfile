# Giai đoạn 1: Build (Sử dụng SDK lớn để biên dịch)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép và khôi phục các dependency
# Mục đích: Đảm bảo chỉ chạy dotnet restore khi tệp csproj thay đổi
COPY ["API.csproj", "."]
RUN dotnet restore

# Sao chép toàn bộ mã nguồn còn lại
COPY . .

# Biên dịch ứng dụng và tạo ra các tệp publish
# -c Release: Build ở chế độ Release (tối ưu)
# -o /app/publish: Đặt output vào thư mục /app/publish
RUN dotnet publish -c Release -o /app/publish

# ----------------------------------------------------------------------

# Giai đoạn 2: Final (Sử dụng Runtime nhỏ gọn để chạy)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Sao chép các tệp đã publish từ giai đoạn 'build'
COPY --from=build /app/publish .

# Thiết lập cổng mặc định cho ứng dụng ASP.NET Core trong Docker
ENV ASPNETCORE_URLS=http://+:80

# Lệnh chạy ứng dụng khi Container khởi động
ENTRYPOINT ["dotnet", "API.dll"]