USE ORBITECH_DATA;
GO

IF COL_LENGTH('dbo.ORBI_USER', 'u_RegisteredDate') IS NULL
BEGIN
    ALTER TABLE ORBI_USER ADD u_RegisteredDate DATETIME NULL;
END
GO

-- New batch: the column now exists at compile time
UPDATE ORBI_USER SET u_RegisteredDate = GETDATE() WHERE u_RegisteredDate IS NULL;
GO
