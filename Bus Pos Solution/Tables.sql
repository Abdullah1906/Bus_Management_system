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

    [CreatedBy] INT NULL,

    UpdatedAt DATETIME2 NULL,

    [UpdatedBy] INT NULL,


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

    Total AS (Price + TipAmount) PERSISTED,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_TripRecords_CreatedAt DEFAULT GETUTCDATE(),

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