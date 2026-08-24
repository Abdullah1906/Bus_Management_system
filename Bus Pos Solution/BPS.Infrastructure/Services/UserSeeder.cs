using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using BPS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BPS.Infrastructure.Services
{
    public class UserSeeder : IUserSeeder
    {
        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;

        public UserSeeder(SqlConnectionFactory connectionFactory, IConfiguration configuration,IPasswordHasher passwordHasher)
        {
            _connectionFactory = connectionFactory;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAdminAsync()
        {
            var adminPassword =
                _configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException(
                    "AdminSeed:Password is not configured.");
            }

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync();

            // -----------------------------------------
            // 1. Check admin already exists
            // -----------------------------------------

            User? existingAdmin = null;

            await using (var command = new SqlCommand(
                "SP_User_GetByUsername",
                connection))
            {
                command.CommandType =
                    CommandType.StoredProcedure;

                command.Parameters.Add(
                    "@Username",
                    SqlDbType.NVarChar,
                    100).Value = "admin";

                await using var reader =
                    await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    existingAdmin = new User
                    {
                        Id = reader.GetInt32(
                            reader.GetOrdinal("Id")),

                        Username = reader.GetString(
                            reader.GetOrdinal("Username")),

                        PasswordHash = reader.GetString(
                            reader.GetOrdinal("PasswordHash")),

                        FullName = reader.GetString(
                            reader.GetOrdinal("FullName")),

                        Role = reader.GetString(
                            reader.GetOrdinal("Role")),
                        Email = reader["Email"]?.ToString()
                            ?? string.Empty,

                        PhoneNumber = reader["PhoneNumber"]?.ToString()
                            ?? string.Empty,

                        IsActive = reader.GetBoolean(
                            reader.GetOrdinal("IsActive"))
                    };
                }
            }

            // Admin already exists
            if (existingAdmin != null)
                return;

            // -----------------------------------------
            // 2. Create Admin
            // -----------------------------------------
            var adminEmail = _configuration["AdminSeed:Email"];

            var adminPhoneNumber = _configuration["AdminSeed:PhoneNumber"];

            var admin = new User
            {
                Username = "admin",
                FullName = "System Administrator",
                Role = "Admin",
                Email = adminEmail,
                PhoneNumber = adminPhoneNumber,
                IsActive = true,
                CreatedBy = "System",
                UpdatedBy = ""
            };

            // -----------------------------------------
            // 3. Generate password hash
            // -----------------------------------------

            admin.PasswordHash =_passwordHasher.HashPassword(adminPassword);

            // -----------------------------------------
            // 4. Insert Admin using Stored Procedure
            // -----------------------------------------

            await using var insertCommand =
                new SqlCommand(
                    "SP_User_Create",
                    connection);

            insertCommand.CommandType =
                CommandType.StoredProcedure;

            insertCommand.Parameters.Add(
                "@Username",
                SqlDbType.NVarChar,
                100).Value = admin.Username;

            insertCommand.Parameters.Add(
                "@PasswordHash",
                SqlDbType.NVarChar,
                500).Value = admin.PasswordHash;

            insertCommand.Parameters.Add(
                "@FullName",
                SqlDbType.NVarChar,
                150).Value = admin.FullName;

            insertCommand.Parameters.Add(
                "@Email",
                SqlDbType.NVarChar,
                255).Value = admin.Email;

            insertCommand.Parameters.Add(
                "@PhoneNumber",
                SqlDbType.NVarChar,
                20).Value = admin.PhoneNumber;

            insertCommand.Parameters.Add(
                "@Role",
                SqlDbType.NVarChar,
                50).Value = admin.Role;

            insertCommand.Parameters.Add(
                "@IsActive",
                SqlDbType.Bit).Value = admin.IsActive;

            insertCommand.Parameters.Add(
                "@CreatedBy",
                SqlDbType.NVarChar,
                100).Value = admin.CreatedBy;

            insertCommand.Parameters.Add(
                "@UpdatedBy",
                SqlDbType.NVarChar,
                100).Value = admin.UpdatedBy;

            await insertCommand.ExecuteNonQueryAsync();
        }
    }
}