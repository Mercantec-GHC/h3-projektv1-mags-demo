namespace API.Models
{
    public class Device : Common
    {
        public string Name { get; set; }
        public ICollection<UserDevice> UserDevices { get; set; }
        public ICollection<DeviceData> DeviceData { get; set; }
    }
    public class DeviceCreateDTO
    {
        public string Name { get; set; }
    }

}
