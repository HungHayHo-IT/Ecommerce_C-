-- Chạy một lần trên database LiteCommerceDB để bật đăng ký/đăng nhập khách hàng trên Shop.
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Customers') AND name = N'Password')
BEGIN
    ALTER TABLE dbo.Customers ADD [Password] NVARCHAR(200) NULL;
END
GO
