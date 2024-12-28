namespace CsvToSqlETL.Interfaces;

public interface IDataLoader
{
    Task LoadData(IEnumerable<Trip> trip);
}
