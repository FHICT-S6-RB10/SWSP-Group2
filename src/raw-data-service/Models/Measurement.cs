using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Raw_Data_Service.Models
{
    [BsonIgnoreExtraElements]
    public class Measurement
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string PatientId { get; set; }

        public string WearableId { get; set; }

        public string Timestamp { get; set; }

        public BsonDocument Data { get; set; }
    }
}
