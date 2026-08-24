USE [BPS]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_Place_Create]
    @PlaceName NVARCHAR(150),
    @PricePerTrip DECIMAL(18,2),
    @IsActive BIT,
    @CreatedBy NVARCHAR(100),
    @CreatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Places
    (
        PlaceName,
        PricePerTrip,
        IsActive,
        CreatedAt,
        CreatedBy

    )
    VALUES
    (
        @PlaceName,
        @PricePerTrip,
        @IsActive,
        @CreatedAt,
        @CreatedBy

    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END;



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[SP_Place_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Places
    SET
        IsActive = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END;

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

  ALTER   PROCEDURE [dbo].[SP_Place_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        PlaceName,
        PricePerTrip,
        IsActive
    FROM Places
    WHERE IsActive = 1
    ORDER BY PlaceName;
END;

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[SP_Place_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        PlaceName,
        PricePerTrip,
        IsActive
    FROM Places
    WHERE Id = @Id;
END;


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[SP_Place_Update]
    @Id INT,
    @PlaceName NVARCHAR(150),
    @PricePerTrip DECIMAL(18,2),
    @IsActive BIT,
	@UpdatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Places
    SET
        PlaceName = @PlaceName,
        PricePerTrip = @PricePerTrip,
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE(),
		UpdatedBy = @UpdatedBy
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END;


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   PROCEDURE [dbo].[SP_User_Create]
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(500),
    @FullName NVARCHAR(150),
    @Role NVARCHAR(50),
	@Email NVARCHAR(255),
    @PhoneNumber NVARCHAR(20),
    @IsActive BIT,
    @CreatedBy NVARCHAR(100),
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Users
    (
        Username,
        PasswordHash,
        FullName,
        Role,
		Email,
        PhoneNumber,
        IsActive,
        CreatedBy,
        UpdatedBy
    )
    VALUES
    (
        @Username,
        @PasswordHash,
        @FullName,
        @Role,
		@Email,
		@PhoneNumber,
        @IsActive,
        @CreatedBy,
        @UpdatedBy
    );
END;

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  ALTER PROCEDURE [dbo].[SP_User_GetByUsername]
    @Username NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Username,
        PasswordHash,
        FullName,
        Email,
        PhoneNumber,
        Role,
        IsActive,
        CreatedAt,
        CreatedBy,
        UpdatedAt,
        UpdatedBy
    FROM dbo.Users
    WHERE Username = @Username;
END