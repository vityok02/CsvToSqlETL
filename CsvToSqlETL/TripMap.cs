using CsvHelper.Configuration;

namespace CsvToSqlETL;

public class TripMap : ClassMap<Trip>
{
    public TripMap()
    {
        DateTime date = new(2000, 01, 01, 01, 01, 01, DateTimeKind.Utc);

        Map(m => m.PickupDatetime)
            .Name("tpep_pickup_datetime")
            .Default(new DateTime(2000, 1, 1));

        Map(m => m.DropoffDatetime)
            .Name("tpep_dropoff_datetime")
            .Default(new DateTime(2000, 1, 1));

        Map(m => m.PassengerCount)
            .Name("passenger_count")
            .Default(0);

        Map(m => m.TripDistance)
            .Name("trip_distance")
            .Default(0.0);

        Map(m => m.StoreAndFwdFlag)
            .Name("store_and_fwd_flag")
            .TypeConverter<FlagConverter>()
            .Default(string.Empty);

        Map(m => m.PULocationID)
            .Name("PULocationID")
            .Default(0);

        Map(m => m.DOLocationID)
            .Name("DOLocationID")
            .Default(0);

        Map(m => m.FareAmount)
            .Name("fare_amount")
            .Default(0.0m);

        Map(m => m.TipAmount)
            .Name("tip_amount")
            .Default(0.0m);
    }
}
