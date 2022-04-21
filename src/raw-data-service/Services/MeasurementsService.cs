using Raw_Data_Service.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Raw_Data_Service.Services;

public class MeasurementsService
{
    private readonly IMongoCollection<Measurement> _measurementsCollection;

    public MeasurementsService(IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _measurementsCollection = mongoDatabase.GetCollection<Measurement>(
            databaseSettings.Value.MeasurementsCollectionName);
    }

    public async Task<List<Measurement>> GetAsync() =>
        await _measurementsCollection.Find(_ => true).ToListAsync();

    public async Task<Measurement?> GetAsync(string id) =>
        await _measurementsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Measurement newMeasurement) =>
        await _measurementsCollection.InsertOneAsync(newMeasurement);

    public async Task UpdateAsync(string id, Measurement updatedMeasurement) =>
        await _measurementsCollection.ReplaceOneAsync(x => x.Id == id, updatedMeasurement);

    public async Task RemoveAsync(string id) =>
        await _measurementsCollection.DeleteOneAsync(x => x.Id == id);
}