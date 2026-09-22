-- ============================================================
-- 07_FeedbackAndReports.sql
-- Member C reports integration for the CURRENT OrbiTech schema.
--
-- Run after 05_OrdersTable.sql.
-- Do NOT run the ORDERS / ORDER_DET definitions from Report.zip:
-- this project already uses ORBI_ORDER / ORDER_ITEM.
-- ============================================================

IF OBJECT_ID('dbo.ORBI_ORDER', 'U') IS NULL
BEGIN
    THROW 50001, 'ORBI_ORDER is missing. Run 05_OrdersTable.sql first.', 1;
END;
GO

IF OBJECT_ID('dbo.FEEDBACK', 'U') IS NULL
BEGIN
    CREATE TABLE FEEDBACK(
        f_ID INT IDENTITY(1,1) NOT NULL,
        o_ID INT NOT NULL,
        f_DeliveryRating INT NOT NULL,
        f_SatisfactionRating INT NOT NULL,
        f_Comments VARCHAR(MAX) NULL,
        f_IsComplaint BIT NOT NULL DEFAULT 0,
        f_ComplaintCategory VARCHAR(50) NULL,
        f_DateSubmitted DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_FEEDBACK PRIMARY KEY(f_ID),
        CONSTRAINT UQ_FEEDBACK_Order UNIQUE(o_ID),
        CONSTRAINT FK_FEEDBACK_Order FOREIGN KEY(o_ID) REFERENCES ORBI_ORDER(o_ID),
        CONSTRAINT CK_FEEDBACK_DeliveryRating CHECK(f_DeliveryRating BETWEEN 1 AND 5),
        CONSTRAINT CK_FEEDBACK_SatisfactionRating CHECK(f_SatisfactionRating BETWEEN 1 AND 5),
        CONSTRAINT CK_FEEDBACK_ComplaintCategory CHECK(
            f_IsComplaint = 0 OR f_ComplaintCategory IS NOT NULL)
    );
END;
GO

PRINT 'Feedback/report schema is ready.';
GO
