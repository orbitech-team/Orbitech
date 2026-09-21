-- ============================================================
-- ORBITECH DATABASE - STEP 2: CREATE TABLES (FIXED)
-- ============================================================
-- This script creates all the tables in the ORBITECH_DATA database.
-- Run this AFTER running 01_CreateDatabase.sql.
--
-- FIXES MADE TO THE ORIGINAL SCHEMA:
--   1. Added missing p_Colour column to ORBI_PRODUCT (was referenced in INSERTs but didn't exist)
--   2. Fixed CATEGORY CHECK constraint - missing closing parenthesis
--   3. Fixed p_Condition CHECK: 'Rurbished' → 'Refurbished' (typo fix, INSERTs use 'Refurbished')
--   4. Standardized all IDENTITY seeds to (1,1) for consistency
--   5. Changed u_Password to u_PasswordHash in CREATE TABLE (INSERTs reference u_PasswordHash)
--   6. Added u_Role column to ORBI_USER for admin vs customer distinction
-- ============================================================

USE ORBITECH_DATA;
GO

-- ============================================================
-- TABLE 1: ORBI_USER
-- Stores user accounts (both customers and admins)
-- ============================================================
-- TUTORIAL STEP: This table holds every person who can log in.
-- u_Role determines if they're an 'admin' or 'customer'.
CREATE TABLE ORBI_USER(
    u_ID INT IDENTITY(1,1) NOT NULL,
    u_Username VARCHAR(25) UNIQUE NOT NULL,
    u_PasswordHash VARCHAR(256) NOT NULL,    -- FIXED: was u_Password VARCHAR(50), now u_PasswordHash VARCHAR(256) to hold SHA-256 hashes
    u_Role VARCHAR(20) DEFAULT 'customer' NOT NULL,  -- NEW: distinguishes 'admin' from 'customer'
    u_Email VARCHAR(250) NULL,                -- NEW: optional email for registration
    u_CreatedDate DATETIME DEFAULT GETDATE() NOT NULL,  -- NEW: track when account was created
    PRIMARY KEY(u_ID)
);
GO

-- ============================================================
-- TABLE 2: CATEGORY
-- Stores product categories (Phone, Tablet, Smartwatch)
-- ============================================================
-- TUTORIAL STEP: The CHECK constraint ensures only valid category names can be inserted.
-- The original had a typo (missing closing parenthesis) — now fixed.
CREATE TABLE CATEGORY(
    c_ID INT IDENTITY(1,1) NOT NULL,
    c_Name VARCHAR(50) NOT NULL,
    PRIMARY KEY(c_ID),
    CONSTRAINT CHK_CategoryName CHECK (c_Name IN ('Phone', 'Tablet', 'Smartwatch'))  -- FIXED: added closing parenthesis
);
GO

-- ============================================================
-- TABLE 3: ORBI_PRODUCT
-- Stores all products (phones, tablets, smartwatches)
-- ============================================================
-- TUTORIAL STEP: This is the main product table. Every product you see on the
-- website comes from here. The admin panel adds/removes rows in this table.
CREATE TABLE ORBI_PRODUCT(
    p_ID INT IDENTITY(1,1) NOT NULL,
    p_Name VARCHAR(50) NOT NULL,
    p_Description VARCHAR(MAX) NOT NULL,
    p_Price DECIMAL(10,2) NOT NULL,
    p_Quantity INT NOT NULL,
    p_ImageURL VARCHAR(MAX) NOT NULL,
    c_ID INT NOT NULL,                        -- Foreign key to CATEGORY
    p_Brand VARCHAR(250) NOT NULL,
    p_Colour VARCHAR(50) NULL,                 -- NEW: was missing from original schema but referenced in INSERTs
    p_Condition VARCHAR(100) CHECK (p_Condition IN ('New', 'Refurbished')) NOT NULL,  -- FIXED: 'Rurbished' → 'Refurbished'
    p_Grade VARCHAR(1) CHECK(p_Grade IN ('A', 'B', 'C')) NOT NULL,
    PRIMARY KEY(p_ID),
    FOREIGN KEY(c_ID) REFERENCES CATEGORY(c_ID)
);
GO

-- ============================================================
-- TABLE 4: CART
-- Each user has one cart
-- ============================================================
CREATE TABLE CART(
    ct_ID INT IDENTITY(1,1) NOT NULL,
    u_ID INT NOT NULL,
    ct_CreatedDate DATETIME DEFAULT GETDATE() NOT NULL,  -- NEW: track cart creation
    PRIMARY KEY(ct_ID),
    FOREIGN KEY(u_ID) REFERENCES ORBI_USER(u_ID)
);
GO

-- ============================================================
-- TABLE 5: CART_ITEM
-- Individual items inside a cart
-- ============================================================
CREATE TABLE CART_ITEM(
    ci_ID INT IDENTITY(1,1) NOT NULL,
    ct_ID INT NOT NULL,
    p_ID INT NOT NULL,
    ci_Quantity INT NOT NULL,
    PRIMARY KEY(ci_ID),
    FOREIGN KEY(ct_ID) REFERENCES CART(ct_ID),
    FOREIGN KEY(p_ID) REFERENCES ORBI_PRODUCT(p_ID)
);
GO

-- ============================================================
-- TABLE 6: CONTACT
-- Stores messages submitted through the contact form
-- ============================================================
-- TUTORIAL STEP: When someone fills out the Contact form on the website,
-- their message gets saved here so the admin can read it later.
CREATE TABLE CONTACT(
    cn_ID INT IDENTITY(1,1) NOT NULL,
    cn_Name VARCHAR(50) NULL,
    cn_Email VARCHAR(250) NOT NULL,
    cn_Subject VARCHAR(100) NOT NULL,
    cn_Message VARCHAR(MAX) NOT NULL,
    cn_CreatedDate DATETIME DEFAULT GETDATE() NOT NULL,  -- NEW: track when message was sent
    PRIMARY KEY(cn_ID)
);
GO

PRINT 'All tables created successfully.';
GO


CREATE TABLE FAVOURITE
(
    fav_ID   INT IDENTITY(1,1) PRIMARY KEY,
    u_ID     INT NOT NULL,
    p_ID     INT NOT NULL,
    fav_Date DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Favourite_User    FOREIGN KEY (u_ID) REFERENCES ORBI_USER(u_ID),
    CONSTRAINT FK_Favourite_Product FOREIGN KEY (p_ID) REFERENCES ORBI_PRODUCT(p_ID),

    CONSTRAINT UQ_Favourite_UserProduct UNIQUE (u_ID, p_ID)
);
