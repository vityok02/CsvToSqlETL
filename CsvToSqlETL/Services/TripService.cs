using CsvToSqlETL.Interfaces;

namespace CsvToSqlETL.Services;

public class TripService
{
    private readonly ICsvRepository<Trip> _csvTripsRepository;
    private readonly IDataLoader _dataLoader;

    public TripService(ICsvRepository<Trip> csvTripsRepository, IDataLoader dataLoader)
    {
        _csvTripsRepository = csvTripsRepository;
        _dataLoader = dataLoader;
    }

    public IEnumerable<Trip> ExtractTrips(string filePath)
    {
        return _csvTripsRepository.ReadAll(filePath);
    }

    public IEnumerable<Trip> Transform(IEnumerable<Trip> trips)
    {
        trips = ProcessDuplicates(trips);

        return trips.Select(t =>
        {
            t.StoreAndFwdFlag = t.StoreAndFwdFlag?.Trim();
            return t;
        })
        .ToArray();
    }

    public async Task LoadTrips(IEnumerable<Trip> trips)
    {
        await _dataLoader.LoadData(trips);
    }

    private IEnumerable<Trip> ProcessDuplicates(IEnumerable<Trip> trips)
    {
        List<Trip> duplicates = [];

        trips = trips
            .GroupBy(x => new
            {
                x.PickupDatetime,
                x.DropoffDatetime,
                x.PassengerCount,
            })
            .Select(g =>
            {
                if (g.Count() > 1)
                {
                    duplicates.AddRange(g.Skip(1));
                }
                return g.First();
            }).ToArray();

        Console.WriteLine($"{duplicates.Count} duplicates were found");

        _csvTripsRepository.WriteAll(duplicates, AppConfiguration.DublicatesFilePath);

        return trips;
    }
}
