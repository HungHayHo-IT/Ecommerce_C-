-- Chạy một lần trên database LiteCommerceDB để bật đăng ký/đăng nhập khách hàng trên Shop.
-- Ensure the Password column exists and is large enough for hashed values (PBKDF2 storage)
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.Customers') AND name = N'Password')
BEGIN
    ALTER TABLE dbo.Customers ADD [Password] NVARCHAR(500) NULL;
END
ELSE
BEGIN
    -- If column exists but is too short, increase its size to 500
    DECLARE @len int = (
        SELECT max_length FROM sys.columns
        WHERE object_id = OBJECT_ID(N'dbo.Customers') AND name = N'Password'
    );
    IF @len < 500
    BEGIN
        ALTER TABLE dbo.Customers ALTER COLUMN [Password] NVARCHAR(500) NULL;
    END
END
GO
