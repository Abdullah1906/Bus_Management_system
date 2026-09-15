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

ALTER PROCEDURE [dbo].[SP_CreateTripSchedule]
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
        -- 5. Check Bus Schedule Conflict (With 2 Hours Buffer)
        ------------------------------------------------
        DECLARE @BaseDate DATETIME2 = CAST(@TripDate AS DATETIME2);
        DECLARE @NewStart DATETIME2 = DATEADD(second, DATEDIFF(second, '00:00:00', @DepartureTime), @BaseDate);
        DECLARE @NewEnd DATETIME2;

        IF @ArrivalTime IS NOT NULL
        BEGIN
            -- Handle overnight arrival (e.g. 22:00 to 04:30 next day)
            IF @ArrivalTime < @DepartureTime
            BEGIN
                SET @NewEnd = DATEADD(second, DATEDIFF(second, '00:00:00', @ArrivalTime), DATEADD(DAY, 1, @BaseDate));
            END
            ELSE
            BEGIN
                SET @NewEnd = DATEADD(second, DATEDIFF(second, '00:00:00', @ArrivalTime), @BaseDate);
            END
        END
        ELSE
        BEGIN
            -- Default 6 hours duration if ArrivalTime is NULL
            SET @NewEnd = DATEADD(HOUR, 6, @NewStart);
        END

        IF EXISTS
        (
            SELECT 1
            FROM Trips t
            CROSS APPLY (
                SELECT 
                    DATEADD(second, DATEDIFF(second, '00:00:00', t.DepartureTime), CAST(t.TripDate AS DATETIME2)) AS ExistingStart,
                    CASE 
                        WHEN t.ArrivalTime IS NULL 
                            THEN DATEADD(HOUR, 6, DATEADD(second, DATEDIFF(second, '00:00:00', t.DepartureTime), CAST(t.TripDate AS DATETIME2)))
                        WHEN t.ArrivalTime < t.DepartureTime 
                            THEN DATEADD(second, DATEDIFF(second, '00:00:00', t.ArrivalTime), DATEADD(DAY, 1, CAST(t.TripDate AS DATETIME2)))
                        ELSE DATEADD(second, DATEDIFF(second, '00:00:00', t.ArrivalTime), CAST(t.TripDate AS DATETIME2))
                    END AS ExistingEnd
            ) calc
            WHERE t.BusId = @BusId
              AND t.IsActive = 1
              AND ISNULL(t.IsDeleted, 0) = 0
              -- Overlap check logic with 2 Hours Rest Buffer
              AND (@NewStart < DATEADD(HOUR, 2, calc.ExistingEnd))
              AND (@NewEnd > DATEADD(HOUR, -2, calc.ExistingStart))
        )
        BEGIN
            THROW 50005, 'Selected bus is already scheduled or requires a 2-hour rest buffer between trips.', 1;
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

