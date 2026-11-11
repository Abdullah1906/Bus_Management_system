using BPS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Entities
{
    public class Staff
    {
        public int StaffId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? AlternativeContact { get; set; }

        public string? PresentAddress { get; set; }

        public string? PermanentAddress { get; set; }

        public string NIDNumber { get; set; } = string.Empty;

        public string? PassportOrBirthCertificate { get; set; }

        public StaffRole RoleType { get; set; }

        public DateOnly JoiningDate { get; set; }

        public string? DrivingLicenseNumber { get; set; }

        public DateOnly? LicenseExpiryDate { get; set; }

        public LicenseType? LicenseType { get; set; }

        public decimal BaseSalary { get; set; }

        public string? BankName { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? MobileBankingProvider { get; set; }

        public string? MobileBankingNumber { get; set; }

        public StaffStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
