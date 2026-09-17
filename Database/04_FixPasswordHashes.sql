-- ============================================================
-- ORBITECH DATABASE - BONUS: Fix Demo Password Hashes
-- ============================================================
-- TUTORIAL STEP: Run this script ONLY if the demo credentials don't work.
-- It updates the demo users' password hashes using SQL Server's built-in
-- HASHBYTES function, which produces the same SHA-256 hash as the C# code.
--
-- The C# code uses: SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password))
-- SQL Server uses: HASHBYTES('SHA2_256', CAST(password AS NVARCHAR(MAX)))
-- Both produce identical hashes for the same input.
-- ============================================================

USE ORBITECH_DATA;
GO

-- Update admin password to "Admin@123"
UPDATE ORBI_USER 
SET u_PasswordHash = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CAST('Admin@123' AS NVARCHAR(MAX))), 2)
WHERE u_Username = 'admin_1';
GO

-- Update customer password to "John@123"
UPDATE ORBI_USER 
SET u_PasswordHash = CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', CAST('John@123' AS NVARCHAR(MAX))), 2)
WHERE u_Username = 'john_doe';
GO

-- Verify the hashes were updated
SELECT u_Username, u_PasswordHash, u_Role FROM ORBI_USER;
GO

PRINT 'Password hashes updated. You can now login with:';
PRINT 'Admin:    admin_1 / Admin@123';
PRINT 'Customer: john_doe / John@123';
GO
