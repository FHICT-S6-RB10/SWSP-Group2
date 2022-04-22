namespace Raw_Data_Service.Models{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; } = null;
        public string DatabaseName { get; set; } = null;
        public string MeasurementsCollectionName { get; set; } = null;

    }
}