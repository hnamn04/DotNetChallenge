CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;

CREATE TABLE categories (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_categories PRIMARY KEY (id)
);

CREATE TABLE customers (
    id uuid NOT NULL,
    name character varying(200) NOT NULL,
    email character varying(255),
    phone character varying(30),
    address character varying(500),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_customers PRIMARY KEY (id)
);

CREATE TABLE roles (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_roles PRIMARY KEY (id)
);

CREATE TABLE suppliers (
    id uuid NOT NULL,
    name character varying(200) NOT NULL,
    email character varying(255),
    phone character varying(30),
    address character varying(500),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_suppliers PRIMARY KEY (id)
);

CREATE TABLE units (
    id uuid NOT NULL,
    name character varying(100) NOT NULL,
    symbol character varying(20) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_units PRIMARY KEY (id)
);

CREATE TABLE users (
    id uuid NOT NULL,
    username character varying(100) NOT NULL,
    email character varying(255) NOT NULL,
    password_hash character varying(500) NOT NULL,
    is_active boolean NOT NULL DEFAULT TRUE,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_users PRIMARY KEY (id)
);

CREATE TABLE sales_orders (
    id uuid NOT NULL,
    order_number character varying(50) NOT NULL,
    customer_id uuid NOT NULL,
    order_date timestamp with time zone NOT NULL,
    status character varying(30) NOT NULL,
    total_amount numeric(18,2) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_sales_orders PRIMARY KEY (id),
    CONSTRAINT fk_sales_orders_customers_customer_id FOREIGN KEY (customer_id) REFERENCES customers (id) ON DELETE RESTRICT
);

CREATE TABLE purchase_orders (
    id uuid NOT NULL,
    order_number character varying(50) NOT NULL,
    supplier_id uuid NOT NULL,
    order_date timestamp with time zone NOT NULL,
    status character varying(30) NOT NULL,
    total_amount numeric(18,2) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_purchase_orders PRIMARY KEY (id),
    CONSTRAINT fk_purchase_orders_suppliers_supplier_id FOREIGN KEY (supplier_id) REFERENCES suppliers (id) ON DELETE RESTRICT
);

CREATE TABLE products (
    id uuid NOT NULL,
    code character varying(50) NOT NULL,
    name character varying(200) NOT NULL,
    description character varying(2000),
    cost_price numeric(18,2) NOT NULL,
    selling_price numeric(18,2) NOT NULL,
    category_id uuid NOT NULL,
    unit_id uuid NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_products PRIMARY KEY (id),
    CONSTRAINT ck_products_cost_price CHECK (cost_price >= 0),
    CONSTRAINT ck_products_selling_price CHECK (selling_price >= 0),
    CONSTRAINT fk_products_categories_category_id FOREIGN KEY (category_id) REFERENCES categories (id) ON DELETE RESTRICT,
    CONSTRAINT fk_products_units_unit_id FOREIGN KEY (unit_id) REFERENCES units (id) ON DELETE RESTRICT
);

