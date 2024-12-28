# CsvToSqlETL

This project is designed for reading data from CSV files, converting it into the appropriate format, and loading it into an SQL Server database. The program also supports optimization for large datasets, including bulk insert operations.

## How to Run the Program

### 1. Clone the Repository

Clone the repository to your machine using Git:

```bash
git clone https://github.com/yourusername/CsvToSqlETL.git
```

### 2. Change directory
```bash
cd CsvToSqlETL
```

### 3. Set Up the SQL Server Docker container
```bash
docker-compose up -d
```

### 4. Run program
```bash
dotnet run
```

## Sql-scripts
### 1. Create database
```sql
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'TaxiTrips')
BEGIN
    ALTER DATABASE TaxiTrips SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TaxiTrips;
END
CREATE DATABASE TaxiTrips;
```

### 2. Create table
```sql
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
```

### 3. Add indexes
```sql
use TaxiTrips;
CREATE INDEX idx_pulocationid_tipamount ON Trips (PickupLocationId, TipAmount);
CREATE INDEX idx_tripdistance ON Trips (TripDistance DESC);
CREATE INDEX idx_trip_duration ON Trips (PickupDateTime, DropoffDateTime);
CREATE INDEX idx_pulocationid ON Trips (PickupLocationId);
```

## Potential Improvements
To effectively work with large CSV files (for example, 10 GB), you need to radically change the approach to data processing. First, instead of loading the entire file into memory, you need to process it in parts. This allows you to reduce the load on memory. Second, instead of accumulating all the data in memory before inserting it into the database, it is better to insert it in parts — for example, every few thousand rows. You can also process several parts of the file in parallel, which will also reduce processing time. Instead of storing all the data, it is better to use more efficient structures to process unique records without duplication in memory.

## Comments on assumptions
- It is assumed that the data in the CSV file is in the correct format and that there are no significant errors in the data itself, such as invalid or empty values ​​in date fields.
- The date format may vary. You need to handle the following cases.
- Some data may be invalid or missing altogether.
- To work with large amounts of data, you need to implement the possible improvements listed above.
- Possible duplication of data in csv files

## Number of rows after data processing: 
29889
