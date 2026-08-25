using BPS.Application.DTOs.Reports;
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
    public class ReportRepository : IReportRepository
    {
        private readonly SqlConnectionFactory
            _connectionFactory;


        public ReportRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory =
                connectionFactory;
        }


        public async Task<IEnumerable<ReportDto>> GetAsync(
            ReportFilterDto filter)
        {
            var reports =
                new List<ReportDto>();


            await using var connection =
                _connectionFactory
                    .CreateConnection();


            await using var command =
                new SqlCommand(
                    "SP_Report_Get",
                    connection);


            command.CommandType =
                CommandType.StoredProcedure;


            /* =========================
               FROM DATE
            ========================= */

            command.Parameters.Add(
                "@FromDate",
                SqlDbType.Date)
                .Value =
                    filter.FromDate.HasValue
                        ? filter.FromDate.Value.Date
                        : DBNull.Value;


            /* =========================
               TO DATE
            ========================= */

            command.Parameters.Add(
                "@ToDate",
                SqlDbType.Date)
                .Value =
                    filter.ToDate.HasValue
                        ? filter.ToDate.Value.Date
                        : DBNull.Value;


            /* =========================
               PLACE
            ========================= */

            command.Parameters.Add(
                "@PlaceId",
                SqlDbType.Int)
                .Value =
                    filter.PlaceId.HasValue
                        ? filter.PlaceId.Value
                        : DBNull.Value;


            /* =========================
               PERIOD
            ========================= */

            command.Parameters.Add(
                "@Period",
                SqlDbType.NVarChar,
                20)
                .Value =
                    string.IsNullOrWhiteSpace(
                        filter.Period)
                        ? DBNull.Value
                        : filter.Period;


            await connection.OpenAsync();


            await using var reader =
                await command.ExecuteReaderAsync();


            while (
                await reader.ReadAsync())
            {
                reports.Add(
                    new ReportDto
                    {
                        Id =
                            reader.GetInt64(
                                reader.GetOrdinal(
                                    "Id")),

                        ReportDate =
                            reader.GetDateTime(
                                reader.GetOrdinal(
                                    "ReportDate")),

                        PlaceId =
                            reader.GetInt32(
                                reader.GetOrdinal(
                                    "PlaceId")),

                        PlaceName =
                            reader.GetString(
                                reader.GetOrdinal(
                                    "PlaceName")),

                        Price =
                            reader.GetDecimal(
                                reader.GetOrdinal(
                                    "Price")),

                        TipAmount =
                            reader.GetDecimal(
                                reader.GetOrdinal(
                                    "TipAmount")),

                        Total =
                            reader.GetDecimal(
                                reader.GetOrdinal(
                                    "Total")),

                        TipStatus =
                            reader.GetBoolean(
                                reader.GetOrdinal(
                                    "TipStatus"))
                    });
            }


            return reports;
        }
    }
}