CREATE TABLE user_roles (
    user_id uuid NOT NULL,
    role_id uuid NOT NULL,
    CONSTRAINT pk_user_roles PRIMARY KEY (user_id, role_id),
    CONSTRAINT fk_user_roles_roles_role_id FOREIGN KEY (role_id) REFERENCES roles (id) ON DELETE CASCADE,
    CONSTRAINT fk_user_roles_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE payments (
    id uuid NOT NULL,
    sales_order_id uuid NOT NULL,
    amount numeric(18,2) NOT NULL,
    method character varying(50) NOT NULL,
    status character varying(30) NOT NULL,
    paid_at timestamp with time zone,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_payments PRIMARY KEY (id),
    CONSTRAINT fk_payments_sales_orders_sales_order_id FOREIGN KEY (sales_order_id) REFERENCES sales_orders (id) ON DELETE RESTRICT
);

CREATE TABLE inventory (
    id uuid NOT NULL,
    product_id uuid NOT NULL,
    quantity integer NOT NULL DEFAULT 0,
    reserved_quantity integer NOT NULL DEFAULT 0,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_inventory PRIMARY KEY (id),
    CONSTRAINT ck_inventory_quantity CHECK (quantity >= 0),
    CONSTRAINT ck_inventory_reserved_not_greater CHECK (reserved_quantity <= quantity),
    CONSTRAINT ck_inventory_reserved_quantity CHECK (reserved_quantity >= 0),
    CONSTRAINT fk_inventory_products_product_id FOREIGN KEY (product_id) REFERENCES products (id) ON DELETE CASCADE
);

CREATE TABLE purchase_order_items (
    id uuid NOT NULL,
    purchase_order_id uuid NOT NULL,
    product_id uuid NOT NULL,
    quantity integer NOT NULL,
    unit_price numeric(18,2) NOT NULL,
    total_price numeric(18,2) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_purchase_order_items PRIMARY KEY (id),
    CONSTRAINT ck_purchase_order_items_quantity CHECK (quantity > 0),
    CONSTRAINT ck_purchase_order_items_total_price CHECK (total_price >= 0),
    CONSTRAINT ck_purchase_order_items_unit_price CHECK (unit_price >= 0),
    CONSTRAINT fk_purchase_order_items_products_product_id FOREIGN KEY (product_id) REFERENCES products (id) ON DELETE RESTRICT,
    CONSTRAINT fk_purchase_order_items_purchase_orders_purchase_order_id FOREIGN KEY (purchase_order_id) REFERENCES purchase_orders (id) ON DELETE CASCADE
);

CREATE TABLE sales_order_items (
    id uuid NOT NULL,
    sales_order_id uuid NOT NULL,
    product_id uuid NOT NULL,
    quantity integer NOT NULL,
    unit_price numeric(18,2) NOT NULL,
    total_price numeric(18,2) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_sales_order_items PRIMARY KEY (id),
    CONSTRAINT ck_sales_order_items_quantity CHECK (quantity > 0),
    CONSTRAINT ck_sales_order_items_unit_price CHECK (unit_price >= 0),
    CONSTRAINT fk_sales_order_items_products_product_id FOREIGN KEY (product_id) REFERENCES products (id) ON DELETE RESTRICT,
    CONSTRAINT fk_sales_order_items_sales_orders_sales_order_id FOREIGN KEY (sales_order_id) REFERENCES sales_orders (id) ON DELETE CASCADE
);

CREATE TABLE stock_transactions (
    id uuid NOT NULL,
    product_id uuid NOT NULL,
    type character varying(30) NOT NULL,
    quantity integer NOT NULL,
    reference_type character varying(50),
    reference_id uuid,
    note character varying(1000),
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    CONSTRAINT pk_stock_transactions PRIMARY KEY (id),
    CONSTRAINT fk_stock_transactions_products_product_id FOREIGN KEY (product_id) REFERENCES products (id) ON DELETE RESTRICT
);

CREATE UNIQUE INDEX ix_categories_name ON categories (name);

CREATE UNIQUE INDEX ix_customers_email ON customers (email);

CREATE INDEX ix_customers_phone ON customers (phone);

CREATE UNIQUE INDEX ix_inventory_product_id ON inventory (product_id);

CREATE INDEX ix_payments_sales_order_id ON payments (sales_order_id);

CREATE INDEX ix_payments_status ON payments (status);

CREATE INDEX ix_products_category_id ON products (category_id);

CREATE UNIQUE INDEX ix_products_code ON products (code);

CREATE INDEX ix_products_name ON products (name);

CREATE INDEX ix_products_unit_id ON products (unit_id);

CREATE INDEX ix_purchase_order_items_product_id ON purchase_order_items (product_id);

CREATE INDEX ix_purchase_order_items_purchase_order_id ON purchase_order_items (purchase_order_id);

CREATE INDEX ix_purchase_order_items_purchase_order_id_product_id ON purchase_order_items (purchase_order_id, product_id);

CREATE UNIQUE INDEX ix_purchase_orders_order_number ON purchase_orders (order_number);

CREATE INDEX ix_purchase_orders_supplier_id ON purchase_orders (supplier_id);

CREATE INDEX ix_purchase_orders_supplier_id_order_date ON purchase_orders (supplier_id, order_date);

CREATE UNIQUE INDEX ix_roles_name ON roles (name);

CREATE INDEX ix_sales_order_items_product_id ON sales_order_items (product_id);

CREATE INDEX ix_sales_order_items_sales_order_id ON sales_order_items (sales_order_id);

CREATE INDEX ix_sales_order_items_sales_order_id_product_id ON sales_order_items (sales_order_id, product_id);

CREATE INDEX ix_sales_orders_customer_id ON sales_orders (customer_id);

CREATE INDEX ix_sales_orders_customer_id_order_date ON sales_orders (customer_id, order_date);

CREATE UNIQUE INDEX ix_sales_orders_order_number ON sales_orders (order_number);

CREATE INDEX ix_stock_transactions_product_id ON stock_transactions (product_id);

CREATE INDEX ix_stock_transactions_product_id_created_at ON stock_transactions (product_id, created_at);

CREATE INDEX ix_stock_transactions_reference_type_reference_id ON stock_transactions (reference_type, reference_id);

CREATE INDEX ix_stock_transactions_type ON stock_transactions (type);

CREATE UNIQUE INDEX ix_suppliers_email ON suppliers (email);

CREATE INDEX ix_suppliers_phone ON suppliers (phone);

CREATE UNIQUE INDEX ix_units_name ON units (name);

CREATE UNIQUE INDEX ix_units_symbol ON units (symbol);

CREATE INDEX ix_user_roles_role_id ON user_roles (role_id);

CREATE UNIQUE INDEX ix_users_email ON users (email);

CREATE UNIQUE INDEX ix_users_username ON users (username);

INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
VALUES ('20260810071451_InitialCreate', '8.0.8');

COMMIT;

