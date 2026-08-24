using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using BPS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BPS.Infrastructure.Repositories;

public class PlaceRepository : IPlaceRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public PlaceRepository(
        SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    // GET ALL
    public async Task<IEnumerable<Place>> GetAllAsync()
    {
        var places = new List<Place>();

        await using var connection =
            _connectionFactory.CreateConnection();

        await using var command =
            new SqlCommand(
                "SP_Place_GetAll",
                connection);

        command.CommandType =
            CommandType.StoredProcedure;

        await connection.OpenAsync();

        await using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            places.Add(MapPlace(reader));
        }

        return places;
    }


    // GET BY ID
    public async Task<Place?> GetByIdAsync(int id)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await using var command =
            new SqlCommand(
                "SP_Place_GetById",
                connection);

        command.CommandType =
            CommandType.StoredProcedure;

        command.Parameters.Add(
            "@Id",
            SqlDbType.Int).Value = id;

        await connection.OpenAsync();

        await using var reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return MapPlace(reader);
    }


    // CREATE
    public async Task<int> CreateAsync(Place place)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await using var command =
            new SqlCommand(
                "SP_Place_Create",
                connection);

        command.CommandType =
            CommandType.StoredProcedure;

        command.Parameters.Add(
            "@PlaceName",
            SqlDbType.NVarChar,
            150).Value = place.PlaceName;

        var priceParameter =
            command.Parameters.Add(
                "@PricePerTrip",
                SqlDbType.Decimal);

        priceParameter.Precision = 18;
        priceParameter.Scale = 2;
        priceParameter.Value = place.PricePerTrip;

        command.Parameters.Add(
            "@IsActive",
            SqlDbType.Bit).Value = place.IsActive;

        command.Parameters.Add(
            "@CreatedBy",
            SqlDbType.NVarChar,
            100).Value =
                (object?)place.CreatedBy ?? DBNull.Value;

        command.Parameters.Add(
            "@CreatedAt",
            SqlDbType.DateTime2).Value =
                place.CreatedAt;


        await connection.OpenAsync();

        return Convert.ToInt32(
            await command.ExecuteScalarAsync());
    }

    // UPDATE
    public async Task<bool> UpdateAsync(Place place)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await using var command =
            new SqlCommand(
                "SP_Place_Update",
                connection);

        command.CommandType =
            CommandType.StoredProcedure;

        command.Parameters.Add(
            "@Id",
            SqlDbType.Int).Value = place.Id;

        command.Parameters.Add(
            "@PlaceName",
            SqlDbType.NVarChar,
            150).Value = place.PlaceName;

        var priceParameter =
            command.Parameters.Add(
                "@PricePerTrip",
                SqlDbType.Decimal);

        priceParameter.Precision = 18;
        priceParameter.Scale = 2;
        priceParameter.Value = place.PricePerTrip;

        command.Parameters.Add(
            "@IsActive",
            SqlDbType.Bit).Value = place.IsActive;

        command.Parameters.Add(
            "@UpdatedBy",
            SqlDbType.NVarChar,
            100).Value =
                (object?)place.UpdatedBy ?? DBNull.Value;


        await connection.OpenAsync();

        var result =
            Convert.ToInt32(
                await command.ExecuteScalarAsync());

        return result > 0;
    }


    // DELETE / DEACTIVATE
    public async Task<bool> DeleteAsync(int id)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await using var command =
            new SqlCommand(
                "SP_Place_Delete",
                connection);

        command.CommandType =
            CommandType.StoredProcedure;

        command.Parameters.Add(
            "@Id",
            SqlDbType.Int).Value = id;

        await connection.OpenAsync();

        var result =
            Convert.ToInt32(
                await command.ExecuteScalarAsync());

        return result > 0;
    }


    // MAPPING
    private static Place MapPlace(
        SqlDataReader reader)
    {
        return new Place
        {
            Id = reader.GetInt32(
                reader.GetOrdinal("Id")),

            PlaceName = reader.GetString(
                reader.GetOrdinal("PlaceName")),

            PricePerTrip = reader.GetDecimal(
                reader.GetOrdinal("PricePerTrip")),

            IsActive = reader.GetBoolean(
                reader.GetOrdinal("IsActive")),

            //CreatedAt = reader.GetDateTime(
            //    reader.GetOrdinal("CreatedAt")),

            //CreatedBy = reader.IsDBNull(
            //    reader.GetOrdinal("CreatedBy"))
            //    ? null
            //    : reader.GetString(
            //        reader.GetOrdinal("CreatedBy")),

            //UpdatedBy = reader.IsDBNull(
            //    reader.GetOrdinal("UpdatedBy"))
            //    ? null
            //    : reader.GetString(
            //        reader.GetOrdinal("UpdatedBy")),

            //UpdatedAt = reader.IsDBNull(
            //    reader.GetOrdinal("UpdatedAt"))
            //    ? null
            //    : reader.GetDateTime(
            //        reader.GetOrdinal("UpdatedAt"))
        };
    }
}