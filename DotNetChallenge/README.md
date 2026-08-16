# DotNetChallenge

## 1. Giới thiệu

**DotNetChallenge** là một RESTful Web API được xây dựng bằng ASP.NET Core .NET 8, mô phỏng hệ thống quản lý bán hàng và kho hàng.

Project bao gồm các nghiệp vụ chính:

- Quản lý Customer và Supplier
- Quản lý Product, Category và Unit
- Quản lý tồn kho
- Nhập hàng từ Supplier thông qua Purchase Order
- Bán hàng cho Customer thông qua Sales Order
- Quản lý Payment
- Authentication bằng JWT
- Role & Permission
- Search, Filter và Pagination
- Report và Export
- Background Job
- Transaction handling
- Docker deployment
- Swagger API documentation

Project được phát triển theo từng Challenge từ Challenge 1 đến Challenge 12.

---

## 2. Công nghệ sử dụng

### Backend

- C#
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- PostgreSQL
- Npgsql
- LINQ
- JWT Authentication
- BCrypt
- FluentValidation
- Swagger / OpenAPI

### Database

- PostgreSQL 16
- Entity Framework Core Code First
- EF Core Migrations

### Deployment

- Docker
- Docker Compose

### Các thư viện chính

- `Microsoft.EntityFrameworkCore`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `BCrypt.Net-Next`
- `FluentValidation.AspNetCore`
- `Swashbuckle.AspNetCore`
- `EFCore.NamingConventions`

---

## 3. Cấu trúc project

```text
DotNetChallenge/
│
├── Controllers/
├── Data/
├── DTOs/
├── Exceptions/
├── Middleware/
├── Models/
│   ├── Entities/
│   └── Enums/
├── Services/
├── Validators/
├── Migrations/
│
├── Dockerfile
├── .dockerignore
├── docker-compose.yml
├── .env.example
├── .gitignore
├── Program.cs
└── DotNetChallenge.csproj
```

Project được tổ chức theo hướng tách Controller, Service, DTO, Entity, Validator và Data Access.

---

# 4. Cách chạy Local

## 4.1. Yêu cầu

Cần cài đặt:

- .NET 8 SDK
- PostgreSQL 16
- Git

Kiểm tra .NET:

```bash
dotnet --version
```

---

## 4.2. Clone project

```bash
git clone https://github.com/hnamn04/DotNetChallenge.git
cd DotNetChallenge/DotNetChallenge
```

---

## 4.3. Cấu hình database

Tạo PostgreSQL database:

```text
Database: dotnetchallenge
Username: postgres
Password: <your-password>
Port: 5432
```

Cấu hình connection string trong `appsettings.json` hoặc thông qua environment variable.

Không commit password hoặc secret thật lên GitHub.

---

## 4.4. Chạy migrations

Tại thư mục chứa `DotNetChallenge.csproj`:

```bash
dotnet ef database update
```

Nếu máy chưa có EF CLI:

```bash
dotnet tool install --global dotnet-ef
```

---

## 4.5. Chạy project

```bash
dotnet run
```

Sau khi application chạy, mở Swagger tại:

```text
http://localhost:<port>/swagger
```

Port thực tế phụ thuộc vào cấu hình launch profile của project.

---

# 5. Cách chạy bằng Docker

Project cung cấp:

- Dockerfile cho ASP.NET Core application
- Docker Compose cho application và PostgreSQL

Kiến trúc Docker:

```text
                 Docker Compose
                       │
          ┌────────────┴────────────┐
          │                         │
          ▼                         ▼
  DotNetChallenge App        PostgreSQL
      Container                Container
          │                         │
          └──────────────┬──────────┘
                         │
                  Docker Network
```

## 5.1. Environment variables

Tạo file `.env` ở thư mục chứa `docker-compose.yml`.

Ví dụ:

```env
POSTGRES_PASSWORD=your-password
```

Không commit `.env`.

Có thể sử dụng `.env.example` làm template:

```env
POSTGRES_PASSWORD=your-password-here
```

---

## 5.2. Build Docker image

```bash
docker compose build
```

---

## 5.3. Start containers

```bash
docker compose up -d
```

Kiểm tra:

```bash
docker compose ps
```

Cần có:

```text
dotnetchallenge-app
dotnetchallenge-postgres
```

ở trạng thái `Up`.

---

