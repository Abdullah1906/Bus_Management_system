using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.StaffPayroll
{
    public class StaffResponse
    {
        public int StaffId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string NIDNumber { get; set; } = string.Empty;

        public string RoleType { get; set; } = string.Empty;

        public DateOnly JoiningDate { get; set; }

        public string? DrivingLicenseNumber { get; set; }

        public DateOnly? LicenseExpiryDate { get; set; }

        public string? LicenseType { get; set; }

        public decimal BaseSalary { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
