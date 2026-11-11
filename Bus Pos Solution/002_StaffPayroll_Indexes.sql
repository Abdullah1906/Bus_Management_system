
USE BPS;
GO

-- Unique NID
CREATE UNIQUE INDEX UX_Staff_NID
ON dbo.Staff(NIDNumber);
GO

-- Unique Driving License
CREATE UNIQUE INDEX UX_Staff_License
ON dbo.Staff(DrivingLicenseNumber)
WHERE DrivingLicenseNumber IS NOT NULL;
GO

-- Staff filtering
CREATE INDEX IX_Staff_Status_Role
ON dbo.Staff(Status, RoleType);
GO

-- Payroll month and payment status
CREATE INDEX IX_Payroll_Month_Status
ON dbo.StaffSalaryPayments
(
    PayrollMonth,
    PaymentStatus
)
INCLUDE
(
    StaffId,
    BaseSalary,
    BonusAmount,
    DeductionAmount
);
GO

-- Payment history
CREATE INDEX IX_SalaryLogs_PaymentId
ON dbo.SalaryPaymentLogs(PaymentId, PaidAt DESC);
GO
