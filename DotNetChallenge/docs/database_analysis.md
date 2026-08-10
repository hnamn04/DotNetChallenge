# Database Analysis

## 1. Overview

This project uses **PostgreSQL** as the relational database management system and **Entity Framework Core Code First** with migrations for database schema management.

The database is designed for an inventory and order management system.

It supports the following main functions:

- User and role management.
- Authentication and authorization support.
- Customer management.
- Supplier management.
- Product and category management.
- Unit management.
- Inventory management.
- Purchase order management.
- Sales order management.
- Payment tracking.
- Stock transaction history.

The database contains the following required entities:

- `users`
- `roles`
- `user_roles`
- `customers`
- `suppliers`
- `categories`
- `products`
- `units`
- `inventory`
- `purchase_orders`
- `purchase_order_items`
- `sales_orders`
- `sales_order_items`
- `payments`
- `stock_transactions`

---

# 2. Database Design Principles

The database design follows several important principles:

- Each entity represents a clear business concept.
- Primary keys are used to uniquely identify records.
- Foreign keys maintain relationships between entities.
- Unique constraints prevent duplicate business data.
- Indexes improve query performance.
- Current inventory and stock transaction history are stored separately.
- Junction tables are used for many-to-many relationships.
- Order header and order detail tables are separated.
- Referential integrity is maintained through foreign key constraints.
- Business entities are separated from authentication and authorization entities.

---

# 3. User and Customer Separation

The `users` and `customers` entities represent different concepts and should not be considered the same entity.

## 3.1 Users

The `users` table represents people who can access and use the system.

Examples include:

- Administrator.
- Manager.
- Warehouse staff.
- Sales staff.

A user can authenticate with the system and can be assigned one or more roles.

Typical user information includes:

```text
id
username
email
password_hash
is_active
created_at
updated_at
```

The `users` entity is primarily used for:

- Authentication.
- Authorization.
- JWT authentication.
- Role assignment.
- System access management.

---

## 3.2 Customers

The `customers` table represents customers in the sales business domain.

A customer does not necessarily have a system account.

Examples include:

- An individual customer.
- A company.
- A retail customer.
- A business customer.

Typical customer information includes:

```text
id
name
email
phone
address
created_at
updated_at
```

The `customers` entity is primarily used for:

- Creating sales orders.
- Storing customer contact information.
- Tracking customer purchase history.

---

## 3.3 Relationship Between Users and Customers

In the current database design, `users` and `customers` are independent entities.

A customer does not need to be a system user.

For example:

```text
Admin User
    ↓
Logs into the system
    ↓
Creates a sales order
    ↓
Customer purchases products
```

Therefore:

```text
users
    = people who operate the system

customers
    = people or organizations who buy products
```

If the system later supports customer registration and online ordering, a relationship can be added between `users` and `customers`.

For example:

```text
users
    1
    |
    | 0..1
    |
customers
```

In that case, the `customers` table could contain a nullable or unique `user_id` foreign key.

However, this relationship is not required in the current Challenge 01 design.

---

# 4. Entity Relationships

## 4.1 User and Role

The relationship between `users` and `roles` is many-to-many.

A user can have multiple roles.

A role can be assigned to multiple users.

The `user_roles` table is used as the junction table.

Relationship:

```text
users 1 --- N user_roles N --- 1 roles
```

Example:

```text
User: admin

Roles:
- Admin
- Manager
```

The composite primary key of `user_roles` is:

```text
user_id + role_id
```

This prevents the same role from being assigned multiple times to the same user.

---

## 4.2 Category and Product

A category can contain multiple products.

Each product belongs to one category.

Relationship:

```text
categories 1 --- N products
```

Example:

```text
Electronics
    ├── Laptop
    ├── Mouse
    └── Keyboard
```

The category name should be unique.

---

## 4.3 Unit and Product

A unit can be used by multiple products.

Each product belongs to one unit.

Relationship:

```text
units 1 --- N products
```

Examples of units:

- Piece
- Kilogram
- Bottle
- Box

Example:

```text
Laptop
Unit: Piece

Apple
Unit: Kilogram
```

The `name` and `symbol` fields should be unique.

---

## 4.4 Product and Inventory

Each product has one inventory record.

Each inventory record belongs to one product.

Relationship:

```text
products 1 --- 1 inventory
```

The `product_id` column in the `inventory` table is unique.

This ensures that one product cannot have multiple current inventory records.

The inventory table stores current stock information:

```text
quantity
reserved_quantity
```

Available stock can be calculated as:

```text
available_quantity = quantity - reserved_quantity
```

Example:

```text
quantity = 100
reserved_quantity = 20

available_quantity = 80
```

---

