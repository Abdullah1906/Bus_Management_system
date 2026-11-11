
USE BPS;
GO

/* ============================================
   1. STAFF MASTER TABLE
============================================ */

CREATE TABLE dbo.Staff
(
    StaffId INT IDENTITY(1,1) PRIMARY KEY,

    -- Personal Details
    FullName NVARCHAR(150) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    AlternativeContact NVARCHAR(20) NULL,
    PresentAddress NVARCHAR(500) NULL,
    PermanentAddress NVARCHAR(500) NULL,

    -- Identification
    NIDNumber NVARCHAR(30) NOT NULL,
    PassportOrBirthCertificate NVARCHAR(50) NULL,

    -- Professional Details
    RoleType NVARCHAR(30) NOT NULL,
    JoiningDate DATE NOT NULL,

    -- Driving License
    DrivingLicenseNumber NVARCHAR(50) NULL,
    LicenseExpiryDate DATE NULL,
    LicenseType NVARCHAR(20) NULL,

    -- Financial Details
    BaseSalary DECIMAL(18,2) NOT NULL,
    BankName NVARCHAR(100) NULL,
    BankAccountNumber NVARCHAR(100) NULL,
    MobileBankingProvider NVARCHAR(50) NULL,
    MobileBankingNumber NVARCHAR(20) NULL,

    -- Status
    Status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Staff_Status DEFAULT 'Active',

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_Staff_CreatedAt DEFAULT SYSDATETIME(),

    UpdatedAt DATETIME2(0) NULL,

    CONSTRAINT CK_Staff_RoleType
        CHECK (RoleType IN
        ('Driver', 'Supervisor', 'Helper', 'Office Staff')),

    CONSTRAINT CK_Staff_Status
        CHECK (Status IN ('Active', 'Inactive')),

    CONSTRAINT CK_Staff_Salary
        CHECK (BaseSalary >= 0),

    CONSTRAINT CK_Staff_License
        CHECK
        (
            (
                RoleType = 'Driver'
                AND DrivingLicenseNumber IS NOT NULL
                AND LicenseExpiryDate IS NOT NULL
                AND LicenseType IN ('Heavy', 'Medium')
            )
            OR
            (
                RoleType <> 'Driver'
                AND DrivingLicenseNumber IS NULL
                AND LicenseExpiryDate IS NULL
                AND LicenseType IS NULL
            )
        )
);
GO

/* ============================================
   2. MONTHLY SALARY PAYMENT TABLE
============================================ */

CREATE TABLE dbo.StaffSalaryPayments
(
    PaymentId BIGINT IDENTITY(1,1) PRIMARY KEY,

    StaffId INT NOT NULL,

    -- Always first date of a month
    PayrollMonth DATE NOT NULL,

    -- Salary Snapshot
    BaseSalary DECIMAL(18,2) NOT NULL,

    BonusAmount DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_Payroll_Bonus DEFAULT 0,

    DeductionAmount DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_Payroll_Deduction DEFAULT 0,

    NetPayable AS
        (BaseSalary + BonusAmount - DeductionAmount) PERSISTED,

    AdjustmentRemarks NVARCHAR(500) NULL,

    PaymentStatus NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Payroll_Status DEFAULT 'Unpaid',

    PaymentMethod NVARCHAR(30) NULL,

    TransactionRef NVARCHAR(150) NULL,

    PaidAt DATETIME2(0) NULL,

    CreatedAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_Payroll_CreatedAt DEFAULT SYSDATETIME(),

    UpdatedAt DATETIME2(0) NULL,

    CONSTRAINT FK_Payroll_Staff
        FOREIGN KEY (StaffId)
        REFERENCES dbo.Staff(StaffId),

    CONSTRAINT UQ_Staff_PayrollMonth
        UNIQUE (StaffId, PayrollMonth),

    CONSTRAINT CK_Payroll_Month
        CHECK (DAY(PayrollMonth) = 1),

    CONSTRAINT CK_Payroll_Amounts
        CHECK
        (
            BaseSalary >= 0
            AND BonusAmount >= 0
            AND DeductionAmount >= 0
            AND BaseSalary + BonusAmount >= DeductionAmount
        ),

    CONSTRAINT CK_Payroll_Status
        CHECK (PaymentStatus IN ('Paid', 'Unpaid')),

    CONSTRAINT CK_Payroll_Method
        CHECK
        (
            PaymentMethod IS NULL
            OR PaymentMethod IN ('Cash', 'Bkash', 'Bank Transfer')
        ),

    CONSTRAINT CK_Payroll_PaidDetails
        CHECK
        (
            (
                PaymentStatus = 'Unpaid'
                AND PaidAt IS NULL
                AND PaymentMethod IS NULL
            )
            OR
            (
                PaymentStatus = 'Paid'
                AND PaidAt IS NOT NULL
                AND PaymentMethod IS NOT NULL
            )
        )
);
GO

/* ============================================
   3. SALARY PAYMENT AUDIT LOG
============================================ */

CREATE TABLE dbo.SalaryPaymentLogs
(
    LogId BIGINT IDENTITY(1,1) PRIMARY KEY,

    PaymentId BIGINT NOT NULL,

    PaymentMethod NVARCHAR(30) NOT NULL,

    Amount DECIMAL(18,2) NOT NULL,

    TransactionRef NVARCHAR(150) NULL,

    PaidByUserId INT NULL,

    PaidAt DATETIME2(0) NOT NULL
        CONSTRAINT DF_SalaryLog_PaidAt DEFAULT SYSDATETIME(),

    Remarks NVARCHAR(500) NULL,

    CONSTRAINT FK_SalaryLog_Payment
        FOREIGN KEY (PaymentId)
        REFERENCES dbo.StaffSalaryPayments(PaymentId),

    CONSTRAINT CK_SalaryLog_Amount
        CHECK (Amount >= 0),

    CONSTRAINT CK_SalaryLog_Method
        CHECK
        (
            PaymentMethod IN ('Cash', 'Bkash', 'Bank Transfer')
        )
);
GO
