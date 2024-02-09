using System.ComponentModel.DataAnnotations.Schema;


namespace WebApplication1.Models
{
    public class Active
    {
        public int Id { get; set; }
        public string? User { get; set; }
        public string? Server { get; set; }
        public string? IPAddress { get; set; }
        public string? MacAddress { get; set; }
        public string? SessionTime { get; set; }
        public long BytesSent { get; set; }
        public long BytesReceived { get; set; }
    }
}