## 4.5 Supplier and Purchase Order

A supplier can have multiple purchase orders.

Each purchase order belongs to one supplier.

Relationship:

```text
suppliers 1 --- N purchase_orders
```

Example:

```text
ABC Electronics Supplier
    ├── PO-2026-0001
    ├── PO-2026-0002
    └── PO-2026-0003
```

---

## 4.6 Purchase Order and Purchase Order Item

A purchase order contains multiple items.

Each purchase order item belongs to one purchase order.

Relationship:

```text
purchase_orders 1 --- N purchase_order_items
```

Each purchase order item references one product:

```text
products 1 --- N purchase_order_items
```

The relationship can be visualized as:

```text
Supplier
    ↓
Purchase Order
    ↓
Purchase Order Items
    ↓
Products
```

The `purchase_orders` table stores general order information.

Examples:

```text
order_number
supplier_id
order_date
status
total_amount
```

The `purchase_order_items` table stores individual products in the order.

Examples:

```text
purchase_order_id
product_id
quantity
unit_price
total_price
```

---

## 4.7 Customer and Sales Order

A customer can have multiple sales orders.

Each sales order belongs to one customer.

Relationship:

```text
customers 1 --- N sales_orders
```

Example:

```text
Nguyen Van A
    ├── SO-2026-0001
    └── SO-2026-0002
```

The `sales_orders` table stores general sales order information.

Examples:

```text
order_number
customer_id
order_date
status
total_amount
```

---

## 4.8 Sales Order and Sales Order Item

A sales order contains multiple products.

Each sales order item belongs to one sales order.

Relationship:

```text
sales_orders 1 --- N sales_order_items
```

Each sales order item references one product:

```text
products 1 --- N sales_order_items
```

Example:

```text
Sales Order
    ├── Laptop x 1
    ├── Mouse x 2
    └── Keyboard x 1
```

The `sales_order_items` table contains:

```text
sales_order_id
product_id
quantity
unit_price
total_price
```

---

## 4.9 Sales Order and Payment

A sales order can have one or multiple payments.

Relationship:

```text
sales_orders 1 --- N payments
```

This design supports:

- Full payment.
- Partial payment.
- Multiple payment attempts.
- Different payment methods.

Example:

```text
Sales Order Total: 10,000,000

Payment 1: 5,000,000
Payment 2: 5,000,000
```

The payment information may include:

```text
sales_order_id
amount
method
status
paid_at
```

---

## 4.10 Product and Stock Transaction

A product can have multiple stock transactions.

Relationship:

```text
products 1 --- N stock_transactions
```

Stock transactions are used to track the history of inventory changes.

Examples of transaction types include:

```text
StockIn
StockOut
Adjustment
```

Examples:

```text
Purchase Order Completed
    ↓
StockIn

Sales Order Completed
    ↓
StockOut

Manual Correction
    ↓
Adjustment
```

Each stock transaction can optionally reference another business document using:

```text
reference_type
reference_id
```

Example:

```text
reference_type = PurchaseOrder
reference_id = purchase_order_id
```

or:

```text
reference_type = SalesOrder
reference_id = sales_order_id
```

---

# 5. Entity Relationship Summary

The complete relationship structure is:

```text
USERS
  |
  | 1:N
  |
USER_ROLES
  |
  | N:1
  |
ROLES


CATEGORIES
  |
  | 1:N
  |
PRODUCTS
  |
  | N:1
  |
UNITS


PRODUCTS
  |
  | 1:1
  |
INVENTORY


SUPPLIERS
  |
  | 1:N
  |
PURCHASE_ORDERS
  |
  | 1:N
  |
PURCHASE_ORDER_ITEMS
  |
  | N:1
  |
PRODUCTS


CUSTOMERS
  |
  | 1:N
  |
SALES_ORDERS
  |
  | 1:N
  |
SALES_ORDER_ITEMS
  |
  | N:1
  |
PRODUCTS


SALES_ORDERS
  |
  | 1:N
  |
PAYMENTS


PRODUCTS
  |
  | 1:N
  |
STOCK_TRANSACTIONS
```

---

# 6. Primary Keys

Most entities use UUID as the primary key.

Examples:

```text
users.id
roles.id
customers.id
suppliers.id
categories.id
units.id
products.id
inventory.id
purchase_orders.id
purchase_order_items.id
sales_orders.id
sales_order_items.id
payments.id
stock_transactions.id
```

UUID provides globally unique identifiers.

The benefits include:

- Reduced risk of ID collisions.
- Easier data integration between systems.
- IDs are not sequential.
- Suitable for distributed applications.

The `user_roles` table uses a composite primary key:

```text
user_id + role_id
```

---

# 7. Foreign Keys

