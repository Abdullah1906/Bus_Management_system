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



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_CreateTripSchedule]
    @BusId INT,
    @RouteId INT,
    @TripDate DATE,
    @DepartureTime TIME,
    @ArrivalTime TIME = NULL,
    @Fare DECIMAL(18,2),
    @CreatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        ------------------------------------------------
        -- 1. Validate Bus
        ------------------------------------------------
        IF NOT EXISTS
        (
            SELECT 1
            FROM Buses
            WHERE Id = @BusId
              AND IsActive = 1
        )
        BEGIN
            THROW 50001,
                'Bus not found or inactive.',
                1;
        END;


        ------------------------------------------------
        -- 2. Validate Route
        ------------------------------------------------
        IF NOT EXISTS
        (
            SELECT 1
            FROM Routes
            WHERE Id = @RouteId
              AND IsActive = 1
        )
        BEGIN
            THROW 50002,
                'Route not found or inactive.',
                1;
        END;


        ------------------------------------------------
        -- 3. Validate Fare
        ------------------------------------------------
        IF @Fare < 0
        BEGIN
            THROW 50003,
                'Fare cannot be negative.',
                1;
        END;


        ------------------------------------------------
        -- 4. Check Bus has active seats
        ------------------------------------------------
        IF NOT EXISTS
        (
            SELECT 1
            FROM BusSeats
            WHERE BusId = @BusId
              AND IsActive = 1
        )
        BEGIN
            THROW 50004,
                'Bus does not have any active seats.',
                1;
        END;


        ------------------------------------------------
        -- 5. Prevent duplicate trip
        ------------------------------------------------
        IF EXISTS
        (
            SELECT 1
            FROM Trips
            WHERE BusId = @BusId
              AND RouteId = @RouteId
              AND TripDate = @TripDate
              AND DepartureTime = @DepartureTime
              AND IsActive = 1
        )
        BEGIN
            THROW 50005,
                'A trip with the same bus, route, date and departure time already exists.',
                1;
        END;


        ------------------------------------------------
        -- 6. Create Trip
        ------------------------------------------------
        INSERT INTO Trips
        (
            BusId,
            RouteId,
            TripDate,
            DepartureTime,
            ArrivalTime,
            Fare,
            IsActive,
            CreatedAt,
            CreatedBy
        )
        VALUES
        (
            @BusId,
            @RouteId,
            @TripDate,
            @DepartureTime,
            @ArrivalTime,
            @Fare,
            1,
            GETUTCDATE(),
            @CreatedBy
        );


        DECLARE @TripId BIGINT =
            SCOPE_IDENTITY();


        ------------------------------------------------
        -- 7. Create TripSeats
        ------------------------------------------------
        INSERT INTO TripSeats
        (
            TripId,
            BusSeatId,
            Status,
            LockedByCustomerId,
            LockedUntil,
            BookedAt,
            CreatedAt
        )
        SELECT
            @TripId,
            bs.Id,
            1,              -- Available
            NULL,
            NULL,
            NULL,
            GETUTCDATE()
        FROM BusSeats bs
        WHERE bs.BusId = @BusId
          AND bs.IsActive = 1;


        ------------------------------------------------
        -- 8. Commit
        ------------------------------------------------
        COMMIT TRANSACTION;


        ------------------------------------------------
        -- 9. Return created Trip
        ------------------------------------------------
        SELECT
            t.Id,
            t.BusId,
            b.BusName,
            b.BusNumber,
            t.RouteId,
            r.FromPlace,
            r.ToPlace,
            t.TripDate,
            t.DepartureTime,
            t.ArrivalTime,
            t.Fare,
            t.IsActive,
            t.CreatedAt,
            t.CreatedBy
        FROM Trips t
        INNER JOIN Buses b
            ON t.BusId = b.Id
        INNER JOIN Routes r
            ON t.RouteId = r.Id
        WHERE t.Id = @TripId;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_LockSeat]
    @TripId BIGINT,
    @BusSeatId BIGINT,
    @CustomerId BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        DECLARE
            @TripSeatId BIGINT,
            @Status TINYINT,
            @LockedByCustomerId BIGINT,
            @LockedUntil DATETIME2;

        ------------------------------------------------
        -- 1. Find and LOCK the TripSeat row
        ------------------------------------------------
        SELECT
            @TripSeatId = ts.Id,
            @Status = ts.Status,
            @LockedByCustomerId = ts.LockedByCustomerId,
            @LockedUntil = ts.LockedUntil
        FROM TripSeats ts WITH (UPDLOCK, HOLDLOCK)
        WHERE ts.TripId = @TripId
          AND ts.BusSeatId = @BusSeatId;


        ------------------------------------------------
        -- 2. Seat does not exist for this trip
        ------------------------------------------------
        IF @TripSeatId IS NULL
        BEGIN
            THROW 50010,
                'Seat is not available for this trip.',
                1;
        END;


        ------------------------------------------------
        -- 3. Check Trip is active
        ------------------------------------------------
        IF NOT EXISTS
        (
            SELECT 1
            FROM Trips
            WHERE Id = @TripId
              AND IsActive = 1
        )
        BEGIN
            THROW 50011,
                'Trip not found or inactive.',
                1;
        END;


        ------------------------------------------------
        -- 4. If lock expired, release it first
        ------------------------------------------------
        IF @Status = 2
           AND @LockedUntil IS NOT NULL
           AND @LockedUntil <= GETUTCDATE()
        BEGIN
            UPDATE TripSeats
            SET
                Status = 1,
                LockedByCustomerId = NULL,
                LockedUntil = NULL
            WHERE Id = @TripSeatId;

            SET @Status = 1;
            SET @LockedByCustomerId = NULL;
            SET @LockedUntil = NULL;
        END;


        ------------------------------------------------
        -- 5. Seat already booked
        ------------------------------------------------
        IF @Status = 3
        BEGIN
            THROW 50012,
                'Seat is already booked.',
                1;
        END;


        ------------------------------------------------
        -- 6. Seat locked by another customer
        ------------------------------------------------
        IF @Status = 2
           AND @LockedByCustomerId <> @CustomerId
        BEGIN
            THROW 50013,
                'Seat is currently locked by another customer.',
                1;
        END;


        ------------------------------------------------
        -- 7. Lock the seat
        ------------------------------------------------
        UPDATE TripSeats
        SET
            Status = 2,
            LockedByCustomerId = @CustomerId,
            LockedUntil = DATEADD(MINUTE, 10, GETUTCDATE())
        WHERE Id = @TripSeatId;


        ------------------------------------------------
        -- 8. Commit
        ------------------------------------------------
        COMMIT TRANSACTION;


        ------------------------------------------------
        -- 9. Return result
        ------------------------------------------------
        SELECT
            ts.Id AS TripSeatId,
            ts.TripId,
            ts.BusSeatId,
            bs.SeatNumber,
            ts.Status,
            ts.LockedByCustomerId,
            ts.LockedUntil
        FROM TripSeats ts
        INNER JOIN BusSeats bs
            ON ts.BusSeatId = bs.Id
        WHERE ts.Id = @TripSeatId;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_ConfirmBooking]
    @TripId BIGINT,
    @CustomerId BIGINT,
    @PassengersJson NVARCHAR(MAX),
    @PaymentMethod NVARCHAR(50),
    @TransactionId NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        DECLARE
            @BookingId BIGINT,
            @PNR NVARCHAR(30),
            @TotalAmount DECIMAL(18,2),
            @ConfirmedAt DATETIME2 = GETUTCDATE();


        ---------------------------------------------------------
        -- 1. Validate Customer
        ---------------------------------------------------------
        IF NOT EXISTS
        (
            SELECT 1
            FROM Customers
            WHERE Id = @CustomerId
              AND IsActive = 1
        )
        BEGIN
            THROW 50200,
                'Customer not found or inactive.',
                1;
        END;


        ---------------------------------------------------------
        -- 2. Validate Trip
        ---------------------------------------------------------
        IF NOT EXISTS
        (
            SELECT 1
            FROM Trips
            WHERE Id = @TripId
              AND IsActive = 1
        )
        BEGIN
            THROW 50201,
                'Trip not found or inactive.',
                1;
        END;


        ---------------------------------------------------------
        -- 3. Validate JSON
        ---------------------------------------------------------
        IF @PassengersJson IS NULL
           OR ISJSON(@PassengersJson) <> 1
        BEGIN
            THROW 50202,
                'Invalid passenger information.',
                1;
        END;


        ---------------------------------------------------------
        -- 4. Parse passenger data
        ---------------------------------------------------------
        DECLARE @Passengers TABLE
        (
            TripSeatId BIGINT PRIMARY KEY,
            PassengerName NVARCHAR(150),
            PassengerPhone NVARCHAR(30),
            PassengerNID NVARCHAR(50)
        );


        INSERT INTO @Passengers
        (
            TripSeatId,
            PassengerName,
            PassengerPhone,
            PassengerNID
        )
        SELECT
            TripSeatId,
            PassengerName,
            PassengerPhone,
            PassengerNID
        FROM OPENJSON(@PassengersJson)
        WITH
        (
            TripSeatId BIGINT
                '$.tripSeatId',

            PassengerName NVARCHAR(150)
                '$.passengerName',

            PassengerPhone NVARCHAR(30)
                '$.passengerPhone',

            PassengerNID NVARCHAR(50)
                '$.passengerNID'
        );


        ---------------------------------------------------------
        -- 5. Validate passenger count
        ---------------------------------------------------------
        IF NOT EXISTS
        (
            SELECT 1
            FROM @Passengers
        )
        BEGIN
            THROW 50203,
                'At least one passenger is required.',
                1;
        END;


        ---------------------------------------------------------
        -- 6. Validate passenger information
        ---------------------------------------------------------
        IF EXISTS
        (
            SELECT 1
            FROM @Passengers
            WHERE PassengerName IS NULL
               OR LTRIM(RTRIM(PassengerName)) = ''
               OR PassengerPhone IS NULL
               OR LTRIM(RTRIM(PassengerPhone)) = ''
        )
        BEGIN
            THROW 50204,
                'Passenger name and phone are required.',
                1;
        END;


        ---------------------------------------------------------
        -- 7. Lock TripSeats
        ---------------------------------------------------------
        DECLARE @LockedSeats TABLE
        (
            TripSeatId BIGINT PRIMARY KEY,
            BusSeatId BIGINT,
            SeatNumber NVARCHAR(50),
            Fare DECIMAL(18,2)
        );


        INSERT INTO @LockedSeats
        (
            TripSeatId,
            BusSeatId,
            SeatNumber,
            Fare
        )
        SELECT
            ts.Id,
            ts.BusSeatId,
            bs.SeatNumber,
            t.Fare
        FROM TripSeats ts WITH (UPDLOCK, HOLDLOCK)
        INNER JOIN Trips t
            ON t.Id = ts.TripId
        INNER JOIN BusSeats bs
            ON bs.Id = ts.BusSeatId
        INNER JOIN @Passengers p
            ON p.TripSeatId = ts.Id
        WHERE ts.TripId = @TripId
          AND ts.Status = 2
          AND ts.LockedByCustomerId = @CustomerId
          AND ts.LockedUntil > GETUTCDATE();


        ---------------------------------------------------------
        -- 8. Every requested seat must be valid and locked
        ---------------------------------------------------------
        IF
        (
            SELECT COUNT(*)
            FROM @LockedSeats
        )
        <>
        (
            SELECT COUNT(*)
            FROM @Passengers
        )
        BEGIN
            THROW 50205,
                'One or more seats are not locked by this customer or the lock has expired.',
                1;
        END;


        ---------------------------------------------------------
        -- 9. Calculate total
        ---------------------------------------------------------
        SELECT
            @TotalAmount = SUM(Fare)
        FROM @LockedSeats;


        ---------------------------------------------------------
        -- 10. Generate PNR
        ---------------------------------------------------------
        SET @PNR =
            'BPS' +
            CONVERT(CHAR(8), GETDATE(), 112) +
            RIGHT(
                '000000' +
                CAST(
                    ABS(CHECKSUM(NEWID()))
                    AS VARCHAR(6)
                ),
                6
            );


        ---------------------------------------------------------
        -- 11. Create Booking
        ---------------------------------------------------------
        INSERT INTO Bookings
        (
            PNR,
            TripId,
            CustomerId,
            TotalAmount,
            BookingStatus,
            CreatedAt,
            ConfirmedAt
        )
        VALUES
        (
            @PNR,
            @TripId,
            @CustomerId,
            @TotalAmount,
            2,
            GETUTCDATE(),
            @ConfirmedAt
        );


        SET @BookingId =
            CONVERT(BIGINT, SCOPE_IDENTITY());


        ---------------------------------------------------------
        -- 12. Create Booking Details
        ---------------------------------------------------------
        INSERT INTO BookingDetails
        (
            BookingId,
            TripSeatId,
            PassengerName,
            PassengerPhone,
            PassengerNID,
            Fare,
            CreatedAt
        )
        SELECT
            @BookingId,
            ls.TripSeatId,
            p.PassengerName,
            p.PassengerPhone,
            p.PassengerNID,
            ls.Fare,
            GETUTCDATE()
        FROM @LockedSeats ls
        INNER JOIN @Passengers p
            ON p.TripSeatId = ls.TripSeatId;


        ---------------------------------------------------------
        -- 13. Create Payment
        ---------------------------------------------------------
        INSERT INTO Payments
        (
            BookingId,
            Amount,
            PaymentMethod,
            TransactionId,
            PaymentStatus,
            PaidAt,
            CreatedAt
        )
        VALUES
        (
            @BookingId,
            @TotalAmount,
            @PaymentMethod,
            @TransactionId,
            2,
            GETUTCDATE(),
            GETUTCDATE()
        );


        ---------------------------------------------------------
        -- 14. Locked -> Booked
        ---------------------------------------------------------
        UPDATE ts
        SET
            Status = 3,
            LockedByCustomerId = NULL,
            LockedUntil = NULL,
            BookedAt = GETUTCDATE()
        FROM TripSeats ts
        INNER JOIN @LockedSeats ls
            ON ls.TripSeatId = ts.Id;


        ---------------------------------------------------------
        -- 15. Commit
        ---------------------------------------------------------
        COMMIT TRANSACTION;


        ---------------------------------------------------------
        -- 16. Return Booking
        ---------------------------------------------------------
        SELECT
            b.Id AS BookingId,
            b.PNR,
            b.TripId,
            b.CustomerId,
            b.TotalAmount,
            b.BookingStatus,
            pay.PaymentStatus,
            pay.PaymentMethod,
            pay.TransactionId,
            b.CreatedAt,
            b.ConfirmedAt
        FROM Bookings b
        INNER JOIN Payments pay
            ON pay.BookingId = b.Id
        WHERE b.Id = @BookingId;


    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_ReleaseExpiredSeatLocks]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ReleasedCount INT = 0;

    UPDATE TripSeats
    SET
        Status = 1, -- Available
        LockedByCustomerId = NULL,
        LockedUntil = NULL
    WHERE Status = 2 -- Locked
      AND LockedUntil IS NOT NULL
      AND LockedUntil <= GETUTCDATE();

    SET @ReleasedCount = @@ROWCOUNT;

    SELECT @ReleasedCount AS ReleasedCount;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_LockSeats]
    @TripId BIGINT,
    @TripSeatIds NVARCHAR(MAX),
    @CustomerId BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        ------------------------------------------------
        -- 1. Validate Customer
        ------------------------------------------------
        IF NOT EXISTS
        (
            SELECT 1
            FROM Customers
            WHERE Id = @CustomerId
              AND IsActive = 1
        )
        BEGIN
            THROW 50100,
                'Customer not found or inactive.',
                1;
        END;


        ------------------------------------------------
        -- 2. Validate Trip
        ------------------------------------------------
        IF NOT EXISTS
        (
            SELECT 1
            FROM Trips
            WHERE Id = @TripId
              AND IsActive = 1
        )
        BEGIN
            THROW 50101,
                'Trip not found or inactive.',
                1;
        END;


        ------------------------------------------------
        -- 3. Parse requested seats
        ------------------------------------------------
        DECLARE @RequestedSeats TABLE
        (
            TripSeatId BIGINT PRIMARY KEY
        );

        INSERT INTO @RequestedSeats
        (
            TripSeatId
        )
        SELECT DISTINCT
            TRY_CAST([value] AS BIGINT)
        FROM OPENJSON(@TripSeatIds)
        WHERE TRY_CAST([value] AS BIGINT) IS NOT NULL;


        ------------------------------------------------
        -- 4. At least one seat required
        ------------------------------------------------
        IF NOT EXISTS
        (
            SELECT 1
            FROM @RequestedSeats
        )
        BEGIN
            THROW 50102,
                'At least one seat is required.',
                1;
        END;


        ------------------------------------------------
        -- 5. Lock rows using UPDLOCK + HOLDLOCK
        ------------------------------------------------
        DECLARE @Seats TABLE
        (
            TripSeatId BIGINT,
            BusSeatId BIGINT,
            SeatNumber NVARCHAR(50),
            Status TINYINT,
            LockedByCustomerId BIGINT NULL,
            LockedUntil DATETIME2 NULL
        );


        INSERT INTO @Seats
        (
            TripSeatId,
            BusSeatId,
            SeatNumber,
            Status,
            LockedByCustomerId,
            LockedUntil
        )
        SELECT
            ts.Id,
            ts.BusSeatId,
            bs.SeatNumber,
            ts.Status,
            ts.LockedByCustomerId,
            ts.LockedUntil
        FROM TripSeats ts WITH (UPDLOCK, HOLDLOCK)
        INNER JOIN BusSeats bs
            ON bs.Id = ts.BusSeatId
        INNER JOIN @RequestedSeats rs
            ON rs.TripSeatId = ts.Id
        WHERE ts.TripId = @TripId;


        ------------------------------------------------
        -- 6. Make sure all requested seats exist
        ------------------------------------------------
        IF
        (
            SELECT COUNT(*)
            FROM @Seats
        )
        <>
        (
            SELECT COUNT(*)
            FROM @RequestedSeats
        )
        BEGIN
            THROW 50103,
                'One or more seats do not belong to this trip.',
                1;
        END;


        ------------------------------------------------
        -- 7. Release expired locks
        ------------------------------------------------
        UPDATE ts
        SET
            Status = 1,
            LockedByCustomerId = NULL,
            LockedUntil = NULL
        FROM TripSeats ts
        INNER JOIN @Seats s
            ON s.TripSeatId = ts.Id
        WHERE ts.Status = 2
          AND ts.LockedUntil <= GETUTCDATE();


        ------------------------------------------------
        -- 8. Check already booked
        ------------------------------------------------
        IF EXISTS
        (
            SELECT 1
            FROM TripSeats ts
            INNER JOIN @RequestedSeats rs
                ON rs.TripSeatId = ts.Id
            WHERE ts.Status = 3
        )
        BEGIN
            THROW 50104,
                'One or more selected seats are already booked.',
                1;
        END;


        ------------------------------------------------
        -- 9. Check locked by another customer
        ------------------------------------------------
        IF EXISTS
        (
            SELECT 1
            FROM TripSeats ts
            INNER JOIN @RequestedSeats rs
                ON rs.TripSeatId = ts.Id
            WHERE ts.Status = 2
              AND ts.LockedByCustomerId <> @CustomerId
              AND ts.LockedUntil > GETUTCDATE()
        )
        BEGIN
            THROW 50105,
                'One or more selected seats are locked by another customer.',
                1;
        END;


        ------------------------------------------------
        -- 10. Lock selected seats
        ------------------------------------------------
        DECLARE @LockedUntil DATETIME2 =
            DATEADD(MINUTE, 10, GETUTCDATE());


        UPDATE ts
        SET
            Status = 2,
            LockedByCustomerId = @CustomerId,
            LockedUntil = @LockedUntil
        FROM TripSeats ts
        INNER JOIN @RequestedSeats rs
            ON rs.TripSeatId = ts.Id;


        ------------------------------------------------
        -- 11. Commit
        ------------------------------------------------
        COMMIT TRANSACTION;


        ------------------------------------------------
        -- 12. Return locked seats
        ------------------------------------------------
        SELECT
            ts.Id AS TripSeatId,
            ts.TripId,
            ts.BusSeatId,
            bs.SeatNumber,
            ts.Status,
            ts.LockedUntil
        FROM TripSeats ts
        INNER JOIN BusSeats bs
            ON bs.Id = ts.BusSeatId
        INNER JOIN @RequestedSeats rs
            ON rs.TripSeatId = ts.Id
        WHERE ts.LockedByCustomerId = @CustomerId
          AND ts.Status = 2;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE SP_Bus_Create
    @BusName NVARCHAR(150),
    @BusNumber NVARCHAR(50),
    @TotalSeats INT,
    @CreatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @TotalSeats <= 0
    BEGIN
        THROW 50010,
            'Total seats must be greater than zero.',
            1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM Buses
        WHERE BusNumber = @BusNumber
    )
    BEGIN
        THROW 50011,
            'Bus number already exists.',
            1;
    END;

    INSERT INTO Buses
    (
        BusName,
        BusNumber,
        TotalSeats,
        IsActive,
        CreatedAt,
        CreatedBy
    )
    VALUES
    (
        @BusName,
        @BusNumber,
        @TotalSeats,
        1,
        GETUTCDATE(),
        @CreatedBy
    );

    DECLARE @Id INT = SCOPE_IDENTITY();

    SELECT
        Id,
        BusName,
        BusNumber,
        TotalSeats,
        IsActive,
        CreatedAt,
        CreatedBy,
        UpdatedAt,
        UpdatedBy
    FROM Buses
    WHERE Id = @Id;
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE SP_Bus_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        BusName,
        BusNumber,
        TotalSeats,
        IsActive,
        CreatedAt,
        CreatedBy,
        UpdatedAt,
        UpdatedBy
    FROM Buses
    ORDER BY Id DESC;
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE SP_Bus_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        BusName,
        BusNumber,
        TotalSeats,
        IsActive,
        CreatedAt,
        CreatedBy,
        UpdatedAt,
        UpdatedBy
    FROM Buses
    WHERE Id = @Id;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE SP_Bus_Update
    @Id INT,
    @BusName NVARCHAR(150),
    @BusNumber NVARCHAR(50),
    @TotalSeats INT,
    @IsActive BIT,
    @UpdatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @TotalSeats <= 0
    BEGIN
        THROW 50012,
            'Total seats must be greater than zero.',
            1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Buses
        WHERE Id = @Id
    )
    BEGIN
        THROW 50013,
            'Bus not found.',
            1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM Buses
        WHERE BusNumber = @BusNumber
          AND Id <> @Id
    )
    BEGIN
        THROW 50014,
            'Bus number already exists.',
            1;
    END;

    UPDATE Buses
    SET
        BusName = @BusName,
        BusNumber = @BusNumber,
        TotalSeats = @TotalSeats,
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE(),
        UpdatedBy = @UpdatedBy
    WHERE Id = @Id;

    SELECT
        Id,
        BusName,
        BusNumber,
        TotalSeats,
        IsActive,
        CreatedAt,
        CreatedBy,
        UpdatedAt,
        UpdatedBy
    FROM Buses
    WHERE Id = @Id;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE SP_Bus_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Buses
        WHERE Id = @Id
    )
    BEGIN
        THROW 50015,
            'Bus not found.',
            1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM BusSeats
        WHERE BusId = @Id
    )
    BEGIN
        THROW 50016,
            'Cannot delete bus because seats are configured for this bus.',
            1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM Trips
        WHERE BusId = @Id
    )
    BEGIN
        THROW 50017,
            'Cannot delete bus because trips exist for this bus.',
            1;
    END;

    DELETE FROM Buses
    WHERE Id = @Id;

    SELECT 1 AS Result;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE SP_Bus_ChangeStatus
    @Id INT,
    @IsActive BIT,
    @UpdatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Buses
        WHERE Id = @Id
    )
    BEGIN
        THROW 50018,
            'Bus not found.',
            1;
    END;

    UPDATE Buses
    SET
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE(),
        UpdatedBy = @UpdatedBy
    WHERE Id = @Id;

    SELECT
        Id,
        BusName,
        BusNumber,
        TotalSeats,
        IsActive,
        CreatedAt,
        CreatedBy,
        UpdatedAt,
        UpdatedBy
    FROM Buses
    WHERE Id = @Id;
END;
GO