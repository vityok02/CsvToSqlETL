use TaxiTrips;
BEGIN
    CREATE TABLE dbo.Trips (
        PickupDateTime DATETIME,
        DropoffDateTime DATETIME,
        PassengerCount INT,
        TripDistance FLOAT,
        StoreAndFwdFlag NVARCHAR(10),
        PickupLocationId INT,
        DropoffLocationId INT,
        FareAmount DECIMAL(18,2),
        TipAmount DECIMAL(18,2)
    );
END