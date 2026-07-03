using RocksDbSharp;

namespace Telemetry.Ingress.API.Infrastructure.Services;

/// <summary>
/// A class that provides local buffer functionality for storing messages. <br/> Uses RocksDb
/// </summary>
public class LocalBufferService : IDisposable
{
    private readonly RocksDb _db;
    private readonly WriteOptions _writeOptions;

    public LocalBufferService(string dbPath)
    {
        var opt = new DbOptions().SetCreateIfMissing(true);
        _db = RocksDb.Open(opt, dbPath);

        _writeOptions = new WriteOptions().SetSync(false); // async write
    }

    /// <summary>
    /// Appends an entry to a WAL storage and saves that entry
    /// </summary>
    /// <param name="key">Message key</param>
    /// <param name="value">Message value</param>
    public void Put(byte[] key, byte[] value)
    {
        _db.Put(key, value, writeOptions: _writeOptions);
    }

    /// <summary>
    /// Writes all changes to WAL that are present in passed <see cref="WriteBatch"/> instance
    /// </summary>
    /// <param name="writeBatch">Batch instructions</param>
    public void Write(WriteBatch writeBatch)
    {
        _db.Write(writeBatch, writeOptions: _writeOptions);
    }

    /// <summary>
    /// Returns an iterator for moving through the data
    /// </summary>
    /// <returns><see cref="Iterator"/> - iterator instance</returns>
    public Iterator NewIterator()
    {
        return _db.NewIterator();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _db.Dispose();
    }
}
