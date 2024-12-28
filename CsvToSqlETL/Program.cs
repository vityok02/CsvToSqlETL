using CsvToSqlETL;
using CsvToSqlETL.Data.Csv;
using CsvToSqlETL.Data.Database;
using CsvToSqlETL.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        TripService tripService = new(
            new CsvRepository(),
            new DataLoader(AppConfiguration.ConnectionString));

        var trips = tripService.ExtractTrips(AppConfiguration.CsvFilePath);
        Console.WriteLine("Data extracted successfully");

        trips = tripService.Transform(trips);
        Console.WriteLine("Data transformed successfully");

        var watch = System.Diagnostics.Stopwatch.StartNew();

        await tripService.LoadTrips(trips);
        Console.WriteLine("Data successfully loaded into database");

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Console.WriteLine($"Loading data into the database took {elapsedMs} ms");
        Console.WriteLine();
        Console.WriteLine("ECL process completed successfully");
    }
}

/*
 * To effectively work with large CSV files (for example, 10 GB), 
 * you need to radically change the approach to data processing. 
 * First, instead of loading the entire file into memory, 
 * you need to process it in parts. 
 * This allows you to reduce the load on memory. 
 * Second, instead of accumulating all the data in memory before inserting it into the database, 
 * it is better to insert it in parts — for example, every few thousand rows. 
 * You can also process several parts of the file in parallel, 
 * which will also reduce processing time. 
 * Instead of storing all the data, it is better to use more 
 * efficient structures to process unique records without duplication in memory.
 */