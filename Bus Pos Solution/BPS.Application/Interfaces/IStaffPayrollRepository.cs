using BPS.Application.DTOs.StaffPayroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IStaffPayrollRepository
    {
        Task<int> CreateStaffAsync(
            CreateStaffRequest request,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<StaffResponse>> GetStaffListAsync(
            StaffListRequest request,
            CancellationToken cancellationToken = default);

        Task<int> GenerateMonthlyPayrollAsync(
            DateOnly payrollMonth,
            CancellationToken cancellationToken = default);

        Task<MonthlyPayrollResponse> GetMonthlyPayrollAsync(
            DateOnly payrollMonth,
            string? roleType,
            string? paymentStatus,
            string? search,
            CancellationToken cancellationToken = default);

        Task UpdatePayrollAdjustmentAsync(
            long paymentId,
            UpdatePayrollAdjustmentRequest request,
            CancellationToken cancellationToken = default);

        Task<PayrollResponse> ProcessSalaryPaymentAsync(
            long paymentId,
            ProcessSalaryPaymentRequest request,
            int paidByUserId,
            CancellationToken cancellationToken = default);


        Task<StaffDetailsResponse?> GetStaffByIdAsync(
            int staffId,
            CancellationToken cancellationToken = default);

        Task UpdateStaffAsync(
            int staffId,
            UpdateStaffRequest request,
            CancellationToken cancellationToken = default);

        Task UpdateStaffStatusAsync(
            int staffId,
            string status,
            CancellationToken cancellationToken = default);

        Task DeactivateStaffAsync(
            int staffId,
            CancellationToken cancellationToken = default);

    }
}