ALTER PROCEDURE [dbo].[SP_ConfirmBooking]
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
        IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @CustomerId AND IsActive = 1)
        BEGIN
            THROW 50200, 'Admin Counter not found or inactive.', 1;
        END;

        ---------------------------------------------------------
        -- 2. Validate Trip
        ---------------------------------------------------------
        IF NOT EXISTS (SELECT 1 FROM Trips WHERE Id = @TripId AND IsActive = 1)
        BEGIN
            THROW 50201, 'Trip not found or inactive.', 1;
        END;

        ---------------------------------------------------------
        -- 3. Validate JSON
        ---------------------------------------------------------
        IF @PassengersJson IS NULL OR ISJSON(@PassengersJson) <> 1
        BEGIN
            THROW 50202, 'Invalid passenger information.', 1;
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

        INSERT INTO @Passengers (TripSeatId, PassengerName, PassengerPhone, PassengerNID)
        SELECT TripSeatId, PassengerName, PassengerPhone, PassengerNID
        FROM OPENJSON(@PassengersJson)
        WITH
        (
            TripSeatId BIGINT '$.TripSeatId',
            PassengerName NVARCHAR(150) '$.PassengerName',
            PassengerPhone NVARCHAR(30) '$.PassengerPhone',
            PassengerNID NVARCHAR(50) '$.PassengerNID'
        );

        ---------------------------------------------------------
        -- 5. Validate passenger count
        ---------------------------------------------------------
        IF NOT EXISTS (SELECT 1 FROM @Passengers)
        BEGIN
            THROW 50203, 'At least one passenger is required.', 1;
        END;

        ---------------------------------------------------------
        -- 6. Validate passenger information
        ---------------------------------------------------------
        IF EXISTS
        (
            SELECT 1 FROM @Passengers
            WHERE PassengerName IS NULL OR LTRIM(RTRIM(PassengerName)) = ''
               OR PassengerPhone IS NULL OR LTRIM(RTRIM(PassengerPhone)) = ''
        )
        BEGIN
            THROW 50204, 'Passenger name and phone are required.', 1;
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

        INSERT INTO @LockedSeats (TripSeatId, BusSeatId, SeatNumber, Fare)
        SELECT ts.Id, ts.BusSeatId, bs.SeatNumber, t.Fare
        FROM TripSeats ts WITH (UPDLOCK, HOLDLOCK)
        INNER JOIN Trips t ON t.Id = ts.TripId
        INNER JOIN BusSeats bs ON bs.Id = ts.BusSeatId
        INNER JOIN @Passengers p ON p.TripSeatId = ts.Id
        WHERE ts.TripId = @TripId
          AND ts.Status = 2
          AND ts.LockedByCustomerId = @CustomerId
          AND ts.LockedUntil > GETUTCDATE();

        ---------------------------------------------------------
        -- 8. Every requested seat must be valid and locked
        ---------------------------------------------------------
        IF (SELECT COUNT(*) FROM @LockedSeats) <> (SELECT COUNT(*) FROM @Passengers)
        BEGIN
            THROW 50205, 'One or more seats are not locked by this customer or the lock has expired.', 1;
        END;

        ---------------------------------------------------------
        -- 9. Calculate total
        ---------------------------------------------------------
        SELECT @TotalAmount = SUM(Fare) FROM @LockedSeats;

        ---------------------------------------------------------
        -- 10. Generate Unique PNR
        ---------------------------------------------------------
        DECLARE @IsPnrUnique BIT = 0;
        WHILE @IsPnrUnique = 0
        BEGIN
            SET @PNR = 'BPS' + 
                       CONVERT(CHAR(8), GETUTCDATE(), 112) + 
                       UPPER(SUBSTRING(REPLACE(CONVERT(VARCHAR(36), NEWID()), '-', ''), 1, 6));

            IF NOT EXISTS (SELECT 1 FROM Bookings WHERE PNR = @PNR)
            BEGIN
                SET @IsPnrUnique = 1;
            END
        END;

        ---------------------------------------------------------
        -- 11. Create Booking
        ---------------------------------------------------------
        INSERT INTO Bookings (PNR, TripId, CustomerId, TotalAmount, BookingStatus, CreatedAt, ConfirmedAt)
        VALUES (@PNR, @TripId, @CustomerId, @TotalAmount, 2, GETUTCDATE(), @ConfirmedAt);

        SET @BookingId = CONVERT(BIGINT, SCOPE_IDENTITY());

        ---------------------------------------------------------
        -- 12. Create Booking Details
        ---------------------------------------------------------
        INSERT INTO BookingDetails (BookingId, TripSeatId, PassengerName, PassengerPhone, PassengerNID, Fare, CreatedAt)
        SELECT @BookingId, ls.TripSeatId, p.PassengerName, p.PassengerPhone, p.PassengerNID, ls.Fare, GETUTCDATE()
        FROM @LockedSeats ls
        INNER JOIN @Passengers p ON p.TripSeatId = ls.TripSeatId;

        ---------------------------------------------------------
        -- 13. Create Payment
        ---------------------------------------------------------
        INSERT INTO Payments (BookingId, Amount, PaymentMethod, TransactionId, PaymentStatus, PaidAt, CreatedAt)
        VALUES (@BookingId, @TotalAmount, @PaymentMethod, @TransactionId, 2, GETUTCDATE(), GETUTCDATE());

        ---------------------------------------------------------
        -- 14. Locked -> Booked
        ---------------------------------------------------------
        UPDATE ts
        SET Status = 3,
            LockedByCustomerId = NULL,
            LockedUntil = NULL,
            BookedAt = GETUTCDATE()
        FROM TripSeats ts
        INNER JOIN @LockedSeats ls ON ls.TripSeatId = ts.Id;

        ---------------------------------------------------------
        -- 15. Commit
        ---------------------------------------------------------
        COMMIT TRANSACTION;

        ---------------------------------------------------------
        -- 16. Return Booking (RESULT SET 1)
        ---------------------------------------------------------
        SELECT
            CAST(b.Id AS BIGINT) AS BookingId,
            b.PNR,
            CAST(b.TripId AS BIGINT) AS TripId,
            CAST(b.CustomerId AS BIGINT) AS CustomerId,
            b.TotalAmount,
            CAST(b.BookingStatus AS TINYINT) AS BookingStatus,
            CAST(pay.PaymentStatus AS TINYINT) AS PaymentStatus,
            pay.PaymentMethod,
            pay.TransactionId,
            b.CreatedAt,
            b.ConfirmedAt,
			r.FromPlace AS FromPlaceName,
            r.ToPlace AS ToPlaceName,
            t.TripDate
        FROM Bookings b
        INNER JOIN Payments pay ON pay.BookingId = b.Id
		INNER JOIN Trips t ON t.Id = b.TripId
		INNER JOIN Routes r ON r.Id = t.RouteId

        WHERE b.Id = @BookingId;

        ---------------------------------------------------------
        -- 17. Return Passengers (RESULT SET 2 - যুক্ত করা হলো)
        ---------------------------------------------------------
        SELECT
            bd.TripSeatId,
            ls.SeatNumber,
            bd.PassengerName,
            bd.PassengerPhone,
            bd.PassengerNID,
            bd.Fare
        FROM BookingDetails bd
        INNER JOIN @LockedSeats ls ON ls.TripSeatId = bd.TripSeatId
        WHERE bd.BookingId = @BookingId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;



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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE SP_BusSeat_Create
    @BusId INT,
    @SeatNumber NVARCHAR(20),
    @RowNumber INT,
    @ColumnNumber INT,
    @IsWindow BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM Buses
        WHERE Id = @BusId
          AND IsActive = 1
    )
    BEGIN
        THROW 50100,
            'Bus not found or inactive.',
            1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM BusSeats
        WHERE BusId = @BusId
          AND SeatNumber = @SeatNumber
    )
    BEGIN
        THROW 50101,
            'Seat number already exists for this bus.',
            1;
    END;

    IF @RowNumber <= 0
    BEGIN
        THROW 50102,
            'Row number must be greater than zero.',
            1;
    END;

    IF @ColumnNumber <= 0
    BEGIN
        THROW 50103,
            'Column number must be greater than zero.',
            1;
    END;

    INSERT INTO BusSeats
    (
        BusId,
        SeatNumber,
        RowNumber,
        ColumnNumber,
        IsWindow,
        IsActive,
        CreatedAt
    )
    VALUES
    (
        @BusId,
        @SeatNumber,
        @RowNumber,
        @ColumnNumber,
        @IsWindow,
        1,
        GETUTCDATE()
    );

    DECLARE @Id BIGINT =
        SCOPE_IDENTITY();

    SELECT
        Id,
        BusId,
        SeatNumber,
        RowNumber,
        ColumnNumber,
        IsWindow,
        IsActive,
        CreatedAt
    FROM BusSeats
    WHERE Id = @Id;
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE SP_BusSeat_GetByBusId
    @BusId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        BusId,
        SeatNumber,
        RowNumber,
        ColumnNumber,
        IsWindow,
        IsActive,
        CreatedAt
    FROM BusSeats
    WHERE BusId = @BusId
    ORDER BY
        RowNumber,
        ColumnNumber;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE SP_BusSeat_GetById
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        BusId,
        SeatNumber,
        RowNumber,
        ColumnNumber,
        IsWindow,
        IsActive,
        CreatedAt
    FROM BusSeats
    WHERE Id = @Id;
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE SP_BusSeat_Update
    @Id BIGINT,
    @SeatNumber NVARCHAR(20),
    @RowNumber INT,
    @ColumnNumber INT,
    @IsWindow BIT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @BusId INT;

    SELECT
        @BusId = BusId
    FROM BusSeats
    WHERE Id = @Id;

    IF @BusId IS NULL
    BEGIN
        THROW 50104,
            'Seat not found.',
            1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM BusSeats
        WHERE BusId = @BusId
          AND SeatNumber = @SeatNumber
          AND Id <> @Id
    )
    BEGIN
        THROW 50105,
            'Seat number already exists for this bus.',
            1;
    END;

    UPDATE BusSeats
    SET
        SeatNumber = @SeatNumber,
        RowNumber = @RowNumber,
        ColumnNumber = @ColumnNumber,
        IsWindow = @IsWindow,
        IsActive = @IsActive
    WHERE Id = @Id;

    SELECT
        Id,
        BusId,
        SeatNumber,
        RowNumber,
        ColumnNumber,
        IsWindow,
        IsActive,
        CreatedAt
    FROM BusSeats
    WHERE Id = @Id;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE SP_BusSeat_Delete
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM BusSeats
        WHERE Id = @Id
    )
    BEGIN
        THROW 50106,
            'Seat not found.',
            1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM TripSeats
        WHERE BusSeatId = @Id
    )
    BEGIN
        THROW 50107,
            'Cannot delete seat because it is already used by a trip.',
            1;
    END;

    DELETE FROM BusSeats
    WHERE Id = @Id;

    SELECT 1 AS Result;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE SP_BusSeat_ChangeStatus
    @Id BIGINT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM BusSeats
        WHERE Id = @Id
    )
    BEGIN
        THROW 50108,
            'Seat not found.',
            1;
    END;

    UPDATE BusSeats
    SET
        IsActive = @IsActive
    WHERE Id = @Id;

    SELECT
        Id,
        BusId,
        SeatNumber,
        RowNumber,
        ColumnNumber,
        IsWindow,
        IsActive,
        CreatedAt
    FROM BusSeats
    WHERE Id = @Id;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE dbo.SP_Route_Create
    @FromPlace NVARCHAR(150),
    @ToPlace NVARCHAR(150),
    @DistanceKm DECIMAL(10,2) = NULL,
    @EstimatedMinutes INT = NULL,
    @CreatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @FromPlace = LTRIM(RTRIM(@FromPlace));
    SET @ToPlace = LTRIM(RTRIM(@ToPlace));

    IF NULLIF(@FromPlace, '') IS NULL
    BEGIN
        THROW 50200, 'From place is required.', 1;
    END;

    IF NULLIF(@ToPlace, '') IS NULL
    BEGIN
        THROW 50201, 'To place is required.', 1;
    END;

    IF UPPER(@FromPlace) = UPPER(@ToPlace)
    BEGIN
        THROW 50202, 'From place and To place cannot be same.', 1;
    END;

    IF @DistanceKm IS NOT NULL
       AND @DistanceKm < 0
    BEGIN
        THROW 50203, 'Distance cannot be negative.', 1;
    END;

    IF @EstimatedMinutes IS NOT NULL
       AND @EstimatedMinutes <= 0
    BEGIN
        THROW 50204, 'Estimated minutes must be greater than zero.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Routes
        WHERE UPPER(FromPlace) = UPPER(@FromPlace)
          AND UPPER(ToPlace) = UPPER(@ToPlace)
          AND IsActive = 1
    )
    BEGIN
        THROW 50205, 'This route already exists.', 1;
    END;

    INSERT INTO dbo.Routes
    (
        FromPlace,
        ToPlace,
        DistanceKm,
        EstimatedMinutes,
        IsActive,
        CreatedAt,
        CreatedBy
    )
    VALUES
    (
        @FromPlace,
        @ToPlace,
        @DistanceKm,
        @EstimatedMinutes,
        1,
        GETUTCDATE(),
        @CreatedBy
    );

    DECLARE @Id INT = SCOPE_IDENTITY();

    SELECT
        Id,
        FromPlace,
        ToPlace,
        DistanceKm,
        EstimatedMinutes,
        IsActive,
        CreatedAt,
        CreatedBy,
        UpdatedAt,
        UpdatedBy
    FROM dbo.Routes
    WHERE Id = @Id;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_Route_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        FromPlace,
        ToPlace,
        DistanceKm,
        EstimatedMinutes,
        IsActive,
        CreatedAt,
        CreatedBy,
        UpdatedAt,
        UpdatedBy
    FROM dbo.Routes
    ORDER BY
        FromPlace,
        ToPlace,
        Id DESC;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_Route_GetById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        FromPlace,
        ToPlace,
        DistanceKm,
        EstimatedMinutes,
        IsActive,
        CreatedAt,
        CreatedBy,
        UpdatedAt,
        UpdatedBy
    FROM dbo.Routes
    WHERE Id = @Id;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_Route_Update
    @Id INT,
    @FromPlace NVARCHAR(150),
    @ToPlace NVARCHAR(150),
    @DistanceKm DECIMAL(10,2) = NULL,
    @EstimatedMinutes INT = NULL,
    @IsActive BIT,
    @UpdatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @FromPlace = LTRIM(RTRIM(@FromPlace));
    SET @ToPlace = LTRIM(RTRIM(@ToPlace));

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Routes
        WHERE Id = @Id
    )
    BEGIN
        THROW 50210, 'Route not found.', 1;
    END;

    IF NULLIF(@FromPlace, '') IS NULL
    BEGIN
        THROW 50211, 'From place is required.', 1;
    END;

    IF NULLIF(@ToPlace, '') IS NULL
    BEGIN
        THROW 50212, 'To place is required.', 1;
    END;

    IF UPPER(@FromPlace) = UPPER(@ToPlace)
    BEGIN
        THROW 50213,
            'From place and To place cannot be same.',
            1;
    END;

    IF @DistanceKm IS NOT NULL
       AND @DistanceKm < 0
    BEGIN
        THROW 50214,
            'Distance cannot be negative.',
            1;
    END;

    IF @EstimatedMinutes IS NOT NULL
       AND @EstimatedMinutes <= 0
    BEGIN
        THROW 50215,
            'Estimated minutes must be greater than zero.',
            1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Routes
        WHERE UPPER(FromPlace) = UPPER(@FromPlace)
          AND UPPER(ToPlace) = UPPER(@ToPlace)
          AND Id <> @Id
          AND IsActive = 1
    )
    BEGIN
        THROW 50216,
            'Another active route with same origin and destination exists.',
            1;
    END;

    UPDATE dbo.Routes
    SET
        FromPlace = @FromPlace,
        ToPlace = @ToPlace,
        DistanceKm = @DistanceKm,
        EstimatedMinutes = @EstimatedMinutes,
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE(),
        UpdatedBy = @UpdatedBy
    WHERE Id = @Id;

    SELECT
        Id,
        FromPlace,
        ToPlace,
        DistanceKm,
        EstimatedMinutes,
        IsActive,
        CreatedAt,
        CreatedBy,
        UpdatedAt,
        UpdatedBy
    FROM dbo.Routes
    WHERE Id = @Id;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_Route_Delete
    @Id INT,
    @UpdatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Routes
        WHERE Id = @Id
    )
    BEGIN
        THROW 50220, 'Route not found.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Routes
        WHERE Id = @Id
          AND IsActive = 0
    )
    BEGIN
        THROW 50221, 'Route is already inactive.', 1;
    END;

    UPDATE dbo.Routes
    SET
        IsActive = 0,
        UpdatedAt = GETUTCDATE(),
        UpdatedBy = @UpdatedBy
    WHERE Id = @Id;

    SELECT 1 AS Result;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE dbo.SP_Route_ChangeStatus
    @Id INT,
    @IsActive BIT,
    @UpdatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Routes
        WHERE Id = @Id
    )
    BEGIN
        THROW 50230, 'Route not found.', 1;
    END;

    IF @IsActive = 1
       AND EXISTS
       (
           SELECT 1
           FROM dbo.Routes r
           WHERE r.Id <> @Id
             AND UPPER(r.FromPlace) =
                 UPPER(
                    (SELECT FromPlace
                     FROM dbo.Routes
                     WHERE Id = @Id)
                 )
             AND UPPER(r.ToPlace) =
                 UPPER(
                    (SELECT ToPlace
                     FROM dbo.Routes
                     WHERE Id = @Id)
                 )
             AND r.IsActive = 1
       )
    BEGIN
        THROW 50231,
            'Another active route with same origin and destination exists.',
            1;
    END;

    UPDATE dbo.Routes
    SET
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE(),
        UpdatedBy = @UpdatedBy
    WHERE Id = @Id;

    SELECT
        Id,
        FromPlace,
        ToPlace,
        DistanceKm,
        EstimatedMinutes,
        IsActive,
        CreatedAt,
        CreatedBy,
        UpdatedAt,
        UpdatedBy
    FROM dbo.Routes
    WHERE Id = @Id;
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[SP_TripSchedule_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @TodayDate DATE = CAST(GETUTCDATE() AS DATE);
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

    FROM dbo.Trips t

    INNER JOIN dbo.Buses b
        ON b.Id = t.BusId

    INNER JOIN dbo.Routes r
        ON r.Id = t.RouteId
	WHERE ISNULL(t.IsDeleted, 0) = 0 AND CAST(t.TripDate AS DATE) >= @TodayDate
    ORDER BY
        t.TripDate ASC,
        t.DepartureTime ASC,
        t.Id DESC;
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_TripSchedule_GetById]
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

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
    FROM dbo.Trips t
    INNER JOIN dbo.Buses b
        ON b.Id = t.BusId
    INNER JOIN dbo.Routes r
        ON r.Id = t.RouteId
    WHERE t.Id = @Id AND ISNULL(t.IsDeleted, 0) = 0;
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_TripSchedule_Update]
    @Id BIGINT,
    @BusId INT,
    @RouteId INT,
    @TripDate DATE,
    @DepartureTime TIME,
    @ArrivalTime TIME = NULL,
    @Fare DECIMAL(18,2),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
	IF EXISTS (SELECT 1 FROM TripSeats WHERE TripId = @Id)
	   AND (
			@BusId <> (SELECT BusId FROM Trips WHERE Id = @Id)
			OR @TripDate <> (SELECT TripDate FROM Trips WHERE Id = @Id)
			OR @DepartureTime <> (SELECT DepartureTime FROM Trips WHERE Id = @Id)
	   )
	BEGIN
		THROW 50006,
			'Cannot change bus, trip date or departure time after seats are generated. Create a new trip instead.',
			1;
	END;
    UPDATE dbo.Trips
    SET
        BusId = @BusId,
        RouteId = @RouteId,
        TripDate = @TripDate,
        DepartureTime = @DepartureTime,
        ArrivalTime = @ArrivalTime,
        Fare = @Fare,
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id AND ISNULL(IsDeleted, 0) = 0;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT CAST(0 AS BIT) AS Success;
        RETURN;
    END;

    SELECT CAST(1 AS BIT) AS Success;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_TripSchedule_Delete]
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM dbo.Bookings
        WHERE TripId = @Id and CreatedAt >= GETUTCDATE()
    )
    BEGIN
        THROW 50001, 'Trip cannot be deleted because booking exists.', 1;
    END;

    UPDATE dbo.Trips
    SET IsDeleted = 1,
        IsActive = 0
    WHERE Id = @Id;

    SELECT
        CAST(
            CASE
                WHEN @@ROWCOUNT > 0 THEN 1
                ELSE 0
            END
        AS BIT) AS Success;
