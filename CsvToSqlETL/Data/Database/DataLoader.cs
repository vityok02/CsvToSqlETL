using CsvToSqlETL.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CsvToSqlETL.Data.Database;

public class DataLoader : IDataLoader
{
    private readonly string _connectionString;
    private const int BatchSize = 10000;
    private readonly TimeZoneInfo _estTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

    public DataLoader(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task LoadData(IEnumerable<Trip> trips)
    {
        var builder = new SqlConnectionStringBuilder(_connectionString);
        var dbName = builder.InitialCatalog;

        builder.InitialCatalog = "master";

        using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        await DeleteDb(dbName, connection);
        await CreateDb(dbName, connection);

        builder.InitialCatalog = dbName;
        using var dbConnection = new SqlConnection(builder.ConnectionString);
        await dbConnection.OpenAsync();

        var tableName = "Trips";
        await CreateTable(dbConnection, tableName);

        using var transaction = dbConnection.BeginTransaction();
        try
        {
            await BulkInsert(trips, dbConnection, transaction, tableName);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        await AddIndexes(dbConnection);
    }

    private async Task BulkInsert(IEnumerable<Trip> trips, SqlConnection dbConnection, SqlTransaction transaction, string tableName)
    {
        var table = CreateDataTable();

        using var bulkCopy = new SqlBulkCopy(dbConnection, SqlBulkCopyOptions.TableLock, transaction)
        {
            DestinationTableName = tableName,
            BatchSize = BatchSize,
            BulkCopyTimeout = 600
        };

        foreach (var batch in trips.Chunk(BatchSize))
        {
            table.Clear();
            AddRows(batch, table);
            await bulkCopy.WriteToServerAsync(table);
        }
    }

    private void AddRows(IEnumerable<Trip> trips, DataTable table)
    {
        foreach (var trip in trips)
        {
            var pickupUtc = TimeZoneInfo.ConvertTimeToUtc(trip.PickupDatetime, _estTimeZone);
            var dropoffUtc = TimeZoneInfo.ConvertTimeToUtc(trip.DropoffDatetime, _estTimeZone);

            table.Rows.Add(
                pickupUtc,
                dropoffUtc,
                trip.PassengerCount,
                trip.TripDistance,
                trip.StoreAndFwdFlag,
                trip.PULocationID,
                trip.DOLocationID,
                trip.FareAmount,
                trip.TipAmount
            );
        }
    }

    private async Task DeleteDb(string dbName, SqlConnection connection)
    {
        await ExecuteNonQuery(connection, $@"
        IF EXISTS (SELECT name FROM sys.databases WHERE name = '{dbName}')
        BEGIN
            ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            DROP DATABASE [{dbName}];
        END");
    }

    private async Task CreateDb(string dbName, SqlConnection connection)
    {
        await ExecuteNonQuery(connection, $@"
            CREATE DATABASE [{dbName}];");
    }

    private async Task CreateTable(SqlConnection dbConnection, string tableName)
    {
        await ExecuteNonQuery(dbConnection, $@"
        CREATE TABLE {tableName} (
            PickupDateTime DATETIME,
            DropoffDateTime DATETIME,
            PassengerCount INT,
            TripDistance FLOAT,
            StoreAndFwdFlag NVARCHAR(10),
            PickupLocationId INT,
            DropoffLocationId INT,
            FareAmount DECIMAL(18,2),
            TipAmount DECIMAL(18,2)
        );");
    }

    private async Task AddIndexes(SqlConnection connection)
    {
        await ExecuteNonQuery(connection, @"
            CREATE INDEX idx_pulocationid_tipamount ON Trips (PickupLocationId, TipAmount);
            CREATE INDEX idx_tripdistance ON Trips (TripDistance DESC);
            CREATE INDEX idx_trip_duration ON Trips (PickupDateTime, DropoffDateTime);
            CREATE INDEX idx_pulocationid ON Trips (PickupLocationId);");
    }

    private DataTable CreateDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("PickupDateTime", typeof(DateTime));
        table.Columns.Add("DropoffDateTime", typeof(DateTime));
        table.Columns.Add("PassengerCount", typeof(int));
        table.Columns.Add("TripDistance", typeof(double));
        table.Columns.Add("StoreAndFwdFlag", typeof(string));
        table.Columns.Add("PickupLocationId", typeof(int));
        table.Columns.Add("DropoffLocationId", typeof(int));
        table.Columns.Add("FareAmount", typeof(decimal));
        table.Columns.Add("TipAmount", typeof(decimal));
        return table;
    }

    private async Task ExecuteNonQuery(SqlConnection connection, string commandText)
    {
        using var command = connection.CreateCommand();
        command.CommandText = commandText;
        await command.ExecuteNonQueryAsync();
    }
}
