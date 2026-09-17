-- ============================================================
-- ORBITECH DATABASE - STEP 1: CREATE DATABASE
-- ============================================================
-- This script creates the ORBITECH_DATA database.
-- Run this FIRST in SQL Server Management Studio (SSMS).
--
-- TUTORIAL STEP: Open SSMS → Connect to your SQL Server instance
-- (usually "localhost" or ".\SQLEXPRESS") → New Query → Paste this → Execute (F5)
-- ============================================================

-- Step 1.1: Create the database
CREATE DATABASE ORBITECH_DATA;
GO

-- Step 1.2: Switch to the new database so all subsequent commands run against it
USE ORBITECH_DATA;
GO

-- Step 1.3: Print a confirmation message (you should see this in the Messages pane)
PRINT 'Database ORBITECH_DATA created successfully.';
GO
