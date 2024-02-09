namespace WebApplication1.Models
{
    public class User
    {
        public string? Name { get; set; }
        public string? Server { get; set; }
        public string? Password { get; set; }
        public string? Profile { get; set; }
        public TimeSpan LimitUptime { get; set; }
        public TimeSpan Uptime { get; set; }
        public long BytesIn { get; set; }
        public long BytesOut { get; set; }
    }
}