## 5.4. Xem application logs

```bash
docker compose logs app
```

Hoặc:

```bash
docker compose logs -f app
```

---

## 5.5. Swagger

Sau khi application container chạy:

```text
http://localhost:8080/swagger
```

## 5.6. Health Check

```text
GET /api/health
```

Ví dụ:

```text
http://localhost:8080/api/health
```

Response:

```json
{
  "success": true,
  "message": "API is running.",
  "data": {
    "status": "Healthy"
  }
}
```

---

# 6. Database Design

Database sử dụng PostgreSQL và được quản lý bằng Entity Framework Core Code First.

Các bảng chính:

```text
users
roles
user_roles

customers
suppliers

categories
units
products

inventory
stock_transactions

purchase_orders
purchase_order_items

sales_orders
sales_order_items

payments
```

## Quan hệ chính

### Product

Product thuộc:

```text
Category
Unit
```

và có quan hệ với:

```text
Inventory
PurchaseOrderItem
SalesOrderItem
StockTransaction
```

### Purchase Order

```text
Supplier
    │
    ▼
PurchaseOrder
    │
    ▼
PurchaseOrderItem
    │
    ▼
Product
```

### Sales Order

```text
Customer
    │
    ▼
SalesOrder
    │
    ▼
SalesOrderItem
    │
    ▼
Product
```

### Payment

```text
SalesOrder
    │
    ▼
Payment
```

### Authorization

```text
User
  │
  ▼
UserRole
  │
  ▼
Role
```

---

# 7. API Document

Swagger được sử dụng để document và test API.

Swagger UI:

```text
/swagger
```

Các nhóm API chính:

## Authentication

```text
POST /api/auth/register
POST /api/auth/login
GET  /api/auth/profile
```

Authentication sử dụng JWT Bearer Token.

---

## Customer

```text
POST /api/customers
GET  /api/customers
GET  /api/customers/{id}
PUT  /api/customers/{id}
DELETE /api/customers/{id}
```

Danh sách Customer hỗ trợ:

```text
search
page
limit
```

---

## Supplier

```text
POST /api/suppliers
GET  /api/suppliers
GET  /api/suppliers/{id}
PUT  /api/suppliers/{id}
DELETE /api/suppliers/{id}
```

---

## Product

```text
POST /api/products
GET  /api/products
GET  /api/products/{id}
PUT  /api/products/{id}
DELETE /api/products/{id}
```

Product listing hỗ trợ:

```text
search
categoryId
page
limit
```

---

## Inventory

```text
POST /api/inventory/import
POST /api/inventory/export
GET  /api/inventory/products/{productId}
GET  /api/inventory/transactions
```

---

## Purchase Order

```text
POST /api/purchase-orders
GET  /api/purchase-orders
GET  /api/purchase-orders/{id}

POST /api/purchase-orders/{id}/confirm
POST /api/purchase-orders/{id}/cancel
```

---

## Sales Order

```text
POST /api/sales-orders
GET  /api/sales-orders
GET  /api/sales-orders/{id}

POST /api/sales-orders/{id}/confirm
POST /api/sales-orders/{id}/cancel
```

---

## Payment

```text
POST /api/sales-orders/{id}/payments
GET  /api/sales-orders/{id}/payments
```

---

## Role

```text
GET  /api/roles
POST /api/users/{id}/roles
```

Role được sử dụng:

```text
ADMIN
STAFF
ACCOUNTANT
MANAGER
```

---

## Reports

```text
GET /api/reports/revenue
GET /api/reports/inventory-low-stock
GET /api/reports/sales/export
```

---

## Background Job

```text
POST /api/jobs/daily-summary/run
```

---

## Health

```text
GET /api/health
```

---

# 8. Purchase Order Flow

Purchase Order được sử dụng để xử lý nghiệp vụ nhập hàng từ Supplier.

Flow:

```text
Create Purchase Order
        │
        ▼
Validate Supplier
        │
        ▼
Validate Products
        │
        ▼
Create Purchase Order Items
        │
        ▼
Calculate Total Amount
        │
        ▼
Draft
        │
        ├──────────────► Cancel
        │
        ▼
Confirm
        │
        ▼
Increase Inventory
        │
        ▼
Create Stock Transactions
        │
        ▼
Completed
```

Các validation chính:

