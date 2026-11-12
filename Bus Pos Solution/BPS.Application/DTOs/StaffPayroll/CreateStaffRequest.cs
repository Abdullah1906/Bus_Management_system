using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.StaffPayroll
{
    public class CreateStaffRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string? AlternativeContact { get; set; }
        public string? PresentAddress { get; set; }
        public string? PermanentAddress { get; set; }

        public string NIDNumber { get; set; } = string.Empty;

        public string? PassportOrBirthCertificate { get; set; }

        public string RoleType { get; set; } = string.Empty;

        public DateOnly JoiningDate { get; set; }

        public string? DrivingLicenseNumber { get; set; }
        public DateOnly? LicenseExpiryDate { get; set; }
        public string? LicenseType { get; set; }

        public decimal BaseSalary { get; set; }

        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }

        public string? MobileBankingProvider { get; set; }
        public string? MobileBankingNumber { get; set; }
    }
}
