namespace VIOBANK.Domain.Models
{
    public class Settings
    {
        public int SettingId { get; set; }
        public int UserId { get; set; }
        public bool NotificationsEmail { get; set; } = true;
        public bool NotificationsSms { get; set; } = true;
        public string Language { get; set; } = "en";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; }
    }
}
