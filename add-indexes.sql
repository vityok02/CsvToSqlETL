use TaxiTrips;
CREATE INDEX idx_pulocationid_tipamount ON Trips (PickupLocationId, TipAmount);
CREATE INDEX idx_tripdistance ON Trips (TripDistance DESC);
CREATE INDEX idx_trip_duration ON Trips (PickupDateTime, DropoffDateTime);
CREATE INDEX idx_pulocationid ON Trips (PickupLocationId);