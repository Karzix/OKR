# Bước 1: Sử dụng Node.js để xây dựng ứng dụng Vue
FROM node:20.3.0 AS build

# Tạo thư mục làm việc trong container
WORKDIR /app

# Sao chép package.json và package-lock.json vào thư mục làm việc
COPY package*.json ./

# Cài đặt các phụ thuộc
RUN npm install --legacy-peer-deps

# Sao chép mã nguồn vào thư mục làm việc
COPY . .

# Xây dựng ứng dụng Vue
RUN npm run build

# Bước 2: Sử dụng Nginx để phục vụ ứng dụng đã xây dựng
FROM nginx:alpine

# Copy cấu hình Nginx (nếu có)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Sao chép các tệp đã xây dựng từ bước 1 vào thư mục phục vụ của Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Expose cổng 80
EXPOSE 80

# Khởi động Nginx
CMD ["nginx", "-g", "daemon off;"]
