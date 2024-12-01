# Bước 1: Sử dụng hình ảnh Node.js để xây dựng ứng dụng
FROM node:20.3.0 AS build

# Tạo thư mục làm việc
WORKDIR /app

# Sao chép package.json và package-lock.json
COPY package*.json ./

# Xóa cache cũ và cài đặt các phụ thuộc
RUN npm cache clean --force && npm install --legacy-peer-deps

# Sao chép mã nguồn vào thư mục làm việc
COPY . .

# Debug: Kiểm tra phiên bản Node.js và NPM
RUN node --version && npm --version

# Xây dựng ứng dụng
RUN npm run build || cat /app/node_modules/rollup/dist/native.js

FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

# Expose cổng 80
EXPOSE 80

# Khởi động Nginx
CMD ["nginx", "-g", "daemon off;"]
