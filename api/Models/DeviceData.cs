namespace API.Models
{
    public class DeviceData : Common
    {
        public string DeviceId { get; set; }
        public Device Device { get; set; }
        private float temperature;
        private float humidity;
        private float gasResistor;
        private float volatileOrganicCompounds;
        private float co2;
    }
    public class DeviceDataCreateDTO
    {
        public string DeviceId { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float GasResistor { get; set; }
        public float VolatileOrganicCompounds { get; set; }
        public float CO2 { get; set; }
    }

    public class DeviceDataReadDTO
    {
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float GasResistor { get; set; }
        public float VolatileOrganicCompounds { get; set; }
        public float CO2 { get; set; }
    }
}
