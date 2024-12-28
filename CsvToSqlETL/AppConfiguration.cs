namespace CsvToSqlETL;

public static class AppConfiguration
{
    public readonly static string CsvFilePath = "sample-cab-data.csv";
    public readonly static string ConnectionString = "Data Source=localhost;Initial Catalog=TaxiTrips;User ID=sa;Password=yourStrong(!)Password;Encrypt=true;TrustServerCertificate=true";
    public readonly static string DublicatesFilePath = Path.Combine("Data", "Csv", "duplicates.csv");
}
