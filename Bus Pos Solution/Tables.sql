USE [BPS]
GO

-- =============================================
-- Users
-- =============================================

CREATE TABLE Users
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    Username NVARCHAR(100) NOT NULL,

    PasswordHash NVARCHAR(500) NOT NULL,

    FullName NVARCHAR(150) NOT NULL,

    Email NVARCHAR(255) NOT NULL,

    PhoneNumber NVARCHAR(20) NOT NULL,

    Role NVARCHAR(50) NOT NULL
        CONSTRAINT DF_Users_Role DEFAULT 'User',

    IsActive BIT NOT NULL
        CONSTRAINT DF_Users_IsActive DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Users_CreatedAt DEFAULT GETUTCDATE(),

    CreatedBy NVARCHAR(100) NOT NULL
        CONSTRAINT DF_Users_CreatedBy DEFAULT '',

    UpdatedAt DATETIME2 NULL,

    UpdatedBy NVARCHAR(100) NOT NULL
        CONSTRAINT DF_Users_UpdatedBy DEFAULT '',

    CONSTRAINT UQ_Users_Username
        UNIQUE (Username)
);

GO


-- =============================================
-- Places
-- =============================================

CREATE TABLE Places
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    PlaceName NVARCHAR(150) NOT NULL,

    PricePerTrip DECIMAL(18,2) NOT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Places_IsActive DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Places_CreatedAt DEFAULT GETUTCDATE(),

    CreatedBy NVARCHAR(100) NOT NULL
        CONSTRAINT DF_Places_CreatedBy DEFAULT '',

    UpdatedAt DATETIME2 NULL,

    UpdatedBy NVARCHAR(100) NOT NULL
        CONSTRAINT DF_Places_UpdatedBy DEFAULT '',


    CONSTRAINT UQ_Places_PlaceName
        UNIQUE (PlaceName),

    CONSTRAINT CK_Places_Price
        CHECK (PricePerTrip >= 0)
);
GO

CREATE TABLE TripRecords
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    PlaceId INT NOT NULL,

    TripDate DATE NOT NULL,

    TipStatus BIT NOT NULL,

    TipAmount DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_TripRecords_TipAmount DEFAULT 0,

    Price DECIMAL(18,2) NOT NULL,

    Total AS (Price * TipAmount) PERSISTED,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_TripRecords_CreatedAt DEFAULT GETUTCDATE(),

        
    CreatedBy NVARCHAR(100) NOT NULL
        CONSTRAINT DF_TripRecords_CreatedBy DEFAULT '',

    UpdatedAt DATETIME2 NULL,

    UpdatedBy NVARCHAR(100) NULL
        CONSTRAINT DF_TripRecords_UpdatedBy DEFAULT '',

    -- Foreign Key
    CONSTRAINT FK_TripRecords_Places
        FOREIGN KEY (PlaceId)
        REFERENCES Places(Id),

    -- Basic Validations
    CONSTRAINT CK_TripRecords_TipAmount CHECK (TipAmount >= 0),
    CONSTRAINT CK_TripRecords_Price CHECK (Price >= 0),

    -- Tip Rules (TipStatus = 0 হলে TipAmount অবশ্যই 0 হতে হবে)
    CONSTRAINT CK_TripRecords_Tip CHECK 
    (
        (TipStatus = 0 AND TipAmount = 0)
        OR
        (TipStatus = 1 AND TipAmount >= 0)
    )
);
GO


-- =============================================
-- Bus Ticket
-- =============================================

CREATE TABLE Buses
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    BusName NVARCHAR(150) NOT NULL,

    BusNumber NVARCHAR(50) NOT NULL,

    TotalSeats INT NOT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Buses_IsActive DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Buses_CreatedAt
        DEFAULT GETUTCDATE(),

    CreatedBy NVARCHAR(100) NULL,

    UpdatedAt DATETIME2 NULL,

    UpdatedBy NVARCHAR(100) NULL,

    CONSTRAINT UQ_Buses_BusNumber
        UNIQUE (BusNumber),

    CONSTRAINT CK_Buses_TotalSeats
        CHECK (TotalSeats > 0)
);
GO

CREATE TABLE BusSeats
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    BusId INT NOT NULL,

    SeatNumber NVARCHAR(20) NOT NULL,

    RowNumber INT NOT NULL,

    ColumnNumber INT NOT NULL,

    IsWindow BIT NOT NULL
        CONSTRAINT DF_BusSeats_IsWindow DEFAULT 0,

    IsActive BIT NOT NULL
        CONSTRAINT DF_BusSeats_IsActive DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_BusSeats_CreatedAt
        DEFAULT GETUTCDATE(),

    CONSTRAINT FK_BusSeats_Buses
        FOREIGN KEY (BusId)
        REFERENCES Buses(Id),

    CONSTRAINT UQ_BusSeats_Bus_Seat
        UNIQUE (BusId, SeatNumber)
);
GO

