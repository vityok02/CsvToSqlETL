using CsvHelper;
using CsvHelper.Configuration;
using CsvToSqlETL.Interfaces;
using System.Globalization;

namespace CsvToSqlETL.Data.Csv;

public class CsvRepository : ICsvRepository<Trip>
{
    public IEnumerable<Trip> ReadAll(string filePath)
    {
        try
        {
            using var reader = new StreamReader(@filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                HeaderValidated = null,
            });
            csv.Context.RegisterClassMap<TripMap>();

            var trips = csv.GetRecords<Trip>().ToArray();

            return trips;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error reading file: {ex.Message}");
        }
    }

    public void WriteAll(IEnumerable<Trip> trips, string filePath)
    {
        try
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(trips);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error writing to CSV file at {filePath}.", ex);
        }
    }
}