Foreign keys are used to maintain referential integrity.

Important relationships include:

```text
user_roles.user_id
    -> users.id

user_roles.role_id
    -> roles.id

products.category_id
    -> categories.id

products.unit_id
    -> units.id

inventory.product_id
    -> products.id

purchase_orders.supplier_id
    -> suppliers.id

purchase_order_items.purchase_order_id
    -> purchase_orders.id

purchase_order_items.product_id
    -> products.id

sales_orders.customer_id
    -> customers.id

sales_order_items.sales_order_id
    -> sales_orders.id

sales_order_items.product_id
    -> products.id

payments.sales_order_id
    -> sales_orders.id

stock_transactions.product_id
    -> products.id
```

These foreign keys prevent invalid references.

For example:

```text
A sales_order_item cannot reference a product that does not exist.
```

---

# 8. Unique Constraints

Unique constraints are used to prevent duplicate business data.

Important unique fields include:

```text
users.username
users.email

roles.name

categories.name

units.name
units.symbol

products.code

purchase_orders.order_number

sales_orders.order_number
```

The inventory table also contains a unique constraint:

```text
inventory.product_id
```

This guarantees the one-to-one relationship between products and inventory.

The `user_roles` composite primary key also prevents duplicate user-role assignments.

---

# 9. Indexes

Indexes are used to improve performance for frequently searched and joined columns.

Important indexed columns may include:

```text
products.code
products.name
products.category_id
products.unit_id

customers.email
customers.phone

suppliers.email
suppliers.phone

purchase_orders.order_number
purchase_orders.supplier_id

sales_orders.order_number
sales_orders.customer_id

payments.sales_order_id

stock_transactions.product_id
stock_transactions.type
```

Composite indexes can also be used for common business queries.

Examples:

```text
purchase_orders
    supplier_id + order_date

sales_orders
    customer_id + order_date

stock_transactions
    product_id + created_at
```

These indexes can improve queries such as:

- Find purchase orders by supplier and date.
- Find sales orders by customer and date.
- Find inventory transaction history for a product.

---

# 10. Data Integrity Rules

The database design should enforce data integrity through constraints.

Important rules include:

```text
quantity >= 0

reserved_quantity >= 0

reserved_quantity <= quantity

cost_price >= 0

selling_price >= 0

purchase_order_item.quantity > 0

sales_order_item.quantity > 0

payment.amount > 0
```

These rules prevent invalid data.

Examples:

```text
Invalid:
quantity = -10

Invalid:
reserved_quantity = 100
quantity = 50

Invalid:
sales_order_item.quantity = 0
```

---

# 11. Delete Behavior

Delete behavior should be configured according to the relationship between entities.

## 11.1 Cascade Delete

Cascade delete is appropriate for child records that should not exist without their parent.

Examples:

```text
users
    -> user_roles

roles
    -> user_roles

purchase_orders
    -> purchase_order_items

sales_orders
    -> sales_order_items
```

---

## 11.2 Restrict Delete

Restrict delete is appropriate for important business data that is still referenced.

Examples:

```text
categories
    -> products

units
    -> products

suppliers
    -> purchase_orders

customers
    -> sales_orders

products
    -> purchase_order_items

products
    -> sales_order_items

products
    -> stock_transactions
```

This prevents accidental deletion of important data.

For example:

```text
A product should not be deleted if it already appears in a sales order.
```

---

# 12. Inventory Design

The inventory system separates current stock information from stock history.

## 12.1 Current Inventory

The `inventory` table stores the current inventory state.

Fields include:

```text
quantity
reserved_quantity
```

Example:

```text
Product: Laptop

quantity = 50
reserved_quantity = 5
available_quantity = 45
```

---

## 12.2 Stock Transaction History

The `stock_transactions` table stores every important inventory movement.

Examples:

```text
StockIn
StockOut
Adjustment
```

Example flow:

```text
Purchase Order
    ↓
StockIn Transaction
    ↓
Inventory Quantity Increases


Sales Order
    ↓
StockOut Transaction
    ↓
Inventory Quantity Decreases
```

This design allows the system to provide:

- Fast access to current inventory.
- Historical stock tracking.
- Auditing of inventory changes.
- Investigation of stock differences.

---

# 13. Purchase Order Design

The purchase order structure is divided into two tables:

```text
purchase_orders
purchase_order_items
```

The `purchase_orders` table represents the order header.

Example:

```text
PO-2026-0001

Supplier: ABC Electronics
Date: 2026-08-10
Status: Completed
Total: 15,250,000
```

The `purchase_order_items` table represents the products inside the order.

Example:

```text
Laptop
Quantity: 1
Unit Price: 15,000,000

Mouse
Quantity: 1
Unit Price: 250,000
```

