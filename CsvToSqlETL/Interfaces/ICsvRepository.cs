namespace CsvToSqlETL.Interfaces;

public interface ICsvRepository<TEntity>
    where TEntity : class
{
    IEnumerable<TEntity> ReadAll(string filePath);
    void WriteAll(IEnumerable<TEntity> entities, string filePath);
}
