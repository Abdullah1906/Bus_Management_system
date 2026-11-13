using BPS.Application.DTOs.StaffPayroll;
using BPS.Application.Interfaces;
using BPS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Infrastructure.Repositories
{
    public class StaffPayrollRepository : IStaffPayrollRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public StaffPayrollRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // ==========================================
        // CREATE STAFF
        // ==========================================

        public async Task<int> CreateStaffAsync(
            CreateStaffRequest request,
            CancellationToken cancellationToken = default)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command = new SqlCommand(
                "dbo.sp_CreateStaff",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            AddParameter(command, "@FullName", SqlDbType.NVarChar, request.FullName, 150);
            AddParameter(command, "@PhoneNumber", SqlDbType.NVarChar, request.PhoneNumber, 20);

            AddParameter(command, "@AlternativeContact", SqlDbType.NVarChar, request.AlternativeContact, 20);
            AddParameter(command, "@PresentAddress", SqlDbType.NVarChar, request.PresentAddress, 500);
            AddParameter(command, "@PermanentAddress", SqlDbType.NVarChar, request.PermanentAddress, 500);

            AddParameter(command, "@NIDNumber", SqlDbType.NVarChar, request.NIDNumber, 30);
            AddParameter(command, "@PassportOrBirthCertificate", SqlDbType.NVarChar, request.PassportOrBirthCertificate, 50);

            AddParameter(command, "@RoleType", SqlDbType.NVarChar, request.RoleType, 30);
            AddParameter(command, "@JoiningDate", SqlDbType.Date, request.JoiningDate);

            AddParameter(command, "@DrivingLicenseNumber", SqlDbType.NVarChar, request.DrivingLicenseNumber, 50);
            AddParameter(command, "@LicenseExpiryDate", SqlDbType.Date, request.LicenseExpiryDate);
            AddParameter(command, "@LicenseType", SqlDbType.NVarChar, request.LicenseType, 20);

            command.Parameters.Add("@BaseSalary", SqlDbType.Decimal).Value =
                request.BaseSalary;

            command.Parameters["@BaseSalary"].Precision = 18;
            command.Parameters["@BaseSalary"].Scale = 2;

            AddParameter(command, "@BankName", SqlDbType.NVarChar, request.BankName, 100);
            AddParameter(command, "@BankAccountNumber", SqlDbType.NVarChar, request.BankAccountNumber, 100);
            AddParameter(command, "@MobileBankingProvider", SqlDbType.NVarChar, request.MobileBankingProvider, 50);
            AddParameter(command, "@MobileBankingNumber", SqlDbType.NVarChar, request.MobileBankingNumber, 20);

            await connection.OpenAsync(cancellationToken);

            var result = await command.ExecuteScalarAsync(cancellationToken);

            if (result == null || result == DBNull.Value)
                throw new InvalidOperationException(
                    "Staff creation did not return an ID.");

            return Convert.ToInt32(result);
        }

        // ==========================================
        // GET STAFF LIST
        // ==========================================

        public async Task<PagedResponse<StaffResponse>> GetStaffListAsync(
            StaffListRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = new PagedResponse<StaffResponse>
            {
                Page = request.Page,
                PageSize = request.PageSize
            };

            var items = new List<StaffResponse>();

            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command = new SqlCommand(
                "dbo.sp_GetStaffList",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            AddParameter(command, "@RoleType", SqlDbType.NVarChar, request.RoleType, 30);
            AddParameter(command, "@Status", SqlDbType.NVarChar, request.Status, 20);
            AddParameter(command, "@Search", SqlDbType.NVarChar, request.Search, 150);

            AddParameter(command, "@Page", SqlDbType.Int, request.Page);
            AddParameter(command, "@PageSize", SqlDbType.Int, request.PageSize);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            // First Result: Staff List
            while (await reader.ReadAsync(cancellationToken))
            {
                items.Add(new StaffResponse
                {
                    StaffId = reader.GetInt32(
                        reader.GetOrdinal("StaffId")),

                    FullName = reader.GetString(
                        reader.GetOrdinal("FullName")),

                    PhoneNumber = reader.GetString(
                        reader.GetOrdinal("PhoneNumber")),

                    NIDNumber = reader.GetString(
                        reader.GetOrdinal("NIDNumber")),

                    RoleType = reader.GetString(
                        reader.GetOrdinal("RoleType")),

                    JoiningDate = DateOnly.FromDateTime(
                        reader.GetDateTime(
                            reader.GetOrdinal("JoiningDate"))),

                    DrivingLicenseNumber = GetNullableString(
                        reader, "DrivingLicenseNumber"),

                    LicenseExpiryDate = GetNullableDate(
                        reader, "LicenseExpiryDate"),

                    LicenseType = GetNullableString(
                        reader, "LicenseType"),

                    BaseSalary = reader.GetDecimal(
                        reader.GetOrdinal("BaseSalary")),

                    Status = reader.GetString(
                        reader.GetOrdinal("Status"))
                });
            }

            // Second Result: Total Count
            if (await reader.NextResultAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    response.TotalRecords =
                        reader.GetInt64(
                            reader.GetOrdinal("TotalRecords"));
                }
            }

            response.Items = items;

            return response;
        }

        // ==========================================
        // COMMON PARAMETER HELPER
        // ==========================================

        private static void AddParameter(
            SqlCommand command,
            string name,
            SqlDbType type,
            object? value,
            int? size = null)
        {
            var parameter = size.HasValue
                ? command.Parameters.Add(name, type, size.Value)
                : command.Parameters.Add(name, type);

            parameter.Value = value ?? DBNull.Value;
        }

        private static string? GetNullableString(
            SqlDataReader reader,
            string column)
        {
            var ordinal = reader.GetOrdinal(column);

            return reader.IsDBNull(ordinal)
                ? null
                : reader.GetString(ordinal);
        }

        private static DateOnly? GetNullableDate(
            SqlDataReader reader,
            string column)
        {
            var ordinal = reader.GetOrdinal(column);

            return reader.IsDBNull(ordinal)
                ? null
                : DateOnly.FromDateTime(reader.GetDateTime(ordinal));
        }

        // Remaining methods will be implemented below.

        public async Task<int> GenerateMonthlyPayrollAsync(
            DateOnly payrollMonth,
            CancellationToken cancellationToken = default)
        {
            if (payrollMonth.Day != 1)
                throw new ArgumentException(
                    "Payroll month must start on day 1.");

            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command = new SqlCommand(
                "dbo.sp_GenerateMonthlyPayroll",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            AddParameter(
                command,
                "@PayrollMonth",
                SqlDbType.Date,
                payrollMonth);

            await connection.OpenAsync(cancellationToken);

            var result = await command.ExecuteScalarAsync(cancellationToken);

            return result == null || result == DBNull.Value
                ? 0
                : Convert.ToInt32(result);
        }


        public async Task<MonthlyPayrollResponse> GetMonthlyPayrollAsync(
            DateOnly payrollMonth,
            string? roleType,
            string? paymentStatus,
            string? search,
            CancellationToken cancellationToken = default)
        {
            if (payrollMonth.Day != 1)
                throw new ArgumentException(
                    "Payroll month must start on day 1.");

            var response = new MonthlyPayrollResponse();

            var items = new List<PayrollResponse>();

            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command = new SqlCommand(
                "dbo.sp_GetMonthlyPayroll",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            AddParameter(command, "@PayrollMonth", SqlDbType.Date, payrollMonth);
            AddParameter(command, "@RoleType", SqlDbType.NVarChar, roleType, 30);
            AddParameter(command, "@PaymentStatus", SqlDbType.NVarChar, paymentStatus, 20);
            AddParameter(command, "@Search", SqlDbType.NVarChar, search, 150);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            // Result Set 1: Payroll List
            while (await reader.ReadAsync(cancellationToken))
            {
                items.Add(new PayrollResponse
                {
                    PaymentId = reader.GetInt64(reader.GetOrdinal("PaymentId")),
                    StaffId = reader.GetInt32(reader.GetOrdinal("StaffId")),

                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    RoleType = reader.GetString(reader.GetOrdinal("RoleType")),

                    PayrollMonth = DateOnly.FromDateTime(
                        reader.GetDateTime(reader.GetOrdinal("PayrollMonth"))),

                    BaseSalary = reader.GetDecimal(reader.GetOrdinal("BaseSalary")),
                    BonusAmount = reader.GetDecimal(reader.GetOrdinal("BonusAmount")),
                    DeductionAmount = reader.GetDecimal(reader.GetOrdinal("DeductionAmount")),
                    NetPayable = reader.GetDecimal(reader.GetOrdinal("NetPayable")),

                    AdjustmentRemarks = GetNullableString(reader, "AdjustmentRemarks"),

                    PaymentStatus = reader.GetString(
                        reader.GetOrdinal("PaymentStatus")),

                    PaymentMethod = GetNullableString(reader, "PaymentMethod"),

                    TransactionRef = GetNullableString(reader, "TransactionRef"),

                    PaidAt = GetNullableDateTime(reader, "PaidAt")
                });
            }

            // Result Set 2: Summary
            if (await reader.NextResultAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    response.Summary = new PayrollSummaryResponse
                    {
                        TotalStaff = reader.GetInt64(
                            reader.GetOrdinal("TotalStaff")),

                        PaidCount = reader.GetInt32(
                            reader.GetOrdinal("PaidCount")),

                        UnpaidCount = reader.GetInt32(
                            reader.GetOrdinal("UnpaidCount")),

                        TotalPayable = reader.GetDecimal(
                            reader.GetOrdinal("TotalPayable")),

                        TotalPaid = reader.GetDecimal(
                            reader.GetOrdinal("TotalPaid")),

                        TotalUnpaid = reader.GetDecimal(
                            reader.GetOrdinal("TotalUnpaid"))
                    };
                }
            }

            response.Items = items;

            return response;
        }

        private static DateTime? GetNullableDateTime(
            SqlDataReader reader,
            string column)
        {
            var ordinal = reader.GetOrdinal(column);

            return reader.IsDBNull(ordinal)
                ? null
                : reader.GetDateTime(ordinal);
        }


        public async Task UpdatePayrollAdjustmentAsync(
            long paymentId,
            UpdatePayrollAdjustmentRequest request,
            CancellationToken cancellationToken = default)
                {
                    await using var connection =
                        _connectionFactory.CreateConnection();

                    await using var command = new SqlCommand(
                        "dbo.sp_UpdatePayrollAdjustment",
                        connection);

                    command.CommandType = CommandType.StoredProcedure;

                    AddParameter(command, "@PaymentId", SqlDbType.BigInt, paymentId);

                    var bonus = command.Parameters.Add(
                        "@BonusAmount", SqlDbType.Decimal);

                    bonus.Precision = 18;
                    bonus.Scale = 2;
                    bonus.Value = request.BonusAmount;

                    var deduction = command.Parameters.Add(
                        "@DeductionAmount", SqlDbType.Decimal);

                    deduction.Precision = 18;
                    deduction.Scale = 2;
                    deduction.Value = request.DeductionAmount;

                    AddParameter(
                        command,
                        "@AdjustmentRemarks",
                        SqlDbType.NVarChar,
                        request.AdjustmentRemarks,
                        500);

                    await connection.OpenAsync(cancellationToken);

                    await command.ExecuteNonQueryAsync(cancellationToken);
                }



        public async Task<PayrollResponse> ProcessSalaryPaymentAsync(
            long paymentId,
            ProcessSalaryPaymentRequest request,
            int paidByUserId,
            CancellationToken cancellationToken = default)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command = new SqlCommand(
                "dbo.sp_ProcessStaffSalaryPayment",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            AddParameter(command, "@PaymentId", SqlDbType.BigInt, paymentId);

            AddParameter(
                command,
                "@PaymentMethod",
                SqlDbType.NVarChar,
                request.PaymentMethod,
                30);

            AddParameter(
                command,
                "@TransactionRef",
                SqlDbType.NVarChar,
                request.TransactionRef,
                150);

            AddParameter(
                command,
                "@PaidByUserId",
                SqlDbType.Int,
                paidByUserId);

            AddParameter(
                command,
                "@Remarks",
                SqlDbType.NVarChar,
                request.Remarks,
                500);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
                throw new InvalidOperationException(
                    "Payment result was not returned.");

            return new PayrollResponse
            {
                PaymentId = reader.GetInt64(
                    reader.GetOrdinal("PaymentId")),

                PaymentStatus = reader.GetString(
                    reader.GetOrdinal("PaymentStatus")),

                PaymentMethod = GetNullableString(
                    reader, "PaymentMethod"),

                TransactionRef = GetNullableString(
                    reader, "TransactionRef"),

                PaidAt = GetNullableDateTime(
                    reader, "PaidAt"),

                NetPayable = reader.GetDecimal(
                    reader.GetOrdinal("NetPayable"))
            };
        }




        public async Task<StaffDetailsResponse?> GetStaffByIdAsync(
            int staffId,
            CancellationToken cancellationToken = default)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command = new SqlCommand(
                "dbo.sp_GetStaffById",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            AddParameter(command, "@StaffId", SqlDbType.Int, staffId);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
                return null;

            return new StaffDetailsResponse
            {
                StaffId = reader.GetInt32(reader.GetOrdinal("StaffId")),

                FullName = reader.GetString(reader.GetOrdinal("FullName")),

                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),

                AlternativeContact = GetNullableString(reader, "AlternativeContact"),

                PresentAddress = GetNullableString(reader, "PresentAddress"),

                PermanentAddress = GetNullableString(reader, "PermanentAddress"),

                NIDNumber = reader.GetString(reader.GetOrdinal("NIDNumber")),

                PassportOrBirthCertificate =
                    GetNullableString(reader, "PassportOrBirthCertificate"),

                RoleType = reader.GetString(reader.GetOrdinal("RoleType")),

                JoiningDate = DateOnly.FromDateTime(
                    reader.GetDateTime(reader.GetOrdinal("JoiningDate"))),

                DrivingLicenseNumber =
                    GetNullableString(reader, "DrivingLicenseNumber"),

                LicenseExpiryDate =
                    GetNullableDate(reader, "LicenseExpiryDate"),

                LicenseType = GetNullableString(reader, "LicenseType"),

                BaseSalary = reader.GetDecimal(
                    reader.GetOrdinal("BaseSalary")),

                BankName = GetNullableString(reader, "BankName"),

                BankAccountNumber =
                    GetNullableString(reader, "BankAccountNumber"),

                MobileBankingProvider =
                    GetNullableString(reader, "MobileBankingProvider"),

                MobileBankingNumber =
                    GetNullableString(reader, "MobileBankingNumber"),

                Status = reader.GetString(reader.GetOrdinal("Status")),

                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),

                UpdatedAt = GetNullableDateTime(reader, "UpdatedAt")
            };
        }



        public async Task UpdateStaffAsync(
            int staffId,
            UpdateStaffRequest request,
            CancellationToken cancellationToken = default)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command = new SqlCommand(
                "dbo.sp_UpdateStaff",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            AddParameter(command, "@StaffId", SqlDbType.Int, staffId);

            AddParameter(command, "@FullName", SqlDbType.NVarChar, request.FullName, 150);
            AddParameter(command, "@PhoneNumber", SqlDbType.NVarChar, request.PhoneNumber, 20);

            AddParameter(command, "@AlternativeContact", SqlDbType.NVarChar, request.AlternativeContact, 20);
            AddParameter(command, "@PresentAddress", SqlDbType.NVarChar, request.PresentAddress, 500);
            AddParameter(command, "@PermanentAddress", SqlDbType.NVarChar, request.PermanentAddress, 500);

            AddParameter(command, "@NIDNumber", SqlDbType.NVarChar, request.NIDNumber, 30);

            AddParameter(
                command,
                "@PassportOrBirthCertificate",
                SqlDbType.NVarChar,
                request.PassportOrBirthCertificate,
                50);

            AddParameter(command, "@RoleType", SqlDbType.NVarChar, request.RoleType, 30);

            AddParameter(command, "@JoiningDate", SqlDbType.Date, request.JoiningDate);

            AddParameter(
                command,
                "@DrivingLicenseNumber",
                SqlDbType.NVarChar,
                request.DrivingLicenseNumber,
                50);

            AddParameter(
                command,
                "@LicenseExpiryDate",
                SqlDbType.Date,
                request.LicenseExpiryDate);

            AddParameter(
                command,
                "@LicenseType",
                SqlDbType.NVarChar,
                request.LicenseType,
                20);

            var salary = command.Parameters.Add(
                "@BaseSalary", SqlDbType.Decimal);

            salary.Precision = 18;
            salary.Scale = 2;
            salary.Value = request.BaseSalary;

            AddParameter(command, "@BankName", SqlDbType.NVarChar, request.BankName, 100);

            AddParameter(
                command,
                "@BankAccountNumber",
                SqlDbType.NVarChar,
                request.BankAccountNumber,
                100);

            AddParameter(
                command,
                "@MobileBankingProvider",
                SqlDbType.NVarChar,
                request.MobileBankingProvider,
                50);

            AddParameter(
                command,
                "@MobileBankingNumber",
                SqlDbType.NVarChar,
                request.MobileBankingNumber,
                20);

            await connection.OpenAsync(cancellationToken);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }




        public async Task UpdateStaffStatusAsync(
            int staffId,
            string status,
            CancellationToken cancellationToken = default)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command = new SqlCommand(
                "dbo.sp_UpdateStaffStatus",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            AddParameter(command, "@StaffId", SqlDbType.Int, staffId);

            AddParameter(
                command,
                "@Status",
                SqlDbType.NVarChar,
                status,
                20);

            await connection.OpenAsync(cancellationToken);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }



        public async Task DeactivateStaffAsync(
            int staffId,
            CancellationToken cancellationToken = default)
        {
            await using var connection =
                _connectionFactory.CreateConnection();

            await using var command = new SqlCommand(
                "dbo.sp_DeactivateStaff",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            AddParameter(command, "@StaffId", SqlDbType.Int, staffId);

            await connection.OpenAsync(cancellationToken);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

    }
}