This structure follows the standard order header and order detail pattern.

---

# 14. Sales Order Design

The sales order structure is divided into:

```text
sales_orders
sales_order_items
```

The `sales_orders` table represents the sales order header.

The `sales_order_items` table represents the products included in the order.

Example:

```text
SO-2026-0001

Customer: Nguyen Van A

Items:
- Laptop x 1
- Mouse x 1
```

The relationship is:

```text
Customer
    ↓
Sales Order
    ↓
Sales Order Items
    ↓
Products
```

This design supports multiple products in a single sales order.

---

# 15. Payment Design

The `payments` table stores payment information related to sales orders.

Relationship:

```text
sales_orders 1 --- N payments
```

This supports different payment scenarios.

Example:

```text
Sales Order Total: 10,000,000

Payment 1:
5,000,000

Payment 2:
5,000,000
```

Payment methods may include:

```text
Cash
BankTransfer
CreditCard
```

Payment statuses may include:

```text
Pending
Paid
Failed
Refunded
```

---

# 16. Status and Transaction Type Management

The system contains several fields representing business states.

Examples:

```text
PurchaseOrderStatus
SalesOrderStatus
PaymentStatus
StockTransactionType
```

These can be represented in C# using enums.

For example:

```text
PurchaseOrderStatus
- Pending
- Approved
- Completed
- Cancelled
```

```text
SalesOrderStatus
- Pending
- Confirmed
- Completed
- Cancelled
```

```text
PaymentStatus
- Pending
- Paid
- Failed
- Refunded
```

```text
StockTransactionType
- StockIn
- StockOut
- Adjustment
```

Using enums helps prevent invalid status values in application code.

---

# 17. Sample Data Strategy

Sample data is stored separately from the database schema.

Schema script:

```text
database/init.sql
```

Sample data:

```text
database/sample_data.sql
```

This separation allows the database to be recreated in two independent steps.

Step 1:

```text
Create database schema
```

Step 2:

```text
Insert sample data
```

The sample data includes examples of:

- Roles.
- Users.
- User role assignments.
- Customers.
- Suppliers.
- Categories.
- Units.
- Products.
- Inventory.
- Purchase orders.
- Purchase order items.
- Sales orders.
- Sales order items.
- Payments.
- Stock transactions.

---

# 18. Migration Strategy

The database schema is managed using Entity Framework Core migrations.

The workflow is:

```text
Entity Models
    ↓
AppDbContext Configuration
    ↓
EF Core Migration
    ↓
PostgreSQL Database
```

The initial migration is:

```text
InitialCreate
```

Migration files are stored in:

```text
Migrations/
```

The SQL schema script generated from the migration is stored in:

```text
database/init.sql
```

The sample data script is stored separately:

```text
database/sample_data.sql
```

This approach provides:

- Version control for database changes.
- Reproducible database creation.
- Easier team collaboration.
- Easier deployment.
- Clear database history.

---

# 19. Database Structure Summary

The database can be divided into five main domains.

## 19.1 Authentication and Authorization

```text
users
roles
user_roles
```

Purpose:

- System access.
- Authentication.
- Authorization.
- Role management.

---

## 19.2 Master Data

```text
customers
suppliers
categories
units
products
```

Purpose:

- Store reusable business information.

---

## 19.3 Inventory

```text
inventory
stock_transactions
```

Purpose:

- Track current stock.
- Track inventory history.

---

## 19.4 Purchasing

```text
purchase_orders
purchase_order_items
```

Purpose:

- Track products purchased from suppliers.

---

## 19.5 Sales

```text
sales_orders
sales_order_items
payments
```

Purpose:

- Track customer purchases.
- Track sales order details.
- Track payments.

---

# 20. Conclusion

The database design supports a complete inventory and order management workflow.

The main business flow is:

```text
Supplier
    ↓
Purchase Order
    ↓
Purchase Order Items
    ↓
Stock Transaction: StockIn
    ↓
Inventory Increases


Customer
    ↓
Sales Order
    ↓
Sales Order Items
    ↓
Payment
    ↓
Stock Transaction: StockOut
    ↓
Inventory Decreases
```

The database uses:

- UUID primary keys.
- Foreign key relationships.
- Unique constraints.
- Indexes.
- One-to-one relationships.
- One-to-many relationships.
- Many-to-many relationships.
- Order header and order detail tables.
- Current inventory and transaction history separation.
- Entity Framework Core Code First migrations.
- PostgreSQL.

The design is structured to support future development of:

- REST APIs.
- JWT authentication.
- Role-based authorization.
- Inventory operations.
- Purchase management.
- Sales management.
- Payment processing.
- Reporting and analytics.