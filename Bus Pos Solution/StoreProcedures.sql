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
END;



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[SP_Trip_Create]
    @PlaceId INT,
    @TripDate DATE,
    @TipStatus BIT,
    @TipAmount DECIMAL(18,2),
	@CreatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Price DECIMAL(18,2);
    DECLARE @Total DECIMAL(18,2);

    -- Get current place price
    SELECT
        @Price = PricePerTrip
    FROM Places
    WHERE Id = @PlaceId
      AND IsActive = 1;

    -- Place not found
    IF @Price IS NULL
    BEGIN
        THROW 50001,
            'Place not found or inactive.',
            1;
    END;

    -- Tip OFF means tip must be zero
    IF @TipStatus = 0
    BEGIN
        SET @TipAmount = 0;
    END;

    -- Tip cannot be negative
    IF @TipAmount < 0
    BEGIN
        THROW 50002,
            'Tip amount cannot be negative.',
            1;
    END;

    -- Calculate total
    SET @Total = @Price * @TipAmount;

    INSERT INTO TripRecords
    (
        PlaceId,
        TripDate,
        TipStatus,
        TipAmount,
        Price,
        CreatedAt,
        CreatedBy,
        UpdatedAt,
        UpdatedBy,
		IsActive
    )
    VALUES
    (
        @PlaceId,
        @TripDate,
        @TipStatus,
        @TipAmount,
        @Price,
        GETUTCDATE(),
        @CreatedBy,
        NULL,
        NULL,
		1
    );

    DECLARE @Id BIGINT =
        SCOPE_IDENTITY();

    SELECT
        tr.Id,
        tr.PlaceId,
        p.PlaceName,
        tr.TripDate,
        tr.TipStatus,
        tr.TipAmount,
        tr.Price,
        tr.Total,
        tr.CreatedAt,
        tr.CreatedBy,
        tr.UpdatedAt,
        tr.UpdatedBy,
		tr.IsActive
    FROM TripRecords tr
    INNER JOIN Places p
        ON tr.PlaceId = p.Id
    WHERE tr.Id = @Id;
END;


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[SP_Trip_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        tr.Id,
        tr.PlaceId,
        p.PlaceName,
        tr.TripDate,
        tr.TipStatus,
        tr.TipAmount,
        tr.Price,
        tr.Total,
        tr.CreatedAt,
        tr.UpdatedAt,
		tr.CreatedBy,
		tr.UpdatedBy,
        tr.IsActive
    FROM TripRecords tr
    INNER JOIN Places p
        ON tr.PlaceId = p.Id
	WHERE tr.IsActive = 1
    ORDER BY
        tr.TripDate DESC,
        tr.Id DESC;
END;



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[SP_Trip_GetById]
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        tr.Id,
        tr.PlaceId,
        p.PlaceName,
        tr.TripDate,
        tr.TipStatus,
        tr.TipAmount,
        tr.Price,
        tr.Total,
        tr.CreatedAt,
        tr.UpdatedAt,
		tr.CreatedBy,
		tr.UpdatedBy,
        tr.IsActive
    FROM TripRecords tr
    INNER JOIN Places p
        ON tr.PlaceId = p.Id
    WHERE tr.Id = @Id;
END;

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[SP_Trip_Delete]
    @Id BIGINT,
    @UpdatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE TripRecords
    SET
        IsActive = 0,
        UpdatedAt = GETUTCDATE(),
        UpdatedBy = @UpdatedBy
    WHERE Id = @Id
      AND IsActive = 1;

    SELECT @@ROWCOUNT AS RowsAffected;
END;

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[SP_Trip_Update]
    @Id BIGINT,
    @PlaceId INT,
    @TripDate DATE,
    @TipStatus BIT,
    @TipAmount DECIMAL(18,2),
    @UpdatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Price DECIMAL(18,2);
    DECLARE @Total DECIMAL(18,2);

    IF NOT EXISTS
    (
        SELECT 1
        FROM TripRecords
        WHERE Id = @Id
          AND IsActive = 1
    )
    BEGIN
        THROW 50003,
            'Trip record not found.',
            1;
    END;

    SELECT
        @Price = PricePerTrip
    FROM Places
    WHERE Id = @PlaceId
      AND IsActive = 1;

    IF @Price IS NULL
    BEGIN
        THROW 50001,
            'Place not found or inactive.',
            1;
    END;

    IF @TipStatus = 0
    BEGIN
        SET @TipAmount = 0;
    END;

    IF @TipAmount < 0
    BEGIN
        THROW 50002,
            'Tip amount cannot be negative.',
            1;
    END;

    SET @Total = @Price * @TipAmount;

    UPDATE TripRecords
    SET
        PlaceId = @PlaceId,
        TripDate = @TripDate,
        TipStatus = @TipStatus,
        TipAmount = @TipAmount,
        Price = @Price,
        UpdatedAt = GETUTCDATE(),
        UpdatedBy = @UpdatedBy
    WHERE Id = @Id
      AND IsActive = 1;

    SELECT
        tr.Id,
        tr.PlaceId,
        p.PlaceName,
        tr.TripDate,
        tr.TipStatus,
        tr.TipAmount,
        tr.Price,
        tr.Total,
        tr.CreatedAt,
        tr.CreatedBy,
        tr.UpdatedAt,
        tr.UpdatedBy,
        tr.IsActive
    FROM TripRecords tr
    INNER JOIN Places p
        ON tr.PlaceId = p.Id
    WHERE tr.Id = @Id;
END;