- Supplier phải tồn tại.
- Purchase Order phải có ít nhất một item.
- Quantity phải lớn hơn 0.
- Unit Price phải lớn hơn hoặc bằng 0.
- Product phải tồn tại.

Khi Confirm:

```text
Inventory.Quantity += PurchaseOrderItem.Quantity
```

Đồng thời tạo `StockTransaction` với loại:

```text
StockIn
```

Purchase Order không được Confirm hoặc Cancel nhiều lần gây sai dữ liệu.

---

# 9. Sales Order Flow

Sales Order được sử dụng để xử lý nghiệp vụ bán hàng cho Customer.

Flow:

```text
Create Sales Order
        │
        ▼
Validate Customer
        │
        ▼
Validate Product
        │
        ▼
Validate Quantity
        │
        ▼
Get Product Selling Price
        │
        ▼
Calculate Total Amount
        │
        ▼
Draft
        │
        ├──────────────► Cancel
        │
        ▼
Confirm
        │
        ▼
Check Inventory
        │
        ▼
Sufficient Stock?
      /     \
    No       Yes
    │         │
    ▼         ▼
 Reject    Decrease Inventory
              │
              ▼
        Create Stock Transaction
              │
              ▼
           Completed
```

Các validation chính:

- Customer phải tồn tại.
- Sales Order phải có ít nhất một item.
- Product phải tồn tại.
- Product phải đang active.
- Quantity phải lớn hơn 0.
- Total Amount được backend tính.
- Không được Confirm nếu tồn kho không đủ.
- Unit Price được lưu tại thời điểm bán.

---

# 10. Transaction Handling

EF Core transaction được sử dụng cho các nghiệp vụ có nhiều thao tác database liên quan với nhau.

Ví dụ khi Confirm Sales Order:

```text
Begin Transaction
       │
       ▼
Load Sales Order
       │
       ▼
Validate Order
       │
       ▼
Check Inventory
       │
       ▼
Decrease Inventory
       │
       ▼
Create Stock Transaction
       │
       ▼
Update Sales Order Status
       │
       ▼
Save Changes
       │
       ▼
Commit
```

Nếu bất kỳ bước nào xảy ra lỗi:

```text
Error
  │
  ▼
Rollback
  │
  ├── Inventory không bị trừ
  ├── Stock transaction không được lưu
  └── Sales order không bị cập nhật một phần
```

Payment cũng sử dụng transaction để đảm bảo dữ liệu thanh toán nhất quán.

Các ràng buộc chính:

- Payment amount phải lớn hơn 0.
- Tổng payment không được vượt quá `TotalAmount`.
- Payment status được cập nhật theo số tiền đã thanh toán:

```text
UNPAID
PARTIAL
PAID
```

---

# 11. Role & Permission

Hệ thống sử dụng JWT Authentication kết hợp Role-based Authorization.

Các role:

| Role | Permission |
|---|---|
| ADMIN | Toàn quyền |
| MANAGER | Xem report, duyệt đơn |
| STAFF | Tạo đơn bán, nhập hàng, xem tồn kho |
| ACCOUNTANT | Xem thanh toán, công nợ, báo cáo |

Flow:

```text
Login
  │
  ▼
JWT Token
  │
  ▼
Role Claims
  │
  ▼
Authorization
  │
  ▼
API
```

API có thể được bảo vệ bằng:

```csharp
[Authorize]
```

hoặc:

```csharp
[Authorize(Roles = "ADMIN")]
```

User chỉ có thể gọi các API mà role của user được phép truy cập.

---

# 12. Search, Filter & Pagination

Các API danh sách hỗ trợ pagination để tránh trả toàn bộ dữ liệu trong một request.

Ví dụ:

```text
GET /api/products?search=phone&categoryId={id}&page=1&limit=10
```

Response chứa pagination metadata:

```json
{
  "items": [],
  "pageIndex": 1,
  "pageSize": 10,
  "totalItems": 0,
  "totalPages": 0,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

Các API hỗ trợ:

```text
GET /api/products
GET /api/customers
GET /api/sales-orders
GET /api/purchase-orders
```

Search được xử lý không phân biệt chữ hoa/chữ thường.

---

# 13. Report & Background Job

## Revenue Report

```text
GET /api/reports/revenue
```

Báo cáo doanh thu theo khoảng thời gian.

---

## Low Stock Report

```text
GET /api/reports/inventory-low-stock
```

Báo cáo các sản phẩm có số lượng tồn kho thấp hơn threshold được cung cấp.

---

## Sales Export

```text
GET /api/reports/sales/export
```

Export dữ liệu Sales Report.

---

## Daily Summary Job

```text
POST /api/jobs/daily-summary/run
```

Job thực hiện tổng hợp dữ liệu hằng ngày.

Job có logging khi xảy ra lỗi để hỗ trợ theo dõi và debugging.

Flow:

```text
Trigger Job
     │
     ▼
