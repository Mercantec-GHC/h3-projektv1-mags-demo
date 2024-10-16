namespace API.Models
{
    public class UserDevice : Common
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public string DeviceId { get; set; }
        public Device Device { get; set; }
    }

    public class UserDeviceDTO
    {
        public string UserID { get; set; }
        public string DeviceId { get; set; }
    }
}
