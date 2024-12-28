using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace CsvToSqlETL;

public class FlagConverter : DefaultTypeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        return text?.Trim() switch
        {
            "N" => "No",
            "Yes" => "Yes",
            _ => string.Empty
        };
    }
}