Start Job
     │
     ▼
Collect Data
     │
     ▼
Calculate Summary
     │
     ▼
Log Result
     │
     ▼
Finish
```

Nếu job xảy ra exception:

```text
Exception
    │
    ▼
Log Error
    │
    ▼
Job Failed
```

---

# 14. Docker Architecture

Application được chạy trong một container riêng:

```text
dotnetchallenge-app
```

PostgreSQL được chạy trong container:

```text
dotnetchallenge-postgres
```

Docker Compose quản lý cả hai service.

```text
┌─────────────────────────────────────┐
│          Docker Compose             │
│                                     │
│  ┌──────────────────────────────┐   │
│  │ DotNetChallenge App          │   │
│  │ ASP.NET Core .NET 8          │   │
│  └──────────────┬───────────────┘   │
│                 │                   │
│                 ▼                   │
│  ┌──────────────────────────────┐   │
│  │ PostgreSQL 16                │   │
│  │ dotnetchallenge database     │   │
│  └──────────────────────────────┘   │
│                                     │
└─────────────────────────────────────┘
```

Application container kết nối PostgreSQL thông qua Docker Compose service name:

```text
Host=postgres
Port=5432
```

Không sử dụng `localhost` để kết nối PostgreSQL từ application container.

---

# 15. Security

Project sử dụng environment variables cho các thông tin nhạy cảm.

Ví dụ:

```env
POSTGRES_PASSWORD=your-password
```

File `.env` không được commit lên GitHub.

Project cung cấp:

```text
.env.example
```

để mô tả các biến môi trường cần thiết.

JWT secret và database password thật không được lưu trực tiếp trong source code.

---

# 16. Database Migration

Database sử dụng EF Core migrations.

Kiểm tra migrations:

```bash
dotnet ef migrations list
```

Update database:

```bash
dotnet ef database update
```

Migration được sử dụng để tạo và cập nhật database schema.

---

# 17. Testing / Demo

Có thể sử dụng Swagger để test các nghiệp vụ chính.

Recommended demo flow:

```text
1. Health Check
       ↓
2. Register / Login
       ↓
3. Authorize bằng JWT
       ↓
4. Create Product
       ↓
5. Create Supplier
       ↓
6. Create Purchase Order
       ↓
7. Confirm Purchase Order
       ↓
8. Check Inventory
       ↓
9. Create Customer
       ↓
10. Create Sales Order
       ↓
11. Confirm Sales Order
       ↓
12. Check Inventory
       ↓
13. Create Payment
       ↓
14. Check Payment Status
       ↓
15. View Reports
```

---

# 18. Challenge Summary

| Challenge | Nội dung |
|---|---|
| Challenge 1 | Database design |
| Challenge 2 | Customer & Supplier API |
| Challenge 3 | Product, Category & Unit |
| Challenge 4 | Inventory & Stock Transactions |
| Challenge 5 | Authentication & JWT |
| Challenge 6 | Purchase Order |
| Challenge 7 | Sales Order |
| Challenge 8 | Transaction & Payment |
| Challenge 9 | Search, Filter & Pagination |
| Challenge 10 | Role & Permission |
| Challenge 11 | Report, Export & Background Job |
| Challenge 12 | Docker & Final Documentation |

---

# 19. Conclusion

DotNetChallenge hoàn thiện một hệ thống Web API quản lý bán hàng và kho hàng với các thành phần:

- RESTful API
- PostgreSQL
- Entity Framework Core
- JWT Authentication
- Role-based Authorization
- Inventory Management
- Purchase Order
- Sales Order
- Payment
- Transaction
- Search / Filter / Pagination
- Report
- Background Job
- Swagger
- Docker

Project có thể chạy độc lập bằng .NET hoặc được triển khai bằng Docker Compose với ASP.NET Core application và PostgreSQL database.