END;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_TripSchedule_ChangeStatus]
    @Id BIGINT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Trips
    SET
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id AND ISNULL(IsDeleted, 0) = 0;

    SELECT CAST(
        CASE
            WHEN @@ROWCOUNT > 0 THEN 1
            ELSE 0
        END
    AS BIT) AS Success;
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_Trip_Search]
    @FromPlace NVARCHAR(150),
    @ToPlace NVARCHAR(150),
    @TripDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.Id AS TripId,

        b.Id AS BusId,
        b.BusName,
        b.BusNumber,

        r.Id AS RouteId,
        r.FromPlace,
        r.ToPlace,

        t.TripDate,
        t.DepartureTime,
        t.ArrivalTime,
        t.Fare,

        b.TotalSeats,

        COUNT(
            CASE
                WHEN ts.Status = 1 THEN 1
            END
        ) AS AvailableSeats

    FROM Trips t

    INNER JOIN Buses b
        ON b.Id = t.BusId

    INNER JOIN Routes r
        ON r.Id = t.RouteId

    LEFT JOIN TripSeats ts
        ON ts.TripId = t.Id

    WHERE
        t.IsActive = 1
        AND b.IsActive = 1
        AND r.IsActive = 1
        AND t.TripDate = @TripDate
        AND r.FromPlace = @FromPlace
        AND r.ToPlace = @ToPlace
		AND (
			 t.TripDate > CAST(GETUTCDATE() AS DATE)
			 OR (
				  t.TripDate = CAST(GETUTCDATE() AS DATE)
				  AND t.DepartureTime > CAST(GETUTCDATE() AS TIME)
			 )
		)

    GROUP BY
        t.Id,
        b.Id,
        b.BusName,
        b.BusNumber,
        r.Id,
        r.FromPlace,
        r.ToPlace,
        t.TripDate,
        t.DepartureTime,
        t.ArrivalTime,
        t.Fare,
        b.TotalSeats

    ORDER BY
        t.DepartureTime;