CREATE TABLE Routes
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    FromPlace NVARCHAR(150) NOT NULL,

    ToPlace NVARCHAR(150) NOT NULL,

    DistanceKm DECIMAL(10,2) NULL,

    EstimatedMinutes INT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Routes_IsActive DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Routes_CreatedAt
        DEFAULT GETUTCDATE(),

    CreatedBy NVARCHAR(100) NULL,

    UpdatedAt DATETIME2 NULL,

    UpdatedBy NVARCHAR(100) NULL,

    CONSTRAINT CK_Routes_FromTo
        CHECK (FromPlace <> ToPlace)
);
GO

CREATE TABLE Trips
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    BusId INT NOT NULL,

    RouteId INT NOT NULL,

    TripDate DATE NOT NULL,

    DepartureTime TIME NOT NULL,

    ArrivalTime TIME NULL,

    Fare DECIMAL(18,2) NOT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Trips_IsActive DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Trips_CreatedAt
        DEFAULT GETUTCDATE(),

    CreatedBy NVARCHAR(100) NULL,

    UpdatedAt DATETIME2 NULL,

    UpdatedBy NVARCHAR(100) NULL,

    CONSTRAINT FK_Trips_Buses
        FOREIGN KEY (BusId)
        REFERENCES Buses(Id),

    CONSTRAINT FK_Trips_Routes
        FOREIGN KEY (RouteId)
        REFERENCES Routes(Id),

    CONSTRAINT CK_Trips_Fare
        CHECK (Fare >= 0)
);
GO

CREATE TABLE TripSeats
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    TripId BIGINT NOT NULL,

    BusSeatId BIGINT NOT NULL,

    Status TINYINT NOT NULL
        CONSTRAINT DF_TripSeats_Status
        DEFAULT 1,

    LockedByCustomerId BIGINT NULL,

    LockedUntil DATETIME2 NULL,

    BookedAt DATETIME2 NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_TripSeats_CreatedAt
        DEFAULT GETUTCDATE(),

    CONSTRAINT FK_TripSeats_Trips
        FOREIGN KEY (TripId)
        REFERENCES Trips(Id),

    CONSTRAINT FK_TripSeats_BusSeats
        FOREIGN KEY (BusSeatId)
        REFERENCES BusSeats(Id),

    CONSTRAINT CK_TripSeats_Status
        CHECK (Status IN (1, 2, 3))
);
GO

CREATE TABLE Customers
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    FullName NVARCHAR(150) NOT NULL,

    Phone NVARCHAR(30) NOT NULL,

    Email NVARCHAR(150) NULL,

    PasswordHash NVARCHAR(500) NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_Customers_IsActive DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Customers_CreatedAt
        DEFAULT GETUTCDATE(),

    UpdatedAt DATETIME2 NULL,

    CONSTRAINT UQ_Customers_Phone
        UNIQUE (Phone)
);
GO

CREATE TABLE Bookings
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    PNR NVARCHAR(30) NOT NULL,

    TripId BIGINT NOT NULL,

    CustomerId BIGINT NOT NULL,

    TotalAmount DECIMAL(18,2) NOT NULL,

    BookingStatus TINYINT NOT NULL
        CONSTRAINT DF_Bookings_Status
        DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Bookings_CreatedAt
        DEFAULT GETUTCDATE(),

    ConfirmedAt DATETIME2 NULL,

    CONSTRAINT FK_Bookings_Trips
        FOREIGN KEY (TripId)
        REFERENCES Trips(Id),

    CONSTRAINT FK_Bookings_Customers
        FOREIGN KEY (CustomerId)
        REFERENCES Customers(Id),

    CONSTRAINT UQ_Bookings_PNR
        UNIQUE (PNR)
);
GO


CREATE TABLE BookingDetails
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    BookingId BIGINT NOT NULL,

    TripSeatId BIGINT NOT NULL,

    PassengerName NVARCHAR(150) NOT NULL,

    PassengerPhone NVARCHAR(30) NOT NULL,

    PassengerNID NVARCHAR(50) NULL,

    Fare DECIMAL(18,2) NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_BookingDetails_CreatedAt
        DEFAULT GETUTCDATE(),

    CONSTRAINT FK_BookingDetails_Bookings
        FOREIGN KEY (BookingId)
        REFERENCES Bookings(Id),

    CONSTRAINT FK_BookingDetails_TripSeats
        FOREIGN KEY (TripSeatId)
        REFERENCES TripSeats(Id)
);
GO


CREATE TABLE Payments
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    BookingId BIGINT NOT NULL,

    Amount DECIMAL(18,2) NOT NULL,

    PaymentMethod NVARCHAR(50) NOT NULL,

    TransactionId NVARCHAR(100) NULL,

    PaymentStatus TINYINT NOT NULL
        CONSTRAINT DF_Payments_Status
        DEFAULT 1,

    PaidAt DATETIME2 NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Payments_CreatedAt
        DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Payments_Bookings
        FOREIGN KEY (BookingId)
        REFERENCES Bookings(Id),

    CONSTRAINT CK_Payments_Status
        CHECK (PaymentStatus IN (1,2,3))
);
GO