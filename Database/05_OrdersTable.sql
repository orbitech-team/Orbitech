-- ============================================================
-- 04_OrdersTables.sql
-- PART A: Order, Order Item, and Invoice tables.
-- Run this AFTER 01, 02, 03 in SSMS.
--
-- WHY A SEPARATE ORDER_ITEM TABLE:
-- A customer's ORDER is one row. The PRODUCTS in that order are
-- many rows. That one-to-many relationship is exactly why we need
-- two tables joined by a foreign key.
--
-- WHY oi_UnitPrice COPIES the price at purchase time:
-- If a product's price changes next month, old invoices must still
-- show what the customer ACTUALLY paid. We never join back to
-- ORBI_PRODUCT for the price - we snapshot it here.
-- ============================================================

CREATE TABLE ORBI_ORDER(
    o_ID INT IDENTITY(1,1) UNIQUE NOT NULL,
    u_ID INT NOT NULL,
    o_Date DATETIME NOT NULL DEFAULT GETDATE(),
    o_Subtotal DECIMAL(10,2) NOT NULL,
    o_Discount DECIMAL(10,2) NOT NULL DEFAULT 0,
    o_Shipping DECIMAL(10,2) NOT NULL DEFAULT 0,
    o_Tax DECIMAL(10,2) NOT NULL DEFAULT 0,
    o_Total DECIMAL(10,2) NOT NULL,
    o_PromoCode VARCHAR(20) NULL,
    o_Status VARCHAR(20) NOT NULL DEFAULT 'Placed',
    PRIMARY KEY(o_ID),
    FOREIGN KEY(u_ID) REFERENCES ORBI_USER(u_ID)
);

CREATE TABLE ORDER_ITEM(
    oi_ID INT IDENTITY(1,1) UNIQUE NOT NULL,
    o_ID INT NOT NULL,
    p_ID INT NOT NULL,
    oi_Quantity INT NOT NULL,
    oi_UnitPrice DECIMAL(10,2) NOT NULL,
    PRIMARY KEY(oi_ID),
    FOREIGN KEY(o_ID) REFERENCES ORBI_ORDER(o_ID),
    FOREIGN KEY(p_ID) REFERENCES ORBI_PRODUCT(p_ID)
);

CREATE TABLE INVOICE(
    inv_ID INT IDENTITY(1,1) UNIQUE NOT NULL,
    o_ID INT NOT NULL,
    inv_Number VARCHAR(20) UNIQUE NOT NULL,
    inv_Date DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY(inv_ID),
    FOREIGN KEY(o_ID) REFERENCES ORBI_ORDER(o_ID)
);

-- Quick sanity check: this should fail with a foreign key error (that's GOOD, it proves the FK works):
-- INSERT INTO ORDER_ITEM (o_ID, p_ID, oi_Quantity, oi_UnitPrice) VALUES (9999, 1, 1, 10.00);