END;

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE SP_Trip_GetSeats
    @TripId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate trip
    IF NOT EXISTS
    (
        SELECT 1
        FROM Trips
        WHERE Id = @TripId
          AND IsActive = 1
    )
    BEGIN
        THROW 50001, 'Trip not found or inactive.', 1;
    END;

    -- Release expired locks
    UPDATE TripSeats
    SET
        Status = 1,
        LockedByCustomerId = NULL,
        LockedUntil = NULL
    WHERE TripId = @TripId
      AND Status = 2
      AND LockedUntil IS NOT NULL
      AND LockedUntil <= GETUTCDATE();

    SELECT
        ts.Id AS TripSeatId,
        ts.BusSeatId,
        bs.SeatNumber,
        bs.RowNumber,
        bs.ColumnNumber,
        bs.IsWindow,
        ts.Status

    FROM TripSeats ts

    INNER JOIN BusSeats bs
        ON bs.Id = ts.BusSeatId

    WHERE
        ts.TripId = @TripId
        AND bs.IsActive = 1

    ORDER BY
        bs.RowNumber,
        bs.ColumnNumber;
END;
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[SP_Trip_CreateMultiple]
(
    @PlaceIds dbo.PlaceIdTable READONLY,
    @TripDate DATE,
    @TipStatus BIT,
    @TipAmount DECIMAL(18,2),
    @CreatedBy NVARCHAR(100) = NULL
)
AS
BEGIN

    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        -- Validate Place IDs
        IF EXISTS
        (
            SELECT 1
            FROM @PlaceIds ids
            LEFT JOIN Places p
                ON p.Id = ids.PlaceId
               AND p.IsActive = 1
            WHERE p.Id IS NULL
        )
        BEGIN
            THROW 50001,
                'One or more places were not found or inactive.',
                1;
        END;


        -- Tip OFF
        IF @TipStatus = 0
        BEGIN
            SET @TipAmount = 0;
        END;


        -- Tip validation
        IF @TipAmount < 0
        BEGIN
            THROW 50002,
                'Tip amount cannot be negative.',
                1;
        END;


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
        SELECT
            p.Id,
            @TripDate,
            @TipStatus,
            @TipAmount,
            p.PricePerTrip,
            GETUTCDATE(),
            @CreatedBy,
            NULL,
            NULL,
            1
        FROM @PlaceIds ids
        INNER JOIN Places p
            ON p.Id = ids.PlaceId
        WHERE p.IsActive = 1;


        -- Return created records
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

        INNER JOIN @PlaceIds ids
            ON ids.PlaceId = tr.PlaceId

        WHERE tr.TripDate = @TripDate
          AND tr.CreatedBy = @CreatedBy

        ORDER BY tr.Id;


        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH

END;
GO